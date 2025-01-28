using FinancialDatabase;
using System;
using System.Windows.Forms;
using Date = Util.Date;

public abstract class Tab
{

    
    public bool inEditingState;
    protected List<ControlLabelPair> mutableAttribValueControls;
    protected List<Control> hideableAttribValueControls;

    protected List<TextBoxLabelPair> shippingTBoxes;
    protected List<TextBox> purcNewItemShippingTBoxes;
    protected List<TextBoxLabelPair> itemTBoxes;
    protected List<TextBox> newItemTBoxes; // NOTE: Keep as TextBox, these are for new items and don't need TLP's
    protected List<Control> allAttributeValueLabels;


    protected List<Control> controlBackup;

    protected List<Control> allClearableControl;

    protected Button updateButton;
    protected Button editButton;

    protected TabController tabController;

    protected Form1 Form1;

    private static bool TESTING = false;

    public Tab(Form1 Form1)
    {
        this.Form1 = Form1;
        inEditingState = false;

        allClearableControl = new List<Control>()
        {
            Form1.itemPurcPriceLbl,
            Form1.itemSoldPriceLbl,
            Form1.itemItemNoLbl,
            Form1.itemDatePurcLbl,
            Form1.PurcPurcDateLbl,
            Form1.SaleNameLbl,
            Form1.SaleDateLbl,
            Form1.itemNameTLP,
            Form1.itemInitQtyTLP,
            Form1.itemCurrQtyTLP,
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP,
            Form1.itemLengthTLP,
            Form1.itemWidthTLP,
            Form1.itemHeightTLP,
            Form1.PurcInitQtyTextbox,
            Form1.PurcLengthTextbox,
            Form1.PurcWidthTextbox,
            Form1.PurcHeightTextbox,
            Form1.PurcWeightLbsTextbox,
            Form1.PurcWeightOzTextbox,
            Form1.PurcNameTextbox,
            Form1.PurcCurrQtyTextbox,
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.SaleAmountTLP,
            Form1.SaleNewSaleAmountTextbox,
            Form1.SaleNewSaleDatePicker,
            Form1.PurcDatePicker,
            Form1.SaleDatePicker,
            Form1.PurchaseListBox,
            Form1.saleListBox
        };
    }
    


    // Returns only the textboxes and such that the user has changed while editing
    protected List<ControlLabelPair> getChangedFields()
    {
        List<ControlLabelPair> changedFields = new List<ControlLabelPair>();

        foreach (ControlLabelPair CLP in mutableAttribValueControls)
        {
            if (CLP.userChangedValue())
            {
                changedFields.Add(CLP);
            }
        }
        return changedFields;
    }


    internal bool typeCheckUserInput(List<TextBoxLabelPair> userInputFields)
    {
        List<ControlLabelPair> userInputControls = new List<ControlLabelPair>();
        foreach (TextBoxLabelPair userInputField in userInputFields)
        {
            userInputControls.Add(userInputField as ControlLabelPair);
        }
        return typeCheckUserInput(userInputControls);
    }

    internal bool typeCheckUserInput(List<ControlLabelPair> userInputFields)
    {
        foreach (ControlLabelPair c in userInputFields)
        {

            // Special case, weight textboxes
            if (!Int32.TryParse(Form1.itemWeightLbsTLP.getControlValue(), out _)
                || !Int32.TryParse(Form1.itemWeightOzTLP.getControlValue(), out _))
            {
                showWarning("Must Input Correct Numerical Format For weight. No decimals/commas allowed!");
                return false;
            }

            string attrib = c.attrib;
            string type = tabController.colDataTypes[attrib];

            if (c is TextBoxLabelPair && !Util.checkTypeOkay(c.getControlValueAsStr(), type))
            {
                return false;
            }
        }
        return true;
    }


    // Check if a value is a defualt value
    protected string checkDefault(int val)
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
            recordAttributeStates();
            flipEditMode();
        }
    }


    // Get a record before editing values of the mutable attributes
    // This way you can see which ones have been changed
    protected void recordAttributeStates()
    {
        if (controlBackup == null)
        {
            controlBackup = new List<Control>();
        }
        controlBackup.Clear();
        foreach (Control c in mutableAttribValueControls)
        {
            switch (c)
            {
                case TextBox:
                    TextBox tBoxRecord = new TextBox();
                    tBoxRecord.Text = c.Text;
                    controlBackup.Add(tBoxRecord);
                    break;
                case DateTimePicker:
                    DateTimePicker dt = c as DateTimePicker;
                    DateTimePicker dtPickerRecord = new DateTimePicker();
                    dtPickerRecord.Value = dt.Value;
                    controlBackup.Add(dtPickerRecord);
                    break;
            }
        }
    }

    abstract public void flipEditMode();

    public void showControlVisibility()
    {
        editButton.Text = inEditingState ? "View" : "Edit";
        updateButton.Visible = inEditingState;

        foreach (Control field in hideableAttribValueControls)
        {
            if (field is ControlLabelPair)
            {
                (field as ControlLabelPair).setEditMode(inEditingState);
            }
            else
            {
                field.Visible = inEditingState;
            }
        }
    }

    public void updateUserInputDefaultText()
    {
        foreach (Control field in mutableAttribValueControls)
        {
            if (field is TextBoxLabelPair)
            {
                (field as TextBoxLabelPair).updateControlValWithLabelText();
            }
            if (field is DateTimePicker)
            {
                DateTimePicker d = field as DateTimePicker;
                if (labelTextboxPairs[field].Text != "")
                {
                    Date date = new Date(labelTextboxPairs[field].Text);
                    d.Value = date.toDateTime();
                }
                else
                {
                    d.Value = System.DateTime.Now;
                }
            }
        }
    }


    abstract public void showItemAttributesAndPics(Item item);

    // Clear all shown info about currItem
    public void clearCurrItemControls()
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

    // Check if a user-inputted control attribute corresponds to data
    // for which there is an entry in the database
    // Don't want to be updating data that doesn't exist
    protected bool databaseEntryExists(ControlLabelPair c)
    {
        if (!allClearableControl.Contains(c))
        {
            return false;
        }

        // Check if the attribute associated with the textbox is a default value in the curr item
        string ret = tabController.getCurrItem().getAttribAsStr(c.attrib);
        if (ret.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_DATE.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_INT.ToString()) == 0 ||
            ret is null)
        {
            return false;
        }
        return true;
    }

    public void showWarning(string message)
    {
        if (!TESTING)
        {
            MessageBox.Show(
                    message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
        }
    }

    public bool showWarningYESNO(string message)
    {
        if (TESTING) { return true; }

        DialogResult result = MessageBox.Show(
                            message,
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                            );
        return result == DialogResult.Yes;
    }
}
