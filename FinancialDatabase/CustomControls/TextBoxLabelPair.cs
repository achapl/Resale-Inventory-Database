namespace FinancialDatabase
{
    public partial class TextBoxLabelPair : ControlLabelPair
    {
        public TextBoxLabelPair() : base()
        {
            InitializeComponent();
            control = new TextBox();
        }

        protected override void initializeControl(Object e, EventArgs ea)
        {
            control.Parent = this.Parent;
            Parent.Controls.Add(control);
            control.Location = new Point(this.Location.X, this.Location.Y - 3);
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

        public bool hasDoubleText()
        {
            return Double.TryParse(control.Text, out double _);
        }

        public bool hasIntText()
        {
            return Int32.TryParse(control.Text, out int _);
        }
    }
}
