using System;
using System.Drawing;
using System.Xml.Linq;
using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using NUnit.Framework.Internal.Execution;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FinancialDatabaseTesting;
public class TestItem
{

    public static List<string> colNames = new List<string>
    {
        "ITEM_ID",
        "Name",
        "PurchaseID",
        "SaleID",
        "ShippingID",
        "InitialQuantity",
        "CurrentQuantity",
        "Notes_item",
        "thumbnail",
        "Date_Purchased",
        "Amount_purchase",
        "Tax",
        "Fees_purchase",
        "Seller",
        "Notes_purchase",
        "Length",
        "Width",
        "Height",
        "Weight",
        "Notes_shipping",
    };

    public static List<string> colNamesWTable = new List<string>
    {
        "item.ITEM_ID",
        "item.Name",
        "item.PurchaseID",
        "item.",
        "item.ShippingID",
        "item.CurrentQuantity",
        "item.Notes_item",
        "thumbnail.thumbnail",
        "purchase.Date_Purchased",
        "purchase.Amount_purchase",
        "purchase.Tax",
        "purchase.Fees_purchase",
        "purchase.Seller",
        "purchase.Notes_purchase",
        "shipping.Length",
        "shipping.Width",
        "shipping.Height",
        "shipping.Weight",
        "shipping.Notes_shipping",
    };

    public static MyImage image1 = new MyImage(System.Drawing.Image.FromFile("C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabase\\Resources\\NoImage.png"), -2);
    public static MyImage image2 = new MyImage(System.Drawing.Image.FromFile("C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabase\\Resources\\s-l1600.png"), -2);


    [Test]
    public static void Test_ConstructorDefault()
    {
        Item i = new Item();
        // From item table
        Assert.AreEqual(i.ITEM_ID, Util.DEFAULT_INT);
        Assert.AreEqual(i.Name, Util.DEFAULT_STRING);
        Assert.AreEqual(i.PurchaseID, Util.DEFAULT_INT);
        Assert.AreEqual(i.ShippingID, Util.DEFAULT_INT);
        Assert.AreEqual(i.InitialQuantity, Util.DEFAULT_INT);
        Assert.AreEqual(i.CurrentQuantity, Util.DEFAULT_INT);
        Assert.AreEqual(i.Notes_item, Util.DEFAULT_STRING);

        // From purchase table
        Assert.AreEqual(i.Date_Purchased, Util.DEFAULT_DATE);
        Assert.AreEqual(i.Amount_purchase, Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.Tax, Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.Fees_purchase, Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.Seller, Util.DEFAULT_STRING);
        Assert.AreEqual(i.Notes_purchase, Util.DEFAULT_STRING);

        // From shipping table
        Assert.AreEqual(i.Length, Util.DEFAULT_INT);
        Assert.AreEqual(i.Width, Util.DEFAULT_INT);
        Assert.AreEqual(i.Height, Util.DEFAULT_INT);
        Assert.AreEqual(i.Weight, Util.DEFAULT_INT);
        Assert.AreEqual(i.Notes_shipping, Util.DEFAULT_STRING);

        //From image table
        Assert.AreEqual(i.images, Util.DEFAULT_IMAGES);
    }


    /* TEMPLATES
     * [TestCase(,"", , , , , , "",
        new Util.Date(, , ), , , , "", "",
        new Util.Date(, , ), , , "",
         , , , , "")]
    
     [TestCase(null,"", null, null, null, null, null, "",
        new Util.Date(, , ), null, null, null, "", "",
        new Util.Date(, , ), null, null, "",
         null, null, null, null, "")]*/
    [TestCase(5, "Name", 5, 6, 7, 8, 9, "Item Notes",
        10.0, 11.0, 12.0, "Seller", "Purchase Notes",
        13, 14, 15, 16, "Shipping Notes")]

