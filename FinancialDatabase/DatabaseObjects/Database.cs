using Python.Runtime;
using Date = Util.Date;
using System.Security.Principal;
using FinancialDatabase.DatabaseObjects;

public static class Database
{
    static bool pythonInitialized = false;

    const string PYTHON_EXEC = @"/K python "/* + "-m pdb "*/ + @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Python\Connection\DCQControl.py ";
    const string START_COL_MARKER = "Column Names:"; // Marker to the Start of Column Names
    const string END_COL_MARKER = "END OF COLUMN NAMES"; // Marker to the End of Column Names
    const string EOS = "EOS";     // end-of-stream
    static Size maxDims = new Size(300, 300);

    private static bool TESTING = false;

    
    private static List<string> getTableNames()
    {
        // Init vals
        List<string> colNames;
        string query = "SHOW TABLES";
        
        
        // Run query to get table names
        string rawTablenames = runStatement(query, out colNames);

        // Return them as individual strings
        List<string> tableNames = Util.mySplit(rawTablenames[3..^3], "'], ['");

        return tableNames;
    }



    /// <summary>
    /// Gets the names of a table's columns
    /// And for each column, and list of attributes as strings
    /// </summary>
    /// <param name="tableName">Table to get the col info for</param>
    /// <returns>A list of (a list of attributes as strings for each col)</returns>
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

    /// <summary>
    /// Get a table that covers all column-info for each column for each table
    /// </summary>
    /// <returns>Returns something such as:
    /// { { {'item',     'ITEM_ID',     'int unsigned', ...},
    ///     {'item',     'PurcID_item', 'int unsigned', ...} },
    ///   { {'purchace', 'PURCHACE_ID', 'int unsigned', ...},
    ///     {'purchace', 'Amount_purchase',      'int unsigned', ...} } }</returns>
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


    /// <summary>
    /// Gets a Item from the database given the item's itemID
    /// </summary>
    /// <param name="itemID">ID of item to get from database</param>
    /// <returns>Item represenation of info in databaes</returns>
    public static Item getItem(int itemID)
    {
        string query = QueryBuilder.completeItemIDSearchQuery(itemID);

        List<Item> result = getItems(itemID, true);

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
        Item item = result[0];
        return item;
    }


    /// <summary>
    /// Gets all items from a single purchase
    /// Purchase is identified by an object from that purchase group
    /// </summary>
    /// <param name="item">Item from the purchase used to identify that purchase group</param>
    /// <returns>List of items from that purchse</returns>
    public static List<Item> getPurchItems(Item item)
    {
        string query = QueryBuilder.purchaseQuery(item);
        return Database.getItems(query, false);
    }


    /// <summary>
    /// Gets a Sale from the database given the sale's saleID
    /// </summary>
    /// <param name="saleID">ID of Sale to search for</param>
    /// <returns>Object representing sale</returns>
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


    /// <summary>
    /// Get the types of each column from all tables form the database
    /// </summary>
    /// <returns> Returns dictionary of the format <key,value>
    ///      <columnName, sqlType>
    /// where columnName is of the format
    ///      tableName.columnName
    /// ex:  colDataTypes[ITEM_ID] = 'item.int unsigned' </returns>
    public static Dictionary<string, string> getColDataTypes()
    {
        Dictionary<string, string> colDataTypes = new Dictionary<string, string>();
        List<List<List<string>>> colsInfoALlTables = getColsInfoAllTables();

        // Aggregate each entry in colInfo into colDataTypes like:
        // colDataTypes[ITEM_ID] = 'item.int unsigned'
        foreach (List<List<string>> tableCols in colsInfoALlTables)
        {
            foreach (List<string> colInfo in tableCols)
            {
                string tableName = colInfo[0];
                string colName = tableName + "." + colInfo[1];
                string type = colInfo[2];

                // Test if column name already entered
                if (colDataTypes.TryGetValue(colName, out _))
                {
                    throw new Exception("Error: Duplicate column name: " + colName);
                }
                
                colDataTypes[colName] = type;
            }
        }


        //Hardcoded types for special cases that can't be found in database
        colDataTypes["shipping.WeightLbs"] = "int unsigned";
        colDataTypes["shipping.WeightOz"] = "int unsigned";
        QueryBuilder.setColDataTypesLocal(colDataTypes);
        return colDataTypes;
    }


