using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a builder for constructing a file system tree that follows symbolic links.
   /// </summary>
   public class SymlinkFollowingBuilder : FileSystemBuilder
   {
      public override FileSystemNode Build(string rootPath)
      {
         return BuildRecursive(rootPath);
      }

      private FileSystemNode BuildRecursive(string path)
      {
         if (Visited.Contains(path))
         {
            return null;
         }

         Visited.Add(path);

         if (File.Exists(path))
         {
            var info = new FileInfo(path);
            return new FileNode(path, info.Length);
         }

         var dirNode = new DirectoryNode(path);
         foreach (var entry in Directory.EnumerateFileSystemEntries(path))
         {
            var child = BuildRecursive(entry);
            if (child != null)
            {
               dirNode.AddChild(child);
            }
         }

         return dirNode;
      }

   }
}
