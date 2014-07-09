using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinect.Toolbox{
    public class TwoHandsTemplatedGestureDetector: GestureDetector
    {
        public float Epsilon { get; set; }
        public float MinimalScore { get; set; }
        public float MinimalSize { get; set; }
        readonly LearningMachine learningMachine;
        RecordedPath path;
        readonly string gestureName;

        readonly List<Entry> leftsEntries = new List<Entry>();
        readonly List<Entry> bothHandsEntries = new List<Entry>();


        protected List<Entry> LeftEntries
        {
            get { return leftsEntries; }
        }

        protected List<Entry> BothHandsEntries
        {
            get { return bothHandsEntries; }
        }


        public bool IsRecordingPath
        {
            get { return path != null; }
        }

        public LearningMachine LearningMachine
        {
            get { return learningMachine; }
        }

        public TwoHandsTemplatedGestureDetector(string gestureName, Stream kbStream, int windowSize = 120)
            : base(windowSize)
        {
            Epsilon = 0.035f;
            MinimalScore = 0.80f;
            MinimalSize = 0.1f;
            this.gestureName = gestureName;
            learningMachine = new LearningMachine(kbStream);
        }

        public override void Add(SkeletonPoint position, KinectSensor sensor)
        {
            //if (isRightHand)
            //    base.Add(position, sensor);
            //else
            //    AddLeft(position, sensor);

            //if (path != null)
            //{
            //    path.Points.Add(position.ToVector2());
            //}
        }

        private void JoinEntriesFromTwoHands()
        {
            bothHandsEntries.Clear();

            bothHandsEntries.AddRange(Entries);
            bothHandsEntries.AddRange(leftsEntries);
            //foreach (Entry entry in Entries)
            //    BothHandsEntries.Add(entry);

            //foreach (Entry entry in LeftEntries)
            //    BothHandsEntries.Add(entry);
        }

        //private void AddLeft(SkeletonPoint position, KinectSensor sensor) //obsługa lewej reki
        //{
        //    Entry newEntry = new Entry { Position = position.ToVector3(), Time = DateTime.Now };
        //    LeftEntries.Add(newEntry);

        //    // Drawing
        //    if (DisplayCanvas != null)
        //    {
        //        newEntry.DisplayEllipse = new Ellipse
        //        {
        //            Width = 4,
        //            Height = 4,
        //            HorizontalAlignment = HorizontalAlignment.Left,
        //            VerticalAlignment = VerticalAlignment.Top,
        //            StrokeThickness = 2.0,
        //            Stroke = new SolidColorBrush(DisplayColor),
        //            StrokeLineJoin = PenLineJoin.Round
        //        };

        //        Vector2 vector2 = Tools.Convert(sensor, position);

        //        float x = (float)(vector2.X * DisplayCanvas.ActualWidth);
        //        float y = (float)(vector2.Y * DisplayCanvas.ActualHeight);

        //        Canvas.SetLeft(newEntry.DisplayEllipse, x - newEntry.DisplayEllipse.Width / 2);
        //        Canvas.SetTop(newEntry.DisplayEllipse, y - newEntry.DisplayEllipse.Height / 2);

        //        DisplayCanvas.Children.Add(newEntry.DisplayEllipse);
        //    }

        //    // Remove too old positions
        //    if (LeftEntries.Count > WindowSize)
        //    {
        //        Entry entryToRemove = Entries[0];

        //        if (DisplayCanvas != null)
        //        {
        //            DisplayCanvas.Children.Remove(entryToRemove.DisplayEllipse);
        //        }

        //        LeftEntries.Remove(entryToRemove);
        //    }

        //    // Look for gestures
        //    LookForGesture();
        //}

        public new void Add(Skeleton skeleton, KinectSensor sensor)
        {
            SkeletonPoint rightPosition;

            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
            {
                rightPosition = skeleton.Joints[JointType.HandRight].Position;
                base.Add(rightPosition, sensor);

                if (path != null)
                {
                    path.Points.Add(rightPosition.ToVector2());
                }
            }
            //Entry rightEntry = new Entry { Position = rightPosition.ToVector3(), Time = DateTime.Now };
            //BothHandsEntries.Add(rightEntry);
            SkeletonPoint leftPosition;
            if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                leftPosition = skeleton.Joints[JointType.HandLeft].Position;
                Entry leftEntry = new Entry { Position = leftPosition.ToVector3(), Time = DateTime.Now };
                LeftEntries.Add(leftEntry);

                leftEntry.DisplayEllipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    StrokeThickness = 2.0,
                    Stroke = new SolidColorBrush(Colors.Blue),
                    StrokeLineJoin = PenLineJoin.Round
                };

                Vector2 leftVector2 = Tools.Convert(sensor, leftPosition);

                float xl = (float)(leftVector2.X * DisplayCanvas.ActualWidth);
                float yl = (float)(leftVector2.Y * DisplayCanvas.ActualHeight);

                Canvas.SetLeft(leftEntry.DisplayEllipse, xl - leftEntry.DisplayEllipse.Width / 2);
                Canvas.SetTop(leftEntry.DisplayEllipse, yl - leftEntry.DisplayEllipse.Height / 2);

                DisplayCanvas.Children.Add(leftEntry.DisplayEllipse);

                if (path != null)
                {
                    path.Points.Add(leftPosition.ToVector2());
                }
            }

            // Drawing
            //if (DisplayCanvas != null)
            //{
            //    rightEntry.DisplayEllipse = new Ellipse
            //    {
            //        Width = 4,
            //        Height = 4,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        VerticalAlignment = VerticalAlignment.Top,
            //        StrokeThickness = 2.0,
            //        Stroke = new SolidColorBrush(DisplayColor),
            //        StrokeLineJoin = PenLineJoin.Round
            //    };

            

            //Vector2 rightVector2 = Tools.Convert(sensor, rightPosition);

            //float xr = (float)(rightVector2.X * DisplayCanvas.ActualWidth);
            //float yr = (float)(rightVector2.Y * DisplayCanvas.ActualHeight);

            //Canvas.SetLeft(rightEntry.DisplayEllipse, xr - rightEntry.DisplayEllipse.Width / 2);
            //Canvas.SetTop(rightEntry.DisplayEllipse, yr - rightEntry.DisplayEllipse.Height / 2);

            //DisplayCanvas.Children.Add(rightEntry.DisplayEllipse);

            

            // Remove too old positions
            if (LeftEntries.Count > WindowSize / 2)
            {
                Entry entryToRemove = LeftEntries[0];

                if (DisplayCanvas != null)
                {
                    DisplayCanvas.Children.Remove(entryToRemove.DisplayEllipse);
                }

                LeftEntries.Remove(entryToRemove);
            }

            // Look for gestures
            LookForGesture();


            //if (path != null)
            //{
            //    path.Points.Add(rightPosition.ToVector2());
            //    path.Points.Add(leftPosition.ToVector2());
            //}
        }
        

        protected override void LookForGesture()
        {
            JoinEntriesFromTwoHands();
            if (LearningMachine.Match(BothHandsEntries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Epsilon, MinimalScore, MinimalSize))
                RaiseGestureDetected(gestureName);
        }

        public void StartRecordTemplate()
        {
            BothHandsEntries.Clear();
            path = new RecordedPath(WindowSize);
        }

        public void EndRecordTemplate()
        {
            LearningMachine.AddPath(path);
            path = null;
        }

        public void SaveState(Stream kbStream)
        {
            LearningMachine.Persist(kbStream);
        }
    }
}
