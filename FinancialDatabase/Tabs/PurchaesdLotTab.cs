using FinancialDatabase;
using Date = Util.Date;

public class PurchasedLotTab : Tab
{

    public bool isNewPurchase;

    private List<Control> newPurchaseGroupControls;

    public PurchasedLotTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
	{
        this.tabController = tabController;
        isNewPurchase = false;
        updateButton = Form1.UpdatePurcButton;
        editButton   = Form1.EditPurcButton;
        generateTBoxGroups();
        Util.clearLabelText(attributeValueLabels);
        showControlVisibility();
    }


    protected override void generateTBoxGroups()
    {
        mutableAttribValueControls = new List<Control>()
        {
            Form1.PurcPurcPriceTextbox,
            Form1.PurcPurcNotesTextbox,
            Form1.PurcDatePicker
        };

        hideableAttribValueControls = new List<Control>()
        {
            Form1.PurcPurcPriceTextbox,
            Form1.PurcPurcNotesTextbox,
            Form1.PurcDatePicker
        };

        mutableAttribValueLabels = new List<Label>()
        {
            Form1.PurcPurcPriceLbl,
            Form1.PurcPurcNotesLbl,
            Form1.PurcPurcDateLbl
        };

        attributeValueLabels = new List<Label>()
        {
            Form1.PurcPurcPriceLbl,
            Form1.PurcPurcNotesLbl,
            Form1.PurcPurcDateLbl
        };

        newItemTBoxes = new List<TextBox>()
        {
            Form1.PurcNameTextbox,
            Form1.PurcInitQtyTextbox,
            Form1.PurcCurrQtyTextbox,

        };

        shippingTBoxes = new List<TextBox>()
        {
            Form1.PurcLengthTextbox,
            Form1.PurcWidthTextbox,
            Form1.PurcHeightTextbox,
            Form1.PurcWeightLbsTextbox,
            Form1.PurcWeightOzTextbox

        };

        newPurchaseGroupControls = new List<Control>()
        {
            Form1.PurcPurcPriceTextbox,
            Form1.PurcDatePicker
        };

        foreach (Control c in newItemTBoxes)
        {
            newPurchaseGroupControls.Add(c);
        }

        labelTextboxPairs = new Dictionary<Control, Label>();

        int i = 0;
        foreach (Control c in mutableAttribValueControls)
        {
            if (c is not Button)
            {
                labelTextboxPairs[c] = mutableAttribValueLabels[i++];
            }
        }

        controlAttrib = new Dictionary<Control, string>
        {
            { Form1.PurcDatePicker,  "purchase.Date_Purchased" },
            { Form1.PurcPurcPriceTextbox,        "purchase.Amount_purchase" },
            { Form1.PurcPurcNotesTextbox,        "purchase.Notes_purchase" }
        };
    }


    // Todo: Delete?
    public void update(ResultItem item)
	{
        Form1.PurchaseListBox.Items.Clear();
		tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.getItems(QueryBuilder.purchaseQuery(item), false);

		foreach(ResultItem i in result)
		{
			Form1.PurchaseListBox.Items.Add(i.get_Name());
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.PurcPurcPriceLbl.Text = item.get_Amount_purchase().ToString();
        Form1.PurcPurcNotesLbl.Text = item.get_Notes_purchase();

	}


    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!isNewPurchase && !inEditingState && tabController.getCurrItem() == null) { return; }

