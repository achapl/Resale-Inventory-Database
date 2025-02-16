using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Date = Util.Date;

namespace FinancialDatabase.DatabaseObjects
{
    public static class QueryBuilder
    {
        public const string DEFAULTQUERY = "SELECT item.ITEM_ID, item.Name, thumbnail.thumbnail FROM item LEFT JOIN thumbnail ON thumbnail.ThumbnailID = item.ThumbnailID;";

        static Dictionary<string, string> colDataTypesLocal = null;

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
            cols = string.Join(", ", columns);

            string dateRange = "AND purchase.Date_Purchased > '" + Q.getStartDate() + "' AND purchase.Date_Purchased < '" + Q.getEndDate() + "'";

            purchaseJoin = "JOIN purchase ON purchase.PURCHASE_ID = item.PurchaseID ";

            thumbnailJoin = "LEFT JOIN thumbnail ON thumbnail.ThumbnailID = item.ThumbnailID ";

            // If both are true, don't need to add to query since it's looking for all cases of the current quantity
            if (Q.getInStock() && !Q.getSoldOut())
            {
                stock = " AND item.CurrentQuantity > 0 ";
            }
            else if (!Q.getInStock() && Q.getSoldOut())
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

            query = "SELECT " + cols + " FROM item " + purchaseJoin + thumbnailJoin + "WHERE item.name LIKE '%" + Q.getSingleTerm() + "%'" + stock + " " + dateRange + ";";

            return query;
        }

        public static string purchaseQuery(int purcID)
        {
            string query = "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM purchase WHERE PURCHASE_ID = " + purcID + ") subPurchase LEFT JOIN item ON item.PurchaseID = subPurchase.PURCHASE_ID) subPurchase) subSale LEFT JOIN sale ON sale.SALE_ID = subSale.SaleID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";
            // string query = "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM item WHERE item.PurchaseID = " + purcID + ") subItem LEFT JOIN purchase ON purchase.PURCHASE_ID = subItem.PurchaseID) subPurchase) subSale LEFT JOIN sale ON sale.SALE_ID = subSale.SaleID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";
            return query;
        }

        public static string purchaseQuery(Item item)
        {
            return purchaseQuery(item.get_PurchaseID());
        }

        public static string saleQuery(Item item)
        {
            return "SELECT * FROM sale WHERE ItemID_sale = " + item.get_ITEM_ID().ToString() + ";";
        }

        public static string insertPurchaseQuery(double purcPrice, string purcNotes, Date d)
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

        public static string shipInfoInsertQuery(Item item)
        {
            List<int> weight = Util.ozToOzLbs(item.get_Weight());
            return shipInfoInsertQuery(item, weight[0], weight[1], item.get_Length(), item.get_Width(), item.get_Height());
        }

