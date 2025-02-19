using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using FinancialDatabase.Tabs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinancialDatabaseTesting
{
    public class TestTabController
    {

        public static Form1 form1;
        public static TabController tabController;
        private static List<Purchase> purchases;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
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
                    item.PurchaseID = purchase.PURCHASE_ID;
                    Database.insertItem(item, out int itemID);
                    item.ITEM_ID = itemID;
                    if (item.Length != Util.DEFAULT_INT)
                    {
                        int shipID = Database.insertShipInfo(item);
                        item.ShippingID = shipID;
                    }
                    if (item.sales != null)
                    {
                        foreach (Sale sale in item.sales)
                        {
                            sale.set_ItemID_sale(item.ITEM_ID);
                            Database.insertSale(sale, out int saleID);
                            sale.set_SALE_ID(saleID);
                        }
                    }
                }
            }
        }



        [SetUp]
        public static void SetUp()
        {
            form1 = new Form1();
            tabController = form1.tabControl;
            tabController.clearSearchItems();
        }


        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            form1.Dispose();
            TestingUtil.setDatabaseTesting(false);
            TestingUtil.setTabTesting(false);
        }


        [TearDown]
        public static void TearDown()
        {
            form1.Dispose();
        }


        [Test]
        public static void Test_ItemViewTab()
        {
            Assert.IsNull(tabController.getCurrItem());
        }



        [Test]
        public static void Test_getCurrItemNull()
        {
            tabController.setCurrItem(null);
            Assert.IsNull(tabController.getCurrItem());
        }



        [Test]
        public static void Test_getCurrItem()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    setCurrItem(item);
                    Assert.AreEqual(item.ITEM_ID, tabController.getCurrItem().ITEM_ID);
                }
            }
        }



        [Test]
        public static void Test_setCurrItem()
        {
            
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    setCurrItem(item);

                    // ItemViewTab
                    MyAssert.LabelTextAreEqual(item.Name, form1.itemNameTLP.getLabelText());
                    MyAssert.LabelTextAreEqual(item.Date_Purchased, form1.itemDatePurcLbl.Text);
                    MyAssert.LabelTextAreEqual(item.Amount_purchase, form1.itemPurcPriceLbl.Text);
                    MyAssert.LabelTextAreEqual(item.getTotalSales(), form1.itemSoldPriceLbl.Text);
                    MyAssert.LabelTextAreEqual(item.InitialQuantity, form1.itemInitQtyTLP.getLabelText());
                    MyAssert.LabelTextAreEqual(item.CurrentQuantity, form1.itemCurrQtyTLP.getLabelText());
                    MyAssert.LabelTextAreEqual(item.ITEM_ID, form1.itemItemNoLbl.Text);
                    if (item.hasShippingEntry())
                    {
                        MyAssert.LabelTextAreEqual(item.Length, form1.itemLengthTLP.getLabelText());
                        MyAssert.LabelTextAreEqual(item.Width, form1.itemWidthTLP.getLabelText());
                        MyAssert.LabelTextAreEqual(item.Height, form1.itemHeightTLP.getLabelText());
                        MyAssert.LabelTextAreEqual(item.Weight % 16, form1.itemWeightOzTLP.getLabelText());
                        MyAssert.LabelTextAreEqual(item.Weight / 16, form1.itemWeightLbsTLP.getLabelText());
                    }
                }
            }
            

        }



        // searchItems
        [Test]
        public static void Test_getAndSetSearchItems()
        {
            int totalTests = 10;
            int totalItems = 10;
            for (int i = 0; i < totalTests; i++)
            {
                List<Item> testingItems = generateRandomItems(purchases, totalItems);

                tabController.setSearchItems(testingItems);

                List<Item> actualItems = tabController.getSearchItems();

                Assert.AreEqual(testingItems, actualItems);
            }
        }

        

        [Test]
        public static void Test_getSearchItemsAt()
        {
            int totalTests = 10;
            int totalItems = 10;
            for (int i = 0; i < totalTests; i++)
            {
                List<Item> testingItems = generateRandomItems(purchases, totalItems);

                tabController.setSearchItems(testingItems);

                for (int j = 0; j < totalItems; j++)
                {
                    Item actualItem = tabController.getSearchItemsAt(j);
                    Assert.AreEqual(testingItems[j], actualItem);

                }
            }
        }



        [Test]
        public static void Test_getSearchItemsAtNull()
        {
            int totalItems = 10;
            List<Item> testingItems = generateRandomItems(purchases, totalItems);

            tabController.setSearchItems(testingItems);
            Assert.Catch<Exception>(() => tabController.getSearchItemsAt(totalItems+1));
            Assert.Catch<Exception>(() => tabController.getSearchItemsAt(-1));
        }



        [Test]
        public static void Test_clearSearchItems()
        {
            int totalItems = 10;
            List<Item> testingItems = generateRandomItems(purchases, totalItems);
            tabController.setSearchItems(testingItems);

            Assert.AreNotEqual(0, tabController.getSearchItems().Count());
            tabController.clearSearchItems();
            Assert.AreEqual(0, tabController.getSearchItems().Count());
        }



        // currPurc
        [Test]
        public static void Test_getCurrPurc()
        {
            foreach (Purchase purchase in purchases)
            {
                setCurrItem(purchase.items[0]);
                Assert.AreEqual(purchase.PURCHASE_ID, tabController.getCurrPurc().PURCHASE_ID);
            }
        }



        // currPurcItems
        [Test]
        public static void Test_getAndSetCurrPurcItems()
        {
            foreach (Purchase purchase in purchases)
            {
                setCurrItem(purchase.items[0]);
                Assert.AreEqual(purchase.PURCHASE_ID, tabController.getCurrPurc().PURCHASE_ID);
            }
        }



        [Test]
        public static void Test_getCurrPurcItemsAt()
        {
            foreach (Purchase purchase in purchases)
            {
                setCurrItem(purchase.items[0]);
                for (int i = 0; i < purchase.items.Count(); i++)
                {
                    Assert.AreEqual(purchase.items[i], tabController.getCurrPurcItemsAt(i));
                }
            }
        }



        [Test]
        public static void Test_clearCurrPurcItems()
        {
            setCurrItem(purchases[0].items[0]);
            Assert.NotZero(tabController.getCurrPurcItems().Count());
            tabController.clearCurrPurcItems();
            Assert.Zero(tabController.getCurrPurcItems().Count());
        }



        [Test]
        public static void Test_getAndSetCurrSale()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    if (item.sales == null || item.sales.Count() == 0) { continue; }
                    setCurrItem(item);
                    int randSaleNum = (int)TestingUtil.random.NextInt64(tabController.getCurrPurcItems().Count() - 1);
                    Sale expectedSale = item.sales[randSaleNum];
                    tabController.setCurrSale(randSaleNum);
                    Sale actualSale = tabController.getCurrSale();
                    Assert.AreEqual(expectedSale.get_SALE_ID(), actualSale.get_SALE_ID());
                }
            }
        }


        
        [Test]
        public static void Test_getCurrSaleAt()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    setCurrItem(item);
                    for (int i = 0; i < item.sales.Count(); i++)
                    {
                        Assert.AreEqual(item.sales[i].get_SALE_ID(), tabController.getCurrSaleAt(i).get_SALE_ID());
                    }
                }
            }
        }



        [Test]
        public static void Test_getCurrentItemSales()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    setCurrItem(item);
                    List<Sale> sales = tabController.getCurrentItemSales();
                    for (int i = 0; i < item.sales.Count(); i++)
                    {
                        Assert.AreEqual(item.sales[i].get_SALE_ID(), sales[i].get_SALE_ID());
                    }
                }
            }
        }



        [Test]
        public static void Test_getItemInEditingStateAndFlipIVEditMode()
        {
            Assert.IsFalse(tabController.getItemInEditingState());
            setCurrItem(null);
            // Don't filp edit mode if no curr item to edit
            tabController.itemViewFlipEditMode();
            Assert.IsFalse(tabController.getItemInEditingState());

            setCurrItem(purchases[0].items[0]);
            // Now able to filp edit mode with a curr item to edit
            tabController.itemViewFlipEditMode();
            Assert.IsTrue(tabController.getItemInEditingState());

        }



        [Test]
        public static void Test_setCurrSaleInt()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    setCurrItem(item);
                    for (int i = 0; i < item.sales.Count(); i++)
                    {
                        tabController.setCurrSale(i);
                        Assert.AreEqual(item.sales[i].get_SALE_ID(), tabController.getCurrSale().get_SALE_ID());
                    }
                }
            }
        }



        [Test]
        public static void Test_getSaleInEditingState_InitialState()
        {
            Assert.IsFalse(tabController.saleTabGetInEditingState());
        }



        [Test]
        public static void Test_getSaleInEditingState_NoCurrItemOrCurrSale()
        {
            tabController.saleTabEditMode();
            Assert.IsFalse(tabController.saleTabGetInEditingState());
        }



        [Test]
        public static void Test_getSaleInEditingState_NoCurrSale()
        {
            setCurrItem(purchases[0].items[0]);
            tabController.saleTabEditMode();
            Assert.IsFalse(tabController.saleTabGetInEditingState());
        }




        [Test]
        public static void Test_getSaleInEditingStateAndSaleTflipEditMode()
        {
            Assert.IsFalse(tabController.saleTabGetInEditingState());
            tabController.saleTabflipEditMode();
            setCurrItem(null);
            // Don't filp edit mode if no curr sale to edit
            tabController.saleTabflipEditMode();
            Assert.IsFalse(tabController.saleTabGetInEditingState());

            setCurrItem(purchases[0].items[0]);
            // Still don't flip edit mode without a current sale
            tabController.saleTabflipEditMode();

            tabController.setCurrSale(0);

            // Now able to filp edit mode with a curr item to edit
            tabController.saleTabflipEditMode();
            Assert.IsTrue(tabController.saleTabGetInEditingState());

        }



        [Test]
        public static void Test_getItemInEditingState_InitialState()
        {
            Assert.IsFalse(tabController.getItemInEditingState());
        }



        [Test]
        public static void Test_getSaleInEditingState_NoCurrItem()
        {
            tabController.itemViewFlipEditMode();
            Assert.IsFalse(tabController.getItemInEditingState());
        }



        [Test]
        public static void Test_getSaleInEditingState_WithBothCurrItemAndCurrSale()
        {
            setCurrItem(purchases[0].items[0]);
            tabController.itemViewFlipEditMode();
            Assert.IsTrue(tabController.getItemInEditingState());
        }



        [Test]
        public static void Test_saleTabUpdate()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    if (item.sales is null || item.sales.Count() == 0) { continue; }

                    // Set curr item
                    setCurrItem(item);
                    tabController.setCurrSale(0);

                    // Enter user input mode for sale
                    tabController.saleTabEditMode();

                    // Set user input
                    int soldPrice = (int) TestingUtil.random.NextInt64(999999);
                    form1.SaleAmountTLP.setControlVal(soldPrice.ToString());
                    form1.SaleDatePickerDLP.setControlVal( new DateTime(2020, 1, 1));

                    if (item.Name == "Item3" &&
                        item.get_Date_Purchased_str() == "1978-12-16")
                    {
                        int a = 0;
                    }

                    // Run saleTabUpdate
                    tabController.saleTabUpdate();

                    // Test that sale user input matches what was inputted into database
                    List<Sale> itemSales = tabController.getCurrentItemSales();
                    Sale actualSale = itemSales[0];
                    Assert.AreEqual(soldPrice, actualSale.get_Amount_sale());

                    // Test that all labels represent the same info as in database
                    MyAssert.StringsEqual(soldPrice.ToString(), form1.SaleAmountTLP.getLabelText());
                    MyAssert.StringsEqual(new Util.Date(2020, 1, 1).toDateString(), form1.SaleDatePickerDLP.getControlValueAsStr());


                    // Test that sale tab is in view mode
                    Assert.IsFalse(tabController.saleTabGetInEditingState());
                }
            }
        }



        [Test]
        public static void Test_search()
        {
            OneTimeSetup();
            SetUp();
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    // Set search terms
                    string name = item.Name;
                    form1.searchBox.Text = name;

                    // Run search
                    Util.Date purcDate = item.Date_Purchased;
                    tabController.search();

                    // Check search list for searched items
                    List<Item> searchItems = tabController.getSearchItems();

                    bool found = false;
                    foreach (Item result in searchItems)
                    {
                        if (result.ITEM_ID == item.ITEM_ID &&
                            result.Name == item.Name)
                        {
                            found = true;
                        }
                    }
                    Assert.IsTrue(found);

                    // Check gui for searched elements
                    Assert.IsTrue(form1.itemSearchView.countItems() >= 1);
                }
            }
        }



        [Test]
        public static void Test_search_NoResults()
        {
            // Set search terms
            form1.searchBox.Text = "NonameBadName1234253";

            // Run search
            tabController.search();

            // Check gui for searched elements
            Assert.AreEqual(0, form1.itemSearchView.countItems());
        }



        [Test]
        public static void Test_search_DefaultSearch()
        {
            // Set search terms
            form1.searchBox.Text = "";

            // Run search
            tabController.search();

            // Check gui for searched elements
            Assert.AreNotEqual(0, form1.itemSearchView.countItems());

            int totalItems = 0;
            foreach(Purchase purchase in purchases)
            {
                totalItems += purchase.items.Count();
            }

            Assert.AreEqual(totalItems, form1.itemSearchView.countItems());
        }



        [Test]
        public static void Test_itemViewUpdate()
        {
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {

                    // Set curr item
                    setCurrItem(item);
                    if (item.sales.Count() > 0)
                    {
                        tabController.setCurrSale(0);
                    }
                    // Enter user input mode for item
                    tabController.itemViewFlipEditMode();

                    // Set user input
                    string name = item.Name + "Name Extension";
                    int initQty = (int) TestingUtil.random.NextInt64(999999);
                    int currQty = (int) TestingUtil.random.NextInt64(999999);
                    int length = (int) TestingUtil.random.NextInt64(999999);
                    int width = (int) TestingUtil.random.NextInt64(999999);
                    int height = (int) TestingUtil.random.NextInt64(999999);
                    int weightOz = (int) TestingUtil.random.NextInt64(999999);
                    int weightLbs = (int) TestingUtil.random.NextInt64(999999);
                    form1.itemNameTLP.setControlVal(name);
                    form1.itemCurrQtyTLP.setControlVal(currQty.ToString());
                    form1.itemInitQtyTLP.setControlVal(initQty.ToString());
                    form1.itemLengthTLP.setControlVal(length.ToString());
                    form1.itemWidthTLP.setControlVal(width.ToString());
                    form1.itemHeightTLP.setControlVal(height.ToString());
                    form1.itemWeightOzTLP.setControlVal(weightOz.ToString());
                    form1.itemWeightLbsTLP.setControlVal(weightLbs.ToString());
                    item.Name = name;
                    item.InitialQuantity = initQty;
                    item.CurrentQuantity = currQty;
                    item.Length = length;
                    item.Width = width;
                    item.Height = height;
                    item.Weight = weightOz+16*weightLbs;

                    // Run itemViewUpdate
                    tabController.itemViewUpdateFromUserInput();

                    // Test that sale user input matches what was inputted into database
                    Item actualItem = tabController.getCurrItem();
                    MyAssert.StringsEqual(name, actualItem.Name);
                    Assert.AreEqual(initQty, actualItem.InitialQuantity);
                    Assert.AreEqual(currQty, actualItem.CurrentQuantity);
                    Assert.AreEqual(length, actualItem.Length);
                    Assert.AreEqual(width, actualItem.Width);
                    Assert.AreEqual(height, actualItem.Height);
                    Assert.AreEqual((weightOz + 16 * weightLbs) % 16, actualItem.get_WeightOz());
                    Assert.AreEqual((weightOz + 16 * weightLbs) / 16, actualItem.get_WeightLbs());

                    // Test that all labels represent the same info as in database
                    MyAssert.StringsEqual(initQty.ToString(), form1.itemInitQtyTLP.getLabelText());
                    MyAssert.StringsEqual(currQty.ToString(), form1.itemCurrQtyTLP.getLabelText());
                    MyAssert.StringsEqual(length.ToString(), form1.itemLengthTLP.getLabelText());
                    MyAssert.StringsEqual(width.ToString(), form1.itemWidthTLP.getLabelText());
                    MyAssert.StringsEqual(height.ToString(), form1.itemHeightTLP.getLabelText());
                    MyAssert.StringsEqual(((weightOz + 16 * weightLbs) % 16).ToString(), form1.itemWeightOzTLP.getLabelText());
                    MyAssert.StringsEqual(((weightOz + 16 * weightLbs) / 16).ToString(), form1.itemWeightLbsTLP.getLabelText());


                    // Test that sale tab is in view mode
                    Assert.IsFalse(tabController.getItemInEditingState());
                }
            }
        }

        

        [Test]
        public static void Test_IVdeleteShippingInfo()
        {
            bool testedShippingInfo = false;
            foreach (Purchase purchase in purchases)
            {
                foreach (Item item in purchase.items)
                {
                    int length = item.Length;
                    int width = item.Width;
                    int height = item.Height;
                    int weightLbs = item.get_WeightLbs();
                    int weightOz = item.get_WeightOz();

                    // Don't test deleting shipping info if no shipping info exists
                    if (length == Util.DEFAULT_INT)
                    {
                        continue;
                    }

                    setCurrItem(item);

                    tabController.itemViewFlipEditMode();

                    tabController.itemViewDeleteShipInfo();

                    Item actualItem = tabController.getCurrItem();

                    Assert.AreEqual(Util.DEFAULT_INT, actualItem.Length);
                    Assert.AreEqual(Util.DEFAULT_INT, actualItem.Width);
                    Assert.AreEqual(Util.DEFAULT_INT, actualItem.Height);
                    Assert.AreEqual(Util.DEFAULT_INT, actualItem.Weight);

                    testedShippingInfo = true;
                }
            }
            Assert.IsTrue(testedShippingInfo);
            // Reset items
            OneTimeSetup();

        }



        [Test]
        public static void Test_getPLInEditingStateAndPLflipEditMode()
        {
            Assert.IsFalse(tabController.getPLInEditingState());
            setCurrItem(null);
            // Don't filp edit mode if no curr purc to edit
            tabController.purcItemsflipEditMode();
            Assert.IsFalse(tabController.getPLInEditingState());

            setCurrItem(purchases[0].items[0]);
            // Now able to filp edit mode with a curr item to edit
            tabController.purcItemsflipEditMode();
            Assert.IsTrue(tabController.getPLInEditingState());
        }



        [Test]
        public static void Test_PLaddItem()
        {
            // Make sure no curr purc for init test
            Assert.IsNull(tabController.getCurrPurc());
            form1.PurcNameTextbox.Text = "NewItem";
            form1.PurcNewPurcPurcPriceTextbox.Text = "123";

            tabController.purcItemsNewPurchase();

            form1.PurcNameTextbox.Text = "NewItem1";
            form1.PurcNewPurcPurcPriceTextbox.Text = "1234";

            tabController.purcItemsAddItem();
            Assert.IsNotNull(tabController.getCurrPurc());
            Purchase actualPurc = tabController.getCurrPurc();
            Item actualItem = actualPurc.items[0];
            MyAssert.StringsEqual("NewItem", actualItem.Name);
            Assert.AreEqual(1, actualItem.InitialQuantity);
            Assert.AreEqual(1, actualItem.CurrentQuantity);

            // Make sure textboxes are cleared
            MyAssert.StringsEqual("", form1.PurcNameTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcInitQtyTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcCurrQtyTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcLengthTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcWidthTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcHeightTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcWeightLbsTextbox.Text);
            MyAssert.StringsEqual("", form1.PurcWeightOzTextbox.Text);

            // Make sure item viewer can see the name of the new item in the purchase
            bool foundItem = false;
            foreach (string item in form1.PurchaseListBox.Items)
            {
                if (item.CompareTo("NewItem") == 0)
                {
                    foundItem = true;
                    break;
                }
            }
            Assert.IsTrue(foundItem);
            OneTimeSetup();
        }



        [Test]
        public static void Test_PLnewPurchase()
        {
            tabController.purcItemsNewPurchase();
            Assert.AreEqual(0, form1.PurchaseListBox.Items.Cast<List<string>>().Count());
            Assert.IsNull(tabController.getCurrItem());
            Test_clearCurrItemControls();
        }



        [Test]
        public static void Test_clearCurrItemControls()
        {
            setCurrItem(purchases[0].items[0]);
            tabController.clearCurrItemControls();

            string now = new Util.Date(DateTime.Now).toDateString();

            MyAssert.StringsEqual("", form1.itemPurcPriceLbl.Text);
            MyAssert.StringsEqual("", form1.itemSoldPriceLbl.Text);
            MyAssert.StringsEqual("", form1.itemInitQtyTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemCurrQtyTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemItemNoLbl.Text);
            MyAssert.StringsEqual("", form1.itemWeightLbsTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemWeightOzTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemLengthTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemWidthTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemHeightTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemNameTLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemDatePurcLbl.Text);
            MyAssert.StringsEqual("", form1.itemInitQtyTLP.getLabelText());
            MyAssert.StringsEqual("", form1.SaleAmountTLP.getLabelText());
            MyAssert.StringsEqual("", form1.SaleNameLbl.Text);
            MyAssert.StringsEqual("", form1.SaleDatePickerDLP.getLabelText());
            MyAssert.StringsEqual("", form1.itemNameTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemInitQtyTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemCurrQtyTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemWeightLbsTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemWeightOzTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemLengthTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemWidthTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.itemHeightTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.SaleAmountTLP.getControlValueAsStr());
            MyAssert.StringsEqual("", form1.SaleNewSaleAmountTextbox.Text);
            Assert.AreEqual(new Util.Date(now), new Util.Date(form1.SaleNewSaleDatePicker));
            Assert.AreEqual(new Util.Date(now), new Util.Date(form1.SaleDatePickerDLP));
            Assert.AreEqual(0, form1.saleListBox.Items.Cast<List<string>>().Count());
        }




        [Test]
        public static void Test_saleTaddSale()
        {
            // Make sure no curr sale for init test
            Assert.IsNull(tabController.getCurrSale());
            form1.SaleNewSaleAmountTextbox.Text = "123";
            form1.SaleNewSaleDatePicker.Value = new DateTime(2000, 1, 1);
            setCurrItem(purchases[0].items[0]);
            tabController.saleTabAddSale();
            Assert.IsNull(tabController.getCurrSale());
            Sale actualSale = tabController.getCurrSale();

            // Make sure textboxes are cleared
            MyAssert.StringsEqual("", form1.SaleNewSaleAmountTextbox.Text);

            // Make sure item viewer can see the name of the new sale in the purchase
            bool foundItem = false;
            foreach (string item in form1.saleListBox.Items)
            {
                if (item.CompareTo("2000-1-1, 123") == 0)
                {
                    foundItem = true;
                    break;
                }
            }
            Assert.IsTrue(foundItem);
        }



        [Test]
        public static void Test_purchasedLotUpdate()
        {
            // Set curr item
            Item item = purchases[0].items[0];
            setCurrItem(item);
            Purchase expectedPurc = purchases[0];

            // Enter user input mode for item
            tabController.purcItemsflipEditMode();

            // Set user input
            string purcNotes = "PurchaseNotes";
            int purcPrice = (int)TestingUtil.random.NextInt64(999999);
            DateTime purcDate = new DateTime(2000, 1, 1);
            Util.Date myPurcDate = new Util.Date(purcDate.Year, purcDate.Month, purcDate.Day);
            form1.PurcPurcNotesTLP.setControlVal(purcNotes);
            form1.PurcPurcPriceTLP.setControlVal(purcPrice.ToString());
            form1.PurcDatePickerDLP.setControlVal(purcDate);
            expectedPurc.Amount_purchase = purcPrice;
            expectedPurc.Notes_purchase = purcNotes;
            expectedPurc.Date_Purchased = myPurcDate;


            // Run itemViewUpdate
            tabController.purcItemsUpdateFromUserInput();

            // Test that sale user input matches what was inputted into database
            Purchase actualPurchase = tabController.getCurrPurc();
            MyAssert.StringsEqual(purcNotes, actualPurchase.Notes_purchase);
            Assert.AreEqual(purcPrice, actualPurchase.Amount_purchase);
            Assert.AreEqual(myPurcDate, actualPurchase.Date_Purchased);

            // Test that all labels represent the same info as in database
            MyAssert.StringsEqual(myPurcDate.toDateString(), form1.PurcDatePickerDLP.getLabelText());
            MyAssert.StringsEqual(purcPrice.ToString(), form1.PurcPurcPriceTLP.getLabelText());
            MyAssert.StringsEqual(purcNotes.ToString(), form1.PurcPurcNotesTLP.getLabelText());


            // Test that sale tab is in view mode
            Assert.IsFalse(tabController.getPLInEditingState());

        }



        [Test]
        public static void Test_deleteCurrSale()
        {
            form1.SaleNewSaleAmountTextbox.Text = "123";
            form1.SaleNewSaleDatePicker.Value = new DateTime(2000, 1, 1);
            setCurrItem(purchases[0].items[0]);
            tabController.saleTabAddSale();

            int lastSaleIndex = tabController.getCurrItem().sales.Count() - 1;
            tabController.setCurrSale(lastSaleIndex);
            tabController.deleteCurrSale();
            int newLastSaleIndex = tabController.getCurrItem().sales.Count() - 1;
            Assert.AreEqual(lastSaleIndex - 1, newLastSaleIndex);
        }



        [Test]
        public static void Test_deleteCurrItem()
        {
            setCurrItem(purchases[0].items[0]);
            tabController.deleteCurrItem();
            Assert.Catch<Exception>(() => setCurrItem(purchases[0].items[0]));
            OneTimeSetup();
        }



        [Test]
        public static void Test_setMainImageAndInsertImage()
        {
            // Nothing should happen since no curr item is selected
            tabController.insertImage();


            setCurrItem(purchases[0].items[0]);

            Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, form1.mainPictureViewer.getMainImage()));

            string expectedImagePath = "C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabaseTesting\\TestingItems\\Images\\20210118_133444.jpg";
            
            Image expectedImage = Image.FromFile(expectedImagePath);

            Database.insertImage(expectedImagePath, tabController.getCurrItem());

            setCurrItem(purchases[0].items[0]);

            Item actualItem = tabController.getCurrItemWithImages();
            List<MyImage> actualImages = actualItem.images;
            Image actualImage = actualImages[actualImages.Count() - 1].image;

            Assert.IsTrue(TestingUtil.compareImages(expectedImage, actualImage));

        }



        [Test]
        public static void Test_setThumbnail()
        {
            OneTimeSetup();
            SetUp();
            string expectedImagePath = "C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabaseTesting\\TestingItems\\Images\\20210118_133444.jpg";
            Image expectedImage = Image.FromFile(expectedImagePath);
            string expectedImagePath2 = "C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabaseTesting\\TestingItems\\Images\\20210118_133514.jpg";
            Image expectedImage2 = Image.FromFile(expectedImagePath);

            setCurrItem(purchases[0].items[0]);


            // Test that the defualt thumbnail is there
            setCurrItem(tabController.getCurrItem());
            Item actualItem = tabController.getCurrItem();
            Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, actualItem.thumbnail.image, true));
            
            
            // Give 1st image and test that it is by default made the thumbnail
            Database.insertImage(expectedImagePath, tabController.getCurrItem());
            // Update internal item
            setCurrItem(tabController.getCurrItem());
            actualItem = tabController.getCurrItem();
            // First added image should become thumbnail by default
            Assert.IsTrue(TestingUtil.compareImages(expectedImage, actualItem.thumbnail.image, true));

            // Insert 2nd image into the databaes and make sure the first image is still the thumbnail
            Database.insertImage(expectedImagePath, tabController.getCurrItem());
            // Update internal item
            setCurrItem(tabController.getCurrItem());
            actualItem = tabController.getCurrItem();
            Assert.IsTrue(TestingUtil.compareImages(expectedImage, actualItem.thumbnail.image, true));


            // Set the second image as the thumbnail
            tabController.setMainImage(1);
            tabController.setThumbnail();

            setCurrItem(tabController.getCurrItem());
            actualItem = tabController.getCurrItem();
            Assert.IsTrue(TestingUtil.compareImages(expectedImage2, actualItem.thumbnail.image, true));
        }



        [Test]
        public static void Test_getCurrItemWithImages()
        {
            string expectedImagePath = "C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabaseTesting\\TestingItems\\Images\\20210118_133444.jpg";
            Image expectedImage = Image.FromFile(expectedImagePath);

            setCurrItem(purchases[0].items[0]);
            Item actualItem = tabController.getCurrItemWithImages();
            List<MyImage> actualDefaultImages = actualItem.images;
            Image actualDefaultImage = actualDefaultImages[actualDefaultImages.Count() - 1].image;

            Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE.image, actualDefaultImage));


            Database.insertImage(expectedImagePath, tabController.getCurrItem());

            setCurrItem(purchases[0].items[0]);

            actualItem = tabController.getCurrItemWithImages();
            List<MyImage> actualImages = actualItem.images;
            Image actualImage = actualImages[actualImages.Count() - 1].image;

            Assert.IsTrue(TestingUtil.compareImages(expectedImage, actualImage));

        }



        [Test]
        public static void Test_didTextboxChange()
        {
            OneTimeSetup();
            SetUp();

            Item currItem = purchases[0].items[0];

            setCurrItem(currItem);

            tabController.itemViewFlipEditMode();

            form1.itemNameTLP.setControlVal("ItemName");
            form1.itemCurrQtyTLP.setControlVal("123");
            form1.itemHeightTLP.setControlVal("234");
            form1.itemWeightLbsTLP.setControlVal("345");
            form1.itemWidthTLP.setControlVal("456");

            Assert.IsTrue(tabController.didTextboxChange(form1.itemNameTLP));
            Assert.IsTrue(tabController.didTextboxChange(form1.itemCurrQtyTLP));
            Assert.IsTrue(tabController.didTextboxChange(form1.itemHeightTLP));
            Assert.IsTrue(tabController.didTextboxChange(form1.itemWeightLbsTLP));
            Assert.IsTrue(tabController.didTextboxChange(form1.itemWidthTLP));
            
            Assert.IsFalse(tabController.didTextboxChange(form1.itemInitQtyTLP));
            Assert.IsFalse(tabController.didTextboxChange(form1.itemLengthTLP));
            Assert.IsFalse(tabController.didTextboxChange(form1.itemWeightOzTLP));
        }



        [Test]
        public static void Test_checkTypeOkay()
        {
            form1.itemNameTLP.setControlVal("ItemName");
            form1.itemCurrQtyTLP.setControlVal("123");
            form1.itemHeightTLP.setControlVal("234");
            form1.itemWeightLbsTLP.setControlVal("345");
            form1.itemWidthTLP.setControlVal("456");

            Assert.IsTrue(tabController.checkTypeOkay(form1.itemNameTLP));
            Assert.IsTrue(tabController.checkTypeOkay(form1.itemCurrQtyTLP));
            Assert.IsTrue(tabController.checkTypeOkay(form1.itemHeightTLP));
            Assert.IsTrue(tabController.checkTypeOkay(form1.itemWeightLbsTLP));
            Assert.IsTrue(tabController.checkTypeOkay(form1.itemWidthTLP));


            form1.itemCurrQtyTLP.setControlVal("A");
            form1.itemHeightTLP.setControlVal("BCE");
            form1.itemWeightLbsTLP.setControlVal("1.234");
            form1.itemWidthTLP.setControlVal(" ");

            Assert.IsFalse(tabController.checkTypeOkay(form1.itemCurrQtyTLP));
            Assert.IsFalse(tabController.checkTypeOkay(form1.itemHeightTLP));
            Assert.IsFalse(tabController.checkTypeOkay(form1.itemWeightLbsTLP));
            Assert.IsFalse(tabController.checkTypeOkay(form1.itemWidthTLP));
        }




        private static List<Item> generateRandomItems(List<Purchase> purchases, int totalItems)
        {
            List<Item> testingItems = new List<Item>();
            int numPurchases = purchases.Count() - 1;
            int numItems;
            for (int j = 0; j < totalItems; j++)
            {
                int purchaseNum = (int)TestingUtil.random.NextInt64(numPurchases);
                numItems = purchases[purchaseNum].items.Count();
                int itemNum = (int)TestingUtil.random.NextInt64(numItems);

                testingItems.Add(purchases[purchaseNum].items[itemNum]);
            }
            return testingItems;
        }


        public static void setCurrItem(Item item)
        {
            if (item is null) {
                tabController.setCurrItem(null);
                return;
            }
            form1.searchBox.Text = item.Name;
            tabController.search();
            List<Item> searchItems = tabController.getSearchItems();
            foreach(Item searchItem in searchItems)
            {
                if (searchItem.ITEM_ID == item.ITEM_ID)
                {
                    tabController.setCurrItem(searchItem);
                    return;
                }
            }
            throw new Exception("Error: Item Not Found: " + item.Name);
        }
    }
}