        inEditingState = !inEditingState;
        showControlVisibility();

    }


    public override void showItemAttributes(ResultItem item)
    {

        Util.clearLabelText(attributeValueLabels);

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.PurcDatePicker.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.PurcPurcPriceLbl.Text = checkDefault(item.get_Amount_purchase());
            Form1.PurcPurcNotesLbl.Text = checkDefault(item.get_Notes_purchase());
            Form1.PurcPurcDateLbl.Text = item.get_Date_Purchased().toDateString();
        }
        updateUserInputDefaultText();
        
    }


    // Update purchase button is clicked
    public bool editUpdate()
    {
        // Check if adding a new purchase
        // or editing existing one
        if (isNewPurchase)
        {
            addItem();
            return true;
        }
        // Null check
        else if (tabController.getCurrItem() is null)
        {
            return false;
        }

        List<Control> changedFields = getChangedFields();

        foreach (Control c in changedFields)
        {
            if (c is null) { throw new Exception("ERROR: Control Object c is null, ItemViewTab.cs"); }

            string query = "";
            
            if (tableEntryExists(c))
            {
                string type = tabController.colDataTypes[controlAttrib[c]];
                if (c is TextBox)
                {
                    TextBox userInputTBox = c as TextBox;
                    string attrib = userInputTBox.Text;
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        return false;
                    }
                    DatabaseConnector.updateRow(tabController.getCurrItem(), controlAttrib[c], type, userInputTBox.Text);
                }
                else if (c is DateTimePicker)
                {
                    DatabaseConnector.updateRow(tabController.getCurrItem(), controlAttrib[c], type, new Date(c));
                }
            }
            else if (!tableEntryExists(c))
            {
                Console.WriteLine("ERROR: no purchase entry for CurrItem, This should not be possible");
                continue;
            }

        }

        updatePurchasedLotView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
        showItemAttributes(tabController.getCurrItem());
        return true;
    }


    // TODO: Delete or make part of TabController?
    public void updatePurchasedLotView(ResultItem item)
    {
        if (item == null) { return; }
        showItemAttributes(item);

        Form1.PurchaseListBox.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.getItems(QueryBuilder.purchaseQuery(item), false);

        foreach (ResultItem i in result)
        {
            Form1.PurchaseListBox.Items.Add(i.get_Name());
            tabController.addCurrentPurchaseItems(DatabaseConnector.getItem(i.get_ITEM_ID()));
        }

        Form1.PurcPurcPriceLbl.Text = item.get_Amount_purchase().ToString();
        Form1.PurcPurcNotesLbl.Text = item.get_Notes_purchase();
    }

    public bool allNewPurchaseBoxesFilled()
    {
        foreach (Control c in newPurchaseGroupControls)
        {
            if (c is TextBox)
            {
                TextBox t = c as TextBox;
                if (t.Text.CompareTo("") == 0)
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    public bool allNewShippingBoxesFilled()
    {
        foreach (Control c in shippingTBoxes)
        {
            if (c.Text.CompareTo("") == 0)
            {
                return false;
            }
        }
        return true;
    }


    public void addItem()
    {
        int purcID = -1;
        Date purcDate = new Date();
        if (tabController.getCurrItem() is not null)
        {
            purcDate = tabController.getCurrItem().get_Date_Purchased();
        }
        
        if (isNewPurchase && allNewPurchaseBoxesFilled())
        { 
            DateTime dt = Form1.PurcDatePicker.Value;
            purcDate = new (dt.Year, dt.Month, dt.Day);
            purcID = DatabaseConnector.newPurchase(Int32.Parse(Form1.PurcPurcPriceTextbox.Text), Form1.PurcPurcNotesTextbox.Text, purcDate);

        }
        // Incorrectly formed new purchase from user input, don't continue on
        else if (isNewPurchase && !allNewPurchaseBoxesFilled())
        {
            MessageBox.Show(
                            "To Add New Purchase, a Purchase Price, Purchase Date, and NEW ITEM Name, Initial Quantity, and Current Quantity must each be filled out",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                            );
            return;
        }
        ResultItem newItem = getNewItemFromUserInput(purcID);
        // Make the new initial item given by the user to go along with the purchase
        

        Util.clearTBox(newItemTBoxes);
        Util.clearTBox(shippingTBoxes);
        int itemID = -1;
        DatabaseConnector.insertItem(newItem, out itemID);
        newItem.set_ITEM_ID(itemID);
        string attrib = "item.PurchaseID";
        string type = tabController.colDataTypes[attrib];
        string query = QueryBuilder.updateQuery(newItem, attrib, type, purcID.ToString());
        DatabaseConnector.runStatement(query);
        //TODO: Can the following line be removed since tabController.setCurrItem(newItem) is called and should update the currItem with modified purc date?
        newItem.set_PurchaseID(purcID);
        tabController.setCurrItem(newItem);
        updatePurchasedLotView(tabController.getCurrItem());
        isNewPurchase = false;
    }

    private ResultItem getNewItemFromUserInput(int purcID)
    {
        ResultItem newItem = new ResultItem();

        newItem.set_Name(Form1.PurcNameTextbox.Text);

        // Set date
        DateTime dt = Form1.PurcDatePicker.Value;
        Date purcDate = new(dt.Year, dt.Month, dt.Day);
        newItem.set_Date_Purchased(purcDate);

        // If no Init or curr quantities, set to default 1
        string initQty = "1";
        if (Form1.PurcInitQtyTextbox.Text.CompareTo("") != 0)
        {
            initQty = Form1.PurcInitQtyTextbox.Text;
        }
        newItem.set_InitialQuantity(initQty);

        string currQty = "1";
        if (Form1.PurcCurrQtyTextbox.Text.CompareTo("") != 0)
        {
            currQty = Form1.PurcCurrQtyTextbox.Text;
        }
        newItem.set_CurrentQuantity(currQty);
        
        // Add user-inputted shipping info
        if (allNewShippingBoxesFilled())
        {
            int ttlWeight = Int32.Parse(Form1.PurcWeightLbsTextbox.Text) * 16 + Int32.Parse(Form1.PurcWeightOzTextbox.Text);
            newItem.set_Weight(ttlWeight);
            newItem.set_Length(Int32.Parse(Form1.PurcLengthTextbox.Text));
            newItem.set_Width(Int32.Parse(Form1.PurcWidthTextbox.Text));
            newItem.set_Height(Int32.Parse(Form1.PurcHeightTextbox.Text));
        }

        newItem.set_PurchaseID(purcID);

        return newItem;
    }

    // Clear out old information
    // If there is a purchased item able to be added,
    // make a new purchase in the database and add it
    public void newPurchase()
    {
        Form1.PurchaseListBox.Items.Clear();
        isNewPurchase = true;
        editMode();
        //addItem();

    }
}
