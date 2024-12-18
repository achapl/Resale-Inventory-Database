namespace FinancialDatabaseTesting;

using FinancialDatabase;
using NUnit.Framework.Internal.Execution;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;

public class TestDatabase
{


    private static object[] dtbTestingItems =
    {
        //            name,      price, purc date,                 initQty, currQty,
        new object[] {"Item1",   100,   new Util.Date(1978,12,16), 1,       1        },
        new object[] {"Item2",   200,   new Util.Date(1978,12,17), 1,       1        },
        new object[] {"Item3",   300,   new Util.Date(1978,12,18), 1,       1        },
    };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var DtbObject = typeof(Database);
        var testingVar = DtbObject.GetField("TESTING", BindingFlags.NonPublic | BindingFlags.Static);
        testingVar.SetValue(null, true);

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

    private bool addTestingItems()
    {
        foreach (object[] item in dtbTestingItems)
        {
            int purcID = Database.insertPurchase((int) item[1], "", (Util.Date) item[2]);

            Item newItem = new Item();
            newItem.set_Name((string) item[0]);
            newItem.set_InitialQuantity((int) item[3]);
            newItem.set_CurrentQuantity((int) item[4]);
            newItem.set_PurchaseID(purcID);
            Database.insertItem(newItem);
        }

        return true;
    }


    [Test]
    public static void testMe()
    {
        Assert.IsTrue(true);
    }

}
