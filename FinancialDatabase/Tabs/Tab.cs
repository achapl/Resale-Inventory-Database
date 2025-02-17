using FinancialDatabase;
using FinancialDatabase.Tabs;
using System;
using System.Linq;
using System.Windows.Forms;
using Date = Util.Date;

public abstract class Tab
{

    
    public bool inEditingState;
    protected List<ControlLabelPair> mutableAttribValueControls;
    protected List<Control> hideableAttribValueControls;

    protected List<TextBoxLabelPair> shippingTBoxes;
    protected List<TextBox> purcNewItemShippingTBoxes; // NOTE: Keep as TextBox, these are for new items and don't need TLP's
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
            Form1.SaleNameLbl,
            Form1.SaleDatePickerDLP,
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
            Form1.PurcDatePickerDLP,
            Form1.SaleDatePickerDLP,
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


    // Note: 2 versions of func are required since List<TextBoxLabelPair> and List<ControlLabelPair> don't inherit from each other
    internal bool typeCheckUserInput(List<TextBoxLabelPair> userInputFields)
    {
        List<ControlLabelPair> userInputControls = new List<ControlLabelPair>();
        foreach (TextBoxLabelPair userInputField in userInputFields)
        {
            userInputControls.Add(userInputField as ControlLabelPair);
        }
        return typeCheckUserInput(userInputControls);
    }


    // Note: 2 versions of func are required since List<TextBoxLabelPair> and List<ControlLabelPair> don't inherit from each other
    internal bool typeCheckUserInput(List<ControlLabelPair> userInputFields)
    {

        // Special case, weight textboxes
        if (userInputFields.Contains(Form1.itemWeightLbsTLP) ||
            userInputFields.Contains(Form1.itemWeightLbsTLP))
        {
            if (!Form1.itemWeightLbsTLP.hasIntText() &&
            Form1.itemWeightLbsTLP.Text != ""
            ||
            !Form1.itemWeightOzTLP.hasIntText() &&
            Form1.itemWeightOzTLP.Text != "")
            {
                showWarning("Must Input Correct Numerical Format For weight. No decimals/commas allowed!");
                return false;
            }
        }

        foreach (ControlLabelPair c in userInputFields)
        {
            string attrib = c.attrib;
            string type = tabController.colDataTypes[attrib];

            if (c is TextBoxLabelPair && !Util.checkTypeOkay(c.getControlValueAsStr(), type))
            {
                return false;
            }
        }
        return true;
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
            if (field is DateTimePickerLabelPair)
            {
                DateTimePickerLabelPair d = field as DateTimePickerLabelPair;
                if (d.getLabelText() != "")
                {
                    (d as DateTimePickerLabelPair).updateControlValWithLabelText();
                }
                else
                {
                    d.setControlVal(System.DateTime.Now);
                }
            }
        }
    }


    abstract public void showItemAttributes(Item item);

    

    // Check if a user-inputted control attribute corresponds to data
    // for which there is an entry in the database
    // Don't want to be updating data that doesn't exist
    protected bool databaseEntryExists(ControlLabelPair c)
    {
        if (!allClearableControl.Contains(c))
        {
            return false;
        }
        string ret = null;
        string table = c.getAttribTable();
        switch (table)
        {
            // Shipping is lumped in with item object since they are 1 to 1
            case "shipping":

            case "item":
                if (tabController.getCurrItem() == null)
                {
                    throw new Exception("Error: Trying to check if database entry exist for purchase table, when no curr purchase exists. This shouldn't happen");
                }
                ret = tabController.getCurrItem().getAttribAsStr(c.attrib);
                break;
            case "purchase":
                if (tabController.getCurrPurc() == null)
                {
                    throw new Exception("Error: Trying to check if database entry exist for purchase table, when no curr purchase exists. This shouldn't happen");
                }
                ret = tabController.getCurrPurc().getAttribAsString(c.attrib);
                break;

            case "sale":
                if (tabController.getCurrSale() != null)
                    {
                        ret = tabController.getCurrSale().getAttribAsStr(c.attrib);
                    }
                break;
        }

        // Check if the attribute associated with the textbox is a default value in the curr item
        
        if (ret is null ||
            ret.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_DATE.ToString()) == 0 ||
            ret.CompareTo(Util.DEFAULT_INT.ToString()) == 0)
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
