using FinancialDatabase.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FinancialDatabaseTesting
{
    internal class TestingUtil
    {
        public static Image defImage = Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png");

        public static Random random = new Random();

        public static void setDatabaseTesting(bool testingVal)
        {
            var DtbObject = typeof(Database);
            var testingVar = DtbObject.GetField("TESTING", BindingFlags.NonPublic | BindingFlags.Static);
            testingVar.SetValue(null, testingVal);
        }


        public static bool compareImages(MyImage image1, MyImage image2, bool resizeLargerimage)
        {
            return compareImages(image1.image, image2.image, resizeLargerimage) && image1.imageID == image2.imageID;
        }
        
        public static bool compareImages(MyImage image1, MyImage image2)
        {
            return compareImages(image1.image, image2.image) && image1.imageID == image2.imageID;
        }

        public static bool compareImages(Image image1, Image image2)
        {
            return compareImages(image1, image2, false);
        }

        public static bool compareImages(Image image1, Image image2, bool resizeLargerImage)
        {
            Bitmap bmp1;
            Bitmap bmp2;

            // Resize larger image to be same size as smaller one
            if (resizeLargerImage && image1.Height > image2.Height)
            {
                bmp2 = new Bitmap(image2);
                bmp1 = new Bitmap(image1, bmp2.Size);
            }
            else if (resizeLargerImage && image1.Height < image2.Height)
            {
                bmp1 = new Bitmap(image1);
                bmp2 = new Bitmap(image2, bmp1.Size);
            }
            else
            {
                bmp1 = new Bitmap(image1);
                bmp2 = new Bitmap(image2);
            }

            for (int i = 0; i < bmp1.Width; i++)
            {
                for (int j = 0; j < bmp1.Height; j++)
                {
                    if (!bmp1.GetPixel(i, j).Equals(bmp2.GetPixel(i, j)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static List<Purchase> getTestingItems()
        {
            string inFile = File.ReadAllText("C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabaseTesting\\TestingItems.json");
            JsonNode wholeFile = JsonArray.Parse(inFile);
            List<Purchase> purchases = wholeFile.Deserialize<List<Purchase>>();

            foreach (Purchase purchase in purchases)
            {
                purchase.Date_Purchased = new Util.Date(purchase.Date_Purchased_str);
                if (purchase.items != null)
                {
                    foreach (Item item in purchase.items)
                    {
                        item.set_Date_Purchased(purchase.Date_Purchased);
                        item.set_Amount_purchase(purchase.Amount_purchase);
                        item.set_Seller(purchase.Seller);
                        item.set_Fees_purchase(purchase.Fees_purchase);
                        item.set_Tax(purchase.Tax);
                        item.set_Notes_purchase(purchase.Notes_purchase);
                        if (item.sales != null)
                        {
                            foreach (Sale sale in item.sales)
                            {
                                sale.set_Date_Sold(new Util.Date(sale.Date_Sold_str));
                            }
                        } else
                        {
                            item.sales = new List<Sale>();
                        }
                    }
                }
            }

            return purchases;
        }

        internal static void setTabTesting(bool testingVal)
        {
            var Tab = typeof(Tab);
            var testingVar = Tab.GetField("TESTING", BindingFlags.NonPublic | BindingFlags.Static);
            testingVar.SetValue(null, testingVal);
        }
    }

    
}
