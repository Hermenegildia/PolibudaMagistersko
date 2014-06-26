using GestureKinectTools.MathTools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Gestures
{
    public class TemplatedGestureDetector: GestureDetector
    {
        public float Epsilon { get; set; }
        public float MinScore { get; set; }
        public float MinSize { get; set; }

        readonly LearningMachine learningMachine;
        RecordedPath path;
        readonly string gestureName;

        public bool IsRecordingPath
        {
            get { return path != null; }
        }

        public LearningMachine LearningMachine
        {
            get { return learningMachine; }
        }

        public TemplatedGestureDetector(string gestureName, Stream kbStream, int iterationsCount=60) 
            :base(iterationsCount)
        {
            Epsilon = 0.035f;
            MinScore = 0.80f;
            MinSize = 0.1f;
            this.gestureName = gestureName;
            learningMachine = new LearningMachine(kbStream);
        }

        public override void AddEntry(SkeletonPoint position, KinectSensor sensor)
        {            base.AddEntry(position, sensor);

            if (path != null)
            {
                path.Points.Add(new Vector2(position.X, position.Y));
            }        }

        protected override void LookForGesture()
        {
            if (LearningMachine.Match(Entries.Select(e => new Vector2(e.Position.X, e.Position.Y)).ToList(), Epsilon, MinScore, MinSize))
                RaiseGestureDetected(gestureName);
        }

        public void StartRecordingTemplate()
        {
            path = new RecordedPath(IterationsCount);
        }

        public void EndRecordingTemplate()
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
