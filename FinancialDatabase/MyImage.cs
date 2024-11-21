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
}
