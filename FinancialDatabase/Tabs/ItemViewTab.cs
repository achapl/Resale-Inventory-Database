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
using Microsoft.VisualBasic;

public class ItemViewTab : Tab
{
    protected List<TextBox> weightTBoxes;

    private Item? currItem;

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


    public Item getCurrItem()
    {
        if (currItem != null)
        {
            updateCurrItemUsingDtb();
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
            //Form1.itemSoldPriceLbl,
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
                Label l = mutableAttribValueLabels[i];
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
    public void updateCurrItemUsingDtb()
    {
        currItem = Database.getItem(currItem.get_ITEM_ID());
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
        bool success = updateCurrItemWithUserInput();
        if (success)
        {
            viewMode();
        }
    }


    // Use user input to update the currItem
    public bool updateCurrItemWithUserInput()
    {
        if (tabController.getCurrItem() == null) { return false; }

        List<Control> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields))  { return false; }

        bool weightIsUpdated = false;

        foreach (Control userInputContainer in changedFields)
        {
            TextBox userInputTextbox = userInputContainer as TextBox;
            if (databaseEntryExists(userInputTextbox))
            {
                if (userInputTextbox == Form1.itemWeightLbsTxtbox ||
                     userInputTextbox == Form1.itemWeightOzTxtbox)
                {
                    if (!weightIsUpdated)
                    {
                        int ttlWeight = getUserInputWeight();
                        Database.updateRow(getCurrItem(), "shipping.Weight", ttlWeight.ToString());
                        weightIsUpdated = true;
                    }
                }
                else
                {
                    string attrib = controlAttrib[userInputContainer];
                    string newVal = getUserInputVal(userInputContainer);

                    Database.updateRow(getCurrItem(), attrib, newVal);
                }
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

        updateCurrItemUsingDtb();
        showItemAttributesAndPics(currItem);
        return true;
    }


    private string getUserInputVal(Control c)
    {
        switch (c)
        {
            case TextBox:
                return (c as TextBox).Text;
            case DateTimePicker:
                return new Date(c).toDateString();
            default:
                throw new Exception("Error: Unaccounted for Control Type");
        }
    }


    private int getUserInputWeight()
    {
        int ttlWeight = 0;
        int lbs = 0;
        int oz = 0;
        if (!Int32.TryParse(Form1.itemWeightLbsTxtbox.Text, out lbs)
         || !Int32.TryParse(Form1.itemWeightOzTxtbox.Text, out oz))
        {
            ttlWeight = -1;
            showWarning("Must Input Correct Numerical Format For weight. No decimals/commas allowed!");
            return -1;
        }
        Util.clearTBox(Form1.itemWeightLbsTxtbox);
        Util.clearTBox(Form1.itemWeightOzTxtbox);

        ttlWeight = lbs * 16 + oz;

        return ttlWeight;
    }


    private void makeShippingEntry()
    {
        if (allShippingBoxesFilled())
        {
            if (!typeCheckUserInput(shippingTBoxes))
            {
                showWarning("Warning: Input shipping information as integers!");
            }

            int weightLbs = Int32.Parse(Form1.itemWeightLbsTxtbox.Text);
            int weightOz = Int32.Parse(Form1.itemWeightOzTxtbox.Text);
            int l = Int32.Parse(Form1.itemLengthTxtbox.Text);
            int w = Int32.Parse(Form1.itemWidthTxtbox.Text);
            int h = Int32.Parse(Form1.itemHeightTxtbox.Text);

            Database.insertShipInfo(getCurrItem(), weightLbs, weightOz, l, w, h);
            Util.clearTBox(weightTBoxes);
            showItemAttributesAndPics(Database.getItem(tabController.getCurrItem().get_ITEM_ID())); // Will also reset currItem with new search for it
        }
        else
        {
            showWarning("To Add Shipping Info, all fields must be filled (Lbs, Oz, L, W, H)");
        }
    }


    public void deleteShippingInfo()
    {
        // Delete shipping info entry
        Database.deleteShipInfo(tabController.getCurrItem());

        // Remove foreign key reference to shipping info from item table
        string attrib = "item.ShippingID";
        bool success = Database.updateRow(tabController.getCurrItem(), attrib, null);

        if (success)
        {
            showItemAttributesAndPics(Database.getItem(tabController.getCurrItem().get_ITEM_ID()));
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

    public override void showItemAttributesAndPics(Item item)
    { 

        Util.clearLabelText(attributeValueLabels);

        if (item is null) { return; }

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
            Date datePurc = tabController.getCurrPurc().Date_Purchased;
            Form1.itemDatePurcLbl.Text = datePurc.toDateString();
            Form1.itemPurcPriceLbl.Text = checkDefault(tabController.getCurrPurc().Amount_purchase);
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
            if (!showWarningYESNO(
                            "This is the last item left in the purchased lot, are you sure you want to delete it? Doing so will delete the whole purchase." ))
            {
                return false;
            }
        }

        Database.deleteItem(tabController.getCurrItem());

        return true;
    }

    public void showItemPictures(Item newItem)
    {
        newItem.set_images();
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

    public void setCurrItem(Item? newItem)
    {
       
        if (newItem == null) {
            currItem = null;
            return;
        }
        
        // Items must only come from user input or currently indexed items such as from the current purchase or item search list.
        // If one has to bring an item outside of these places, there's something wrong. Possibility of bugs introduced.
        if (!tabController.getSearchItems().Contains(newItem) &&
            !tabController.getCurrPurcItems().Contains(newItem) &&
            !tabController.isNewPurchase())
        {
            throw new Exception("Error: ItemViewTab.setCurrItem,"
                                + "Result item ID: " + newItem.get_ITEM_ID().ToString() + ", "
                                + "Name: " + newItem.get_Name() + ", "
                                + "Not found!");
        }

        currItem = newItem;
        updateCurrItemUsingDtb();
    }

    public void clearCurrItem()
    {
        currItem = null;
    }

    public void setThumbnail()
    {
        int currImageID = getCurrImageID();

        // Check defualt val. Do nothing
        if (currImageID == Util.DEFAULT_INT)
        {
            return;
        }
        Database.setThumbnail(tabController.getCurrItem().get_ITEM_ID(), currImageID);
    }
}
