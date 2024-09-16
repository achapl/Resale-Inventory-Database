using System;
using System.Collections.Generic;
using FinancialDatabase;

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

		Form1.tabControl1.SelectTab(2);

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
}