        public static string shipInfoInsertQuery(Item currItem, int weightLbs, int weightOz, int l, int w, int h)
        {
            if (l <= 0 || w <= 0 || h <= 0 || weightOz < 0 || weightLbs < 0 || weightOz == 0 && weightLbs == 0)
            {
                throw new Exception("ERROR: Incorrect Shipping Info Given. Possible value <=0");
            }

            int ttlWeight = weightLbs * 16 + weightOz;
            return "INSERT INTO shipping (Length, Width, Height, Weight, ItemID_shipping) VALUES (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + currItem.get_ITEM_ID() + ")";
        }

        public static string deleteShipInfoQuery(Item item)
        {

            int shipID = item.get_ShippingID();

            return "DELETE FROM shipping WHERE SHIPPING_ID = " + shipID + ";";
        }

        public static string deleteSaleQuery(Sale sale)
        {
            return "DELETE FROM sale WHERE SALE_ID = " + sale.get_SALE_ID().ToString() + ";";
        }

        public static string updateQuery(Item currItem, string controlAttribute, Date updateDate)
        {
            string type = colDataTypesLocal[controlAttribute];
            if (!Util.checkTypeOkay(updateDate.toDateString(), type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string query;
            string itemID = "";

            string updatedText = formatAttribute(updateDate.toDateString(), "date");
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


            }
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }

        public static string updateQuery(Item currItem, string controlAttribute, string updateText)
        {
            string type = colDataTypesLocal[controlAttribute];
            if (!Util.checkTypeOkay(updateText, type)) { throw new Exception("ERROR: BAD USER INPUT"); }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string attrib = controlAttribute.Split('.')[1];
            string query;
            string itemID = "";


            string updatedText = formatAttribute(updateText, type);

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

            }
            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE " + itemID + ";";
            return query;
        }

        public static string updateQuery(Sale sale, string controlAttribute, string updateText)
        {
            string type = colDataTypesLocal[controlAttribute];

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

            if (table.CompareTo("sale") != 0)
            {
                throw new Exception("Trying to update SALE item, but not updating Sale table");
            }

            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE sale.SALE_ID = " + itemID + ";";
            return query;
        }

        public static string updateQuery(Sale sale, string controlAttribute, Date updateDate)
        {
            string type = colDataTypesLocal[controlAttribute];

            if (!Util.checkTypeOkay(updateDate.toDateString(), type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string query;
            string itemID = sale.get_SALE_ID().ToString();


            string updatedText = formatAttribute(updateDate.toDateString(), type);

            if (table.CompareTo("sale") != 0)
            {
                throw new Exception("Trying to update SALE item, but not updating Sale table");
            }

            // Note: Since, for example, item : purchase is a many to 1 relationship (buying a lot),
            // one must update the purchase price with the purchaseID, not itemID of the current item
            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE sale.SALE_ID = " + itemID + ";";
            return query;
        }

        public static string updateQuery(MyImage image, string controlAttribute, string updateText)
        {
            string type = colDataTypesLocal[controlAttribute];

            if (image is null)
            {
                throw new Exception("image was null, QueryBuilder.updateQuery()");
            }

            if (!Util.checkTypeOkay(updateText, type)) { return "ERROR: BAD USER INPUT"; }

            if (controlAttribute.Split('.').Length != 2)
            {
                Console.WriteLine("ERROR: controlAttribute does not have exactly 2 fields when splitting on '.': " + controlAttribute);
            }
            string table = controlAttribute.Split('.')[0];
            string attrib = controlAttribute.Split('.')[1];
            string query;
            string imageID = image.imageID.ToString();

            string updatedText = formatAttribute(updateText, type);

            if (table.CompareTo("image") != 0)
            {
                throw new Exception("Trying to update image item, but not updating image table");
            }

            query = "UPDATE " + table + " SET " + controlAttribute + " = " + updatedText + " WHERE image.IMAGE_ID = " + imageID + ";";
            return query;
        }

        public static string itemInsertQuery(Item item)
        {
            return "INSERT INTO item (Name, InitialQuantity, CurrentQuantity, PurchaseID) VALUES (" + "\"" + item.get_Name() + "\"" + ", " + item.get_InitialQuantity() + ", " + item.get_CurrentQuantity() + ", " + item.get_PurchaseID() + ");";
        }

        public static string saleInsertQuery(Sale sale)
        {
            return "INSERT INTO sale (Date_Sold, Amount_sale, ItemID_sale) VALUES (" + formatAttribute(sale.get_Date_Sold().toDateString(), "date") + ", " + sale.get_Amount_sale().ToString() + ", " + sale.get_ItemID_sale().ToString() + ");";
        }

        public static string deletePurchaseQuery(Item item)
        {
            int purcID = item.get_PurchaseID();
            return "DELETE FROM purchase WHERE purchase.PURCHASE_ID = " + purcID + ";";
        }

        public static string deleteAllSalesQuery(Item item)
        {
            return "DELETE FROM sale WHERE sale.ItemID_sale = " + item.get_ITEM_ID() + ";";
        }

        public static string deleteItemQuery(Item item)
        {
            return "DELETE FROM item WHERE ITEM_ID = " + item.get_ITEM_ID() + ";";
        }

        public static string thumbnailQuery(List<Item> parsedItems)
        {
            if (parsedItems == null || parsedItems.Count == 0) { throw new Exception("ERROR: QueryBuilder.thumbnailQuery(): Null or Empty list passed into it"); }

            string query = "SELECT item.ITEM_ID, thumbnail.ThumbnailID, thumbnail.thumbnail FROM item JOIN thumbnail on item.ThumbnailID = thumbnail.ThumbnailID WHERE item.ITEM_ID IN (";
            query += parsedItems[0].get_ITEM_ID().ToString();

            if (parsedItems.Count > 1)
            {
                foreach (Item item in parsedItems[1..])
                {
                    query += ", " + item.get_ITEM_ID().ToString();
                }
            }

            query += ");";//ORDER BY image.ItemID;";
            return query;

        }

        public static string completeItemIDSearchQuery(int itemID)
        {
            return "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM item WHERE ITEM_ID = "
                + itemID.ToString()
                + ") subItem LEFT JOIN purchase ON purchase.PURCHASE_ID = subItem.PurchaseID) subPurchase) subSale LEFT JOIN sale ON sale.ItemID_sale = subSale.ITEM_ID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";
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

        internal static void setColDataTypesLocal(Dictionary<string, string> colDataTypes)
        {
            colDataTypesLocal = colDataTypes;
        }

        internal static string deleteImagesQuery(Item item)
        {
            return "DELETE FROM image WHERE ItemID =" + item.get_ITEM_ID() + ";";
        }

        internal static string setThumbnail(int itemID, int? newThumbnailID)
        {
            if (newThumbnailID.HasValue)
                return "UPDATE item SET ThumbnailID = " + newThumbnailID + " WHERE item.ITEM_ID = " + itemID + ";";
            else
                return "UPDATE item SET ThumbnailID = NULL WHERE item.ITEM_ID = " + itemID + ";";
        }

        internal static string getImages(Item item)
        {
            return "SELECT * FROM image WHERE ItemID = " + item.get_ITEM_ID() + ";";
        }

        internal static string deleteThumbnailQuery(int thumbnailID)
        {
            return "DELETE FROM thumbnail WHERE ThumbnailID = " + thumbnailID + ";";
        }

        internal static string deleteImageQuery(int imageID)
        {
            return "DELETE FROM image WHERE IMAGE_ID = " + imageID + ";";
        }

        internal static string getImage(int imageID)
        {
            return "SELECT * FROM image WHERE IMAGE_ID = " + imageID + ";";
        }

        internal static string getItemByThumbnail(int imageID)
        {
            return "SELECT * FROM item WHERE ThumbnailID = " + imageID + ";";
        }

        internal static string insertImageQuery(string filePath, int itemID)
        {
            return "INSERT INTO image (image, ItemID) VALUES (LOAD_FILE('" + filePath + "'), " + itemID + ");";
        }
    }
}