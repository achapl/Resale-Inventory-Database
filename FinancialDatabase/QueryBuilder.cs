using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Date = Util.Date;

namespace FinancialDatabase
{
	public static class QueryBuilder
	{
        public const string DEFAULTQUERY = "SELECT item.ITEM_ID, item.Name, thumbnail.thumbnail FROM item LEFT JOIN thumbnail ON thumbnail.ThumbnailID = item.ThumbnailID;";

        public static string defaultQuery()
        {
            return placeEscChars(DEFAULTQUERY);
        }


        private static string placeEscChars(string query)
        {
            string escapeChar = "^";
            // Add '^' before special characters ('*', '<', '>', .etc), 

            int maxQueryLen = 1024;
            StringBuilder sb = new StringBuilder(query, maxQueryLen);
            int count = 0;
            // Note: for edge case, special char at start of string, copy switch
            // inside for loop, and modify it outside for case i=0
            for (int i = 0; i < query.Length; i++)
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

        public static string searchByNameQuery(SearchQuery Q)
        {
            string query;
            string stock = "";
            string purchaseJoin = "";
            string thumbnailJoin = "";


            List<string> columns = new List<string>();
            string cols;

            // Default, always include ITEM_ID and Name
            columns.Add("item.ITEM_ID");
            columns.Add("item.Name");
            columns.Add("thumbnail.thumbnail");

            if (Q.getDateCol()) { columns.Add("purchase.Date_Purchased"); }
            if (Q.getPriceCol()) { columns.Add("purchase.Amount_purchase"); }
            cols = String.Join(", ", columns);

            string dateRange = "AND purchase.Date_Purchased > '" + Q.getStartDate() + "' AND purchase.Date_Purchased < '" + Q.getEndDate() + "'";

            purchaseJoin = "JOIN purchase ON purchase.PURCHASE_ID = item.PurchaseID ";

            thumbnailJoin = "LEFT JOIN thumbnail ON thumbnail.ThumbnailID = item.ThumbnailID ";

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
                query = defaultQuery(); 
            }
            else
            {
                query = "SELECT " + cols + " FROM item " + purchaseJoin + thumbnailJoin + "WHERE item.name LIKE '%" + Q.getSingleTerm() + "%'" + stock + " " + dateRange + ";";
            }
            return query;
        }

        public static string purchaseQuery(int purcID)
        {
            string query = "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM item WHERE item.PurchaseID = " + purcID + ") subItem LEFT JOIN purchase ON purchase.PURCHASE_ID = subItem.PurchaseID) subPurchase) subSale LEFT JOIN sale ON sale.SALE_ID = subSale.SaleID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";
            return query;
        }

        public static string purchaseQuery(ResultItem item)
        {
            return purchaseQuery(item.get_PurchaseID());
        }

        public static string saleQuery(ResultItem item)
        {
            return "SELECT * FROM sale WHERE ItemID_sale = " + item.get_ITEM_ID().ToString() + ";";
        }

        public static string insertPurchaseQuery(int purcPrice, string purcNotes, Date d)
        {
            return "INSERT INTO purchase (Amount_purchase, Notes_purchase, Date_Purchased) Values (" + purcPrice.ToString() + ", \"" + purcNotes + "\", " + formatAttribute(d.toDateString(), "date") + ");";
        }
        
        
        // Correct the formatting for sending different datatypes in a string query 
        private static string formatAttribute(string attrib, string type)
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

        public static string shipInfoInsertQuery(ResultItem item) {
            List<int> weight = Util.ozToOzLbs(item.get_Weight());
            return shipInfoInsertQuery(item, weight[0], weight[1], item.get_Length(), item.get_Width(), item.get_Height());
        }

        public static string shipInfoInsertQuery(ResultItem currItem, int weightLbs, int weightOz, int l, int w, int h)
        {
            if (l <= 0 ||  w <= 0 || h <= 0 || weightOz < 0 || weightLbs < 0 || (weightOz == 0 && weightLbs == 0))
            {
                return "ERROR: Incorrect Shipping Info Given";
            }

            int ttlWeight = weightLbs * 16 + weightOz;
            return "INSERT INTO shipping (Length, Width, Height, Weight, ItemID_shipping) VALUES (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + currItem.get_ITEM_ID() + ")";
        }

