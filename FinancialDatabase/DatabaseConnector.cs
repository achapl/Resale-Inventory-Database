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
using System.Drawing.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static FinancialDatabase.Form1;

public static class DatabaseConnector
{
    static bool pythonInitialized = false;

    const string PYTHON_EXEC = @"/K python "/* + "-m pdb "*/ + @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\Connection\DCQControl.py ";
    const string START_COL_MARKER = "Column Names:"; // Marker to the Start of Column Names
    const string END_COL_MARKER = "END OF COLUMN NAMES"; // Marker to the End of Column Names
    const string EOS = "EOS";     // end-of-stream
    static Size maxDims = new Size(300, 300);


    
    private static List<string> getTableNames()
    {
        // Init vals
        List<string> colNames = new List<string>(new string[] { "" });
        string query = "SHOW TABLES";
        
        
        // Run query to get table names
        string rawTablenames = runStatement(query, ref colNames);

        // Return them as individual strings
        List<string> tableNames = Util.mySplit(rawTablenames[3..^3], "'], ['");

        return tableNames;
    }


    // Gets the names of a table's columns
    // And for each column, and list of attributes as strings
    private static List<List<string>> getColumnsInfo(string tableName)
    {
        // Init vals
        string query = QueryBuilder.getColumnNames(tableName);
        string rawOutput = runStatement(query);
        List<List<string>> infoForEachCol = new List<List<string>>();


        // colInfo will be List of strings like:
        //      { "['col1', 'type1', ...]",  "['col2', 'type2', ...]" }
        List<string> colInfos = Util.mySplit(rawOutput[3..^3], "'], ['");

        // Seperate each colInfo into attributes, and compile them for return
        // attribsForCol will be like:
        // { "col1", "type1", ... } for each element in colInfo
        foreach (string colInfo in colInfos)
        {
            List<string> attribsForCol = Util.mySplit(colInfo, "', '");
            // Append table name to start
            attribsForCol.Insert(0, tableName);
            infoForEachCol.Add(attribsForCol);
        }

        return infoForEachCol;
    }


    // Get a table that covers all column-info for each column for each table
    // Returns something such as:
    // { { {'item',     'ITEM_ID',     'int unsigned', ...},
    //     {'item',     'PurcID_item', 'int unsigned', ...} },
    //   { {'purchace', 'PURCHACE_ID', 'int unsigned', ...},
    //     {'purchace', 'purcAmount',  'int unsigned', ...} } }
    private static List<List<List<string>>> getColsInfoAllTables()
    {
        List<List<List<string>>> colInfoAllTables = new List<List<List<string>>>();

        List<string> tableNames = getTableNames();
        foreach (string tableName in tableNames)
        {
            List<List<string>> colInfos = getColumnsInfo(tableName);
            colInfoAllTables.Add(colInfos);
        }


        return colInfoAllTables;
    }
    

    // Gets a ResultItem from the database given the item's itemID
    public static ResultItem getItem(int itemID)
    {
        string query = QueryBuilder.completeItemIDSearchQuery(itemID);

        List<ResultItem> result = getItems(itemID, true);

        // Error Checking
        if (result.Count > 1)
        {
            string error = "Error: >1 Items Found for itemID: " + itemID + "\n";
            for (int i = 0; i < result.Count; i++)
            {
                error += result[i].ToString() + "\n";
            }
            throw new Exception(error);
        }
        else if (result.Count() == 0)
        {
            throw new Exception("Error: Item Not Found for itemID: " + itemID);
        }

        // Only option left, a single item was found (Count will not be negative)
        ResultItem item = result[0];
        return item;
    }

    public static List<ResultItem> getPurchItems(ResultItem item)
    {
        string query = QueryBuilder.purchaseQuery(item);
        return DatabaseConnector.getItems(query, false);
    }


