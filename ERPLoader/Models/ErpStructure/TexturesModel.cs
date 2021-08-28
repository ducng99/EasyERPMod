using EgoEngineLibrary.Archive.Erp;
using EgoEngineLibrary.Archive.Erp.Data;
using EgoEngineLibrary.Graphics;
using EgoEngineLibrary.Graphics.Dds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ERPLoader.Models
{
    public class TexturesModel
    {
        private readonly List<ErpResource> Textures = new();
        private readonly Dictionary<string, ReaderWriterLockSlim> MipMapLocks = new();

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

        public void Export(string texturesFolderPath)
        {
            Directory.CreateDirectory(texturesFolderPath);

            Parallel.ForEach(Textures, texture =>
            {
                try
                {
                    var srvRes = new ErpGfxSRVResource();
                    srvRes.FromResource(texture);

                    var textureArraySize = srvRes.SurfaceRes.Fragment0.ArraySize;

                    var mipFullPath = Path.Combine(Settings.Instance.F1GameDirectory, srvRes.SurfaceRes.Frag2.MipMapFileName);
                    var foundMipMapFile = File.Exists(mipFullPath);
                    var hasValidMips = srvRes.SurfaceRes.HasValidMips;

                    ReaderWriterLockSlim mipMapLock = null;
                    if (!MipMapLocks.TryGetValue(mipFullPath, out mipMapLock))
                    {
                        mipMapLock = new();
                        MipMapLocks[mipFullPath] = mipMapLock;
                    }

                    Stream mipMapStream = foundMipMapFile ? File.Open(mipFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read) : null;
                    bool mipmapFileCheckOK = true;

                    if (srvRes.SurfaceRes.HasMips)
                    {
                        if (hasValidMips && !foundMipMapFile)
                        {
                            Logger.Error($"Cannot find mipmaps file at {mipFullPath}.");
                            mipMapStream.Dispose();
                            mipmapFileCheckOK = false;
                        }

                        if (!hasValidMips)
                        {
                            Logger.Warning($"Mipmaps file at {mipFullPath} does not match texture file! I will try to recover it for you");

                            mipMapLock.EnterWriteLock();
                            srvRes.ToDdsFile(mipMapStream, false, 0);
                            mipMapLock.ExitWriteLock();
                        }
                    }

                    if (mipmapFileCheckOK)
                    {
                        DdsFile dds;
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

                                    using var stream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                                    dds.Write(stream, -1);

                                    Logger.Log($"Exported {Path.GetFileName(filePath)}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occurred while exporting \"{texture.FileName}\"");
                    Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                }
            });
        }

        public void Import(string texturesFolderPath)
        {
            Parallel.ForEach(Textures, texture =>
            {
                if (Utils.FileExists(new Regex($@"{texture.FileName}(\.\d+)?\.dds", RegexOptions.Compiled), texturesFolderPath))
                {
                    try
                    {
                        var srvRes = new ErpGfxSRVResource();
                        srvRes.FromResource(texture);

                        var mipFullPath = srvRes.SurfaceRes.HasMips ? Path.Combine(Settings.Instance.F1GameDirectory, srvRes.SurfaceRes.Frag2.MipMapFileName) : null;
                        var textureArraySize = srvRes.SurfaceRes.Fragment0.ArraySize;

                        Stream mipMapStream = null;
                        ReaderWriterLockSlim mipMapLock = null;

                        if (!string.IsNullOrWhiteSpace(mipFullPath))
                        {
                            if (!MipMapLocks.TryGetValue(mipFullPath, out mipMapLock))
                            {
                                mipMapLock = new();
                                MipMapLocks[mipFullPath] = mipMapLock;
                            }

                            mipMapLock.EnterWriteLock();

                            if (!File.Exists(mipFullPath + Settings.Instance.BackupFileExtension))
                                File.Copy(mipFullPath, mipFullPath + Settings.Instance.BackupFileExtension);
                            mipMapStream = File.Open(mipFullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                        }

                        for (uint i = 0; i < textureArraySize; i++)
                        {
                            var textureFileName = texture.FileName + (textureArraySize > 1 ? $".{i}.dds" : ".dds");
                            string filePath = Path.Combine(texturesFolderPath, textureFileName);

                            if (File.Exists(filePath))
                            {
                                using var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                var dds = new DdsFile(fs);

                                using (mipMapStream)
                                    dds.ToErpGfxSRVResource(srvRes, mipMapStream, false, i);

                                srvRes.ToResource(texture);

                                Logger.Log($"Imported {Path.GetFileName(filePath)}");
                            }
                        }

                        if (mipMapLock != null) mipMapLock.ExitWriteLock();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed importing " + texture.FileName);
                        Logger.FileWrite(ex.ToString(), Logger.MessageType.Error);
                    }
                }
            });
        }
    }
}