    /// <summary>
    /// For the item search tab
    /// Given a search query, will search the database for items related to the query
    /// </summary>
    /// <param name="Q"> Object that represents user-input search query</param>
    /// <returns>List of Items from the database</returns>
    public static List<Item> getItems(SearchQuery Q)
    {
        string query = QueryBuilder.searchByNameQuery(Q);
        return getItems(query, true);
    }


    private static List<Item> getItems(string query, bool includeThumbnails)
    {
        // Empty case
        if (query.CompareTo("") == 0)
        {
            query = QueryBuilder.defaultQuery();
        }

        // Get raw items
        List<string> colNames;
        string queryOutput = runStatement(query, out colNames);

        // Parse raw items
        List<Item> parsedItems = DtbParser.parseItems(queryOutput, colNames);

        // Attach thumbnails
        if (includeThumbnails)
        {
            parsedItems = Database.attachThumbnails(parsedItems);
        }

        return parsedItems;
    }



    public static void deleteByName(string name)
    {
        List<Item> items = getItems(new SearchQuery(new List<string> {  }, name, new Util.Date(1000,1,1), new Util.Date(9999,1,1), true, true, false, false));
        
        if (items.Count() > 1)
        {
            throw new Exception("Error: Trying to delete by name, but multiple items exist with the same name. This method is only intended to delete a single item and this is a catch in case there are more items with the same name");
        }

        if (items.Count == 0)
        {
            throw new Exception("Error: Item not found to delete by name: " + name);
        }
        
        Database.deleteItem(items[0]);
    }



    /// <summary>
    /// For the item search tab
    /// Given an itemID, it will search the database for items related to the query
    /// This gives a complete item, with purchase info, shipping info, .etc
    /// </summary>
    /// <param name="itemID"> itemID of object to search for</param>
    /// <param name="includeThumbnails"> Include thumbnail images with the returned items</param>
    /// <returns>List of Items from the database</returns>
    public static List<Item> getItems(int itemID, bool includeThumbnails)
    {
        string query = QueryBuilder.completeItemIDSearchQuery(itemID);
        return getItems(query, includeThumbnails);
    }


    // Get a mapping of itemID's and their corresponding thumbnails
    private static Dictionary<int, MyImage> getThumbnails(List<Item> items)
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
        List<string> rawThumbnails = Util.pairedCharTopLevelSplit(rawResult, '[');

