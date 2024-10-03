using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class PurchasedLotTab : Tab
{

    bool isNewPurchase;

    public PurchasedLotTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
	{
        this.tabController = tabController;
        isNewPurchase = false;
        updateButton = Form1.button7;
        editButton   = Form1.button6;
        generateTBoxGroups();
        Util.clearLabelText(clearableAttribLables);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        editingAttributeControls = new List<Control>()
        {
            Form1.textBox20,
            Form1.textBox21,
            Form1.dateTimePicker4
        };

        editingControls = new List<Control>()
        {
            Form1.textBox20,
            Form1.textBox21,
            Form1.dateTimePicker4
        };

        editableAttribLables = new List<Label>()
        {
            Form1.label15,
            Form1.label41,
            Form1.label44
        };

        clearableAttribLables = new List<Label>()
        {
            Form1.label15,
            Form1.label41,
            Form1.label44
        };

        itemTBoxes = new List<TextBox>()
        {
            Form1.textBox2,
            Form1.textBox14,
            Form1.textBox11,

        };

        shippingTBoxes = new List<TextBox>()
        {
            Form1.textBox15,
            Form1.textBox16,
            Form1.textBox17,
            Form1.textBox18,
            Form1.textBox19

        };

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
            { Form1.dateTimePicker4,  "purchase.Date_Purchased" },
            { Form1.textBox20,        "purchase.Amount_purchase" },
            { Form1.textBox21,        "purchase.Notes_purchase" }
        };
    }

    public void update(ResultItem item)
	{
        Form1.listBox2.Items.Clear();
		tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item));

		foreach(ResultItem i in result)
		{
			Form1.listBox2.Items.Add(i.get_Name());
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();

	}

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState && tabController.getCurrItem() == null) { return; }

        inEditingState = !inEditingState;
        showControlVisibility();

    }

    public override void showItem(ResultItem item)
    {
        Util.clearLabelText(clearableAttribLables);

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.dateTimePicker4.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.label15.Text = checkDefault(item.get_Amount_purchase());
            Form1.label41.Text = checkDefault(item.get_Notes_purchase());
            Form1.label44.Text = item.get_Date_Purchased().toDateString();
        }
        updateUserInputDefaultText();
    }

    public void editUpdate()
    {
        if (tabController.getCurrItem() == null) { return; }
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
                        updateItemView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
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
            updateItemView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
            showItem(tabController.getCurrItem());
            viewMode();
        }
    }

    public void updateItemView(ResultItem item)
    {
        showItem(item);

        Form1.listBox2.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item));

        foreach (ResultItem i in result)
        {
            Form1.listBox2.Items.Add(i.get_Name());
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();
    }

    public bool allNewPurchaseBoxesFilled()
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

        // Must at least have name. Init and curr quantites are given a default val of 1
        if (Form1.textBox2.Text == "")
        {
            return;
        }
        int purcID = -1;
        if (isNewPurchase && allNewPurchaseBoxesFilled())
        { 
            DateTime dt = Form1.dateTimePicker4.Value;
            Date d = new (dt.Year, dt.Month, dt.Day);
            purcID = DatabaseConnector.newPurchase(Int32.Parse(Form1.textBox20.Text),Form1.textBox21.Text, d);

        }
        // Incorrectly formed new purchase from user input, don't continue on
        else if (!allNewPurchaseBoxesFilled())
        {
            return;
        }
        
        ResultItem newItem = new ResultItem();
        newItem.set_Name(Form1.textBox2.Text);

        // If no Init or curr quantities, set to default 1
        if (Form1.textBox14.Text.CompareTo("") == 0)
        {
            newItem.set_InitialQuantity(1);
        }
        else
        {
            newItem.set_InitialQuantity(Form1.textBox14.Text);
        }

        if (Form1.textBox11.Text.CompareTo("") == 0)
        {
            newItem.set_CurrentQuantity(1);
        }
        else
        {
            newItem.set_CurrentQuantity(Form1.textBox11.Text);
        }

        if (allNewShippingBoxesFilled())
        {
            int ttlWeight = Int32.Parse(Form1.textBox18.Text) * 16 + Int32.Parse(Form1.textBox19.Text);
            newItem.set_Weight(ttlWeight);
            newItem.set_Length(Int32.Parse(Form1.textBox15.Text));
            newItem.set_Width( Int32.Parse(Form1.textBox16.Text));
            newItem.set_Height(Int32.Parse(Form1.textBox17.Text));
        }
        if (isNewPurchase)
        {
            newItem.set_PurchaseID(purcID);
            isNewPurchase = false;
        } else
        {
            newItem.set_PurchaseID(tabController.getCurrItem().get_PurchaseID());
        }

        Util.clearTBox(itemTBoxes);
        Util.clearTBox(shippingTBoxes);
        DatabaseConnector.insertItem(newItem);

        updateItemView(tabController.getCurrItem());
    }

    // Clear out old information
    // If there is a purchased item able to be added,
    // make a new purchase in the database and add it
    public void newPurchase()
    {
        Form1.listBox2.Items.Clear();
        editMode();
        isNewPurchase = true;
        addItem();

    }
}
