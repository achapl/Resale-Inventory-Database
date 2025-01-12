using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDatabaseTesting
{
    public class TestItemViewTab
    {

        public static Form1 form1;
        public static TabController tabController;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            form1 = new Form1();
            tabController = new TabController(form1);

            List<Purchase> purchases = TestingUtil.getTestingItems();
            foreach (Purchase purchase in purchases)
            {
                int purcID = Database.insertPurchase(purchase.Amount_purchase, purchase.Notes_purchase, new Util.Date(purchase.Date_Purchased_str));
                foreach (Item item in purchase.items)
                {
                    item.set_PurchaseID(purcID);
                    Database.insertItem(item, out int itemID);

                    foreach (Sale sale in item.sales)
                    {
                        sale.set_ItemID_sale(item.get_ITEM_ID());
                        sale.set_Date_Sold(new Util.Date(sale.Date_Sold_str));
                        Database.insertSale(sale);
                    }
                }
            }
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            form1.Dispose();
        }



        [Test]
        public static void Test_ItemViewTab()
        {
            ItemViewTab tab = new ItemViewTab(tabController, form1);
            Assert.IsNull(tab.getCurrItem());
        }
    }
}
