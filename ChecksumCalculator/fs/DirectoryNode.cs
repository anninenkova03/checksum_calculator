using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// Represents a directory in the file system, which can contain files and other directories.
   /// </summary>
   public class DirectoryNode : FileSystemNode
   {
      public List<FileSystemNode> Children { get; private set; }
      public DirectoryNode(string path) : base(path) {
         Children = new List<FileSystemNode>();
      }

      public void AddChild(FileSystemNode child)
      {
         if (Children == null)
         {
            Children = new List<FileSystemNode>();
         }
         Children.Add(child);
         Size += child.Size;
      }

      public override void Accept(FileSystemVisitor visitor)
      {
         visitor.VisitDirectory(this);
      }
   }
}
