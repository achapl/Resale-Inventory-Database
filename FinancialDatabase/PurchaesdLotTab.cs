using System;
using System.Collections.Generic;
using FinancialDatabase;
using Date = Util.Date;

public class PurchasedLotTab
{
	Form1 Form1;
	CtrlerOfPythonToDTBConnector PyConnector;
	QueryBuilder QB;

    bool inEditingState;
    List<Control> editingControls;
    List<TextBox> editingTextBoxes;
    List<Control> nonEditingControls;
    List<Label>   nonEditingLabels;
    List<Label>   allPurchaseLabels;
    List<TextBox> shippingTBoxes;
    List<TextBox> itemTBoxes;
    Dictionary<TextBox, Label> editableFieldPairs;
    Dictionary<Control, string> controlBoxAttrib;


    public PurchasedLotTab(Form1 Form1)
	{
		this.Form1 = Form1;
		QB = new QueryBuilder();
        PyConnector = new CtrlerOfPythonToDTBConnector();


        inEditingState = false;

        editingControls = new List<Control>()
        {
            Form1.textBox20,
            Form1.textBox21
        };

        editingTextBoxes = new List<TextBox>()
        {
            Form1.textBox20,
            Form1.textBox21
        };

        nonEditingControls = new List<Control>()
        {
            Form1.label15,
            Form1.label41
        };
        
        nonEditingLabels = new List<Label>()
        {
            Form1.label15,
            Form1.label41
        };
        
        allPurchaseLabels = new List<Label>()
        {
            Form1.label15,
            Form1.label41
        };

        itemTBoxes = new List<TextBox>()
        {
            Form1.textBox2,
            Form1.textBox14,
            Form1.textBox11

        };
        
        shippingTBoxes = new List<TextBox>()
        {
            Form1.textBox15,
            Form1.textBox16,
            Form1.textBox17,
            Form1.textBox18,
            Form1.textBox19,

        };

        editableFieldPairs = new Dictionary<TextBox, Label>();

        for (int i = 0; i < editingTextBoxes.Count; i++)
        {
            editableFieldPairs[editingTextBoxes[i]] = nonEditingLabels[i];
        }

        controlBoxAttrib = new Dictionary<Control, string>
        {
            { Form1.dateTimePicker4,  "purchase.Date_Purchased" },
            { Form1.textBox20,        "purchase.Amount_purchase" },
            { Form1.textBox21,        "purchase.Notes_purchase" }
        };

        Util.clearLabelText(allPurchaseLabels);
        updateEditableVisibility();
    }