    // Gets a Sale from the database given the sale's saleID
    public static Sale getSale(int saleID)
    {
        string querySale = QueryBuilder.getSaleByID(saleID);
        List<Sale> result = runSaleSearchQuery(querySale);


        // Error Checking
        if (result.Count > 1)
        {
            string error = "Error: >1 Sale Found for saleID: " + saleID + "\n";
            for (int i = 0; i < result.Count; i++)
            {
                error += result[i].ToString() + "\n";
            }
            throw new Exception(error);
        }
        else if (result.Count() == 0)
        {
            throw new Exception("Error: Sale Not Found for saleID: " + saleID);
        }

        // Only option left, a single sale was found (Count will not be negative)
        Sale sale = result[0];
        return sale;
    }


    // Get the types of each column from all tables form the database
    // Returns dictionary of the format <key,value>
    //      <columnName, sqlType>
    // where columnName is of the format
    //      tableName.columnName
    // ex:  colDataTypes[ITEM_ID] = 'item.int unsigned' 
    public static Dictionary<string, string> getColDataTypes()
    {
        // Init vals
        Dictionary<string, string> colDataTypes = new Dictionary<string, string>();
        List<List<List<string>>> colsInfoALlTables = getColsInfoAllTables();
        string tableName;
        string colName;
        string type;


        // Aggregate each entry in colInfo into colDataTypes like:
        // colDataTypes[ITEM_ID] = 'item.int unsigned'
        foreach (List<List<string>> tableCols in colsInfoALlTables)
        {
            foreach (List<string> colInfo in tableCols)
            {
                tableName = colInfo[0];
                colName = tableName + "." + colInfo[1];
                type = colInfo[2];

                // Test if column name already entered
                string throwaway;
                if (colDataTypes.TryGetValue(colName, out throwaway))
                {
                    throw new Exception("Error: Duplicate column name: " + colName);
                }
                
                colDataTypes[colName] = type;
            }
        }


        //Hardcoded types for special cases that can't be found in database
        colDataTypes["shipping.WeightLbs"] = "int unsigned";
        colDataTypes["shipping.WeightOz"] = "int unsigned";

        return colDataTypes;
    }

    public static List<ResultItem> getItems(SearchQuery Q)
    {
        string query = QueryBuilder.searchByNameQuery(Q);
        return getItems(query, true);
    }

    private static List<ResultItem> getItems(string query, bool includeThumbnails)
    {
        // Empty case
        if (query.CompareTo("") == 0)
        {
            query = QueryBuilder.defaultQuery();
        }

        // Get raw items
        List<string> colNames = new List<string>(new string[] { "" });
        string queryOutput = runStatement(query, ref colNames);

        // Parse raw items
        List<ResultItem> parsedItems = DtbParser.parseItems(queryOutput, colNames);

        // Attach thumbnails
        if (includeThumbnails)
        {
            parsedItems = DatabaseConnector.attachThumbnails(parsedItems);
        }

        return parsedItems;
    }

    // General search queries, done by manual string input
    // Returns a list of the items found by the query
    // Can opt with includeThumbnails to attach thumbnails with ResultItems
    public static List<ResultItem> getItems(ResultItem item, bool includeThumbnails)
    {
        return getItems(item.get_ITEM_ID(), includeThumbnails);
    }

    public static List<ResultItem> getItems(int itemID, bool includeThumbnails)
    {
        string query = QueryBuilder.completeItemIDSearchQuery(itemID);
        return getItems(query, includeThumbnails);
    }


