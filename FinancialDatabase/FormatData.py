import csv
import datetime
import re

#   Date,   Name,   Cost Bought,    Sold Val,   Profit Made,    Sold Date,  Shipping Info,  Notes 

# If 'Notes' containes any letter other than l,b,s,o, or z (lbs, oz), it is a textual note, not shipping dimensions
# Shipping info is unique purchaced item number


# Lengthen year month and day to instead of 3-5-20 to be 03-05-2020
def formatMDY(date):
    if (len(date) == 3):
        if len(date[2]) == 2:
                    date[2] = "20" + date[2]
        if len(date[1]) == 1:
            date[1] = "0" + date[1]
        if len(date[0]) == 1:
            date[0] = "0" + date[0]
    elif len(date) == 2:
        if len(date[1]) == 1:
            date[1] = "0" + date[1]
        if len(date[0]) == 1:
            date[0] = "0" + date[0]
    return date

def reformatToYMD(date):
    
    if len(date) == 3:
        date = [date[2], date[0], date[1]]
    
    if len(date) == 2:
        date = ["NOYEAR", date[0], date[1]]
    return date

def dateHasYear(date):
    return date[0] != "NOYEAR"

def dateIsNextYear(oldDate, newDate):
    oldMonth = oldDate.month
    newMonth = int(newDate[1])
    return newMonth < oldMonth

def incrYear(date):
    return datetime.date(date.year+1, date.month, date.day)


"""
Col A - Date
        - Give date do each purchase
        - Default is 1/1/20
        - Ignore any '?' for simplicity and lump it in with last date given """
def assignDate(Sheet):
    
    NOYEAR = "NOYEAR"
    lastDate = datetime.date(2020,1,1)
    
    for i in range(len(Sheet)):
        row = Sheet[i]
        # Replace '/' with '-'
        row[0] = row[0].replace('/', '-')
        date = row[0].split('-')
        date = formatMDY(date)
        date = reformatToYMD(date)


        # No Date, use last-used date
        if row[0] == "" or row[0] == "?" or row[0] == "(BLANK)" or row[0] == "Date" or row[0] == " ":
            row[0] = lastDate.__str__()

        # if date is format  04-11 (when real date is 04-11-2024)
        elif date[0] == NOYEAR:

            #Check if new year started, ie, month of current row is less than previous entry's month
            if dateIsNextYear(lastDate, date):
                #increment year
                lastDate = incrYear(lastDate)

            row[0] = str(lastDate.year) + '-' + date[1] + '-' + date[2]
            date = row[0].split('-')
            lastDate = datetime.date(lastDate.year, int(date[1]), int(date[2]))

        # if date is MDY format 04-11-2023, rearrange to YMD, and reset lastDate
        elif row[0].count("-") == 2:
            row[0] = date[0] + '-' + date[1] + '-' + date[2]
            lastDate = datetime.date(int(date[0]),int(date[1]), int(date[2]))
        else:
            print("UNFORSEEN CASE!!!!!!!!!!")
            print("Date: " + date.join())

    return Sheet

"""
Col B - Name
        - Input correct escape characters for special character such as "     """

def formatName(Sheet):
    for i in range(len(Sheet)):
        row = Sheet[i]
        name = row[1]
        row[1] = row[1].replace("\"", "\\\"")
    return Sheet

"""
Col F - Sold Date """
def formatSoldDate(Sheet):
    # Defaults
    NOYEAR = "NOYEAR"
    lastDate = datetime.date(2020,1,1)
    
    for i in range(len(Sheet)):
        row = Sheet[i]
        # Replace '/' with '-'
        row[5] = row[5].replace('/', '-')
        date = row[5].split('-')
        date = formatMDY(date)
        date = reformatToYMD(date)
        row[5] = '-'.join(date)

        # if date is format  04-11 (when real date is 04-11-2024)
        if date[0] == NOYEAR:
            print("UNFORSEEN SOLD DATE CASE!!!!!!!!!!")
            print(date)

    return Sheet

