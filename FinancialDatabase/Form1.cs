using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace FinancialDatabase
{

    public partial class Form1 : Form
    {
        public List<ResultItem> currentItems; // list of [sale name, ITEM_ID]
        public List<ResultItem> currentPurchaseItems; // list of [sale name, ITEM_ID]
        public List<Sale> currentItemSales;
        public Dictionary<string, string> colDataTypes;
        // Each tab has it's own object
        public SaleTab saleT;
        public SearchTab ST;
        public ItemViewTab IV;
        public PurchasedLotTab PL;
        QueryBuilder QB;
        DatabaseConnector PyConnector;
        public ResultItem currItem;
        public Sale currSale;


        public Form1()
        {
            InitializeComponent();
            currentItems = new List<ResultItem>();
            currentPurchaseItems = new List<ResultItem>();
            currentItemSales = new List<Sale>();
            QB = new QueryBuilder();
            saleT = new SaleTab(this);
            ST = new SearchTab(this);
            IV = new ItemViewTab(this);
            PL = new PurchasedLotTab(this);

            PyConnector = new DatabaseConnector();

            colDataTypes = PyConnector.getColDataTypes();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Make default selection "Item"
            comboBox1.SelectedIndex = 0;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            currentItems.Clear();


            // Activate Python Script 'DtbConnAndQuery.py' as process 'p'
            string query = this.textBox1.Text;
            if (query == "")
            {
                query = QB.defaultQuery();
            }

            List<ResultItem> result = PyConnector.RunItemSearchQuery(query);

            foreach (ResultItem item in result)
            {
                this.currentItems.Add(item);
                this.listBox1.Items.Add(item.get_Name());
            }
        }

        // Search Button in Search Tab
        private void button2_Click(object sender, EventArgs e)
        {
            ST.search();
        }



        // Search listbox double click
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            int item_id = currentItems[index].get_ITEM_ID();

            ResultItem item = PyConnector.getItem(item_id);
            saleT.updateItemView(item);
            PL.updateItemView(item);
            
            IV.updateItemView(item);
        }

        // Purchased Lot listbox double click
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox2.IndexFromPoint(e.Location);
            int item_id = currentPurchaseItems[index].get_ITEM_ID();

            ResultItem item = PyConnector.getItem(item_id);
            PL.updateItemView(item);
            IV.updateItemView(item);
            saleT.updateItemView(item);
        }

        // Enter key pressed for search
        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ST.search();
            }
        }

        // Update Item in ItemTab
        private void button1_Click_1(object sender, EventArgs e)
        {
            IV.editUpdate();
            IV.showControlVisibility();

        }

        private void TextBoxAttribute_Leave(object sender, EventArgs e)
        {
            if (sender is null) return;

#pragma warning disable CS8600 // Checked if sender is null

            TextBox t = sender as TextBox;

#pragma warning disable CS8604 // Checked if sender is null
            string attrib = "";
            if (IV.controlBoxAttrib.ContainsKey(t))
            {
                attrib = IV.controlBoxAttrib[t];
            }
            else if (PL.controlBoxAttrib.ContainsKey(t))
            {
                attrib = PL.controlBoxAttrib[t];
            }
            else
            {
                return;
            }

            string type = colDataTypes[attrib];

            // If not right type, return
            if (!Util.checkTypeOkay(t.Text, type))
            {
                t.Text = "";
                return;
            }

            string ret = "";
            currItem.getAttribAsStr(attrib, ref ret);
            if (ret.CompareTo(t.Text) != 0)
            {
                t.BackColor = Color.LightGray;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            IV.flipEditMode();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IV.deleteShippingInfo();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PL.flipEditMode();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PL.editUpdate();
            PL.flipEditMode();
        }

        // Add sale to purchase
        private void button2_Click_1(object sender, EventArgs e)
        {
            PL.addItem();
        }

        // New Purchase
        private void button3_Click(object sender, EventArgs e)
        {
            PL.newPurchase();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            saleT.flipEditMode();
        }

        // SaleTab select sale
        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox3.IndexFromPoint(e.Location);

            // Bad mouse click
            if (index == -1) { return; }
            int sale_id = currentItemSales[index].get_SALE_ID();

            Sale sale = PyConnector.getSale(sale_id);
            saleT.updateSale(sale);
        }

        // Update SaleTab Sale info
        private void button10_Click(object sender, EventArgs e)
        {
            saleT.editUpdate();
            saleT.viewMode();
        }

        // SaleTab Add Sale
        private void button8_Click(object sender, EventArgs e)
        {
            saleT.addItem();
        }
    }
}
