using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;
using Date = Util.Date;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Diagnostics.Eventing.Reader;
using System.Transactions;
using System.ComponentModel;
using Button = System.Windows.Forms.Button;

public class ItemViewTab : Tab
{
    protected List<TextBox> weightTBoxes;
    public ItemViewTab(Form1.TabController tabController, Form1 Form1) : base(Form1)
    {
        this.tabController = tabController;
        updateButton = Form1.button1;
        editButton   = Form1.button4;
        generateTBoxGroups();
        Util.clearLabelText(clearableAttribLables);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        clearableAttribLables = new List<Label>() {
            Form1.label40,
            Form1.label17,
            Form1.label18,
            Form1.label19,
            Form1.label20,
            Form1.label21,
            Form1.label22,
            Form1.label23,
            Form1.label24,
            Form1.label25,
            Form1.label26,
            Form1.label43
        };
        editableAttribLables = new List<Label>(){
            Form1.label40,
            Form1.label19,
            Form1.label20,
            Form1.label22,
            Form1.label23,
            Form1.label24,
            Form1.label25,
            Form1.label26,
        };

        itemTBoxes = new List<TextBox>()
        {
            Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5
        };
        weightTBoxes = new List<TextBox>()
        {
            Form1.textBox6,
            Form1.textBox7
        };
        shippingTBoxes = new List<TextBox>()
        {
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10
        };
        editingAttributeControls = new List<Control>(){
            Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5,
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10

        };

        editingControls = new List<Control>(){
            Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5,
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10,
            Form1.button5
        };

        labelTextboxPairs = new Dictionary<Control, Label>();

        int i = 0;
        foreach (Control c in editingAttributeControls)
        {
            if (c is not Button)
            {
                labelTextboxPairs[c] = editableAttribLables[i++];
            }
        }

        controlBoxAttrib = new Dictionary<Control, string>
        {{ Form1.textBox3,  "item.Name" },
        { Form1.textBox4,  "item.InitialQuantity" },
        { Form1.textBox5,  "item.CurrentQuantity" },
        { Form1.textBox6,  "shipping.WeightLbs" },
        { Form1.textBox7, "shipping.WeightOz" },
        { Form1.textBox8, "shipping.Length" },
        { Form1.textBox9, "shipping.Width" },
        { Form1.textBox10, "shipping.Height" }};
    }


    public bool allShippingBoxesFilled()
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

