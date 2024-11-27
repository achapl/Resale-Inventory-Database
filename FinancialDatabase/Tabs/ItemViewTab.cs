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
        updateButton = Form1.itemUpdateButton;
        editButton   = Form1.itemEditButton;
        generateTBoxGroups();
        Util.clearLabelText(attributeValueLabels);
        showControlVisibility();
    }

    protected override void generateTBoxGroups()
    {
        attributeValueLabels = new List<Label>() {
            Form1.itemNameLbl,
            Form1.itemPurcPriceLbl,
            Form1.itemSoldPriceLbl,
            Form1.itemInitQtyLbl,
            Form1.itemCurrQtyLbl,
            Form1.itemItemNoLbl,
            Form1.itemWeightLbsLbl,
            Form1.itemWeightOzLbl,
            Form1.itemLengthLbl,
            Form1.itemWidthLbl,
            Form1.itemHeightLbl,
            Form1.itemDatePurcLbl
        };
        mutableAttribValueLabels = new List<Label>(){
            Form1.itemNameLbl,
            Form1.itemInitQtyLbl,
            Form1.itemCurrQtyLbl,
            Form1.itemWeightLbsLbl,
            Form1.itemWeightOzLbl,
            Form1.itemLengthLbl,
            Form1.itemWidthLbl,
            Form1.itemHeightLbl,
        };

        newItemTBoxes = new List<TextBox>()
        {
            Form1.itemNameTxtbox,
            Form1.itemInitQtyTxtbox,
            Form1.itemCurrQtyTxtbox
        };
        weightTBoxes = new List<TextBox>()
        {
            Form1.itemWeightLbsTxtbox,
            Form1.itemWeightOzTxtbox
        };
        shippingTBoxes = new List<TextBox>()
        {
            Form1.itemWeightLbsTxtbox,
            Form1.itemWeightOzTxtbox,
            Form1.itemLengthTxtbox,
            Form1.itemWidthTxtbox,
            Form1.itemHeightTxtbox
        };
        mutableAttribValueControls = new List<Control>(){
            Form1.itemNameTxtbox,
            Form1.itemInitQtyTxtbox,
            Form1.itemCurrQtyTxtbox,
            Form1.itemWeightLbsTxtbox,
            Form1.itemWeightOzTxtbox,
            Form1.itemLengthTxtbox,
            Form1.itemWidthTxtbox,
            Form1.itemHeightTxtbox

        };

        hideableAttribValueControls = new List<Control>(){
            Form1.itemNameTxtbox,
            Form1.itemInitQtyTxtbox,
            Form1.itemCurrQtyTxtbox,
            Form1.itemWeightLbsTxtbox,
            Form1.itemWeightOzTxtbox,
            Form1.itemLengthTxtbox,
            Form1.itemWidthTxtbox,
            Form1.itemHeightTxtbox,
            Form1.deleteShipInfoButton
        };

        labelTextboxPairs = new Dictionary<Control, Label>();

        int i = 0;
        foreach (Control c in mutableAttribValueControls)
        {
            if (c is not Button)
            {
                labelTextboxPairs[c] = mutableAttribValueLabels[i++];
            }
        }

        controlAttrib = new Dictionary<Control, string>
        {{ Form1.itemWeightLbsTxtbox,  "shipping.WeightLbs" },
        { Form1.itemInitQtyTxtbox,  "item.InitialQuantity" },
        { Form1.itemCurrQtyTxtbox,  "item.CurrentQuantity" },
        { Form1.itemWeightOzTxtbox, "shipping.WeightOz" },
        { Form1.itemHeightTxtbox, "shipping.Height" },
        { Form1.itemLengthTxtbox, "shipping.Length" },
        { Form1.itemWidthTxtbox, "shipping.Width" },
        { Form1.itemNameTxtbox,  "item.Name" }};
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
                    if (!Int32.TryParse(Form1.itemWeightLbsTxtbox.Text, out lbs)
                     || !Int32.TryParse(Form1.itemWeightOzTxtbox.Text, out oz))
                    {
                        goodEdit = false;
                        Util.clearTBox(Form1.itemWeightLbsTxtbox);
                        Util.clearTBox(Form1.itemWeightOzTxtbox);
                        continue;
                    }
                    int ttlWeight = lbs * 16 + oz;

                    // Execute query
                    string attrib = "shipping.Weight";
                    string type = tabController.colDataTypes[attrib];
                    query = QueryBuilder.updateQuery(tabController.getCurrItem(), attrib, type, ttlWeight.ToString());
                    output = DatabaseConnector.runStatement(query);
                    // These must be cleared manually since they are both used at the same time.
                    // Clearing one produces an error when the other textbox is then used to get the total weight
                    Util.clearTBox(Form1.itemWeightLbsTxtbox);
                    Util.clearTBox(Form1.itemWeightOzTxtbox);
                    skipElem = true;

                }
                else
                {
                    string attrib = t.Text;
                    string type = tabController.colDataTypes[controlAttrib[c]];
                    if (!Util.checkTypeOkay(attrib, type))
                    {
                        goodEdit = false;
                        continue;
                    }
                    switch (c)
                    {
                        case TextBox:
                            query = QueryBuilder.updateQuery(tabController.getCurrItem(), controlAttrib[c], type, t.Text);
                            break;
                        case DateTimePicker:
                            query = QueryBuilder.updateQuery(tabController.getCurrItem(), controlAttrib[c], type, new Date(c));
                            break;
                    }
                    output = DatabaseConnector.runStatement(query);
                }
                // Update the item in the view
                if (output.CompareTo("ERROR") != 0)
                {
                    showItemAttributes(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
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
                            weightLbs = Int32.Parse(Form1.itemWeightLbsTxtbox.Text);
                            weightOz = Int32.Parse(Form1.itemWeightOzTxtbox.Text);
                            l = Int32.Parse(Form1.itemLengthTxtbox.Text);
                            w = Int32.Parse(Form1.itemWidthTxtbox.Text);
                            h = Int32.Parse(Form1.itemHeightTxtbox.Text);
                        }
                        catch
                        {
                            goodEdit = false;
                            continue;
                        }

                        //TODO: Why is this not happening all in DatabaesConnector?
                        query = QueryBuilder.shipInfoInsertQuery(tabController.getCurrItem(), weightLbs, weightOz, l, w, h);

                        int lastrowid;
                        string output = DatabaseConnector.runStatement(query, out lastrowid);

                        string attrib = "item.ShippingID";
                        string type = tabController.colDataTypes[attrib];
                        int shippingID = lastrowid;
                        query = QueryBuilder.updateQuery(tabController.getCurrItem(), attrib, type, shippingID.ToString());

                        // Update the item table with the new shipping info
                        output = DatabaseConnector.runStatement(query);
                        if (output.CompareTo("ERROR") != 0)
                        {
                            goodEdit = false;
                            Util.clearTBox(weightTBoxes);
                        }
                        showItemAttributes(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
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
                else if (newItemTBoxes.Contains(t))
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
        showItemAttributes(tabController.getCurrItem());
        viewMode();
    }


    public void deleteShippingInfo()
    {
        // Delete shipping info entry
        string query = QueryBuilder.deleteShipInfoQuery(tabController.getCurrItem());
        string output = DatabaseConnector.runStatement(query);

        // Remove foreign key reference to shipping info from item table
        string attrib = "item.ShippingID";
        string type = tabController.colDataTypes[attrib];
        query = QueryBuilder.updateQuery(tabController.getCurrItem(), attrib, type, null);
        output = DatabaseConnector.runStatement(query);

        if (output.CompareTo("ERROR") != 0)
        {
            showItemAttributes(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID()));
        }
        flipEditMode();
    }

    override public void flipEditMode()
    {
        // Don't go into edit mode if there is no item to edit
        if (!inEditingState)
        {
            if (tabController.getCurrItem() == null) { return; }
            recordAttributeStates();
        }
        inEditingState = !inEditingState;
        
        showControlVisibility();

    }

    public override void showItemAttributes(ResultItem item)
    { 
        Util.clearLabelText(attributeValueLabels);

        if (item.hasItemEntry())
        {
            Form1.itemNameLbl.Text = checkDefault(item.get_Name());
            Form1.itemInitQtyLbl.Text = checkDefault(item.get_InitialQuantity());
            Form1.itemCurrQtyLbl.Text = checkDefault(item.get_CurrentQuantity());
            Form1.itemItemNoLbl.Text = checkDefault(item.get_ITEM_ID());
            Form1.SaleNameLbl.Text = checkDefault(item.get_Name());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = item.get_Date_Purchased();
            Form1.itemDatePurcLbl.Text = datePurc.toDateString();
            Form1.itemPurcPriceLbl.Text = checkDefault(item.get_Amount_purchase());
        }

        if (item.hasSaleEntry())
        {
            Form1.itemSoldPriceLbl.Text = item.getTotalSales().ToString();
        }

        if (item.hasShippingEntry())
        {
            List<int> WeightLbsOz = Util.ozToOzLbs(item.get_Weight());
            Form1.itemWeightLbsLbl.Text = checkDefault(WeightLbsOz[0]);
            Form1.itemWeightOzLbl.Text = checkDefault(WeightLbsOz[1]);
            Form1.itemLengthLbl.Text = checkDefault(item.get_Length());
            Form1.itemWidthLbl.Text = checkDefault(item.get_Width());
            Form1.itemHeightLbl.Text = checkDefault(item.get_Height());
        }
        updateUserInputDefaultText();
    }

    public bool deleteItem()
    {
        if (tabController.getCurrItem() is null) { return false; }

        // Warn user if it is the last item in the lot
        // Deleting this item results in deletion of the lot purchase entry
        if (tabController.currentPurchaseItems.Count == 1)
        {
            DialogResult result = MessageBox.Show(
                            "This is the last item left in the purchased lot, are you sure you want to delete it? Doing so will delete the whole purchas.",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                            );
            if (result == DialogResult.No) { return false; }
        }

        DatabaseConnector.deleteItem(tabController.getCurrItem());

        return true;
    }

    public void showItemPictures(ResultItem newItem)
    {
        Form1.allPictureViewer.setImages(newItem.get_Images());
        if (newItem.get_Images().Count > 0)
        {
            Form1.mainPictureViewer.setImage(newItem.get_Images()[0]);
        }
    }

    public void setMainImage(int currIndex)
    {
        Form1.mainPictureViewer.setImage(Form1.allPictureViewer.getImage(currIndex));
    }

    public int getCurrImageID()
    {
        return Form1.mainPictureViewer.getCurrImageID();
    }
}
