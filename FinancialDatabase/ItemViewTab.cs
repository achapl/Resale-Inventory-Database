using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;
using Date = Util.Date;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

public class ItemViewTab
{
	Form1 Form1;
    ResultItem currItem;
    QueryBuilder QB;
    CtrlerOfPythonToDTBConnector PyConnector;
    bool inEditingState;
    List<Label> allItemLabels;
    List<Label> labelsForEditbleFields;
    List<TextBox> tBoxesForEditableFields;
    Dictionary<TextBox, Label> editableFieldPairs;

    public ItemViewTab(Form1 Form1)
	{
		this.Form1 = Form1;
        QB = new QueryBuilder();
        PyConnector = new CtrlerOfPythonToDTBConnector();
        currItem = new ResultItem();
        inEditingState = false;


        allItemLabels = new List<Label>() {
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
            Form1.label26
        };
        labelsForEditbleFields = new List<Label>(){
            Form1.label40,
            Form1.label19,
            Form1.label20,
            Form1.label22,
            Form1.label23,
            Form1.label24,
            Form1.label25,
            Form1.label26
        };
        tBoxesForEditableFields = new List<TextBox>(){
            Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5,
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10
        };
        editableFieldPairs = new Dictionary<TextBox, Label>();
        for (int i = 0; i < tBoxesForEditableFields.Count; i++)
        {
            editableFieldPairs[tBoxesForEditableFields[i]] = labelsForEditbleFields[i];
        }

        Util.clearLabelText(allItemLabels);
        updateEditableVisibility();
    }


    public void flipEditState()
    {

        
        inEditingState = !inEditingState;

        if (inEditingState)
        {
            // Now button, when pressed will change it to "viewing" state
            Form1.button4.Text = "View";
            
        }
        else
        {
            Form1.button4.Text = "Edit";
        }

        updateEditableVisibility();
    }

    private void updateEditableVisibility()
    {
        if (inEditingState)
        {
            Form1.button1.Visible = true;
        }
        else
        {
            Form1.button1.Visible = false;
        }

        foreach (Label l in labelsForEditbleFields)
        {
            if (inEditingState)
            {
                l.Visible = false;
            }
            else
            {
                l.Visible = true;
            }
        }
        foreach (TextBox t in tBoxesForEditableFields)
        {
            if (inEditingState)
            {
                t.Visible = true;
                t.Text = editableFieldPairs[t].Text;
            }
            else
            {
                t.Visible = false;
            }
        }
    }

   
    public List<int> ozToOzLbs(int ozs)
    {
        // Default
        if (ozs == -1)
        {
            return new List<int>(){ -1, -1};
        }
        int lbs = ozs / 16;
        int oz = ozs - (16 * lbs);
        return new List<int>(new int[] { lbs, oz });
    }
    
    public string checkDefault(int val)
    {
        if (val == -1) { return ""; }
        else { return val.ToString(); }
    }

    public string checkDefault(double val)
    {
        if (val == -1) { return ""; }
        else { return val.ToString(); }
    }

    // Redundant, but exists for sake of extensibility
    public string checkDefault(string val)
    {
        if (val.CompareTo("") == 0) { return ""; }
        else { return val.ToString(); }
    }
    
    public void showItem(ResultItem item)
	{
        Util.clearLabelText(allItemLabels);

        currItem = item;

        if (item.hasItemEntry())
        {
            Form1.label40.Text = checkDefault(item.get_Name());
            Form1.label19.Text = checkDefault(item.get_InitialQuantity());
            Form1.label20.Text = checkDefault(item.get_CurrentQuantity());
            Form1.label21.Text = checkDefault(item.get_ITEM_ID());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.dateTimePicker3.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.label17.Text = checkDefault(item.get_Amount_purchase());
        }
                
        if (item.hasSaleEntry())
        {
            Form1.label18.Text = checkDefault(item.get_Amount_sale());
        }
        
        if (item.hasShippingEntry())
        {
            List<int> WeightLbsOz = ozToOzLbs(item.get_Weight());
            Form1.label22.Text = checkDefault(WeightLbsOz[0]);
            Form1.label23.Text = checkDefault(WeightLbsOz[1]);
            Form1.label24.Text = checkDefault(item.get_Length());
            Form1.label25.Text = checkDefault(item.get_Width());
            Form1.label26.Text = checkDefault(item.get_Height());
        }
    }

    private List<Control> getChangedFields()
    {
        List<Control> fields = new List<Control>(new Control[] {Form1.dateTimePicker3,
                                                    Form1.textBox3,
                                                    Form1.textBox4,
                                                    Form1.textBox5,
                                                    Form1.textBox6,
                                                    Form1.textBox7,
                                                    Form1.textBox8,
                                                    Form1.textBox9,
                                                    Form1.textBox10});
        List<Control> returnList = new List<Control>();
        foreach (Control f in fields)
        {
            if (f.GetType() == typeof(TextBox) && f.Text.Length > 0)
            {
                string ret = "";
                currItem.getAttribAsString(Form1.controlBoxAttrib[f], ref ret);
                // If text doesn't match currItem for same field add it
                // Ignore any given text that already matches currItem
                if (((TextBox)f).Text.CompareTo(ret) != 0)
                {
                    returnList.Add(f);

                }
            } else if (f.GetType() == typeof(NumericUpDown) && ((NumericUpDown)f).Value != null)
            {
                string ret = "";
                currItem.getAttrib(Form1.controlBoxAttrib[f], ref ret);
                // If text doesn'f match currItem for same field add it
                // Ignore any given text that already matches currItem
                if (((TextBox)f).Text.CompareTo(ret) != 0)
                {
                    returnList.Add(f);

                }
            }

            else if (f.GetType() == typeof(DateTimePicker) && new Date(f).toDateString().CompareTo(currItem.get_Date_Purchased().toDateString()) != 0)
            {
                returnList.Add(f);
            }
        }

        return returnList;

    }

    private void clearTextBoxes()
    {
        List<TextBox> textBoxes = new List<TextBox>() 
        {   Form1.textBox3,
            Form1.textBox4,
            Form1.textBox5,
            Form1.textBox6,
            Form1.textBox6,
            Form1.textBox7,
            Form1.textBox8,
            Form1.textBox9,
            Form1.textBox10};

        foreach (TextBox t in textBoxes)
        {
            t.Clear();
            t.BackColor = Color.White;
        }
    }

    public void editUpdate()
    {
        if (currItem == null) { return; }
        List<Control> changedFields = getChangedFields();
        

        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            string query = "";
            string type = "";
            string s = Form1.controlBoxAttrib[c!];
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
                type = Form1.colDataTypes[Form1.controlBoxAttrib[c]];
                TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment
                query = QB.buildUpdateQuery(currItem, Form1.controlBoxAttrib[t], type, t.Text);
                t.Clear();
                t.BackColor = Color.White;
            }
            else if (c.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker dt = c as DateTimePicker ?? new DateTimePicker();
                type = Form1.colDataTypes[Form1.controlBoxAttrib[c]];
                query = QB.buildUpdateQuery(currItem, Form1.controlBoxAttrib[c], type, new Date(dt));
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
            string output = PyConnector.runStatement(query, ref colNames, ref lastrowid);

        }
        Form1.ST.updateItemView(currItem.get_ITEM_ID());
        showItem(currItem);
    }

    public ResultItem getCurrItem() => currItem;
}
