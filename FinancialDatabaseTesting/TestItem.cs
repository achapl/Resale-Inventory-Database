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
        "item.ITEM_ID",
        "item.Name",
        "item.PurchaseID",
        "item.SaleID",
        "item.ShippingID",
        "item.InitialQuantity",
        "item.CurrentQuantity",
        "item.Notes_item",
        "sale.Amount_sale",
        "purchase.Tax",
        "fee.FEE_ID",
        "fee.Date",
        "fee.ApplicableTableID",
        "fee.ApplicableTableName",
        "fee.Amount",
        "fee.Type",
        "purchase.Seller",
        "purchase.Notes_purchase",

        
        "item.ThumbnailID",
        "purchase.PURCHASE_ID",
        "purchase.Date_Purchased",
        "purchase.Amount_purchase",
        "purchase.Fees_purchase",
        "sale.SALE_ID",
        "sale.Date_Sold",
        "sale.Fees_sale",
        "sale.Buyer",
        "sale.ItemID_sale",
        "shipping.SHIPPING_ID",
        "shipping.Length",
        "shipping.Width",
        "shipping.Height",
        "shipping.Weight",
        "shipping.ItemID_shipping",
        "shipping.Notes_shipping",
        "thumbnail.ThumbnailID",
        "thumbnail.thumbnail",
        "shipping.WeightLbs",
        "shipping.WeightOz"
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
    [TestCase(5, "Name", 5, 5, 5, 5, 5, "Item Notes",
        5.0, 5.0, 5.0, "Seller", "Purchase Notes",
        5, 5, 5, 5, "Shipping Notes")]

    public static void Test_ConstructorList(// Item
                                            int? ITEM_ID, string Name, int? PurchaseID, int? SaleID, int? ShippingID, int? InitialQuantity, int? CurrentQuantity, string Notes_item,
                                            // Purchase
                                            double? Amount_purchase, double? Tax, double? Fees_purchase, string Seller, string Notes_purchase,
                                            // Shipping
                                            int? Length, int? Width, int? Height, int? Weight, string Notes_shipping)
    { 

        string thumbnail = System.Drawing.Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png").ToString();
        Util.Date Date_Purchased = new Util.Date(1978, 12, 16);

        List<string> itemAttribs = new List<string>
            {
                ITEM_ID.ToString(), Name, PurchaseID.ToString(), SaleID.ToString(), ShippingID.ToString(), InitialQuantity.ToString(), CurrentQuantity.ToString(), Notes_item.ToString(), thumbnail.ToString(),
                Date_Purchased.ToString(), Amount_purchase.ToString(), Tax.ToString(), Fees_purchase.ToString(), Seller.ToString(), Notes_purchase.ToString(),
                Length.ToString(), Width.ToString(), Height.ToString(), Weight.ToString(), Notes_shipping.ToString()
            };


        Item item = new Item(itemAttribs, colNames);
        Assert.Equals(ITEM_ID, item.get_ITEM_ID());
        Assert.Equals(Name, item.get_Name());
        Assert.Equals(PurchaseID, item.get_PurchaseID());
        Assert.Equals(SaleID, item.get_SaleID());
        Assert.Equals(ShippingID, item.get_ShippingID());
        Assert.Equals(InitialQuantity, item.get_InitialQuantity());
        Assert.Equals(CurrentQuantity, item.get_CurrentQuantity());
        Assert.Equals(Notes_item, item.get_Notes_item());
        Assert.Equals(thumbnail, item.get_Thumbnail());
        Assert.Equals(Date_Purchased, item.get_Date_Purchased());
        Assert.Equals(Amount_purchase, item.get_Amount_purchase());
        Assert.Equals(Tax, item.get_Tax());
        Assert.Equals(Fees_purchase, item.get_Fees_purchase());
        Assert.Equals(Seller, item.get_Seller());
        Assert.Equals(Notes_purchase, item.get_Notes_purchase());
        Assert.Equals(Length, item.get_Length());
        Assert.Equals(Width, item.get_Width());
        Assert.Equals(Height, item.get_Height());
        Assert.Equals(Weight, item.get_Weight());
        Assert.Equals(Notes_shipping, item.get_Notes_shipping());
    }
}

