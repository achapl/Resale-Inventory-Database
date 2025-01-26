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
    public partial class TextBoxLabelPair : TextBox
    { 
        Label label;

        public TextBoxLabelPair()
        {
            InitializeComponent();
            label = new Label();

            ParentChanged += ParentChangedMethod;
            this.LocationChanged += UpdatedAttributes;
            this.VisibleChanged += VisibilityChangedMethod;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        internal void ParentChangedMethod(Object e, EventArgs ea)
        {
            Parent.Controls.Add(label);
        }

        internal void UpdatedAttributes(Object e, EventArgs ea)
        {
            label.Location = new Point(this.Location.X+20, this.Location.Y+30);
            label.Size = new Size(100, 30);
            label.BackColor = Color.Transparent;
            label.Text = "This Is A Test!!!";
            label.Name = "Label-" + this.Name;
            label.Visible = true;
            label.Click += labelClicked;
        }

        internal void VisibilityChangedMethod(Object e, EventArgs ea)
        {
            label.Visible = !this.Visible;
        }

        internal void labelClicked(Object e, EventArgs ea)
        {
            this.Visible = true;
        }

        public bool getLabelVisible()
        {
            return label.Visible;
        }

        public string getLabelText()
        {

        }

        public string getTextBoxText()
        {

        }

        public void flipVisibility()
        {

        }


        public void editMode()
        {

        }


        public void viewMode()
        {

        }
    }
}