    // Take user input, and use it to update the currItem
    // UpdateCurrItemWithUserInput
    public void UpdateCurrItemWithUserInput()
    {
        // Triggers escape of edit mode into view mode if true. Will otherwise cause to stay in edit mode
        bool goodEdit = true;
        if (tabController.getCurrItem() == null) { return; }
        List<Control> changedFields = getChangedFields();

        // Skip the current (what was previously the "next") element in the loop
        // Used for skipping ounces after it is cleared from being "used" in conjugation with lbs textbox
        bool skipElem = false;
        foreach (Control c in changedFields)
        {
            if (skipElem)
            {
                skipElem = false;
                continue;
            }
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment

            string query = "";
            if (tableEntryExists(t))
            {
                string output = "";
                if (weightTBoxes.Contains(t))
                {
                    // Get info for weight
                    int lbs = 0;
                    int oz = 0;
                    if (!Int32.TryParse(Form1.textBox6.Text, out lbs)
                     || !Int32.TryParse(Form1.textBox7.Text, out oz))
                    {
                        goodEdit = false;
                        Util.clearTBox(Form1.textBox6);
                        Util.clearTBox(Form1.textBox7);
                        continue;
                    }
                    int ttlWeight = lbs * 16 + oz;

                    // Execute query
                    string attrib = "shipping.Weight";
                    string type = tabController.colDataTypes[attrib];
                    query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), attrib, type, ttlWeight.ToString());
                    output = DatabaseConnector.runStatement(query);
                    // These must be cleared manually since they are both used at the same time.
                    // Clearing one produces an error when the other textbox is then used to get the total weight
                    Util.clearTBox(Form1.textBox6);
                    Util.clearTBox(Form1.textBox7);
                    skipElem = true;

                }
                else
                {
                    string attrib = t.Text;
                    string type = tabController.colDataTypes[controlBoxAttrib[c]];
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        goodEdit = false;
                        continue;
                    }
                    switch (c)
                    {
                        case TextBox:
                            query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), controlBoxAttrib[c], type, t.Text);
                            break;
                        case DateTimePicker:
                            query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), controlBoxAttrib[c], type, new Date(c));
                            break;
                    }
                    output = DatabaseConnector.runStatement(query);
                }
                // Update the item in the view
                if (output.CompareTo("ERROR") != 0)
                {
                    showItem(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
                    Util.clearTBox(t);
                }

            }
            else if (!tableEntryExists(t))
            {
                if (shippingTBoxes.Contains(t))
                {
                    if (allShippingBoxesFilled())
                    {
                        int weightLbs = 0;
                        int weightOz = 0;
                        int l = 0;
                        int w = 0;
                        int h = 0;
                        try
                        {
                            weightLbs = Int32.Parse(Form1.textBox6.Text);
                            weightOz = Int32.Parse(Form1.textBox7.Text);
                            l = Int32.Parse(Form1.textBox8.Text);
                            w = Int32.Parse(Form1.textBox9.Text);
                            h = Int32.Parse(Form1.textBox10.Text);
                        }
                        catch
                        {
                            goodEdit = false;
                            continue;
                        }

                        //TODO: Why is this not happening all in DatabaesConnector?
                        query = QueryBuilder.buildShipInfoInsertQuery(tabController.getCurrItem(), weightLbs, weightOz, l, w, h);

                        int lastrowid;
                        string output = DatabaseConnector.runStatement(query, out lastrowid);

                        string attrib = "item.ShippingID";
                        string type = tabController.colDataTypes[attrib];
                        int shippingID = lastrowid;
                        query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), attrib, type, shippingID.ToString());

                        // Update the item table with the new shipping info
                        output = DatabaseConnector.runStatement(query);
                        if (output.CompareTo("ERROR") != 0)
                        {
                            goodEdit = false;
                            Util.clearTBox(weightTBoxes);
                        }
                        showItem(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
                    }
                    else
                    {
                        MessageBox.Show(
                            "To Add Shipping Info, all fields must be filled (Lbs, Oz, L, W, H)",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                            );
                        goodEdit = false;
                        break;
                    }
                }
                else if (itemTBoxes.Contains(t))
                {
                    goodEdit = false;
                    Console.WriteLine("ERROR: no item entry for CurrItem, This should not be possible");
                    continue;
                }
            }
        }

        if (goodEdit) inEditingState = false;

        // It is correct to run showItem, updateCurrItem, and viewMode
        // from inside this function since you will always need to
        // update the shown informationafter updating the
        // ResultItem copy of it
        tabController.updateCurrItem();
        showItem(tabController.getCurrItem());
        viewMode();
    }


    public void deleteShippingInfo()
    {
        // Delete shipping info entry
        string query = QueryBuilder.buildDelShipInfoQuery(tabController.getCurrItem());
        string output = DatabaseConnector.runStatement(query);

        // Remove foreign key reference to shipping info from item table
        string attrib = "item.ShippingID";
        string type = tabController.colDataTypes[attrib];
        query = QueryBuilder.buildUpdateQuery(tabController.getCurrItem(), attrib, type, null);
        output = DatabaseConnector.runStatement(query);

        if (output.CompareTo("ERROR") != 0)
        {
            showItem(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
        }
        flipEditMode();
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState && tabController.getCurrItem() == null) { return; }

        inEditingState = !inEditingState;
        showControlVisibility();

    }

    public override void showItem(ResultItem item)
    { 
        Util.clearLabelText(clearableAttribLables);

        if (item.hasItemEntry())
        {
            Form1.label40.Text = checkDefault(item.get_Name());
            Form1.label19.Text = checkDefault(item.get_InitialQuantity());
            Form1.label20.Text = checkDefault(item.get_CurrentQuantity());
            Form1.label21.Text = checkDefault(item.get_ITEM_ID());
            Form1.label51.Text = checkDefault(item.get_Name());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.label43.Text = datePurc.toDateString();
            Form1.label17.Text = checkDefault(item.get_Amount_purchase());
        }

        if (item.hasSaleEntry())
        {
            Form1.label18.Text = item.getTotalSales().ToString();
        }

        if (item.hasShippingEntry())
        {
            List<int> WeightLbsOz = Util.ozToOzLbs(item.get_Weight());
            Form1.label22.Text = checkDefault(WeightLbsOz[0]);
            Form1.label23.Text = checkDefault(WeightLbsOz[1]);
            Form1.label24.Text = checkDefault(item.get_Length());
            Form1.label25.Text = checkDefault(item.get_Width());
            Form1.label26.Text = checkDefault(item.get_Height());
        }
        updateUserInputDefaultText();
    }
    
}
