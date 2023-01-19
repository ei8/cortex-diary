using System.Reflection;
using System.Runtime.Loader;
using System;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public class LibraryLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver fileResolver;

        public LibraryLoadContext(string binFolder)
        {
            this.fileResolver = new AssemblyDependencyResolver(binFolder);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = this.fileResolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return this.LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var filePath = this.fileResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (filePath != null)
            {
                return this.LoadUnmanagedDllFromPath(filePath);
            }

            return IntPtr.Zero;
        }
    }
}