    public string checkDefault(int val)
    {
        if (val == ResultItem.DEFAULT_INT) { return ""; }
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
        Util.clearLabelText(allPurchaseLabels);

        Form1.currItem = item;

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.dateTimePicker4.Value = new DateTime(datePurc.year, datePurc.month, datePurc.day);
            Form1.label15.Text = checkDefault(item.get_Amount_purchase());
            Form1.label41.Text = checkDefault(item.get_Notes_purchase());
        }
    }


    public void update(ResultItem item)
	{
        Form1.listBox2.Items.Clear();
		Form1.currentPurchaseItems.Clear();
        List<ResultItem> result = PyConnector.RunItemSearchQuery(QB.buildPurchaseQuery(item));

		foreach(ResultItem i in result)
		{
			Form1.listBox2.Items.Add(i.get_Name());
            Form1.currentPurchaseItems.Add(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();

	}

    public void flipEditState()
    {


        inEditingState = !inEditingState;

        if (inEditingState)
        {
            // Now button, when pressed will change it to "viewing" state
            Form1.button6.Text = "View";

        }
        else
        {
            Form1.button6.Text = "Edit";
        }

        updateEditableVisibility();
    }

    public void updateEditableVisibility()
    {
        if (inEditingState)
        {
            Form1.button7.Visible = true;
        }
        else
        {
            Form1.button7.Visible = false;
        }

        foreach (Control c in nonEditingControls)
        {
            if (inEditingState)
            {
                c.Visible = false;
            }
            else
            {
                c.Visible = true;
            }
        }
        foreach (Control c in editingControls)
        {
            if (inEditingState)
            {
                c.Visible = true;
                if (c is TextBox)
                {
                    c.Text = editableFieldPairs[c as TextBox].Text;
                }
            }
            else
            {
                c.Visible = false;
            }
        }
    }


    private bool tableEntryExists(TextBox t)
    {
        if (controlBoxAttrib.ContainsKey(t))
        {
            string ret = "";
            // Check if the attribute associated with the textbox is a default value in the curr item
            Form1.currItem.getAttribAsString(controlBoxAttrib[t], ref ret);
            if (ret.CompareTo(ResultItem.DEFAULT_INT.ToString()) == 0 ||
                ret.CompareTo(ResultItem.DEFAULT_DOUBLE.ToString()) == 0 ||
                ret is null ||
                ret.CompareTo(ResultItem.DEFAULT_DATE.ToString()) == 0)
            {
                return false;
            }
        }
        return true;
    }

    private List<Control> getChangedFields()
    {
        List<Control> fields = new List<Control>(new Control[] {Form1.dateTimePicker4,
                                                    Form1.textBox20,
                                                    Form1.textBox21,});
        List<Control> returnList = new List<Control>();
        foreach (Control f in fields)
        {
            if (f.GetType() == typeof(TextBox) && f.Text.Length > 0)
            {
                string ret = "";
                Form1.currItem.getAttribAsString(controlBoxAttrib[f], ref ret);
                // If text doesn't match currItem for same field add it
                // Ignore any given text that already matches currItem
                if (((TextBox)f).Text.CompareTo(ret) != 0)
                {
                    returnList.Add(f);

                }
            }
            else if (f.GetType() == typeof(NumericUpDown) && ((NumericUpDown)f).Value != null)
            {
                string ret = "";
                Form1.currItem.getAttrib(controlBoxAttrib[f], ref ret);
                // If text doesn'f match currItem for same field add it
                // Ignore any given text that already matches currItem
                if (((TextBox)f).Text.CompareTo(ret) != 0)
                {
                    returnList.Add(f);

                }
            }

            else if (f.GetType() == typeof(DateTimePicker) && new Date(f).toDateString().CompareTo(Form1.currItem.get_Date_Purchased().toDateString()) != 0)
            {
                returnList.Add(f);
            }
        }

        return returnList;

    }


    public void editUpdate()
    {
        if (Form1.currItem == null) { return; }
        List<Control> changedFields = getChangedFields();


        foreach (Control c in changedFields)
        {
            if (c is null) { Console.WriteLine("ERROR: Control Object c is null, ItemViewTab.cs"); continue; }

            TextBox t = c as TextBox ?? new TextBox();// ?? denotes null assignment

            string query = "";
            if (tableEntryExists(t))
            {
                string type = Form1.colDataTypes[controlBoxAttrib[c]];

                query = QB.buildUpdateQuery(Form1.currItem, controlBoxAttrib[t], type, t.Text);

                // Update the item table with the new shipping info
                string output = PyConnector.runStatement(query);
                updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID())); // Will also reset currItem with new search for it
                t.Clear();
                t.BackColor = Color.White;
            }
            else if (!tableEntryExists(t))
            {
                Console.WriteLine("ERROR: no purchase entry for CurrItem, This should not be possible");
                continue;
            }

        }
        updateItemView(PyConnector.getItem(Form1.currItem.get_ITEM_ID()));
        showItem(Form1.currItem);
    }

    public void updateItemView(ResultItem item)
    {
        showItem(item);

        Form1.listBox2.Items.Clear();
        Form1.currentPurchaseItems.Clear();
        List<ResultItem> result = PyConnector.RunItemSearchQuery(QB.buildPurchaseQuery(item));

        foreach (ResultItem i in result)
        {
            Form1.listBox2.Items.Add(i.get_Name());
            Form1.currentPurchaseItems.Add(i);
        }

        Form1.label15.Text = item.get_Amount_purchase().ToString();
        Form1.label41.Text = item.get_Notes_purchase();
    }


    public bool allNewShippingBoxesFilled()
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




    public void addItem()
    {
        // Must at least have name. Init and curr quantites are given a default val of 1
        if (Form1.textBox2.Text == "")
        {
            return;
        }
        ResultItem newItem = new ResultItem();
        newItem.set_Name(Form1.textBox2.Text);

        // If no Init or curr quantities, set to default 1
        if (Form1.textBox14.Text.CompareTo("") == 0)
        {
            newItem.set_InitialQuantity(1);
        }
        else
        {
            newItem.set_InitialQuantity(Form1.textBox14.Text);
        }

        if (Form1.textBox11.Text.CompareTo("") == 0)
        {
            newItem.set_CurrentQuantity(1);
        }
        else
        {
            newItem.set_CurrentQuantity(Form1.textBox11.Text);
        }

        if (allNewShippingBoxesFilled())
        {
            int ttlWeight = Int32.Parse(Form1.textBox18.Text) * 16 + Int32.Parse(Form1.textBox19.Text);
            newItem.set_Weight(ttlWeight);
            newItem.set_Length(Int32.Parse(Form1.textBox15.Text));
            newItem.set_Width( Int32.Parse(Form1.textBox16.Text));
            newItem.set_Height(Int32.Parse(Form1.textBox17.Text));
        }

        newItem.set_PurchaseID(Form1.currItem.get_PurchaseID());

        PyConnector.insertItem(newItem);

        updateItemView(Form1.currItem);
    }

}
