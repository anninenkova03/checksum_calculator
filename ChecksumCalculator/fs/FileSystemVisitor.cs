using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a visitor for traversing file system nodes.
   /// </summary>
   public abstract class FileSystemVisitor
   {
      public abstract void VisitFile(FileNode fileNode);
      public abstract void VisitDirectory(DirectoryNode dir);

   }
}
