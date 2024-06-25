from ..DtbConnAndQuery import runQuery



purcTable = "purchase"
shipTable = "shipping"
saleTable = "sale"
itemTable = "item"
feeTable  = "fee"

# Determine if sold by checking if it has a sold date
# This prevents lot headers (purchase: "Lot" which covers purchase of all items below) which have a net sold price but no date, from being inputted into the sales table
def isSold(row):
	return (row[5] != '')

def hasPackingDims(row):
	return (row[7] != '')

def hasPurchasePrice(row):
	return row[2] != ""

def isFee(row):
	return (row[12] != '')

def deleteTable(table):
	modifiedItemQuery = "DELETE FROM " + table + ";"
	result = runQuery(modifiedItemQuery)
	if result[0] == "!!!ERROR!!!":
		print("!!!ERROR!!!")
		print(modifiedItemQuery)

def clearDatabase():
	deleteTable(purcTable)
	deleteTable(shipTable)
	deleteTable(saleTable)
	deleteTable(itemTable)
	deleteTable(feeTable)

def updateItemIDs(itemID, purcID, saleID, shipID):

	if purcID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET PurchaseID = " + purcID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)
	else:
		print("ERROR, NO PURC_ID for ITEM_ID: " + itemID)

	if saleID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET SaleID = "     + saleID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)

	if shipID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET ShippingID = " + shipID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)

	return

def formatRows(data):
	for row in data:
		for i, elem in enumerate(row):
			row[i] = row[i].replace("\"", "\\\"")
	return data

def extractQuantity(row):
	boolIsSold = isSold(row)

	currQuantity = 0
	if not boolIsSold:
		currQuantity = 1
		
	initQuantity = 0
	if row[10] == "":
		initQuantity = 1
	else:
		initQuantity = row[10]

	return currQuantity, initQuantity 

def getShippingDims(row):
	ouncesPerPound = 16
	# [lbs, oz, l,w,h]
	shipDims = row[7].split(",")
	lbs = shipDims[0]
	oz  = shipDims[1]
	l   = shipDims[2]
	w   = shipDims[3]
	h   = shipDims[4]

	if oz == "":
		oz = "0"
	if lbs == "":
		lbs = "0"

	totalWeight = str(int(lbs)*ouncesPerPound + int(oz))
	return totalWeight, l, w, h


def inputIntoDatabase(data):
	
	data = formatRows(data)
	purcID = "" # This needs to be outside the for loop so the last purchaceID can carry over into next item 
	for index, row in enumerate(data):

		itemID, saleID, shipID = "", "", ""

		currQuantity, initQuantity = extractQuantity(row)

		if isFee(row):
			# Fee entry
			feeQuery = "INSERT INTO " + feeTable + " (Date, Amount, Type) VALUES (STR_TO_DATE('" + row[0] + "', '%Y-%m-%d')," + str(row[2]) + ", \"" + row[12] + "\");"
			feeID = str(runQuery(feeQuery)[1])
			continue
		
		#Item entry
		itemQuery = "INSERT INTO " + itemTable + " (Name, InitialQuantity, CurrentQuantity, Notes) VALUES (\"" + row[1] + "\"" + ", " + str(initQuantity) + ", " + str(currQuantity) + ", " + "\"" + row[8] + "\"" + ");" # Note: Change current quantity later based on small_sales.
		itemID = str(runQuery(itemQuery)[1])

		# If it is the purchase of a new lot or single item lot, insert that purchase into the database, and
		# update the most recent purchaseID to be used for following items of the same lot if any exist
		if hasPurchasePrice(row):
			purchaseQuery = "INSERT INTO " + purcTable + " (Date_Purchased, Amount, ItemID) VALUES (STR_TO_DATE('" + row[0] + "', '%Y-%m-%d')," + row[2] + ", " + itemID + ");"
			purcID = str(runQuery(purchaseQuery)[1])
		
		if isSold(row):
			saleQuery = "INSERT INTO " + saleTable + " (Date_Sold, Amount, ItemID) VALUES (STR_TO_DATE('" + row[5] + "', '%Y-%m-%d')" + ", " + row[3] + ", " + itemID + ");"
			saleID = str(runQuery(saleQuery)[1])

		if hasPackingDims(row):
			ttlWeight, l, w, h = getShippingDims(row)
			shipQuery = "INSERT INTO " + shipTable + " (Length, Width, Height, Weight, ItemID, Notes) VALUES (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + itemID + ", \"" + row[11] + "\");"
			shipID = str(runQuery(shipQuery)[1])

		updateItemIDs(itemID, purcID, saleID, shipID)
	