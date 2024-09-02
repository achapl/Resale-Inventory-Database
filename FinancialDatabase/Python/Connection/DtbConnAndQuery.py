import mysql.connector
import sys


errorCount = 0



def search():
	return

def runQuery(query):
	# user: testuser, pass: testuser
	cnx = mysql.connector.connect(user='testuser', password='testuser', host='127.0.0.1', database='tool_database')
	cnx.autocommit=True
	cursor = cnx.cursor()
	cnx.rollback()
	result = None
	colNames = None
	try:
		result = cursor.execute(query)
		# cursor.execute(query) only returns first item. fetchall() will collect remaining items
		# only use fetchall()?
		#result = [result, cursor.fetchall()]
		result = cursor.fetchall()

		# If any changes (Create, Update, Delete) have been perforned, try and commit them to the database
		try:
			cnx.commit()
		except mysql.connector.errors.InternalError:
			print("Error: Unread Result")
		# Unneeded? main program works. If the database input from CSV works as well, remove this
			#retStr = ""
		#if result is not None:
		#	retStr = str(result)

		# Get column names
		if cursor.description is not None:
			colNames = [i[0] for i in cursor.description]

		cursor.close()
		cnx.close()
	except Exception as e:
		global errorCount
		errorCount += 1
		print("!!!ERROR!!! " + str(e), file=sys.stderr)
		print(query, file=sys.stderr)
		print("", file=sys.stdout)
		return ["ERROR"], [-1], [-1]
	return result, colNames, cursor.lastrowid