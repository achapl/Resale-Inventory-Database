using FinancialDatabase.DatabaseObjects;
using System;
using System.Security.Cryptography;
namespace FinancialDatabase.Tabs
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


        }


        private void clearAllNonSearchTabsAndVars()
        {
            saleTab.setCurrSale(null);
            clearCurrSaleItems();
            clearCurrPurcItems();
            clearCurrItem();
        }

        private void clearCurrItem()
        {
            itemViewTab.setCurrItemAndShowView(null);
            itemViewTab.clearCurrItemControls();
        }



        // Search Tab
        public void setSearchItems(List<Item> items) => searchTab.setSearchItems(items);
        
        public Item getSearchItemsAt(int index) => getSearchItems()[index];
        
        public List<Item> getSearchItems() => searchTab.getSearchItems();
       
        public void clearSearchItems()
        {
            Form1.itemSearchView.clearItems();
            searchTab.clearCurrItemsVar();
        }

        public void search()
        {
            searchTab.search();
            Form1.itemSearchView.updatePaint();
        }

        public void deleteCurrItem()
        {
            bool deletedItem = itemViewTab.deleteItem();
            if (!deletedItem) { return; }

            saleTab.setCurrSale(null);
            clearCurrSaleItems();

            if (getCurrPurcItems().Count == 1)
            {
                clearCurrPurcItems();
            } else
            {
                purchasedLotTab.setCurrPurcAndShowView(purchasedLotTab.getCurrPurc().PURCHASE_ID);

            }
            clearCurrItem();
            search();
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
                    Database.insertImage(file, itemViewTab.getCurrItem());
                }
            }
            itemViewTab.showItemPictures(getCurrItem());
        }

        public void setThumbnail()
        {
            itemViewTab.setThumbnail();
        }






        // ItemViewTab
        public Item getCurrItem() => itemViewTab.getCurrItem();

        public bool getItemInEditingState() => itemViewTab.inEditingState;

        // Update the program (the model of the database) with a new resultItem, not just the backend variable currItem
        // Will work with null val for newItem
        public void setCurrItem(Item newItem)
        {
            if (newItem == null)
            {
                // TODO: Does this clear the curr Purc and not reset the view of it?
                clearAllNonSearchTabsAndVars();
                itemViewTab.setCurrItemAndShowView(newItem);
                return;
            }

            // New item may be coming from search result as a shell item. Need to update it into a full item
            newItem = Database.getItem(newItem.ITEM_ID);


            // Note: Order matters: Must set currPurc before item. Must also set sale after item.
            purchasedLotTab.setCurrPurcAndShowView(newItem.PurchaseID);    
            itemViewTab.setCurrItemAndShowView(newItem);
            saleTab.showItemAttributes(newItem);

            Form1.tabCollection.SelectTab(itemViewTabNum);
        }

        // Update the curr item given its position in the search results
        public void setCurrItem(int index)
        {
            if (index > Form1.itemSearchView.countItems())
            {
                throw new Exception("Index of the search results to set the new currItem to in Form1.TabController setCurrItem() is greater than the number of items in the search result");
            }
            Item shellItem = getSearchItemsAt(index);

            Item newItem = Database.getItem(shellItem.ITEM_ID);
            setCurrItem(newItem);

        }

        public void itemViewUpdateFromUserInput()
        {
            itemViewTab.updateFromUserInput();
        }

        public void itemViewFlipEditMode()
        {
            itemViewTab.flipEditMode();
        }

        public void itemViewDeleteShipInfo()
        {
            itemViewTab.deleteShippingInfo();
        }
        
        public void clearCurrItemControls()
        {
            itemViewTab.clearCurrItemControls();
            saleTab.clearCurrItemSales();
        }

        public Item getCurrItemWithImages()
        {
            Item item = getCurrItem();
            if (item != null)
            {
                item.set_imagesFromDatabase();
            }
            return item;
        }

        public void itemViewShowUpdate()
        {
            itemViewTab.showItemAttributes(getCurrItemWithImages());
        }





        // Purchase Tab
        public Purchase getCurrPurc() => purchasedLotTab.getCurrPurc();

        public Item getCurrPurcItemsAt(int index) => purchasedLotTab.getCurrPurcItemsAt(index);
        
        public List<Item> getCurrPurcItems() => purchasedLotTab.getCurrPurcItems();
        
        public void clearCurrPurcItems() => purchasedLotTab.clearCurrPurcItems();

        public void purcItemsflipEditMode()
        {
            purchasedLotTab.flipEditMode();
        }

        public void purcItemsAddItem()
        {
            purchasedLotTab.addUserInputItemToCurrPurc();
        }

        public void purcItemsNewPurchase()
        {
            bool success = purchasedLotTab.createNewPurchase(out int newItemID);
            if (success)
            {
                itemViewTab.clearCurrItem();
                itemViewTab.setCurrItemAndShowView(new Item(newItemID));
            }
        }

        public void purcItemsUpdateFromUserInput()
        {
            purchasedLotTab.updateFromUserInput();
            Form1.tabCollection.SelectTab(purcLotTabNum);
        }
        
        public bool getPLInEditingState() => purchasedLotTab.inEditingState;





        // Sale Tab
        public Sale getCurrSale() => saleTab.getCurrSale();

        internal void addCurrentItemSales(Sale newSales) => saleTab.addSale(newSales);
        
        public Sale getCurrSaleAt(int index) => saleTab.getCurrItemSales(index);
       
        public List<Sale> getCurrentItemSales() => saleTab.getCurrItemSales();
        
        public void setCurrSale(int index) => saleTab.setCurrSale(index);

        public bool saleTabGetInEditingState() => saleTab.inEditingState;

        public void saleTabUpdate()
        {
            bool success = saleTab.updateFromUserInput();
            if (success)
            {
                itemViewTab.showItemAttributes(getCurrItemWithImages());
            }
        }

        private void clearCurrSaleItems()
        {
            saleTab.clearCurrItemSales();
        }
        
        public void saleTabClearControls()
        {
            saleTab.clearAttribs();
        }

        public void saleTabflipEditMode()
        {
            saleTab.flipEditMode();
        }

        public void saleTabEditMode()
        {
            if (!saleTab.inEditingState)
            {
                saleTab.flipEditMode();
            }
        }

        public void saleTabViewMode()
        {
            if (saleTab.inEditingState)
            {
                saleTab.flipEditMode();
            }
        }

        public void saleTabAddSale()
        {
            saleTab.addSale();
            itemViewTab.showItemAttributes(getCurrItemWithImages());
        }
        
        public void deleteCurrSale()
        {
            bool success = saleTab.deleteCurrSale();
            if (success)
            {
                saleTab.showItemAttributes(itemViewTab.getCurrItem());
                itemViewTab.updateCurrItemUsingDtb();
                Form1.tabCollection.SelectTab(saleTabNum);
            }
        }

        public void saleTabShowUpdate()
        {
            saleTab.showItemAttributes(getCurrItem());
        }




        

        // FOR TESTING
        public bool didTextboxChange(TextBoxLabelPair TLP)
        {
            string attrib = TLP.attrib;

            string attribVal = getCurrItem().getAttribAsStr(attrib);

            if (attribVal is null ||
                attribVal.CompareTo(Util.DEFAULT_DATE.toDateString()) == 0 ||
                attribVal.CompareTo(Util.DEFAULT_DOUBLE.ToString()) == 0 ||
                attribVal.CompareTo(Util.DEFAULT_INT.ToString()) == 0)
            {
                return TLP.getControlValueAsStr().CompareTo("") != 0;
            }

            return attribVal.CompareTo(TLP.getControlValueAsStr()) != 0;
        }


        public bool checkTypeOkay(TextBoxLabelPair textBox)
        {
            string attrib = textBox.attrib;
            // If not right type, return
            string type = colDataTypes[attrib];

            if (Util.checkTypeOkay(textBox.getControlValueAsStr(), type))
            {
                return true;
            }
            return false;
        }

        internal void deleteCurrImage()
        {
            itemViewTab.deleteCurrImage();
            itemViewTab.showItemPictures(getCurrItem());
        }
    }
}