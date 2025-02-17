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
using System.Runtime.InteropServices.Marshalling;
using FinancialDatabase.Tabs;
using System.Xml.Linq;

public class TestDatabase
{
    private static Random rand = new Random();

    private static List<int> itemIDsGeneric = new List<int> { };
    private static List<int> itemIDsSamePurc = new List<int> { };

    private static string imagesDir = @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabaseTesting\TestingItems\Images";

    private static string imagePath = @"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\s-l1600.png";

    static List<Purchase> purchases;

    static Form1 form1;
    static TabController tabController;



    public static void insertAllTestingItems()
    {
        purchases = TestingUtil.getTestingItems();
        TestingUtil.setDatabaseTesting(true);
        TestingUtil.setTabTesting(true);
        Database.clearAll();
        Database.getColDataTypes();

        purchases = TestingUtil.getTestingItems();
        foreach (Purchase purchase in purchases)
        {
            purchase.PURCHASE_ID = Database.insertPurchase(purchase.Amount_purchase, purchase.Notes_purchase, new Util.Date(purchase.Date_Purchased_str));

            foreach (Item item in purchase.items)
            {
                item.set_PurchaseID(purchase.PURCHASE_ID);
                Database.insertItem(item, out int itemID);
                item.set_ITEM_ID(itemID);
                if (item.get_Length() != Util.DEFAULT_INT)
                {
                    int shipID = Database.insertShipInfo(item);
                    item.set_ShippingID(shipID);
                }
                if (item.sales != null)
                {
                    foreach (Sale sale in item.sales)
                    {
                        sale.set_ItemID_sale(item.get_ITEM_ID());
                        Database.insertSale(sale, out int saleID);
                        sale.set_SALE_ID(saleID);
                    }
                }
            }
        }
    }


    [OneTimeSetUp]
    public static void OneTimeSetup()
    {
        TestingUtil.setDatabaseTesting(true);
        Database.clearAll();

    }


    [SetUp]
    public static void SetUp()
    {
        insertAllTestingItems();
        form1 = new Form1();
        tabController = form1.tabControl;
        tabController.clearSearchItems();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Database.clearAll();
        TestingUtil.setDatabaseTesting(false);
    }

