using Image = System.Drawing.Image;

namespace FinancialDatabase
{
    public partial class AllPictureViewer : ScrollableControl
    {
        List<MyImage> imageList;


        int auxImageHeight;
        int auxImageWidth;
        int auxImagePadding;
        double auxImageAspectRatio;
        

        public AllPictureViewer()
        {
            InitializeComponent();

            this.AutoScrollMinSize = this.Size;

            auxImagePadding = 5;
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            int imageSquareSize = this.Size.Width;
            auxImageHeight = imageSquareSize;
            auxImageWidth = imageSquareSize;
            auxImageAspectRatio = (double)auxImageHeight / auxImageWidth;

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
            foreach (MyImage pair in imageList)
            {
                Image image = pair.image;
                vertOffset = i++ * (auxImageHeight + auxImagePadding);
                drawImage(pe, image, getAuxImageSize(image), vertOffset, 0);
            }

            reAdjustAutoScrollMinSize();
        }


        /// <summary>
        /// Readjust AutoScrollMinSize to encompass the height of all current pictures (totalImagesHeight)
        /// and "round" the AutoScrollMinSize to the nearest image cell size
        /// AutoScrollMinSize is the area when objects are drawn outside of it, it will be scroll-able to get to it
        /// </summary>
        private void reAdjustAutoScrollMinSize()
        {
            int imageCellHeight = auxImageHeight + auxImagePadding;
            int totalImagesHeight = imageList.Count() * (imageCellHeight);

            // Account for taking off padding below the last image
            if (imageList.Count() > 0)
            {
                totalImagesHeight -= auxImagePadding;
            }
            
            this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Width, totalImagesHeight);
        }


        private Size getNewSize(Image image, Size maxSize)
        {
            int w = image.Size.Width;
            int h = image.Size.Height;
            double aspectRatio = (double)w / (double)h;

            double maxSizeAspectRatio = (double)maxSize.Width / maxSize.Height;

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
            double aspectRatio = (double)image.Size.Width / image.Size.Height;

            Size newSize;

            // image W > H relative to aspect ratio
            // Set W to auxImageWidth, and use aspect ratio to det. height
            if (aspectRatio > auxImageAspectRatio)
            {
                newSize = new Size(auxImageWidth, (int) Math.Round((double) auxImageWidth / aspectRatio, 0));
            }
            // image W <= H relative to aspect ratio
            // Set H to auxImageWidth, and use aspect ratio to det. with
            else
            {
                newSize = new Size((int)Math.Round((double) auxImageHeight * aspectRatio, 0), auxImageHeight);
            }
            return newSize;
        }

        
        private void drawImage(PaintEventArgs pe, Image image, Size imageSize, int vertOffset, int horizOffset)
        {
            Image resizedImage = new Bitmap(image, imageSize);

            pe.Graphics.DrawImage(resizedImage, new Point(horizOffset, vertOffset));
        }


        public void addImage(MyImage image)
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
            {
                imageList.Clear();
            }

            updatePaint();
        }


        public void setImages(List<MyImage> images)
        {
            if (images == null || images.Count == 0)
            {
                return;
            }

            clearImages();
            this.imageList = images;
            updatePaint();
        }


        public void updatePaint()
        {
            InvokePaint(this, new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }


        public int getRowNum(int y)
        {
            double cellHeight = this.auxImageHeight + this.auxImagePadding;
            int amountScrolled = this.VerticalScroll.Value;
            y += amountScrolled;

            int rowNumClicked = (int)Math.Ceiling((double)y / cellHeight) - 1;
            int rowNum = Math.Min(rowNumClicked, imageList.Count - 1);

            return rowNum;
        }

        public MyImage getImage(int currIndex)
        {
            if (currIndex < 0) { return Util.DEFAULT_IMAGE; }
            if (currIndex > imageList.Count) { throw new Exception("ERROR: Trying to access image that is outside of bounds of imageList"); }
            return imageList[currIndex];
        }
    }
}
