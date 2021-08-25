using System.Collections.Generic;

namespace EasyERPExplorer.Models
{
    class PathComparer : IComparer<IOTemplate>
    {
        public int Compare(IOTemplate x, IOTemplate y)
        {
            return x.FullPath.CompareTo(y.FullPath);
        }
    }
}
