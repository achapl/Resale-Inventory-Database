using FinancialDatabase;
using FinancialDatabase.DatabaseObjects;
using FinancialDatabase.Tabs;
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
        Util.clearLabelText(allAttributeValueLabels);
        showControlVisibility();
    }

    public override void showItemAttributes(Item item)
    {
        // TODO: DELETE THIS and change it so that this method is not an inherited method. Not all tabs show item attributes.

        // Or account for the equivilant with ShowAttributes(). IE: sale tab shows sale attributes, item tab shows item attributes, purc tab shows purc attributes
        // Note this method is currently listed as showPurchaseAttributes()
        throw new NotImplementedException();
    }

    protected override void generateTBoxGroups()
    {
        mutableAttribValueControls = new List<ControlLabelPair>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.PurcDatePickerDLP
        };

        hideableAttribValueControls = new List<Control>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.PurcDatePickerDLP
        };

        allAttributeValueLabels = new List<Control>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.PurcDatePickerDLP
        };

        newItemTBoxes = new List<TextBox>()
        {
            Form1.PurcNameTextbox,
            Form1.PurcInitQtyTextbox,
            Form1.PurcCurrQtyTextbox,

        };

        purcNewItemShippingTBoxes = new List<TextBox>()
        {
            Form1.PurcLengthTextbox,
            Form1.PurcWidthTextbox,
            Form1.PurcHeightTextbox,
            Form1.PurcWeightLbsTextbox,
            Form1.PurcWeightOzTextbox

        };

        newPurchaseGroupControls = new List<Control>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcDatePickerDLP
        };

        foreach (Control c in newItemTBoxes)
        {
            newPurchaseGroupControls.Add(c);
        }

        Form1.PurcDatePickerDLP.attrib =  "purchase.Date_Purchased";
        Form1.PurcPurcPriceTLP.attrib = "purchase.Amount_purchase";
        Form1.PurcPurcNotesTLP.attrib = "purchase.Notes_purchase";
    }


    public Purchase getCurrPurc() => currPurc;

    public void setCurrPurc(Purchase purc) { currPurc = purc; }

    public List<Item> getCurrPurcItems()
    {
        if (currPurc is null)
        {
            return new List<Item>();
        }
        
        if (currPurc.items is null)
        {
            throw new Exception("Error: Curr purc has no initialized items list. Not 0 items, just no initialized list");
        }

        return currPurc.items;
    }
    private void addCurrPurcItem(Item item)
    {
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        currPurc.add(item);
    }

    public void addCurrPurchaseItems(List<Item> newPurchaseItems)
    {
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        if (currPurc.items is null)
        {
            currPurc.items = currPurc.items = new List<Item>();
        }
        currPurc.items.AddRange(newPurchaseItems);
    }

    public void clearCurrPurcItems()
    {
        setCurrPurc(null);
        Util.clearControls(allAttributeValueLabels);

        /*if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        if (currPurc.items is null)
        {
            currPurc.items = new List<Item>();
        }
        currPurc.items.Clear();*/
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!isNewPurchase && !inEditingState && tabController.getCurrItem() == null) { return; }
        inEditingState = !inEditingState;
        recordAttributeStates();
        showControlVisibility();

    }


    public void showPurchaseAttributes(Purchase purchase)
    {
        Util.clearLabelText(allAttributeValueLabels);
        Purchase currPurc = tabController.getCurrPurc();
        Date datePurc = currPurc.Date_Purchased;
        Form1.PurcDatePickerDLP.setLabelText(Util.checkDefault(currPurc.Date_Purchased.toDateString()));
        Form1.PurcDatePickerDLP.setControlVal(Util.checkDefault(currPurc.Date_Purchased.toDateString()));
        Form1.PurcPurcPriceTLP.setLabelText(Util.checkDefault(currPurc.Amount_purchase));
        Form1.PurcPurcPriceTLP.setControlVal(Util.checkDefault(currPurc.Amount_purchase));
        Form1.PurcPurcNotesTLP.setLabelText(Util.checkDefault(currPurc.Notes_purchase));
        Form1.PurcPurcNotesTLP.setControlVal(Util.checkDefault(currPurc.Notes_purchase));
        updateUserInputDefaultText();
    }

    public bool updateFromUserInput()
    {
        bool success = getUserInputUpdate();

        if (success)
        {
            setCurrPurcAndShowView(currPurc.PURCHASE_ID);
            showPurchaseAttributes(tabController.getCurrPurc());
            tabController.itemViewShowUpdate();
            tabController.saleTabShowUpdate();
            viewMode();
        }
        return success;
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

        List<ControlLabelPair> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields)) { return false; }

        foreach (ControlLabelPair CLP in changedFields)
        {
            if (CLP is null) { throw new Exception("ERROR: Control Object CLP is null, ItemViewTab.cs"); }

            string query = "";
            
            if (databaseEntryExists(CLP))
            {
                if (CLP is TextBoxLabelPair)
                {
                    Database.updateRow(tabController.getCurrItem(), (CLP as TextBoxLabelPair).attrib, (CLP as TextBoxLabelPair).getControlValueAsStr());
                }
                // TODO:
                else if ( CLP is DateTimePickerLabelPair)
                {
                    Database.updateRow(tabController.getCurrItem(), (CLP as DateTimePickerLabelPair).attrib, new Date(CLP));
                }
            }
            else if (!databaseEntryExists(CLP))
            {
                throw new Exception("ERROR: no purchase entry for CurrItem, This should not be possible");
            }

        }

        
        return true;
    }


    public void setCurrPurcAndShowView(int purchaseID)
    {
        if (purchaseID == null || purchaseID <= 0) { throw new Exception("Error: trying to set curr purchase and show its items from a bad purchaseID!"); }

        this.isNewPurchase = false;

        viewMode();

        // Update purchase from database
        currPurc = Database.getPurchase(purchaseID);

        showPurchaseAttributes(currPurc);

        Form1.PurchaseListBox.Items.Clear();

        foreach (Item i in currPurc.items)
        {
            Form1.PurchaseListBox.Items.Add(i.get_Name());
        }
    }


    public bool allNewPurchaseBoxesFilled()
    {
        // Necessary user input
        if (Form1.PurcNameTextbox.Text == "" ||
            !Double.TryParse(Form1.PurcPurcPriceTLP.getControlValueAsStr(), out double _))
        {
            showWarning("Error: Must have correct formatting for Purchase Date and Purchase Price");
            return false;
        }

        bool hasShippingInfo = false;
        bool hasBlankTextbox = false;
        foreach (TextBox t in purcNewItemShippingTBoxes)
        {
            if (t.Text != "")
            {
                hasShippingInfo = true;
                if (!Int32.TryParse(t.Text, out int _))
                {
                    showWarning("Error: Must have all shipping boxes filled out with correct integers, either none filled out at all");
                    return false;
                }
            }
            if (t.Text == "")
            {
                hasBlankTextbox = true;
            }            
        }

        if (hasShippingInfo &&
            hasBlankTextbox)
        {
            showWarning("Error: Must have all shipping boxes filled out with correct integers, either none filled out at all");
            return false;
        }

        // Set defaults
        if (Form1.PurcInitQtyTextbox.Text == "")
        {
            Form1.PurcInitQtyTextbox.Text = "1";
        }
        if (Form1.PurcCurrQtyTextbox.Text == "")
        {
            Form1.PurcCurrQtyTextbox.Text = "1";
        }
        return true;
    }


    public bool allNewShippingBoxesFilled()
    {
        foreach (TextBox t in purcNewItemShippingTBoxes)
        {
            if (t.Text.CompareTo("") == 0)
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
        int purcID;
        Date purcDate;
        bool userInputGood = allNewPurchaseBoxesFilled();
        if (!userInputGood) { return; }
        if ((currPurc is null || isNewPurchase)
             && userInputGood)
        {
            double amount = Double.Parse(Form1.PurcPurcPriceTLP.getControlValueAsStr());
            purcDate = new(Form1.PurcDatePickerDLP.getControlValueAsStr());
            string notes = Form1.PurcPurcNotesTLP.getControlValueAsStr();
            purcID = Database.insertPurchase(amount, notes, purcDate);
            setCurrPurc(Database.getPurchase(purcID));

        }
        purcID = currPurc.PURCHASE_ID;
        purcDate = tabController.getCurrPurc().Date_Purchased;

        // Make the new Item given by the user to go into the current purchase
        int itemID = -1;

        if (!Item.isValidName(Form1.PurcNameTextbox.Text))
        {
            showWarning("To Add New Item, a valid name for the item must be filled out");
            return;
        }

        Item newItem = getNewItemFromUserInput();
        newItem.set_PurchaseID(purcID);
        Database.insertItem(newItem, out itemID);
        newItem.set_ITEM_ID(itemID);

        // Cleanup
        Util.clearTBox(newItemTBoxes);
        Util.clearTBox(purcNewItemShippingTBoxes);
        // Currently, this method gets the current item and uses it to update the curr purchase so that the idsplay can be updated with the new purchase associated with it
        // Should instead update the curr purchase and use that to update the purchase tab with the new item.
        setCurrPurcAndShowView(tabController.getCurrPurc().PURCHASE_ID);
        isNewPurchase = false;
    }

    private Item getNewItemFromUserInput()
    {
        Item newItem = new Item();

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
        Util.clearLabelText(allClearableControl);
        clearCurrPurcItems();
        isNewPurchase = true;
        editMode();
    }

    public Item getCurrPurcItemsAt(int index)
    {
        if (currPurc is not null)
        {
            return currPurc.items[index];
        }
        else { return null; }
    }
}
