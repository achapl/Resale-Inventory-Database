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
        public bool inEditMode { get; private set; }

        public TextBoxLabelPair()
        {
            this.Visible = false;
            inEditMode = false;
            InitializeComponent();
            label = new Label();

            ParentChanged += ParentChangedMethod;
            this.LocationChanged += UpdatedAttributes;
            this.VisibleChanged += VisibilityChangedMethod;
            this.Click += Clicked;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        internal void Clicked(Object e, EventArgs ea)
        {
            viewMode();
        }

        internal void ParentChangedMethod(Object e, EventArgs ea)
        {
            if (Parent != null)
            {
                Parent.Controls.Add(label);
            }
        }

        internal void UpdatedAttributes(Object e, EventArgs ea)
        {
            label.Location = new Point(this.Location.X, this.Location.Y+30);
            label.Size = new Size(100, 30);
            label.BackColor = Color.Transparent;
            label.Text = "This Is A Test!!!";
            label.Name = "Label-" + this.Name;
            label.Visible = true;
            label.Click += labelClicked;
        }

        internal void VisibilityChangedMethod(Object e, EventArgs ea)
        {
            if (label is null) throw new Exception("Error TextBoxLabelPair: Trying to change label visibility when label does not exist yet, meaning this object was not added to its parent's control");
            label.Visible = !this.Visible;
        }

        internal void labelClicked(Object e, EventArgs ea)
        {
            flipEditMode();
        }

        public string getLabelText()
        {
            if (label is null) throw new Exception("Error TextBoxLabelPair: Trying to access label text when label does not exist yet, meaning this object was not added to its parent's control");
            return label.Text;
        }

        public string getTextBoxText()
        {
            return this.Text;
        }

        public void flipEditMode()
        {
            if (inEditMode)
            {
                viewMode();
            } else
            {
                editMode();
            }
        }


        public void editMode()
        {
            if (label is null) throw new Exception("Error TextBoxLabelPair: Trying to switch into edit mode when label does not exist yet, meaning this object was not added to its parent's control");

            label.Visible = false;
            this.Visible = true;
            inEditMode = true;

            this.Text = label.Text;
        }


        public void viewMode()
        {
            if (label is null) throw new Exception("Error TextBoxLabelPair: Trying to switch into view mode when label does not exist yet, meaning this object was not added to its parent's control");
            label.Visible = true;
            this.Visible = false;
            inEditMode = false;

            label.Text = this.Text;
        }
    }
}
