using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Date = Util.Date;

namespace FinancialDatabase
{
	public class QueryBuilder
	{
        private const string DEFAULTQUERY = "SELECT * FROM item;";

        public string defaultQuery()
        {
            return formatQuery(DEFAULTQUERY);
        }

        // Insert escape characters
        private string formatQuery(string query)
        {
            string escapeChar = "^";
            // Add '^' before special characters ('*', '<', '>', .etc), 


            StringBuilder sb = new StringBuilder(query, 1024);
            int count = 0;
            // Note: for edge case, special char at start of string, copy swithc inside for loop, and modify it outside for case i=0
            for (int i = 1; i < query.Length; i++)
            {
                switch (query[i])
                {
                    case '*':
                    case '<':
                    case '>':
                    case '&':
                    case '"':
                        sb.Insert(i + count, escapeChar);
                        count++;
                        break;
                }
            }

            return sb.ToString();
        }

        public string buildSearchByNameQuery(SearchQuery Q)
        {
            string query;
            string stock = "";
            string join = "";


            List<string> columns = new List<string>();
            string cols;

            // Default, always include ITEM_ID and Name
            columns.Add("item.ITEM_ID");
            columns.Add("item.Name");

            if (Q.getDateCol()) { columns.Add("purchase.Date_Purchased"); }
            if (Q.getPriceCol()) { columns.Add("purchase.Amount_purchase"); }
            cols = String.Join(", ", columns);

            string dateRange = "AND purchase.Date_Purchased > '" + Q.getStartDate() + "' AND purchase.Date_Purchased < '" + Q.getEndDate() + "'";

            join = "JOIN purchase ON purchase.PURCHASE_ID = item.PurchaseID ";

            // If both are true, don't need to add to query since it's looking for all cases of the current quantity
            if (Q.getInStock() && !Q.getSoldOut())
            {
                stock = " AND item.CurrentQuantity > 0 ";
            } else if (!Q.getInStock() && Q.getSoldOut())
            {
                stock = " AND item.CurrentQuantity = 0 ";
            }
            else if (Q.getInStock() && Q.getSoldOut())
            {
                stock = "";

            }
            // Special case of stupid user looking for no items. Make query fail so no items show up
            else if (!Q.getInStock() && !Q.getSoldOut())
            {
                stock = " AND item.CurrentQuantity < 0 ";
            }

            // NOTE: '^' is a special defined escape character removed in the python script so CMD doesn't assume '>' means pipe command
            if (Q.getSingleTerm().CompareTo("") == 0)
            {
                query = "SELECT * FROM item;";
            }
            else
            {
                //query = "SELECT " + cols + " FROM item JOIN purchase ON item.ITEM_ID = purchase.ItemID WHERE name LIKE '%" + term + "%' AND purchase.Date_Purchased > '" + startDate + "' AND purchase.Date_Purchased < '" + endDate + "' " + stock + ";";
                query = "SELECT " + cols + " FROM item " + join  + "WHERE item.name LIKE '%" + Q.getSingleTerm() + "%'" + stock + " " + dateRange + ";";
            }
            return query;
        }

        public string buildPurchaseQuery(ResultItem item)
        {
            string query = "SELECT item.ITEM_ID, item.Name FROM item WHERE item.PurchaseID = " + item.get_PurchaseID() + ";";
            return query;
        }

        public string buildSaleQuery(ResultItem item)
        {
            return "SELECT * FROM sale WHERE ItemID_sale = " + item.get_ITEM_ID().ToString() + ";";
        }

        public string buildInsertPurchaseQuery(int purcPrice, string purcNotes, Date d)
        {
            return "INSRET INTO purchase (Amount_purchase, Notes_purchase, Date_Purchased) Values (" + purcPrice.ToString() + ", " + purcNotes + ", " + formatAttribute(d.toDateString(), "date") + ");";
        }

        enum AttributeType
        {
            Null   = -1,
            String = 0,
            Int    = 1,
            Double = 2,
            Date   = 3
        }

        

        private string formatAttribute(string attrib, string type)
        {

            if (attrib == null) { return "NULL"; }

            attrib = attrib.Replace("\"", "\\\"");

            if (type.CompareTo("date") == 0)
            {
                return "DATE(\"" + attrib + "\")";
            }
            if (type.Contains("varchar") || type.Contains("text") || type.Contains("blob"))
            {
                return "\"" + attrib + "\"";
            }
            else
            {
                return attrib;
            }
        }

        public string buildShipInfoInsertQuery(ResultItem item) {
            List<int> weight = Util.ozToOzLbs(item.get_Weight());
            return buildShipInfoInsertQuery(item, weight[0], weight[1], item.get_Length(), item.get_Width(), item.get_Height());
        }

