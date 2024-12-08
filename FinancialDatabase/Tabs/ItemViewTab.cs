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

    
    private ResultItem currItem;

    public ItemViewTab(TabController tabController, Form1 Form1) : base(Form1)
    {
        this.tabController = tabController;
        currItem = null;
        updateButton = Form1.itemUpdateButton;
        editButton = Form1.itemEditButton;
        generateTBoxGroups();
        Util.clearLabelText(attributeValueLabels);
        showControlVisibility();
    }

    
    
    internal ResultItem getCurrItemsAt(int index)
    {
        if (index == -1) { return null; }
        return tabController.getSearchItems()[index];
    }
    public ResultItem getCurrItem()
    {
        if (currItem != null)
        {
            updateCurrItem();
        }
        return currItem;
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


    // Update the under-hood reference to the object in the database
    public void updateCurrItem()
    {
        currItem = DatabaseConnector.getItem(currItem.get_ITEM_ID());
    }


    // Checks that all shipping boxes are filled, and are integers
    public bool allShippingBoxesFilled()
    {
        foreach (Control c in shippingTBoxes)
        {
            if (c.Text.CompareTo("") == 0 ||
                !Int32.TryParse(c.Text, out int _))
            {
                return false;
            }
        }
        return true;
    }


    public void updateFromUserInput()
    {
        bool success = UpdateCurrItemWithUserInput();
        if (success)
        {
            viewMode();
        }
    }


    // Take user input, and use it to update the currItem
    // UpdateCurrItemWithUserInput
    public bool UpdateCurrItemWithUserInput()
    {
        bool goodEdit = true;
        if (tabController.getCurrItem() == null) { return false; }
        List<Control> changedFields = getChangedFields();

        foreach (Control userInputContainer in changedFields)
        {
            if (userInputContainer is null) { throw new Exception("ERROR: Control Object userInputContainer is null, ItemViewTab.cs"); }

            TextBox userInputTextbox = userInputContainer as TextBox;
            if (tableEntryExists(userInputTextbox))
            {
                string output = "";
                if (userInputTextbox == Form1.itemWeightLbsTxtbox)
                {
                    int ttlWeight;
                    bool success = getUserInputWeight(out ttlWeight);
                    
                    // Update the database with new weight
                    string attrib = "shipping.Weight";
                    string type = tabController.colDataTypes[attrib];
                    DatabaseConnector.updateRow(getCurrItem(), attrib, type, ttlWeight.ToString());

                    // These must be cleared manually since they are both used at the same time.
                    // Clearing one produces an error when the other textbox is then used to get the total weight
                    Util.clearTBox(Form1.itemWeightLbsTxtbox);
                    Util.clearTBox(Form1.itemWeightOzTxtbox);

                }
                else
                {
                    string newAttribVal = userInputTextbox.Text;
                    string type = tabController.colDataTypes[controlAttrib[userInputContainer]];
                    if (!Util.checkTypeOkay(newAttribVal, type))
                    {
                        goodEdit = false;
                        continue;
                    }

                    string attrib = controlAttrib[userInputContainer];
                    string newVal = getUserInputVal(userInputContainer);

                    DatabaseConnector.updateRow(getCurrItem(), attrib, type, newVal);
                }
                // Update the item in the view
                showItemAttributes(DatabaseConnector.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
                Util.clearTBox(userInputTextbox);

            }
            else
            {
                if (shippingTBoxes.Contains(userInputTextbox))
                {
                    makeShippingEntry();
                }
                else if (newItemTBoxes.Contains(userInputTextbox))
                {
                    // For there to be an item in the ItemTab, it must have an
                    // entry in the item table which requires at minimum
                    // the attributes of the newItemTBoxes
                    throw new Exception("ERROR: no item entry for CurrItem, This should not be possible");
                }
            }
            
        }

        // It is correct to run showItem, updateCurrItem, and viewMode
        // from inside this function since you will always need to
        // update the shown information after updating the
        // ResultItem copy of it
        updateCurrItem();
        showItemAttributes(getCurrItem());
        return goodEdit;
    }


    private string getUserInputVal(Control c)
    {
        switch (c)
        {
            case TextBox:
                return (c as TextBox).Text;
                break;
            case DateTimePicker:
                return new Date(c).toDateString();
                break;
            default:
                throw new Exception("Error: Unaccounted for Control Type");
        }
    }


    private bool getUserInputWeight(out int ttlWeight)
    {
        int lbs = 0;
        int oz = 0;
        if (!Int32.TryParse(Form1.itemWeightLbsTxtbox.Text, out lbs)
         || !Int32.TryParse(Form1.itemWeightOzTxtbox.Text, out oz))
        {
            Util.clearTBox(Form1.itemWeightLbsTxtbox);
            Util.clearTBox(Form1.itemWeightOzTxtbox);
            ttlWeight = -1;
            return false;
        }
        ttlWeight = lbs * 16 + oz;

        return true;
    }


    private void makeShippingEntry()
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
                MessageBox.Show(
                    "To Add Shipping Info, all fields must be filled with integers",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
                }

            string type = getShippingInfoType(); 
            DatabaseConnector.insertShipInfo(getCurrItem(), weightLbs, weightOz, l, w, h, type);
            Util.clearTBox(weightTBoxes);
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
        }

        
    }


    // Get the mySQL type of the shipping information.
    // All shipping info should have the same mySQL type
    private string getShippingInfoType()
    {
        if (shippingTBoxes == null || shippingTBoxes[0] == null)
        {
            throw new Exception("Error: There are no shipping textboxes initialized!");
        }
        string attrib = controlAttrib[shippingTBoxes[0]];
        string type = tabController.colDataTypes[attrib];
        
        // Double check that all shipping info attributes are the same
        // Should all be integers, but if that ever changes to something like double,
        // all shipping info elements should have the same type.
        foreach (Control tBox in shippingTBoxes)
        {
            string otherAttrib = "";
            string otherType = "";

            if (!controlAttrib.TryGetValue(tBox, out otherAttrib))
            {
                throw new Exception("Error: No mySQL attrib entry exists for a shipping information textbox");
            }
            
            if (!tabController.colDataTypes.TryGetValue(otherAttrib, out otherType))
            {
                throw new Exception("Error: No mySQL type entry exists for a shipping information attribute");
            }

            if (type.CompareTo(otherType) != 0)
            {
                throw new Exception("Error: Not all shipping information types are the same!");
            }
        }


        return type;
    }


    public void deleteShippingInfo()
    {
        // Delete shipping info entry
        DatabaseConnector.deleteShipInfo(tabController.getCurrItem());

        // Remove foreign key reference to shipping info from item table
        string attrib = "item.ShippingID";
        string type = tabController.colDataTypes[attrib];
        string output = DatabaseConnector.updateRow(tabController.getCurrItem(), attrib, type, null);

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
            inEditingState = true;
        } else
        {
            inEditingState = false;
        }
        
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

        showItemPictures(item);

        updateUserInputDefaultText();
    }

    public bool deleteItem()
    {
        if (tabController.getCurrItem() is null) { return false; }

        // Warn user if it is the last item in the lot
        // Deleting this item results in deletion of the lot purchase entry
        if (tabController.getCurrPurcItems().Count == 1)
        {
            DialogResult result = MessageBox.Show(
                            "This is the last item left in the purchased lot, are you sure you want to delete it? Doing so will delete the whole purchase.",
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

    internal void setCurrItem(ResultItem newItem)
    {
       
        //if (newItem == null) { return; }
        
        if (!tabController.getSearchItems().Contains(newItem) &&
            !tabController.getCurrPurcItems().Contains(newItem) && !tabController.isNewPurchase())
        {
            throw new Exception("Error: ItemViewTab.setCurrItem,"
                                + "Result item ID: " + newItem.get_ITEM_ID().ToString() + ", "
                                + "Name: " + newItem.get_Name() + ", "
                                + "Not found!");
        }

        if (newItem == null)
        {
            throw new Exception("Error: newItem is null for ItemViewTab.setCurrItem()");
        }

        this.currItem = newItem;
        updateCurrItem();

        
        
    }

    internal void clearCurrItem()
    {
        currItem = null;
    }

    internal void setThumbnail()
    {
        int currImageID = getCurrImageID();

        // Check defualt val. Do nothing
        if (currImageID == -1 || currImageID == null)
        {
            return;
        }

        int newThumbnailID = DatabaseConnector.getImageThumbnailID(currImageID);
        DatabaseConnector.runStatement("UPDATE item SET ThumbnailID = " + newThumbnailID + " WHERE item.ITEM_ID = " + getCurrItem().get_ITEM_ID() + ";");
    }
}
