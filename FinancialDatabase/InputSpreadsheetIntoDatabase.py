from DtbConnAndQuery import runQuery
import csv

itemTable = "item"
purcTable = "purchase"
saleTable = "sale"
shipTable = "shipping"


def isSold(row):
	return (row[3] != '')

def hasPackingDims(row):
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
	
	if purchaseID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET PurchaseID = " + purhcaseID
		runQuery(modifiedItemQuery)
	if saleID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET SaleID = " + saleID
		runQuery(modifiedItemQuery)
	if shipID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET ShippingID = " + shipID
		runQuery(modifiedItemQuery)

	return

def inputIntoDatabase(data):
	for index, row in enumerate(data):
		prevRow = None
		nextRow = None
		if index > 0:
			prevRow = data[index-1]
		if index < len(data) - 1:
			nextRow = data[index+1]

	#for prevRow, row, nextRow in data:
		boolIsSold		   = isSold(row)
		boolhasPackingDims = hasPackingDims(row)		

		#Default - Item entry and purchase entry
		
		itemQuery = "USE tool_database; INSERT INTO " + itemTable + " (Name) VALUES  (\"" + row[1] + "\");"
		runQuery(itemQuery)

		"""currItemID = getLastID(itemTable)

		purchaseQuery = "INSERT INTO " + purcTable + " VALUES (Date_Purchased, Amount, ItemID) (" + row[0] + ", STR_TO_DATE('" + row[2] + "', '%Y-%m-%d')" + "," + currItemID + ")"
		runQuery(purchaseQuery)

		if boolIsSold:
			saleQuery = "INSERT INTO " + soldQuery + " VALUES (Date_Sold, Amount, ItemID) (" + ", STR_TO_DATE('" + row[5] + "', '%Y-%m-%d')" + ", " + row[3] + ", " + currItemID + ")"
			runQuery(saleQuery)

		if boolhasPackingDims:
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
		
		updateItemIDs(currItemID)"""




with open('Tool Buys - Sheet1_FMT_Testing.csv') as csvfile:
	CSVSheet = list(csv.reader(csvfile, delimiter=',', quotechar='"'))
	inputIntoDatabase(CSVSheet)
	"""for row in CSVSheet:
		inputIntoDatabase(row)"""
