using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;
using System.Linq;
using System.Web;

namespace EggOn.Web.UI.Utilities
{
    public class ModuleVirtualPathProvider : VirtualPathProvider
    {
        private VirtualPathProvider previousPathProvider;
        private string modulesPath;

        public ModuleVirtualPathProvider(string path, VirtualPathProvider previous)
        {
            this.modulesPath = path;
            this.previousPathProvider = previous;
        }

        public override bool FileExists(string virtualPath)
        {
            if (!IsModulePath(virtualPath))
            {
                return previousPathProvider.FileExists(virtualPath);
            }

            var absolutePath = GetAbsolutePathFromModulePath(virtualPath);

            return File.Exists(absolutePath);
        }

        public override bool DirectoryExists(string virtualPath)
        {
            if (!IsModulePath(virtualPath))
            {
                return previousPathProvider.DirectoryExists(virtualPath);
            }

            var absolutePath = GetAbsolutePathFromModulePath(virtualPath);

            return Directory.Exists(absolutePath);
        }

        public override VirtualDirectory GetDirectory(string virtualPath)
        {
            if (!IsModulePath(virtualPath))
            {
                return previousPathProvider.GetDirectory(virtualPath);
            }

            return new ModuleVirtualDirectory(virtualPath, this);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (!IsModulePath(virtualPath))
            {
                return previousPathProvider.GetFile(virtualPath);
            }

            return new ModuleVirtualFile(virtualPath, this);
        }

        public bool IsModulePath(string path)
        {
            return path.StartsWith("~/Modules/");
        }

        public string GetAbsolutePathFromModulePath(string modulePath)
        {
            int charsToRemove = "/Modules/".Length;

            // TODO: Fix this hack.
            if (modulePath.StartsWith("~"))
            {
                charsToRemove++;
            }

            var relativePath = modulePath.Remove(0, Math.Min(modulePath.Length, charsToRemove));
            var absolutePath = Path.Combine(this.modulesPath, relativePath);

            return absolutePath;
        }
    }

    public class ModuleVirtualDirectory : VirtualDirectory
    {
        private ModuleVirtualPathProvider pathProvider;

        public ModuleVirtualDirectory(string virtualPath, ModuleVirtualPathProvider pathProvider)
            : base(virtualPath)
        {
            this.pathProvider = pathProvider;
        }

        public override IEnumerable Children
        {
            get
            {
                var absolutePath = this.pathProvider.GetAbsolutePathFromModulePath(this.VirtualPath);

                return Directory.EnumerateFileSystemEntries(absolutePath)
                    .Where(path => {
                        return Directory.Exists(path) || File.Exists(path);
                    })
                    .Select<string, VirtualFileBase>(path =>
                    {
                        var virtualPath = VirtualPathUtility.Combine(this.VirtualPath, Path.GetFileName(path));

                        if (Directory.Exists(path))
                        {
                            return new ModuleVirtualDirectory(virtualPath, this.pathProvider);
                        } 
                        else
                        {
                            return new ModuleVirtualFile(virtualPath, this.pathProvider);
                        }
                    });
            }
        }

        public override IEnumerable Directories
        {
            get
            {
                var absolutePath = this.pathProvider.GetAbsolutePathFromModulePath(this.VirtualPath);

                return Directory.EnumerateDirectories(absolutePath).Select(path =>
                {
                    var virtualPath = VirtualPathUtility.Combine(this.VirtualPath, Path.GetFileName(path));
                    return new ModuleVirtualDirectory(virtualPath, this.pathProvider);
                });
            }
        }

        public override IEnumerable Files
        {
            get
            {
                var absolutePath = this.pathProvider.GetAbsolutePathFromModulePath(this.VirtualPath);

                return Directory.EnumerateFiles(absolutePath).Select(path =>
                {
                    var virtualPath = VirtualPathUtility.Combine(this.VirtualPath, Path.GetFileName(path));
                    return new ModuleVirtualFile(virtualPath, this.pathProvider);
                });
            }
        }
    }

    public class ModuleVirtualFile : VirtualFile
    {
        private ModuleVirtualPathProvider pathProvider;

        public ModuleVirtualFile(string virtualPath, ModuleVirtualPathProvider pathProvider)
            : base(virtualPath)
        {
            this.pathProvider = pathProvider;
        }

        public override Stream Open()
        {
            var absolutePath = this.pathProvider.GetAbsolutePathFromModulePath(this.VirtualPath);

            return new FileStream(absolutePath, FileMode.Open);
        }
    } 
}