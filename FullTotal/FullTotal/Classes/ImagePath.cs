using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTotal
{
    public class ImagePath
    {
        String path;

        public ImagePath()
        {
        }

        public ImagePath(string filePath)
        {
            this.Path = filePath;
        }


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

    public class ImagePathList : List<ImagePath>
    {
        public ImagePathList()
        {
            
        }
    }
}
