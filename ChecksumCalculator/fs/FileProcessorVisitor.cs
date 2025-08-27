using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a visitor for processing files in the file system.
   /// </summary>
   public abstract class FileProcessorVisitor : FileSystemVisitor
   {
      public sealed override void VisitFile(FileNode file)
      {
         ProcessFile(file);
      }

      public override void VisitDirectory(DirectoryNode dir)
      {
         foreach (var child in dir.Children)
         {
            child.Accept(this);
         }
      }

      protected abstract void ProcessFile(FileNode file);
   }
}