    // Get a mapping of itemID's and their corresponding thumbnails
    private static Dictionary<int, MyImage> getThumbnails(List<ResultItem> items)
    {
        // Empty case
        if (items.Count == 0)
        {
            return new Dictionary<int, MyImage>();
        }        

        // Get raw thumbnail data from database
        string query = QueryBuilder.thumbnailQuery(items);
        string rawResult = runStatement(query);

        // Parse raw data into individual raw thumbnails
        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawThumbnails = Util.PairedCharTopLevelSplit(rawResult, '[');

        Dictionary<int, MyImage> results = new Dictionary<int, MyImage>();
        List<string> thumbnailAttribs;
        List<string> imageStrBytes;
        MyImage thumbnail;
        byte[] imageBytes;
        int imageID;
        int itemID;
        // 
        foreach (string rawThumbnail in rawThumbnails)
        {
            // Seperate each image item into individual image attributes
            thumbnailAttribs = new List<string>(Util.splitOnTopLevelCommas(rawThumbnail));

            itemID  = Int32.Parse(thumbnailAttribs[0]);
            imageID = Int32.Parse(thumbnailAttribs[1]);
            imageBytes = new byte[thumbnailAttribs[2].Length];

            // Get list of the individual bytes, each as a string
            imageStrBytes = new List<string>(thumbnailAttribs[2].Trim(new char[] { '[', ']' }).Split(", "));

            // Convert strings to bytes
            for (int j = 0; j < imageStrBytes.Count; j++)
            {
                imageBytes[j] = (byte)Int32.Parse(imageStrBytes[j]);
            }
            // Make an image from the list of bytes
            thumbnail = new MyImage(Image.FromStream(new MemoryStream(imageBytes)), -1);

            if (!results.ContainsKey(itemID))
            {
                results.Add(itemID, thumbnail);
            }
        }
        return results;
    }

    // Given a list of ResultItems without a thumbnail,
    // it will return the list with all items having a thumbnail
    // Default thumbnail is provided for those without one
    private static List<ResultItem> attachThumbnails(List<ResultItem> items)
    {
        // Empty case
        if (items.Count == 0)
        {
            return new List<ResultItem>();
        }

        // Init vals
        MyImage thumbnail;

        Dictionary<int, MyImage> thumbnails = getThumbnails(items);
        
        // Attach each thumbnail to the respective ResultItem
        foreach (ResultItem item in items)
        {
            if (thumbnails.TryGetValue(item.get_ITEM_ID(), out thumbnail))
            {
                item.set_Thumbnail(thumbnail);
            }
            else
            {
                item.set_Thumbnail(Util.DEFAULT_IMAGE);
            }
        }

        return items;
    }


