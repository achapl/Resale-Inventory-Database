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
using FinancialDatabase.Tabs;

public class ItemViewTab : Tab
{
    protected List<TextBoxLabelPair> weightTBoxes;

    private Item? currItem;

    public ItemViewTab(TabController tabController, Form1 Form1) : base(Form1)
    {
        this.tabController = tabController;
        currItem = null;
        updateButton = Form1.itemUpdateButton;
        editButton = Form1.itemEditButton;
        generateTBoxGroups();
        Util.clearControlText(allAttributeValueLabels);
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
        allAttributeValueLabels = new List<Control>() {
            Form1.itemNameTLP,
            Form1.itemPurcPriceLbl,
            Form1.itemSoldPriceLbl,
            Form1.itemInitQtyTLP,
            Form1.itemCurrQtyTLP,
            Form1.itemItemNoLbl,
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP,
            Form1.itemLengthTLP,
            Form1.itemWidthTLP,
            Form1.itemHeightTLP,
            Form1.itemDatePurcLbl
        };

        itemTBoxes = new List<TextBoxLabelPair>()
        {
            Form1.itemNameTLP,
            Form1.itemInitQtyTLP,
            Form1.itemCurrQtyTLP
        };
        weightTBoxes = new List<TextBoxLabelPair>()
        {
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP
        };
        shippingTBoxes = new List<TextBoxLabelPair>()
        {
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP,
            Form1.itemLengthTLP,
            Form1.itemWidthTLP,
            Form1.itemHeightTLP
        };
        mutableAttribValueControls = new List<ControlLabelPair>(){
            Form1.itemNameTLP,
            Form1.itemInitQtyTLP,
            Form1.itemCurrQtyTLP,
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP,
            Form1.itemLengthTLP,
            Form1.itemWidthTLP,
            Form1.itemHeightTLP

        };

        hideableAttribValueControls = new List<Control>(){
            Form1.itemNameTLP,
            Form1.itemInitQtyTLP,
            Form1.itemCurrQtyTLP,
            Form1.itemWeightLbsTLP,
            Form1.itemWeightOzTLP,
            Form1.itemLengthTLP,
            Form1.itemWidthTLP,
            Form1.itemHeightTLP,
            Form1.deleteShipInfoButton
        };
        
        Form1.itemWeightLbsTLP.attrib =  "shipping.WeightLbs";
        Form1.itemInitQtyTLP.attrib =  "item.InitialQuantity";
        Form1.itemCurrQtyTLP.attrib =  "item.CurrentQuantity";
        Form1.itemWeightOzTLP.attrib =  "shipping.WeightOz";
        Form1.itemHeightTLP.attrib =  "shipping.Height";
        Form1.itemLengthTLP.attrib =  "shipping.Length";
        Form1.itemWidthTLP.attrib =  "shipping.Width";
        Form1.itemNameTLP.attrib =  "item.Name";
    }


    // Update the under-hood reference to the object in the database
    public void updateCurrItemUsingDtb()
    {
        currItem = Database.getItem(currItem.ITEM_ID);
    }


    // Checks that all shipping boxes are filled, and are integers
    public bool allShippingBoxesFilled()
    {
        foreach (TextBoxLabelPair c in shippingTBoxes)
        {
            if (c.getControlValueAsStr().CompareTo("") == 0 ||
                !Int32.TryParse(c.getControlValueAsStr(), out int _))
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
            updateCurrItemUsingDtb();
            showItemAttributes(currItem);
            viewMode();
        }
    }


