using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinancialDatabase
{
    public partial class CustomControl1 : ScrollableControl
    {

        int rowHeight = 50;
        int rowPadding = 3;
        int maxImgWidth = 50;
        double textSizeToRowHeightRatio = 0.3;
        int imageTextPadding = 10;
        
        private List<row> rowList = new List<row>();

        public struct row
        {
            public Image i;
            public string name;
            public row(Image i, string name)
            {
                this.i = i;
                this.name = name;
            }
        }
        

        public CustomControl1()
        {
            this.AutoScrollMinSize = this.Size;
            InitializeComponent();
        }

        public void addRow(Image i, string text)
        {
            Size newSize;
            if (i.Height > i.Width)
            {
                newSize = new Size((int)(rowHeight * ((double)i.Width / (double)i.Height)), rowHeight);
            }
            else
            {
                newSize = new Size(maxImgWidth,(int)(maxImgWidth * ((double)i.Height / (double)i.Width)));
            }
            rowList.Add(new row((Image)new Bitmap(i, newSize), text));
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            pe.Graphics.DrawRectangle(new Pen(this.BackColor), new Rectangle(this.Location, this.Size));

            drawRows(pe);
            
        }

        protected void drawRows(PaintEventArgs pe)
        {
            
            int rowCount = 0;
            foreach (row row in rowList)
            {
                drawRow(pe, row, rowCount++);
            }
        }

        protected void drawRow(PaintEventArgs pe, row r, int rowNum)
        {
            int maxRows = this.rowList.Count();
            if (maxRows * (rowHeight + rowPadding) > this.AutoScrollMinSize.Height) { this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Height, maxRows * (rowHeight + rowPadding)); }
            if (maxRows * (rowHeight + rowPadding) < this.AutoScrollMinSize.Height + (rowHeight + rowPadding)) { this.AutoScrollMinSize = new Size(this.AutoScrollMinSize.Height, (maxRows+1) * (rowHeight + rowPadding)); }
            pe.Graphics.DrawImage(r.i, new Point(0,rowNum*(rowHeight + rowPadding)));
            pe.Graphics.DrawString(r.name, this.Font, new SolidBrush(Color.Black), new PointF(maxImgWidth + imageTextPadding, rowNum * (rowHeight + rowPadding) + rowHeight / 2 - this.FontHeight / 2));
        }

        internal int getRowNum(int y)
        {
            double height = this.rowHeight + this.rowPadding;
            return (int) Math.Ceiling((double)y / height) - 1; 
        }

        internal void clearItems()
        {
            this.rowList.Clear();
            this.Update();

        }
    }
}
