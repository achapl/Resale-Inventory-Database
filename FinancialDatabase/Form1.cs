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

            }

            // currItem
            public void setItemViewCurrItem(ResultItem newItem) => itemViewTab.setCurrItem(newItem);
            public ResultItem getCurrItem() => itemViewTab.getCurrItem();
            

            // currItems
            public void setCurrItems(List<ResultItem> newItems) => searchTab.addCurrItems(newItems);
            public ResultItem getCurrItemsAt(int index) => itemViewTab.getCurrItemsAt(index);
            public void addCurrItems(ResultItem newItem) => searchTab.addCurrItems(newItem);
            public List<ResultItem> getCurrItems() => searchTab.getCurrItems();
            public void clearCurrItems() => searchTab.clearCurrItems();


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
                setItemViewCurrItem(newItem);

                if (purchasedLotTab.isNewPurchase)
                {
                    addSearchResultItem(newItem);
                }

                setNewItemItemView(newItem);
                setNewItemPurchasedLots(newItem);
                setNewItemSaleItem(newItem);
                Form1.tabCollection.SelectTab(itemViewTabNum);
            }


            private void clearSearchItems()
            {
                Form1.itemSearchView.clearItems();
                clearCurrItems();
            }


            public void addSearchResultItem(ResultItem newItem)
            {
                Form1.itemSearchView.addRow(newItem.get_Images()[0].image, newItem.get_Name());
                addCurrItems(newItem);
                // TODO: CHECK IF newItem already in list
            }



            // Update the curr item given its position in the search results
            public void setCurrItem(int index)
            {
                if (index > Form1.itemSearchView.countItems())
                {
                    throw new Exception("Index of the search results to set the new currItem to in Form1.TabController setCurrItem() is greater than the number of items in the search result");
                }
                ResultItem indexItem = getCurrItemsAt(index);

                ResultItem newItem = DatabaseConnector.getItem(indexItem.get_ITEM_ID());
                newItem.set_images(DatabaseConnector.getAllImages(newItem));
                setCurrItem(newItem);

            }



            private void setNewItemPurchasedLots(ResultItem newItem)
            {
                purchasedLotTab.updatePurchasedLotView(newItem);
            }

            private void setNewItemItemView(ResultItem newItem)
            {
                itemViewTab.showItemAttributes(newItem);
                itemViewTab.showItemPictures(newItem);
            }

            public void setNewItemSaleItem(ResultItem newItem)
            {
                saleTab.updateSaleViewListBox(newItem);
            }

            public void runManualQuery(string query)
            {
                clearSearchItems();

                List<ResultItem> result = DatabaseConnector.getItems(query, true);

                foreach (ResultItem item in result)
                {
                    addSearchResultItem(item);
                }
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

            public void greyTextBox(TextBox textBox)
            {

                string attrib = "";
                if (itemViewTab.controlAttrib.ContainsKey(textBox))
                {
                    attrib = itemViewTab.controlAttrib[textBox];
                }
                else if (purchasedLotTab.controlAttrib.ContainsKey(textBox))
                {
                    attrib = purchasedLotTab.controlAttrib[textBox];
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
                itemViewTab.getCurrItem().getAttribAsStr(attrib, ref attribVal);
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
                    saleTab.updateSaleViewListBox(itemViewTab.getCurrItem());
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
                clearCurrItems();
                saleTab.clearCurrItemSales();
                clearCurrPurcItems();

                purchasedLotTab.clearCurrItemControls();
                itemViewTab.clearCurrItemControls();
                saleTab.clearCurrItemControls();
                searchTab.clearItems();

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
                int currImageID = itemViewTab.getCurrImageID();

                // Check defualt val. Do nothing
                if (currImageID == -1 || currImageID == null)
                {
                    return;
                }

                int newThumbnailID = DatabaseConnector.getImageThumbnailID(currImageID);
                DatabaseConnector.runStatement("UPDATE item SET ThumbnailID = " + newThumbnailID + " WHERE item.ITEM_ID = " + itemViewTab.getCurrItem().get_ITEM_ID() + ";");
            }

            internal bool isNewPurchase()
            {
                return purchasedLotTab.isNewPurchase;
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


        // ItemViewTab manual query button
        private void manualQueryButton_Click(object sender, EventArgs e)
        {
            tabControl.runManualQuery(manualQueryTBox.Text);
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
            if (sender is null) return;

            #pragma warning disable CS8600 // Checked if sender is null
            TextBox textBox = sender as TextBox;

            #pragma warning disable CS8604 // Checked if sender is null
            tabControl.greyTextBox(textBox);

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
