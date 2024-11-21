using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Util.clearLabelText(clearableAttribLables);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        editingAttributeControls = new List<Control>()
        {
            Form1.PurcPurcPriceTextbox,
            Form1.PurcPurcNotesTextbox,
            Form1.PurcDatePicker
        };

        editingControls = new List<Control>()
        {
            Form1.PurcPurcPriceTextbox,
            Form1.PurcPurcNotesTextbox,
            Form1.PurcDatePicker
        };

        editableAttribLables = new List<Label>()
        {
            Form1.PurcPurcPriceLbl,
            Form1.PurcPurcNotesLbl,
            Form1.PurcPurcDateLbl
        };

        clearableAttribLables = new List<Label>()
        {
            Form1.PurcPurcPriceLbl,
            Form1.PurcPurcNotesLbl,
            Form1.PurcPurcDateLbl
        };

        itemTBoxes = new List<TextBox>()
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

        foreach (Control c in itemTBoxes)
        {
            newPurchaseGroupControls.Add(c);
        }

        labelTextboxPairs = new Dictionary<Control, Label>();

        int i = 0;
        foreach (Control c in editingAttributeControls)
        {
            if (c is not Button)
            {
                labelTextboxPairs[c] = editableAttribLables[i++];
            }
        }

        controlBoxAttrib = new Dictionary<Control, string>
        {
            { Form1.PurcDatePicker,  "purchase.Date_Purchased" },
            { Form1.PurcPurcPriceTextbox,        "purchase.Amount_purchase" },
            { Form1.PurcPurcNotesTextbox,        "purchase.Notes_purchase" }
        };
    }

    public void update(ResultItem item)
	{
        Form1.PurchaseListBox.Items.Clear();
		tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item), false);

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

        Util.clearLabelText(clearableAttribLables);

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

    public void editUpdate()
    {

        if (isNewPurchase)
        {
            addItem();
            return;
        }
        else if (tabController.getCurrItem() is null)
        {
            return;
        }

        List<Control> changedFields = getChangedFields();

        bool goodEdit = true;
        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment

            string query = "";
            if (tableEntryExists(t))
            {
                string type = tabController.colDataTypes[controlBoxAttrib[c]];
                if (c is TextBox)
                {
                    
                    string attrib = t.Text;
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        goodEdit = false;
                        continue;
                    }
                    query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), controlBoxAttrib[c], type, t.Text);
                }
                else if (c is DateTimePicker)
                {
                    query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), controlBoxAttrib[c], type, new Date(c));
                }

                if (goodEdit)
                {
                    // Update the item table with the new shipping info
                    string output = DatabaseConnector.runStatement(query);
                    if (output.CompareTo("ERROR") != 0)
                    {
                        updatePurchasedLotView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
                        t.Clear();
                        t.BackColor = Color.White;
                    }
                }
            }
            else if (!tableEntryExists(t))
            {
                Console.WriteLine("ERROR: no purchase entry for CurrItem, This should not be possible");
                continue;
            }

        }
        if (goodEdit)
        {
            updatePurchasedLotView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
            showItemAttributes(tabController.getCurrItem());
            viewMode();
        }
    }

    // TODO: Delete or make part of TabController?
    public void updatePurchasedLotView(ResultItem item)
    {
        if (item == null) { return; }
        showItemAttributes(item);

        Form1.PurchaseListBox.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        // TODO: Make RunItemSearchQuery for ResultItem parameter
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item), false);

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
        } else
        {
            purcDate = new Date();
        }
        
        if (isNewPurchase && allNewPurchaseBoxesFilled())
        { 
            DateTime dt = Form1.PurcDatePicker.Value;
            purcDate = new (dt.Year, dt.Month, dt.Day);
            purcID = DatabaseConnector.newPurchase(Int32.Parse(Form1.PurcPurcPriceTextbox.Text),Form1.PurcPurcNotesTextbox.Text, purcDate);

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
        
        ResultItem newItem = new ResultItem();
        newItem.set_Name(Form1.PurcNameTextbox.Text);
        newItem.set_Date_Purchased(purcDate);

        // If no Init or curr quantities, set to default 1
        if (Form1.PurcInitQtyTextbox.Text.CompareTo("") == 0)
        {
            newItem.set_InitialQuantity(1);
        }
        else
        {
            newItem.set_InitialQuantity(Form1.PurcInitQtyTextbox.Text);
        }

        if (Form1.PurcCurrQtyTextbox.Text.CompareTo("") == 0)
        {
            newItem.set_CurrentQuantity(1);
        }
        else
        {
            newItem.set_CurrentQuantity(Form1.PurcCurrQtyTextbox.Text);
        }

        if (allNewShippingBoxesFilled())
        {
            int ttlWeight = Int32.Parse(Form1.PurcWeightLbsTextbox.Text) * 16 + Int32.Parse(Form1.PurcWeightOzTextbox.Text);
            newItem.set_Weight(ttlWeight);
            newItem.set_Length(Int32.Parse(Form1.PurcLengthTextbox.Text));
            newItem.set_Width( Int32.Parse(Form1.PurcWidthTextbox.Text));
            newItem.set_Height(Int32.Parse(Form1.PurcHeightTextbox.Text));
        }
        if (isNewPurchase)
        {
            newItem.set_PurchaseID(purcID);
            
        } else
        {
            newItem.set_PurchaseID(tabController.getCurrItem().get_PurchaseID());
        }

        Util.clearTBox(itemTBoxes);
        Util.clearTBox(shippingTBoxes);
        int itemID = -1;
        DatabaseConnector.insertItem(newItem, out itemID);
        newItem.set_ITEM_ID(itemID);
        string attrib = "item.PurchaseID";
        string type = tabController.colDataTypes[attrib];
        string query = QueryBuilder.buildUpdateQuery(newItem, attrib, type, purcID.ToString());
        DatabaseConnector.runStatement(query);
        //TODO: Can the following line be removed since tabController.setCurrItem(newItem) is called and shoeuld update the currItem with modified purc date?
        newItem.set_PurchaseID(purcID);
        tabController.setCurrItem(newItem);
        updatePurchasedLotView(tabController.getCurrItem());
        isNewPurchase = false;
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
