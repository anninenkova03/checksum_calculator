using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a builder for constructing a file system tree.
   /// </summary>
   public abstract class FileSystemBuilder
   {
      protected readonly HashSet<string> Visited = new(StringComparer.OrdinalIgnoreCase);
      public abstract FileSystemNode Build(string rootPath);
   }
}
