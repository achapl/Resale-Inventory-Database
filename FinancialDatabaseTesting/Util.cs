using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialDatabaseTesting
{
    internal class TestingUtil
    {
        public static Image defImage = Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png");


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
    }

    
}
