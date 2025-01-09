using FinancialDatabase;
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

            tab.setCurrItem(Database.getItem())
        }
    }
}
