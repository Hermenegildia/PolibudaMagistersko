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
        PathSorter pathSorter = new PathSorter();

        readonly List<Entry> leftEntries = new List<Entry>();
        readonly List<Entry> bothHandsEntries = new List<Entry>();


        protected List<Entry> LeftEntries
        {
            get { return leftEntries; }
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

        public TwoHandsTemplatedGestureDetector(string gestureName, Stream kbStream, int windowSize = 35)
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
        }

        //private void JoinEntriesFromTwoHands()
        //{
        //    BothHandsEntries.Clear();

        //    BothHandsEntries.AddRange(Entries);
        //    BothHandsEntries.AddRange(LeftEntries);
        
        //}

     

        public void Add(Skeleton skeleton, KinectSensor sensor)
        {
            SkeletonPoint rightPosition ;

            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
            {
                rightPosition = skeleton.Joints[JointType.HandRight].Position;
                Entry rightEntry = new Entry { Position = rightPosition.ToVector3(), Time = DateTime.Now };
                Entries.Add(rightEntry);

                rightEntry.DisplayEllipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    StrokeThickness = 2.0,
                    Stroke = new SolidColorBrush(DisplayColor),
                    StrokeLineJoin = PenLineJoin.Round
                };

                Vector2 leftVector2 = Tools.Convert(sensor, rightPosition);
                
                float xl = (float)(leftVector2.X * DisplayCanvas.ActualWidth);
                float yl = (float)(leftVector2.Y * DisplayCanvas.ActualHeight);

                Canvas.SetLeft(rightEntry.DisplayEllipse, xl - rightEntry.DisplayEllipse.Width / 2);
                Canvas.SetTop(rightEntry.DisplayEllipse, yl - rightEntry.DisplayEllipse.Height / 2);

                DisplayCanvas.Children.Add(rightEntry.DisplayEllipse);

                //if (path != null)
                if(pathSorter != null)
                {
                    pathSorter.Add(rightPosition.ToVector2(), true);
                }
            }
        
            SkeletonPoint leftPosition ;
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
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeLineJoin = PenLineJoin.Round
                };

                Vector2 leftVector2 = Tools.Convert(sensor, leftPosition);

                float xl = (float)(leftVector2.X * DisplayCanvas.ActualWidth);
                float yl = (float)(leftVector2.Y * DisplayCanvas.ActualHeight);

                Canvas.SetLeft(leftEntry.DisplayEllipse, xl - leftEntry.DisplayEllipse.Width / 2);
                Canvas.SetTop(leftEntry.DisplayEllipse, yl - leftEntry.DisplayEllipse.Height / 2);

                DisplayCanvas.Children.Add(leftEntry.DisplayEllipse);

                //if (path != null)
                if (pathSorter!= null)
                {
                    pathSorter.Add(leftPosition.ToVector2(), false);
                }
            }


            // Remove too old positions
            if (Entries.Count > WindowSize)
            {
                Entry entryToRemove = Entries[0];

                if (DisplayCanvas != null)
                {
                    DisplayCanvas.Children.Remove(entryToRemove.DisplayEllipse);
                }

                Entries.Remove(entryToRemove);
            }
            
            if (LeftEntries.Count > WindowSize )
            {
                Entry entryToRemove = LeftEntries[0];

                if (DisplayCanvas != null)
                {
                    DisplayCanvas.Children.Remove(entryToRemove.DisplayEllipse);
                }

                LeftEntries.Remove(entryToRemove);
            }

          
              LookForGesture();

        }
        

        protected override void LookForGesture()
        {
            //JoinEntriesFromTwoHands();
            //if (LearningMachine.Match(BothHandsEntries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Epsilon, MinimalScore, MinimalSize))
            //if (LearningMachine.Match(pathSorter.LeftHandPositions, pathSorter.RightHandPositions, Epsilon, MinimalScore, MinimalSize))
            if (LearningMachine.Match(LeftEntries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Entries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Epsilon, MinimalScore, MinimalSize))
                RaiseGestureDetected(gestureName);
        }

        public void StartRecordTemplate()
        {
            ClearEntries(Entries);  //nagrywanie czysci stare kropki tego gestu
            ClearEntries(LeftEntries);
            pathSorter = new PathSorter();
            path = new RecordedPath(WindowSize);
        }

        private void ClearEntries(List<Entry> entriesList) //dopisane przeze mnie - czyściciel kropek 
        {
            if (DisplayCanvas != null)
            {
                DisplayCanvas.Children.Clear();
            }
                Entries.Clear();
        }

        public void ClearEntries() //dopisane przeze mnie - czyściciel kropek 
        {
            ClearEntries(Entries);  //czysci stare kropki
            ClearEntries(LeftEntries);
        }

        public void EndRecordTemplate()
        {
            path.Points = pathSorter.GetPoints();
            //Tools.SavePointsToFile(path.Points, "do_zapisu");
            //LearningMachine.AddPath(path);
            LearningMachine.AddPath(pathSorter.LeftHandPositions, pathSorter.RightHandPositions, path);
            path = null;
            pathSorter = null;

        }

        public void SaveState(Stream kbStream)
        {
            LearningMachine.Persist(kbStream);
        }

      
    }
}
