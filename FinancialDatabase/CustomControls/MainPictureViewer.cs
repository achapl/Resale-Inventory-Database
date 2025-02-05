namespace FinancialDatabase
{
    /// <summary>
    /// When viewing an image with multiple auxiliary images in the ItemViewTab
    /// This is the large main image
    /// </summary>
    public partial class MainPictureViewer : Control
    {

        int mainImageHeight;
        int mainImageWidth;
        double mainImageAspectRatio;
        MyImage mainImage;

        public MainPictureViewer()
        {
            InitializeComponent();
        }

        public Image getMainImage()
        {
            if (mainImage == null)
            {
                return null;
            }

            return mainImage.image;
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

            drawImage(pe, mainImage.image, getMainImageSize(mainImage), 0, 0);
        }


        public Size getMainImageSize(MyImage image)
        {
            int w = image.image.Size.Width;
            int h = image.image.Size.Height;
            double aspectRatio = (double)w / (double)h;

            Size newSize = Util.getImageSizeFittedIntoMaxDims(image, new Size(mainImageWidth, mainImageHeight));

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

        public void setImage(MyImage image)
        {
            this.mainImage = image;
            updatePaint();
        }

        public void clearImage()
        {
            mainImage = null;
            updatePaint();
        }

        public int getCurrImageID()
        {
            return mainImage.imageID; 
        }
    }
}
