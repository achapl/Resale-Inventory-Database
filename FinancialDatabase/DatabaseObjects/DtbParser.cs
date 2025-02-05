using FinancialDatabase.DatabaseObjects;
using System;
using System.CodeDom.Compiler;

// Takes raw string outputs of querying the database
// and parses them into usable objects
public static class DtbParser
{
    // raw result from a search query is given from the result of the query in the format
    //
    //                    "[(itemName, itemID, .etc)(item2Name, item2ID, .etc)]"
    //
    // Seperates whole string into list of multiple item strings,
    //
    //       List<string>{ "(itemName, itemID, .etc)", "(item2Name, item2ID, .etc)" }
    public static List<string> parseItemsIntoAttribs(string rawResult)
    {
        if (rawResult.CompareTo("[]") == 0)
        {
            return new List<string>();
        }

        // Note format changes to square brackets if any attribute **contains parenthesis**
        // Example: [[itemName, Date(1/1/11), .etc], [item2Name, Date(1/1/11), .etc]]
        rawResult = Util.myTrim(rawResult, new string[] { "[", "]" });

        List<string> rawItems = new List<string>();

        // Split the rawResult into rawThumbnails based on whether mysql sends each returned row encapsulated in '[]' or '()'
        // Which is based on whether there exist parenthesises inside of an attribute or not (see above)
        if (rawResult.Length > 0 && rawResult[0] == '[')
        {
            rawItems = Util.pairedCharTopLevelSplit(rawResult, '[');
        }
        else if (rawResult.Length > 0 && rawResult[0] == '(')
        {
            rawItems = Util.pairedCharTopLevelSplit(rawResult, '(');
        }
        else
        {
            throw new Exception("Error: Unexpected character for when parsing individual search results: " + rawResult[0]);
        }
        return rawItems;
    }

    // raw result from a search query is given from the result of the query in the format "[(itemName, itemID, .etc)(item2Name, item2ID, .etc)]"
    // Seperates them all and creates Item objects from each of them. Returns these as a list
    public static List<Item> parseItems(string rawResult, List<string> colNames)
    {

        List<string> rawItems = parseItemsIntoAttribs(rawResult);

        List<Item> results = new List<Item>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a Item with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            // If no ITEM_ID, there cannot be an object there.
            if (itemAttributes[colNames.IndexOf("ITEM_ID")] != "None" &&
                getItemIndex(results, Int32.Parse(itemAttributes[colNames.IndexOf("ITEM_ID")])) == -1)
            {
                results.Add(new Item(itemAttributes, colNames));
            }


            if (colNames.IndexOf("SALE_ID") != -1 &&
                itemAttributes[colNames.IndexOf("SALE_ID")] != "None")
            {
                int saleID = Int32.Parse(itemAttributes[colNames.IndexOf("ItemID_sale")]);
                results[getItemIndex(results, saleID)].sales.Add(new Sale(itemAttributes, colNames));
            }
        }

