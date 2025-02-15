from signal import raise_signal
from PIL import Image
from Connection.DtbConnAndQuery import runQuery
from ResizeDatabaseImages import imageToThumbnail
import os
import time
import shutil

#parentDir = "C:\\Users\\Owner\\Desktop\\InsertImagesTest\\"
parentDir = "C:\\Users\\Owner\\source\\repos\\FinancialDatabaseSolution\\FinancialDatabase\\Resources\\Selling"
parentDirSingleSlash = parentDir.replace('\\\\','\\')

imageDir = "C:/ProgramData/MySQL/MySQL Server 8.0/Uploads/"

def getFolders():
    folderWalk  = os.walk(parentDir)
    folders = []
    for (root, dirs, files) in folderWalk:
        folders.append(root.removeprefix(parentDir))
    folders = folders[1:]
    return folders

# Return an int of the position
def getFirstInstanceOfChar (char:chr,string:str) -> int:
    if len(string) == 0:
        return -9999
    else:
        if string[0] == char:
           return 0
        else:
            # End of string, could not find char
            if len(string) == 1:
                return -9999
            else:
                return getFirstInstanceOfChar(char, string[1:]) + 1

# Get position of the dash in the folder name
def  getDashPos (fileName : str):
    return getFirstInstanceOfChar('-', fileName)


# Get the number associated with the folder name
def  getFolderItemNum (fileName : str):
    dashPos = getDashPos(fileName)
    if dashPos <= 5 and dashPos >= 0:
        ret = fileName[:dashPos - 1]
        return ret
    else:
        return "DNE"
    

# Get map <fileName,shipNum>
def getFolderNumbersMap():
    return [[getFolderItemNum(f), f] for f in getFolders()]


# Get list of images in the folder
def getFolderContents(folder):
    return os.listdir(parentDirSingleSlash + folder)


# take parentDir+fileName, move to MySql folder
def copyFolderContents (folder:str): 
    files = getFolderContents(folder)
    for file in files:
        filePath = parentDirSingleSlash + folder + "\\" + file
        destPath = imageDir + file
        shutil.copyfile(filePath, destPath)

def getItemID(shipNum:int):
    shipNum = runQuery("SELECT ITEM_ID FROM item WHERE (Notes_item LIKE '%Orig Shipping Info Number: " + str(shipNum) + ",%' OR Notes_item LIKE '%Orig Shipping Info Number: " + str(shipNum) + "');", False)[0]
    if len(shipNum) != 1:
        return -1
    return int(shipNum[0][0])

runQuery("UPDATE item SET thumbnailID = NULL;", False)
runQuery("DELETE FROM image;", False)
runQuery("DELETE FROM thumbnail;", False)

# Clear the mySQL upload directory
for root, dirs, files in os.walk(imageDir):
    if root == imageDir:
        continue
    for f in files:
        os.unlink(os.path.join(root, f))
    for d in dirs:
        shutil.rmtree(os.path.join(root, d))
map1 = getFolderNumbersMap()
for [shipNum, folder] in map1:
    pics = getFolderContents(folder)
    copyFolderContents(folder)

map2 = getFolderNumbersMap()
for [shipNum, folder] in map2:
    pics = getFolderContents(folder)
    itemID = getItemID(shipNum)
    print("ShipNum: " + str(shipNum) + "\n")
    if itemID != -1:
        for pic in pics:
            runQuery("INSERT INTO image (image, ItemID) VALUES (LOAD_FILE('" + imageDir + pic + "'), " + str(itemID) +");", False)    
        imageToThumbnail(itemID)
        
