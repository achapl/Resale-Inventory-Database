
eos = "EOS"
import mysql.connector
import sys


def getArgs():
	
	numArgs = len(sys.argv)
	argStr = ""

	for i in range(1,numArgs):
		argStr += sys.argv[i] + " "
	
	return argStr


def formatArgs(argStr):
	escapeChar = "^"
	formatArgString = argStr.replace(escapeChar, '')

	return formatArgString


def getQuery():
	args = getArgs()
	query = formatArgs(args)
	return query


def runQuery(query):
	print(query)
	# user: testuser, pass: testuser
	cnx = mysql.connector.connect(user='testuser', password='testuser', host='127.0.0.1', database='world')
	cursor = cnx.cursor()
	cnx.rollback()
	cursor.execute("USE tool_database; SELECT * FROM item;")#query)
	cnx.commit()
	retStr = ""
	for row in cursor:
		retStr = retStr + "\n" + row
	cursor.close()
	cnx.close()
	return retStr

"""
runQuery(getQuery())

#print each row in cursor:
for row in cursor:
	print(row)

print(eos)
cursor.close()

cnx.close()"""