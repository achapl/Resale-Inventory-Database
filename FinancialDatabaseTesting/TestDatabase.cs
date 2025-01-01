namespace FinancialDatabaseTesting;

using FinancialDatabase;
using NUnit.Framework.Internal.Execution;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using NUnit.Framework.Constraints;
using FinancialDatabase.DatabaseObjects;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Image = System.Drawing.Image;
using System.Windows.Forms.VisualStyles;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using NUnit.Framework.Internal.Commands;

public class TestDatabase
{
    private static Random rand = new Random();

    private static List<int> itemIDsGeneric = new List<int> { };
    private static List<int> itemIDsSamePurc = new List<int> { };

    private static string imagesDir = @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabaseTesting\TestingItems\Images";

    private static string imagePath = @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\s-l1600.png";



    private static object[] testingItemsArrays =
    {
        //            0          1      2                          3        4        5          6         7       8                            8[0]   8[1]                        8[2]     9
        //            name,      price, purc date,                 initQty, currQty, image,     imageID,  itemID, sales                        amount, date,                      saleID   purcID
        new object[] {"Item1",   100.0, new Util.Date(1978,12,16), 2,       1,       imagePath, null,     null,   new object[] { new object[] { 110.0, new Util.Date(1979, 12, 16), null },
                                                                                                                                 new object[] { 100.0, new Util.Date(1979, 12, 17), null }}, null,},
        new object[] {"Item2",   200.0, new Util.Date(1978,12,17), 2,       1,       imagePath, null,     null,   new object[] { new object[] { 220.0, new Util.Date(1979, 12, 16), null }}, null,},
        new object[] {"Item3",   300.0, new Util.Date(1978,12,18), 2,       1,       null,      null,     null,   new object[] { new object[] { 330.0, new Util.Date(1979, 12, 16), null }}, null,},
        new object[] {"Item4",   400.0, new Util.Date(1978,12,18), 2,       1,       null,      null,     null,   new object[] { new object[] { 440.0, new Util.Date(1979, 12, 16), null }}, null,},
    };

    private static object[] dtbTestingItemsSamePurc =
    {
        //            name,     price, purc date,                 initQty, currQty, purcID
        new object[] {"ItemA",  100.0, new Util.Date(1978,12,16), 1,       1,       null   },
        new object[] {"ItemB",  null,                       null, 1,       1,       null   },
        new object[] {"ItemC",  null,                       null, 1,       1,       null   },
    };

    private static int getNumTestingItems()
    {
        return testingItemsArrays.Length + dtbTestingItemsSamePurc.Length;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var DtbObject = typeof(Database);
        var testingVar = DtbObject.GetField("TESTING", BindingFlags.NonPublic | BindingFlags.Static);
        testingVar.SetValue(null, true);

        Database.clearAll();
        Database.getColDataTypes();

        addTestingItems();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Database.clearAll();
        var DtbObject = typeof(Database);
        var testingVar = DtbObject.GetField("TESTING", BindingFlags.NonPublic | BindingFlags.Static);
        testingVar.SetValue(null, true);
    }


    private static void addGenericItems_Sales()
    {
        foreach (object[] item in testingItemsArrays)
        {
            
            object[] sales = (object[]) item[8];
            foreach (object[] sale in sales)
            {
                // Make sale object to insert
                Sale s = new Sale();
                s.set_Date_Sold((Util.Date)sale[1]);
                s.set_Amount_sale((double)sale[0]);
                s.set_ItemID_sale((int) item[7]);
                // Add sale
                if (Database.insertSale(s, out int saleID).CompareTo("['ERROR']") == 0)
                {
                    throw new Exception("TESTING ERROR: Not all sales could be inputted into the database!");
                }

                // Update corresponding item
                if (!Database.updateRow(s, "sale.SALE_ID", saleID.ToString()))
                {
                    throw new Exception("TESTING ERROR: Not all items could be updated with their respective sales in the database!");
                }

                sale[2] = saleID;
            }

        }
    }


