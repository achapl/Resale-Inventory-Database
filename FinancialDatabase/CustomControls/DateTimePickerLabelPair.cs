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
    public partial class DateTimePickerLabelPair : ControlLabelPair
    {
        public DateTimePickerLabelPair() : base()
        {
            InitializeComponent();
            control = new DateTimePicker();
        }

        protected override void UpdatedAttributes(Object e, EventArgs ea)
        {
            control.Parent = this.Parent;
            Parent.Controls.Add(control);
            control.Location = new Point(this.Location.X, this.Location.Y - 5);
            control.Name = "DateTimePicker-" + this.Name;
            control.Visible = false;
        }

        public override string getControlValue()
        {
            return getControlValueAsStr();
        }
        public override string getControlValueAsStr()
        {
            if (control is null) throw new Exception("Error DateTimePickerLabelPair: Trying to access dateTimePicker value when control does not exist yet, meaning this object was not added to its parent's controls");
            return new Util.Date(control).toDateString();
        }


        public override void setControlVal(string dateStr)
        {
            (control as DateTimePicker).Value = new Util.Date(dateStr).toDateTime();
        }


        public void setControlVal(DateTime dateTime)
        {
            (control as DateTimePicker).Value = dateTime;
        }

        public void setControlVal(Util.Date date)
        {
            (control as DateTimePicker).Value = date.toDateTime();
        }

        public override void updateControlValWithLabelText()
        {
            setControlVal(getLabelText());
        }

        public override bool userChangedValue()
        {
            return this.Text.CompareTo(new Util.Date(control).toDateString()) != 0;
        }
    }
}
