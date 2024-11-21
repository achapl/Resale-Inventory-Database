# Will resize all images in the database that are not resized to thumbnails,
# And insert those thumbnails into the images table in the database,
# With the original image's thumbnailID pointing to the new smaller thumbnail version

from asyncio.windows_events import NULL
from re import M
from Connection.DtbConnAndQuery import runQuery
from PIL import Image
import os
import io

imageDir = "C:/ProgramData/MySQL/MySQL Server 8.0/Uploads/"
maxDims = (300, 300)

f = None

# Get all image and image IDs, where there is no thumbnail ID

def getImagesAndData():
    imagesAndData, colNames, lastColID = runQuery("SELECT IMAGE_ID, image FROM image WHERE thumbnailID IS NULL OR thumbnailID = 0")
    return imagesAndData

# Convert bytearray to image
def byteArrToImage(byteArr):
    try:
        image = Image.open(io.BytesIO(byteArr))
    except:
        return NULL
    return image

# Resize Image

def resizeImage(image):
    if type(image) == type(1):
        pass
    image.thumbnail(maxDims)
    return image


def getImagePath(name, image):
    return imageDir + str(name) + "." + image.format

# Save the image
def saveImage(image, imagePath):
    image.save(imagePath)

# Insert new image into database / Get result ID
def insertImage(imagePath):
    a, b, lastRowID = runQuery("INSERT INTO thumbnail (thumbnail) VALUES (LOAD_FILE('" + imagePath + "'));")
    return lastRowID
    


# Update old image ID to reflect new thumbnail ID

def updateID(imageID, thumbnailID):
    query = "UPDATE image SET thumbnailID = " + str(thumbnailID) + " WHERE IMAGE_ID = " + str(imageID) + ";" 
    runQuery(query)


def imageIDToThumbnail(imageID):
    image = byteArrToImage(convertedIm)


def imageToThumbnail(itemID):
    # Modify next line to get 
    imagesAndData, colNames, lastColID = runQuery("SELECT IMAGE_ID, image FROM image WHERE ItemID = " + str(itemID) + ";")
    hasThumbnail = False
    count = 0
    for [imageID, byteImage] in imagesAndData:
        convertedIm = bytes(byteImage)
        imageUnresized = byteArrToImage(convertedIm)
        image = resizeImage(imageUnresized)
    
        imagePath = getImagePath(count, image)
        count += 1
        saveImage(image, imagePath)

        thumbnailID = insertImage(imagePath)
        insertImage(imagePath)
        updateID(imageID, thumbnailID)

        # Only give the thumbnail as the first image in the series
        if hasThumbnail == True:
            continue
        

        result,_,_  = runQuery("SELECT thumbnailID FROM item WHERE ITEM_ID = " + str(itemID) + ";")
        # Only 1 should be returned since itemID is unique. More than that would be an error
        if len(result) > 1:
            print("ERROR: Multiple Returns for single ITEM_ID: " + str(itemID))
            exit

        if len(result) == 0:
            print("ERROR: No Item Returned for ITEM_ID: " + str(itemID))
            exit

        oldThumbnailID = result[0][0]

        if oldThumbnailID is None:
            runQuery("UPDATE item SET thumbnailID = " + str(thumbnailID) + " WHERE ITEM_ID = " + str(itemID) + ";")

        hasThumbnail = True




"""runQuery("DELETE FROM thumbnail")

imagesAndData = getImagesAndData()

count = 0
for row in imagesAndData:
    imageID = row[0]
    byteImage = row[1]
    convertedIm = bytes(byteImage)

    image = byteArrToImage(convertedIm)
    if image == NULL:
        continue
    imageToThumbnail(image, imageID) 

    count += 1
    """