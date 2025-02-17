using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Security.Permissions;
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
        bool success = updateDtbFromUserInput();
        if (success)
        {
            updateSaleFromCurrSalesUsingDtb(currSale);
            currSale = currItemSales[currItemSales.IndexOf(currSale)];

            showSale(currSale);
            viewMode();
            Form1.itemSoldPriceLbl.Text = getTotalSales(tabController.getCurrItem()).ToString();
        }
        return success;
        
    }

    public Sale getCurrSale()
    {
        return this.currSale;
    }

    public void updateSaleFromCurrSalesUsingDtb(Sale s)
    {
        int index = currItemSales.IndexOf(s);
        if (index == -1)
        {
            throw new Exception("Sale Not Found: Form1.TabController.updateSale()");
        }

        currItemSales[index] = Database.getSale(s.get_SALE_ID());
    }

    public bool updateDtbFromUserInput() {

        if (tabController.getCurrItem() == null) { return false; }
        List<ControlLabelPair> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields)) { return false; }

        foreach (ControlLabelPair c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: ControlLabelPair Object c is null, ItemViewTab.cs"); continue; }
            
            Database.updateRow(currSale, c.attrib, c.getControlValueAsStr());
            Util.clearControl(c);
        }
        return true;
    }


    private void clearCurrSale()
    {
        if (inEditingState)
        {
            flipEditMode();
        }

        Util.clearControlText(allAttributeValueLabels);
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

    private void showItemSales(Item item)
    {
        Form1.saleListBox.Items.Clear();
        clearCurrItemSales();

        Form1.SaleNameLbl.Text = item.Name;

        if (item is null) { return; }

        List<Sale> sales = getSales(item);

        foreach (Sale s in sales)
        {
            Form1.saleListBox.Items.Add(s.get_Date_Sold().toDateString() + ", " + s.get_Amount_sale());
            tabController.addCurrentItemSales(s);
        }

        Form1.itemSoldPriceLbl.Text = getTotalSales(item).ToString();
        Form1.itemCurrQtyTLP.Text = item.CurrentQuantity.ToString();
    }


    public static List<Sale> getSales(Item item)
    {
        return Database.runSaleSearchQuery(item);
    }


    public double getTotalSales(Item item)
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
        if (tabController.getCurrItem().CurrentQuantity == 0)
        {
            showWarning("Cannot add sale, current quantity is 0");
            return;
        }
        if (tabController.getCurrItem() == null) return;

        if (Form1.SaleNewSaleAmountTextbox.Text == "")
        {
            return;
        }

        // Generate sale to insert
        Sale newItem = new Sale();
        newItem.set_Amount_sale(Int32.Parse(Form1.SaleNewSaleAmountTextbox.Text));
        newItem.set_Date_Sold(new Date(Form1.SaleNewSaleDatePicker));
        newItem.set_ItemID_sale(tabController.getCurrItem().ITEM_ID);

        // Insert sale
        Util.clearTBox(newItemTBoxes);
        Database.insertSale(newItem);

        Item curritem = tabController.getCurrItem();
        int newQty = curritem.CurrentQuantity - 1;
        Database.updateRow(curritem, "item.CurrentQuantity", newQty.ToString());
        
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
        Util.clearControlText(allAttributeValueLabels);
        Util.clearTBox(newItemTBoxes);

    }

    public void showSale(Sale sale)
    {
        Form1.SaleDatePickerDLP.setLabelText(sale.get_Date_Sold().toDateString());
        Form1.SaleDatePickerDLP.setControlVal(sale.get_Date_Sold().toDateString());
        Form1.SaleAmountTLP.setLabelText(sale.get_Amount_sale().ToString());
        Form1.SaleAmountTLP.setControlVal(sale.get_Amount_sale().ToString());

    }

    public override void showItemAttributes(Item item)
    {
        Form1.SaleNameLbl.Text = ""; // Must be cleared manually as to not clear sale fields after showSale has been called (or visa versa from showSale's perspective)
        
        if (item != null &&
            item.hasItemEntry()){
            Form1.SaleNameLbl.Text = item.Name;
        } else
        {
            Form1.SaleNameLbl.Text = "";
        }

        // TODO: Combine above functionality into showItemSales or showAttributes?
        showItemSales(tabController.getCurrItem());
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


    public void setCurrSale(object? o)
    {
        if (o is null)
        {
            currSale = null;
            return;
        }
    }


    public void setCurrSale(int index)
    {
        
        // Check bad mouse click
        if (index == -1) { return; }
        if (currItemSales is null || currItemSales.Count() == 0)
        {
            throw new Exception("Error: Trying to set sales when item sales don't exist");
        }
        if (index > currItemSales.Count())
        {
            throw new Exception("Error: Trying to set sales for index '" + index + "' when only total of '" + currItemSales.Count() + "' sales");
        }
        int sale_id = currItemSales[index].get_SALE_ID();

        Sale sale = Database.getSale(sale_id);
        setCurrSale(sale);
    }


    private void setCurrSale(Sale s)
    {
        if (s == null)
        {
            currSale = null;
            return;
        }
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
        setCurrSale((Sale) null);
        currItemSales.Clear();
        Form1.saleListBox.Items.Clear();
        Util.clearControls(allAttributeValueLabels);
    }
}
