using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using FinancialDatabase;
using FinancialDatabase.Tabs;
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
        clearAttribs();
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        

        mutableAttribValueControls = new List<ControlLabelPair>()
        {
            Form1.SaleAmountTLP,
            Form1.SaleDatePickerDLP
        };

        hideableAttribValueControls = new List<Control>()
        {
            Form1.SaleAmountTLP,
            Form1.SaleDatePickerDLP,
            Form1.SaleUpdateButton,
            Form1.SaleDeleteSaleButton
        };

        allAttributeValueLabels = new List<Control>()
        {
            Form1.SaleNameLbl,
            Form1.SaleAmountTLP,
            Form1.SaleDatePickerDLP
        };

        newItemTBoxes = new List<TextBox>()
        {
            Form1.SaleNewSaleAmountTextbox
        };


        Form1.SaleAmountTLP.attrib = "sale.Amount_sale";
        Form1.SaleDatePickerDLP.attrib = "sale.Date_Sold";
    }

    public bool updateFromUserInput()
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
        return success;
        
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
        List<ControlLabelPair> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields)) { return false; }

        foreach (ControlLabelPair c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: ControlLabelpair Object c is null, ItemViewTab.cs"); continue; }

            TextBoxLabelPair t = (c as TextBoxLabelPair);

            if (databaseEntryExists(c))
            {
                Database.updateRow(currSale, c.attrib, c.getControlValueAsStr());
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

        Util.clearLabelText(allAttributeValueLabels);
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        // Unsuccessful flip of edit mode
        if (!inEditingState && getCurrSale() == null) { return; }// throw new Exception("Error: No Current Sale to Edit for flipping Sale Tab edit mode"); }

        if (!inEditingState) { recordAttributeStates(); }

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
        Util.clearLabelText(allAttributeValueLabels);
        Util.clearTBox(newItemTBoxes);

    }

    public void showSale(Sale sale)
    {
        Form1.SaleDatePickerDLP.setLabelText(sale.get_Date_Sold().toDateString());
        Form1.SaleAmountTLP.setLabelText(sale.get_Amount_sale().ToString());

    }

    public override void showItemAttributesAndPics(Item item)
    {
        Form1.SaleNameLbl.Text = ""; // Must be cleared manually as to not clear sale fields after showSale has been called (or visa versa from showSale's perspective)
        
        if (item.hasItemEntry()){
            Form1.SaleNameLbl.Text = item.get_Name();
        }
    }


    public Sale getCurrItemSales(int index)
    {
        updateCurrSalesFromDtb();
        return currItemSales[index];
    }


    public List<Sale> getCurrItemSales()
    {
        updateCurrSalesFromDtb();
        return currItemSales;
    }


    public void updateCurrSalesFromDtb()
    {
        currItemSales = getSales(tabController.getCurrItem());
    }


    public void setCurrSale(int index)
    {
        // Check bad mouse click
        if (index == -1) { return; }
        int sale_id = currItemSales[index].get_SALE_ID();

        Sale sale = Database.getSale(sale_id);
        setCurrSale(sale);
    }


    private void setCurrSale(Sale s)
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
}
