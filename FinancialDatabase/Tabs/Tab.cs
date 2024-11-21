using FinancialDatabase;
using System;
using Date = Util.Date;

public abstract class Tab
{


    public bool inEditingState;
    protected List<Control> editingAttributeControls;
    protected List<Label> editableAttribLables;
    protected List<Control> editingControls;
    private List<Control> saleControls;

    protected List<TextBox> shippingTBoxes;
    protected List<TextBox> itemTBoxes;
    protected Dictionary<Control, Label> labelTextboxPairs;
    public    Dictionary<Control, string> controlBoxAttrib;
    protected List<Label> clearableAttribLables;
    

    protected List<Control> allClearableControl;

    protected Button updateButton;
    protected Button editButton;

    protected Form1.TabController tabController;

    protected Form1 Form1;

    public Tab(Form1 Form1)
    {
        this.Form1 = Form1;
        inEditingState = false;

        allClearableControl = new List<Control>()
        {
            Form1.itemPurcPriceLbl,
            Form1.itemSoldPriceLbl,
            Form1.itemInitQtyLbl,
            Form1.itemCurrQtyLbl,
            Form1.itemItemNoLbl,
            Form1.itemWeightLbsLbl,
            Form1.itemWeightOzLbl,
            Form1.itemLengthLbl,
            Form1.itemWidthLbl,
            Form1.itemHeightLbl,
            Form1.itemNameLbl,
            Form1.itemDatePurcLbl,
            Form1.PurcPurcPriceLbl,
            Form1.PurcPurcNotesLbl,
            Form1.PurcPurcDateLbl,
            Form1.SaleAmountLbl,
            Form1.SaleNameLbl,
            Form1.SaleDateLbl,
            Form1.textBox3,
            Form1.itemInitQtyTxtbox,
            Form1.itemCurrQtyTxtbox,
            Form1.itemWeightLbsTxtbox,
            Form1.itemWeightOzTxtbox,
            Form1.itemLengthTxtbox,
            Form1.itemWidthTxtbox,
            Form1.itemHeightTxtbox,
            Form1.PurcInitQtyTextbox,
            Form1.PurcLengthTextbox,
            Form1.PurcWidthTextbox,
            Form1.PurcHeightTextbox,
            Form1.PurcWeightLbsTextbox,
            Form1.PurcWeightOzTextbox,
            Form1.PurcNameTextbox,
            Form1.PurcCurrQtyTextbox,
            Form1.PurcPurcPriceTextbox,
            Form1.PurcPurcNotesTextbox,
            Form1.SaleAmountTextbox,
            Form1.SaleNewSaleAmountTextbox,
            Form1.SaleNewSaleDatePicker,
            Form1.PurcDatePicker,
            Form1.SaleDatePicker,
            Form1.PurchaseListBox,
            Form1.saleListBox
        };
        saleControls = new List<Control> { Form1.SaleAmountTextbox };
    }

    public ResultItem getCurrItem() => tabController.getCurrItem();


    protected List<Control> getChangedFields()
    {
        
        List<Control> returnList = new List<Control>();
        foreach (Control control in editingAttributeControls)
        {
            string itemValue = "";
            string saleValue = "";
            string userValue = "";

            bool hasMatch = false;

            switch (control)
            {
                case TextBox:
                    string attrib = controlBoxAttrib[control];
                    
                    // Determine which table (sale/item) the Textbox belongs to to compare it with the propor attribute
                    if (saleControls.Contains(control as TextBox))
                    {
                        tabController.getCurrSale().getAttribAsStr(attrib, ref saleValue);
                    }
                    else
                    {
                        // If there is no item to compare it against, compare it to empty string, which is what textbox is by default when there is no currItem
                        if (getCurrItem() is null)
                        {
                            itemValue = "";
                        }
                        else
                        {
                            getCurrItem().getAttribAsStr(attrib, ref itemValue);
                        }
                    }
                    userValue = control.Text;
                    hasMatch = true;
                    break;
                case DateTimePicker:
                    // If there is no item to compare it against, compare it to "empty" date
                    if (getCurrItem() is null)
                    {
                        itemValue = new Date(0,0,0).toDateString();
                    }
                    else
                    {
                        itemValue = getCurrItem().get_Date_Purchased_str();
                    }
                    
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
        if (val == Util.DEFAULT_INT) { return ""; }
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
        }
    }

    public void updateUserInputDefaultText()
    {
        foreach (Control field in editingControls)
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


    abstract public void showItemAttributes(ResultItem item);

    // Set curr item to null and clear all shown info about currItem
    public void clearCurrItem()
    {
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
        viewMode();
    }

    protected bool tableEntryExists(TextBox t)
    {
        if (controlBoxAttrib.ContainsKey(t))
        {
            string ret = "";
            // Check if the attribute associated with the textbox is a default value in the curr item
            tabController.getCurrItem().getAttribAsStr(controlBoxAttrib[t], ref ret);
            if (ret.CompareTo(Util.DEFAULT_INT.ToString()) == 0 ||
                ret.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
                ret is null ||
                ret.CompareTo(Util.DEFAULT_DATE.ToString()) == 0)
            {
                return false;
            }
        }
        return true;
    }
}
