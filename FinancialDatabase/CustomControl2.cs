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


        int auxImageHeight;
        int auxImageWidth;
        int auxImagePadding;
        double auxImageAspectRatio;
        

        public CustomControl2()
        {
            InitializeComponent();

            this.AutoScrollMinSize = this.Size;

            auxImagePadding = 5;
            

            

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            auxImageHeight = this.Size.Width; // Not a typo. auxImage max dimensions should be a square that fits in the width of the component
            auxImageWidth = this.Size.Width;
            auxImageAspectRatio = (double)auxImageHeight / (double)auxImageWidth;

            pe.Graphics.Clear(this.BackColor);
            pe.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            
            base.OnPaint(pe);

            drawAuxImages(pe);
            

        }

        private void drawAuxImages(PaintEventArgs pe)
        {
            int i = 0;
            if (imageList == null) { return; }
            int vertOffset = 0;
            foreach (Image image in imageList)
            {
                vertOffset = i++ * (auxImageHeight + auxImagePadding);
                drawImage(pe, image, getAuxImageSize(image), vertOffset, 0);
            }
            if (vertOffset + auxImageHeight > this.AutoScrollMinSize.Height) { this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Width, vertOffset + auxImageHeight); }
            if (vertOffset + auxImageHeight < this.AutoScrollMinSize.Height + auxImageHeight) { this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Width, vertOffset + auxImageHeight + auxImageHeight); }
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
                newSize = new Size(auxImageWidth, (int) Math.Round((double) auxImageWidth/aspectRatio,0));
            }
            // image W <= H relative to aspect ratio
            // Set H to auxImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double) auxImageHeight / aspectRatio, 0), auxImageHeight);
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
            updatePaint();
        }

        

        public void setImages(List<Image> images)
        {
            if (images == null || images.Count == 0) { return; }
            clearImages();
            this.imageList = images;
            updatePaint();
        }

        public void updatePaint()
        {
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        internal int getRowNum(int y)
        {
            double height = this.auxImageHeight + this.auxImagePadding;
            int scrollAmount = this.VerticalScroll.Value;
            y += scrollAmount;
            return (int)Math.Ceiling((double)y / height) - 1;
        }

        internal Image getImage(int currIndex)
        {
            if (currIndex < 0) { return Util.DEFAULT_IMAGE; }
            if (currIndex > imageList.Count) { throw new Exception("ERROR: Trying to access image that is outside of bounds of imageList"); }
            return imageList[currIndex];
        }
    }
}
