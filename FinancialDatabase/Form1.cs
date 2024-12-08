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
        public TabController tabControl;



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
            private Dictionary<Control, string> allControlAttribs;
            


            // Tab references
            PurchasedLotTab purchasedLotTab;
            ItemViewTab itemViewTab;
            SearchTab searchTab;
            SaleTab saleTab;


            // TODO: Make CONST
            public int searchTabNum     = 0;
            public int itemViewTabNum  = 1;
            public int purcLotTabNum  = 2;
            public int saleTabNum    = 3;

            public TabController(Form1 Form1)
            {
                this.Form1 = Form1;
                colDataTypes = DatabaseConnector.getColDataTypes();
                

                purchasedLotTab = new PurchasedLotTab(this, Form1);
                itemViewTab = new ItemViewTab(this, Form1);
                searchTab = new SearchTab(this, Form1);
                saleTab = new SaleTab(this, Form1);

                allControlAttribs = Util.combineDictionaries(purchasedLotTab.controlAttrib,
                                                             itemViewTab.controlAttrib,
                                                             saleTab.controlAttrib);

            }

            // currItem
            public void setCurrItemVar(ResultItem newItem) => itemViewTab.setCurrItem(newItem);
            public ResultItem getCurrItem() => itemViewTab.getCurrItem();
            

            // searchItems
            public void setSearchItems(List<ResultItem> newItems) => searchTab.addSearchItems(newItems);
            public ResultItem getSearchItemsAt(int index) => itemViewTab.getCurrItemsAt(index);
            public void addSearchItems(ResultItem newItem) => searchTab.addSearchItems(newItem);
            public List<ResultItem> getSearchItems() => searchTab.getSearchItems();
            public void clearSearchItems()
            {
                Form1.itemSearchView.clearItems();
                searchTab.clearCurrItemsVar();
            }


            // currPurcItems
            public void setCurrPurcItems(List<ResultItem> newPurcItems) => purchasedLotTab.setCurrPurcItems(newPurcItems);
            public void addCurrPurcItems(ResultItem newPurcItem) => purchasedLotTab.addCurrPurcItem(newPurcItem);
            public ResultItem getCurrPurcItemsAt(int index) => purchasedLotTab.getCurrPurcItemsAt(index);
            public List<ResultItem> getCurrPurcItems() => purchasedLotTab.getCurrPurcItems();
            public void clearCurrPurcItems() => purchasedLotTab.clearCurrPurcItems();
            
            
            // currSale
            public Sale getCurrSale() => saleTab.getCurrSale();


            // currSales
            public void setCurrentItemSales(List<Sale> newSales) => saleTab.setCurrSales(newSales);
            public void addCurrentItemSales(Sale newSales) => saleTab.addSale(newSales);
            public Sale getCurrSaleAt(int index) => saleTab.getCurrItemSales(index);
            public List<Sale> getCurrentItemSales() => saleTab.getCurrItemSales();
            public void setCurrSale(int index) => saleTab.setCurrSale(index);
            public void setCurrSale(Sale s) => saleTab.setCurrSale(s);



            public void saleTabUpdate()
            {
                saleTab.updateFromUserInput();
            }
            

            // Update the program (the model of the database) with a new resultItem, not just the backend variable currItem
            public void setCurrItem(ResultItem newItem)
            {
                setCurrItemVar(newItem);

                itemViewTab.showItemAttributes(newItem);
                purchasedLotTab.showItemsFromLot(newItem);
                saleTab.showItemSales(newItem);

                Form1.tabCollection.SelectTab(itemViewTabNum);
            }


            // Update the curr item given its position in the search results
            public void setCurrItem(int index)
            {
                if (index > Form1.itemSearchView.countItems())
                {
                    throw new Exception("Index of the search results to set the new currItem to in Form1.TabController setCurrItem() is greater than the number of items in the search result");
                }
                ResultItem shellItem = getSearchItemsAt(index);

                ResultItem newItem = DatabaseConnector.getItem(shellItem.get_ITEM_ID());
                newItem.set_images(DatabaseConnector.getAllImages(newItem));
                setCurrItem(newItem);

            }


            public void search()
            {
                searchTab.search();
                Form1.itemSearchView.updatePaint();
            }


            public void itemViewUpdate()
            {
                itemViewTab.updateFromUserInput();
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
                itemViewTab.clearCurrItem();
                purchasedLotTab.newPurchase();
                itemViewTab.clearCurrItemControls();
            }

            public void saleTflipEditMode()
            {
                saleTab.flipEditMode();
            }

            public void saleTaddSale()
            {
                saleTab.addSale();
                itemViewTab.setCurrItem(itemViewTab.getCurrItem());
            }

            public void purchasedLotUpdate()
            {
                purchasedLotTab.updateFromUserInput();
            }



            public void deleteCurrSale()
            {
                bool success = saleTab.deleteCurrSale();
                if (success)
                {
                    saleTab.showItemSales(itemViewTab.getCurrItem());
                    itemViewTab.updateCurrItem();
                    itemViewTab.setCurrItem(itemViewTab.getCurrItem());
                    Form1.tabCollection.SelectTab(saleTabNum);
                }
            }


            public void deleteItemFromDtb()
            {
                bool deletedItem = itemViewTab.deleteItem();
                if (!deletedItem) { return; }
                itemViewTab.setCurrItem(null);
                clearSearchItems();
                saleTab.clearCurrItemSales();
                clearCurrPurcItems();

                purchasedLotTab.clearCurrItemControls();
                itemViewTab.clearCurrItemControls();
                saleTab.clearCurrItemControls();
                searchTab.clearSearchItems();

                Form1.tabCollection.SelectTab(searchTabNum);
                searchTab.search();

            }

            internal void setMainImage(int currIndex)
            {
                itemViewTab.setMainImage(currIndex);
            }

            public void insertImage()
            {
                if (Form1.tabCollection.SelectedIndex != itemViewTabNum ||
                    getCurrItem() == null)
                {
                    return;
                }
                DialogResult isOkay = Form1.openFileDialog1.ShowDialog();
                if (isOkay == DialogResult.OK)
                {
                    List<string> files = new List<string>(Form1.openFileDialog1.FileNames);
                    foreach (string file in files)
                    {
                        DatabaseConnector.insertImage(file, itemViewTab.getCurrItem().get_ITEM_ID());
                    }
                }
            }

            public void setThumbnail()
            {
                itemViewTab.setThumbnail();
            }

            internal bool isNewPurchase()
            {
                return purchasedLotTab.isNewPurchase;
            }

            internal bool didTextboxChange(TextBox textBox)
            {
                string attrib = allControlAttribs[textBox];

                string attribVal = "";
                itemViewTab.getCurrItem().getAttribAsStr(attrib, ref attribVal);

                if (attribVal.CompareTo(textBox.Text) != 0)
                {
                    return true;
                }
                return false;
            }

            internal bool checkTypeOkay(TextBox textBox)
            {
                string attrib = allControlAttribs[textBox];
                // If not right type, return
                string type = colDataTypes[attrib];

                if (Util.checkTypeOkay(textBox.Text, type))
                {
                    return true;
                }
                return false;
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


        // Search Button in Search Tab
        private void searchButton_Click(object sender, EventArgs e)
        {
            tabControl.search();
        }


        // Purchased Lot listbox double click
        private void purcListBox_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.PurchaseListBox.IndexFromPoint(e.Location);
            ResultItem item = tabControl.getCurrPurcItemsAt(index);
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
        private void itemUpdateButton_Click(object sender, EventArgs e)
        {
            tabControl.itemViewUpdate();
        }


        // Change TextBox color if changed
        private void TextBoxAttribute_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // User entered wrong information type
            if (!tabControl.checkTypeOkay(textBox))
            {
                textBox.Text = "";
            }

            if (tabControl.didTextboxChange(textBox))
            {
                textBox.BackColor = Color.LightGray;
            }
        }


        // ItemView View/Edit Button
        private void itemEditButton_Click(object sender, EventArgs e)
        {
            tabControl.flipIVEditMode();
        }


        // ItemView Delete Shipping Info
        private void deleteShipInfoButton_Click(object sender, EventArgs e)
        {
            tabControl.IVdeleteShippingInfo();
        }


        // PurchasedLot View/Edit Button
        private void editPurcButton_Click(object sender, EventArgs e)
        {
            tabControl.PLflipEditMode();
        }


        // PurchasedLot update item
        private void updatePurcButton_Click(object sender, EventArgs e)
        {
            tabControl.purchasedLotUpdate();
        }


        // Add sale to purchase
        private void addItemButton_Click(object sender, EventArgs e)
        {
            tabControl.PLaddItem();
        }


        // New Purchase
        private void newPurcButton_Click(object sender, EventArgs e)
        {
            tabControl.PLnewPurchase();
        }


        // SaleTab View/Edit button
        private void saleEditSaleButton_Click(object sender, EventArgs e)
        {
            tabControl.saleTflipEditMode();
        }


        // SaleTab select sale
        private void saleListBox_DoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.saleListBox.IndexFromPoint(e.Location);
            tabControl.setCurrSale(index);
            tabCollection.SelectTab(tabControl.saleTabNum);
        }


        // SaleTab Update button
        private void saleUpdateButton_Click(object sender, EventArgs e)
        {
            tabControl.saleTabUpdate();
        }


        // SaleTab Add Sale
        private void addSaleButton_Click(object sender, EventArgs e)
        {
            tabControl.saleTaddSale();
        }


        // SaleTab Delete Button
        private void saleDeleteButton_Click(object sender, EventArgs e)
        {
            tabControl.deleteCurrSale();
        }

        // Item View Delete Item Button
        private void deleteItemButton_Click(object sender, EventArgs e)
        {
            tabControl.deleteItemFromDtb();
        }


        // Item View Click Search Result
        private void itemSearchView_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            int currIndex = this.itemSearchView.getRowNum(mea.Y);
            tabControl.setCurrItem(currIndex);
            tabCollection.SelectTab(tabControl.itemViewTabNum);

        }


        // Select Image To View
        private void allPictureViewer_Click(object sender, EventArgs e)
        {
            MouseEventArgs mea = (MouseEventArgs)e;
            int currIndex = this.allPictureViewer.getRowNum(mea.Y);
            tabControl.setMainImage(currIndex);
        }


        // Add Image(s)
        private void addImageButton_Click(object sender, EventArgs e)
        {
            tabControl.insertImage();
            //tabControl.updateCurrItem();
        }


        // Set Thumbnail
        private void setThumbnailButton_Click(object sender, EventArgs e)
        {
            tabControl.setThumbnail();
        }
    }
}
