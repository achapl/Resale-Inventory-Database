using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace FinancialDatabase
{
    public partial class CustomControl3 : Control
    {

        int mainImageHeight;
        int mainImageWidth;
        double mainImageAspectRatio;
        Image mainImage;

        public CustomControl3()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            mainImageHeight = this.Size.Height;
            mainImageWidth = this.Size.Width;
            mainImageAspectRatio = (double)mainImageHeight / (double)mainImageWidth;
            pe.Graphics.Clear(BackColor);
            base.OnPaint(pe);
            drawMainImage(pe);

        }

        private void drawMainImage(PaintEventArgs pe)
        {
            if (mainImage == null)
            {
                return;
            }

            drawImage(pe, mainImage, getMainImageSize(mainImage), 0, 0);
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
                newSize = new Size(mainImageWidth, (int)Math.Round((double)mainImageWidth / aspectRatio, 0));
            }
            // image W <= H relative to aspect ratio
            // Set H to mainImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double)mainImageHeight / aspectRatio, 0), mainImageHeight);
            }
            return newSize;
        }

        private void drawImage(PaintEventArgs pe, Image image, Size imageSize, int vertOffset, int horizOffset)
        {
            Image resizedImage = new Bitmap(image, imageSize);

            pe.Graphics.DrawImage(resizedImage, new Point(horizOffset, vertOffset));
        }

        public void updatePaint()
        {
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        public void setImage(Image image)
        {
            this.mainImage = image;
            updatePaint();
        }

        public void clearImage()
        {
            mainImage = null;
            updatePaint();
        }

        

    }
}
