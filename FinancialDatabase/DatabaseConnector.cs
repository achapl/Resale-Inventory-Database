using FinancialDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using Python.Runtime;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Reflection.Metadata.Ecma335;
using Date = Util.Date;
using System.Runtime.CompilerServices;
using System.Drawing.Configuration;
using System.Security.Principal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

public static class DatabaseConnector
{
    static bool pythonInitialized = false;

    const string PYTHON_EXEC = @"/K python "/* + "-m pdb "*/ + @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\Connection\DCQControl.py ";
    const string START_COL_MARKER = "Column Names:"; // Marker to the Start of Column Names
    const string END_COL_MARKER = "END OF COLUMN NAMES"; // Marker to the End of Column Names
    const string EOS = "EOS";     // end-of-stream
    static Size maxDims = new Size(300, 300);
    public static List<string> getTableNames()
    {

        string query = "SHOW TABLES";
        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });
        string rawTablenames = runStatement(query, ref colNames, out lastrowid);
        List<string> tableNames = new List<string>(rawTablenames.Substring(3, rawTablenames.Length - 6).Split("'], ['"));

        return tableNames;
    }

    public static ResultItem getItem(int itemID)
    {
        string queryItem = "SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM (SELECT * FROM item WHERE ITEM_ID = " + itemID.ToString() + ") subItem LEFT JOIN purchase ON purchase.PURCHASE_ID = subItem.PurchaseID) subPurchase) subSale LEFT JOIN sale ON sale.SALE_ID = subSale.SaleID) subShipping LEFT JOIN shipping on shipping.SHIPPING_ID = subShipping.shippingID;";

        List<ResultItem> result = RunItemSearchQuery(queryItem);

        // Error Checking
        if (result.Count > 1)
        {
            Console.WriteLine("Error: >1 Items Found for saleID: " + itemID.ToString());
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine(result[i].ToString());
            }
            return null;
        }
        else if (result.Count() == 0)
        {
            Console.WriteLine("Error: No Items Found for ItemID: " + itemID.ToString());
        }

        ResultItem item = result[0];
        return item;
    }

    public static Sale getSale(int saleID)
    {
        string querySale = "SELECT * FROM sale WHERE SALE_ID = " + saleID.ToString() + ";";

        List<Sale> result = RunSaleSearchQuery(querySale);

        // Error Checking
        if (result.Count > 1)
        {
            Console.WriteLine("Error: >1 Sales Found for SALE_ID: " + saleID.ToString());
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine(result[i].ToString());
            }
            return null;
        }
        else if (result.Count() == 0)
        {
            Console.WriteLine("Error: No Items Found for SALE_ID: " + saleID.ToString());
        }

        Sale sale = result[0];
        return sale;
    }

    public static Dictionary<string, string> getColDataTypes()
    {

        List<string> tableNames = getTableNames();

        Dictionary<string, string> colDataTypes = new Dictionary<string, string>();
        string query;
        List<string> output;
        foreach (string tableName in tableNames)
        {
            query = "SHOW COLUMNS FROM " + tableName + ";";

            int lastrowid;
            List<string> colNames = new List<string>(new string[] { "" });
            string rawOutput = runStatement(query, ref colNames, out lastrowid);
            output = new List<string>(rawOutput.Substring(3, rawOutput.Length - 7).Split("'), ('"));


            //removeColumnNames(ref output);
            string[] startAndEnd = { "('", "',)" };
            foreach (string colAndType in output)
            {
                List<string> typesForCol = new List<string>(colAndType.Split(new string[] { "', '" }, StringSplitOptions.None));
                string colName = tableName + "." + typesForCol[0];
                string type = typesForCol[1];
                colDataTypes[colName] = type;
            }
        }

        //Hardcoded types for special cases
        colDataTypes["shipping.WeightLbs"] = "int unsigned";
        colDataTypes["shipping.WeightOz"] = "int unsigned";

        return colDataTypes;
    }

    // General queries, done by manual string input
    public static List<ResultItem> RunItemSearchQuery(string query)
    {
        if (query.CompareTo("") == 0)
        {
            query = QueryBuilder.defaultQuery();
        }

        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });
        string queryOutput = runStatement(query, ref colNames, out lastrowid);

        List<ResultItem> parsedItems = parseItemSearchResult(queryOutput, colNames);
        parsedItems = getSearchImages(parsedItems);

        return parsedItems;
    }

    private static List<ResultItem> getSearchImages(List<ResultItem> parsedItems)
    {
        if (parsedItems.Count == 0)
        {
            return new List<ResultItem>();
        }


        string query = QueryBuilder.buildThumbnailsSearchQuery(parsedItems);
        string rawResult = runStatement(query);

        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawItems = Util.PairedCharTopLevelSplit(rawResult, '[');

        Dictionary<int, Image> results = new Dictionary<int, Image>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> imageAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));

            int itemID = Int32.Parse(imageAttributes[0]);

            byte[] imageRawBytes = new byte[imageAttributes[1].Length];

            imageAttributes[1] = imageAttributes[1].Trim(new char[] { '[', ']' });

            List<string> s = new List<string>(imageAttributes[1].Split(", "));
            for (int j = 0; j < s.Count; j++)
            {
                string elem = s[j];
                imageRawBytes[j] = (byte)Int32.Parse(elem);
            }
            Image i = Image.FromStream(new MemoryStream(imageRawBytes));

            if (!results.ContainsKey(itemID))
            {
                results.Add(itemID, i);
            }
        }

        Image thumbnail;
        foreach (ResultItem item in parsedItems)
        {
            if (results.TryGetValue(item.get_ITEM_ID(), out thumbnail))
            {
                item.add_image(thumbnail);
            } else
            {
                item.add_image(Util.DEFAULT_IMAGE);
            }
            
        }

        return parsedItems;
    }

    public static List<Sale> RunSaleSearchQuery(string query)
    {
        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });
        string queryOutput = runStatement(query, ref colNames, out lastrowid);

        // No sales found, return empty list
        if (queryOutput.CompareTo("[]") == 0)
        {
            return new List<Sale>();
        }

        List<Sale> parsedItems = parseSaleSearchResult(queryOutput, colNames);

        return parsedItems;
    }

    public static string runStatement(string statement, ref List<string> colNames, out int lastrowid)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            throw new Exception("ERROR: Invalid Statement/Query sent to database: " + statement + "\n" + "Python Exception: " + result[1]);
        }
        // Returns [0,1,2] -> result, colNames, cursor.lastrowid
        retList = result[0];
        colNames = new List<string>(result[1].Substring(2, result[1].Length - 4).Split("', '"));
        lastrowid = Int32.Parse(result[2]);
        return retList;
    }

    public static string runStatement(string statement, ref List<string> colNames)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("ERROR") == 0)
        {
            Console.WriteLine("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colNames, cursor.lastrowid
        retList = result[0];
        colNames = new List<string>(result[1].Substring(2, result[1].Length - 4).Split("', '"));
        return retList;
    }

    public static string runStatement(string statement, out int lastrowid)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            Console.WriteLine("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colNames, cursor.lastrowid
        retList = result[0];
        lastrowid = Int32.Parse(result[2]);
        return retList;
    }

    public static string runStatement(string statement)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            Console.WriteLine("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colNames, cursor.lastrowid
        retList = result[0];
        return retList;
    }

    public static int newPurchase(int purcPrice, string notes, Date PurcDate)
    {

        string query = QueryBuilder.buildInsertPurchaseQuery(purcPrice, notes, PurcDate);
        int purcID;
        runStatement(query, out purcID);
        return purcID;
    }

    public static string insertItem(ResultItem item)
    {
        int throwaway;
        return insertItem(item, out throwaway);
    }
    public static string insertItem(ResultItem item, out int lastrowid)
    {
        string query = QueryBuilder.buildItemInsertQuery(item);
        lastrowid = -1;
        string output = runStatement(query, out lastrowid);
        if (lastrowid == -1)
        {
            Console.WriteLine("ERROR, BAD INPUT INTO DATABASE: insertItem");
            return null;
        }

        if (output.CompareTo("ERROR") == 0)
        {
            Console.WriteLine("ERROR insertItem");
            return null;
        }
        int lastrowid2 = -1;
        // If given item also has shipping info, insert that into the database too
        if (item.get_Weight() != Util.DEFAULT_INT)
        {
            item.set_ITEM_ID(lastrowid);
            query = QueryBuilder.buildShipInfoInsertQuery(item);
            output = runStatement(query, out lastrowid2);
            if (lastrowid2 == -1)
            {
                Console.WriteLine("ERROR IN INSERTING SHIPPING INFO");
            }

            string attrib = "item.ShippingID";
            string type = "int unsigned";
            int shippingID = lastrowid2;
            query = QueryBuilder.buildUpdateQuery(item, attrib, type, shippingID.ToString());

            // Update the item table with the new shipping info
            output = runStatement(query);
        }
        
        return output;

    }

    public static string insertSale(Sale sale)
    {
        string query = QueryBuilder.buildSaleInsertQuery(sale);
        int lastrowid;
        string output = runStatement(query, out lastrowid);
        if (lastrowid == -1)
        {
            Console.WriteLine("ERROR, BAD INPUT INTO DATABASE: insertSale");
            return null;
        }

        if (output.CompareTo("ERROR") == 0)
        {
            Console.WriteLine("ERROR insertSale");
            return null;
        }
        return output;
    }


    private static bool isImage(string fileName)
    {
        try
        {
            Image.FromFile(fileName);
            return true;
        }
        catch { return false; }
    }

    private static Size getImageSize(Image image)
    {
        int w = image.Size.Width;
        int h = image.Size.Height;
        double aspectRatio = (double)w / (double)h;
        double maxDimsAspectRatio = (double)maxDims.Height / (double)maxDims.Width;

        Size newSize;

        // image W > H relative to aspect ratio
        // Set W to auxImageWidth, and use aspect ratio to det. height
        if (aspectRatio > maxDimsAspectRatio)
        {
            newSize = new Size(maxDims.Width, (int)Math.Round((double)maxDims.Width / aspectRatio, 0));
        }
        // image W <= H relative to aspect ratio
        // Set H to auxImageWidth, and use aspect ratio to det. with
        else
        {
            newSize = new Size((int)Math.Round((double)maxDims.Height * aspectRatio, 0), maxDims.Height);
        }
        return newSize;
    }

    public static string insertImage(String filePath, int itemID)
    {
        if (!isImage(filePath)) { return null; }

        string fileExt = Path.GetExtension(filePath);
        string fileName = Path.GetFileName(filePath);
        string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
        string imageDest = "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\";

        // Copy file to where MySQL can read it
        string copiedFile = imageDest + fileName;
        
        if (File.Exists(copiedFile))
        {
            File.Delete(copiedFile);
        }
        File.Copy(filePath, copiedFile);

        copiedFile = copiedFile.Replace("\\","\\\\");

        // Insert into database
        int imageID;
        string userID = WindowsIdentity.GetCurrent().Name;

        string query = "INSERT INTO image (image, ItemID) VALUES (LOAD_FILE('" + copiedFile + "'), " + itemID + ");";
        
        runStatement(query, out imageID);

        // Resize the image for a thumbnail
        Image origImage = Image.FromFile(filePath);
        Bitmap resizedImage = new Bitmap(origImage, getImageSize(origImage));
        string resizedImFileDest = imageDest + fileNameNoExt + "_RESIZED" + fileExt;
        if (File.Exists(resizedImFileDest))
        {
            File.Delete(resizedImFileDest);
        }
        resizedImage.Save(resizedImFileDest);
        resizedImFileDest = resizedImFileDest.Replace("\\", "\\\\");

        // Insert resized image into database
        int thumbnailID;
        runStatement("INSERT INTO thumbnail (thumbnail) VALUES (LOAD_FILE('" + resizedImFileDest + "'));", out thumbnailID);
        runStatement("UPDATE image SET thumbnailID = " + thumbnailID + " WHERE IMAGE_ID = " + imageID);

        // Clean up
        File.Delete(copiedFile);
        File.Delete(resizedImFileDest);
        return null;
    }


    // Given a search query, turn it into a string query and run it
    public static List<ResultItem> RunSearchQuery(SearchQuery Q)
    {
        string query = QueryBuilder.buildSearchByNameQuery(Q);
        return RunItemSearchQuery(query);
    }

    private static List<string> runPython(string query)
    {
        // Startup Python
        if (pythonInitialized == false) {
            Runtime.PythonDLL = @"C:\Users\Owner\AppData\Local\Programs\Python\Python311\python311.dll";
            PythonEngine.Initialize();
            pythonInitialized = true;
        }
        
        // Use Python
        List<string> result2 = new List<string>(){ "","",""};
        
        using (Py.GIL())
        {
            

            // Modify path to work
            dynamic os = Py.Import("os");
            dynamic sys = Py.Import("sys");
            sys.path.append(os.path.dirname(os.path.expanduser(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\")));

            // Start Dtb Connector
            dynamic Connector = Py.Import("Connection.DtbConnAndQuery");
            PyObject[] rawResult = Connector.runQuery(query);
            // Convert Python Objects to normal C# strings
            // ?? denotes alternative assignment to
            // result[n] if rawResult[n] is null
            string ErrMsg = "ERROR: NULL rawResult val in DatabaseConnector.cs:runPython()";
            List<List<string>> result = new List<List<string>>();

            //TODO: DELETE
            for(int i = 0; i < rawResult[0].Length(); i++)
            {
                //for (int j = 0; j < rawResult[0,i].Length(); j++)
                {
                  //  result[i][j] = rawResult[0,i,j].ToString();
                }
            }
            result2[0] = rawResult[0].ToString() ?? ErrMsg;
            result2[1] = rawResult[1].ToString() ?? ErrMsg;
            result2[2] = rawResult[2].ToString() ?? ErrMsg;


        }
        return result2;
    }


    // raw result is now the format "[(itemName, itemID, .etc)(item2Name, item2ID, .etc)]"
    // Seperate whole string into list of multiple item strings, "List<string>{ "(itemName, itemID, .etc)", "(item2Name, item2ID, .etc)" }"
    private static List<string> parseMySqlResultIntoItems(string rawResult)
    {
        if (rawResult.CompareTo("[]") == 0)
        {
            return new List<String>();
        }

        // Note format changes to square brackets if any attribute **contains parenthesis**
        // Example: [[itemName, Date(1/1/11), .etc], [item2Name, Date(1/1/11), .etc]]
        rawResult = Util.myTrim(rawResult, new string[] { "[", "]" });

        List<string> rawItems = new List<string>();

        // Split the rawResult into rawItems based on whether mysql sends each returned row encapsulated in '[]' or '()'
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


    private static List<ResultItem> parseItemSearchResult(string rawResult, List<string> colNames)
    {

        List<string> rawItems = parseMySqlResultIntoItems(rawResult);

        List<ResultItem> results = new List<ResultItem>();
        foreach(string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new ResultItem(itemAttributes, colNames));
        }

        return results;
    }

    private static List<Sale> parseSaleSearchResult(string rawResult, List<string> colNames)
    {
        List<string> rawItems = parseMySqlResultIntoItems(rawResult);

        List<Sale> results = new List<Sale>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> itemAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            results.Add(new Sale(itemAttributes, colNames));
        }

        return results;
    }

    internal static void deleteItem(ResultItem item)
    {
        if (item is null) { return; }
        if (item.hasPurchaseEntry()) {
            if (isLastItemInPurchase(item))
            {
                
                string attrib = "item.PurchaseID";
                string updateQuery = QueryBuilder.buildUpdateQuery(item, "item.PurchaseID", "int unsigned", "0");

                // May not be necessary since auto-cascade on delete on database may be a feature
                //runStatement(updateQuery);
                deletePurchase(item);
            }
        }
        if (item.hasShippingEntry())
        {
            // TODO: Make this a function deleteShipInfo for DatabaseConnector
            string shippingDelQuery = QueryBuilder.buildDelShipInfoQuery(item);
            runStatement(shippingDelQuery);
        }
        deleteSales(item);
        string delItemQuery = QueryBuilder.buildDelItemQuery(item);
        runStatement(delItemQuery);
    }

    private static void deleteSales(ResultItem item)
    {
        string query = QueryBuilder.buildDelAllSalesQuery(item);
        runStatement(query);
    }

    private static void deletePurchase(ResultItem item)
    {
        string query = QueryBuilder.buildDeletePurchaseQuery(item);
        runStatement(query);
    }

    private static bool isLastItemInPurchase(ResultItem item)
    {
        // TODO: make this a function for databaseconnector
        // RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item)) part
        return (RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item)).Count() == 1);   
    }

    public static List<Image> getImages(ResultItem newItem)
    {

        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });



        string rawResult = runStatement("SELECT * FROM image WHERE ItemID = " + newItem.get_ITEM_ID(), ref colNames, out lastrowid);


        // No images returned from query
        // TODO: Possibly select a default image
        if (rawResult.CompareTo("[]") == 0) { return Util.DEFAULT_IMAGES; }

        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawItems = Util.PairedCharTopLevelSplit(rawResult, '[');

        List<Image> results = new List<Image>();
        foreach (string rawItem in rawItems)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> imageAttributes = new List<string>(Util.splitOnTopLevelCommas(rawItem));
            // Trim off "b'[byteArr]'"

            byte[] ret2 = new byte[imageAttributes[1].Length];

            imageAttributes[1] = imageAttributes[1].Trim(new char[] { '[', ']' });
            
            List<string> s = new List<string>(imageAttributes[1].Split(", "));
            for (int j = 0; j < s.Count; j++)
            {
                string elem = s[j];
                ret2[j] = (byte)Int32.Parse(elem);
            }
            Image i = Image.FromStream(new MemoryStream(ret2));

            results.Add(i);
        }

        
        return results;
    }

    // Get rid of all '\x' ()
    private static byte[] convertToBytes(byte[] bytesIN)
    {
        byte[] bytesOUT = new byte[bytesIN.Length];
        int iOUT = 0;
        int iIN = 0;
        for (iIN = 0; iIN < bytesIN.Length; iIN++)
        {
            /*if ( (iIN == 0 &&
                iIN < bytesIN.Length - 4 &&
                bytesIN[iIN] == (byte)'\\' &&
                bytesIN[iIN + 1] == (byte)'\\' &&
                bytesIN[iIN + 2] == (byte)'\\' &&
                bytesIN[iIN + 3] == (byte)'\\') 
                ||
                (iIN > 0 &&
                iIN < bytesIN.Length - 4 &&
                bytesIN[iIN - 1] != (byte)'\\' &&
                bytesIN[iIN + 0] == (byte)'\\' &&
                bytesIN[iIN + 1] == (byte)'\\' &&
                bytesIN[iIN + 2] == (byte)'\\' &&
                bytesIN[iIN + 3] == (byte)'\\'))
            {
                bytesOUT[iOUT] = (byte)'\\';
                iOUT++;
                iIN += 3;
                continue;
            }*/

            if (iIN < bytesIN.Length - 1 &&
                iIN > 0 &&
                bytesIN[iIN - 1] != (byte)'\\' &&
                bytesIN[iIN] == (byte)'\\' &&
                bytesIN[iIN + 1] == (byte)'\\')
            {
                if (iIN > 0 && bytesIN[iIN - 1] != (byte)'\\')
                    // Skip over '\\x'
                    if (iIN < bytesIN.Length - 4 &&
                        bytesIN[iIN + 2] == (byte)'x')
                    {
                        char firstDig = (char)Int32.Parse(bytesIN[iIN + 3].ToString());
                        char secondDig = (char)Int32.Parse(bytesIN[iIN + 4].ToString());
                        char[] combChars = { firstDig, secondDig };
                        string total = "0x" + new string(combChars);
                        int totalINT = Convert.ToInt32(total, 16);
                        bytesOUT[iOUT] = (byte)totalINT;
                        iIN += 4;
                        iOUT++;
                        continue;
                    }

                    // Change '\\r' to carriage return
                    else if (bytesIN[iIN + 2] == (byte)'r')
                    {
                        bytesOUT[iOUT] = (byte)13;
                    }

                    else if (bytesIN[iIN + 2] == (byte)'n')
                    {
                        bytesOUT[iOUT] = (byte)10;
                    }
                    else
                    {
                        bytesOUT[iOUT] = bytesIN[iIN];
                        iOUT++;
                        continue;
                    }
                iIN += 2;
            }
            // Special case for first bytes of the  array
            else if (
                iIN == 0 &&
                bytesIN[iIN] == (byte)'\\' &&
                bytesIN[iIN + 1] == (byte)'\\')
                {
                    // Skip over '\\x'
                    if (iIN < bytesIN.Length - 4 &&
                        bytesIN[iIN + 2] == (byte)'x')
                    {
                        char firstDig = (char)Int32.Parse(bytesIN[iIN + 3].ToString());
                        char secondDig = (char)Int32.Parse(bytesIN[iIN + 4].ToString());
                        char[] combChars = { firstDig, secondDig };
                        string total = "0x" + new string(combChars);
                        int totalINT = Convert.ToInt32(total, 16);
                        bytesOUT[iOUT] = (byte)totalINT;
                        iIN += 4;
                        iOUT++;
                        continue;
                    }

                    // Change '\\r' to carriage return
                    else if (bytesIN[iIN + 2] == (byte)'r')
                    {
                        bytesOUT[iOUT] = (byte)13;
                    }

                    else if (bytesIN[iIN + 2] == (byte)'n')
                    {
                        bytesOUT[iOUT] = (byte)10;
                    }
                iIN += 2;
            }
            else
            {
                bytesOUT[iOUT] = bytesIN[iIN];
            }
            iOUT++;
        }
        return bytesOUT;
    }
}