    public static void Test_ConstructorList(// Item
                                            int? ITEM_ID, string Name, int? PurchaseID, int? SaleID, int? ShippingID, int? InitialQuantity, int? CurrentQuantity, string Notes_item,
                                            // Purchase
                                            double? Amount_purchase, double? Tax, double? Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            int? Length, int? Width, int? Height, int? Weight, string Notes_shipping)
    {

        string thumbnailRAW = BitConverter.ToString(File.ReadAllBytes(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png"));
        MyImage thumbnail = new MyImage(Util.rawImageStrToImage(thumbnailRAW), -1);
        Util.Date Date_Purchased = new Util.Date(1978, 12, 16);

        List<string> itemAttribs = new List<string>
            {
                ITEM_ID.ToString(), Name, PurchaseID.ToString(), SaleID.ToString(), ShippingID.ToString(), InitialQuantity.ToString(), CurrentQuantity.ToString(), Notes_item.ToString(), thumbnailRAW.ToString(),
                Date_Purchased.toDateString(), Amount_purchase.ToString(), Tax.ToString(), Fees_purchase.ToString(), Seller.ToString(), Notes_purchase.ToString(),
                Length.ToString(), Width.ToString(), Height.ToString(), Weight.ToString(), Notes_shipping.ToString()
            };


        Item item = new Item(itemAttribs, colNames);
        Assert.AreEqual(ITEM_ID, item.ITEM_ID);
        Assert.AreEqual(Name, item.Name);
        Assert.AreEqual(PurchaseID, item.PurchaseID);
        Assert.AreEqual(ShippingID, item.ShippingID);
        Assert.AreEqual(InitialQuantity, item.InitialQuantity);
        Assert.AreEqual(CurrentQuantity, item.CurrentQuantity);
        Assert.AreEqual(Notes_item, item.Notes_item);
        Assert.AreEqual(Date_Purchased, item.Date_Purchased);
        Assert.AreEqual(Amount_purchase, item.Amount_purchase);
        Assert.AreEqual(Tax, item.Tax);
        Assert.AreEqual(Fees_purchase, item.Fees_purchase);
        Assert.AreEqual(Seller, item.Seller);
        Assert.AreEqual(Notes_purchase, item.Notes_purchase);
        Assert.AreEqual(Length, item.Length);
        Assert.AreEqual(Width, item.Width);
        Assert.AreEqual(Height, item.Height);
        Assert.AreEqual(Weight, item.Weight);
        Assert.AreEqual(Notes_shipping, item.Notes_shipping);

        Assert.IsTrue(TestingUtil.compareImages(thumbnail, item.thumbnail));
    }



    [TestCase(5, "Name", 5, 6, 7, 8, 9, "Item Notes",
        10.0, 11.0, 12.0, "Seller", "Purchase Notes",
        13, 14, 15, 16, "Shipping Notes")]
    public static void Test_gettersAndSettersNormalValues(// Item
                                            int ITEM_ID, string Name, int PurchaseID, int SaleID, int ShippingID, int InitialQuantity, int CurrentQuantity, string Notes_item,
                                            // Purchase
                                            double Amount_purchase, double Tax, double Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            int Length, int Width, int Height, int Weight, string Notes_shipping)
    {

        string thumbnailRAW = BitConverter.ToString(File.ReadAllBytes(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png"));
        MyImage thumbnail = new MyImage(Util.rawImageStrToImage(thumbnailRAW), -1);
        Util.Date Date_Purchased = new Util.Date(1978, 12, 16);

        Item item = new Item();

        item.ITEM_ID = ITEM_ID;
        item.Name = Name;
        item.PurchaseID = PurchaseID;
        item.ShippingID = ShippingID;
        item.InitialQuantity = InitialQuantity;
        item.CurrentQuantity = CurrentQuantity;
        item.Notes_item = Notes_item;
        item.Date_Purchased = Date_Purchased;
        item.Amount_purchase = Amount_purchase;
        item.Tax = Tax;
        item.Fees_purchase = Fees_purchase;
        item.Seller = Seller;
        item.Notes_purchase = Notes_purchase;
        item.Length = Length;
        item.Width = Width;
        item.Height = Height;
        item.Weight = Weight;
        item.Notes_shipping = Notes_shipping;
        item.thumbnail = thumbnail;


        Assert.AreEqual(ITEM_ID, item.ITEM_ID);
        Assert.AreEqual(Name, item.Name);
        Assert.AreEqual(PurchaseID, item.PurchaseID);
        Assert.AreEqual(ShippingID, item.ShippingID);
        Assert.AreEqual(InitialQuantity, item.InitialQuantity);
        Assert.AreEqual(CurrentQuantity, item.CurrentQuantity);
        Assert.AreEqual(Notes_item, item.Notes_item);
        Assert.AreEqual(Date_Purchased, item.Date_Purchased);
        Assert.AreEqual(Amount_purchase, item.Amount_purchase);
        Assert.AreEqual(Tax, item.Tax);
        Assert.AreEqual(Fees_purchase, item.Fees_purchase);
        Assert.AreEqual(Seller, item.Seller);
        Assert.AreEqual(Notes_purchase, item.Notes_purchase);
        Assert.AreEqual(Length, item.Length);
        Assert.AreEqual(Width, item.Width);
        Assert.AreEqual(Height, item.Height);
        Assert.AreEqual(Weight, item.Weight);
        Assert.AreEqual(Notes_shipping, item.Notes_shipping);
        Assert.IsTrue(TestingUtil.compareImages(thumbnail, item.thumbnail));
    }



    [TestCase("5", "Name", "5", "6", "7", "8", "9", "Item Notes",
        "10.0", "11.0", "12.0", "Seller", "Purchase Notes",
        "13", "14", "15", "16", "Shipping Notes")]
    public static void Test_gettersAndSettersStrings(// Item
                                            string ITEM_ID, string Name, string PurchaseID, string SaleID, string ShippingID, string InitialQuantity, string CurrentQuantity, string Notes_item,
                                            // Purchase
                                            string Amount_purchase, string Tax, string Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            string Length, string Width, string Height, string Weight, string Notes_shipping)
    {

        string thumbnailRAW = BitConverter.ToString(File.ReadAllBytes(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png"));
        MyImage thumbnail = new MyImage(Util.rawImageStrToImage(thumbnailRAW), -1);
        Util.Date Date_Purchased = new Util.Date(1978, 12, 16);

        Item item = new Item();

        item.set_ITEM_ID_str(ITEM_ID);
        item.Name = Name;
        item.set_PurchaseID_str(PurchaseID);
        item.set_ShippingID_str(ShippingID);
        item.set_InitialQuantity_str(InitialQuantity);
        item.set_CurrentQuantity_str(CurrentQuantity);
        item.Notes_item = (Notes_item);
        item.Date_Purchased = Date_Purchased;
        item.set_Amount_purchase_str(Amount_purchase);
        item.set_Tax_str(Tax);
        item.set_Fees_purchase_str(Fees_purchase);
        item.Seller = Seller;
        item.Notes_purchase = Notes_purchase;
        item.set_Length_str(Length);
        item.set_Width_str(Width);
        item.set_Height_str(Height);
        item.set_Weight_str(Weight);
        item.Notes_shipping = Notes_shipping;
        item.thumbnail = thumbnail;


        Assert.AreEqual(Int32.Parse(ITEM_ID), item.ITEM_ID);
        Assert.AreEqual(Name, item.Name);
        Assert.AreEqual(Int32.Parse(PurchaseID), item.PurchaseID);
        Assert.AreEqual(Int32.Parse(ShippingID), item.ShippingID);
        Assert.AreEqual(Int32.Parse(InitialQuantity), item.InitialQuantity);
        Assert.AreEqual(Int32.Parse(CurrentQuantity), item.CurrentQuantity);
        Assert.AreEqual(Notes_item, item.Notes_item);
        Assert.AreEqual(Date_Purchased, item.Date_Purchased);
        Assert.AreEqual(Double.Parse(Amount_purchase), item.Amount_purchase);
        Assert.AreEqual(Double.Parse(Tax), item.Tax);
        Assert.AreEqual(Double.Parse(Fees_purchase), item.Fees_purchase);
        Assert.AreEqual(Seller, item.Seller);
        Assert.AreEqual(Notes_purchase, item.Notes_purchase);
        Assert.AreEqual(Int32.Parse(Length), item.Length);
        Assert.AreEqual(Int32.Parse(Width), item.Width);
        Assert.AreEqual(Int32.Parse(Height), item.Height);
        Assert.AreEqual(Int32.Parse(Weight), item.Weight);
        Assert.AreEqual(Notes_shipping, item.Notes_shipping);
        Assert.IsTrue(TestingUtil.compareImages(thumbnail, item.thumbnail));
    }



    [Test]
    public static void Test_gettersNoFieldValue()
    {
        Item item = new Item();

        Assert.AreEqual(Util.DEFAULT_INT, item.ITEM_ID);
        Assert.AreEqual(Util.DEFAULT_STRING, item.Name);
        Assert.AreEqual(Util.DEFAULT_INT, item.PurchaseID);
        Assert.AreEqual(Util.DEFAULT_INT, item.ShippingID);
        Assert.AreEqual(Util.DEFAULT_INT, item.InitialQuantity);
        Assert.AreEqual(Util.DEFAULT_INT, item.CurrentQuantity);
        Assert.AreEqual(Util.DEFAULT_STRING, item.Notes_item);
        Assert.AreEqual(Util.DEFAULT_DATE, item.Date_Purchased);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Amount_purchase);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Tax);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Fees_purchase);
        Assert.AreEqual(Util.DEFAULT_STRING, item.Seller);
        Assert.AreEqual(Util.DEFAULT_STRING, item.Notes_purchase);
        Assert.AreEqual(Util.DEFAULT_INT, item.Length);
        Assert.AreEqual(Util.DEFAULT_INT, item.Width);
        Assert.AreEqual(Util.DEFAULT_INT, item.Height);
        Assert.AreEqual(Util.DEFAULT_INT, item.Weight);
        Assert.AreEqual(Util.DEFAULT_STRING, item.Notes_shipping);
        Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE, item.thumbnail));
    }



    [TestCase("", "", "", "", "", "", "", "",
        "", "", "", "", "", "",
        "", "", "", "", "",
        "")]
    public static void Test_gettersAndSettersEmptyStrings(// Item
                                            string ITEM_ID, string Name, string PurchaseID, string SaleID, string ShippingID, string InitialQuantity, string CurrentQuantity, string Notes_item,
                                            // Purchase
                                            string Date_Purchased, string Amount_purchase, string Tax, string Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            string Length, string Width, string Height, string Weight, string Notes_shipping,
                                            // Thumbnail
                                            string thumbnail)
    {

        Item item = new Item();

        Assert.Catch<Exception>(() => item.set_ITEM_ID_str(ITEM_ID));
        item.Name = Name;
        item.set_PurchaseID_str(PurchaseID);
        item.set_ShippingID_str(ShippingID);
        item.set_InitialQuantity_str(InitialQuantity);
        item.set_CurrentQuantity_str(CurrentQuantity);
        item.Notes_item = Notes_item; // Valid to have miscellaneous notes just be empty
        item.set_Date_Purchased_str(Date_Purchased);
        item.set_Amount_purchase_str(Amount_purchase);
        item.set_Tax_str(Tax);
        item.set_Fees_purchase_str(Fees_purchase);
        item.Seller = Seller; // Valid to have Seller just be empty, it is not necessary information
        item.Notes_purchase = Notes_purchase;  // Valid to have miscellaneous notes just be empty
        item.set_Length_str(Length);
        item.set_Width_str(Width);
        item.set_Height_str(Height);
        item.set_Weight_str(Weight);
        item.Notes_shipping = Notes_shipping; // Valid to have miscellaneous notes just be empty
        item.set_Thumbnail(thumbnail);


        Assert.AreEqual("", item.Name);
        Assert.AreEqual(Util.DEFAULT_INT, item.PurchaseID);
        Assert.AreEqual(Util.DEFAULT_INT, item.ShippingID);
        Assert.AreEqual(Util.DEFAULT_INT, item.InitialQuantity);
        Assert.AreEqual(Util.DEFAULT_INT, item.CurrentQuantity);
        Assert.AreEqual("", item.Notes_item);
        Assert.AreEqual(Util.DEFAULT_DATE, item.Date_Purchased);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Amount_purchase);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Tax);
        Assert.AreEqual(Util.DEFAULT_DOUBLE, item.Fees_purchase);
        Assert.AreEqual("", item.Seller);
        Assert.AreEqual("", item.Notes_purchase);
        Assert.AreEqual(Util.DEFAULT_INT, item.Length);
        Assert.AreEqual(Util.DEFAULT_INT, item.Width);
        Assert.AreEqual(Util.DEFAULT_INT, item.Height);
        Assert.AreEqual(Util.DEFAULT_INT, item.Weight);
        Assert.AreEqual("", item.Notes_shipping);
        Assert.IsTrue(TestingUtil.compareImages(Util.DEFAULT_IMAGE, item.thumbnail));
    }



    [TestCase("Name1")]
    [TestCase("")]
    [TestCase("\"\"")]
    [TestCase("\"")]
    public static void Test_set_NameQuotes(string Name)
    {
        Item item = new Item();
        item.Name = "\"" + Name + "\"";
        Assert.AreEqual(0, Name.CompareTo(item.Name));
    }




    [TestCase("5", "Name", "5", "7", "8", "9", "Item Notes",
        "1978-12-16", "10", "10", "12", "Seller", "Purchase Notes",
        "13", "14", "15", "16", "Shipping Notes")]
    public static void Test_getAttribAsStr( // Item
                                            string ITEM_ID, string Name, string PurchaseID, string ShippingID, string InitialQuantity, string CurrentQuantity, string Notes_item,
                                            // Purchase
                                            string Date_Purchased, string Amount_purchase, string Tax, string Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            string Length, string Width, string Height, string Weight, string Notes_shipping)
    {
        Item item = new Item();

        item.set_ITEM_ID_str(ITEM_ID);
        item.Name = Name;
        item.set_PurchaseID_str(PurchaseID);
        item.set_ShippingID_str(ShippingID);
        item.set_InitialQuantity_str(InitialQuantity);
        item.set_CurrentQuantity_str(CurrentQuantity);
        item.Notes_item = Notes_item;
        item.set_Date_Purchased_str(Date_Purchased);
        item.set_Amount_purchase_str(Amount_purchase);
        item.set_Tax_str(Tax);
        item.set_Fees_purchase_str(Fees_purchase);
        item.Seller = Seller;
        item.Notes_purchase = Notes_purchase;
        item.set_Length_str(Length);
        item.set_Width_str(Width);
        item.set_Height_str(Height);
        item.set_Weight_str(Weight);
        item.Notes_shipping = Notes_shipping;


        Assert.AreEqual(0, ITEM_ID.CompareTo(item.getAttribAsStr("item.ITEM_ID")));
        Assert.AreEqual(0, Name.CompareTo(item.getAttribAsStr("item.Name")));
        Assert.AreEqual(0, PurchaseID.CompareTo(item.getAttribAsStr("item.PurchaseID")));
        Assert.AreEqual(0, ShippingID.CompareTo(item.getAttribAsStr("item.ShippingID")));
        Assert.AreEqual(0, InitialQuantity.CompareTo(item.getAttribAsStr("item.InitialQuantity")));
        Assert.AreEqual(0, CurrentQuantity.CompareTo(item.getAttribAsStr("item.CurrentQuantity")));
        Assert.AreEqual(0, Notes_item.CompareTo(item.getAttribAsStr("item.Notes_item")));
        Assert.AreEqual(0, Date_Purchased.CompareTo(item.getAttribAsStr("purchase.Date_Purchased")));
        Assert.AreEqual(0, Amount_purchase.CompareTo(item.getAttribAsStr("purchase.Amount_purchase")));
        Assert.AreEqual(0, Tax.CompareTo(item.getAttribAsStr("purchase.Tax")));
        Assert.AreEqual(0, Fees_purchase.CompareTo(item.getAttribAsStr("purchase.Fees_purchase")));
        Assert.AreEqual(0, Seller.CompareTo(item.getAttribAsStr("purchase.Seller")));
        Assert.AreEqual(0, Notes_purchase.CompareTo(item.getAttribAsStr("purchase.Notes_purchase")));
        Assert.AreEqual(0, Length.CompareTo(item.getAttribAsStr("shipping.Length")));
        Assert.AreEqual(0, Width.CompareTo(item.getAttribAsStr("shipping.Width")));
        Assert.AreEqual(0, Height.CompareTo(item.getAttribAsStr("shipping.Height")));
        Assert.AreEqual(0, Weight.CompareTo(item.getAttribAsStr("shipping.Weight")));
        Assert.AreEqual(0, Notes_shipping.CompareTo(item.getAttribAsStr("shipping.Notes_shipping")));
    }



    [TestCase("item.Name", "2B")]
    [TestCase("item.Notes_item", "3C")]
    [TestCase("purchase.Seller", "4D")]
    [TestCase("purchase.Notes_purchase", "5E")]
    [TestCase("shipping.Notes_shipping", "6F")]
    public static void Test_getAttribSTRING(string attrib, string val)
    {
        Item item = new Item(new List<string> { val }, new List<string> { attrib });
        item.getAttrib(attrib, out string actualVal);
        Assert.AreEqual(0, val.CompareTo(actualVal));
    }



    [TestCase("item.ITEM_ID", 1)]
    [TestCase("item.PurchaseID", 2)]
    [TestCase("item.ShippingID", 3)]
    [TestCase("item.InitialQuantity", 4)]
    [TestCase("item.CurrentQuantity", 5)]
    [TestCase("shipping.Length", 6)]
    [TestCase("shipping.Width", 7)]
    [TestCase("shipping.Height", 8)]
    [TestCase("shipping.Weight", 9)]
    public static void Test_getAttribINT(string attrib, int val)
    {
        Item item = new Item(new List<string> { val.ToString() }, new List<string> { attrib });
        item.getAttrib(attrib, out int actualVal);
        Assert.AreEqual(val, actualVal);
    }



    [TestCase("purchase.Amount_purchase", 1.0)]
    [TestCase("purchase.Tax", 2.0)]
    [TestCase("purchase.Fees_purchase", 3.0)]
    public static void Test_getAttribDouble(string attrib, double val)
    {
        Item item = new Item(new List<string> { val.ToString() }, new List<string> { attrib });
        item.getAttrib(attrib, out double actualVal);
        Assert.AreEqual(val, actualVal);
    }



    [TestCase("purchase.Date_Purchased")]
    public static void Test_getAttribDate(string attrib)
    {
        Util.Date val = new Util.Date(1978, 12, 16);
        Item item = new Item(new List<string> { val.toDateString() }, new List<string> { attrib });
        item.getAttrib(attrib, out Util.Date actualVal);
        Assert.AreEqual(val, actualVal);
    }



    [TestCase("", false)]
    [TestCase("namehere", true)]
    [TestCase("\"namehere\"", true)]
    [TestCase("-1", true)]
    [TestCase("\\\"", false)]
    [TestCase("\"", false)]
    public static void Test_isValidName(string name, bool actual)
    {
        Assert.AreEqual(actual, Item.isValidName(name));
    }



    [Test]
    public static void Test_Equals()
    {
        Item itemA = new Item();
        itemA.ITEM_ID = 1234;

        Item itemB = new Item();
        itemB.ITEM_ID = itemA.ITEM_ID;

        Item itemC = new Item();
        itemC.ITEM_ID = itemA.ITEM_ID + 1;

        Assert.IsFalse(itemA.Equals(itemC));
    }


    [Test]
    public static void Test_clear_images()
    {
        Item i = new Item();
        i.add_image(image1);
        i.add_image(image1);
        List<MyImage> imagesBefore = i.images;
        Assert.AreEqual(2, imagesBefore.Count());

        i.clear_images();

        List<MyImage> imagesAfter = i.images;
        Assert.AreEqual(1, imagesAfter.Count());
    }



    [Test]
    public static void Test_add_image()
    {
        Item i = new Item();
        
        List<MyImage> imagesBefore = i.images;
        Assert.IsTrue(imagesBefore.Count() > 0);

        i.add_image(image1);

        List<MyImage> imagesAfter = i.images;
        Assert.AreEqual(1, imagesAfter.Count());
        Assert.IsTrue(TestingUtil.compareImages(image1, imagesAfter[0]));

    }



    [Test]
    public static void Test_add_images()
    {
        Item i = new Item();

        List<MyImage> imagesBefore = i.images;
        Assert.IsTrue(imagesBefore.Count() > 0);

        i.add_image(image1);

        List<MyImage> imagesAfter = i.images;
        Assert.AreEqual(1, imagesAfter.Count());
        Assert.IsTrue(TestingUtil.compareImages(image1, imagesAfter[0]));

        i.add_image(image1);
        i.add_image(image2);

        imagesAfter = i.images;
        Assert.AreEqual(3, imagesAfter.Count());
        Assert.IsTrue(TestingUtil.compareImages(image1, imagesAfter[0]));
        Assert.IsTrue(TestingUtil.compareImages(image1, imagesAfter[1]));
        Assert.IsTrue(TestingUtil.compareImages(image2, imagesAfter[2]));
    }



    [Test]
    public static void Test_set_images()
    {
        Item i = new Item();

        List<MyImage> imagesBefore = i.images;
        Assert.IsTrue(imagesBefore.Count() > 0);

        i.add_images(new List<MyImage> { image1, image2 });

        List<MyImage> imagesAfter = i.images;
        Assert.AreEqual(2, imagesAfter.Count());
        Assert.IsTrue(TestingUtil.compareImages(image1, imagesAfter[0]));
        Assert.IsTrue(TestingUtil.compareImages(image2, imagesAfter[1]));
    }
}

