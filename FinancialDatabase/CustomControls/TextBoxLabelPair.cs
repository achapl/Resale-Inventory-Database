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
        TextBox textBox;
        public TextBoxLabelPair() : base()
        {
            InitializeComponent();
        }

        protected override void UpdatedAttributes(Object e, EventArgs ea)
        {
            textBox.Location = new Point(this.Location.X, this.Location.Y);
            textBox.Size = new Size(100, 30);
            textBox.Name = "TextBox-" + this.Name;
            textBox.Visible = false;
        }

        public override string getControlValue()
        {
            return getControlValueAsStr();
        }
        public override string getControlValueAsStr()
        {
            if (textBox is null) throw new Exception("Error TextBoxLabelPair: Trying to access TextBox value when control does not exist yet, meaning this object was not added to its parent's controls");
            return textBox.Text;
        }

        public void setBackgroundColor(Color color)
        {
            textBox.BackColor = color;
        }

        public override void setControlVal(string newText)
        {
            textBox.Text = newText;
        }

        public override void updateControlValWithLabelText()
        {
            setControlVal(getLabelText());
        }

        public override bool userChangedValue()
        {
            return this.Text.CompareTo(textBox.Text) != 0;
        }
    }
}
