from Connection.CSVImporter.InputSpreadsheetIntoDatabase import clearDatabase, inputIntoDatabase
import csv


noAnswer = True
boolClearDatabase = False


while(noAnswer):
	boolClearDatabase = input("Clear Database? (Y/N)")
	
	acceptableAnswers = ['Y', 'y', 'N', 'n']
	if any(x in boolClearDatabase for x in acceptableAnswers):
		noAnswer = False




noFile = True
DEFAULTFILE = "Connection\CSVImporter\Tool Buys - Sheet1_FMT.csv"
while(noFile):
	f = None
	fileName = input("CSV File to Import (or use 'D' or 'DEFAULT'): ")
	acceptableAnswers = ['D', 'd', 'DEFAULT', 'defualt', 'Default']
	if any(x in fileName for x in acceptableAnswers):
		fileName = DEFAULTFILE
		noFile = False
		continue
	try:
		f = open(fileName)
	except:
		print("Error: Invalid File")
		continue
	else:
		noFile = False
		f.close()


	
	
with open(fileName) as csvfile:
	if boolClearDatabase:
		clearDatabase()
	CSVSheet = list(csv.reader(csvfile, delimiter=',', escapechar = '\\', quoting = csv.QUOTE_NONE, lineterminator = '\r\r\n'))
	inputIntoDatabase(CSVSheet)
	