    private static void addGenericItems()
    {
        itemIDsGeneric.Clear();
        foreach (object[] item in testingItemsArrays)
        {
            int purcID = Database.insertPurchase((double)item[1], "", (Util.Date)item[2]);
            Item newItem = new Item();
            newItem.set_Name((string)item[0]);
            newItem.set_InitialQuantity((int)item[3]);
            newItem.set_CurrentQuantity((int)item[4]);
            newItem.set_PurchaseID(purcID);
            int itemID;
            Database.insertItem(newItem, out itemID);
            if (item[5] != null)
            {
                Database.insertImage((string)item[5], itemID);
            }
            item[7] = itemID;
            item[9] = purcID;
            itemIDsGeneric.Add(itemID);
        }

        if (itemIDsGeneric.Count != testingItemsArrays.Length) { throw new Exception("TESTING ERROR: Not all items could be inputted into the database!"); }
    }


    private static void addSamePurcItems()
    {
        itemIDsSamePurc.Clear();
        int purcID = Database.insertPurchase((double)((object[])dtbTestingItemsSamePurc[0])[1], "", (Util.Date)((object[])dtbTestingItemsSamePurc[0])[2]);
        foreach (object[] item in dtbTestingItemsSamePurc)
        {
            item[5] = purcID;

            Item newItem = new Item();
            newItem.set_Name((string)item[0]);
            newItem.set_InitialQuantity((int)item[3]);
            newItem.set_CurrentQuantity((int)item[4]);
            newItem.set_PurchaseID(purcID);
            int itemID;
            Database.insertItem(newItem, out itemID);
            itemIDsSamePurc.Add(itemID);
        }
        if (itemIDsSamePurc.Count != dtbTestingItemsSamePurc.Length) { throw new Exception("TESTING ERROR: Not all items could be inputted into the database!"); }
    }


    private static void addTestingItems()
    {
        addGenericItems();
        addSamePurcItems();
        addGenericItems_Sales();
    }



    private static bool hasUniqItems(List<Item> searchItems)
    {
        List<int> itemIDs = new List<int>();
        foreach (Item item in searchItems)
        {
            if (itemIDs.Contains(item.get_ITEM_ID()))
            {
                return false;
            }
            itemIDs.Add(item.get_ITEM_ID());
        }
        return true;
    }



    [Test]
    public static void Test_getItem()
    {
        for (int i = 0; i < itemIDsGeneric.Count; i++)
        {
            int itemID = itemIDsGeneric[i];
            object[] expectedItem = (object[])testingItemsArrays[i];
            Item result = Database.getItem(itemID);

            Assert.AreEqual(itemID, result.get_ITEM_ID());
            Assert.AreEqual(expectedItem[0], result.get_Name());
            Assert.AreEqual(expectedItem[1], result.get_Amount_purchase());
            Assert.AreEqual(expectedItem[2], result.get_Date_Purchased());
            Assert.AreEqual(expectedItem[3], result.get_InitialQuantity());
            Assert.AreEqual(expectedItem[4], result.get_CurrentQuantity());
        }
    }



    [Test]
    public static void Test_getPurchItems()
    {
        for (int i = 0; i < dtbTestingItemsSamePurc.Length; i++)
        {
            // Get items from database
            object[] item = (object[])dtbTestingItemsSamePurc[i];
            Item currItem = new Item();
            currItem.set_PurchaseID((int)((object[])dtbTestingItemsSamePurc[i])[5]);
            List<Item> purcItems = Database.getPurchItems(currItem);

            // Check propor amount of items returned
            Assert.AreEqual(dtbTestingItemsSamePurc.Length, purcItems.Count());

            // Check all items are from the same purchase
            for (int j = 0; j < purcItems.Count(); j++)
            {
                Assert.AreEqual(purcItems[0].get_PurchaseID(), purcItems[j].get_PurchaseID());
            }
        }
    }



