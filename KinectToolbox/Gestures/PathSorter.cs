using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
    public class PathSorter
    {
        readonly List<PathSorterEntry> rightHandEntries = new List<PathSorterEntry>();
        readonly List<PathSorterEntry> leftHandEntries = new List<PathSorterEntry>();

        protected List<PathSorterEntry> ExtendedEntiesList
        { get { return rightHandEntries.Concat(leftHandEntries).ToList<PathSorterEntry>(); } }

        public PathSorter()
        {
        }
        
        public List<Vector2> Add(Vector2 position, bool isRightHand)
        {
            PathSorterEntry newEntry = new PathSorterEntry{Position = position, Time = DateTime.Now};
            if (isRightHand)
                rightHandEntries.Add(newEntry);
            else
                leftHandEntries.Add(newEntry);

            return ExtendedEntiesList.Select(x => x.Position).ToList<Vector2>();
        }
    }
}