    // Use user input to update the currItem
    public bool updateCurrItemWithUserInput()
    {
        if (tabController.getCurrItem() == null) { return false; }

        List<ControlLabelPair> changedFields = getChangedFields();
        if (!typeCheckUserInput(changedFields))  { return false; }

        bool weightIsUpdated = false;

        foreach (ControlLabelPair userInputContainer in changedFields)
        {
            TextBoxLabelPair userInputTLP = userInputContainer as TextBoxLabelPair;

            if (userInputContainer == Form1.itemWeightOzTLP &&
                weightIsUpdated)
            {
                continue;
            }
            if (databaseEntryExists(userInputTLP))
            {

                // Special case, weight TLP's
                if (userInputTLP == Form1.itemWeightLbsTLP ||
                     userInputTLP == Form1.itemWeightOzTLP)
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
                    string attrib = userInputContainer.attrib;

                    string newVal = getUserInputVal(userInputContainer);

                    Database.updateRow(getCurrItem(), attrib, newVal);
                }
                Util.clearControl(userInputTLP);
            }
            else
            {
                if (shippingTBoxes.Contains(userInputTLP))
                {
                    // NOTE: Why is weight textbox blank at THIS point?
                    makeShippingEntry();
                }
                else if (itemTBoxes.Contains(userInputTLP))
                {
                    // For there to be an item in the ItemTab, it must have an
                    // entry in the item table which requires at minimum
                    // the attributes of the newItemTBoxes
                    throw new Exception("ERROR: no item entry for CurrItem, This should not be possible");
                }
            }
        }

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
            case TextBoxLabelPair:
                return (c as TextBoxLabelPair).getControlValueAsStr();
            case DateTimePickerLabelPair:
                return new Util.Date(c as DateTimePickerLabelPair).toDateString();
            default:
                throw new Exception("Error: Unaccounted for Control Type");
        }
    }


    private int getUserInputWeight()
    {
        int ttlWeight = 0;
        int lbs = 0;
        int oz = 0;
        if (!Int32.TryParse(Form1.itemWeightLbsTLP.getControlValueAsStr(), out lbs)
         || !Int32.TryParse(Form1.itemWeightOzTLP.getControlValueAsStr(), out oz))
        {
            ttlWeight = -1;
            showWarning("Must Input Correct Numerical Format For weight. No decimals/commas allowed!");
            return -1;
        }
        Util.clearControl(Form1.itemWeightLbsTLP);
        Util.clearControl(Form1.itemWeightOzTLP);

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

            int weightLbs = Int32.Parse(Form1.itemWeightLbsTLP.getControlValueAsStr());
            int weightOz = Int32.Parse(Form1.itemWeightOzTLP.getControlValueAsStr());
            int l = Int32.Parse(Form1.itemLengthTLP.getControlValueAsStr());
            int h = Int32.Parse(Form1.itemHeightTLP.getControlValueAsStr());
            int w = Int32.Parse(Form1.itemWidthTLP.getControlValueAsStr());

            Database.insertShipInfo(getCurrItem(), weightLbs, weightOz, l, w, h);
            Util.clearTLPs(weightTBoxes);
            showItemAttributes(Database.getItem(tabController.getCurrItem().ITEM_ID)); // Will also reset currItem with new search for it
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
            showItemAttributes(Database.getItem(tabController.getCurrItem().ITEM_ID));
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

    public override void showItemAttributes(Item item)
    { 

        Util.clearControlText(allAttributeValueLabels);

        if (item is null) { return; }

        if (item.hasItemEntry())
        {
            Form1.itemNameTLP.setLabelText(Util.checkDefault(item.Name));
            Form1.itemNameTLP.setControlVal(Util.checkDefault(item.Name));
            Form1.itemInitQtyTLP.setLabelText(Util.checkDefault(item.InitialQuantity));
            Form1.itemInitQtyTLP.setControlVal(Util.checkDefault(item.InitialQuantity));
            Form1.itemCurrQtyTLP.setLabelText(Util.checkDefault(item.CurrentQuantity));
            Form1.itemCurrQtyTLP.setControlVal(Util.checkDefault(item.CurrentQuantity));
            Form1.itemItemNoLbl.Text = Util.checkDefault(item.ITEM_ID);
            Form1.SaleNameLbl.Text = Util.checkDefault(item.Name);

        }
        if (item.sales != null && item.sales.Count() > 0)
        {
            Form1.itemSoldPriceLbl.Text = Util.checkDefault(item.getTotalSales());
        }

        if (item.hasPurchaseEntry())
        {
            Date datePurc = tabController.getCurrPurc().Date_Purchased;
            Form1.itemDatePurcLbl.Text = datePurc.toDateString();
            Form1.itemPurcPriceLbl.Text = Util.checkDefault(tabController.getCurrPurc().Amount_purchase);
        }

        if (item.hasShippingEntry())
        {
            List<int> WeightLbsOz = Util.ozToOzLbs(item.Weight);
            Form1.itemWeightLbsTLP.setLabelText(Util.checkDefault(WeightLbsOz[0]));
            Form1.itemWeightLbsTLP.setControlVal(Util.checkDefault(WeightLbsOz[0]));
            Form1.itemWeightOzTLP.setLabelText(Util.checkDefault(WeightLbsOz[1]));
            Form1.itemWeightOzTLP.setControlVal(Util.checkDefault(WeightLbsOz[1]));
            Form1.itemLengthTLP.setLabelText(Util.checkDefault(item.Length));
            Form1.itemLengthTLP.setControlVal(Util.checkDefault(item.Length));
            Form1.itemWidthTLP.setLabelText(Util.checkDefault(item.Width));
            Form1.itemWidthTLP.setControlVal(Util.checkDefault(item.Width));
            Form1.itemHeightTLP.setLabelText(Util.checkDefault(item.Height));
            Form1.itemHeightTLP.setControlVal(Util.checkDefault(item.Height));
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
        newItem.set_imagesFromDatabase();
        Form1.allPictureViewer.setImages(newItem.images);
        if (newItem.images.Count > 0)
        {
            Form1.mainPictureViewer.setImage(newItem.images[0]);
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

    public void setCurrItemAndShowView(Item? newItem)
    {
       
        if (newItem == null ||
            newItem.ITEM_ID == Util.DEFAULT_INT) {
            currItem = null;
            return;
        }
        
        // Items must only come from user input or currently indexed items such as from the current purchase or item search list.
        // If one has to bring an item outside of these places, there's something wrong. Possibility of bugs introduced.
        if (!tabController.getSearchItems().Contains(newItem) &&
            !tabController.getCurrPurcItems().Contains(newItem))
        {
            throw new Exception("Error: ItemViewTab.setCurrItem,"
                                + "Result item ID: " + newItem.ITEM_ID.ToString() + ", "
                                + "Name: " + newItem.Name + ", "
                                + "Not found!");
        }

        currItem = newItem;

        showItemAttributes(currItem);
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
        Database.setThumbnail(tabController.getCurrItem().ITEM_ID, currImageID);
    }

    // Clear all shown info about currItem
    internal void clearCurrItemControls()
    {
        foreach (Control c in allAttributeValueLabels)
        {
            switch (c)
            {
                case DateTimePickerLabelPair:
                    (c as DateTimePickerLabelPair).setControlVal(DateTime.Now);
                    (c as DateTimePickerLabelPair).setLabelText("");
                    break;

                case TextBoxLabelPair:
                    (c as TextBoxLabelPair).setControlVal("");
                    (c as TextBoxLabelPair).setLabelText("");
                    break;

                case TextBox:
                    c.Text = "";
                    break;

                case Label:
                    c.Text = "";
                    break;

                case DateTimePicker:
                    DateTimePicker d = c as DateTimePicker;
                    d.Value = DateTime.Now;
                    break;

                case ListBox:
                    ListBox b = c as ListBox;
                    b.Items.Clear();
                    break;
            }
        }
        viewMode();
    }

    internal void deleteCurrImage()
    {
        if (currItem == null) return;

        int currImage = Form1.mainPictureViewer.getCurrImageID();

        Database.deleteImage(currImage);
    }
}
