using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using System.Xml.Linq;
using static FinancialDatabase.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace FinancialDatabase
{

    public partial class Form1 : Form
    {
        TabController tabControl;



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


            // TODO: Make CONST
            public int searchTabNum = 0;
            public int itemViewTabNum = 1;
            public int purcLotTabNum = 2;
            public int saleTabNum = 3;

            public TabController(Form1 Form1)
            {
                this.Form1 = Form1;

                colDataTypes = DatabaseConnector.getColDataTypes();
                currentPurchaseItems = new List<ResultItem>();
                currentItems = new List<ResultItem>();
                currentItemSales = new List<Sale>();

                currItem = null;

                purchasedLotTab = new PurchasedLotTab(this, Form1);
                itemViewTab = new ItemViewTab(this, Form1);
                searchTab = new SearchTab(this, Form1);
                saleTab = new SaleTab(this, Form1);
            }

            public ResultItem getCurrItem()
            {
                if (currItem != null)
                {
                    updateItem(currItem);
                }
                return currItem;
            }
            public ResultItem getCurrentItemsAt(int index)
            {
                updateItem(currentItems[index]);
                return currentItems[index];
            }
            public ResultItem getCurrentPurchaseItemsAt(int index) => currentPurchaseItems[index];
            public List<ResultItem> getCurrentPurchaseItems() => currentPurchaseItems;
            public List<ResultItem> getCurrentItems() => currentItems;
            public List<Sale> getCurrentItemSales() => currentItemSales;

            public void setCurrentPurchaseItems(List<ResultItem> newPurcItems) => this.currentPurchaseItems = newPurcItems;
            public void setCurrentItems(List<ResultItem> newItems) => this.currentItems = newItems;
            public void setCurrentItemSales(List<Sale> newSales) => this.currentItemSales = newSales;

            public void addCurrentPurchaseItems(ResultItem newPurcItems) => currentPurchaseItems.Add(newPurcItems);
            public void addCurrentItems(ResultItem newItems) => currentItems.Add(newItems);
            public void addCurrentItemSales(Sale newSales) => currentItemSales.Add(newSales);

            public void clearCurrentPurchaseItems() => currentPurchaseItems.Clear();
            public void clearCurrentItems() => currentItems.Clear();
            public void clearCurrentItemSales() => currentItemSales.Clear();

            public Sale getCurrSale() => currSale;

            public void updateCurrItem()
            {
                updateItem(currItem);
            }

            public void updateItem(ResultItem item)
            {
                if (purchasedLotTab.isNewPurchase) { return; }

                int index = currentItems.IndexOf(item);
                if (index == -1)
                {
                    throw new Exception("Item Not Found: Form1.TabController.updateItem()");
                }
                currentItems[index] = DatabaseConnector.getItem(item.get_ITEM_ID());
            }

            public void UpdateCurrSaleWithUserInput()
            {
                saleTab.editUpdate();
                saleTab.updateSaleViewListBox(currItem);
                updateCurrSale();
                saleTab.showSale(getCurrSale());
                saleTab.viewMode();
            }

            public void updateCurrSale()
            {
                updateSale(getCurrSale());
                currSale = currentItemSales[currentItemSales.IndexOf(currSale)];
            }

            public void updateSale(Sale s)
            {
                int index = currentItemSales.IndexOf(s);
                if (index == -1)
                {
                    throw new Exception("Sale Not Found: Form1.TabController.updateSale()");
                }

                currentItemSales[index] = DatabaseConnector.getSale(s.get_SALE_ID());
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



            // Update the curr item given its position in the search results
            public void setCurrItem(int index)
            {
                if (index > Form1.listBox1.Items.Count)
                {
                    throw new Exception("Index of the search results to set the new currItem to in Form1.TabController setCurrItem() is greater than the number of items in the search result");
                }
                ResultItem indexItem = getCurrentItemsAt(index);
                
                ResultItem newItem = DatabaseConnector.getItem(indexItem.get_ITEM_ID());

                List<Image> i = DatabaseConnector.getImages(newItem);

                setCurrItem(newItem);

            }

            public void setCurrItem(ResultItem newItem)
            {
                if (newItem == null) { return; }
                if (purchasedLotTab.isNewPurchase)
                {
                    currentItems.Clear();
                    currentItems.Add(newItem);
                }
                if (!currentItems.Contains(newItem) &&
                    !currentPurchaseItems.Contains(newItem) && !purchasedLotTab.isNewPurchase)
                {
                    throw new Exception("Form1.TabController.setCurrItem,"
                                      + "Result item ID: " + newItem.get_ITEM_ID().ToString() + ", "
                                      + "Name: " + newItem.get_Name() + ", "
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
                Form1.tabControl1.SelectTab(itemViewTabNum);
            }

            public void setCurrSale(int index)
            {
                // Check bad mouse click
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

                currSale = s;

                saleTab.showSale(s);
                saleTab.updateUserInputDefaultText();
                saleTab.viewMode();
            }


            private void setNewItemPurchasedLots(ResultItem newItem)
            {
                purchasedLotTab.updatePurchasedLotView(newItem);
            }

            private void setNewItemItemView(ResultItem newItem)
            {
                itemViewTab.showItemAttributes(newItem);
            }

            public void setNewItemSaleItem(ResultItem newItem)
            {
                saleTab.updateSaleViewListBox(newItem);
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
                if (!saleTab.inEditingState && getCurrSale != null)
                {
                    saleTab.flipEditMode();
                }
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



            public void saleTDelete()
            {
                string query = QueryBuilder.buildDelSaleQuery(currSale);
                string output = "";
                output = DatabaseConnector.runStatement(query);
                if (output.CompareTo("ERROR") == 0)
                {
                    throw new Exception("ERROR: Could not delete sale object");
                }

                saleTab.updateSaleViewListBox(currItem);
                updateCurrItem();
                setCurrItem(currItem);
                saleTab.clearAttribs();
                saleTab.viewMode();
                Form1.tabControl1.SelectTab(3);
                currSale = null;
            }


            public void deleteItemFromDtb()
            {

                bool deletedItem = itemViewTab.deleteItem();
                if (!deletedItem) { return; }
                currItem = null;
                currentItems.Clear();
                currentItemSales.Clear();
                currentPurchaseItems.Clear();

                purchasedLotTab.clearCurrItem();
                itemViewTab.clearCurrItem();
                saleTab.clearCurrItem();
                searchTab.clearItems();

                Form1.tabControl1.SelectTab(searchTabNum);
                searchTab.search();

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
            int currIndex = this.listBox1.IndexFromPoint(e.Location); // listBox1.SelectedIndex???
            tabControl.setCurrItem(currIndex);
            tabControl1.SelectTab(tabControl.itemViewTabNum);

        }

        // Purchased Lot listbox double click
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox2.IndexFromPoint(e.Location);
            ResultItem item = tabControl.getCurrentPurchaseItemsAt(index);
            tabControl.setCurrItem(item);
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
            tabControl1.SelectTab(tabControl.saleTabNum);
        }

        // SaleTab Update button
        private void button10_Click(object sender, EventArgs e)
        {
            tabControl.UpdateCurrSaleWithUserInput();
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

        // Item View Delete Item Button
        private void button12_Click(object sender, EventArgs e)
        {
            tabControl.deleteItemFromDtb();
        }
    }
}