        return DtbParser.parseThumbnails(rawThumbnails);
    }

    /// <summary>
    /// Attaches thumbnails to a list of items
    /// Default thumbnail is provided for those without one
    /// </summary>
    /// <param name="items">list of ResultItems without a thumbnail</param>
    /// <returns>List of thumbnails attached to original items</returns>
    private static List<Item> attachThumbnails(List<Item> items)
    {
        // Empty case
        if (items.Count == 0)
        {
            return new List<Item>();
        }

        Dictionary<int, MyImage> thumbnails = getThumbnails(items);
        
        // Attach each thumbnail to the respective Item
        foreach (Item item in items)
        {
            MyImage thumbnail;
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
        List<string> colNames;
        string queryOutput = runStatement(query, out colNames);

        // No sales found, return empty list
        if (queryOutput.CompareTo("[]") == 0)
        {
            return new List<Sale>();
        }

        List<Sale> parsedItems = DtbParser.parseRawSales(queryOutput, colNames);

        return parsedItems;
    }


    /// <summary>
    /// Get a list of Sale objects given the search query
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static List<Sale> runSaleSearchQuery(Item item)
    {
        string query = QueryBuilder.saleQuery(item);
        List<string> colNames;
        string queryOutput = runStatement(query, out colNames);

        // No sales found, return empty list
        if (queryOutput.CompareTo("[]") == 0)
        {
            return new List<Sale>();
        }

        List<Sale> parsedItems = DtbParser.parseRawSales(queryOutput, colNames);

        return parsedItems;
    }


    /// <summary>
    /// Run a given SQL statement with ability to return col names and lastrowid
    /// </summary>
    /// <param name="statement">MySQL Statement to run</param>
    /// <param name="colNames">ColNames if the statement returns any rows. Note: These rows may be for MySQL Database variables, not necessiarily just column names from a table</param>
    /// <param name="lastrowid">If MySQL Statement creates, updates, or deletes row, that rowID number would be returned here</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static string runStatement(string statement, out List<string> colNames, out int lastrowid)
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
    private static string runStatement(string statement, out List<string> colNames)
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
    private static string runStatement(string statement, out int lastrowid)
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
    private static string runStatement(string statement)
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


    /// <summary>
    /// Insert a new purchase into the database
    /// Note: This does not insert an item to go with it, just the purchase itself
    /// </summary>
    /// <returns>Returns the ID of the purchase inserted</returns>
    public static int insertPurchase(double purcPrice, string notes, Date PurcDate)
    {
        string query = QueryBuilder.insertPurchaseQuery(purcPrice, notes, PurcDate);
        int purcID;
        runStatement(query, out purcID);
        return purcID;
    }


    /// <summary>
    /// Insert a Item into the database
    /// </summary>
    /// <returns>Returns colInfo of running the SQL statement</returns>
    public static string insertItem(Item item)
    {
        int throwaway;
        return insertItem(item, out throwaway);
    }


    // Insert a Item into the database
    // Returns colInfo of running the SQL statement
    // Gives option of keeping the lastrowid
    public static string insertItem(Item item, out int lastrowid)
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
            item.set_ITEM_ID(lastrowid);
            insertShipInfo(item);
        }
        
        return output;
    }

    

    public static string insertSale(Sale sale)
    {
        return insertSale(sale, out int _);
    }


    // Returns colInfo of running the SQL statement
    public static string insertSale(Sale sale, out int lastrowid)
    {
        string query = QueryBuilder.saleInsertQuery(sale);
        string output = runStatement(query, out lastrowid);
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


    /// <summary>
    /// Return the size of a given Image when sized down to fit into
    /// thumbnail dimensions without changing the image's aspect ratio
    /// </summary>
    /// <param name="image">Image to resize</param>
    private static Size getThumbnailSize(Image image)
    {
        return Util.getImageSizeFittedIntoMaxDims(image, maxDims);
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


    public static int insertImage(string filePath, int itemID)
    {
        if (filePath == null) { throw new Exception("ERROR: No image path to inesrt"); }

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
        return imageID;
    }


    // Heart of the Database
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
            PyObject[] rawResult = Connector.runQuery(query, TESTING);
            
            string ErrMsg = "ERROR: NULL rawResult val in Database.cs:runPython()";
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


    // Given a Item, delete from the database
    public static void deleteItem(Item item)
    {
        if (item is null) { return; }

        if (item.hasShippingEntry())
        {
            deleteShipInfo(item);
        }
        
        deleteSales(item);

        Database.deleteThumbnails(item);
        
        Database.deleteImages(item);

        // Delete the item
        string delItemQuery = QueryBuilder.deleteItemQuery(item);
        string result = runStatement(delItemQuery);

        if (result.CompareTo("['ERROR']") == 0)
        {
            throw new Exception("Error: Cannot delete item with id: " + item.get_ITEM_ID());
        }

        // Delete the purchase associated with the item
        // Must do this last because there is a many:1 relationship
        // between items:purchase
        if (item.hasPurchaseEntry())
        {
            if (isEmptyPurchase(item.get_PurchaseID()))
            {
                deletePurchase(item);
            }
        }
    }

    private static void deleteThumbnails(Item item)
    {
        List<MyImage> images = Database.getImages(item);

        foreach (MyImage image in images)
        {
            if (image is null)
            {
                throw new Exception("Error: Trying to delete thumbnail where the the MyImage object does not exist");
            }
            if (image.thumbnailID == Util.DEFAULT_INT)
            {
                continue;
            }

            string removeForeignKeyQuery = QueryBuilder.updateQuery(image, "image.thumbnailID", "NULL");
            Database.runStatement(removeForeignKeyQuery);

            Database.updateRow(item, "item.ThumbnailID", "NULL");

            string query = QueryBuilder.deleteThumbnailQuery(image.thumbnailID);
            Database.runStatement(query);
        }
    }


    // Given an item, delete its sales from the database
    private static void deleteSales(Item item)
    {
        string query = QueryBuilder.deleteAllSalesQuery(item);
        runStatement(query);
    }


    // Given a item, delete its purchase object from the database
    private static void deletePurchase(Item item)
    {
        string query = QueryBuilder.deletePurchaseQuery(item);
        runStatement(query);
    }



    public static void deleteImages(Item item)
    {
        string query = QueryBuilder.deleteImagesQuery(item);
        runStatement(query);

        query = QueryBuilder.setThumbnail(item.get_ITEM_ID(), null);
        runStatement(query);
    }


    public static void deleteShipInfo(Item item)
    {
        string query = QueryBuilder.deleteShipInfoQuery(item);
        runStatement(query);
    }


    // Checks if the given purchase has no items associated with it
    private static bool isEmptyPurchase(int purcID)
    {
        return (getItems(QueryBuilder.purchaseQuery(purcID), false).Count() == 0);   
    }


    // Will get all the full images, not the thumbnails
    public static List<MyImage> getAllImages(Item newItem)
    {
        int lastrowid;
        List<string> colNames;

        string query = QueryBuilder.getImages(newItem);
        string rawResult = runStatement(query, out colNames, out lastrowid);


        // No images returned from query
        if (rawResult.CompareTo("[]") == 0)
        {
            return Util.DEFAULT_IMAGES;
        }
        
        return DtbParser.parseImages(rawResult, colNames);
    }

    
    // Given an imageID, get it's thumbnailID
    private static int getImageThumbnailID(int currImageID)
    {
        string rawResult = runStatement("SELECT thumbnailID FROM image WHERE IMAGE_ID = " + currImageID + ";");

        int thumbnailID = DtbParser.getThumbnailID(rawResult);

        return thumbnailID;
    }

    public static bool updateRow(Item resultItem, string attrib, string newVal)
    {
        string query = QueryBuilder.updateQuery(resultItem, attrib, newVal);
        return Database.runStatement(query).CompareTo("['ERROR']") != 0; // "ERROR"?
    }
    
    public static bool updateRow(Item resultItem, string attrib, int newVal)
    {
        return updateRow(resultItem, attrib, newVal.ToString());
    }

    public static bool updateRow(Item resultItem, string attrib, double newVal)
    {
        return updateRow(resultItem, attrib, newVal.ToString());
    }

    public static bool updateRow(Sale saleItem, string attrib, string newVal) {
        string query = QueryBuilder.updateQuery(saleItem, attrib, newVal);
        return Database.runStatement(query).CompareTo("ERROR") != 0;
    }

    public static bool updateRow(Sale saleItem, string attrib, Date d) {
        string query = QueryBuilder.updateQuery(saleItem, attrib, d);
        return Database.runStatement(query).CompareTo("ERROR") != 0;
    }
    
    public static bool updateRow(Item resultItem, string attrib, Date d)
    {
        string query = QueryBuilder.updateQuery(resultItem, attrib, d);
        return Database.runStatement(query).CompareTo("ERROR") != 0;
    }


    public static int insertShipInfo(Item resultItem, int weightLbs, int weightOz, int l, int w, int h)
    {
        string query = QueryBuilder.shipInfoInsertQuery(resultItem, weightLbs, weightOz, l, w, h);

        int lastrowid;
        string output = Database.runStatement(query, out lastrowid);

        int shippingID = lastrowid;
        query = QueryBuilder.updateQuery(resultItem, "item.ShippingID", shippingID.ToString());

        // Update the item table with the new shipping info ID
        output = Database.runStatement(query);

        return shippingID;

    }


    public static int insertShipInfo(Item item)
    {
        return insertShipInfo(item, item.get_WeightLbs(), item.get_WeightOz(), item.get_Length(), item.get_Width(), item.get_Height());
    }


    public static bool deleteSale(Sale currSale)
    {
        string query = QueryBuilder.deleteSaleQuery(currSale);
        string output = runStatement(query);
        return output.CompareTo("ERROR") == 0;
    }


    public static Purchase getPurchase(Item item)
    {
        List<string> colNames;
        string query = QueryBuilder.purchaseQueryByItemID(item.get_ITEM_ID());
        string output = runStatement(query, out colNames);
        return  DtbParser.parsePurchase(output, colNames);
    }


    public static Purchase getPurchase(int purchaseID)
    {
        List<string> colNames;
        string query = QueryBuilder.purchaseQuery(purchaseID);
        string output = runStatement(query, out colNames);
        return DtbParser.parsePurchase(output, colNames);
    }


    public static List<MyImage> getImages(Item item)
    {
        string query = QueryBuilder.getImages(item);
        string rawOutput = runStatement(query, out List<string> colNames);
        return DtbParser.parseImages(rawOutput, colNames);
    }


    public static void setThumbnail(int itemID, int imageID)
    {
        // Check defualt val. Do nothing
        if (itemID == Util.DEFAULT_INT)
        {
            return;
        }

        int newThumbnailID = Database.getImageThumbnailID(imageID);
        string query = QueryBuilder.setThumbnail(itemID, newThumbnailID);
        Database.runStatement(query);
    }


    public static void clearAll()
    {
        if (TESTING)
        {
            runStatement("DELETE FROM purchase;");
            runStatement("DELETE FROM sale;");
            runStatement("DELETE FROM shipping;");
            runStatement("DELETE FROM thumbnail;");
            runStatement("DELETE FROM image;");
            runStatement("DELETE FROM fee;");
            runStatement("DELETE FROM item;");
        }
    }

    internal static void closeConnection()
    {
        PythonEngine.Shutdown();
        pythonInitialized = false;
    }

    internal static void deleteImage(int imageID)
    {
        deleteThumbnail(imageID);

        string deleteImageQuery = QueryBuilder.deleteImageQuery(imageID);
        Database.runStatement(deleteImageQuery);
    }

    private static void deleteThumbnail(int imageID)
    {
        MyImage image = Database.getImage(imageID);
        
        if (image is null ||
        image.imageID == Util.DEFAULT_INT)
        {
            throw new Exception("Error: Trying to delete thumbnail where the the imagedoes not exist");
        }

        if (image.thumbnailID == Util.DEFAULT_INT)
        {
            throw new Exception("Error: Thumbnail does not exist for imageID: " + imageID + ";");
        }

        // Remove foreign key in item table referencing the thumbnail, if such an item table entry exists

        Item thumbnailItem = getItemByThumbnailID(image.thumbnailID);

        if (thumbnailItem != null)
        {
            Database.updateRow(thumbnailItem, "item.ThumbnailID", "NULL");
        }

        string removeForeignKeyQuery = QueryBuilder.updateQuery(image, "image.thumbnailID", "NULL");
        Database.runStatement(removeForeignKeyQuery);

        string query = QueryBuilder.deleteThumbnailQuery(image.thumbnailID);
        Database.runStatement(query);
    }

    private static Item getItemByThumbnailID(int thumbnailID)
    {
        string getItemByThumbnailQuery = QueryBuilder.getItemByThumbnail(thumbnailID);
        string rawItems = runStatement(getItemByThumbnailQuery, out List<string> colNames);
        List<Item> thumbnailItems = DtbParser.parseItems(rawItems, colNames);
        if (thumbnailItems.Count > 1)
        {
            throw new Exception("Error: Multiple items returned for same thumbnail");
        }
        else if (thumbnailItems.Count == 0)
        {
            return null;
        }
        else if (thumbnailItems.Count == 1)
        {
            return thumbnailItems[0];
        }

        throw new Exception("Error: Unreachable code reached");
    }

    private static MyImage getImage(int imageID)
    {
        string query = QueryBuilder.getImage(imageID);
        string rawOutput = runStatement(query, out List<string> colNames);
        List<MyImage> images = DtbParser.parseImages(rawOutput, colNames);

        if (images.Count > 1)
        {
            throw new Exception("ERROR: >1 image found for ID: " + imageID + ";");
        }

        if (images.Count == 0)
        {
            throw new Exception("ERROR: No image found for ID: " + imageID + ";");
        }

        return images[0];
    }
}
