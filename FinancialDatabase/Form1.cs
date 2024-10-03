﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace FinancialDatabase
{

    public partial class Form1 : Form
    {        
        TabController tabControl;

        // TODO: Make CONST
        int searchTab = 0;
        int itemViewTab = 1;
        int purcLotTab = 2;
        int saleTab = 3;

        // Used when tabs are required to work in harmony and
        // send information between each other
        //
        // Ex: updating currItem in the search tab means one
        // must change the item view tab, and the
        // purchased lot tab, .etc
        public class TabController
        {
            Form1 Form1;

            // Data
            public Dictionary<string, string> colDataTypes;
            public List<ResultItem> currentPurchaseItems;
            public List<ResultItem> currentItems;
            public List<Sale> currentItemSales;
            private ResultItem currItem;
            Sale currSale;

            // Tab references
            PurchasedLotTab purchasedLotTab;
            ItemViewTab itemViewTab;
            SearchTab searchTab;
            SaleTab saleTab;

            public TabController(Form1 Form1)
            {
                this.Form1 = Form1;

                colDataTypes = DatabaseConnector.getColDataTypes();
                purchasedLotTab = new PurchasedLotTab(this, Form1);
                itemViewTab = new ItemViewTab(this, Form1);
                searchTab = new SearchTab(this, Form1);
                saleTab = new SaleTab(this, Form1);
            }
            
            public ResultItem getCurrItem() => currItem;
            public ResultItem getCurrentItemsAt(int index) => currentItems[index];
            public ResultItem getCurrentPurchaseItemsAt(int index) => currentPurchaseItems[index];
            public List<ResultItem> getCurrentPurchaseItems() => currentPurchaseItems;
            public List<ResultItem> getCurrentItems()         => currentItems;
            public List<Sale> getCurrentItemSales()           => currentItemSales;
            
            public void setCurrentPurchaseItems(List<ResultItem> newPurcItems)  => this.currentPurchaseItems = newPurcItems;
            public void setCurrentItems(List<ResultItem> newItems)              => this.currentItems = newItems;
            public void setCurrentItemSales(List<Sale> newSales)                => this.currentItemSales = newSales;

            public void addCurrentPurchaseItems(ResultItem newPurcItems) => currentPurchaseItems.Add(newPurcItems);
            public void addCurrentItems(ResultItem newItems)             => currentItems.Add(newItems);
            public void addCurrentItemSales(Sale newSales)               => currentItemSales.Add(newSales);

            public void clearCurrentPurchaseItems() => currentPurchaseItems.Clear();
            public void clearCurrentItems()         => currentItems.Clear();
            public void clearCurrentItemSales()     => currentItemSales.Clear();

            public Sale getCurrSale() => currSale;

            public void updateCurrItem()
            {
                updateItem(currItem);
            }

            public void updateItem(ResultItem item)
            {
                // Double check if this works!!!
                int index = currentItems.IndexOf(item);
                if (index == -1)
                {
                    throw new Exception("Item Not Found: Form1.TabController.updateItem()");
                }

                currentItems[index] = DatabaseConnector.getItem(index);
            }

            public ResultItem getItem(int index)
            {
                updateItem(currentItems[index]);
                return currentItems[index];
            }

            public Sale getCurrSaleAt(int index)
            {
                return currentItemSales[index];
            }

            public List<Sale> getCurrSales() => currentItemSales;

            private void clearItems()
            {
                Form1.listBox1.Items.Clear();
                currentItems.Clear();
            }

            public void addItem(ResultItem newItem)
            {
                Form1.listBox1.Items.Add(newItem.get_Name());
                currentItems.Add(newItem);
                // TODO: CHECK IF newItem already in list
            }


            
            // Update the curr item given its itemID
            public void setCurrItem(int item_id)
            {
                ResultItem newItem = DatabaseConnector.getItem(item_id);
                setCurrItem(newItem);
                
            }

            public void setCurrItem(ResultItem newItem)
            {
                if (!currentItems.Contains(newItem) ||
                    !currentPurchaseItems.Contains(newItem))
                {
                    throw new Exception("Form1.TabController.setCurrItem,"
                                      + "Result item ID: " + newItem.get_ITEM_ID().ToString() + ", "
                                      +           "Name: " + newItem.get_Name()               + ", "
                                      + "Not found!");
                }
                
                if (newItem == null)
                {
                    throw new Exception("Item Not Found: Form1.TabControl.setCurrItem()");
                }

                updateItem(newItem);

                this.currItem = newItem;

                setNewItemItemView(newItem);
                setNewItemPurchasedLots(newItem);
                setNewItemSaleItem(newItem);
            }

            public void setCurrSale(int index)
            {
                // Bad mouse click
                if (index == -1) { return; }
                int sale_id = currentItemSales[index].get_SALE_ID();

                Sale sale = DatabaseConnector.getSale(sale_id);
                setCurrSale(sale);
            }

            public void setCurrSale(Sale s)
            {
                if (!currentItemSales.Contains(s))
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

                saleTab.updateSale(s); // This func will set currSale. Only tab should update currSale, since of the 2, the tab is the only one that can be justifiablly necessary to have that functionality
                saleTab.viewMode();
            }


            private void setNewItemPurchasedLots(ResultItem newItem)
            {
                purchasedLotTab.showItem(newItem);
            }

            private void setNewItemItemView(ResultItem newItem)
            {
                itemViewTab.showItem(newItem);
            }

            public void setNewItemSaleItem(ResultItem newItem)
            {
                saleTab.showItem(newItem);
            }

            public void runManualQuery(string query)
            {
                clearItems();

                List<ResultItem> result = DatabaseConnector.RunItemSearchQuery(query);

                foreach (ResultItem item in result)
                {
                    addItem(item);
                }
            }

            public void search()
            {
                searchTab.search();
            }

            public void getItemViewUpdate()
            {
                itemViewTab.UpdateCurrItemWithUserInput();
            }

            public void greyTextBox(TextBox textBox)
            {

                string attrib = "";
                if (itemViewTab.controlBoxAttrib.ContainsKey(textBox))
                {
                    attrib = itemViewTab.controlBoxAttrib[textBox];
                }
                else if (purchasedLotTab.controlBoxAttrib.ContainsKey(textBox))
                {
                    attrib = purchasedLotTab.controlBoxAttrib[textBox];
                }
                else
                {
                    return;
                }

                string type = colDataTypes[attrib];

                // If not right type, return
                if (!Util.checkTypeOkay(textBox.Text, type))
                {
                    textBox.Text = "";
                    return;
                }

                string attribVal = "";
                currItem.getAttribAsStr(attrib, ref attribVal);
                if (attribVal.CompareTo(textBox.Text) != 0)
                {
                    textBox.BackColor = Color.LightGray;
                }
            }

            public void flipIVEditMode()
            {
                itemViewTab.flipEditMode();
            }

            public void IVdeleteShippingInfo()
            {
                itemViewTab.deleteShippingInfo();
            }

            public void PLflipEditMode()
            {
                purchasedLotTab.flipEditMode();
            }

            public void PLaddItem()
            {
                purchasedLotTab.addItem();
            }

            public void PLnewPurchase()
            {
                currItem = null;
                purchasedLotTab.newPurchase();
                itemViewTab.clearCurrItem();
            }

            public void saleTflipEditMode()
            {
                saleTab.flipEditMode();
            }

            public void saleTaddSale()
            {
                saleTab.addSale();
                setCurrItem(currItem);
            }

            public void getPurchasedLotUpdate()
            {
                purchasedLotTab.editUpdate();
            }

            public void updateCurrSale()
            {
                saleTab.editUpdate();
                saleTab.viewMode();
            }

            public void saleTDelete()
            {
                string query = QueryBuilder.buildDelSaleQuery(currSale);
                string output = "";
                output = DatabaseConnector.runStatement(query);
                if (output.CompareTo("ERROR") == 0)
                {
                    throw new Exception("ERROR: Could not delete sale object");
                }

                saleTab.updateItemView(currItem);
                updateCurrItem();
                setCurrItem(currItem);
                saleTab.viewMode();
                Form1.tabControl1.SelectTab(3);
                currSale = null;
            }
        }


        public Form1()
        {
            InitializeComponent();

            tabControl = new TabController(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Make default selection "Item"
            comboBox1.SelectedIndex = 0;
        }


        // ItemViewTab Update button
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl.runManualQuery(textBox1.Text);
        }

        // Search Button in Search Tab
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl.search();
        }


        // Search listbox double click
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            tabControl.setCurrItem(tabControl.getCurrentItemsAt(index));
            tabControl1.SelectTab(itemViewTab);
        }

        // Purchased Lot listbox double click
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox2.IndexFromPoint(e.Location);
            tabControl.setCurrItem(tabControl.getCurrentPurchaseItemsAt(index));
        }

        // Enter key pressed for search
        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tabControl.search();
            }
        }

        // Item Tab Update Item
        private void button1_Click_1(object sender, EventArgs e)
        {
            tabControl.getItemViewUpdate();
        }

        // Change TextBox color if changed
        private void TextBoxAttribute_Leave(object sender, EventArgs e)
        {
            if (sender is null) return;

#pragma warning disable CS8600 // Checked if sender is null

            TextBox textBox = sender as TextBox;

#pragma warning disable CS8604 // Checked if sender is null

            tabControl.greyTextBox(textBox);

        }

        // ItemView View/Edit Button
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl.flipIVEditMode();
        }

        // ItemView Delete Shipping Info
        private void button5_Click(object sender, EventArgs e)
        {
            tabControl.IVdeleteShippingInfo();
        }

        // PurchasedLot View/Edit Button
        private void button6_Click(object sender, EventArgs e)
        {
            tabControl.PLflipEditMode();
        }

        // PurchasedLot update item
        private void button7_Click(object sender, EventArgs e)
        {
            tabControl.getPurchasedLotUpdate();
        }

        // Add sale to purchase
        private void button2_Click_1(object sender, EventArgs e)
        {
            tabControl.PLaddItem();
        }

        // New Purchase
        private void button3_Click(object sender, EventArgs e)
        {
            tabControl.PLnewPurchase();
        }

        // SaleTab View/Edit button
        private void button9_Click(object sender, EventArgs e)
        {
            tabControl.saleTflipEditMode();
        }

        // SaleTab select sale
        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox3.IndexFromPoint(e.Location);
            tabControl.setCurrSale(index);
            tabControl1.SelectTab(saleTab);
        }

        // Update SaleTab Sale info
        private void button10_Click(object sender, EventArgs e)
        {
            tabControl.updateCurrSale();
        }

        // SaleTab Add Sale
        private void button8_Click(object sender, EventArgs e)
        {
            tabControl.saleTaddSale();
        }

        // SaleTab Delete Button
        private void button11_Click(object sender, EventArgs e)
        {
            tabControl.saleTDelete();
        }
    }
}
