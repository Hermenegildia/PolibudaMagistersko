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

        public List<PathSorterEntry> ExtendedEntriesList
        { get { return rightHandEntries.Concat(leftHandEntries).ToList<PathSorterEntry>(); } }

        public PathSorter()
        {
        }
        
        public void Add(Vector2 position, bool isRightHand)
        {
            PathSorterEntry newEntry = new PathSorterEntry{Position = position, Time = DateTime.Now};
            if (isRightHand)
                rightHandEntries.Add(newEntry);
            else
                leftHandEntries.Add(newEntry);
        }

        public List<Vector2> GetPoints()
        {
            List<Vector2> result = new List<Vector2>();
            foreach (PathSorterEntry entry in ExtendedEntriesList)
            {
                result.Add(entry.Position);
            }
            return result;
        }
    }
}
