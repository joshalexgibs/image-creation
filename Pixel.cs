using System;

namespace System.Drawing
{
  class Pixel
  {
    private static byte[] _image;
    private static int axisX;
    private static int axisY;

    public static int Main(string[] args)
    {
      if (args.Length < 3)
        Console.WriteLine("Enter 'box', 'circle' or 'dup' followed by width and length.");
      else
      {
        bool pass_x = Int32.TryParse(args[1], out axisX);
        bool pass_y = Int32.TryParse(args[2], out axisY);

        if (!pass_x || !pass_y)
          Console.WriteLine("Please enter a number for width and length.");
        else if (args[0] == "box")
        {
          redBox();
        }
        else if (args[0] == "circle")
        {
          circle();
        }
        else if(args[0] == "dup")
        {
          axisX = (int) (axisX * 1.2);
          axisY = (int) (axisY * 1.2);
          duplicant();
        }
        else
        {
          Console.WriteLine("Invalid input entered.");
          Console.WriteLine("Enter 'box', 'circle' or 'dup' followed by width and length.");
          return 0;
        }
        saveImg($"{args[0]}.png");
      }
      return 0;
    }
    
    static void sizeSet(int area)
    {
      _image = new byte[area];
    }
    
    static void pixSet(int pos, byte blue, byte green, byte red, byte alpha)
    {
      _image[pos] = blue;
      _image[pos + 1] = green;
      _image[pos + 2] = red;
      _image[pos + 3] = alpha;
    }
    
    unsafe static void saveImg(string saveLoc)
    {
      fixed (byte* ptr = _image)
      {
        Bitmap image = new Bitmap(axisX, axisY, axisY * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
        image.Save(@$"{saveLoc}");
        Console.WriteLine("Created {0} in current directory.", saveLoc);
      }
    }
    
    unsafe static void redBox()
    {
      //  { b, g, r  , opacity }
      //  { 0, 0, 255, 255 }
      int area = (int) (axisX * axisY * 4);
      sizeSet(area);
      for (int x = 0; x < area; x+=4)
      {
        pixSet(x, 0, 0, 255, 255);
      }
    }
    
    unsafe static void circle()
    {
      int area = axisX * axisY * 4;
      int halfX = axisX / 2;
      int halfY = axisY / 2;
      int radius = (int) Math.Pow(halfX - 1, 2);
      double upLim = axisX * (1 + ((axisX/1000.0) - 0.02));//1.08 at 100, 1 at 20
      double lowLim = axisX * (0.25 - (axisX/3500.0 + 0.04));//0.18 at 100, 0.25 at 20
      Console.WriteLine("up: {0}, low: {1}", upLim, lowLim);
      sizeSet(area);
      for (int y = 0; y < axisY; y++)
      {
        for(int x = 0; x < axisX; x++)
        {
          int offset = (y * axisX * 4) + (x * 4);
          float z = radius - (x - halfX + 1) * (x - halfX) - (y - halfY + 1) * (y - halfY);
          if (z < upLim && z > lowLim)
          {
            //Console.WriteLine("Debug: {0}, {1}", x, y);
            pixSet(offset, 0, 0, 255, 255);
          }
          else if ((x == halfX - 1 || x == halfX) && (y == halfY - 1 || y == halfY))
          {
            pixSet(offset, 255, 0, 0, 255);
          }
          else
          {
            pixSet(offset, 255, 255, 255, 255);
          }
        }
      }
    }

    unsafe static void duplicant()
    {
      int area = axisX * axisY * 4;
      sizeSet(area);
      int splitX = (int) (axisX / 2.4);
      Console.WriteLine(splitX);
      int halfX = splitX / 2;
      int halfY = (int) (axisY / 2.4);
      int xTwo = (int) (halfX * 1.4);
      Console.WriteLine(xTwo);
      int yTwo = (int) (halfY * 1.2);
      int radiusFirst = (int) Math.Pow(halfX - 1, 2);
      int radiusSecond = (int) Math.Pow(xTwo - 1, 2);
      Console.WriteLine(Math.Sqrt(radiusFirst));
      Console.WriteLine(Math.Sqrt(radiusSecond));
      double upLimFirst = splitX * (1 + ((splitX / 1000.0) - 0.02)); //1.08 at 100, 1 at 20
      double lowLimFirst = splitX * (0.25 - (splitX / 3500.0 + 0.04)); //0.18 at 100, 0.25 at 20
      double upLimSecond = (splitX * 1.2) * (1 + ((splitX * 1.2 / 1000.0) - 0.02)); //1.08 at 100, 1 at 20
      double lowLimSecond = (splitX * 1.2) * (0.25 - (splitX * 1.2 / 3500.0 + 0.04)); //0.18 at 100, 0.25 at 20
      for (int y = 0; y < axisY; y++)
      {
        for (int x = 0; x < axisX; x++)
        {
          int offset = (int) (y * axisX * 4) + (x * 4);
          float z = radiusFirst - (x - halfX + 1) * (x - halfX) - (y - halfY + 1) * (y - halfY);
          float b = radiusSecond - (x - xTwo + 1 - splitX) * (x - xTwo - splitX) - (y - yTwo + 1) * (y - yTwo);
          if (z < upLimFirst && z > lowLimFirst)
          {
            //Console.WriteLine("Debug: {0}, {1}", x, y);
            pixSet(offset, 0, 0, 255, 255);
          }
          //else if ((x == halfX - 1 || x == halfX) && (y == halfY - 1 || y == halfY))
          //{
          //  pixSet(offset, 255, 0, 0, 255);
          //}
          else if (b < upLimSecond && b > lowLimSecond)
          {
            pixSet(offset, 255, 0, 0, 255);
          }
          else
          {
            pixSet(offset, 255, 255, 255, 255);
          }
        }
      }
    }
  }
}
