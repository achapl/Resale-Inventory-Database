using FinancialDatabase;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;
using Date = Util.Date;

public class ItemViewTab
{
	Form1 Form1;
    ResultItem currItem;
    QueryBuilder QB;
    Dictionary<Control, string> controlBoxAttrib;
    CtrlerOfPythonToDTBConnector PyConnector;
	public ItemViewTab(Form1 Form1)
	{
		this.Form1 = Form1;
        QB = new QueryBuilder();
        PyConnector = new CtrlerOfPythonToDTBConnector();

        controlBoxAttrib = new Dictionary<Control, string>
        {
            { Form1.dateTimePicker3,  "purchase.Date_Purchased" },
            { Form1.textBox3,  "item.Name" },
            { Form1.textBox4,  "purchase.Amount_purchase" },
            { Form1.textBox5,  "sale.Amount_sale" },
            { Form1.textBox6,  "item.InitialQuantity" },
            { Form1.textBox7,  "item.CurrentQuantity" },
            { Form1.textBox8,  "item.ITEM_ID" },
            { Form1.textBox9,  "shipping.WeightLbs" },
            { Form1.textBox10, "shipping.WeightOz" },
            { Form1.textBox11, "shipping.Length" },
            { Form1.textBox12, "shipping.Width" },
            { Form1.textBox13, "shipping.Height" }
        };
    }


   
    public List<int> ozToOzLbs(int ozs)
    {
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
        currItem = item;

        Date datePurc = item.get_Date_Purchased();

        List<int> WeightLbsOz = ozToOzLbs(item.get_Weight());

        Form1.label15.Text = checkDefault(item.get_Name());
        Form1.dateTimePicker3.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
        Form1.label17.Text = checkDefault(item.get_Amount_purchase());
        Form1.label18.Text = checkDefault(item.get_Amount_sale());
        Form1.label19.Text = checkDefault(item.get_InitialQuantity());
        Form1.label20.Text = checkDefault(item.get_CurrentQuantity());
        //Form1.label21.Text = checkDefault(.get_checkDefault(No());
        Form1.label22.Text = checkDefault(WeightLbsOz[0]);
        Form1.label23.Text = checkDefault(WeightLbsOz[1]);
        Form1.label24.Text = checkDefault(item.get_Length());
        Form1.label25.Text = checkDefault(item.get_Width());
        Form1.label26.Text = checkDefault(item.get_Height());
    }

    private List<Control> getChangedFields()
    {
        List<Control> textboxes = new List<Control>(new Control[] {Form1.dateTimePicker3,
                                                    Form1.textBox3,
                                                    Form1.textBox4,
                                                    Form1.textBox5,
                                                    Form1.textBox6,
                                                    Form1.textBox7,
                                                    Form1.textBox8,
                                                    Form1.textBox9,
                                                    Form1.textBox10,
                                                    Form1.textBox11,
                                                    Form1.textBox12,
                                                    Form1.textBox13});
        List<Control> returnList = new List<Control>();
        foreach (Control t in textboxes)
        {
            if (t.GetType() == typeof(TextBox) && t.Text.Length > 0)
            {
                returnList.Add(t);
            }

            else if (t.GetType() == typeof(DateTimePicker) && new Date(t).toDateString().CompareTo(currItem.get_Date_Purchased().toDateString()) != 0)
            {
                returnList.Add(t);
            }
        }

        return returnList;

    }

    public void editUpdate()
    {
        if (currItem == null) { return; }
        List<Control> changedFields = getChangedFields();

        foreach (Control c in changedFields)
        {
            string s = controlBoxAttrib[c];
            string type = Form1.colDataTypes[controlBoxAttrib[c]];
            string query = "";
            if (c.GetType() == typeof(TextBox))
            {

                TextBox t = c as TextBox;
                query = QB.buildUpdateQuery(currItem, controlBoxAttrib[t], type, t.Text);
                t.Clear();
            }

            else if (c.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker dt = c as DateTimePicker;
                
                query = QB.buildUpdateQuery(currItem, controlBoxAttrib[c], type, new Date(dt));
                dt.Value = dt.MinDate; // Set as default value to show it has been "cleared" if new date does not show
            }

            // Got bad input from user, QB could not create a query
            if (query.CompareTo("ERROR: BAD USER INPUT") == 0) 
            {
                continue;
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
