using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using FinancialDatabase;
using Date = Util.Date;

public class SaleTab : Tab
{
    Sale currSale;
    List<Sale> currItemSales;
    public SaleTab(TabController tabController, Form1 Form1) : base(Form1)
	{
        currItemSales = new List<Sale>();
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
            { Form1.SaleDatePicker,    "sale.Date_Sold" },
            { Form1.SaleAmountTextbox, "sale.Amount_sale" }
        };
    }

    public void updateFromUserInput()
    {
        bool success = getUserInputUpdate();
        if (success)
        {
            showItemSales(tabController.getCurrItem());
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

        currItemSales[index] = Database.getSale(s.get_SALE_ID());
    }

    public bool getUserInputUpdate() {

        if (tabController.getCurrItem() == null) { return false; }
        List<Control> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields)) { return false; }

        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();

            if (databaseEntryExists(c))
            {
                if (c is TextBox)
                {
                    string attrib = t.Text;
                    Database.updateRow(currSale, controlAttrib[c], t.Text);
                }
                else if (c is DateTimePicker)
                {
                    Database.updateRow(currSale, controlAttrib[c], new Date(c));
                }
                Util.clearTBox(t);
            }
            else if (!databaseEntryExists(t))
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

    public void showItemSales(Item item)
    {
        Form1.saleListBox.Items.Clear();
        clearCurrItemSales();

        if (item is null) { return; }

        List<Sale> sales = getSales(item);

        foreach (Sale s in sales)
        {
            Form1.saleListBox.Items.Add(s.get_Date_Sold().toDateString() + ", " + s.get_Amount_sale());
            tabController.addCurrentItemSales(s);
        }
    }


    public static List<Sale> getSales(Item item)
    {
        return Database.runSaleSearchQuery(item);
    }

    public static double getTotalSales(Item item)
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

        if (Form1.SaleNewSaleAmountTextbox.Text == "")
        {
            return;
        }


        Sale newItem = new Sale();
        newItem.set_Amount_sale(Int32.Parse(Form1.SaleNewSaleAmountTextbox.Text));
        newItem.set_Date_Sold(new Date(Form1.SaleNewSaleDatePicker));
        newItem.set_ItemID_sale(tabController.getCurrItem().get_ITEM_ID());

        Util.clearTBox(newItemTBoxes);
        Database.insertSale(newItem);
        showItemSales(tabController.getCurrItem());
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

    public override void showItemAttributes(Item item)
    {
        Form1.SaleNameLbl.Text = ""; // Must be cleared manually as to not clear sale fields after showSale has been called (or visa versa from showSale's perspective)
        
        if (item.hasItemEntry()){
            Form1.SaleNameLbl.Text = item.get_Name();
        }
    }


    public Sale getCurrItemSales(int index)
    {
        return this.currItemSales[index];
    }


    public List<Sale> getCurrItemSales()
    {
        return this.currItemSales;
    }


    public void setCurrSale(int index)
    {
        // Check bad mouse click
        if (index == -1) { return; }
        int sale_id = currItemSales[index].get_SALE_ID();

        Sale sale = Database.getSale(sale_id);
        setCurrSale(sale);
    }


    public void setCurrSale(Sale s)
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

    public bool deleteCurrSale()
    {
        
        bool success = Database.deleteSale(currSale);
        if (!success) { return false; }

        
        clearAttribs();
        viewMode();
        currSale = null;
        return true;
    }
    
    public void clearCurrItemSales()
    {
        currItemSales.Clear();
    }

    public void setCurrSales(List<Sale> newSales)
    {
        currItemSales = newSales;
    }
}
