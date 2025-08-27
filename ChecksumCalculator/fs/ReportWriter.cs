using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// A class that processes file nodes and writes their paths and sizes to a text writer.  
   /// </summary>
   public class ReportWriter : FileProcessorVisitor
   {
      private readonly TextWriter writer;
      public ReportWriter(TextWriter writer)
      {
         this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
      }
      protected override void ProcessFile(FileNode file)
      {
         if (file == null) throw new ArgumentNullException(nameof(file));
         string filename = Path.GetFileName(file.Path);
         writer.WriteLine($"{filename}: {file.Size} bytes");
      }
   }
}
