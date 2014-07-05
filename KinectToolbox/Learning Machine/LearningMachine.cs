using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Kinect.Toolbox.Gestures.Learning_Machine;
using System;
using System.Text;

namespace Kinect.Toolbox
{
    public class LearningMachine
    {
        readonly List<RecordedPath> paths;

        public LearningMachine(Stream kbStream)
        {
            if (kbStream == null || kbStream.Length == 0)
            {
                paths = new List<RecordedPath>();
                return;
            }

            BinaryFormatter formatter = new BinaryFormatter {Binder = new CustomBinder()};


            paths = (List<RecordedPath>)formatter.Deserialize(kbStream);
          

            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\rozmontowane_gesty_x.txt"))
            {
                writer.Write(string.Empty);
            }
            using (StreamWriter writer = new StreamWriter(mydocpath + @"\rozmontowane_gesty_y.txt"))
            {
                writer.Write(string.Empty);
            }

            foreach (RecordedPath path in paths)
            {
                StringBuilder sbX = new StringBuilder();
                StringBuilder sbY = new StringBuilder();

                //sb.AppendLine("next vector " + DateTime.Now.ToString());
                foreach (Vector2 point in path.Points)
                {
                    sbX.AppendLine(point.X.ToString(System.Globalization.CultureInfo.InvariantCulture));// + " y: " + point.Y.ToString());
                    sbY.AppendLine(point.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));

                }
                //sbX.AppendLine();
                using (StreamWriter writer = new StreamWriter(mydocpath + @"\rozmontowane_gesty_x.txt", true))
                {
                    writer.Write(sbX.ToString());
                }
                using (StreamWriter writer = new StreamWriter(mydocpath + @"\rozmontowane_gesty_y.txt", true))
                {
                    writer.Write(sbY.ToString());
                }


            }

            //paths = new List<RecordedPath>();
        }

        public List<RecordedPath> Paths
        {
            get { return paths; }
        }

        public bool Match(List<Vector2> entries, float threshold, float minimalScore, float minSize)
        {
            return Paths.Any(path => path.Match(entries, threshold, minimalScore, minSize));
        }

        public void Persist(Stream kbStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(kbStream, Paths);
        }

        public void AddPath(RecordedPath path)
        {
            path.CloseAndPrepare();
            Paths.Add(path);
        }
    }
}
