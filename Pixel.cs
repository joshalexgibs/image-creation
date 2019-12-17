using System;

namespace System.Drawing
{
  class Pixel
  {
    private static byte[] _image;
    private static int axisX;
    private static int axisY;

    public static void Main(string[] args)
    {
      if (args.Length < 3)
        Console.WriteLine("Enter 'box' or 'circle' followed by width and length.");
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
      }
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
      int area = axisX * axisY * 4;
      sizeSet(area);
      for (int x = 0; x < area; x+=4)
      {
        pixSet(x, 0, 0, 255, 255);
      }
      saveImg("redBox.png");
    }
    
    unsafe static void circle()
    {
      int area = axisX * axisY * 4;
      int halfX = axisX / 2;
      int halfY = axisY / 2;
      int radius = (int) Math.Pow(halfX - 1, 2);
      double upLim = axisX * (1 + ((axisX/1000.0) - 0.02));//1.08 at 100, 1 at 20
      double lowLim = axisX * (-0.25 + (axisX/1500.0 - 0.13));//0.18 at 100, 0.25 at 20
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
      saveImg("redCircle.png");
    }
  }
}
