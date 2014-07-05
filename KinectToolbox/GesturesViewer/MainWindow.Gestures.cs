using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace GesturesViewer
{
    partial class MainWindow
    {
        //void LoadCircleGestureDetector()
        //{
        //    using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
        //    {
        //        circleGestureRecognizer = new TemplatedGestureDetector("Circle", recordStream);
        //        circleGestureRecognizer.DisplayCanvas = gesturesCanvas;
        //        circleGestureRecognizer.OnGestureDetected += OnGestureDetected;

        //        MouseController.Current.ClickGestureDetector = circleGestureRecognizer;
        //    }
        //}

        void LoadEightGestureDetector()
        {
            using (Stream recordStream = File.Open(nowy_gest, FileMode.OpenOrCreate))
            {
                eightGestureRecognizer = new TemplatedGestureDetector("Ósemeczka!", recordStream);
                eightGestureRecognizer.DisplayCanvas = gesturesCanvas;
                eightGestureRecognizer.OnGestureDetected += OnGestureDetected;

                //MouseController.Current.ClickGestureDetector = circleGestureRecognizer;
            }
        }

        private void recordGesture_Click(object sender, RoutedEventArgs e)
        {
            //if (circleGestureRecognizer.IsRecordingPath)
            //{
            //    circleGestureRecognizer.EndRecordTemplate();
            //    recordGesture.Content = "Record Gesture";
            //    return;
            //}

            //circleGestureRecognizer.StartRecordTemplate();
            //recordGesture.Content = "Stop Recording";

            if (eightGestureRecognizer.IsRecordingPath)
            {
                eightGestureRecognizer.EndRecordTemplate();
                recordGesture.Content = "Record Gesture";
                return;
            }

            eightGestureRecognizer.StartRecordTemplate();
            recordGesture.Content = "Stop Recording";
        }

        void OnGestureDetected(string gesture)
        {
            int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            detectedGestures.SelectedIndex = pos;
        }

        void CloseGestureDetector()
        {
            //if (circleGestureRecognizer == null)
            //    return;

            ////string newPath = Path.Combine(Environment.CurrentDirectory, @"data\eska.save");
            //using (Stream recordStream = File.Create(circleKBPath))
            //{
            //    circleGestureRecognizer.SaveState(recordStream);
            //}
            //circleGestureRecognizer.OnGestureDetected -= OnGestureDetected;


            //obsługa mojego gestu ósemeczki
            if (eightGestureRecognizer == null)
                return;

            
            using (Stream recordStream = File.Create(nowy_gest))
            {
                eightGestureRecognizer.SaveState(recordStream);
            }
            eightGestureRecognizer.OnGestureDetected -= OnGestureDetected;
        }
    }
}
