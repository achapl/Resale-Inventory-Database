using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class SaleTab : Tab
{
    Sale currSale;
    List<Sale> currItemSales;
    public SaleTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
	{
        this.tabController = tabController;
        editButton = Form1.SaleEditSaleButton;
        updateButton = Form1.SaleUpdateButton;
        generateTBoxGroups();
        Util.clearLabelText(attributeValueLabels);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        

        mutableAttribValueControls = new List<Control>()
        {
            Form1.SaleAmountTextbox,
            Form1.SaleDatePicker
        };

        hideableAttribValueControls = new List<Control>()
        {
            Form1.SaleAmountTextbox,
            Form1.SaleDatePicker,
            Form1.SaleUpdateButton,
            Form1.SaleDeleteSaleButton
        };

        attributeValueLabels = new List<Label>()
        {
            Form1.SaleNameLbl,
            Form1.SaleAmountLbl,
            Form1.SaleDateLbl
        };

        newItemTBoxes = new List<TextBox>()
        {
            Form1.SaleNewSaleAmountTextbox
        };

        mutableAttribValueLabels = new List<Label>()
        {
            Form1.SaleAmountLbl,
            Form1.SaleDateLbl
        };

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
            { Form1.SaleDatePicker,  "sale.Date_Sold" },
            { Form1.SaleAmountTextbox,        "sale.Amount_sale" }
        };
    }

    public void update(ResultItem item)
	{
        Form1.PurchaseListBox.Items.Clear();
        tabController.clearCurrentPurchaseItems();
        List<ResultItem> result = DatabaseConnector.getItems(QueryBuilder.purchaseQuery(item), false);

		foreach(ResultItem i in result)
		{
			Form1.PurchaseListBox.Items.Add(i.get_Name() + i.get_Amount_sale);
            tabController.addCurrentPurchaseItems(i);
        }

        Form1.PurcPurcPriceLbl.Text = item.get_Amount_purchase().ToString();
        Form1.PurcPurcNotesLbl.Text = item.get_Notes_purchase();

	}

    public void updateFromUserInput()
    {
        bool success = getUserInputUpdate();
        if (success)
        {
            updateSaleViewListBox(tabController.getCurrItem());
            updateSale(getCurrSale());
            currSale = currItemSales[currItemSales.IndexOf(currSale)];

            showSale(getCurrSale());
            viewMode();
        }
        
    }

    public Sale getCurrSale()
    {
        return this.currSale;
    }

    public void updateSale(Sale s)
    {
        int index = currItemSales.IndexOf(s);
        if (index == -1)
        {
            throw new Exception("Sale Not Found: Form1.TabController.updateSale()");
        }

        currItemSales[index] = DatabaseConnector.getSale(s.get_SALE_ID());
    }

    public bool getUserInputUpdate() {

        if (tabController.getCurrItem() == null) { return false; }
        List<Control> changedFields = getChangedFields();

        bool goodEdit = true;
        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment

            string query = "";
            if (tableEntryExists(t))
            {
                string type = tabController.colDataTypes[controlAttrib[c]];
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
                    query = QueryBuilder.updateQuery(currSale, controlAttrib[c], type, t.Text);
                }
                else if (c is DateTimePicker)
                {
                    query = QueryBuilder.updateQuery(currSale, controlAttrib[c], type, new Date(c));
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
                throw new Exception("ERROR: no purchase entry for CurrItem, This should not be possible");
            }
        }
        return true;
    }



    private void clearCurrSale()
    {
        if (inEditingState)
        {
            flipEditMode();
        }

        Util.clearLabelText(attributeValueLabels);
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState && getCurrSale() == null) { return; }

        inEditingState = !inEditingState;
        if (inEditingState) updateUserInputDefaultText();
        showControlVisibility();

    }

    public void updateSaleViewListBox(ResultItem item)
    {
        Form1.saleListBox.Items.Clear();
        clearCurrItemSales();

        List<Sale> sales = getSales(item);

        foreach (Sale s in sales)
        {
            Form1.saleListBox.Items.Add(s.get_Date_Sold().toDateString() + ", " + s.get_Amount_sale());
            tabController.addCurrentItemSales(s);
        }
    }


    public static List<Sale> getSales(ResultItem item)
    {
        return DatabaseConnector.RunSaleSearchQuery(QueryBuilder.saleQuery(item));
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


    public void addSale(Sale sale)
    {
        currItemSales.Add(sale);
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
        newItem.set_ItemID_sale(tabController.getCurrItem().get_ITEM_ID());

        Util.clearTBox(newItemTBoxes);
        DatabaseConnector.insertSale(newItem);
        updateSaleViewListBox(tabController.getCurrItem());
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
        Util.clearLabelText(attributeValueLabels);
        Util.clearTBox(newItemTBoxes);
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


    internal Sale getCurrItemSales(int index)
    {
        return this.currItemSales[index];
    }


    internal List<Sale> getCurrItemSales()
    {
        return this.currItemSales;
    }


    internal void setCurrSale(int index)
    {
        // Check bad mouse click
        if (index == -1) { return; }
        int sale_id = currItemSales[index].get_SALE_ID();

        Sale sale = DatabaseConnector.getSale(sale_id);
        setCurrSale(sale);
    }


    internal void setCurrSale(Sale s)
    {
        if (!currItemSales.Contains(s))
        {
            throw new Exception("Form1.TabController.setCurrSale,"
                              + "Sale item ID: " + s.get_ItemID_sale().ToString() + ", "
                              + "Sale sale ID: " + s.get_SALE_ID().ToString() + ", "
                              + "Not found!");
        }

        if (s == null)
        {
            throw new Exception("Sale Not Found: Form1.TabControl.setCurrSale()");
        }

        currSale = s;

        showSale(s);
        updateUserInputDefaultText();
        viewMode();
    }

    internal bool deleteCurrSale()
    {
        string query = QueryBuilder.deleteSaleQuery(currSale);
        string output = "";
        output = DatabaseConnector.runStatement(query);
        if (output.CompareTo("ERROR") == 0)
        {
            return false;
        }

        
        clearAttribs();
        viewMode();
        currSale = null;
        return true;
    }

    internal void clearCurrItemSales()
    {
        currItemSales.Clear();
    }

    internal void setCurrSales(List<Sale> newSales)
    {
        currItemSales = newSales;
    }
}
