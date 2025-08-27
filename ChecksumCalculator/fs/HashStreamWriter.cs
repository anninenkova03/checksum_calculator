using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.fs
{
   /// <summary>
   /// A class that processes file nodes and writes their checksums to a text writer.
   /// </summary>
   public class HashStreamWriter : FileProcessorVisitor
   {
      private readonly IChecksumCalculator calculator;
      private readonly TextWriter writer;

      public HashStreamWriter(IChecksumCalculator calculator, TextWriter writer)
      {
         this.calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
         this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
      }

      protected override void ProcessFile(FileNode node)
      {
         if (node == null) throw new ArgumentNullException(nameof(node));
         try
         {
            using (var stream = File.OpenRead(node.Path))
            {
               var hash = calculator.Calculate(stream);
               writer.WriteLine($"{node.Path}: {hash}");
            }
         }
         catch (Exception ex)
         {
            writer.WriteLine($"Error reading file {node.Path}: {ex.Message}");

         }
      }
   }
}
