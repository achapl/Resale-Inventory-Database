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
        public List<ResultItem> currentItems; // list of [item name, ITEM_ID]
        public List<ResultItem> currentPurchaseItems; // list of [item name, ITEM_ID]
        public Dictionary<string, string> colDataTypes;
        // Each tab has it's own object
        public SearchTab ST;
        public ItemViewTab IV;
        public PurchasedLotTab PL;
        QueryBuilder QB;
        CtrlerOfPythonToDTBConnector PyConnector;
        public ResultItem currItem;


        public Form1()
        {
            InitializeComponent();
            currentItems = new List<ResultItem>();
            currentPurchaseItems = new List<ResultItem>();
            QB = new QueryBuilder();
            ST = new SearchTab(this);
            IV = new ItemViewTab(this);
            PL = new PurchasedLotTab(this);
            PyConnector = new CtrlerOfPythonToDTBConnector();

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
            PL.updateItemView(item);
            IV.updateItemView(item);
        }

        // Purchased Lot listbox double click
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox2.IndexFromPoint(e.Location);
            int item_id = currentPurchaseItems[index].get_ITEM_ID();
            
            PL.updateItemView(PyConnector.getItem(item_id));
            IV.updateItemView(PyConnector.getItem(item_id));
        }

        // Link to Purchased Lot
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PL.update(currItem);
        }

        // Enter key pressed for search
        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ST.search();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            IV.editUpdate();
            IV.flipEditState();

        }

        private void TextBoxAttribute_Leave(object sender, EventArgs e)
        {
            if (sender is null) return;
#pragma warning disable CS8600 // Checked if sender is null
            TextBox t = sender as TextBox;
#pragma warning disable CS8604 // Checked if sender is null
            if (!IV.controlBoxAttrib.ContainsKey(t)) return;
            string attrib = IV.controlBoxAttrib[t];
            string type = colDataTypes[attrib];

            if (!Util.checkTypeOkay(t.Text, type))
            {
                t.Text = "";
            }
            else
            {
                t.BackColor = Color.LightGray;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IV.flipEditState();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IV.deleteShippingInfo();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PL.flipEditState();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PL.editUpdate();
            PL.flipEditState();
        }
    }
}