        return results;
    }

    private static int getItemIndex(List<Item> items, int itemID)
    {
        for (int i = 0; i < items.Count(); i++)
        {
            if (items[i].get_ITEM_ID().Equals(itemID))
            {
                return i;
            }
        }
        return -1;
    }

    // raw result from a sale query is given from the result of the query in the format "[(salePrice, itemID, .etc)(salePrice2, item2ID, .etc)]"
    // Seperates them all and creates Sale objects from each of them. Returns these as a list
    public static List<Sale> parseRawSales(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseItemsIntoAttribs(rawResult);

        List<Sale> results = new List<Sale>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a Item with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new Sale(itemAttributes, colNames));
        }

        return results;
    }

    private static bool hasAttrib(int attribIndex)
    {
        return attribIndex != -1;
    }

    private static void setAttrib(out Util.Date date, int index, List<string> purcAttributes)
    {
        if (hasAttrib(index)) {
            date = new Util.Date(purcAttributes[index]);
        }
        else
        {
            date = Util.DEFAULT_DATE;
        }
    }

    private static void setAttrib(out int date, int index, List<string> purcAttributes)
    {
        if (hasAttrib(index))
        {
            date = Int32.Parse(purcAttributes[index]);
        }
        else
        {
            date = Util.DEFAULT_INT;
        }
    }


    private static void setAttrib(out double date, int index, List<string> purcAttributes)
    {
        if (hasAttrib(index))
        {
            date = Double.Parse(purcAttributes[index]);
        }
        else
        {
            date = Util.DEFAULT_DOUBLE;
        }
    }


    private static void setAttrib(out string date, int index, List<string> purcAttributes)
    {
        if (hasAttrib(index))
        {
            date = purcAttributes[index];
        }
        else
        {
            date = Util.DEFAULT_STRING;
        }
    }


    public static Purchase parsePurchase(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseItemsIntoAttribs(rawResult);

        if (rawItems.Count == 0)
        {
            throw new Exception("Error: Unparsable Raw Result, or No rows, not even purchase information returned by query for parsing");
        }

        List<Purchase> results = new List<Purchase>();
        List<Item> items = new List<Item>();
        List<string> purcAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItems[0]));
        Purchase retPurc = new Purchase();

        int indexPurcAmount = colNames.IndexOf("Amount_purchase");
        int indexNotes_purc = colNames.IndexOf("Notes_purchase");
        int indexFees_Purc = colNames.IndexOf("Fees_purchase");
        int indexPurcDate = colNames.IndexOf("Date_Purchased");
        int indexPURC_ID = colNames.IndexOf("PURCHASE_ID");
        int indexSeller = colNames.IndexOf("Seller");
        int indexTax = colNames.IndexOf("Tax");

        setAttrib(out retPurc.Amount_purchase, indexPurcAmount, purcAttributes);
        setAttrib(out retPurc.Notes_purchase, indexNotes_purc, purcAttributes);
        setAttrib(out retPurc.Date_Purchased, indexPurcDate, purcAttributes);
        setAttrib(out retPurc.Fees_purchase, indexFees_Purc, purcAttributes);
        setAttrib(out retPurc.PURCHASE_ID, indexPURC_ID, purcAttributes);
        setAttrib(out retPurc.Seller, indexSeller, purcAttributes);
        setAttrib(out retPurc.Tax, indexTax, purcAttributes);


        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a Item with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            int itemIDCol = colNames.IndexOf("ITEM_ID");
            if (itemIDCol == -1) break;

            // Check if item exists for this row
            if (itemAttributes[itemIDCol] == "None")
            {
                continue;
            }

            items.Add(new Item(itemAttributes, colNames));
        }
        retPurc.items = items;
        // TODO: Add items to the purchase
        return retPurc;
    }


    public static List<MyImage> parseImages(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseItemsIntoAttribs(rawResult);
        List<MyImage> results = new List<MyImage>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a Item with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new MyImage(itemAttributes, colNames));
        }

        return results;
    }

    public static Dictionary<int, MyImage> parseThumbnails(List<string> rawThumbnails)
    {
        Dictionary<int, MyImage> results = new Dictionary<int, MyImage>();
        foreach (string rawThumbnail in rawThumbnails)
        {
            // Seperate each image item into individual image attributes
            List<string> thumbnailAttribs = new List<string>(Util.splitOnTopLevelCommas(rawThumbnail));

            int itemID = Int32.Parse(thumbnailAttribs[0]);
            int imageID = Int32.Parse(thumbnailAttribs[1]);
            byte[] imageBytes = new byte[thumbnailAttribs[2].Length];

            // Get list of the individual bytes, each as a string
            List<string> imageStrBytes = new List<string>(thumbnailAttribs[2].Trim(new char[] { '[', ']' }).Split(", "));

            // Convert strings to bytes
            for (int j = 0; j < imageStrBytes.Count; j++)
            {
                imageBytes[j] = (byte)Int32.Parse(imageStrBytes[j]);
            }
            // Make an image from the list of bytes
            MyImage thumbnail = new MyImage(Image.FromStream(new MemoryStream(imageBytes)), -1);

            if (!results.ContainsKey(itemID))
            {
                results.Add(itemID, thumbnail);
            }
        }
        return results;
    }

    internal static int getThumbnailID(string rawResult)
    {
        // Trim '[]' that surrounds the whole string
        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawImageInfos = Util.pairedCharTopLevelSplit(rawResult, '[');

        return Int32.Parse(rawImageInfos[0]);
    }
}