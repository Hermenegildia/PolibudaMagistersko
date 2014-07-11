﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Kinect.Toolbox.Gestures.Learning_Machine;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Text;
using System.IO;

namespace Kinect.Toolbox
{
    [Serializable]
    public class RecordedPath //szablon, template
    {
        List<Vector2> points;
        readonly int samplesCount;
        [NonSerialized]
        WriteableBitmap displayBitmap;
        

        public List<Vector2> Points
        {
            get { return points; }
            set { points = value; }
        }

        //public List<VecWithHand> PointsWithHand
        //{
        //    get { return pointsWithHand; }
        //    set { pointsWithHand = value; } //todo: Ala tutaj zrobić, ze dodawanie do pointsWithHand będzie dodawało do Points i sortowało też??
        //}

        public WriteableBitmap DisplayBitmap
        {
            get
            {
                if (displayBitmap == null)
                {
                    displayBitmap = new WriteableBitmap(200, 140, 96.0, 96.0, PixelFormats.Bgra32, null);

                    byte[] buffer = new byte[displayBitmap.PixelWidth * displayBitmap.PixelHeight * 4];

                    foreach (Vector2 point in points)
                    {
                        int scaleX = (int)((point.X + 0.5f) * displayBitmap.PixelWidth);
                        int scaleY = (int)((point.Y + 0.5f) * displayBitmap.PixelHeight);

                        for (int x = scaleX - 2; x <= scaleX + 2; x++)
                        {
                            for (int y = scaleY - 2; y <= scaleY + 2; y++)
                            {
                                int clipX = Math.Max(0, Math.Min(displayBitmap.PixelWidth - 1, x));
                                int clipY = Math.Max(0, Math.Min(displayBitmap.PixelHeight - 1, y));
                                int index = (clipX + clipY * displayBitmap.PixelWidth) * 4;

                                buffer[index] = 255;
                                buffer[index + 1] = 0;
                                buffer[index + 2] = 0;
                                buffer[index + 3] = 255;
                            }
                        }
                    }

                    displayBitmap.Lock();

                    int stride = displayBitmap.PixelWidth * displayBitmap.Format.BitsPerPixel / 8;
                    Int32Rect dirtyRect = new Int32Rect(0, 0, displayBitmap.PixelWidth, displayBitmap.PixelHeight);
                    displayBitmap.WritePixels(dirtyRect, buffer, stride, 0);
                    //displayBitmap.AddDirtyRect(dirtyRect);

                    displayBitmap.Unlock();
                }

                return displayBitmap;
            }
        }

        public RecordedPath(int samplesCount)
        {
            this.samplesCount = samplesCount;
            points = new List<Vector2>();
        }

        public void CloseAndPrepare()
        {
            points = GoldenSection.Pack(points, samplesCount);
        }

        public bool Match(List<Vector2> positions, float threshold, float minimalScore, float minSize)
        {
            if (positions.Count < samplesCount)
                return false;

            if (!positions.IsLargeEnough(minSize))
                return false;

            List<Vector2> locals = GoldenSection.Pack(positions, samplesCount);

            //SavePointsToFile(locals, "locals");
            //SavePointsToFile(points, "current");


            float score = GoldenSection.Search(locals, points, -MathHelper.PiOver4, MathHelper.PiOver4, threshold);

            return score > minimalScore;
        }

        private static void SavePointsToFile(List<Vector2> pointsList, string fileName)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            StringBuilder sbX = new StringBuilder();
            StringBuilder sbY = new StringBuilder();

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
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\" + fileName +"_y.txt", true))
            {
                writer.Write(sbY.ToString());
            }
        }
    }
}
