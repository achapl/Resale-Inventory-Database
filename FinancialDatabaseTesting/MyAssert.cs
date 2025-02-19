using System;

public class MyAssert
{
	public MyAssert()
	{
	}

	public static void LabelTextAreEqual(string expected, string labelText)
	{
        Assert.AreEqual(0, expected.CompareTo(labelText));
	}

    public static void LabelTextAreEqual(double expected, string labelText)
    {
        if (expected == Util.DEFAULT_DOUBLE)
        {
            Assert.AreEqual(0, labelText.CompareTo(""));
            return;
        }

        Assert.AreEqual(Double.Parse(labelText), expected);
    }

    public static void LabelTextAreEqual(int expected, string labelText)
    {
        if (expected == Util.DEFAULT_INT)
        {
            Assert.AreEqual(0, labelText.CompareTo(""));
            return;
        }

        Assert.AreEqual(Int32.Parse(labelText), expected);
    }

    public static void LabelTextAreEqual(Util.Date expected, string labelText)
    {
        if (expected.Equals(Util.DEFAULT_DATE))
        {
            Assert.AreEqual(0, labelText.CompareTo(""));
            return;
        }

        Assert.AreEqual(0, expected.toDateString().CompareTo(labelText));
    }

    public static void StringsEqual(string expected, string actual)
    {
        if (expected == null &&
            actual == null)
        {
            Assert.Pass();
            return;
        }

        Assert.AreEqual(0, expected.CompareTo(actual));
    }

    internal static void ItemsEqual(Item expectedItem, Item actualItem)
    {
        Assert.AreEqual(expectedItem.ITEM_ID, actualItem.ITEM_ID);
        Assert.AreEqual(expectedItem.Name, actualItem.Name);
        Assert.AreEqual(expectedItem.Amount_purchase, actualItem.Amount_purchase);
        Assert.AreEqual(expectedItem.Date_Purchased, actualItem.Date_Purchased);
        Assert.AreEqual(expectedItem.InitialQuantity, actualItem.InitialQuantity);
        Assert.AreEqual(expectedItem.CurrentQuantity, actualItem.CurrentQuantity);

        if (expectedItem.hasShippingEntry())
        {
            Assert.AreEqual(expectedItem.Weight, actualItem.Weight);
            Assert.AreEqual(expectedItem.Length, actualItem.Length);
            Assert.AreEqual(expectedItem.Height, actualItem.Height);
            Assert.AreEqual(expectedItem.Width, actualItem.Width);
        }
        for (int i = 0; i < expectedItem.sales.Count(); i++)
        {
            MyAssert.StringsEqual(expectedItem.sales[i].get_Date_Sold().toDateString(), actualItem.sales[i].get_Date_Sold().toDateString());
            Assert.AreEqual(expectedItem.sales[i].get_Amount_sale(), actualItem.sales[i].get_Amount_sale());
        }
    }

    internal static void ItemsEqualWithoutSales(Item expectedItem, Item actualItem)
    {
        Assert.AreEqual(expectedItem.ITEM_ID, actualItem.ITEM_ID);
        Assert.AreEqual(expectedItem.Name, actualItem.Name);
        Assert.AreEqual(expectedItem.Amount_purchase, actualItem.Amount_purchase);
        Assert.AreEqual(expectedItem.Date_Purchased, actualItem.Date_Purchased);
        Assert.AreEqual(expectedItem.InitialQuantity, actualItem.InitialQuantity);
        Assert.AreEqual(expectedItem.CurrentQuantity, actualItem.CurrentQuantity);

        if (expectedItem.hasShippingEntry())
        {
            Assert.AreEqual(expectedItem.Weight, actualItem.Weight);
            Assert.AreEqual(expectedItem.Length, actualItem.Length);
            Assert.AreEqual(expectedItem.Height, actualItem.Height);
            Assert.AreEqual(expectedItem.Width, actualItem.Width);
        }
    }
}