        public string buildShipInfoInsertQuery(ResultItem currItem, int weightLbs, int weightOz, int l, int w, int h)
        {
            if (l <= 0 ||  w <= 0 || h <= 0 || weightOz < 0 || weightLbs < 0 || (weightOz == 0 && weightLbs == 0))
            {
                return "ERROR: Incorrect Shipping Info Given";
            }

            int ttlWeight = weightLbs * 16 + weightOz;
            return "INSERT INTO shipping (Length, Width, Height, Weight, ItemID_shipping) VALUES (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + currItem.get_ITEM_ID() + ")";
        }

        public string buildDelShipInfoQuery(ResultItem item)
        {
            if (item.get_ShippingID() == ResultItem.DEFAULT_INT)
            {
                return "ERROR: NO shipping info to delete. QueryBuilder.BuildDelShipInfoQuery()";
            }

            int shipID = item.get_ShippingID();

            return "DELETE FROM shipping WHERE SHIPPING_ID = " + shipID + ";";
        }

        public string buildDelSaleQuery(Sale sale)
        {
            return "DELETE FROM sale WHERE SALE_ID = " + sale.get_SALE_ID().ToString() + ";";
        }

        public string buildUpdateQuery(ResultItem currItem, string controlAttribute, string type, Date updateDate)
        {
            if (!Util.checkTypeOkay(updateDate.toDateString(), type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string query;
            string itemID = "";

            string updatedText = formatAttribute(updateDate.toDateString(), type);
            switch (table)
            {
                case "item":
                    itemID = table + ".ITEM_ID = " + currItem.get_ITEM_ID();
                    break;
                case "shipping":
                    itemID = table + ".SHIPPING_ID = " + currItem.get_ShippingID();
                    break;
                case "purchase":
                    itemID = table + ".PURCHASE_ID = " + currItem.get_PurchaseID();
                    break;
                case "sale":
                    itemID = table + "._ID = " + currItem.get_SaleID();
                    break;


            }
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }

        public string buildUpdateQuery(ResultItem currItem, string controlAttribute, string type, string updateText)
        {

            if (!Util.checkTypeOkay(updateText, type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string attrib = controlAttribute.Split('.')[1];
            string query;
            string itemID = "";

            
            string updatedText = formatAttribute(updateText, type);

            switch(table)
            {
                case "item":
                    itemID = table + ".ITEM_ID = " + currItem.get_ITEM_ID();
                    break;
                case "shipping":
                    itemID = table + ".SHIPPING_ID = " + currItem.get_ShippingID();
                    break;
                case "purchase":
                    itemID = table + ".PURCHASE_ID = " + currItem.get_PurchaseID();
                    break;
                case "sale":
                    itemID = table + "._ID = " + currItem.get_SaleID();
                    break;


            }
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }
    
        public string buildUpdateQuery(Sale sale, string controlAttribute, string type, string updateText)
        {

            if (!Util.checkTypeOkay(updateText, type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string attrib = controlAttribute.Split('.')[1];
            string query;
            string itemID = sale.get_SALE_ID().ToString();

            
            string updatedText = formatAttribute(updateText, type);

            if (table.CompareTo("sale") != 0) {
                throw new Exception("Trying to update SALE item, but not updating Sale table");
            }
                    
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }

        public string buildUpdateQuery(Sale sale, string controlAttribute, string type, Date updateDate)
        {
            if (!Util.checkTypeOkay(updateDate.toDateString(), type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string query;
            string itemID = sale.get_SALE_ID().ToString();


            string updatedText = formatAttribute(updateDate.toDateString(), type);

            if (table.CompareTo("sale") != 0) {
                throw new Exception("Trying to update SALE item, but not updating Sale table");
            }
                    
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }

        public string buildItemInsertQuery(ResultItem item)
        {
            return "INSERT INTO item (Name, InitialQuantity, CurrentQuantity, PurchaseID) VALUES (" + "\"" + item.get_Name() + "\"" + ", " + item.get_InitialQuantity() + ", " + item.get_CurrentQuantity() + ", " + item.get_PurchaseID() + ");";
        }

        public string buildSaleInsertQuery(Sale sale)
        {
            return "INSERT INTO sale (Date_Sold, Amount_sale, ItemID_sale) VALUES (" + formatAttribute(sale.get_Date_Sold().toDateString(), "date") + ", " + sale.get_Amount_sale().ToString() + ", " + sale.get_ItemID_sale().ToString() + ");";
        }
    
    }
}