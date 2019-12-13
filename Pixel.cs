using System;

namespace System.Drawing
{
  class Pixel
  {
    public static void Main(string[] args)
    {
      if (args.Length == 0)
        Console.WriteLine("Enter 'box' or 'circle'.");
      else if (args[0] == "box")
      {
        redBox();
        Console.WriteLine("Created redBox.png in current directory.");
      }
      else if (args[0] == "circle")
      {
        circle();
        Console.WriteLine("Created redCircle.png in current directory.");
      }
    }
    unsafe static void redBox()
    {
      //  { b, g, r  , opacity }
      //  { 0, 0, 255, 255 }
      byte[] redPix = new byte[400];
      for(int x = 0; x < 400; x+=4)
      {
        redPix[x] = 0;
        redPix[x + 1] = 0;
        redPix[x + 2] = 255;
        redPix[x + 3] = 255;
      }
      fixed (byte* ptr = redPix)
      {
        //              (width, height, stride * 4, format, pointer)
        Bitmap image = new Bitmap(10, 10, 10 * 4, Imaging.PixelFormat.Format32bppRgb, new IntPtr(ptr));
        image.Save(@"redBox.png");
      }
    }
    unsafe static void circle()
    {
      byte[] redCir = new byte[40000];
      for(double y = 0.0; y < 100; y++) // width: 20px
      {
        //int offset = y * 20;
        for(double x = 0.0; x < 100; x++) // height: 20px * 4 bytes
        {
          double offset = (y * 400) + (x * 4);
          //Math.Pow(y + 10, 2) == (400 - Math.Pow(x + 10, 2))
          double z = 1000 - (x - 50) * (x - 50) - (y - 50) * (y - 50);
          if (z < 50 && z > 0.01)
          {
            //Console.WriteLine("Debug: {0}, {1}", x, y);
            redCir[(int) offset] = 0;
            redCir[(int) offset + 1] = 0;
            redCir[(int) offset + 2] = 255;
            redCir[(int) offset + 3] = 255;
          }
          else if ((x == 49 || x==50) && (y == 49 || y ==50))
          {
            redCir[(int)offset] = 255;
            redCir[(int)offset + 1] = 0;
            redCir[(int)offset + 2] = 0;
            redCir[(int)offset + 3] = 255;
          }
          else
          {
            redCir[(int) offset] = 255;
            redCir[(int) offset + 1] = 255;
            redCir[(int) offset + 2] = 255;
            redCir[(int) offset + 3] = 255;
          }
        }
      }
      fixed (byte* ptr = redCir)
      {
        Bitmap image = new Bitmap(100, 100, 100 * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
        image.Save(@"redCircle.png");
      }
    }
  }
}
