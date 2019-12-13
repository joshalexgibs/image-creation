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
      byte[] redCir = new byte[1600];
      for(int y = 0; y < 20; y++) // width: 20px
      {
        //int offset = y * 20;
        for(int x = 0; x < 20; x++) // height: 20px * 4 bytes
        {
          int offset = (y * 20) + (x * 4);
          if((y) == 20 - (x))
          {
            Console.WriteLine("Debug: {0}, {1}", x, y);
            redCir[offset] = 0;
            redCir[offset + 1] = 0;
            redCir[offset + 2] = 255;
            redCir[offset + 3] = 255;
          }
          else
          {
            redCir[offset] = 255;
            redCir[offset + 1] = 255;
            redCir[offset + 2] = 255;
            redCir[offset + 3] = 0;
          }
        }
      }
      fixed (byte* ptr = redCir)
      {
        Bitmap image = new Bitmap(20, 20, 20 * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
        image.Save(@"redCircle.png");
      }
    }
  }
}
