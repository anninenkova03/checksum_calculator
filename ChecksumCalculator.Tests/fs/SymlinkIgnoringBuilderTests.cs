using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChecksumCalculator.fs;

namespace ChecksumCalculator.Tests.fs
{
   public class SymlinkIgnoringBuilderTests
   {
      [Fact]
      public void ReportIgnoringSymlinks()
      {
         string baseDir = AppContext.BaseDirectory;
         string fullPath = Path.Combine(baseDir, @"ChecksumCalculator.Tests\..\..\..\..\resources\symlinks");
         fullPath = Path.GetFullPath(fullPath);
         var builder = new SymlinkIgnoringBuilder();
         var root = builder.Build(fullPath);
         var writer = new StringWriter();
         var reporter = new ReportWriter(writer);

         root.Accept(reporter);
         string output = writer.ToString().Trim();

         var lines = output.Split('\n')
             .Select(line => line.Trim())
             .Where(line => !string.IsNullOrEmpty(line))
             .ToArray();

         Assert.Contains(lines, line => line.Contains("to_symlinks.lnk") && line.EndsWith("bytes"));
         Assert.Contains(lines, line => line.Contains("to_nonempty.lnk") && line.EndsWith("bytes"));
         Assert.Equal(2, lines.Length);
      }
   }
}
