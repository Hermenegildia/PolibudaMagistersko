using GestureKinectTools.MathTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools
{
   public static class GoldenSectionExtensions
    {
       public static float Length(this List<Vector2> points)
       {
           float length = 0;

           for (int i = 1; i < points.Count; i++)
           {
               length += (points[i - 1] - points[i]).Length;
           }

           return length;
       }

       public static Vector2 Center(this List<Vector2> points)
       {
           Vector2 result = points.Aggregate(Vector2.Zero, (current, point) => current + point);

           result /= points.Count;

           return result;
       }

       public static List<Vector2> Rotate(this List<Vector2> positions, float angle)
       {
           List<Vector2> result = new List<Vector2>(positions.Count);
           Vector2 c = positions.Center();

           float cos = (float)Math.Cos(angle);
           float sin = (float)Math.Sin(angle);

           foreach (Vector2 p in positions)
           {
               float dx = p.X - c.X;
               float dy = p.Y - c.Y;

               Vector2 rotatePoint = Vector2.Zero; //todo: Ala rozrysować!!
               rotatePoint.X = dx * cos - dy * sin + c.X;
               rotatePoint.Y = dx * sin + dy * cos + c.Y;

               result.Add(rotatePoint);
           }

           return result;
       }

       public static Rectangle BoundingRectangle(this List<Vector2> points)
       {
           float minX = points.Min(p => p.X);
           float maxX = points.Max(p => p.X);
           float minY = points.Min(p => p.Y);
           float maxY = points.Max(p => p.Y);

           return new Rectangle(minX, minY, maxX - minX, maxY - minY);
       }

       public static void ScaleToReferenceWorld(this List<Vector2> positions)
       {
           Rectangle boundungRectangle = positions.BoundingRectangle();

           for (int i = 0; i < positions.Count; i++)
           {
               Vector2 position = positions[i];
               position.X *= (1.0f / boundungRectangle.Width);
               position.Y *= (1.0f / boundungRectangle.Height);

               positions[i] = position;
           }
       }

       public static void CenterToOrigin(this List<Vector2> positions)
       {
           Vector2 center = positions.Center();

           for (int i = 0; i < positions.Count; i++)
           {
               positions[i] -= center;
           }
       }

       // Average distance betweens paths
       public static float DistanceTo(this List<Vector2> path1, List<Vector2> path2)
       {
           return path1.Select((t, i) => (t - path2[i]).Length).Average();
       }

       public static bool IsLargeEnough(this List<Vector2> positions, float minSize)
       {
           Rectangle boundingRectangle = positions.BoundingRectangle();

           return boundingRectangle.Width > minSize && boundingRectangle.Height > minSize;
       }
    }
}
