using System;
using System.Collections;
using System.Collections.Generic;
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

            bool needPurchaseJoin = false;

            List<string> columns = new List<string>();
            string cols;

            // Default, always include ITEM_ID and Name
            columns.Add("item.ITEM_ID");
            columns.Add("item.Name");

            if (Q.getDateCol()) { columns.Add("purchase.Date_Purchased"); }
            if (Q.getPriceCol()) { columns.Add("purchase.Amount_purchase"); }
            if (Q.getPriceCol() || Q.getDateCol()) {  needPurchaseJoin = true; }

            cols = String.Join(", ", columns);


            if (needPurchaseJoin)
            {
                join = "JOIN purchase ON purchase.PURCHASE_ID = item.PurchaseID ";
            }

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
                query = "SELECT " + cols + " FROM item " + join  + "WHERE item.name LIKE '%" + Q.getSingleTerm() + "%'" + stock +  ";";
            }
            return formatQuery(query);
        }

        public string buildPurchaseQuery(ResultItem item)
        {
            string query = "SELECT item.ITEM_ID, item.Name FROM item WHERE item.PurchaseID = " + item.get_PurchaseID() + ";";
            return query;
        }

        enum AttributeType
        {
            Null   = -1,
            String = 0,
            Int    = 1,
            Double = 2,
            Date   = 3
        }

        private bool checkTypeOkay(string attrib, string type)
        {
            switch(type)
            {
                case "date":
                    return true;

                case "double unsigned":
                    try { Double.Parse(attrib); }
                    catch { return false; }
                    return true;

                case "int unsigned":
                    try { Int32.Parse(attrib); }
                    catch { return false; }
                    return true;                

                case "varchar(255)":
                    return true;

                case "varchar(45)":
                    return true;

                case "mediumtext":
                    return true;

                case "longblob":
                    return true;

                default:
                    return false;

            }
        }

        private string formatAttribute(string attrib, string type)
        {

            if (attrib == null) { return null; }
            if (type.CompareTo("date") == 0)
            {
                return "DATE(\"" + attrib + "\")";
            }
            if (type.Contains("varchar") || type.Contains("text") || type.Contains("blob"))
            {
                return "\\\"" + attrib + "\\\"";
            }
            else
            {
                return attrib;
            }
        }


        public string buildUpdateQuery(ResultItem currItem, string controlAttribute, string type, Date updateDate)
        {
            if (!checkTypeOkay(updateDate.toDateString(), type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string query;
            string itemID;

            string updatedText = formatAttribute(updateDate.toDateString(), type);
            if (table.CompareTo("item") == 0)
            {
                itemID = table + ".ITEM_ID";
            }
            else
            {
                itemID = table + ".ItemID_" + table;
            }

            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + " = " + currItem.get_ITEM_ID() + ";";
            return query;
        }

        public string buildUpdateQuery(ResultItem currItem, string controlAttribute, string type, string updateText)
        {
            if (!checkTypeOkay(updateText, type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string attrib = controlAttribute.Split('.')[1];
            string query;
            string itemID;

            string updatedText = formatAttribute(updateText, type);

            if (table.CompareTo("item") == 0)
            {
                itemID = table + ".ITEM_ID";
            }
            else
            {
                itemID = table + ".ItemID_" + table;
            }

            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + " = " + currItem.get_ITEM_ID() + ";";
            return query;
        }
    }
}