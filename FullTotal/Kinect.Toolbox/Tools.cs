using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Kinect;
using System.Text;
using System.Windows;
using Microsoft.Kinect.Toolkit.Controls;


namespace Kinect.Toolbox
{
    public static class Tools
    {
        public static List<Vector2> ToListOfVector2(this JointCollection joints)
        {
            return joints.Select(j => j.Position.ToVector2()).ToList();
        }

        public static Vector3 ToVector3(this SkeletonPoint vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
        public static Vector2 ToVector2(this SkeletonPoint vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
        public static void Write(this BinaryWriter writer, SkeletonPoint vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }
        public static SkeletonPoint ReadVector(this BinaryReader reader)
        {
            SkeletonPoint result = new SkeletonPoint
                                {
                                    X = reader.ReadSingle(),
                                    Y = reader.ReadSingle(),
                                    Z = reader.ReadSingle()
                                };

            return result;
        }

        public static bool TrySetElevationAngle(this KinectSensor camera, int angle)
        {
            bool success = false;
            try
            {
                camera.ElevationAngle = angle;
                success = true;
            }
            catch { }
            return success;
        }

        public static void GetSkeletons(this SkeletonFrame frame, ref Skeleton[] skeletons)
        {
            if (frame == null)
                return;

            if (skeletons == null || skeletons.Length != frame.SkeletonArrayLength)
            {
                skeletons = new Skeleton[frame.SkeletonArrayLength];
            }
            frame.CopySkeletonDataTo(skeletons);
        }

        public static Skeleton[] GetSkeletons(this SkeletonFrame frame)
        {
            if (frame == null)
                return null;

            var skeletons = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo(skeletons);

            return skeletons;
        }

        public static Vector2 Convert(KinectSensor sensor, SkeletonPoint position)
        {
            float width = 0;
            float height = 0;
            float x = 0;
            float y = 0;

            if (sensor.ColorStream.IsEnabled)
            {
                CoordinateMapper cm = new CoordinateMapper(sensor);
                var colorPoint = cm.MapSkeletonPointToColorPoint(position, sensor.ColorStream.Format);
                x = colorPoint.X;
                y = colorPoint.Y;

                switch (sensor.ColorStream.Format)
                {
                    case ColorImageFormat.RawYuvResolution640x480Fps15:
                    case ColorImageFormat.RgbResolution640x480Fps30:
                    case ColorImageFormat.YuvResolution640x480Fps15:
                        width = 640;
                        height = 480;
                        break;
                    case ColorImageFormat.RgbResolution1280x960Fps12:
                        width = 1280;
                        height = 960;
                        break;
                }
            }
            else if (sensor.DepthStream.IsEnabled)
            {
                CoordinateMapper cm = new CoordinateMapper(sensor);
                var depthPoint =cm.MapSkeletonPointToDepthPoint(position, sensor.DepthStream.Format);
                x = depthPoint.X;
                y = depthPoint.Y;

                switch (sensor.DepthStream.Format)
                {
                    case DepthImageFormat.Resolution80x60Fps30:
                        width = 80;
                        height = 60;
                        break;
                    case DepthImageFormat.Resolution320x240Fps30:
                        width = 320;
                        height = 240;
                        break;
                    case DepthImageFormat.Resolution640x480Fps30:
                        width = 640;
                        height = 480;
                        break;
                }
            }
            else
            {
                width = 1;
                height = 1;
            }

            return new Vector2(x / width, y / height);
        }


        //moje dopisane do zapisu do pliku
        public static void SavePointsToFile(List<Vector2> pointsList, string fileName)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            StringBuilder sbX = new StringBuilder();
            StringBuilder sbY = new StringBuilder();

            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_x.txt")) //wyczysc plik
            {
                writer.Write(string.Empty);
            }
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_y.txt"))
            {
                writer.Write(string.Empty);
            }

            //sb.AppendLine("next vector " + DateTime.Now.ToString());
            foreach (Vector2 point in pointsList)
            {
                sbX.AppendLine(point.X.ToString(System.Globalization.CultureInfo.InvariantCulture));// + " y: " + point.Y.ToString());
                sbY.AppendLine(point.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));

            }
            //sbX.AppendLine();
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_x.txt", true))
            {
                writer.Write(sbX.ToString());
            }
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_y.txt", true))
            {
                writer.Write(sbY.ToString());
            }
        }

        public static void SavePointsToFile(List<RecordedPath> recordedPathsList, string fileName)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_x.txt")) //wyczysc plik
            {
                writer.Write(string.Empty);
            }
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_y.txt"))
            {
                writer.Write(string.Empty);
            }

            //sb.AppendLine("next vector " + DateTime.Now.ToString());
            foreach (RecordedPath path in recordedPathsList)
            {
                StringBuilder sbX = new StringBuilder();
                StringBuilder sbY = new StringBuilder();

                //sb.AppendLine("next vector " + DateTime.Now.ToString());
                foreach (Vector2 point in path.Points)
                {
                    sbX.AppendLine(point.X.ToString(System.Globalization.CultureInfo.InvariantCulture));// + " y: " + point.Y.ToString());
                    sbY.AppendLine(point.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));

                }
                //sbX.AppendLine();
                using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_x.txt", true))
                {
                    writer.Write(sbX.ToString());
                }
                using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName + "_y.txt", true))
                {
                    writer.Write(sbY.ToString());
                }
            }
        }

        public static Point GetJointPoint(KinectSensor sensor, FrameworkElement control, SkeletonPoint jointPosition) //sciagniete z beginning_kinect_programming
        {
            CoordinateMapper cm = new CoordinateMapper(sensor);
            DepthImagePoint point = cm.MapSkeletonPointToDepthPoint(jointPosition, sensor.DepthStream.Format);

            //przerobione z  (int)control.ActualHeight / sensor.DepthStream.FrameHeight na (control.ActualHeight / sensor.DepthStream.FrameHeight)
            double pointX = (double)point.X;
            pointX *= (control.ActualWidth / sensor.DepthStream.FrameWidth);
            double pointY = (double)point.Y;
            pointY *= (control.ActualHeight / sensor.DepthStream.FrameHeight);

            return new Point(pointX, pointY);
        }

      
    }
}
