using FinancialDatabase;
using System;
using Date = Util.Date;

public abstract class Tab
{


    protected bool inEditingState;
    protected List<Control> editingAttributeControls;
    protected List<Label> editableAttribLables;
    protected List<Control> editingControls;

    protected List<TextBox> shippingTBoxes;
    protected List<TextBox> itemTBoxes;
    protected Dictionary<Control, Label> labelTextboxPairs;
    public    Dictionary<Control, string> controlBoxAttrib;
    protected List<Label> clearableAttribLables;

    protected List<Control> allClearableControl;

    protected Button updateButton;
    protected Button editButton;

    // Extra from PurchasedLotsTab
    

    protected Form1 Form1;
    protected DatabaseConnector PyConnector;
    protected QueryBuilder QB;

    public Tab(Form1 Form1)
    {
        this.Form1 = Form1;
        QB = new QueryBuilder();
        PyConnector = new DatabaseConnector();
        inEditingState = false;

        allClearableControl = new List<Control>()
        {
            Form1.label17,
            Form1.label18,
            Form1.label19,
            Form1.label20,
            Form1.label21,
            Form1.label22,
            Form1.label23,
            Form1.label24,
            Form1.label25,
            Form1.label26,
            Form1.label40,
            Form1.label43,
            Form1.label15,
            Form1.label41,
            Form1.label44,
            Form1.label48,
            Form1.label51,
            Form1.label54,
            Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5,
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10,
            Form1.textBox14,
            Form1.textBox15,
            Form1.textBox16,
            Form1.textBox17,
            Form1.textBox18,
            Form1.textBox19,
            Form1.textBox2,
            Form1.textBox11,
            Form1.textBox20,
            Form1.textBox21,
            Form1.textBox22,
            Form1.textBox13,
            Form1.dateTimePicker3,
            Form1.dateTimePicker4,
            Form1.dateTimePicker5,
            Form1.listBox2,
            Form1.listBox3
        };
    }

    public ResultItem getCurrItem() => Form1.currItem;


    protected List<Control> getChangedFields()
    {
        
        List<Control> returnList = new List<Control>();
        foreach (Control control in editingAttributeControls)
        {
            string itemValue = "";
            string userValue = "";

            bool hasMatch = false;

            switch (control)
            {
                case TextBox:
                    string attrib = controlBoxAttrib[control];
                    getCurrItem().getAttribAsStr(attrib, ref itemValue);
                    userValue = control.Text;
                    hasMatch = true;
                    break;
                case DateTimePicker:
                    itemValue = getCurrItem().get_Date_Purchased_str();
                    userValue = new Date(control).toDateString();
                    hasMatch = true;
                    break;
            
            }

            // Must check if hasMatch, so that matching default vals when there is no match does not incorrectly add the control to the return list
            if (hasMatch)
            {
                if (userValue.CompareTo(itemValue) != 0)
                {
                    returnList.Add(control);
                }
            }
        }

        return returnList;

    }

    // Check if a value is a defualt value
    protected string checkDefault(int    val)
    {
        if (val == ResultItem.DEFAULT_INT) { return ""; }
        else { return val.ToString(); }
    }
    // Check if a value is a defualt value
    protected string checkDefault(double val)
    {
        if (val == -1) { return ""; }
        else { return val.ToString(); }
    }
    // Redundant, but exists for sake of extensibility
    protected string checkDefault(string val)
    {
        if (val is null) return "";
        if (val.CompareTo("") == 0) return "";
        else { return val.ToString(); }
    }


    // Opertaions are done to groups of control objects.
    // Thus function defines the group of contorl ojbects that will change state when editing/viewing
    // These groupings also have other functions in terms of representing
    // all parts table entry relavant to that tab the controls reside in
    protected abstract void generateTBoxGroups();


    public void viewMode()
    {
        if (inEditingState)
        {
            flipEditMode();
        }
    }

    public void editMode()
    {
        if (!inEditingState)
        {
            flipEditMode();
        }
    }


    abstract public void flipEditMode();

    public void showControlVisibility()
    {
        editButton.Text = inEditingState ? "View" : "Edit";
        updateButton.Visible = inEditingState;

        foreach (Label field in editableAttribLables)
        {
            field.Visible = !inEditingState;
        }


        foreach (Control field in editingControls)
        {
            field.Visible = inEditingState;

            if (inEditingState)
            {
                if (field is TextBox)
                {
                    field.Text = labelTextboxPairs[field as TextBox].Text;
                }
                if (field is DateTimePicker)
                {
                    DateTimePicker d = field as DateTimePicker;
                    Date date = new Date(labelTextboxPairs[field].Text);
                    d.Value = date.toDateTime();

                }
            }
        }
    }

    public void showItem(Sale sale)
    {
        throw new NotImplementedException();
    }

    public void showItem(ResultItem item)
    {

        // Item View Tab
        Util.clearLabelText(clearableAttribLables);

        Form1.currItem = item;

        if (item.hasItemEntry())
        {
            Form1.label40.Text = checkDefault(item.get_Name());
            Form1.label19.Text = checkDefault(item.get_InitialQuantity());
            Form1.label20.Text = checkDefault(item.get_CurrentQuantity());
            Form1.label21.Text = checkDefault(item.get_ITEM_ID());
            Form1.label51.Text = checkDefault(item.get_Name());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.label43.Text = datePurc.toDateString();
            Form1.label17.Text = Form1.saleT.getTotalSales().ToString();
        }

        if (item.hasSaleEntry())
        {
            Form1.label18.Text = checkDefault(item.get_Amount_sale());
        }

        if (item.hasShippingEntry())
        {
            List<int> WeightLbsOz = Util.ozToOzLbs(item.get_Weight());
            Form1.label22.Text = checkDefault(WeightLbsOz[0]);
            Form1.label23.Text = checkDefault(WeightLbsOz[1]);
            Form1.label24.Text = checkDefault(item.get_Length());
            Form1.label25.Text = checkDefault(item.get_Width());
            Form1.label26.Text = checkDefault(item.get_Height());
        }

        // PurchasedLot Tab

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.dateTimePicker4.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.label15.Text = checkDefault(item.get_Amount_purchase());
            Form1.label41.Text = checkDefault(item.get_Notes_purchase());
            Form1.label44.Text = item.get_Date_Purchased().toDateString();
        }
    }

    // Set curr item to null and clear all shown info about currItem
    public void clearCurrItem()
    {
        Form1.currItem = null;

        foreach (Control c in allClearableControl)
        {
            if (c is Label || c is TextBox) { c.Text = ""; }
            if (c is DateTimePicker)
            {
                DateTimePicker d = c as DateTimePicker;
                d.Value = new DateTime(2000, 1, 1);
            }
            if (c is ListBox)
            {
                ListBox b = c as ListBox;
                b.Items.Clear();
            }
        }

        // TODO: Make tabContorller object to switch all tabs to view mode

    }

    protected bool tableEntryExists(TextBox t)
    {
        if (controlBoxAttrib.ContainsKey(t))
        {
            string ret = "";
            // Check if the attribute associated with the textbox is a default value in the curr item
            Form1.currItem.getAttribAsStr(controlBoxAttrib[t], ref ret);
            if (ret.CompareTo(ResultItem.DEFAULT_INT.ToString()) == 0 ||
                ret.CompareTo(ResultItem.DEFAULT_DOUBLE.ToString()) == 0 ||
                ret is null ||
                ret.CompareTo(ResultItem.DEFAULT_DATE.ToString()) == 0)
            {
                return false;
            }
        }
        return true;
    }
}
