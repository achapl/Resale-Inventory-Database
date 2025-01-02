using System;
using System.Drawing;
using System.Xml.Linq;
using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
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



    [Test]
    public static void Test_ConstructorDefault()
    {
        Item i = new Item();
        // From item table
        Assert.AreEqual(i.get_ITEM_ID(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Name(), Util.DEFAULT_STRING);
        Assert.AreEqual(i.get_PurchaseID(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_SaleID(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_ShippingID(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_InitialQuantity(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_CurrentQuantity(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Notes_item(), Util.DEFAULT_STRING);

        // From purchase table
        Assert.AreEqual(i.get_Date_Purchased(), Util.DEFAULT_DATE);
        Assert.AreEqual(i.get_Amount_purchase(), Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.get_Tax(), Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.get_Fees_purchase(), Util.DEFAULT_DOUBLE);
        Assert.AreEqual(i.get_Seller(), Util.DEFAULT_STRING);
        Assert.AreEqual(i.get_Notes_purchase(), Util.DEFAULT_STRING);

        // From shipping table
        Assert.AreEqual(i.get_Length(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Width(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Height(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Weight(), Util.DEFAULT_INT);
        Assert.AreEqual(i.get_Notes_shipping(), Util.DEFAULT_STRING);

        //From image table
        Assert.AreEqual(i.get_Images(), Util.DEFAULT_IMAGES);
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
        Assert.AreEqual(ITEM_ID, item.get_ITEM_ID());
        Assert.AreEqual(Name, item.get_Name());
        Assert.AreEqual(PurchaseID, item.get_PurchaseID());
        Assert.AreEqual(SaleID, item.get_SaleID());
        Assert.AreEqual(ShippingID, item.get_ShippingID());
        Assert.AreEqual(InitialQuantity, item.get_InitialQuantity());
        Assert.AreEqual(CurrentQuantity, item.get_CurrentQuantity());
        Assert.AreEqual(Notes_item, item.get_Notes_item());
        Assert.AreEqual(Date_Purchased, item.get_Date_Purchased());
        Assert.AreEqual(Amount_purchase, item.get_Amount_purchase());
        Assert.AreEqual(Tax, item.get_Tax());
        Assert.AreEqual(Fees_purchase, item.get_Fees_purchase());
        Assert.AreEqual(Seller, item.get_Seller());
        Assert.AreEqual(Notes_purchase, item.get_Notes_purchase());
        Assert.AreEqual(Length, item.get_Length());
        Assert.AreEqual(Width, item.get_Width());
        Assert.AreEqual(Height, item.get_Height());
        Assert.AreEqual(Weight, item.get_Weight());
        Assert.AreEqual(Notes_shipping, item.get_Notes_shipping());
        
        Assert.IsTrue(TestingUtil.compareImages(thumbnail, item.get_Thumbnail()));
    }
}

