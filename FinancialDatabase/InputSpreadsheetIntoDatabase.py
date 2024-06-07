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
		runQuery(modifiedItemQuery)
	if saleID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET SaleID = "     + saleID + " WHERE ITEM_ID = " + itemID + ";"
		runQuery(modifiedItemQuery)
	if shipID != "":
		modifiedItemQuery = "UPDATE " + itemTable + " SET ShippingID = " + shipID + " WHERE ITEM_ID = " + itemID + ";"
		runQuery(modifiedItemQuery)

	return

def formatRow(row):
	for i, elem in enumerate(row):
		row[i] = row[i].replace("\"", "\\\"")
	return row

def inputIntoDatabase(data):
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

		#Default - Item entry and purchase entry
		
		itemQuery = "INSERT INTO " + itemTable + " (Name) VALUES  (\"" + row[1] + "\");"
		itemID = str(runQuery(itemQuery)[1])

		purchaseQuery = "INSERT INTO " + purcTable + " (Date_Purchased, Amount, ItemID) VALUES (STR_TO_DATE('" + row[0] + "', '%Y-%m-%d')," + row[2] + ", " + itemID + ");"
		purcID = str(runQuery(purchaseQuery)[1])
		
		saleID = ""
		shipID = ""
		if boolIsSold:
			saleQuery = "INSERT INTO " + saleTable + " (Date_Sold, Amount, ItemID) VALUES (STR_TO_DATE('" + row[5] + "', '%Y-%m-%d')" + ", " + row[3] + ", " + itemID + ");"
			saleID = str(runQuery(saleQuery)[1])

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
			shipID = str(runQuery(shipQuery)[1])
		
		updateItemIDs(itemID, purcID, saleID, shipID)




with open('Tool Buys - Sheet1_FMT.csv') as csvfile:
	CSVSheet = list(csv.reader(csvfile, delimiter=',', escapechar = '\\', quoting = csv.QUOTE_NONE, lineterminator = '\r\r\n'))
	#for row in CSVSheet:
	#	print(row)
	inputIntoDatabase(CSVSheet)
	