    [Test]
    public static void Test_getSale()
    {
        foreach (object[] itemArr in testingItemsArrays)
        {
            foreach (object[] saleArr in (object[]) itemArr[8])
            {
                Sale sale = new Sale();
                sale.set_Amount_sale( (double) saleArr[0]);
                sale.set_Date_Sold((Util.Date) saleArr[1]);
                sale.set_SALE_ID((int) saleArr[2]);
                Sale dtbSale = Database.getSale(sale.get_SALE_ID());

                Assert.AreEqual(sale.get_Date_Sold(), dtbSale.get_Date_Sold());
                Assert.AreEqual(sale.get_Amount_sale(), dtbSale.get_Amount_sale());
            }
        }
    }



    [Test]
    public static void Test_getColDataTypes()
    {
        Dictionary<string, string> colDataTypes = new Dictionary<string, string>
        {
            {"fee.FEE_ID", "int unsigned"},
            {"fee.Date", "date"},
            {"fee.ApplicableTableID", "int unsigned"},
            {"fee.ApplicableTableName", "varchar(45)"},
            {"fee.Amount", "double unsigned"},
            {"fee.Type", "varchar(45)"},
            {"image.IMAGE_ID", "int unsigned"},
            {"image.image", "longblob"},
            {"image.ItemID", "int unsigned"},
            {"image.thumbnailID", "int unsigned"},
            {"item.ITEM_ID", "int unsigned"},
            {"item.Name", "varchar(255)"},
            {"item.PurchaseID", "int unsigned"},
            {"item.SaleID", "int unsigned"},
            {"item.ShippingID", "int unsigned"},
            {"item.InitialQuantity", "int unsigned"},
            {"item.CurrentQuantity", "int unsigned"},
            {"item.Notes_item", "mediumtext"},
            {"item.ThumbnailID", "int unsigned"},
            {"purchase.PURCHASE_ID", "int unsigned"},
            {"purchase.Date_Purchased", "date"},
            {"purchase.Amount_purchase", "double unsigned"},
            {"purchase.Tax", "double unsigned"},
            {"purchase.Fees_purchase", "double unsigned"},
            {"purchase.Seller", "varchar(255)"},
            {"purchase.Notes_purchase", "varchar(255)"},
            {"sale.SALE_ID", "int unsigned"},
            {"sale.Date_Sold", "date"},
            {"sale.Amount_sale", "double unsigned"},
            {"sale.Fees_sale", "double unsigned"},
            {"sale.Buyer", "varchar(255)"},
            {"sale.ItemID_sale", "int unsigned"},
            {"shipping.SHIPPING_ID", "int unsigned"},
            {"shipping.Length", "int unsigned"},
            {"shipping.Width", "int unsigned"},
            {"shipping.Height", "int unsigned"},
            {"shipping.Weight", "int unsigned"},
            {"shipping.ItemID_shipping", "int unsigned"},
            {"shipping.Notes_shipping", "varchar(255)"},
            {"thumbnail.ThumbnailID", "int unsigned"},
            {"thumbnail.thumbnail", "longblob"},
            {"shipping.WeightLbs", "int unsigned"},
            {"shipping.WeightOz", "int unsigned"},

        };
        Assert.AreEqual(colDataTypes, Database.getColDataTypes());
    }



    public static object[] getItemsSearchQueryCases = {

    // List<string> searchTerms,    string singleTerm,    Date startDate,        Date endDate,        bool inStock,        bool soldOut,        bool dateCol,        bool priceCol
    new SearchQuery(new List<string> { "Item" }, "", new Util.Date(1970, 1, 1), new Util.Date(2030, 1, 1), true, false, false, false)
    };
    [TestCaseSource(nameof(getItemsSearchQueryCases))]
    public static void Test_getItemsSearchQuery(SearchQuery Q) {
        List<Item> searchItems = Database.getItems(Q);
        Assert.AreEqual(getNumTestingItems(), searchItems.Count);
        Assert.IsTrue(hasUniqItems(searchItems));
    }



