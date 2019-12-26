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
        Console.WriteLine("Enter 'box', 'circle', 'dup', 'circleRad' or 'dupRad' followed by width and length.");
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
        else if (args[0] == "circleRad")
        {
          circleRadian();
        }
        else if (args[0] == "dupRad")
        {
          axisX = (int)(axisX * 1.2);
          axisY = (int)(axisY * 1.2);
          dupRad();
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
        Bitmap image = new Bitmap(axisX, axisY, axisX * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
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
      sizeSet(area);
      int halfX = axisX / 2;
      int halfY = axisY / 2;
      int radius = (int) Math.Pow(halfX - 1, 2);
      Console.WriteLine("Center: {0}, {1}", halfX, halfY);
      double upLim = axisX * (1 + ((axisX/1000.0) - 0.02));//1.08 at 100, 1 at 20
      double lowLim = axisX * (0.25 - (axisX/3500.0 + 0.04));//0.18 at 100, 0.25 at 20
      Console.WriteLine("up: {0}, low: {1}", upLim, lowLim);
      for (int y = 0; y < axisY; y++)
      {
        double yT = (5) + (70 - halfX+5)*(y/12.5);
        double xT = (70) - (5 - halfY+5)*(y/12.5);
        for(int x = 0; x < axisX; x++)
        {
          int offset = (y * axisX * 4) + (x * 4);
          float z = radius - (x - halfX + 1) * (x - halfX) - (y - halfY + 1) * (y - halfY);
          if (z < upLim && z > lowLim)
          {
            pixSet(offset, 0, 0, 255, 255);
          }
          // tangent of circle at (70, 5)
          else if ((xT - yT) < x && (xT -yT) > (x - 2))
          {
            pixSet(offset, 255, 0, 255, 255);
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

    unsafe static void circleRadian()
    {
      // radians = degrees * pi / 180
      // x = xCenter + radius*cos(radians)
      // y = yCenter + radius*sin(radians)
      // radius is 49, center at (50,50)

      int area = axisX * axisY * 4;
      int halfX = axisX / 2;
      int halfY = axisY / 2;
      sizeSet(area);
      int radius = halfX - 1;
      for(int fill = 0; fill < area; fill+=4)
      {
        pixSet(fill, 255, 255, 255, 255);
      }
      int max = (int) (3.15 * axisX);
      int div = max / 2;
      drawRadians(radius, halfX, halfY, axisX);
    }

    static void tangentOld(double xT, double yT, int xCenter, int yCenter)
    {
      double slope = (-xT + xCenter) / (yT - yCenter);
      for (int x = (int) xT; x < axisX; x++)
      {
        int y = (int) (slope * (x - xT) + yT);
        int offset = (int) ((y * axisX * 4) + (x * 4));
        if (y < axisY && y >= 0)
          pixSet(offset, 255, 0, 0, 255);
        else
          break;
      }
    }

    static void tangent(double xTan, double yTan, int xCenter, int yCenter)
    {
      double nomin = xCenter - xTan;
      double denom = yTan - yCenter;
      double slope = nomin / denom;
      Console.WriteLine("slope: {0}", slope);
      //(y - yTan)/(yIntercept - yTan) = (x - xTan)/(xIntercept - xTan)
      for (int x = 0; x < axisX; x++)
      {
        int y = (int) (slope * (x - xTan) + yTan);
        int offset = (int) ((y * axisX * 4) + (x * 4));
        if (y < axisY && y >= 0)
          pixSet(offset, 255, 0, 0, 255);
        else
          break;
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

    unsafe static void dupRad()
    {
      int area = axisX * axisY * 4;
      sizeSet(area);
      int splitX = (int) (axisX / 2.4);
      
      int xCenterOne = splitX / 2;
      int yCenterOne = (int) (axisY / 2.4);
      int xCenterTwo = (int) (xCenterOne * 1.4) + splitX;
      int yCenterTwo = (int) (yCenterOne * 1.2);
      Console.WriteLine("Center: ({0}, {1})", xCenterOne, yCenterOne);

      int radiusOne = xCenterOne - 1;
      int radiusTwo = (int) (radiusOne * 1.4);
      Console.WriteLine("radiusOne: {0}, radiusTwo: {1}", radiusOne, radiusTwo);


      //assuming radiusOne < radiusTwo
      double xIntercept = ((xCenterOne * radiusTwo) - (xCenterTwo * radiusOne)) / (radiusTwo - radiusOne);
      double yIntercept = ((yCenterOne * radiusTwo) - (yCenterTwo * radiusOne)) / (radiusTwo - radiusOne);
      Console.WriteLine("xInt: {0}, yInt: {1}", xIntercept, yIntercept);

      //write functions -> (xIntercept - xCenterOne) and (yIntercept - yCenterOne) basically
      double xPart = eqPart(xIntercept, xCenterTwo);
      double yPart = eqPart(yIntercept, yCenterTwo);
      Console.WriteLine("xPart: {0}, yPart: {1}\n", xPart, yPart);

      //creates xTanOne, xTanTwo, yTanOne, and yTanTwo for use in this function
      //flip tan outputs and 'part' inputs for finding y values
      tanLoc(radiusTwo, xPart, yPart, xCenterTwo, out int xTanOne, out int xTanTwo);
      tanLoc(radiusTwo, yPart, xPart, yCenterTwo, out int yTanTwo, out int yTanOne);

      Console.WriteLine("xTanOne: {0}", xTanOne);
      Console.WriteLine("xTanTwo: {0}", xTanTwo);
      Console.WriteLine("yTanOne: {0}", yTanOne);
      Console.WriteLine("yTanTwo: {0}", yTanTwo);

      //draw
      for (int fill = 0; fill < area; fill += 4)
      {
        pixSet(fill, 255, 255, 255, 255);
      }
      drawRadians(radiusOne, xCenterOne, yCenterOne, splitX);
      drawRadians(radiusTwo, xCenterTwo, yCenterTwo, (axisX/2 + 5));

      /*int offsetOne = (int) ((yTanOne * axisX * 4) + (xTanOne * 4));
      pixSet(offsetOne, 255, 0, 0, 255);
      
      int offsetTwo = (int) ((yTanTwo * axisX * 4) + (xTanTwo * 4));
      pixSet(offsetTwo, 255, 0, 0, 255);*/

      tangent(xTanOne, yTanOne, xCenterTwo, yCenterTwo);
      tangent(xTanTwo, yTanTwo, xCenterTwo, yCenterTwo);
    }

    static void drawRadians(int radius, int xCenter, int yCenter, int span)
    {
      int max = (int)(3.15 * span);
      int div = max / 2;
      /*Console.WriteLine("max: {0}, div: {1}", max, div);
      Console.WriteLine("xCenter: {0}, yCenter: {1}", xCenter, yCenter);
      int degTan = 115;
      int degTanTwo = 195;*/
      for (int degree = 0; degree < max; degree++)
      {
        double radians = (degree * Math.PI / div) - 0.2;
        double x = (xCenter + radius * Math.Cos(radians));
        double y = (yCenter + radius * Math.Sin(radians));
        int offset = (((int)y * axisX * 4) + ((int)x * 4));
        //Console.WriteLine("x: {0}, y: {1}, offset: {2}", x, y, offset);
        /*if (degree == degTan)
        {
          tangentOld(x, y, xCenter, yCenter);
        }
        else if (degree == degTanTwo)
        {
          tangentOld(x, y, xCenter, yCenter);
          break;
        }*/
        pixSet(offset, 0, 0, 255, 255);
      }
    }

    static double eqPart(double intercept, int center)
    {
      return intercept - center;
    }

    static void tanLoc(int radius, double xPart, double yPart, int center,
                             out int tanOne, out int tanTwo)
    {
      //Pow operations, first of the original tanLoc operations
      double xPartPow = Math.Pow(xPart, 2);
      double yPartPow = Math.Pow(yPart, 2);
      double radiusPow = Math.Pow(radius, 2);

      //calculation of identities to be affected by the +-
      double allSqrt = Math.Sqrt(xPartPow + yPartPow - radiusPow);
      double preSqrt = yPart * radius;
      double posNeg = preSqrt * allSqrt;
      
      //final equation prep
      double denom = xPartPow + yPartPow;
      double prepend = radiusPow * xPart;

      //final calculations
      double nominPos = prepend + posNeg;
      double nominNeg = prepend - posNeg;
      double totalPos = nominPos / denom;
      double totalNeg = nominNeg / denom;

      //assignment
      tanOne = (int) Math.Ceiling(totalPos + center);
      tanTwo = (int) Math.Ceiling(totalNeg + center);
    }
  }
}
