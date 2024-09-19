using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class PurchasedLotTab : Tab
{

    bool isNewPurchase;

    public PurchasedLotTab(Form1 Form1) : base(Form1)
	{
        isNewPurchase = false;
        updateButton = Form1.button7;
        editButton   = Form1.button6;
    }

    protected override void generateTBoxGroups()
    {
        editingControls = new List<Control>()
        {
            Form1.textBox20,
            Form1.textBox21,
            Form1.dateTimePicker4
        };

        editControls = new List<Control>()
        {
            Form1.textBox20,
            Form1.textBox21,
            Form1.dateTimePicker4
        };

        nonEditingControls = new List<Control>()
        {
            Form1.label15,
            Form1.label41,
            Form1.label44
        };

        nonEditingLabels = new List<Label>()
        {
            Form1.label15,
            Form1.label41,
            Form1.label44
        };

        allPurchaseLabels = new List<Label>()
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

        for (int i = 0; i < editControls.Count; i++)
        {
            labelTextboxPairs[editControls[i]] = nonEditingLabels[i];
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
		Form1.currentPurchaseItems.Clear();
        List<ResultItem> result = PyConnector.RunItemSearchQuery(QB.buildPurchaseQuery(item));

		foreach(ResultItem i in result)
		{
			Form1.listBox2.Items.Add(i.get_Name());
            Form1.currentPurchaseItems.Add(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();

	}

    public void editUpdate()
    {
        if (Form1.currItem == null) { return; }
        List<Control> changedFields = getChangedFields();


        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment

            string query = "";
            if (tableEntryExists(t))
            {
                string type = Form1.colDataTypes[controlBoxAttrib[c]];

                if (c is TextBox)
                {
                    query = QB.buildUpdateQuery(Form1.currItem, controlBoxAttrib[c], type, t.Text);
                }
                else if (c is DateTimePicker)
                {
                    query = QB.buildUpdateQuery(Form1.currItem, controlBoxAttrib[c], type, new Date(c));
                }

                // Update the item table with the new shipping info
                string output = PyConnector.runStatement(query);
                updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID())); // Will also reset currItem with new search for it
                t.Clear();
                t.BackColor = Color.White;
            }
            else if (!tableEntryExists(t))
            {
                Console.WriteLine("ERROR: no purchase entry for CurrItem, This should not be possible");
                continue;
            }

        }
        updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID()));
        showItem(Form1.currItem);
    }

    public void updateItemView(ResultItem item)
    {
        showItem(item);

        Form1.listBox2.Items.Clear();
        Form1.currentPurchaseItems.Clear();
        List<ResultItem> result = PyConnector.RunItemSearchQuery(QB.buildPurchaseQuery(item));

        foreach (ResultItem i in result)
        {
            Form1.listBox2.Items.Add(i.get_Name());
            Form1.currentPurchaseItems.Add(i);
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
        // Shouldn't be pulling from these fields if not in editing state
        if (!inEditingState)
        {
            return false;
        }
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
            purcID = PyConnector.newPurchase(Int32.Parse(Form1.textBox20.Text),Form1.textBox21.Text, d);

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
            newItem.set_PurchaseID(Form1.currItem.get_PurchaseID());
        }
        

        PyConnector.insertItem(newItem);

        updateItemView(Form1.currItem);
    }

    public void newPurchase()
    {
        // clear the box
        Form1.listBox2.Items.Clear();

        isNewPurchase = true;
        addItem();

    }

   
}
