from DtbConnAndQuery import runQuery
import csv

itemTable = "item"
purcTable = "purchase"
saleTable = "sale"
shipTable = "shipping"

# Determine if sold by checking if it has a sold date
# This prevents lot headers (purchase: "Lot" which covers purchase of all items below) which have a net sold price but no date, from being inputted into the sales table
def isSold(row):
	return (row[5] != '')

def hasPackingDims(row):
	return (row[7] != '')

# Given a table where you just added a row, return the primary key for that row
#
#
#
### DELETE OR FIND BETTER WAY TO EXTRCT THIS FUNCTIONALITY FROM THE DebConnAndQuery.py runQuery() functionality
# Probably need to return the lastID with the runQuery func, since it closes the connection after each query, so that connection's LAST_INSERT_ID is lost.
#
#
def getLastID(table):
	query = "SELECT LAST_INSERT_ID() from " + table + ";"
	return runQuery(query)

def updateItemIDs(itemID, purcID, saleID, shipID):

	if purcID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET PurchaseID = " + purcID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)
		if result[0] == "!!!ERROR!!!":
			print("!!!ERROR!!!")
			print(modifiedItemQuery)
	else:
		print("ERROR, NO PURC_ID for ITEM_ID: " + itemID)

	if saleID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET SaleID = "     + saleID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)
		if result[0] == "!!!ERROR!!!":
			print("!!!ERROR!!!")
			print(modifiedItemQuery)

	if shipID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET ShippingID = " + shipID + " WHERE ITEM_ID = " + itemID + ";"
		result = runQuery(modifiedItemQuery)
		if result[0] == "!!!ERROR!!!":
			print("!!!ERROR!!!")
			print(modifiedItemQuery)

	return

def formatRow(row):
	for i, elem in enumerate(row):
		row[i] = row[i].replace("\"", "\\\"")
	return row

def inputIntoDatabase(data):
	purcID = ""
	for index, row in enumerate(data):
		row = formatRow(row)
		prevRow = None
		nextRow = None
		if index > 0:
			prevRow = data[index-1]
		if index < len(data) - 1:
			nextRow = data[index+1]

		#for prevRow, row, nextRow in data:
		boolIsSold		   = isSold(row)
		boolhasPackingDims = hasPackingDims(row)		
		
		currQuantity = 0
		if boolIsSold:
			currQuantity = 0
		else:
			currQuantity = 1
		
		initQuantity = 0
		if row[10] == "":
			initQuantity = 1
		else:
			initQuantity = row[10]

		#Default - Item entry
		itemQuery = "INSERT INTO " + itemTable + " (Name, InitialQuantity, CurrentQuantity, Notes) VALUES  (\"" + row[1] + "\"" + ", " + str(initQuantity) + ", " + str(currQuantity) + ", " + "\"" + row[8] + "\"" + ");" # Note: Change current quantity later based on small_sales.
		itemID = str(runQuery(itemQuery)[1])

		# If it is the purchase of a new lot or single item lot, insert that purchase into the database, and
		# update the most recent purchaseID to be used for following items of the same lot if any exist
		hasPurchasePrice = row[2] != ""
		if hasPurchasePrice:
			purchaseQuery = "INSERT INTO " + purcTable + " (Date_Purchased, Amount, ItemID) VALUES (STR_TO_DATE('" + row[0] + "', '%Y-%m-%d')," + row[2] + ", " + itemID + ");"
			result = runQuery(purchaseQuery)
			if result[0] == "!!!ERROR!!!":
				print("!!!ERROR!!!")
				print("For: " + row[1])
				print(purchaseQuery)
			purcID = str(result[1])
		
		saleID = ""
		shipID = ""
		if boolIsSold:
			saleQuery = "INSERT INTO " + saleTable + " (Date_Sold, Amount, ItemID) VALUES (STR_TO_DATE('" + row[5] + "', '%Y-%m-%d')" + ", " + row[3] + ", " + itemID + ");"
			result = runQuery(saleQuery)
			if result[0] == "!!!ERROR!!!":
				print("!!!ERROR!!!")
				print("For: " + row[1])
				print(saleQuery)
			saleID = str(result[1])

		if boolhasPackingDims:
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

			ttlWeight = str(int(lbs)*ouncesPerPound + int(oz))

			shipQuery = "INSERT INTO " + shipTable + " (Length, Width, Height, Weight, ItemID) VALUES (" + l + ", " + w + ", " + h + ", " + ttlWeight + ", " + itemID + ");"
			result = runQuery(shipQuery)
			if result[0] == "!!!ERROR!!!":
				print("!!!ERROR!!!")
				print("For: " + row[1])
				print(shipQuery)
			shipID = str(result[1])

		updateItemIDs(itemID, purcID, saleID, shipID)




with open('Tool Buys - Sheet1_FMT.csv') as csvfile:
	CSVSheet = list(csv.reader(csvfile, delimiter=',', escapechar = '\\', quoting = csv.QUOTE_NONE, lineterminator = '\r\r\n'))
	#for row in CSVSheet:
	#	print(row)
	inputIntoDatabase(CSVSheet)
	