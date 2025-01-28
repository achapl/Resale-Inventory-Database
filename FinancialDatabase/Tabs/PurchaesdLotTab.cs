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
        Util.clearLabelText(allAttributeValueLabels);
        showControlVisibility();
    }

    public override void showItemAttributesAndPics(Item item)
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
            Form1.PurcDatePicker
        };

        hideableAttribValueControls = new List<Control>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.PurcDatePicker
        };

        allAttributeValueLabels = new List<Control>()
        {
            Form1.PurcPurcPriceTLP,
            Form1.PurcPurcNotesTLP,
            Form1.PurcPurcDateLbl
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
            Form1.PurcDatePicker
        };

        foreach (Control c in newItemTBoxes)
        {
            newPurchaseGroupControls.Add(c);
        }

        Form1.PurcPurcPriceTLP.attrib = "purchase.Amount_purchase";
        Form1.PurcPurcNotesTLP.attrib = "purchase.Notes_purchase";
        Form1.PurcDatePicker  "purchase.Date_Purchased";
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
        if (currPurc is null)
        {
            currPurc = new Purchase();
        }
        if (currPurc.items is null)
        {
            currPurc.items = new List<Item>();
        }
        currPurc.items.Clear();
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
        Form1.PurcDatePicker.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
        Form1.PurcPurcPriceTLP.setLabelText(checkDefault(currPurc.Amount_purchase));
        Form1.PurcPurcNotesTLP.setLabelText(checkDefault(currPurc.Notes_purchase));
        Form1.PurcPurcDateLbl.Text = checkDefault(currPurc.Date_Purchased.toDateString());
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
                else if (CLP is /*MyDateTimepicker*/)
                {
                    Database.updateRow(tabController.getCurrItem(), (CLP as /*MyDateTimepicker*/).attrib, new Date(CLP));
                }
            }
            else if (!databaseEntryExists(CLP))
            {
                throw new Exception("ERROR: no purchase entry for CurrItem, This should not be possible");
            }

        }

        setCurrPurcAndShowItems(currPurc.PURCHASE_ID);
        showPurchaseAttributes(tabController.getCurrPurc());
        return true;
    }


    public void setCurrPurcAndShowItems(int purchaseID)
    {
        if (purchaseID == null || purchaseID <= 0) { throw new Exception("Error: trying to set curr purchase and show its items from a bad purchaseID!"); }

        // Update purchase from database
        currPurc = Database.getPurchase(purchaseID);

        showPurchaseAttributes(currPurc);

        Form1.PurchaseListBox.Items.Clear();

        foreach (Item i in currPurc.items)
        {
            Form1.PurchaseListBox.Items.Add(i.get_Name());
           // addCurrPurcItem(Database.getItem(i.get_ITEM_ID()));
        }

        Form1.PurcPurcPriceTLP.setLabelText(currPurc.Amount_purchase.ToString());
        Form1.PurcPurcNotesTLP.setLabelText(currPurc.Notes_purchase);
    }


    public bool allNewPurchaseBoxesFilled()
    {
        foreach (Control c in newPurchaseGroupControls)
        {
            if (c is TextBox)
            {
                TextBox t = c as TextBox;
                // Set defaults
                if (t.Name.CompareTo("PurcInitQtyTextbox") == 0 ||
                    t.Name.CompareTo("PurcCurrQtyTextbox") == 0)
                {
                    t.Text = "1";
                }
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
        int purcID;
        Date purcDate;

        if ((currPurc is null || isNewPurchase)
             && allNewPurchaseBoxesFilled())
        {
            DateTime dt = Form1.PurcDatePicker.Value;
            purcDate = new(dt.Year, dt.Month, dt.Day);
            purcID = Database.insertPurchase(Int32.Parse(Form1.PurcPurcPriceTLP.getLabelText()), Form1.PurcPurcNotesTLP.getLabelText(), purcDate);
            setCurrPurc(Database.getPurchase(purcID));

        }
        // Incorrectly formed new purchase from user input, don't continue on
        else if ((currPurc is null || isNewPurchase)
             && !allNewPurchaseBoxesFilled())
        {
            showWarning("To Add New Purchase, a Purchase Price, Purchase Date, and NEW ITEM Name, Initial Quantity, and Current Quantity must each be filled out");
            return;
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
        Util.clearTBox(shippingTBoxes);
        // Currently, this method gets the current item and uses it to update the curr purchase so that the idsplay can be updated with the new purchase associated with it
        // Should instead update the curr purchase and use that to update the purchase tab with the new item.
        setCurrPurcAndShowItems(tabController.getCurrPurc().PURCHASE_ID);
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
        isNewPurchase = true;
        editMode();
        //addSearchResultItem();

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