# Determine case of shippping info formatting
def getShipInfoFormatType(shipInfo):
    """
    Types: 1 = ""
           2 - "251"
           3 - "251A"
           4 - "251-A"   """
    caseNum = 0

    if shipInfo == "":
        return 1;

    try:
        int(shipInfo)
        return 2;
    except:
        pass
    
    try:
        # if last char is not a number, with no dashes found it is  case 3
        if shipInfo == "116E":
            pass
        if len(shipInfo) > 1 and shipInfo.count('-') == 0:
            int(shipInfo[len(shipInfo)-1])
            return 3
    except:
        pass

    if shipInfo.count('-') == 1:
        return 4

    if shipInfo == 0:
        print("UNKNOWN SHIP INFO FORMAT!!!")
        print(shipInfo)

    return caseNum
    
def isInt(num):
    try:
        int(num)
        return True
    except:
        return False


# AAAB -> AAAC, and ABZZ -> ACAA, and ZZ -> AAA
def incrementString(st):
    #Base Case
    if type(st) == 'NoneType' or st == '':
        return 'A'    

    l = len(st)
    
    # Edge case, string of length 1 will be incremented with nothing given in argument to prepend it to
    st2 = ""
    if l > 1:
        st2 = st[0:l-1]

    #Increment Carryover
    lastChar = st[l-1]
    if lastChar == 'Z':
        return incrementString(st2) + 'A'
    
    #Increment No Carryover
    else:
        return st2 + str(chr(ord(lastChar)+1))
                
    

"""

Col G - Shipping info (inventory index)

and

Col J - Old Shipping Num
        - Col G will be copied over to col J as a backup, so the original shipping-info/inventory-index numbers are kept"""

def shippingInfo(Sheet):
    bearingSublot="A"
    for row in Sheet:
        # Copy Col G into col J
        if row[6] != '':
            temp = ''
            if row[9] != '':
                temp = ", " + row[9]
            row[9] = row[6] + temp
        shipInfo = row[6]
        formatType = getShipInfoFormatType(shipInfo)
        
        # Case 4 ("251-A") are already well-formatted, don't have to change it
        match formatType:
            case 1:
                continue
            case 2: # "251"
                row[6] = row[6] + "-A-1"
                
            case 3: # "251A" -> "251-A"
                j = row[6].rfind('-')
                row[6] = row[6][:j-1] + "-" + row[6][j:]

        sp = row[6].split('-')
        if int(sp[0]) in [236,237,248]  or int(sp[0]) in range(251,266):
            sp[0] = '236'
            sp[1] = bearingSublot
            
            bearingSublot = incrementString(bearingSublot)
        row[6] = '-'.join(sp)
    return Sheet


def notesFormat(note, compRegExp):
    #Most characters: "###lbs (###lbs, ##oz), ###x###x###"
    return compRegExp.search(note)

"""
Col H - Notes (will be weight/shipping dims)
        - Can either be weight/dims or misc notes
        - Weight/dims will be of the format "##lbs (##lbs, ##oz), ##x##x##"
        - Put notes in I. Notes may also be found in I and J
        - All rote numbers in I are starting prices. Rote numbers found in rows 355-450 (may be off by 1 removing top column-naming row), refer to initial quantity in the sublot
Col I - Starting Price and Init Qty
        - If starting price, lop in with Notes
        - Starting price may end with A (ie: 0.99A), this means start at 99 cents for an auction """
def seperateNotes(Sheet):
    compRegExp = re.compile("[0-9]+(lbs?|oz)")
    actualWeight = re.compile("\(.*\)")
    for row in Sheet:
        # If found just a text note, copy them into the end of cell for col I of that row
        if not compRegExp.search(row[7]):
            # Copy over notes from H into I
            if row[7] != '':
                row[8] = row[8] + ',' + row[7]
                row[7] = ''

        # If weight/dims is found, cut and paste over any acutual weight in parens to the notes
        else:
            if actualWeight.search(row[7]) != None:
               s = actualWeight.search(row[7])
               row[8] = row[8] + ", Actual Weight: " + row[7][s.start():s.end()]
               row[7] = row[7][0:s.start()] + row[7][s.end():]
        # Copy over notes from J into I
        if row[9] != '':
            row[8] = row[9] + "," + row[8]
            row[9] = ''
            
    return Sheet


