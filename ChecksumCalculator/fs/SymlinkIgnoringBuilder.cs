using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a builder for constructing a file system tree that ignores symbolic links.
   /// </summary>
   public class SymlinkIgnoringBuilder : FileSystemBuilder
   {
      public override FileSystemNode Build(string rootPath)
      {
         if (File.Exists(rootPath))
         {
            var info = new FileInfo(rootPath);
            return new FileNode(rootPath, info.Length);
         }

         var dirNode = new DirectoryNode(rootPath);
         foreach (var file in Directory.GetFiles(rootPath))
         {
            var info = new FileInfo(file);
            dirNode.AddChild(new FileNode(file, info.Length));
         }
         foreach (var subDir in Directory.GetDirectories(rootPath))
         {
            var subDirNode = Build(subDir);
            if (subDirNode != null)
            {
               dirNode.AddChild(subDirNode);
            }
         }

         return dirNode;
      }
   }
}
