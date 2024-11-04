open System.IO


let parentDir = "C:\\Users\\Owner\\Desktop\\Selling"

let imageDir = "C:/ProgramData/MySQL/MySQL Server 8.0/Uploads/"

let getFolders =
 Directory.GetDirectories(parentDir) |> Array.map Path.GetFileName


// Return an int of the position
let rec getFirstInstanceOfChar (chr:char) (str : string)  = 
    match str.Length with
    | 0 -> -99999999
    | _ ->
        if str[0] = chr then 0 else 
            match str.Length with
            | 1 -> -99999999
            | _ -> (getFirstInstanceOfChar chr str[1..] ) + 1

// Get position of the dash in the folder name
let getDashPos (fileName : string) =  getFirstInstanceOfChar '-' fileName


// Get the number associated with the folder name
let getFileNum (fileName : string) =
    do printf("Dash Pos: %d\n") (getDashPos fileName)
    match getDashPos fileName with
    | 4 -> fileName[..2]
    | 3 -> fileName[..1]
    | 2 -> fileName[..0]
    | _ -> "DNE"
    

// Get map <fileName,shipNum>
let getFolderNumbersMap = 
    dict [for f in getFolders -> f, getFileNum f]


// Get list of images in the folder
let getFolderContents folder = 
    Directory.GetFiles(folder) |> Array.map Path.GetFileName


// take parentDir+fileName, move to MySql folder
let copyFolderContents (folder:string) = 
    let files = getFolderContents folder
    for file in files do
        let filePath = folder + file
        let destPath = imageDir + file
        File.Copy(filePath, destPath)


        // Run "SELECT * FROM item WHERE notes LIKE "*Orig Shipping Info Number: shipNum(followed by endofstring or a non-num character so that "...Number: 110 doesn't catch as 11)*"
        
            // If multiple rows returned, output shipping number, do nothing, but this shouldn't ever happen

    // Run "INSERT INTO image (image, itemID) (GET_FILE(currDirName+fileName),)




// For each image in the folder

    