def recurFormatWeightDims(dims, hadPrevComma):
    if dims == "":
        return ""

    if isInt(dims[0]) or dims[0] == '.':
        if recurFormatWeightDims(dims[1:], False) is None:
            pass
        return dims[0] + recurFormatWeightDims(dims[1:], False)
    elif hadPrevComma:
        return recurFormatWeightDims(dims[1:], True)
    else:
        if len(dims) >= 3 and dims[0:3] == "lbs":
            if "oz" not in dims:
                return ",," + recurFormatWeightDims(dims[3:], True)
            return "," + recurFormatWeightDims(dims[3:], True)
        if len(dims) >= 2 and dims[0:2] == "lb":
            if "oz" not in dims:
                return ",," + recurFormatWeightDims(dims[2:], True)
            return "," + recurFormatWeightDims(dims[2:], True)
        if len(dims) >= 2 and dims[0:2] == "oz":
            return "," + recurFormatWeightDims(dims[2:], True)
        if len(dims) >=1 and dims[0] in ["x","X"]:
            return "," + recurFormatWeightDims(dims[1:], True)

def formatWeightDims(Sheet):
    for row in Sheet:
        row[7] = recurFormatWeightDims(row[7].strip(), False)
    return Sheet


def deleteExtraneous2(Sheet):
    l = [['A'],['B'],['']*3,['C'],['D']]
    for i in l:
        if i[0:2] == ['']*2:
            l.remove(i)
    print(l)
    return Sheet

def deleteExtraneous(Sheet):
    Sheet = [i for i in Sheet if i[1] != '']

    Sheet = [row[0:9] for row in Sheet]
    return Sheet



with open('Tool Buys - Sheet1.csv') as csvfile:

    # Open and extract raw data into Sheet
    CSVSheet = csv.reader(csvfile, delimiter=',', quotechar='"')
    Sheet = []
    for row in CSVSheet:
        if row[0] == "Date" or row[0] == "(BLANK)":
            continue
        Sheet.append(row)

    # Format data in Sheet
    Sheet = assignDate(Sheet)
    Sheet = formatName(Sheet)
    Sheet = formatSoldDate(Sheet)
    Sheet = shippingInfo(Sheet)
    Sheet = seperateNotes(Sheet)
    Sheet = formatWeightDims(Sheet)
    Sheet = deleteExtraneous(Sheet)

    # Write formatted Sheet to file
    with open('Tool Buys - Sheet1_FMT.csv', 'x') as csvfileFMT:
        CSVWriter = csv.writer(csvfileFMT, delimiter = ',', quotechar='\'')
        for row in Sheet:
            CSVWriter.writerow(row)


"""
After: 0:Date | 1:Name | 2: Cost Bought | 3: SoldVal | 4: Profit Made | 5: Sold Date | 6: New Ship Num | 7: Weight/Dims | 8: Notes - Old Ship Num, ColJ Orig notes, Col I Orig Notes

Col A - Date
        - Give date do each purchase
        - Default is 1/1/20
        - Ignore any '?' for simplicity and lump it in with last date given
Col B - Name
        - Make seperate table for taxes/fees
Col C - Cost Bought
Col D - Sold Price
        - May involve math for shipping price. Math is not shown, use the given sold price as the sold price anyways, IE ignore this statement.
Col E - Profit
        - Check where there is no "profit" entry when there is a sold val entry
Col F - Sold Date
Col G - Shipping info (inventory index)
        - Note: 251 will mean lot, 251-A will mean a grouping of same items for a lot, 251-A-1 will be first item of grouping in 251-A
        - Individual purchaced items will be considered 1 item lots
        - Given 251 in the orig CSV file (a 1 item lot, ie: an individual item),will change to 251-A-1
        - Given 251-A in the orig CSV file (a group of similar items in a lot purchsaed), will remain unchanged
Col H - Notes (will be weight/shipping dims)
        - Can either be weight/dims or misc notes
        - Weight/dims will be of the format "##lbs (##lbs, ##oz), ##x##x##"
        - Put notes in I. Notes may also be found in I and J
        - All rote numbers in I are starting prices. Rote numbers found in rows 355-450 (may be off by 1 removing top column-naming row), refer to initial quantity in the sublot
Col I - Starting Price and Init Qty
        - If starting price, lop in with Notes
        - Starting price may end with A (ie: 0.99A), this means start at 99 cents for an auction 

Col J - Old Shipping Num
        - Col G will be copied over to col J as a backup, so the original shipping-info/inventory-index numbers are kept

Delete all columns after K




"""

