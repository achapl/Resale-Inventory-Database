using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace FinancialDatabase
{
    public partial class CustomControl2 : ScrollableControl
    {
        List<Image> imageList;

        int mainImageIndex;

        int auxImageHeight;
        int auxImageWidth;
        int auxImagePadding;
        double auxImageAspectRatio;
        int mainImageHeight;
        int mainImageWidth;
        double mainImageAspectRatio;

        int auxAndMainImagePadding;

        public CustomControl2()
        {
            InitializeComponent();

            mainImageIndex = 0;

            auxImageHeight = 100;
            auxImageWidth = 100;
            auxImagePadding = 10;
            auxImageAspectRatio = (double)auxImageHeight / (double)auxImageWidth;
            
            mainImageHeight = 100;
            mainImageWidth = 100;
            mainImageAspectRatio = (double)mainImageHeight / (double)mainImageWidth;

            auxAndMainImagePadding = 20;

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(this.BackColor);
            base.OnPaint(pe);

            drawAuxImages(pe);
            drawMainImage(pe);

        }

        private void drawAuxImages(PaintEventArgs pe)
        {
            int i = 0;
            if (imageList == null) { return; }
            foreach (Image image in imageList)
            {
                int vertOffset = i * (auxImageHeight + auxImagePadding);
                drawImage(pe, image, getAuxImageSize(image), vertOffset, 0);
            }
        }

        private void drawMainImage(PaintEventArgs pe)
        {
            if (imageList == null || imageList.Count == 0 || mainImageIndex == -1)
            {
                return;
            }

            if (mainImageIndex > imageList.Count - 1)
            {
                throw new Exception("mainImageIndex points to an image that does not exist in the imageList");
            }

            Image mainImage = imageList[mainImageIndex];
            drawImage(pe, mainImage, getMainImageSize(mainImage), VerticalScroll.Value, auxImageWidth + auxAndMainImagePadding);
        }

        private Size getNewSize(Image image, Size maxSize)
        {
            int w = image.Size.Width;
            int h = image.Size.Height;
            double aspectRatio = (double)w / (double)h;

            int maxW = maxSize.Width;
            int maxH = maxSize.Height;
            double maxSizeAspectRatio = (double) maxW / (double) maxH;

            Size newSize;

            // image W > H relative to aspect ratio
            // Set W to auxImageWidth, and use aspect ratio to det. height
            if (aspectRatio > auxImageAspectRatio)
            {
                newSize = new Size(auxImageWidth, (int)Math.Round((double)w / aspectRatio, 0));
            }
            // image W <= H relative to aspect ratio
            // Set H to auxImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double)h / aspectRatio, 0), auxImageHeight);
            }
            return newSize;
        }

        public Size getAuxImageSize(Image image)
        {
            int w = image.Size.Width;
            int h = image.Size.Height;
            double aspectRatio = (double) w / (double) h;

            Size newSize;

            // image W > H relative to aspect ratio
            // Set W to auxImageWidth, and use aspect ratio to det. height
            if (aspectRatio > auxImageAspectRatio)
            {
                newSize = new Size(auxImageWidth, (int) Math.Round((double) w/aspectRatio,0));
            }
            // image W <= H relative to aspect ratio
            // Set H to auxImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double) h / aspectRatio, 0), auxImageHeight);
            }
            return newSize;
        }

        public Size getMainImageSize(Image image)
        {
            int w = image.Size.Width;
            int h = image.Size.Height;
            double aspectRatio = (double)w / (double)h;

            Size newSize;

            // image W > H relative to aspect ratio
            // Set W to mainImageWidth, and use aspect ratio to det. height
            if (aspectRatio > mainImageAspectRatio)
            {
                newSize = new Size(mainImageWidth, (int)Math.Round((double)w / aspectRatio, 0));
            }
            // image W <= H relative to aspect ratio
            // Set H to mainImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double)h / aspectRatio, 0), mainImageHeight);
            }
            return newSize;
        }

        private void drawImage(PaintEventArgs pe, Image image, Size imageSize, int vertOffset, int horizOffset)
        {
            Image resizedImage = new Bitmap(image, imageSize);

            pe.Graphics.DrawImage(resizedImage, new Point(horizOffset, vertOffset));
        }

        public void addImage(Image image)
        {
            if (!imageList.Contains(image))
            {
                imageList.Add(image);
            }

            updatePaint();
        }

        public void clearImages()
        {
            if (imageList != null)
                imageList.Clear();
            mainImageIndex = -1;
            updatePaint();
        }

        public void setImage(int index)
        {
            mainImageIndex = index;
            updatePaint();
        }

        public void setImages(List<Image> images)
        {
            clearImages();
            this.imageList = images;
            updatePaint();
        }

        public void updatePaint()
        {
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }
    }
}
