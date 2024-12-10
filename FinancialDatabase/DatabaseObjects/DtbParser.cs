using FinancialDatabase.DatabaseObjects;
using System;

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
    public static List<string> parseItems(string rawResult)
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
            rawItems = Util.PairedCharTopLevelSplit(rawResult, '[');
        }
        else if (rawResult.Length > 0 && rawResult[0] == '(')
        {
            rawItems = Util.PairedCharTopLevelSplit(rawResult, '(');
        }
        else
        {
            throw new Exception("Error: Unexpected character for when parsing individual search results: " + rawResult[0]);
        }
        return rawItems;
    }

    // raw result from a search query is given from the result of the query in the format "[(itemName, itemID, .etc)(item2Name, item2ID, .etc)]"
    // Seperates them all and creates ResultItem objects from each of them. Returns these as a list
    public static List<ResultItem> parseItems(string rawResult, List<string> colNames)
    {

        List<string> rawItems = parseItems(rawResult);

        List<ResultItem> results = new List<ResultItem>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new ResultItem(itemAttributes, colNames));
        }

        return results;
    }

    // raw result from a sale query is given from the result of the query in the format "[(salePrice, itemID, .etc)(salePrice2, item2ID, .etc)]"
    // Seperates them all and creates Sale objects from each of them. Returns these as a list
    public static List<Sale> parseRawSales(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseItems(rawResult);

        List<Sale> results = new List<Sale>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new Sale(itemAttributes, colNames));
        }

        return results;
    }

    public static List<Purchase> parsePurchase(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseItems(rawResult);

        List<Purchase> results = new List<Purchase>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new Purchase(itemAttributes, colNames));
        }

        return results;
    }
}