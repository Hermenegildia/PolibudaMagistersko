using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Kinect;
using GestureKinectTools.MathTools;



namespace GestureKinectTools.Gestures
{
    public abstract class GestureDetector
    {
        public int MinimalPreiodBetweenGestures { get; set; }
        readonly List<Entry> entries;
        public event Action<string> OnGestureDetected;
        DateTime lastGestureDate;
        readonly int iterationsCount;   //number of recorded positions

        public Canvas DisplayCanvas { get; set; }
        public Color DisplayColor { get; set; }

        protected List<Entry> Entries
        {
            get { return entries; }
        }

        public int IterationsCount
        {
            get { return iterationsCount; }
        }


        protected GestureDetector(int iterationsCount = 20)
        {
            this.iterationsCount = iterationsCount;
            MinimalPreiodBetweenGestures = 0;
            DisplayColor = Colors.Red;

            lastGestureDate = DateTime.Now;
            entries = new List<Entry>();
        }

        public virtual void AddEntry(SkeletonPoint position, KinectSensor sensor)
        {
            Entry newEntry = new Entry() { Position = position.ToVector3(), Time = DateTime.Now };
            entries.Add(newEntry);

            //draw
            if (this.DisplayCanvas != null)
            {
                newEntry.DisplayEllipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    StrokeThickness = 2.0,
                    Stroke = new SolidColorBrush(this.DisplayColor),
                    StrokeLineJoin = PenLineJoin.Round
                };

                Vector2 vector2 = Tools.Convert(sensor, position);

                float x = (float)(vector2.X*DisplayCanvas.ActualWidth);
                float y = (float)(vector2.Y*DisplayCanvas.ActualHeight);

                Canvas.SetLeft(newEntry.DisplayEllipse, x - newEntry.DisplayEllipse.Width / 2);
                Canvas.SetTop(newEntry.DisplayEllipse, y - newEntry.DisplayEllipse.Height / 2);

                DisplayCanvas.Children.Add(newEntry.DisplayEllipse);
            }

            //remove old positions
            if (entries.Count > this.iterationsCount)
            {
                Entry entryToRemove = entries[0];
                
                if (DisplayCanvas != null)
                {
                    DisplayCanvas.Children.Remove(entryToRemove.DisplayEllipse);
                }

                entries.Remove(entryToRemove);
            }

            LookForGesture();
        }

       protected abstract void LookForGesture();

       protected void RaiseGestureDetected(string gesture)
       {
           //odstep czasowy
           if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPreiodBetweenGestures) 
           {
               if (OnGestureDetected != null)
                   OnGestureDetected(gesture);

               lastGestureDate = DateTime.Now;
           }

           foreach (Entry entry in entries)
           {
               if (DisplayCanvas != null)
                   DisplayCanvas.Children.Remove(entry.DisplayEllipse);
           }

           entries.Clear();
       }

    }
}
