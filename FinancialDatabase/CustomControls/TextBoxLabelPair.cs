using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace FinancialDatabase
{
    public partial class TextBoxLabelPair : ControlLabelPair
    {
        public TextBoxLabelPair() : base()
        {
            InitializeComponent();
            control = new TextBox();
        }

        protected override void UpdatedAttributes(Object e, EventArgs ea)
        {
            //if (control is null) { control = new TextBox(); }
            control.Parent = this.Parent;
            Parent.Controls.Add(control);
            control.Location = new Point(this.Location.X, this.Location.Y);
            control.Size = new Size(100, 30);
            control.Name = "TextBox-" + this.Name;
            control.Visible = false;
        }

        public override string getControlValue()
        {
            return getControlValueAsStr();
        }
        public override string getControlValueAsStr()
        {
            if (control is null) throw new Exception("Error TextBoxLabelPair: Trying to access TextBox value when control does not exist yet, meaning this object was not added to its parent's controls");
            return control.Text;
        }

        public void setBackgroundColor(Color color)
        {
            (control as TextBox).BackColor = color;
        }

        public override void setControlVal(string newText)
        {
            (control as TextBox).Text = newText;
        }

        public override void updateControlValWithLabelText()
        {
            setControlVal(getLabelText());
        }

        public override bool userChangedValue()
        {
            return this.Text.CompareTo((control as TextBox).Text) != 0;
        }
    }
}
