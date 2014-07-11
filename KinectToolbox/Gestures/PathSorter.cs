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
        List<PathSorterEntry> extendedEntriesList = new List<PathSorterEntry>();
        public List<PathSorterEntry> ExtendedEntriesList
        { get {
            extendedEntriesList.AddRange(rightHandEntries);
            extendedEntriesList.AddRange(leftHandEntries);
            return extendedEntriesList;
        }
        }

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
            //Tools.SavePointsToFile(result, "path_sorter");
            return result;
        }
    }
}
