using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class SaleTab : Tab
{

    public SaleTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
	{
        this.tabController = tabController;
        editButton = Form1.SaleEditSaleButton;
        updateButton = Form1.SaleUpdateButton;
        generateTBoxGroups();
        Util.clearLabelText(clearableAttribLables);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        

        editingAttributeControls = new List<Control>()
        {
            Form1.SaleAmountTextbox,
            Form1.SaleDatePicker
        };

        editingControls = new List<Control>()
        {
            Form1.SaleAmountTextbox,
            Form1.SaleDatePicker,
            Form1.SaleUpdateButton,
            Form1.SaleDeleteSaleButton
        };

        clearableAttribLables = new List<Label>()
        {
            Form1.SaleNameLbl,
            Form1.SaleAmountLbl,
            Form1.SaleDateLbl
        };

        itemTBoxes = new List<TextBox>()
        {
            Form1.SaleNewSaleAmountTextbox
        };

        editableAttribLables = new List<Label>()
        {
            Form1.SaleAmountLbl,
            Form1.SaleDateLbl
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
            { Form1.SaleDatePicker,  "sale.Date_Sold" },
            { Form1.SaleAmountTextbox,        "sale.Amount_sale" }
        };
    }

    public void update(ResultItem item)
	{
        Form1.PurchaseListBox.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(QueryBuilder.buildPurchaseQuery(item), false);

		foreach(ResultItem i in result)
		{
			Form1.PurchaseListBox.Items.Add(i.get_Name() + i.get_Amount_sale);
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.PurcPurcPriceLbl.Text = item.get_Amount_purchase().ToString();
        Form1.PurcPurcNotesLbl.Text = item.get_Notes_purchase();

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
                        // TODO: Show an error message for incorrect attribute inputted!
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
                        t.Clear();
                        t.BackColor = Color.White;
                    }
                    else
                    {
                        throw new Exception("ERROR: Bad DatabaseConnector.runStatement: " + query);
                    }
                }
            }
            else if (!tableEntryExists(t))
            {
                Console.WriteLine("ERROR: no purchase entry for CurrItem, This should not be possible");
                continue;
            }
        }
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
        if (inEditingState) updateUserInputDefaultText();
        showControlVisibility();

    }

    public void updateSaleViewListBox(ResultItem item)
    {
        

        Form1.saleListBox.Items.Clear();
        tabController.clearCurrentItemSales();

        List<Sale> sales = getSales(item);

        foreach (Sale s in sales)
        {
            Form1.saleListBox.Items.Add(s.get_Date_Sold().toDateString() + ", " + s.get_Amount_sale());
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

    public void addSale()
    {
        if (tabController.getCurrItem() == null) return;
        // Must at least have name. Init and curr quantites are given a default val of 1
        if (Form1.SaleNewSaleAmountTextbox.Text == "")
        {
            return;
        }


        Sale newItem = new Sale();
        newItem.set_Amount_sale(Int32.Parse(Form1.SaleNewSaleAmountTextbox.Text));
        newItem.set_Date_Sold(new Date(Form1.SaleNewSaleDatePicker));
        newItem.set_ItemID_sale(getCurrItem().get_ITEM_ID());

        Util.clearTBox(itemTBoxes);
        DatabaseConnector.insertSale(newItem);
        updateSaleViewListBox(getCurrItem());
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
        Form1.PurcDatePicker.Value = DateTime.Now;
    }

    public void showSale(Sale sale)
    {
        Form1.SaleAmountLbl.Text = sale.get_Amount_sale().ToString();
        Form1.SaleDateLbl.Text = sale.get_Date_Sold().toDateString();

    }

    public override void showItemAttributes(ResultItem item)
    {
        Form1.SaleNameLbl.Text = ""; // Must be cleared manually as to not clear sale fields after showSale has been called (or visa versa from showSale's perspective)
        
        if (item.hasItemEntry()){
            Form1.SaleNameLbl.Text = item.get_Name();
        }
    }
}