    private static List<Sale> runSaleSearchQuery(string query)
    {
        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });
        string queryOutput = runStatement(query, ref colNames, out lastrowid);

        // No sales found, return empty list
        if (queryOutput.CompareTo("[]") == 0)
        {
            return new List<Sale>();
        }

        List<Sale> parsedItems = DtbParser.parseRawSales(queryOutput, colNames);

        return parsedItems;
    }

    // Get a list of Sale objects given the search query
    public static List<Sale> runSaleSearchQuery(ResultItem item)
    {
        string query = QueryBuilder.saleQuery(item);
        List<string> colNames = new List<string>(new string[] { "" });
        string queryOutput = runStatement(query, ref colNames);

        // No sales found, return empty list
        if (queryOutput.CompareTo("[]") == 0)
        {
            return new List<Sale>();
        }

        List<Sale> parsedItems = DtbParser.parseRawSales(queryOutput, colNames);

        return parsedItems;
    }


    // Run a given SQL statement with ability to return col names and lastrowid
    public static string runStatement(string statement, ref List<string> colNames, out int lastrowid)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            throw new Exception("ERROR: Invalid Statement/Query sent to database: " + statement + "\n" + "Python Exception: " + result[1]);
        }
        // Returns [0,1,2] -> result, colInfo, cursor.lastrowid
        retList = result[0];
        colNames = Util.mySplit(result[1][2..^2], "', '");
        lastrowid = Int32.Parse(result[2]);
        return retList;
    }


    // Run a given SQL statement with ability to return col names
    public static string runStatement(string statement, ref List<string> colNames)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("ERROR") == 0)
        {
            throw new Exception("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colInfo, cursor.lastrowid
        retList = result[0];
        colNames = Util.mySplit(result[1][2..^2], "', '");
        return retList;
    }


    // Run a given SQL statement with ability to return lastrowid
    public static string runStatement(string statement, out int lastrowid)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            throw new Exception("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colInfo, cursor.lastrowid
        retList = result[0];
        lastrowid = Int32.Parse(result[2]);
        return retList;
    }


    // Run a given SQL statement
    public static string runStatement(string statement)
    {
        string retList;

        List<string> result = runPython(statement);
        if (result[0].CompareTo("['ERROR']") == 0)
        {
            throw new Exception("ERROR: Invalid Statement/Query sent to database: " + statement);
        }
        // Returns [0,1,2] -> result, colInfo, cursor.lastrowid
        retList = result[0];
        return retList;
    }


    // Insert a new purchase into the database
    // Note: This does not insert an item to go with it, just the purchase itself
    // Return the purcID
    public static int newPurchase(int purcPrice, string notes, Date PurcDate)
    {
        string query = QueryBuilder.insertPurchaseQuery(purcPrice, notes, PurcDate);
        int purcID;
        runStatement(query, out purcID);
        return purcID;
    }


    // Insert a ResultItem into the database
    // Returns colInfo of running the SQL statement
    public static string insertItem(ResultItem item)
    {
        int throwaway;
        return insertItem(item, out throwaway);
    }


    // Insert a ResultItem into the database
    // Returns colInfo of running the SQL statement
    // Gives option of keeping the lastrowid
    public static string insertItem(ResultItem item, out int lastrowid)
    {
        // Insert item into database
        string query = QueryBuilder.itemInsertQuery(item);
        lastrowid = -1;
        string output = runStatement(query, out lastrowid);

        int shippingID = -1;
        // If given item also has shipping info,
        // insert that into the database too
        if (item.get_Weight() != Util.DEFAULT_INT)
        {
            // Insert shipping info
            item.set_ITEM_ID(lastrowid);
            query = QueryBuilder.shipInfoInsertQuery(item);
            output = runStatement(query, out shippingID);

            // Update item entry to link to shipping info
            string attrib = "item.ShippingID";
            string type   = "int unsigned";
            query = QueryBuilder.updateQuery(item, attrib, type, shippingID.ToString());

            // Update the item table with the new shipping info
            output = runStatement(query);
        }
        
        return output;

    }


    // Returns colInfo of running the SQL statement
    public static string insertSale(Sale sale)
    {
        string query = QueryBuilder.saleInsertQuery(sale);
        string output = runStatement(query);
        return output;
    }


    private static bool isImage(string filePath)
    {
        try
        {
            Image.FromFile(filePath);
            return true;
        }
        catch { return false; }
    }


    // Return the size of a given Image when sized down to fit into
    // thumbnail dimensions without changing the image's aspect ratio
    private static Size getThumbnailSize(Image image)
    {
        int w = image.Size.Width;
        int h = image.Size.Height;
        double aspectRatio = (double)w / (double)h;
        double maxDimsAspectRatio = (double)maxDims.Height / (double)maxDims.Width;

        Size newSize;

        // image W > H relative to aspect ratio
        // Set W to maxDims.Width, and use aspect ratio to det. height
        if (aspectRatio > maxDimsAspectRatio)
        {
            newSize = new Size(maxDims.Width, (int)Math.Round((double)maxDims.Width / aspectRatio, 0));
        }
        // image W <= H relative to aspect ratio
        // Set H to maxDims.Height, and use aspect ratio to det. width
        else
        {
            newSize = new Size((int)Math.Round((double)maxDims.Height * aspectRatio, 0), maxDims.Height);
        }
        return newSize;
    }


    private static void deleteExistingFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }


    private static string copyImageToDtbFolder(string source)
    {
        string destination = "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\";
        if (!isImage(source))
        {
            throw new Exception("Error: Source Path is not an image: " + source);
        }

        string fileExt = Path.GetExtension(source);
        string fileName = Path.GetFileName(source);
        string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
        

        // Copy file to where MySQL can read it
        string copiedFile = destination + fileName;
        deleteExistingFile(copiedFile);
        File.Copy(source, copiedFile);

        // Make copied file path able to be read elsewhere
        copiedFile = copiedFile.Replace("\\", "\\\\");
        return copiedFile;
    }


    private static void insertThumbnail(string thumbnailPath, int imageID)
    {
        int thumbnailID;
        runStatement("INSERT INTO thumbnail (thumbnail) VALUES (LOAD_FILE('" + thumbnailPath + "'));", out thumbnailID);
        runStatement("UPDATE image SET thumbnailID = " + thumbnailID + " WHERE IMAGE_ID = " + imageID);
    }


    public static void insertImage(string filePath, int itemID)
    {
        // Copy file to database folder
        string copiedFile = copyImageToDtbFolder(filePath);

        // Insert into database
        int imageID;
        string userID = WindowsIdentity.GetCurrent().Name;

        string query = "INSERT INTO image (image, ItemID) VALUES (LOAD_FILE('" + copiedFile + "'), " + itemID + ");";
        
        runStatement(query, out imageID);

        // Resize the image for a thumbnail
        Image origImage = Image.FromFile(filePath);
        Bitmap resizedImage = new Bitmap(origImage, getThumbnailSize(origImage));

        // Get file path info
        string destination = "C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\";
        string fileExt = Path.GetExtension(filePath);
        string fileName = Path.GetFileName(filePath);
        string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
        string resizedImFileDest = destination + fileNameNoExt + "_RESIZED" + fileExt;

        // Save resized image
        deleteExistingFile(resizedImFileDest);
        resizedImage.Save(resizedImFileDest);
        resizedImFileDest = resizedImFileDest.Replace("\\", "\\\\");

        // Insert resized image into database
        insertThumbnail(resizedImFileDest, imageID);

        // Clean up copied files after uploading the image
        File.Delete(copiedFile);
        File.Delete(resizedImFileDest);
    }


    // Heart of the DatabaseConnector
    // Talks to the python that actually interacts with the database.
    // Gives python the query, and gets from python, the result of running the SQL query
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
            
            string ErrMsg = "ERROR: NULL rawResult val in DatabaseConnector.cs:runPython()";
            List<List<string>> result = new List<List<string>>();

            // Convert Python Objects to normal C# strings
            // ?? denotes alternative assignment to
            // result[n] if rawResult[n] is null
            result2[0] = rawResult[0].ToString() ?? ErrMsg;
            result2[1] = rawResult[1].ToString() ?? ErrMsg;
            result2[2] = rawResult[2].ToString() ?? ErrMsg;


        }
        return result2;
    }


    // Given a ResultItem, delete from the database
    public static void deleteItem(ResultItem item)
    {
        // Empty case
        if (item is null) { return; }

        // 
        if (item.hasPurchaseEntry()) {
            if (isLastItemInPurchase(item))
            {
                
                string attrib = "item.PurchaseID";
                string updateQuery = QueryBuilder.updateQuery(item, "item.PurchaseID", "int unsigned", "0");

                // TODO May not be necessary since auto-cascade on delete on database may be a feature
                //runStatement(updateQuery);
                deletePurchase(item);
            }
        }
        if (item.hasShippingEntry())
        {
            // TODO: Make this a function deleteShipInfo for DatabaseConnector
            string shippingDelQuery = QueryBuilder.deleteShipInfoQuery(item);
            runStatement(shippingDelQuery);
        }
        deleteSales(item);
        string delItemQuery = QueryBuilder.deleteItemQuery(item);
        runStatement(delItemQuery);
    }


    // Given an item, delete its sales from the database
    private static void deleteSales(ResultItem item)
    {
        string query = QueryBuilder.deleteAllSalesQuery(item);
        runStatement(query);
    }


    // Given a item, delete its purchase object from the database
    // TODO: Determine if auto-cascade deletes the corresponding item from the database
    private static void deletePurchase(ResultItem item)
    {
        string query = QueryBuilder.deletePurchaseQuery(item);
        runStatement(query);
    }


    public static void deleteShipInfo(ResultItem item)
    {
        string query = QueryBuilder.deleteShipInfoQuery(item);
        runStatement(query);
    }

    // Checks if the given item is the only item from its corresponding purchase
    private static bool isLastItemInPurchase(ResultItem item)
    {
        return (getItems(QueryBuilder.purchaseQuery(item), false).Count() == 1);   
    }


    // Will get all the full images, not the thumbnails
    public static List<MyImage> getAllImages(ResultItem newItem)
    {

        int lastrowid;
        List<string> colNames = new List<string>(new string[] { "" });



        string rawResult = runStatement("SELECT * FROM image WHERE ItemID = " + newItem.get_ITEM_ID(), ref colNames, out lastrowid);


        // No images returned from query
        if (rawResult.CompareTo("[]") == 0) { return Util.DEFAULT_IMAGES; }
        
        // Trim '[]' that surrounds the whole string
        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawImageInfos = Util.PairedCharTopLevelSplit(rawResult, '[');

        List<MyImage> results = [];
        foreach (string rawImageInfo in rawImageInfos)
        {
            // Seperate each item into individual item attributes to make a ResultItem with it
            List<string> imageAttributes = new(Util.splitOnTopLevelCommas(rawImageInfo));
            string rawImage = imageAttributes[1];
            int imageID = Int32.Parse(imageAttributes[0]);
            MyImage i = new(Util.rawImageStrToImage(rawImage), imageID);

            results.Add(i);
        }

        
        return results;
    }

    
    // Given an imageID, get it's thumbnailID
    public static int getImageThumbnailID(int currImageID)
    {
        string rawResult = runStatement("SELECT thumbnailID FROM image WHERE IMAGE_ID = " + currImageID + ";");

        // Trim '[]' that surrounds the whole string
        rawResult = rawResult.Substring(1, rawResult.Length - 2);
        List<string> rawImageInfos = Util.PairedCharTopLevelSplit(rawResult, '[');

        int thumbnailID = Int32.Parse(rawImageInfos[0]);

        return thumbnailID;
    }

    public static bool updateRow(ResultItem resultItem, string attrib, string type, string newVal)
    {
        string query = QueryBuilder.updateQuery(resultItem, attrib, type, newVal);
        return DatabaseConnector.runStatement(query).CompareTo("ERROR") != 0;
    }

    public static bool updateRow(Sale saleItem, string attrib, string type, string newVal) {
        string query = QueryBuilder.updateQuery(saleItem, attrib, type, newVal);
        return DatabaseConnector.runStatement(query).CompareTo("ERROR") != 0;
    }
    
    public static bool updateRow(Sale saleItem, string attrib, string type, Date d) {
        string query = QueryBuilder.updateQuery(saleItem, attrib, type, d);
        return DatabaseConnector.runStatement(query).CompareTo("ERROR") != 0;
    }
    
    public static bool updateRow(ResultItem resultItem, string attrib, string type, Date d)
    {
        string query = QueryBuilder.updateQuery(resultItem, attrib, type, d);
        return DatabaseConnector.runStatement(query).CompareTo("ERROR") != 0;
    }

    public static void insertShipInfo(ResultItem resultItem, int weightLbs, int weightOz, int l, int w, int h, string weightType)
    {
        string query = QueryBuilder.shipInfoInsertQuery(resultItem, weightLbs, weightOz, l, w, h);

        int lastrowid;
        string output = DatabaseConnector.runStatement(query, out lastrowid);

        string attrib = "item.ShippingID";
        // TOOD: This is hardcoded, but dtbconnector doens't have access to tabController.controlAttributes
        string type = "INT UNSIGNED";
        int shippingID = lastrowid;
        query = QueryBuilder.updateQuery(resultItem, attrib, type, shippingID.ToString());

        // Update the item table with the new shipping info
        output = DatabaseConnector.runStatement(query);
    }

    public static bool deleteSale(Sale currSale)
    {
        string query = QueryBuilder.deleteSaleQuery(currSale);
        string output = runStatement(query);
        return output.CompareTo("ERROR") == 0;
    }
}
