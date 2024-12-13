namespace FinancialDatabaseTesting;

using FinancialDatabase;
using System.Drawing;

public class TestUtil
{

    Image defImage;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        defImage = Image.FromFile(@"C:\Users\Owner\source\repos\FinancialDatabaseSolution\FinancialDatabase\Resources\NoImage.png");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        defImage.Dispose();
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void TestDefaultVal_Int()
    {
        Assert.AreEqual(-1,Util.DEFAULT_INT);
    }

    [Test]
    public void TestDefaultVal_Double()
    {
        Assert.AreEqual(-1.0, Util.DEFAULT_DOUBLE);
    }

    [Test]
    public void TestDefaultVal_String()
    {
        Assert.AreEqual(null, Util.DEFAULT_STRING);
    }

    [Test]
    public void TestDefaultVal_Date()
    {
        Assert.IsTrue(Util.DEFAULT_DATE.Equals(new Util.Date(-1, -1, -1)));
    }

    [Test]
    public void TestDefaultVal_Image()
    {
        Assert.IsTrue(compareImages(defImage, Util.DEFAULT_IMAGE.image));
        Assert.AreEqual(-1, Util.DEFAULT_IMAGE.imageID);
    }

    [Test]
    public void TestDefaultVal_Images()
    {
        Assert.IsTrue(compareImages(defImage, Util.DEFAULT_IMAGES[0].image));
        Assert.AreEqual(-1, Util.DEFAULT_IMAGES[0].imageID);
    }





    public bool compareImages(Image image1, Image image2)
    {
        Bitmap bmp1 = new Bitmap(image1);
        Bitmap bmp2 = new Bitmap(image2);

        for (int i = 0; i < defImage.Width; i++)
        {
            for (int j = 0; j < defImage.Height; j++)
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