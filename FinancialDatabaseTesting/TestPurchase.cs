using FinancialDatabase.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace FinancialDatabaseTesting
{
    class TestPurchase
    {
        static List<Purchase> purchases;


        [SetUp]
        public static void SetUp()
        {
            purchases = TestingUtil.getTestingItems();
        }


        [Test]
        public static void Test_ConstructorDefault()
        {
            Purchase purc = new Purchase();
            MyAssert.StringsEqual(Util.DEFAULT_STRING, purc.Notes_purchase);
            Assert.IsTrue(Util.DEFAULT_DATE.Equals(purc.Date_Purchased));
            Assert.AreEqual(Util.DEFAULT_DOUBLE, purc.Fees_purchase);
            Assert.AreEqual(Util.DEFAULT_INT, purc.Amount_purchase);
            MyAssert.StringsEqual(Util.DEFAULT_STRING, purc.Seller);
            Assert.AreEqual(Util.DEFAULT_INT, purc.PURCHASE_ID);
            Assert.AreEqual(Util.DEFAULT_DOUBLE, purc.Tax);
            Assert.IsNull(purc.items);

        }


        [Test]
        public static void Test_ConstructorList()
        {
            List<string> colNames = new List<string> { "Fees_purchase",
                                                        "Amount_purchase",
                                                        "Date_Purchased",
                                                        "Tax",
                                                        "PURCHASE_ID",
                                                        "Notes",
                                                        "Seller",
            };

            List<string> attribs = new List<string> { "1.23",
                                                      "1.02",
                                                      "datetime.date(2015, 10, 11)",
                                                      "1.53",
                                                      "-1",
                                                      "Notes About Purchase",
                                                      "Purchase Seller"
            };

            Purchase purchase = new Purchase(attribs, colNames);



        }




        [Test]
        public static void Test_Add()
        {
            foreach(Purchase expectedPurchase in purchases)
            {
                Purchase actualPurcahse = new Purchase();
                foreach (Item item in expectedPurchase.items)
                {
                    actualPurcahse.add(item);
                }

                List<Item> ActualItems = actualPurcahse.items;

                for (int i = 0; i < ActualItems.Count; i++)
                {
                    MyAssert.ItemsEqual(expectedPurchase.items[i], ActualItems[i]);
                }
            }
        }

        [Test]
        public static void Test_Setters()
        {
            foreach (Purchase purchase in purchases)
            {
                Purchase actualPurchase = new Purchase();
                actualPurchase.set_fees_purchase(purchase.Fees_purchase.ToString());
                actualPurchase.set_date(purchase.Date_Purchased.toDateString());
                actualPurchase.set_amount(purchase.Amount_purchase.ToString());
                actualPurchase.set_notes_purchase(purchase.Notes_purchase);
                actualPurchase.set_id(purchase.PURCHASE_ID.ToString());
                actualPurchase.set_tax(purchase.Tax.ToString());
                actualPurchase.set_seller(purchase.Seller);

                MyAssert.StringsEqual(purchase.Notes_purchase, actualPurchase.Notes_purchase);
                Assert.IsTrue(purchase.Date_Purchased.Equals(actualPurchase.Date_Purchased));
                Assert.AreEqual(purchase.Amount_purchase, actualPurchase.Amount_purchase);
                Assert.AreEqual(purchase.Fees_purchase, actualPurchase.Fees_purchase);
                Assert.AreEqual(purchase.PURCHASE_ID, actualPurchase.PURCHASE_ID);
                MyAssert.StringsEqual(purchase.Seller, actualPurchase.Seller);
                Assert.AreEqual(purchase.Tax, actualPurchase.Tax);
            }
        }



        [Test]
        public static void Test_SettersNULL()
        {
            Purchase actualPurchase = new Purchase();
            actualPurchase.set_notes_purchase(null);
            actualPurchase.set_fees_purchase(null);
            actualPurchase.set_amount(null);
            actualPurchase.set_seller(null);
            actualPurchase.set_date(null);
            actualPurchase.set_tax(null);
            actualPurchase.set_id(null);

            MyAssert.StringsEqual(Util.DEFAULT_STRING, actualPurchase.Notes_purchase);
            Assert.IsTrue(Util.DEFAULT_DATE.Equals(actualPurchase.Date_Purchased));
            Assert.AreEqual(Util.DEFAULT_DOUBLE, actualPurchase.Amount_purchase);
            Assert.AreEqual(Util.DEFAULT_DOUBLE, actualPurchase.Fees_purchase);
            Assert.AreEqual(Util.DEFAULT_INT, actualPurchase.PURCHASE_ID);
            MyAssert.StringsEqual(Util.DEFAULT_STRING, actualPurchase.Seller);
            Assert.AreEqual(Util.DEFAULT_DOUBLE, actualPurchase.Tax);
        }
    }
}