        public static string deleteShipInfoQuery(ResultItem item)
        {
            if (item.get_ShippingID() == Util.DEFAULT_INT)
            {
                return "ERROR: NO shipping info to delete. QueryBuilder.BuildDelShipInfoQuery()";
            }

            int shipID = item.get_ShippingID();

            return "DELETE FROM shipping WHERE SHIPPING_ID = " + shipID + ";";
        }

        public static string deleteSaleQuery(Sale sale)
        {
            return "DELETE FROM sale WHERE SALE_ID = " + sale.get_SALE_ID().ToString() + ";";
        }

        public static string updateQuery(ResultItem currItem, string controlAttribute, string type, Date updateDate)
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

        public static string updateQuery(ResultItem currItem, string controlAttribute, string type, string updateText)
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
    
        public static string updateQuery(Sale sale, string controlAttribute, string type, string updateText)
        {

            if (sale is null)
            {
                throw new Exception("Sale was null, QueryBuilder.updateQuery()");
            }

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

        public static string updateQuery(Sale sale, string controlAttribute, string type, Date updateDate)
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

        public static string itemInsertQuery(ResultItem item)
        {
            return "INSERT INTO item (Name, InitialQuantity, CurrentQuantity, PurchaseID) VALUES (" + "\"" + item.get_Name() + "\"" + ", " + item.get_InitialQuantity() + ", " + item.get_CurrentQuantity() + ", " + item.get_PurchaseID() + ");";
        }

        public static string saleInsertQuery(Sale sale)
        {
            return "INSERT INTO sale (Date_Sold, Amount_sale, ItemID_sale) VALUES (" + formatAttribute(sale.get_Date_Sold().toDateString(), "date") + ", " + sale.get_Amount_sale().ToString() + ", " + sale.get_ItemID_sale().ToString() + ");";
        }

        public static string deletePurchaseQuery(ResultItem item)
        {
            int purcID = item.get_PurchaseID();
            return "DELETE FROM purchase WHERE purchase.PURCHASE_ID = " + purcID + ";";
        }

        public static string deleteAllSalesQuery(ResultItem item)
        {
            return "DELETE FROM sale WHERE sale.Item_ID_sale = " + item.get_ITEM_ID() + ";";
        }

        public static string deleteItemQuery(ResultItem item)
        {
            return "DELETE FROM item WHERE ITEM_ID = " + item.get_ITEM_ID() + ";";
        }

        public static string thumbnailQuery(List<ResultItem> parsedItems)
        {
            if (parsedItems == null || parsedItems.Count == 0) { throw new Exception("ERROR: QuyeryBuilder.thumbnailQuery(): Null or Empty list passed into it"); }

            string query = "SELECT image.ItemID, image.IMAGE_ID, thumbnail.thumbnail FROM image JOIN thumbnail ON image.thumbnailID = thumbnail.ThumbnailID WHERE image.ItemID IN (";
            query += parsedItems[0].get_ITEM_ID().ToString();

            if (parsedItems.Count > 1)
            {
                foreach (ResultItem item in parsedItems[1..])
                {
                    query += ", " + item.get_ITEM_ID().ToString();
                }
            }

            query += ") ORDER BY image.ItemID;";
            return query;

        }

        public static string completeItemIDSearchQuery(int itemID)
        {
            return "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM item WHERE ITEM_ID = "
                + itemID.ToString()
                + ") subItem LEFT JOIN purchase ON purchase.PURCHASE_ID = subItem.PurchaseID) subPurchase) subSale LEFT JOIN sale ON sale.SALE_ID = subSale.SaleID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";
        }

        public static string getColumnNames(string tableName)
        {
            return "SHOW COLUMNS FROM " + tableName + ";";
        }

        public static string getSaleByID(int saleID)
        {
            return "SELECT * FROM sale WHERE SALE_ID = " + saleID.ToString() + ";";
        }

        public static string purchaseQueryByItemID(int itemID)
        {
            return "SELECT * FROM purchase JOIN item ON item.PurchaseID = purchase.PURCHASE_ID WHERE item.ITEM_ID = " + itemID + ";";
        }
    }
}