using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class SaleTab : Tab
{

    public SaleTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
	{
        this.tabController = tabController;
        updateButton = Form1.button10;
        editButton   = Form1.button9;
        generateTBoxGroups();
        Util.clearLabelText(clearableAttribLables);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        editButton = Form1.button9;

        editingAttributeControls = new List<Control>()
        {
            Form1.textBox22,
            Form1.dateTimePicker5
        };

        editingControls = new List<Control>()
        {
            Form1.textBox22,
            Form1.dateTimePicker5,
            Form1.button10,
            Form1.button11
        };

        clearableAttribLables = new List<Label>()
        {
            Form1.label48,
            Form1.label54
        };

        itemTBoxes = new List<TextBox>()
        {
            Form1.textBox13
        };

        editableAttribLables = new List<Label>()
        {
            Form1.label48,
            Form1.label54
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
            { Form1.dateTimePicker5,  "sale.Date_Sold" },
            { Form1.textBox22,        "sale.Amount_sale" }
        };
    }

    public void update(ResultItem item)
	{
        Form1.listBox2.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item));

		foreach(ResultItem i in result)
		{
			Form1.listBox2.Items.Add(i.get_Name() + i.get_Amount_sale);
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();

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
                // TODO: Tpye Check?
                {
                    
                    string attrib = t.Text;
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        goodEdit = false;
                        continue;
                    }
                    query = QueryBuilder.buildUpdateQuery(tabController.getCurrSale(), controlBoxAttrib[c], type, t.Text);
                }
                else if (c is DateTimePicker)
                {
                    query = QueryBuilder.buildUpdateQuery(tabController.getCurrSale(), controlBoxAttrib[c], type, new Date(c));
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
        updateItemView(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
        showItem(tabController.getCurrItem());
        clearCurrSale();
    }

    private void clearCurrSale()
    {
        if (inEditingState)
        {
            flipEditMode();
        }

        Util.clearLabelText(clearableAttribLables);
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState && tabController.getCurrItem() == null) { return; }

        inEditingState = !inEditingState;
        showControlVisibility();

    }

    public void updateItemView(ResultItem item)
    {
        //showItem(item);

        Form1.listBox3.Items.Clear();
        tabController.clearCurrentItemSales();

        List<Sale> sales = getSales(item);

        foreach (Sale s in sales)
        {
            Form1.listBox3.Items.Add(s.get_Date_Sold().toDateString() + ", " + s.get_Amount_sale());
            tabController.addCurrentItemSales(s);
        }
    }

    public static List<Sale> getSales(ResultItem item)
    {
        return DatabaseConnector.RunSaleSearchQuery(QueryBuilder.buildSaleQuery(item));
    }

    public static double getTotalSales(ResultItem item)
    {
        double totalSales = 0;
        List<Sale> sales = getSales(item);
        foreach (Sale s in sales)
        {
            totalSales += s.get_Amount_sale();
        }
        return totalSales;
    }

    public double getTotalSales()
    {
        double totalSales = 0;
        foreach (Sale s in tabController.getCurrSales())
        {
            totalSales += s.get_Amount_sale();
        }
        return totalSales;
    }

    public void updateSale(Sale currSale)
    {
        tabController.setCurrSale(currSale);
        Form1.label48.Text = currSale.get_Amount_sale().ToString();
        Form1.label54.Text = currSale.get_Date_Sold().toDateString();
    }

    public void addSale()
    {

        // Must at least have name. Init and curr quantites are given a default val of 1
        if (Form1.textBox13.Text == "")
        {
            return;
        }


        Sale newItem = new Sale();
        newItem.set_Amount_sale(Int32.Parse(Form1.textBox13.Text));
        newItem.set_Date_Sold(new Date(Form1.dateTimePicker3));
        newItem.set_ItemID_sale(getCurrItem().get_ITEM_ID());

        Util.clearTBox(itemTBoxes);
        DatabaseConnector.insertSale(newItem);
        updateItemView(getCurrItem());
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
    
    public void clearAttribs()
    {
        Util.clearLabelText(clearableAttribLables);
        Util.clearTBox(itemTBoxes);
        Form1.dateTimePicker4.Value = DateTime.Now;
    }

    public void showSale(Sale sale)
    {
        throw new NotImplementedException();
    }

    public override void showItem(ResultItem item)
    {
        Util.clearLabelText(clearableAttribLables);

        if (item.hasItemEntry()){
            Form1.label51.Text = item.get_Name();
        }
    }
}
