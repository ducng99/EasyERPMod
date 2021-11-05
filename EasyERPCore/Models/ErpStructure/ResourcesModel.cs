using EgoEngineLibrary.Archive.Erp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ERPLoader.Models
{
    public class ResourcesModel
    {
        private const string QEscString = "%3F";
        private const string FragNameDelim = "!!!";

        public List<ErpResource> Resources { get; private set; }

        public ResourcesModel(ErpFile data)
        {
            Resources = data.Resources;
        }

        public static string GetFragmentFileName(ErpResource resource, ErpFragment fragment)
        {
            var fragmentIndex = resource.Fragments.IndexOf(fragment);
            if (fragmentIndex < 0)
            {
                throw new InvalidOperationException("The fragment does not belong to this resource.");
            }

            return GetFragmentFileName(resource, fragment, fragmentIndex);
        }

        private static string GetFragmentFileName(ErpResource resource, ErpFragment fragment, int fragmentIndex)
        {
            var name = resource.FileName;
            name = name.Replace("?", QEscString);
            name = Path.GetFileNameWithoutExtension(name) + FragNameDelim
                + fragment.Name + fragmentIndex.ToString("000")
                + Path.GetExtension(name);
            return name;
        }
    }
}
