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
        //Console.WriteLine("Created redBox.png in current directory.");
      }
      else if (args[0] == "circle")
      {
        circle();
        //Console.WriteLine("Created redCircle.png in current directory.");
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
      //fixed (byte* ptr = redPix)
      //{
      //              (width, height, stride * 4, format, pointer)
      //  Bitmap image = new Bitmap(10, 10, 10 * 4, Imaging.PixelFormat.Format32bppRgb, new IntPtr(ptr));
      //  image.Save(@"redBox.png");
      //}
      saveImg(redPix, 10, 10, 10, "redBox.png");
    }
    
    unsafe static void circle()
    {
      byte[] redCir = new byte[40000];
      for(double y = 0.0; y < 100; y++) // width: 20px
      {
        for(double x = 0.0; x < 100; x++) // height: 20px * 4 bytes
        {
          double offset = (y * 400) + (x * 4);
          double z = 1000 - (x - 50) * (x - 50) - (y - 50) * (y - 50);
          if (z < 60 && z > 1)
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
      //fixed (byte* ptr = redCir)
      //{
      //  Bitmap image = new Bitmap(100, 100, 100 * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
      //  image.Save(@"redCircle.png");
      //}
      saveImg(redCir, 100, 100, 100, "redCircle.png");
    }

    unsafe static void saveImg(byte[] img, int x, int y, int stride, string saveLoc)
    {
      fixed (byte* ptr = img)
      {
        Bitmap image = new Bitmap(x, y, stride * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
        image.Save(@$"{saveLoc}");
        Console.WriteLine("Created {0} in current directory.", saveLoc);
      }
    }
  }
}
