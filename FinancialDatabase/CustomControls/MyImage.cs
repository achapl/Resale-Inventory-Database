using System.Windows.Forms.VisualStyles;

public class MyImage
{
	public Image image { get; set; }
	public int imageID { get; set; }
    public int thumbnailID { get; set; }
	public MyImage(Image image, int imageID)
	{
		this.image = image;
		this.imageID = imageID;
        this.thumbnailID = Util.DEFAULT_INT;
	}

    public MyImage(Image image, int imageID, int thumbnailID)
    {
        this.image = image;
        this.imageID = imageID;
        this.thumbnailID = thumbnailID;
    }

    public MyImage(List<string> imageAttribs, List<string> colNames)
    {
        if (imageAttribs.Count() != colNames.Count())  throw new Exception("Error: Creating MyImage with different # attributes and colNames");


        for (int i = 0; i < colNames.Count; i++)
        {
            string imageAttrib = imageAttribs[i];
            string colName = colNames[i];

            // Missing info, skip
            if (imageAttrib.CompareTo("None") == 0)
            {
                imageAttrib = null!;
            }

            setAttribute(imageAttrib, colName);
        }
    }


    private void setAttribute(string imageAttrib, string colName)
    {
        switch (colName)
        {
            // From item table
            case "IMAGE_ID":
                this.imageID = Int32.Parse(imageAttrib);
                break;
            case "image":
                this.image = Util.rawImageStrToImage(imageAttrib);
                break;
            case "thumbnailID":
                if (imageAttrib == null)
                {
                    this.thumbnailID = Util.DEFAULT_INT;
                    break;
                }

                this.thumbnailID = Int32.Parse(imageAttrib);
                break;
        }
    }
}
