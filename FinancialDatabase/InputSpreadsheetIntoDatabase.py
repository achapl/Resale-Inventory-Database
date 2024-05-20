import Pyscript.py

"""
A few cases
	
	- Bought Not Sold
		- Item entry
		- Purhcase entry
	- Bought Not Sold, Packaged
		- Item entry
		- Purchase entry
		- Packaging Dims entry (Make this table)
			- Item ID, weight(ttl oz, over-estimation), l,w,h
	- Bought     Sold, Packaged
		- Same as above
		- Sold entry """

# Given a row from input, determine if it has been
	# 1 - Bought, Not Sold
	# 2 - Bought, Not Sold, Packaged
	# 3 - Bought     Sold, Packaged
def getItemStatus(row):
	case = -1
	hasPackingDims = (row[7] != '')
	isSold		   = (row[3] != '')

	if !hasPackingDims and !isSold:
		case = 1
	elif hasPackingDims and !isSold:
		case = 2
	elif hasPackingDims and isSold:
		case = 3

	return case

def inputIntoDatabase
	
