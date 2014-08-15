using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace connectionChecker.Classes
{
    public class ImagePath
    {
        String path;
        public String Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    FileName = ExtractFileNameFromPath(value);
                }
            }
        }

        public ImagePath(string filePath)
        {
            this.Path = filePath;
        }

        private String ExtractFileNameFromPath(String path)
        {
            int index = path.LastIndexOf('\\');
            return path.Substring(index + 1);
        }
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
