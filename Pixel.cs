using System;

namespace System.Drawing
{
  class Pixel
  {
    private static byte[] _image;
    private static int axis;

    public static int Main(string[] args)
    {
      if (args.Length < 2)
        Console.WriteLine("Enter 'box', 'circle', 'dup', 'circleRad' or 'dupRad' followed by a number.");
      else
      {
        bool pass_x = Int32.TryParse(args[1], out axis);
        if (!pass_x)
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
          axis = (int) (axis * 1.2);
          duplicant();
        }
        else if (args[0] == "circleRad")
        {
          circleRadian();
        }
        else if (args[0] == "dupRad")
        {
          axis = (int)(axis * 1.2);
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
        //                       width, length, bit span, Image formatting routine
        Bitmap image = new Bitmap(axis, axis, axis * 4, Imaging.PixelFormat.Format32bppArgb, new IntPtr(ptr));
        image.Save(@$"{saveLoc}");
        Console.WriteLine("Created {0} in current directory.", saveLoc);
      }
    }
    
    unsafe static void redBox()
    {
      //  { b, g, r  , opacity }
      //  { 0, 0, 255, 255 }
      int area = (int) (Math.Pow(axis, 2) * 4);
      sizeSet(area);
      for (int x = 0; x < area; x+=4)
      {
        pixSet(x, 0, 0, 255, 255);
      }
    }
    
