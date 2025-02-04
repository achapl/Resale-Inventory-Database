using FinancialDatabase.Tabs;
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Database.closeConnection();
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
            Item item = tabControl.getCurrPurcItemsAt(index);
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
            tabControl.deleteCurrItem();
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
            //tabControl.updateCurrItemUsingDtb();
        }


        // Set Thumbnail
        private void setThumbnailButton_Click(object sender, EventArgs e)
        {
            tabControl.setThumbnail();
        }

        public DialogResult showOpenFileDialog()
        {
            return openFileDialog1.ShowDialog();
        }

        public List<string> getOpenFileDialogNames()
        {
            return new List<string>(openFileDialog1.FileNames);
        }
    }
}
