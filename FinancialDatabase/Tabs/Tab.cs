using FinancialDatabase;
using System;
using Date = Util.Date;

public abstract class Tab
{


    protected bool inEditingState;
    protected List<Control> editingControls;
    protected List<Control> editingTextBoxes;
    protected List<Control> nonEditingControls;
    protected List<Label> nonEditingLabels;

    protected List<TextBox> shippingTBoxes;
    protected List<TextBox> itemTBoxes;
    protected Dictionary<Control, Label> editableFieldPairs;
    public    Dictionary<Control, string> controlBoxAttrib;

    protected Button updateButton;
    protected Button editButton;

    // Extra from PurchasedLotsTab
    protected List<Label> allPurchaseLabels;

    // Extra form ItemViewTab
    protected List<Label> allItemLabels;
    protected List<TextBox> weightTBoxes;

    protected Form1 Form1;
    protected CtrlerOfPythonToDTBConnector PyConnector;
    protected QueryBuilder QB;

    public Tab(Form1 Form1)
    {
        this.Form1 = Form1;
        QB = new QueryBuilder();
        PyConnector = new CtrlerOfPythonToDTBConnector();
        inEditingState = false;

        generateTBoxGroups();


        Util.clearLabelText(allItemLabels);

    }

    public ResultItem getCurrItem() => Form1.currItem;


    protected List<Control> getChangedFields()
    {
        
        List<Control> returnList = new List<Control>();
        foreach (Control control in editingTextBoxes)
        {
            string ret = "";
            if (control is TextBox
                && control.Text.Length > 0)
            {
                Form1.currItem.getAttribAsString(controlBoxAttrib[control], ref ret);
                if (((TextBox)control).Text.CompareTo(ret) != 0)
                {
                    returnList.Add(control);
                }
            }
            else if (control is DateTimePicker) {
                string givenDate = new Date(control).toDateString();
                string currItemDate = Form1.currItem.get_Date_Purchased().toDateString();
                if (givenDate.CompareTo(currItemDate) != 0)
                {
                    returnList.Add(control);
                }
            }
        }

        return returnList;

    }


    // Check if a value is a defualt value
    protected string checkDefault(int val)
    {
        if (val == ResultItem.DEFAULT_INT) { return ""; }
        else { return val.ToString(); }
    }
    protected string checkDefault(double val)
    {
        if (val == -1) { return ""; }
        else { return val.ToString(); }
    }
    // Redundant, but exists for sake of extensibility
    protected string checkDefault(string val)
    {
        if (val.CompareTo("") == 0) { return ""; }
        else { return val.ToString(); }
    }


    protected abstract void generateTBoxGroups();


    public void flipEditState()
    {
        if (inEditingState)
        {
            editButton.Text = "View";
            updateButton.Visible = true;
        }
        else
        {
            editButton.Text = "Edit";
            updateButton.Visible = false;
        }

        foreach (Control c in nonEditingControls)
        {
            if (inEditingState)
            {
                c.Visible = false;
            }
            else
            {
                c.Visible = true;
            }
        }
        foreach (Control c in editingControls)
        {
            if (inEditingState)
            {
                c.Visible = true;
                if (c is TextBox)
                {
                    c.Text = editableFieldPairs[c as TextBox].Text;
                }
                if (c is DateTimePicker)
                {
                    DateTimePicker d = c as DateTimePicker;
                    Date date = new Date(editableFieldPairs[c].Text);
                    //d.Value.Year = editableFieldPairs[]
                }
            }
            else
            {
                c.Visible = false;
            }
        }
    }



    public void showItem(ResultItem item)
    {

        // Item View Tab
        Util.clearLabelText(allItemLabels);

        Form1.currItem = item;

        if (item.hasItemEntry())
        {
            Form1.label40.Text = checkDefault(item.get_Name());
            Form1.label19.Text = checkDefault(item.get_InitialQuantity());
            Form1.label20.Text = checkDefault(item.get_CurrentQuantity());
            Form1.label21.Text = checkDefault(item.get_ITEM_ID());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.label43.Text = datePurc.toDateString();
            Form1.label17.Text = checkDefault(item.get_Amount_purchase());
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

        Util.clearLabelText(allPurchaseLabels);

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.dateTimePicker4.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.label15.Text = checkDefault(item.get_Amount_purchase());
            Form1.label41.Text = checkDefault(item.get_Notes_purchase());
            Form1.label44.Text = item.get_Date_Purchased().toDateString();
        }
    }

    protected bool tableEntryExists(TextBox t)
    {
        if (controlBoxAttrib.ContainsKey(t))
        {
            string ret = "";
            // Check if the attribute associated with the textbox is a default value in the curr item
            Form1.currItem.getAttribAsString(controlBoxAttrib[t], ref ret);
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