    [TearDownAttribute]
    public void tearDownAttribute()
    {
        form1.Dispose();
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
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                int itemID = item.get_ITEM_ID();
                Item actualItem = Database.getItem(itemID);
                MyAssert.ItemsEqual(item, actualItem);
            }
        }
    }



    [Test]
    public static void Test_getPurchItems()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                List<Item> purcItems = Database.getPurchItems(item);


                // Check proper Amount_purchase of items returned
                Assert.AreEqual(purchase.items.Count(), purcItems.Count());

                // Check all items are from the same purchase
                for (int j = 0; j < purcItems.Count(); j++)
                {
                    Assert.AreEqual(purcItems[0].get_PurchaseID(), purcItems[j].get_PurchaseID());
                }
                Assert.AreEqual(purcItems[0].get_PurchaseID(), item.get_PurchaseID());
            }
        }
    }



    [Test]
    public static void Test_getSale()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                foreach (Sale sale in item.sales)
                {
                    Sale actualSale = Database.getSale(sale.get_SALE_ID());

                    Assert.AreEqual(sale.get_Date_Sold(), actualSale.get_Date_Sold());
                    Assert.AreEqual(sale.get_Amount_sale(), actualSale.get_Amount_sale());
                }
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
    new SearchQuery(new List<string> {  }, "ItemA", new Util.Date(1970, 1, 1), new Util.Date(2030, 1, 1), true, false, false, false)
    };
    [TestCaseSource(nameof(getItemsSearchQueryCases))]
    public static void Test_getItemsSearchQuery(SearchQuery Q) {
        List<Item> searchItems = Database.getItems(Q);
        Assert.AreEqual(1, searchItems.Count());
        Assert.IsTrue(hasUniqItems(searchItems));
    }



    [Test]
    public static void Test_getItemsItemID()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                List<Item> items = Database.getItems(item.get_ITEM_ID(), true);

                Assert.IsTrue(items.Count == 1);
                Item actualItem = items[0];

                Assert.AreEqual(item.get_ITEM_ID(), actualItem.get_ITEM_ID());
                Assert.AreEqual(item.get_Name(), actualItem.get_Name());
                Assert.AreEqual(item.get_Amount_purchase(), actualItem.get_Amount_purchase());
                Assert.AreEqual(item.get_Date_Purchased(), actualItem.get_Date_Purchased());
                Assert.AreEqual(item.get_InitialQuantity(), actualItem.get_InitialQuantity());
                Assert.AreEqual(item.get_CurrentQuantity(), actualItem.get_CurrentQuantity());

                if (item.hasShippingEntry())
                {
                    Assert.AreEqual(item.get_Weight(), actualItem.get_Weight());
                    Assert.AreEqual(item.get_Length(), actualItem.get_Length());
                    Assert.AreEqual(item.get_Height(), actualItem.get_Height());
                    Assert.AreEqual(item.get_Width(), actualItem.get_Width());
                }
                for (int i = 0; i < item.sales.Count(); i++)
                {
                    MyAssert.StringsEqual(item.sales[i].get_Date_Sold().toDateString(), actualItem.sales[i].get_Date_Sold().toDateString());
                    Assert.AreEqual(item.sales[i].get_Amount_sale(), actualItem.sales[i].get_Amount_sale());
                }
            }
        }
    }


    [Test]
    public static void Test_runSaleSearchQuery()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                List<Sale> actualSales = Database.runSaleSearchQuery(item);

                for (int i = 0; i < item.sales.Count(); i++)
                {
                    Sale expectedSale = item.sales[i];
                    Sale actualSale = actualSales[i];

                    Assert.AreEqual(expectedSale.get_Amount_sale(), actualSale.get_Amount_sale());
                    Assert.IsTrue(expectedSale.get_Date_Sold().Equals(actualSale.get_Date_Sold()));
                    Assert.AreEqual(expectedSale.get_SALE_ID(), actualSale.get_SALE_ID());
                }
            }
        }
    }



    /*public static object[] runStatementCases = {

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
    */


    public static object[] insertPurchaseCases = {
        new object[] { 100.0, "", new Util.Date(1978, 12, 16) },
        new object[] { 0.0, "", new Util.Date(1978, 12, 16) },
        new object[] { 999999999.9, "", new Util.Date(1978, 12, 16) },
    };

    [TestCaseSource(nameof(insertPurchaseCases))]
    public static void Test_insertPurchase(double purcPrice, string notes, Util.Date PurcDate)
    {
        Purchase expectedPurchase = new Purchase();
        expectedPurchase.set_notes_purchase(notes);
        expectedPurchase.Amount_purchase = purcPrice;
        expectedPurchase.Date_Purchased = PurcDate;

        int purcID = Database.insertPurchase(purcPrice, notes, PurcDate);

        expectedPurchase.PURCHASE_ID = purcID;

        Purchase actualPurchase = Database.getPurchase(purcID);

        Assert.AreEqual(expectedPurchase.PURCHASE_ID, actualPurchase.PURCHASE_ID);
        Assert.AreEqual(expectedPurchase.Amount_purchase, actualPurchase.Amount_purchase);
        Assert.AreEqual(expectedPurchase.Date_Purchased.toDateString(), actualPurchase.Date_Purchased.toDateString());
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
        string testingName = "INSERT_ITEM_TEST";

        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                string origName = item.get_Name();
                item.set_Name(testingName);

                string insertResult = Database.insertItem(item);

                Assert.AreNotEqual(0, insertResult.CompareTo("['ERROR']"));

                // Assume no errors
                // No other way to test without getting purcID returned which is tested elsewhere
                Assert.Pass();

                item.set_Name(origName);
                Database.deleteByName(testingName);
            }
        }
    }



    [Test]
    public static void Test_insertItemLastrowid()
    {
        string testingName = "INSERT_ITEM_TEST";
        string result = "";
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                int origItemID = item.get_ITEM_ID();
                string origName = item.get_Name();
                item.set_Name(testingName);

                result = Database.insertItem(item, out int itemID);

                Assert.AreNotEqual(0, result.CompareTo("['ERROR']"));
                item.set_ITEM_ID(itemID);


                Item actualItem = Database.getItem(itemID);

                MyAssert.ItemsEqualWithoutSales(item, actualItem);

                item.set_ITEM_ID(origItemID);
                item.set_Name(origName);
                Database.deleteByName(testingName);
            }
        }
    }



    [Test]
    public static void Test_insertSale()
    {
        Item testingItem = new Item();
        testingItem.set_Name("TestingItem");
        testingItem.set_InitialQuantity(1);
        testingItem.set_CurrentQuantity(1);
        testingItem.set_PurchaseID(purchases[0].PURCHASE_ID);

        Database.insertItem(testingItem, out int tempItemID);
        Item tempItem = Database.getItem(tempItemID);
        string result = "";
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                foreach (Sale sale in item.sales)
                {
                    Sale newSale = new Sale();

                    newSale.set_ItemID_sale(tempItemID);
                    newSale.set_Date_Sold(sale.get_Date_Sold());
                    newSale.set_Amount_sale(sale.get_Amount_sale());
                    Assert.AreNotEqual(0, Database.insertSale(newSale).CompareTo("['ERROR']"));
                }
            }
        }
        Database.deleteItem(tempItem);
        Assert.Catch<Exception>(() => Database.getItem(tempItem.get_ITEM_ID()));
    }



    [Test]
    public static void Test_insertSaleLastrowid()
    {
        Item testingItem = new Item();
        testingItem.set_Name("TestingItem");
        testingItem.set_InitialQuantity(1);
        testingItem.set_CurrentQuantity(1);
        testingItem.set_PurchaseID(purchases[0].PURCHASE_ID);

        Database.insertItem(testingItem, out int tempItemID);
        Item tempItem = Database.getItem(tempItemID);
        string result = "";
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                foreach (Sale sale in item.sales)
                {
                    Sale newSale = new Sale();

                    newSale.set_ItemID_sale(tempItemID);
                    newSale.set_Date_Sold(sale.get_Date_Sold());
                    newSale.set_Amount_sale(sale.get_Amount_sale());
                    Assert.AreNotEqual(0, Database.insertSale(newSale, out int lastrowid).CompareTo("['ERROR']"));
                    Assert.AreNotEqual(-2, lastrowid); // Unchanged Value
                    Assert.AreNotEqual(-1, lastrowid); // Possible Error
                }
            }
        }
        Database.deleteItem(tempItem);
        Assert.Catch<Exception>(() => Database.getItem(tempItem.get_ITEM_ID()));
    }



    [Test]
    public static void Test_insertImage()
    {
        string[] images = Directory.GetFiles(imagesDir);
        foreach (string imagePath in images)
        {
            int itemID = purchases[0].items[0].get_ITEM_ID();
            Item item = Database.getItem(itemID);

            Database.deleteImages(item);
            Database.insertImage(imagePath, purchases[0].items[0]);
            List<MyImage> dtbImages = Database.getAllImages(item);
            Assert.AreEqual(1, dtbImages.Count);

            Image resultImage = dtbImages[0].image;
            Image expectImage = Image.FromFile(imagePath);

            Assert.IsTrue(TestingUtil.compareImages(resultImage, expectImage, false));

            Database.deleteImages(item);
            Assert.AreEqual(0, Database.getImages(item).Count());
        }
    }



    [Test]
    public static void Test_deleteItem()
    {
        string name = "DELETE_ITEM_TEST";
        foreach (Purchase purchase in purchases)
        {
            int purcID = Database.insertPurchase(purchase.Amount_purchase, "", purchase.Date_Purchased);

            foreach (Item item in purchase.items)
            {
                string oldName = item.get_Name();
                item.set_Name(name);
                Assert.AreNotEqual(0, Database.insertItem(item, out int itemID).CompareTo("['ERROR']"));
                item.set_Name(oldName);

                Item itemToDelete = Database.getItem(itemID);
                Database.deleteItem(itemToDelete);

                Assert.Catch<Exception>(() => Database.getItem(itemToDelete.get_ITEM_ID()));
            }
        }

    }



    [Test]
    public static void Test_deleteImages()
    {
        int purcID = Database.insertPurchase(purchases[0].Amount_purchase, "", purchases[0].Date_Purchased);

        Item item = purchases[0].items[0];

        int itemID = item.get_ITEM_ID();
        Database.deleteImages(item);

        // Try inserting 1 image
        Database.insertImage(imagePath, purchases[0].items[0]);
        List<MyImage> dtbImages = Database.getAllImages(item);
        Assert.AreEqual(1, dtbImages.Count);

        // Delete images
        Database.deleteImages(item);
        dtbImages = Database.getAllImages(item);
        Assert.AreEqual(1, dtbImages.Count);
        Assert.IsTrue(TestingUtil.compareImages(dtbImages[0].image, Util.DEFAULT_IMAGE.image, false));

        // Try inserting 2 images
        Database.insertImage(imagePath, purchases[0].items[0]);
        Database.insertImage(imagePath, purchases[0].items[0]);
        dtbImages = Database.getAllImages(item);
        Assert.AreEqual(2, dtbImages.Count);

        // Delete images
        Database.deleteImages(item);
        dtbImages = Database.getAllImages(item);
        Assert.AreEqual(1, dtbImages.Count);
        Assert.IsTrue(TestingUtil.compareImages(dtbImages[0].image, Util.DEFAULT_IMAGE.image, false));
    }



    [Test]
    public static void Test_deleteShipInfo()
    {
        foreach (Purchase purchase in purchases)
        {
            int purcID = Database.insertPurchase(purchase.Amount_purchase, "", purchase.Date_Purchased);

            foreach (Item item in purchase.items)
            {
                int itemID = item.get_ITEM_ID();
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
                Assert.AreEqual(oz + 16 * lbs, itemWithShipInfo.get_Weight());

                Database.deleteShipInfo(itemWithShipInfo);

                Item itemWithoutShipInfo = Database.getItem(item.get_ITEM_ID());

                Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Weight());
                Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Length());
                Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Width());
                Assert.AreEqual(Util.DEFAULT_INT, itemWithoutShipInfo.get_Height());

            }
        }
    }



    [Test]
    public static void Test_getAllImages()
    {
        int maxImages = 10;
        int imgGroupSize = 4;
        Item item = new Item();
        int itemID = purchases[0].items[0].get_ITEM_ID();
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
                Database.insertImage(images[i+j], item);
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
        Database.deleteImages(item);
    }



    [TestCase("item.Name", "123")]
    [TestCase("item.Notes_item", "2bc")]
    [TestCase("purchase.Seller", "2bc")]
    [TestCase("purchase.Notes_purchase", "3ab")]
    [TestCase("shipping.Notes_shipping", "4ab")]
    public static void Test_updateRowString(string attrib, string newVal)
    {
        
        Item item = purchases[0].items[0];


        Database.updateRow(item, attrib, newVal);

        Item changedItem = Database.getItem(item.get_ITEM_ID());
        string changedAttrib = changedItem.getAttribAsStr(attrib);

        Assert.AreEqual(0, newVal.CompareTo(changedAttrib));
    }



    [TestCase("purchase.Amount_purchase", 3.14)]
    [TestCase("purchase.Tax", 3.15)]
    [TestCase("purchase.Fees_purchase", 3.16)]
    public static void Test_updateRowDouble(string attrib, double newVal)
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                Database.updateRow(item, attrib, newVal);

                Item changedItem = Database.getItem(item.get_ITEM_ID());

                string changedAttrib = changedItem.getAttribAsStr(attrib);

                Assert.AreEqual(newVal, Double.Parse(changedAttrib));
            }
        }
        
    }



    /*[TestCase("item.ITEM_ID", 1)]   Shouldn't try and update foreign key
    [TestCase("item.PurchaseID", 2)]
    [TestCase("item.ShippingID", 4)]*/
    [TestCase("item.InitialQuantity", 5)]
    [TestCase("item.CurrentQuantity", 6)]
    [TestCase("shipping.Length", 7)]
    [TestCase("shipping.Width", 8)]
    [TestCase("shipping.Height", 9)]
    [TestCase("shipping.Width", 10)]
    public static void Test_updateRowInt(string attrib, int newVal)
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                // If default attrib, assume no database row entry exists containg the attrib
                if (item.isDefaultValue(attrib))
                {
                    continue;
                }
                Database.updateRow(item, attrib, newVal);
                Item changedItem = Database.getItem(item.get_ITEM_ID());
                string changedAttrib = "-999";
                try
                {
                    changedAttrib = changedItem.getAttribAsStr(attrib);
                }
                catch (Exception ex)
                {
                    // If attribute does not exist in item to begin with, pass the test case by default
                    if (ex.Message == "ERROR: Unknown attribute: " + attrib)
                    {
                        Assert.Pass();
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
                Assert.AreEqual(newVal, Int32.Parse(changedAttrib));
            }

        }
    }



    public static object[] updateRowDateCases =
    {
        new object[] { "purchase.Date_Purchased", new Util.Date(1979,12,1) },
    };
    [TestCaseSource(nameof(updateRowDateCases))]
    public static void Test_updateRowDate(string attrib, Util.Date newVal)
    {
        Item itemToChange = purchases[0].items[0];
        int itemID = itemToChange.get_ITEM_ID();
        int purcID = itemToChange.get_PurchaseID();
        itemToChange.set_ITEM_ID(itemID);
        itemToChange.set_PurchaseID(purcID);
        Database.updateRow(itemToChange, attrib, newVal);

        Item changedItem = Database.getItem(itemToChange.get_ITEM_ID());

        string changedAttrib = changedItem.getAttribAsStr(attrib);

        Assert.IsTrue(newVal.Equals(new Util.Date(changedAttrib)));
    }



    [TestCase("sale.Buyer", "Me")]
    public static void Test_updateRowStringSale(string attrib, string newVal)
    {
        Sale saleToChange = new Sale();
        int saleID = purchases[0].items[0].sales[0].get_SALE_ID();
        saleToChange.set_SALE_ID(saleID);
        Database.updateRow(saleToChange, attrib, newVal);

        Sale changedSale = Database.getSale(saleToChange.get_SALE_ID());

        string changedAttrib = changedSale.getAttribAsStr(attrib);

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
        int saleID = purchases[0].items[0].sales[0].get_SALE_ID();
        saleToChange.set_SALE_ID(saleID);
        Database.updateRow(saleToChange, attrib, newVal);

        Sale changedSale = Database.getSale(saleToChange.get_SALE_ID());

        string changedAttrib = changedSale.getAttribAsStr(attrib);

        Assert.IsTrue(newVal.Equals(new Util.Date(changedAttrib)));
    }



    [Test]
    public static void Test_insertShipInfo()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                int itemID = item.get_ITEM_ID();
                int purcID = item.get_PurchaseID();
                item.set_ITEM_ID(itemID);
                item.set_PurchaseID(purcID);
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
    }



    [Test]
    public static void Test_deleteSale()
    {
        int itemID = purchases[0].items[0].get_ITEM_ID();
        Sale sale = new Sale();
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
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                Assert.AreEqual(item.get_Amount_purchase(), Database.getPurchase(item).Amount_purchase);
            }
        }
    }



    [Test]
    public static void Test_getPurchasePurchaseID()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                int purcID = item.get_PurchaseID();
                Assert.AreEqual(item.get_Amount_purchase(), Database.getPurchase(purcID).Amount_purchase);
            }
        }
    }



    [Test]
    public static void Test_setThumbnail()
    {
        foreach (Purchase purchase in purchases)
        {
            foreach (Item item in purchase.items)
            {
                int itemID = item.get_ITEM_ID();
                Database.deleteImages(item);
                Item newItem = Database.getItem(itemID);
                Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, newItem.get_Thumbnail().image, true));
                int imageID = Database.insertImage(imagePath, item);
                Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, newItem.get_Thumbnail().image, true));
                Database.setThumbnail(itemID, imageID);
                newItem = Database.getItem(itemID);
                Assert.IsTrue(TestingUtil.compareImages(Image.FromFile(imagePath), newItem.get_Thumbnail().image, true));
            }
        }
    }

}
