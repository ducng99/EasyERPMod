using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Archive.Erp.Data;
using EgoEngineLibrary.Graphics;
using EgoEngineLibrary.Graphics.Dds;
using System.Collections.Generic;
using System.IO;

namespace EasyERPMod.Models
{
    class TexturesModel
    {
        private List<ErpResource> Textures = new List<ErpResource>();

        public TexturesModel(ResourcesModel resourcesModel)
        {
            foreach (var resource in resourcesModel.Resources)
            {
                if (resource.ResourceType.Equals("GfxSRVResource"))
                {
                    Textures.Add(resource);
                }
            }
        }

        public void Export(string folderPath)
        {
            string texturesFolderPath = Path.Combine(folderPath, "textures");
            Directory.CreateDirectory(texturesFolderPath);

            foreach (var texture in Textures)
            {
                Logger.Log($"Exporting {texture.FileName}...");

                var srvRes = new ErpGfxSRVResource();
                srvRes.FromResource(texture);

                var textureArraySize = srvRes.SurfaceRes.Fragment0.ArraySize;

                var mipMapFullFileName = Path.Combine("", srvRes.SurfaceRes.Frag2.MipMapFileName);
                var foundMipMapFile = File.Exists(mipMapFullFileName);
                var hasValidMips = srvRes.SurfaceRes.HasValidMips;

                if (srvRes.SurfaceRes.HasMips)
                {
                    if (hasValidMips && !foundMipMapFile)
                    {
                        Logger.Error($"Cannot find mipmaps file at {mipMapFullFileName}.");
                        continue;
                    }

                    if (!hasValidMips)
                    {
                        Logger.Error($"Mipmaps file at {mipMapFullFileName} are incorrectly modded! Use ErpArchiver and try export/import to fix mipmaps file.");
                        Logger.Warning("This will be automated in the future. Don't worry.");
                        continue;
                    }
                }

                DdsFile dds;
                Stream mipMapStream = foundMipMapFile ? File.Open(mipMapFullFileName, FileMode.Open, FileAccess.Read, FileShare.Read) : null;
                using (mipMapStream)
                {
                    for (uint i = 0; i < textureArraySize; i++)
                    {
                        string filePath = Path.Combine(texturesFolderPath, texture.FileName);
                        filePath += textureArraySize > 1 ? $".{i}.dds" : ".dds";

                        if (File.Exists(filePath))
                        {
                            Logger.Warning($"Found duplicate file at \"{filePath}\". This file will NOT be overwritten.");
                        }
                        else
                        {
                            dds = srvRes.ToDdsFile(mipMapStream, false, i);

                            var stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                            dds.Write(stream, -1);
                        }
                    }
                }      
            }
        }

        public void Import(string folderPath)
        {
            string texturesFolderPath = Path.Combine(folderPath, "textures");

            foreach (var texture in Textures)
            {
                Logger.Log($"Importing {texture.FileName}...");

                try
                {
                    var srvRes = new ErpGfxSRVResource();
                    srvRes.FromResource(texture);

                    var mipFullPath = srvRes.SurfaceRes.HasMips ? Path.Combine(Program.WorkingDirectory, srvRes.SurfaceRes.Frag2.MipMapFileName) : null;
                    var textureArraySize = srvRes.SurfaceRes.Fragment0.ArraySize;

                    Stream mipMapStream = !string.IsNullOrWhiteSpace(mipFullPath) ? File.Open(mipFullPath, FileMode.Create, FileAccess.Write, FileShare.Read) : null;
                    using (mipMapStream)
                    {
                        for (uint i = 0; i < textureArraySize; i++)
                        {
                            string filePath = Path.Combine(texturesFolderPath, texture.FileName);
                            filePath += textureArraySize > 1 ? $".{i}.dds" : ".dds";

                            var dds = new DdsFile(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
                            dds.ToErpGfxSRVResource(srvRes, mipMapStream, false, i);
                        }
                    }

                    srvRes.ToResource(texture);
                }
                catch
                {
                    Logger.Error("Failed importing " + texture.FileName);
                }
            }
        }
    }
}