    unsafe static void circle()
    {
      int area = (int) (Math.Pow(axis, 2) * 4);
      sizeSet(area);
      int halfX = axis / 2;
      int halfY = axis / 2;
      int radius = (int) Math.Pow(halfX - 1, 2);
      Console.WriteLine("Center: {0}, {1}", halfX, halfY);
      double upLim = axis * (1 + ((axis/1000.0) - 0.02));//1.08 at 100, 1 at 20
      double lowLim = axis * (0.25 - (axis/3500.0 + 0.04));//0.18 at 100, 0.25 at 20
      Console.WriteLine("up: {0}, low: {1}", upLim, lowLim);
      for (int y = 0; y < axis; y++)
      {
        double yT = (5) + (70 - halfX+5)*(y/12.5);
        double xT = (70) - (5 - halfY+5)*(y/12.5);
        for(int x = 0; x < axis; x++)
        {
          int offset = (y * axis * 4) + (x * 4);
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

      int area = (int) (Math.Pow(axis, 2) * 4);
      int halfX = axis / 2;
      int halfY = axis / 2;
      sizeSet(area);
      int radius = halfX - 1;
      for(int fill = 0; fill < area; fill+=4)
      {
        pixSet(fill, 255, 255, 255, 255);
      }
      int max = (int) (3.15 * axis);
      int div = max / 2;
      drawRadians(radius, halfX, halfY, axis);
    }

    //legacy version kept for circleRad()
    static void tangentOld(double xT, double yT, int xCenter, int yCenter)
    {
      double slope = (-xT + xCenter) / (yT - yCenter);
      for (int x = (int) xT; x < axis; x++)
      {
        int y = (int) (slope * (x - xT) + yT);
        int offset = (int) ((y * axis * 4) + (x * 4));
        if (y < axis && y >= 0)
          pixSet(offset, 255, 0, 0, 255);
        else
          break;
      }
    }

    //using x values to dictate start and stop locations of tangent drawing
    static void tangent(int xTanStart, int yTanStart, int xTanStop, int yTanStop)
    {
      double denom = xTanStart - xTanStop;
      double nomin = yTanStart - yTanStop;
      double slope = nomin / denom;
      Console.WriteLine("slope: {0}", slope);
      //(y - yTan)/(yIntercept - yTan) = (x - xTan)/(xIntercept - xTan)
      Tuple<int, int> dictate;
      if (xTanStart < xTanStop)
        dictate = Tuple.Create(xTanStart, xTanStop);
      else
        dictate = Tuple.Create(xTanStop, xTanStart);
      for (int x = dictate.Item1; x < dictate.Item2; x++)
      {
        int y = (int) ((slope * (x - xTanStart) + yTanStart) + 0.7);
        int offset = (int) ((y * axis * 4) + (x * 4));
        pixSet(offset, 255, 0, 0, 255);
      }
    }

    unsafe static void duplicant()
    {
      int area = (int) (Math.Pow(axis, 2) * 4);
      sizeSet(area);
      int splitX = (int) (axis / 2.4);
      Console.WriteLine(splitX);
      int halfX = splitX / 2;
      int halfY = (int) (axis / 2.4);
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
      for (int y = 0; y < axis; y++)
      {
        for (int x = 0; x < axis; x++)
        {
          int offset = (int) (y * axis * 4) + (x * 4);
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

    //rewrite and pare down
    unsafe static void dupRad()
    {
      int area = (int) (Math.Pow(axis, 2) * 4);
      sizeSet(area);
      int smallHalf = (int) (axis / 2.4);
      int bigHalf = axis - smallHalf;

      double xCenterOne = (smallHalf / 2) + 0.5;
      double yCenterOne = (axis / 2.4) + 0.5;
      double xCenterTwo = (bigHalf / 2) + smallHalf + 0.5;
      double yCenterTwo = (axis / 2) + 0.5;
      Console.WriteLine($"CenterOne: ({xCenterOne}, {yCenterOne})");
      Console.WriteLine($"CenterTwo: ({xCenterTwo}, {yCenterTwo})");

      int radiusOne = (int) (xCenterOne - 1);
      int radiusTwo = (int) Math.Ceiling(xCenterTwo - ((xCenterOne + 1) * 2));
      Console.WriteLine("radiusOne: {0}, radiusTwo: {1}", radiusOne, radiusTwo);
      
      //assuming radiusOne < radiusTwo
      double xIntercept = ((xCenterOne * radiusTwo) - (xCenterTwo * radiusOne)) / (radiusTwo - radiusOne);
      double yIntercept = ((yCenterOne * radiusTwo) - (yCenterTwo * radiusOne)) / (radiusTwo - radiusOne);

      /*(xIntercept - xCenter) and (yIntercept - yCenter)*/
      double xPartOne = eqPart(xIntercept, xCenterTwo);
      double yPartOne = eqPart(yIntercept, yCenterTwo);

      double xPartTwo = eqPart(xIntercept, xCenterOne);
      double yPartTwo = eqPart(yIntercept, yCenterOne);
      
      /*creates xTanOne, xTanTwo, yTanOne, and yTanTwo for use in this function
      flip tan outputs and 'part' inputs for finding y values*/
      //First circle 
      tanLoc(radiusTwo, xPartOne, yPartOne, xCenterTwo, out int xTanOne, out int xTanTwo);
      tanLoc(radiusTwo, yPartOne, xPartOne, yCenterTwo, out int yTanTwo, out int yTanOne);

      //Second circle
      tanLoc(radiusOne, xPartTwo, yPartTwo, xCenterOne, out int xTanThree, out int xTanFour);
      tanLoc(radiusOne, yPartTwo, xPartTwo, yCenterOne, out int yTanFour, out int yTanThree);

      var carrierOne = tanTuple(xTanOne, xTanTwo, yTanOne, yTanTwo, xCenterTwo, yCenterTwo);
      var carrierTwo = tanTuple(xTanThree, xTanFour, yTanThree, yTanFour, xCenterOne, yCenterOne);

      //repetitive, can use tuple to make one single call
      bool dirOne = directioner(xCenterOne, xCenterTwo, radiusOne, radiusTwo);
      bool dirTwo = directioner(xCenterTwo, xCenterOne, radiusTwo, radiusOne);

      //draw blank canvas
      for (int fill = 0; fill < area; fill += 4)
      {
        pixSet(fill, 255, 255, 255, 255);
      }
      
      drawRadians(radiusOne, xCenterOne, yCenterOne, axis, carrierOne, dirOne);
      drawRadians(radiusTwo, xCenterTwo, yCenterTwo, axis, carrierTwo, dirTwo);

      tangent(xTanThree, yTanThree, xTanOne, yTanOne);
      tangent(xTanFour, yTanFour, xTanTwo, yTanTwo);

      /* For tracking tangent line intercept points
      int offsetOne = (int)((yTanThree * 4 * axis) + (xTanThree * 4));
      int offsetTwo = (int)((yTanOne * 4 * axis) + (xTanOne * 4));
      int offsetThree = (int)((yTanTwo * 4 * axis) + (xTanTwo * 4));
      int offsetFour = (int)((yTanFour * 4 * axis) + (xTanFour * 4));

      pixSet(offsetOne, 0, 0, 0, 255);
      pixSet(offsetTwo, 0, 0, 0, 255);
      pixSet(offsetThree, 0, 0, 0, 255);
      pixSet(offsetFour, 0, 0, 0, 255);*/
    }

    static void drawRadians(int radius, int xCenter, int yCenter, int span)
    {
      double spanner = span * 1.2;
      int max = (int)(3.15 * spanner);
      int div = max / 2;
      for (int degree = 0; degree < max; degree++)
      {
        double radians = (degree * Math.PI / div) - 0.2;
        double x = (xCenter + radius * Math.Cos(radians));
        double y = (yCenter + radius * Math.Sin(radians));
        int offset = (((int)y * axis * 4) + ((int)x * 4));
        /*if (x == xTanOne && y == yTanOne)
        {
          tangentOld(x, y, xCenter, yCenter);
        }*/
        pixSet(offset, 0, 0, 255, 255);
      }
    }
    /*overload
    Note: plotting of circles occurs in Q4, not Q1*/
    //redundant code, pare down
    static void drawRadians(int radius, double xCenter, double yCenter, int span,
                            Tuple<double, double> carrier, bool direction)
    {
      // max dictates number of times for loop paints a pixel, affecting line width/shape
      double max = 2.1 * span;
      double div = max / 2;
      double ratio = max / 360;
      double startDeg = findDegree(carrier.Item1) * ratio;
      double maxDeg = findDegree(carrier.Item2) * ratio;
      if (direction)
      {
        for (double degree = startDeg; degree <= maxDeg; degree++)
        {
          double radians = (degree * Math.PI / div);
          int x = (int) (xCenter + radius * Math.Cos(radians));
          int y = (int)(yCenter + radius * Math.Sin(radians));
          int offset = (y * axis * 4) + (x * 4);
          pixSet(offset, 0, 0, 255, 255);
        }
      }
      else {
        /* to print left facing, startDeg must be bigger than maxDeg*/
        startDeg += max;
        for (double degree = startDeg; degree >= maxDeg; degree--)
        {
          double radians = (degree * Math.PI / div);
          int x = (int) (xCenter + radius * Math.Cos(radians));
          int y = (int) (yCenter + radius * Math.Sin(radians));
          int offset = (y * axis * 4) + (x * 4);
          pixSet(offset, 0, 0, 255, 255);
        }
      }
    }

    //returns carrier containing the bounds of what drawRadians() should draw.
    static Tuple<double, double> tanTuple(int xFirst, int xSecond,
                                          int yFirst, int ySecond,
                                          double xCenter, double yCenter)
    {
      double radOne = findRadian(xFirst, yFirst, xCenter, yCenter);
      double radTwo = findRadian(xSecond, ySecond, xCenter, yCenter);
      if (radOne > radTwo)
        return Tuple.Create(radTwo, radOne);
      else
        return Tuple.Create(radOne, radTwo);
    }
    
    static double findDegree(double radian)
    {
      double degree = (radian * 180 / Math.PI);
      Console.WriteLine($"degree: {degree}");
      return degree;
    }
    
    static double findRadian(int xFirst, int yFirst, double xCenter, double yCenter)
    {
      double deltaX = xFirst - xCenter;
      double deltaY = yFirst - yCenter;
      double radian = Math.Atan2(deltaY, deltaX);
      Console.WriteLine($"radian: {radian}");
      return radian;
    }

    static double eqPart(double intercept, double center)
    {
      return intercept - center;
    }

    static void tanLoc(int radius, double xPart, double yPart, double center,
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
      tanOne = (int) (totalPos + center);
      tanTwo = (int) (totalNeg + center);
    }

    /*directioner() decides whether a circle is left or right facing
      Returns True for right facing, or returns False for left facing*/
    static bool directioner(double centerOne, double centerTwo, int radiusOne, int radiusTwo)
    {
      int circleOne = (int) (centerOne + radiusOne);
      int circleTwo = (int) (centerTwo + radiusTwo);
      if (circleOne > circleTwo)
        return true;
      else
        return false;
    }
  }
}
