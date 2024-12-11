using FinancialDatabase.DatabaseObjects;
using System;
namespace FinancialDatabase
{
    public class TabController
    {
        // Used when tabs are required to work in harmony and
        // send information between each other
        //
        // Ex: updating currItem in the search tab means one
        // must change the item view tab, and the
        // purchased lot tab, .etc
        Form1 Form1;

        // Data
        public Dictionary<string, string> colDataTypes;
        private Dictionary<Control, string> allControlAttribs;



        // Tab references
        PurchasedLotTab purchasedLotTab;
        ItemViewTab itemViewTab;
        SearchTab searchTab;
        SaleTab saleTab;


        public int searchTabNum = 0;
        public int itemViewTabNum = 1;
        public int purcLotTabNum = 2;
        public int saleTabNum = 3;

        public TabController(Form1 Form1)
        {
            this.Form1 = Form1;
            colDataTypes = Database.getColDataTypes();


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


        // currPurc
        public Purchase getCurrPurc() => purchasedLotTab.getCurrPurc();

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

            purchasedLotTab.setCurrPurcAndShowItems(newItem);
            itemViewTab.showItemAttributes(newItem);
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

            ResultItem newItem = Database.getItem(shellItem.get_ITEM_ID());
            newItem.set_images();
            setCurrItem(newItem);

        }

        public void clearCurrSaleItems()
        {
            saleTab.clearCurrItemSales();
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
            purchasedLotTab.addItemToPurc();
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
                Form1.tabCollection.SelectTab(saleTabNum);
            }
        }


        private void clearAllNonSearchTabs()
        {
            itemViewTab.setCurrItem(null);
            clearCurrSaleItems();
            clearCurrPurcItems();

            purchasedLotTab.clearCurrItemControls();
            itemViewTab.clearCurrItemControls();
            saleTab.clearCurrItemControls();
        
        }


        public void deleteCurrItem()
        {
            bool deletedItem = itemViewTab.deleteItem();
            if (!deletedItem) { return; }

            clearAllNonSearchTabs();
            Form1.tabCollection.SelectTab(searchTabNum);
        }


        public void setMainImage(int currIndex)
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
            DialogResult success = Form1.showOpenFileDialog();
            if (success == DialogResult.OK)
            {
                List<string> files = Form1.getOpenFileDialogNames();
                foreach (string file in files)
                {
                    Database.insertImage(file, itemViewTab.getCurrItem().get_ITEM_ID());
                }
            }
        }


        public void setThumbnail()
        {
            itemViewTab.setThumbnail();
        }

        public bool isNewPurchase()
        {
            return purchasedLotTab.isNewPurchase;
        }

        public bool didTextboxChange(TextBox textBox)
        {
            string attrib = allControlAttribs[textBox];

            string attribVal = getCurrItem().getAttribVal(attrib);

            if (attribVal.CompareTo(textBox.Text) != 0)
            {
                return true;
            }
            return false;
        }

        public bool checkTypeOkay(TextBox textBox)
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
}