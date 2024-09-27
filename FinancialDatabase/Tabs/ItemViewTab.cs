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
    public ItemViewTab(Form1 Form1) : base(Form1)
    {
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

    public void editUpdate()
    {
        // Triggers escape of edit mode into view mode if true. Will otherwise cause to stay in edit mode
        bool goodEdit = true;
        if (Form1.currItem == null) { return; }
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
                    string type = Form1.colDataTypes[attrib];
                    query = QB.buildUpdateQuery(Form1.currItem, attrib, type, ttlWeight.ToString());
                    output = PyConnector.runStatement(query);
                    // These must be cleared manually since they are both used at the same time.
                    // Clearing one produces an error when the other textbox is then used to get the total weight
                    Util.clearTBox(Form1.textBox6);
                    Util.clearTBox(Form1.textBox7);
                    skipElem = true;

                }
                else
                {
                    string attrib = t.Text;
                    string type = Form1.colDataTypes[controlBoxAttrib[c]];
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        goodEdit = false;
                        continue;
                    }
                    switch (c)
                    {
                        case TextBox:
                            query = QB.buildUpdateQuery(Form1.currItem, controlBoxAttrib[c], type, t.Text);
                            break;
                        case DateTimePicker:
                            query = QB.buildUpdateQuery(Form1.currItem, controlBoxAttrib[c], type, new Date(c));
                            break;
                    }
                    output = PyConnector.runStatement(query);
                }
                // Update the item in the view
                if (output.CompareTo("ERROR") != 0)
                {
                    updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID())); // Will also reset currItem with new search for it
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

                        query = QB.buildShipInfoInsertQuery(Form1.currItem, weightLbs, weightOz, l, w, h);

                        int lastrowid = -1;
                        string output = PyConnector.runStatement(query, ref lastrowid);

                        string attrib = "item.ShippingID";
                        string type = Form1.colDataTypes[attrib];
                        int shippingID = lastrowid;
                        query = QB.buildUpdateQuery(Form1.currItem, attrib, type, shippingID.ToString());

                        // Update the item table with the new shipping info
                        output = PyConnector.runStatement(query);
                        if (output.CompareTo("ERROR") != 0)
                        {
                            goodEdit = false;
                            Util.clearTBox(weightTBoxes);
                        }
                        updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID())); // Will also reset currItem with new search for it
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



            // OLD CODE
            /*
            string s = controlBoxAttrib[c!];
            List<int> WeightLbsOz = ozToOzLbs(currItem.get_Weight());
            
            //Hardcode weight as it is the only case where a comb. of 2 fields must be combined into 1 value
            if (s.CompareTo("shipping.WeightLbs") == 0)
            {
                type = Form1.colDataTypes["shipping.WeightLbs"];
                TextBox t_Lbs = Form1.textBox6;
                TextBox t_oz  = Form1.textBox7;
                Label   l_oz  = Form1.label23;

                // Get lbs
                // Type check to make sure proper numbers are  given
                int throwaway;
                if (!Int32.TryParse(t_Lbs.Text, out throwaway)) {query = "ERROR: BAD USER INPUT"; }
                int lbs = Int32.Parse(t_Lbs.Text);

                // Get oz
                int oz = 0;
                if (!Int32.TryParse(t_oz.Text, out oz)) { oz = WeightLbsOz[1]; }

                int weight = 16 * lbs + oz;

                query = QB.buildUpdateQuery(currItem, "shipping.Weight", type, weight.ToString());
                
            }
            else if (s.CompareTo("shipping.WeightOz") == 0)
            {
                type = Form1.colDataTypes["shipping.WeightOz"];
                // Convert weight lbs to oz
                TextBox t_Lbs = Form1.textBox6;
                TextBox t_oz  = Form1.textBox7;
                Label   l_Lbs = Form1.label22;

                // Get lbs
                int lbs = 0;
                if (!Int32.TryParse(t_Lbs.Text, out lbs)) { lbs = WeightLbsOz[0]; }

                // Get ounces
                // Type theck to make sure proper numbers are  given
                int throwaway;
                if (!Int32.TryParse(t_oz.Text, out throwaway)) {query = "ERROR: BAD USER INPUT"; }
                int oz = Int32.Parse(t_oz.Text);
                
                int weight = 16 * lbs + oz;

                query = QB.buildUpdateQuery(currItem, "shipping.Weight", type, weight.ToString());
                
            }
            // ! denotes to the compiler that c will not be null
            else if(c!.GetType() == typeof(TextBox))                
            {
                type = Form1.colDataTypes[controlBoxAttrib[c]];
                
                query = QB.buildUpdateQuery(currItem, controlBoxAttrib[t], type, t.Text);
                t.Clear();
                t.BackColor = Color.White;
            }
            else if (c.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker dt = c as DateTimePicker ?? new DateTimePicker();
                type = Form1.colDataTypes[controlBoxAttrib[c]];
                query = QB.buildUpdateQuery(currItem, controlBoxAttrib[c], type, new Date(dt));
                dt.Value = dt.MinDate; // Set as default value to show it has been "cleared" if new date does not show
            }

            // Clear shipping textboxes
            Form1.textBox6.Clear();
            Form1.textBox6.BackColor = Color.White;
            Form1.textBox7.Clear();
            Form1.textBox7.BackColor = Color.White;

            // Got bad input from user, QB could not create a query
            if (query.CompareTo("ERROR: BAD USER INPUT") == 0) 
            {
                continue ;
            }

            int lastrowid = -1;
            List<string> colNames = new List<string>(new string[] { "" });
            string output = PyConnector.runStatement(query, ref colNames, ref lastrowid);*/

        }

        if (goodEdit) inEditingState = false;
        updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID()));
        showItem(Form1.currItem);

    }
    public void deleteShippingInfo()
    {
        // Delete shipping info entry
        string query = QB.buildDelShipInfoQuery(Form1.currItem);
        string output = PyConnector.runStatement(query);

        // Remove foreign key reference to shipping info from item table
        string attrib = "item.ShippingID";
        string type = Form1.colDataTypes[attrib];
        query = QB.buildUpdateQuery(Form1.currItem, attrib, type, null);
        output = PyConnector.runStatement(query);

        if (output.CompareTo("ERROR") != 0)
        {
            updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID()));
        }
        flipEditMode();

    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState && Form1.currItem == null) { return; }

        inEditingState = !inEditingState;
        showControlVisibility();

    }


public void updateItemView(ResultItem item)
    {
        showItem(item);
        Form1.tabControl1.SelectTab(1);
    }

    

    
}
