using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   public abstract class FileSystemNode
   {
      /// <summary>
      /// Represents a file system node, which can be either a file or a directory.
      /// Has a path to the node and size in bytes.
      /// Accepsts a visitor for processing the node.
      /// </summary>
      public string Path { get; }
      public long Size { get; protected set; }

      protected FileSystemNode(string path)
      {
         Path = path ?? throw new ArgumentNullException(nameof(path));
      }

      public abstract void Accept(FileSystemVisitor visitor);

   }
}
