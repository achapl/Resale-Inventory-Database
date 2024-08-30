using System;
using System.Collections.Generic;
using FinancialDatabase;

public class PurchasedLotTab
{
	Form1 Form1;
	CtrlerOfPythonToDTBConnector PyConnector;
	QueryBuilder QB;

	public PurchasedLotTab(Form1 Form1)
	{
		this.Form1 = Form1;
		QB = new QueryBuilder();
        PyConnector = new CtrlerOfPythonToDTBConnector();
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

		Form1.tabControl1.SelectTab(2);

	}
}
