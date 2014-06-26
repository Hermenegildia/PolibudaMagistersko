//****************************************
// from and by Programming with the Kinect for Windows SDK.pdf
//
// Dziękuję za uwagę
// Ala
//****************************************
using GestureKinectTools.MathTools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Context

{
    public class ContextTracker
    {
        readonly Dictionary<int, List<ContextPoint>> points = new Dictionary<int, List<ContextPoint>>();
        readonly int iterationsCount;    //todo: Ala - dlaczego nazwa windowSize? zmienione na iterationsCount

        public float Threshold { get; set; }

        public ContextTracker(int iterationsCount = 40, float threshold = 0.05f)
        {
            this.iterationsCount = iterationsCount;
            this.Threshold = threshold;
        }

        public void Add(Vector3 position, int trackingId)
        {
            if (!points.ContainsKey(trackingId))
                points.Add(trackingId, new List<ContextPoint>());

            points[trackingId].Add(new ContextPoint() {Position = position, Time=DateTime.Now });

            if (points[trackingId].Count > iterationsCount)
                points[trackingId].RemoveAt(0);
        }

        public bool isStable(int trackingId)
        {
            List<ContextPoint> currentPositions = points[trackingId];

            if (currentPositions.Count != iterationsCount)
                return false;

            Vector3 current = currentPositions[currentPositions.Count - 1].Position;
            Vector3 average = Vector3.Zero;

            for (int i = 0; i < currentPositions.Count-2; i++)
            {
                average += currentPositions[i].Position;
            }

            average /= currentPositions.Count - 1;

            if ((average - current).Length > Threshold)
                return false;

            return true;
        }

        public void Add(Skeleton skeleton, JointType jointType) //todo: Ala - tutak uwzględnić palyerIndex (z depthFrame), a nie trackingId ze skeletona
        {
            var trackingId = skeleton.TrackingId;
            var position = Vector3.ToVector3(skeleton.Joints[jointType].Position);
            Add(position, trackingId);
            
        }

        public bool IsStableRelativeToCurrentSpeed(int trackingId)
        {
            List<ContextPoint> currentPoints = points[trackingId];
            
            if (currentPoints.Count < 2)
                return false;

            Vector3 previousPosition = currentPoints[currentPoints.Count - 2].Position; 
            Vector3 currentPosition = currentPoints[currentPoints.Count - 1].Position;

            DateTime previousTime = currentPoints[currentPoints.Count - 2].Time;
            DateTime currentTime = currentPoints[currentPoints.Count - 1].Time;

            var currentSpeed = (currentPosition - previousPosition).Length / ((currentTime - previousTime).TotalMilliseconds);
            if (currentSpeed > Threshold)
                return false;

            return true;
        }

        public bool IsStableRelativeToAverageSpeed(int trackingId)
        {

            List<ContextPoint> currentPoints = points[trackingId];

            if (currentPoints.Count != iterationsCount)
                return false;

            Vector3 startPosition = currentPoints[0].Position;
            Vector3 currentPosition = currentPoints[currentPoints.Count - 1].Position;

            DateTime startTime = currentPoints[0].Time;
            DateTime currentTime = currentPoints[currentPoints.Count - 1].Time;

            var averageSpeed = (currentPosition - startPosition).Length / ((currentTime - startTime).TotalMilliseconds);
            if (averageSpeed > Threshold)
                return false;
            
            return true;
        }

        public bool IsTowardsSensor(Skeleton skeleton) //sprawdza odległość ramion od kinecta (stoi na wprost), ale może być plecami! :(
        {
            var leftShoulderPosition = Vector3.ToVector3( skeleton.Joints[JointType.ShoulderLeft].Position);
            var rightShoulderPosition = Vector3.ToVector3(skeleton.Joints[JointType.ShoulderRight].Position);

            var leftDistance = leftShoulderPosition.Z;
            var rightDistance = rightShoulderPosition.Z;

            if (Math.Abs(leftDistance - rightDistance) > Threshold)
                return false;

            return true;
        }
    }
}
