using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   public class FileNode : FileSystemNode
   {
      /// <summary>
      /// Represents a file in the file system, which has a path and size in bytes.
      /// </summary>
      /// <param name="path"></param>
      /// <param name="size"></param>
      public FileNode(string path, long size) : base(path)
      {
         Size = size;
      }
      public override void Accept(FileSystemVisitor visitor)
      {
         visitor.VisitFile(this);
      }
   }
}
