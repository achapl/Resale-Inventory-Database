import DtbConnAndQuery.py

itemTable = "item"
purcTable = "purchase"
saleTable = "sale"
shipTable = "shipping"


def isSold(row):
	return (row[3] != '')

def hasPackingDims
	return (row[7] != '')

# Given a table where you just added a row, return the primary key for that row
def getLastID(table):
	query = "SELECT last_insert_id() from " + table
	return runQuery(query)

def updateItemIDs(currItemID):
	purchaseQuery = "SELECT PURCHASE_ID from " + purcTable + " WHERE ItemID = " + currItemID
	saleQuery     = "SELECT SALE_ID     from " + saleTable + " WHERE ItemID = " + currItemID
	shippingQuery = "SELECT SHIPPING_ID from " + shipTable + " WHERE ItemID = " + currItemID
	
	purchaseID = runQuery(purchaseQuery)
	saleID = runQuery(saleQuery)
	shipID = runQuery(shippingQuery)
	
	if purchaseID Not "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET PurchaseID = " + purhcaseID
		runQuery(modifiedItemQuery)
	if saleID Not "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET SaleID = " + saleID
		runQuery(modifiedItemQuery)
	if shipID Not "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET ShippingID = " + shipID
		runQuery(modifiedItemQuery)

	return

def inputIntoDatabase(data):
	for prevRow, row, nextRow in data:
		isSold		   = isSold(row)
		hasPackingDims = hasPackingDims(row)		

		#Default - Item entry and purchase entry
		
		itemQuery = "INSERT INTO " + itemTable + " VALUES (Name) "
		runQuery(itemQuery)

		currItemID = getLastID(itemTable)

		purchaseQuery = "INSERT INTO " + purcTable + " VALUES (Date_Purchased, Amount, ItemID) (" + data[0] + "," + data[2] + "," + currItemID + ")"
		runQuery(purchaseQuery)

		if isSold:
			saleQuery = "INSERT INTO " + soldQuery + " VALUES (Date_Sold, Amount, ItemID) (" + row[5] + ", " + row[3] + ", " + currItemID + ")"
			runQuery(saleQuery)

		if hasPackingDims:
			ouncesPerPound = 16
			# [lbs, oz, l,w,h]
			shipDims = row[8].split(",")
			lbs = shipDims[0]
			oz  = shipDims[1]
			l   = shipDims[2]
			w   = shipDims[3]
			h   = shipDims[4]

			ttlWeight = lbs*ouncePerPound + oz

			packQuery = "INSERT INTO " + shipTable + " VALUES (Length, Width, Height, Weight, ItemID) (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + currItemID + ")"
			runQuery(packQuery)
		
		updateItemIDs(currItemID)




with open('Tool Buys - Sheet1_FMT.csv') as csvfile:
    CSVSheet = csv.reader(csvfile, delimiter=',', quotechar='"')
    for row in CSVSheet:
		inputIntoDatabase(row)