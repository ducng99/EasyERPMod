using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Data.Pkg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyERPMod.Models
{
    class PkgsModel
    {
        private readonly List<ErpFragment> Packages = new List<ErpFragment>();

        public PkgsModel(ResourcesModel resourcesModel)
        {
            foreach (var resource in resourcesModel.Resources)
            {
                foreach (var fragment in resource.Fragments)
                {
                    try
                    {
                        using var ds = fragment.GetDecompressDataStream(true);
                        if (PkgFile.IsPkgFile(ds))
                        {
                            Packages.Add(fragment);
                        }
                    }
                    catch
                    {
                        Logger.Error("Failed when trying to import packages from " + resource.FileName);
                    }
                }
            }
        }
    }
}
