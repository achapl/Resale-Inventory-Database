using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using Date = Util.Date;

public class PurchasedLotTab : Tab
{

    private List<Control> newPurchaseGroupControls;
    private Purchase currPurc;
    public bool isNewPurchase;

    public PurchasedLotTab(TabController tabController, Form1 Form1) : base(Form1)
	{
        currPurc = null;
        updateButton = Form1.UpdatePurcButton;
        editButton   = Form1.EditPurcButton;
        this.tabController = tabController;
        isNewPurchase = false;
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


    public Purchase getCurrPurc() => currPurc;

    public void setCurrPurc(Purchase purc) { currPurc = purc; }

    public List<ResultItem> getCurrPurcItems() => currPurc.items;
    public void addCurrPurcItem(ResultItem item)
    {
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        currPurc.add(item);
    }

    public void addCurrPurchaseItems(List<ResultItem> newPurchaseItems)
    {
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        if (currPurc.items is null)
        {
            currPurc.items = currPurc.items = new List<ResultItem>();
        }
        currPurc.items.AddRange(newPurchaseItems);
    }

    public void clearCurrPurcItems()
    {
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        if (currPurc.items is null)
        {
            currPurc.items = new List<ResultItem>();
        }
        currPurc.items.Clear();
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
            Purchase currPurc = tabController.getCurrPurc();
            Date datePurc = currPurc.date;
            Form1.PurcDatePicker.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.PurcPurcPriceLbl.Text = checkDefault(currPurc.amount);
            Form1.PurcPurcNotesLbl.Text = checkDefault(currPurc.notes_purchase);
            Form1.PurcPurcDateLbl.Text = checkDefault(currPurc.date.toDateString());
        }
        updateUserInputDefaultText();
        
    }

    public void updateFromUserInput()
    {
        bool success = getUserInputUpdate();

        if (success)
        {
            viewMode();
        }
    }

    // Update purchase button is clicked
    public bool getUserInputUpdate()
    {
        // Check if adding a new purchase
        // or editing existing one
        if (isNewPurchase)
        {
            addItemToPurc();
            return true;
        }
        // Null check: Every currPurc must have at least one item.
        // There if not a new purc, there must therefore be a currItem
        else if (tabController.getCurrItem() is null)
        {
            return false;
        }

        List<Control> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields)) { return false; }

        foreach (Control c in changedFields)
        {
            if (c is null) { throw new Exception("ERROR: Control Object c is null, ItemViewTab.cs"); }

            string query = "";
            
            if (databaseEntryExists(c))
            {
                if (c is TextBox)
                {
                    Database.updateRow(tabController.getCurrItem(), controlAttrib[c], (c as TextBox).Text);
                }
                else if (c is DateTimePicker)
                {
                    Database.updateRow(tabController.getCurrItem(), controlAttrib[c], new Date(c));
                }
            }
            else if (!databaseEntryExists(c))
            {
                throw new Exception("ERROR: no purchase entry for CurrItem, This should not be possible");
            }

        }

        setCurrPurcAndShowItems(Database.getItem(tabController.getCurrItem().get_ITEM_ID()));
        showItemAttributes(tabController.getCurrItem());
        return true;
    }


    public void setCurrPurcAndShowItems(ResultItem item)
    {
        if (item == null) { return; }

        currPurc = Database.getPurchase(item);

        showItemAttributes(item);

        Form1.PurchaseListBox.Items.Clear();
        tabController.clearCurrPurcItems();
        List<ResultItem> result = Database.getPurchItems(item);

        foreach (ResultItem i in result)
        {
            Form1.PurchaseListBox.Items.Add(i.get_Name());
            addCurrPurcItem(Database.getItem(i.get_ITEM_ID()));
        }

        Form1.PurcPurcPriceLbl.Text = currPurc.amount.ToString();
        Form1.PurcPurcNotesLbl.Text = currPurc.notes_purchase;
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


    // Button was pushed to add an item to the current purchase
    // If there is no current purchase, create one from given user input
    public void addItemToPurc()
    {
        int purcID = currPurc.id;
        Date purcDate = new Date();

        if (tabController.getCurrPurc() is not null)
        {
            purcDate = tabController.getCurrPurc().date;
        }
        
        if (isNewPurchase && allNewPurchaseBoxesFilled())
        { 
            DateTime dt = Form1.PurcDatePicker.Value;
            purcDate = new (dt.Year, dt.Month, dt.Day);
            purcID = Database.newPurchase(Int32.Parse(Form1.PurcPurcPriceTextbox.Text), Form1.PurcPurcNotesTextbox.Text, purcDate);

        }
        // Incorrectly formed new purchase from user input, don't continue on
        else if (isNewPurchase && !allNewPurchaseBoxesFilled())
        {
            showWarning("To Add New Purchase, a Purchase Price, Purchase Date, and NEW ITEM Name, Initial Quantity, and Current Quantity must each be filled out");
            return;
        }

        // Make the new ResultItem given by the user to go into the current purchase
        int itemID = -1;

        if (!ResultItem.isValidName(Form1.itemNameTxtbox.Text)) return;

        ResultItem newItem = getNewItemFromUserInput();

        Database.insertItem(newItem, out itemID);
        newItem.set_PurchaseID(purcID);
        newItem.set_ITEM_ID(itemID);

        // Update database to reflect the current purchase's purchaseID
        Database.updateRow(newItem, "item.PurchaseID", purcID);

        // Cleanup
        Util.clearTBox(newItemTBoxes);
        Util.clearTBox(shippingTBoxes);
        setCurrPurcAndShowItems(tabController.getCurrItem());
        isNewPurchase = false;
    }

    private ResultItem getNewItemFromUserInput()
    {
        ResultItem newItem = new ResultItem();

        newItem.set_Name(Form1.PurcNameTextbox.Text);

        // Make to default 1
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
        //addSearchResultItem();

    }

    public ResultItem getCurrPurcItemsAt(int index)
    {
        if (currPurc is not null)
        {
            return currPurc.items[index];
        }
        else { return null; }
    }

    public void setCurrPurcItems(List<ResultItem> newPurcItems)
    {
        currPurc.items = newPurcItems;
    }
}
