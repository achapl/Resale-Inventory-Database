using System;

public class MyImage
{
	public Image image { get; set; }
	public int imageID { get; set; }
	public MyImage(Image image, int imageID)
	{
		this.image = image;
		this.imageID = imageID;
	}

    public MyImage(List<string> imageAttribs, List<string> colNames)
    {

        for (int i = 0; i < colNames.Count; i++)
        {
            string imageAttrib = imageAttribs[i];
            // Missing info, skip
            if (imageAttrib.CompareTo("None") == 0)
            {
                imageAttrib = null!;
            }
            switch (colNames[i])
            {
                // From item table
                case "IMAGE_ID":
                    this.imageID = Int32.Parse(imageAttrib);
                    break;
                case "image":
                    this.image = Util.rawImageStrToImage(imageAttrib);
                    break;
            }
        }
    }
}