    [Test]
    public static void Test_getItemsItemID()
    {
        foreach (object[] item in testingItemsArrays)
        {
            string itemName = (string)item[0];
            int itemID = (int)item[7];
            int initQty = (int)item[3];
            int currQty = (int)item[4];
            List<Item> items = Database.getItems(itemID, true);
            Assert.IsTrue(items.Count == 1);
            Item result = items[0];

            Image image = Image.FromFile(imagePath);


            Assert.AreEqual(itemID, result.get_ITEM_ID());
            Assert.AreEqual(itemName, result.get_Name());
            Assert.AreEqual(initQty, result.get_InitialQuantity());
            Assert.AreEqual(currQty, result.get_CurrentQuantity());

            if (item[5] == null)
            {
                Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, result.get_Thumbnail().image, true));
            }
            else
            {
                Assert.IsTrue(TestingUtil.compareImages(image, result.get_Thumbnail().image, true));
            }
        }
    }


    [Test]
    public static void Test_runSaleSearchQuery()
    {
        List<Item> items = new List<Item>();
        foreach (object[] itemArr in testingItemsArrays)
        {
            Item item = new Item();
            item.set_ITEM_ID((int)itemArr[7]);
            List<Sale> itemSales = Database.runSaleSearchQuery(new Item());

            foreach (Sale sale in itemSales)
            {
                Assert.AreEqual((int)(((object[])itemArr[8])[0]), sale.get_Amount_sale());
                Assert.IsTrue(((Util.Date)((object[])itemArr[8])[1]).Equals(sale.get_Date_Sold()));
                Assert.AreEqual((int)(((object[])itemArr[8])[2]), sale.get_SALE_ID());
            }
        }
    }



    public static object[] runStatementCases = {

        new object[] {
        // CRUD

        //  Create
        new object[] { "INSERT INTO purchase (Amount_purchase, Date_Purchased) VALUES (199, DATE(\"2000-01-01\"));", false, true, "[]"},
        new object[] { "INSERT INTO item (Name, InitialQuantity, CurrentQuantity, PurchaseID) VALUES (\"TestingA\", 1, 1, purcID);", false, true, "[]"},

        //  Read
        new object[] { "SELECT * FROM item WHERE Name LIKE \"TestingA\";", true, false, "[[itemID, 'TestingA', purcID, None, None, 1, 1, None, None]]" },
        new object[] { "SELECT * FROM item WHERE Name LIKE \"TestingB\";", true, false, "[]" },
       
        //  Update
        new object[] { "UPDATE item SET Name = \"TestingB\" WHERE ITEM_ID = itemID;", false, false, "[]" },
        new object[] { "SELECT * FROM item WHERE Name LIKE \"TestingA\";", true, false, "[]" },
        new object[] { "SELECT * FROM item WHERE Name LIKE \"TestingB\";", true, false, "[[itemID, 'TestingB', purcID, None, None, 1, 1, None, None]]" },
        
        //  Delete
        new object[] { "DELETE FROM item WHERE ITEM_ID = itemID;", false, false, "[]"},
        new object[] { "SELECT * FROM item WHERE Name LIKE \"TestingB\";", true, false, "[]" },
        }

    };
    [TestCaseSource(nameof(runStatementCases))]
    public static void Test_runStatement(object[] statements)
    {
        List<string> colNames;
        int lastrowid;
        int insertID = -1;
        int purcID = -1;
        int count = -1;
        foreach (object[] row in statements)
        {
            count++;
            string statement = (string)row[0];
            bool needColNames = (bool)row[1];
            bool needLastRowID = (bool)row[2];
            string expResult = (string)row[3];
            string result;

            statement = statement.Replace("itemID", insertID.ToString());
            expResult = expResult.Replace("itemID", insertID.ToString());
            statement = statement.Replace("purcID", purcID.ToString());
            expResult = expResult.Replace("purcID", purcID.ToString());

            if (statement.Contains("purchase"))
            {
                result = Database.runStatement(statement, out colNames, out purcID);
            }

            else if (needColNames && needLastRowID)
                result = Database.runStatement(statement, out colNames, out insertID);
            else if (needColNames)
                result = Database.runStatement(statement, out colNames);
            else if (needLastRowID)
                result = Database.runStatement(statement, out insertID);
            else
                result = Database.runStatement(statement);


            Assert.AreEqual(0, expResult.CompareTo(result));
        }
    }



    public static object[] insertPurchaseCases = {
        new object[] { 100.0, "", new Util.Date(1978, 12, 16) },
        new object[] { 0.0, "", new Util.Date(1978, 12, 16) },
        new object[] { 999999999.9, "", new Util.Date(1978, 12, 16) },
    };

    [TestCaseSource(nameof(insertPurchaseCases))]
    public static void Test_insertPurchase(double purcPrice, string notes, Util.Date PurcDate)
    {
        Database.insertPurchase(purcPrice, notes, PurcDate);
        Assert.IsTrue(true);
    }



    public static object[] insertPurchaseCasesEXCEPTIONS = {
        new object[] { -1.0, "", new Util.Date(1978, 12, 16) }
    };

    [TestCaseSource(nameof(insertPurchaseCasesEXCEPTIONS))]
    public static void Test_insertPurchaseEXCEPTIONS(double purcPrice, string notes, Util.Date PurcDate)
    {
        Assert.Catch<Exception>(() => Database.insertPurchase(purcPrice, notes, PurcDate));
    }



    [Test]
    public static void Test_insertItem()
    {
        string name = "INSERT_ITEM_TEST";
        foreach (object[] item in testingItemsArrays)
        {
            int purcID = Database.insertPurchase((double)item[1], "", (Util.Date)item[2]);
            Item newItem = new Item();
            newItem.set_Name(name);
            newItem.set_InitialQuantity((int)item[3]);
            newItem.set_CurrentQuantity((int)item[4]);
            newItem.set_PurchaseID(purcID);
            Assert.AreNotEqual(0, Database.insertItem(newItem).CompareTo("['ERROR']"));
        }

        Database.runStatement("DELETE FROM item WHERE Name LIKE \"" + name + "\"");
    }



    [Test]
    public static void Test_insertItemLastrowid()
    {
        string name = "INSERT_ITEM_TEST";
        foreach (object[] item in testingItemsArrays)
        {
            int lastrowid = -2;

            int purcID = Database.insertPurchase((double)item[1], "", (Util.Date)item[2]);
            Item newItem = new Item();
            newItem.set_Name(name);
            newItem.set_InitialQuantity((int)item[3]);
            newItem.set_CurrentQuantity((int)item[4]);
            newItem.set_PurchaseID(purcID);
            Assert.AreNotEqual(0, Database.insertItem(newItem, out lastrowid).CompareTo("['ERROR']"));
            Assert.AreNotEqual(-2, lastrowid); // Unchanged Value
            Assert.AreNotEqual(-1, lastrowid); // Possible Error
        }

        Database.runStatement("DELETE FROM item WHERE Name LIKE \"" + name + "\"");
    }



    [Test]
    public static void Test_insertSale()
    {
        string name = "INSERT_ITEM_TEST";
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        foreach (object[] item in testingItemsArrays)
        {
            int lastrowid = -2;

            object[] salesArr = (object[])item[8];

            foreach (object[] saleArr in salesArr)
            {

                Sale sale = new Sale();
                sale.set_Date_Sold((Util.Date)saleArr[1]);
                sale.set_Amount_sale((double)saleArr[0]);
                sale.set_ItemID_sale((int)item[7]);

                Assert.AreNotEqual(0, Database.insertSale(sale).CompareTo("['ERROR']"));
            }
        }
    }



    [Test]
    public static void Test_insertSaleLastrowid()
    {
        string name = "INSERT_ITEM_TEST";
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        foreach (object[] item in testingItemsArrays)
        {
            int lastrowid = -2;

            object[] salesArr = (object[])item[8];

            foreach (object[] saleArr in salesArr)
            {

                Sale sale = new Sale();
                sale.set_Date_Sold((Util.Date)saleArr[1]);
                sale.set_Amount_sale((double)saleArr[0]);
                sale.set_ItemID_sale((int)item[7]);

                Assert.AreNotEqual(0, Database.insertSale(sale, out lastrowid).CompareTo("['ERROR']"));
                Assert.AreNotEqual(-2, lastrowid); // Unchanged Value
                Assert.AreNotEqual(-1, lastrowid); // Possible Error
            }
        }
    }



    [Test]
    public static void Test_insertImage()
    {
        string[] images = Directory.GetFiles(imagesDir);
        foreach (string imagePath in images)
        {
            int itemID = (int)((object[])testingItemsArrays[0])[7];
            Item item = new Item();
            item.set_ITEM_ID(itemID);

            Database.deleteImages(item);
            Database.insertImage(imagePath, itemID);
            List<MyImage> dtbImages = Database.getAllImages(item);
            Assert.AreEqual(1, dtbImages.Count);

            Image resultImage = dtbImages[0].image;
            Image expectImage = Image.FromFile(imagePath);

            Assert.IsTrue(TestingUtil.compareImages(resultImage, expectImage, false));

            Database.deleteImages(item);
        }
    }



    [Test]
    public static void Test_deleteItem()
    {
        string name = "DELETE_ITEM_TEST";
        foreach (object[] item in testingItemsArrays)
        {
            // Set up new item to delete
            int purcID = Database.insertPurchase((double)item[1], "", (Util.Date)item[2]);
            Item newItem = new Item();
            newItem.set_Name(name);
            newItem.set_InitialQuantity((int)item[3]);
            newItem.set_CurrentQuantity((int)item[4]);
            newItem.set_PurchaseID(purcID);
            int itemID;
            Assert.AreNotEqual(0, Database.insertItem(newItem, out itemID).CompareTo("['ERROR']"));




            Item itemToDelete = Database.getItem(itemID);
            Database.deleteItem(itemToDelete);

            Assert.Catch<Exception>(() => Database.getItem(itemToDelete.get_ITEM_ID()));
        }

    }



    [Test]
    public static void Test_deleteImages()
    {
        foreach (object[] itemArr in testingItemsArrays)
        {
            // Setup to give item exactly 1 image
            int itemID = (int)((object[])itemArr)[7];
            Item item = new Item();
            item.set_ITEM_ID(itemID);
            Database.deleteImages(item);

            Database.insertImage(imagePath, itemID);
            List<MyImage> dtbImages = Database.getAllImages(item);
            Assert.AreEqual(1, dtbImages.Count);

            Database.deleteImages(item);
            dtbImages = Database.getAllImages(item);
            Assert.AreEqual(1, dtbImages.Count);
            Assert.IsTrue(TestingUtil.compareImages(dtbImages[0].image, Util.DEFAULT_IMAGE.image, false));

            Database.insertImage(imagePath, itemID);
            Database.insertImage(imagePath, itemID);
            dtbImages = Database.getAllImages(item);
            Assert.AreEqual(2, dtbImages.Count);

            Database.deleteImages(item);
            dtbImages = Database.getAllImages(item);
            Assert.AreEqual(1, dtbImages.Count);
            Assert.IsTrue(TestingUtil.compareImages(dtbImages[0].image, Util.DEFAULT_IMAGE.image, false));
        }
        Database.clearAll();
        TestDatabase.addTestingItems();
    }



    [Test]
    public static void Test_deleteShipInfo()
    {
        
        foreach (object[] itemArr in testingItemsArrays)
        {
            int itemID = (int)itemArr[7];
            Item item = new Item();
            item.set_ITEM_ID(itemID);


            int l = Math.Abs(rand.Next(2000000000));
            int w = Math.Abs(rand.Next(2000000000));
            int h = Math.Abs(rand.Next(2000000000));
            int oz = Math.Abs(rand.Next(65)); // Don't want potential overflow adding oz and lbs together
            int lbs = Math.Abs(rand.Next(2000000000 / 16)); // Eventually get stored as ounces so will get multiplied by 16

            Database.deleteShipInfo(item);
            // Confirm no prev shipping info exists for the item
            Assert.AreEqual(Util.DEFAULT_INT, Database.getItem(itemID).get_Weight());
            Database.insertShipInfo(item, lbs, oz, l, w, h);

            Item itemWithShipInfo = Database.getItem(item.get_ITEM_ID());

            // check that it has shipping info
            Assert.AreEqual(oz + 16*lbs, itemWithShipInfo.get_Weight());

            Database.deleteShipInfo(itemWithShipInfo);

            Item itemWithoutShipInfo = Database.getItem(item.get_ITEM_ID());

            Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Weight());
            Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Length());
            Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Width());
            Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Height());


        }
    }



    [Test]
    public static void Test_getAllImages()
    {
        int maxImages = 10;
        int imgGroupSize = 4;
        Item item = new Item();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        item.set_ITEM_ID(itemID);
        string[] images = Directory.GetFiles(imagesDir);
        for (int i = 0; i < Math.Min(images.Length, maxImages) - imgGroupSize; i += imgGroupSize)
        {
            // Initial clearing of images
            Database.deleteImages(item);
            Assert.AreEqual(0, Database.getAllImages(item).Count);

            // Insert images
            for (int j = 0; j < imgGroupSize; j++)
            {
                Database.insertImage(images[i+j], item.get_ITEM_ID());
            }
            List<MyImage> resultImages = Database.getAllImages(item);
            // Test all images are there correctly
            for (int k = 0; k < imgGroupSize; k++)
            {
                Assert.IsTrue(TestingUtil.compareImages(Image.FromFile(images[i+k]),
                                                                 resultImages[i+k].image,
                                                                                 false));
            }
        }
    }



    [TestCase("item.Name", "")]
    [TestCase("item.Name", "123")]
    [TestCase("item.Notes_item", "2bc")]
    [TestCase("item.Notes_item", "3ab")]
    public static void Test_updateRowString(string attrib, string newVal)
    {
        Item itemToChange = new Item();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        itemToChange.set_ITEM_ID(itemID);
        Database.updateRow(itemToChange, attrib, newVal);

        Item changedItem = Database.getItem(itemToChange.get_ITEM_ID());

        changedItem.getAttribAsStr(attrib, out string changedAttrib);

        Assert.AreEqual(0, newVal.CompareTo(changedAttrib));
    }



    [TestCase("purchase.Amount_purchase", 3.14)]
    public static void Test_updateRowDouble(string attrib, double newVal)
    {
        Item itemToChange = new Item();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        int purcID = (int)((object[])testingItemsArrays[0])[9];
        itemToChange.set_ITEM_ID(itemID);
        itemToChange.set_PurchaseID(purcID);
        Database.updateRow(itemToChange, attrib, newVal);

        Item changedItem = Database.getItem(itemToChange.get_ITEM_ID());

        changedItem.getAttribAsStr(attrib, out string changedAttrib);

        Assert.AreEqual(newVal, Double.Parse(changedAttrib));
    }



    [TestCase("item.InitialQuantity", 5)]
    public static void Test_updateRowInt(string attrib, int newVal)
    {
        Item itemToChange = new Item();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        itemToChange.set_ITEM_ID(itemID);
        Database.updateRow(itemToChange, attrib, newVal);

        Item changedItem = Database.getItem(itemToChange.get_ITEM_ID());

        changedItem.getAttribAsStr(attrib, out string changedAttrib);

        Assert.AreEqual(newVal, Int32.Parse(changedAttrib));
    }



    public static object[] updateRowDateCases =
    {
        new object[] { "purchase.Date_Purchased", new Util.Date(1979,12,1) },
    };
    [TestCaseSource(nameof(updateRowDateCases))]
    public static void Test_updateRowDate(string attrib, Util.Date newVal)
    {
        Item itemToChange = new Item();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        int purcID = (int)((object[])testingItemsArrays[0])[9];
        itemToChange.set_ITEM_ID(itemID);
        itemToChange.set_PurchaseID(purcID);
        Database.updateRow(itemToChange, attrib, newVal);

        Item changedItem = Database.getItem(itemToChange.get_ITEM_ID());

        changedItem.getAttribAsStr(attrib, out string changedAttrib);

        Assert.IsTrue(newVal.Equals(new Util.Date(changedAttrib)));
    }



    [TestCase("sale.Buyer", "Me")]
    public static void Test_updateRowStringSale(string attrib, string newVal)
    {
        Sale saleToChange = new Sale();
        int saleID = (int)((object[])((object[])((object[])testingItemsArrays[0])[8])[0])[2];
        //int saleID = (int)((object[])((object[])((object[])testingItemsArrays[0])[8])[0])[2];
        saleToChange.set_SALE_ID(saleID);
        Database.updateRow(saleToChange, attrib, newVal);

        Sale changedSale = Database.getSale(saleToChange.get_SALE_ID());

        string changedAttrib = "";
        changedSale.getAttribAsStr(attrib, ref changedAttrib);

        Assert.AreEqual(0, newVal.CompareTo(changedAttrib));
    }



    public static object[] updateRowDateCasesSale =
    {
        new object[] { "sale.Buyer", new Util.Date(1979,12,1) },
    };
    [TestCaseSource(nameof(updateRowDateCasesSale))]
    public static void Test_updateRowDateSale(string attrib, Util.Date newVal)
    {
        Sale saleToChange = new Sale();
        int saleID = (int)((object[])((object[])((object[])testingItemsArrays[0])[8])[0])[2];
        saleToChange.set_SALE_ID(saleID);
        Database.updateRow(saleToChange, attrib, newVal);

        Sale changedSale = Database.getSale(saleToChange.get_SALE_ID());

        string changedAttrib = "";
        changedSale.getAttribAsStr(attrib, ref changedAttrib);

        Assert.IsTrue(newVal.Equals(new Util.Date(changedAttrib)));
    }



    [Test]
    public static void Test_insertShipInfo()
    {
        foreach (object[] itemArr in testingItemsArrays)
        {
            int itemID = (int)itemArr[7];
            Item item = new Item();
            item.set_ITEM_ID(itemID);


            int l = Math.Abs(rand.Next(2000000000));
            int w = Math.Abs(rand.Next(2000000000));
            int h = Math.Abs(rand.Next(2000000000));
            int oz = Math.Abs(rand.Next(65)); // Don't want potential overflow adding oz and lbs together
            int lbs = Math.Abs(rand.Next(2000000000 / 16)); // Eventually get stored as ounces so will get multiplied by 16

            Database.deleteShipInfo(item);
            // Confirm no prev shipping info exists for the item
            Assert.AreEqual(Util.DEFAULT_INT, Database.getItem(itemID).get_Weight());
            Database.insertShipInfo(item, lbs, oz, l, w, h);

            Item itemWithShipInfo = Database.getItem(item.get_ITEM_ID());
            Assert.AreEqual(l, itemWithShipInfo.get_Length());
            Assert.AreEqual(w, itemWithShipInfo.get_Width());
            Assert.AreEqual(h, itemWithShipInfo.get_Height());
            Assert.AreEqual(oz + 16 * lbs, itemWithShipInfo.get_Weight());
        }
    }



    [Test]
    public static void Test_deleteSale()
    {
        Sale sale = new Sale();
        int itemID = (int)((object[])testingItemsArrays[0])[7];
        sale.set_Date_Sold(new Util.Date(1978,12,16));
        sale.set_Amount_sale(100.0);
        sale.set_ItemID_sale(itemID);
        Database.insertSale(sale, out int saleID);

        // Make sure sale exists in database
        Database.getSale(saleID); 

        sale.set_SALE_ID(saleID);

        Database.deleteSale(sale);

        Assert.Catch<Exception> (() => Database.getSale(saleID));
    }



    [Test]
    public static void Test_getPurchase()
    {
        foreach (object[] itemArr in testingItemsArrays)
        {
            Item item = new Item();
            int itemID = (int)itemArr[7];
            item.set_ITEM_ID(itemID);
            item.set_PurchaseID((int) itemArr[9]);

            Purchase purchase = Database.getPurchase(item);
            Assert.AreEqual((double) itemArr[1], purchase.amount);
        }
    }



    [Test]
    public static void Test_setThumbnail()
    {
        foreach (object[] itemArr in testingItemsArrays)
        {
            int itemID = (int)itemArr[7];
            Item item = Database.getItem(itemID);
            Database.deleteImages(item);
            item = Database.getItem(itemID);
            Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, item.get_Thumbnail().image, true));
            int imageID = Database.insertImage(imagePath, itemID);
            Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, item.get_Thumbnail().image, true));
            Database.setThumbnail(itemID, imageID);
        }
    }

}
