import mysql.connector
import sys


errorCount = 0



def convertImageData(result, colNames, f):
	f.write("Colnames: " + str(colNames) + "\n")
	if ("image" in colNames) or ("thumbnail" in colNames):
		f.write("Here1")
		index = 0
		if "image" in colNames:
			f.write(" image Index: " + str(colNames.index("image")))
			index = colNames.index("image")
		elif "thumbnail" in colNames:
			f.write(" thumbnail Index: " + str(colNames.index("thumbnail")))
			index = colNames.index("thumbnail")

		for row in result:
			if row[index] != None:
				f.write("Thumbnail: " + str(row[index]))
				row2 = [x for x in row[index]]
				row[index] = row2
	f.write("Here1.5")
	return result

def runQuery(query):


	f = open("C:/Users/Owner/Desktop/debug.txt", 'a')
	# user: testuser, pass: testuser
	cnx = mysql.connector.connect(user='testuser', password='testuser', host='127.0.0.1', database='tool_database', get_warnings=True)
	cnx.autocommit=True
	cursor = cnx.cursor()
	cnx.rollback()
	result = None
	colNames = None
	try:
		result0 = cursor.execute(query)
		result0 = list(cursor.fetchall())
		result = []
		for row in result0:
			result.append(list(row))

		# If any changes (Create, Update, Delete) have been perforned, try and commit them to the database
		try:
			cnx.commit()
		except mysql.connector.errors.InternalError:
			print("Error: Unread Result")
		# Get column names and num columns
		numCols = 0
		if cursor.description is not None:#if cursor.description is not None:
			colNames = [i[0] for i in cursor.description]#cursor.description]
			numCols = len(colNames)
		numItems = cursor.rowcount

		if colNames is not None:
			result = convertImageData(result, colNames, f)
		cursor.close()
		cnx.close()
		f.close()
	except Exception as e:
		f.close()
		cursor.close()
		cnx.close()
		global errorCount
		errorCount += 1
		print("!!!ERROR!!! " + str(e), file=sys.stderr)
		print(query, file=sys.stderr)
		print("", file=sys.stdout)
		return ["ERROR"], [e], [-1]
	return result, colNames, cursor.lastrowid