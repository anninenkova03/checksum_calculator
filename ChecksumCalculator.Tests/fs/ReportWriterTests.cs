using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChecksumCalculator.fs;

namespace ChecksumCalculator.Tests.fs
{
   public class ReportWriterTests
   {
      [Fact]
      public void ThowsIfWriterIsNull()
      {
         var ex = Assert.Throws<ArgumentNullException>(() => new ReportWriter(null));
         Assert.Equal("writer", ex.ParamName);
      }

      [Fact]
      public void ThrowsIfFileIsNull()
      {
         var writer = new StringWriter();
         var reporter = new ReportWriter(writer);
         var ex = Assert.Throws<ArgumentNullException>(() => reporter.VisitFile(null));
         Assert.Equal("file", ex.ParamName);
      }

      [Fact]
      public void WritesSingleFileReport()
      {
         string baseDir = AppContext.BaseDirectory;
         string fullPath = Path.Combine(baseDir, @"ChecksumCalculator.Tests\..\..\..\..\resources\nonempty\hello.txt");
         fullPath = Path.GetFullPath(fullPath);
         var builder = new SymlinkIgnoringBuilder();
         var file = builder.Build(fullPath) as FileNode;
         Assert.NotNull(file);

         var writer = new StringWriter();
         var reporter = new ReportWriter(writer);

         file.Accept(reporter);
         string output = writer.ToString().Trim();

         string filename = Path.GetFileName(file.Path);
         Assert.Equal($"{filename}: {file.Size} bytes", output);
      }

      [Fact]
      public void WritesNothingForEmptyDirectory()
      {
         string baseDir = AppContext.BaseDirectory;
         string fullPath = Path.Combine(baseDir, @"ChecksumCalculator.Tests\..\..\..\..\resources\empty");
         fullPath = Path.GetFullPath(fullPath);
         var builder = new SymlinkIgnoringBuilder();
         var writer = new StringWriter();
         var root = builder.Build(fullPath);
         var reporter = new ReportWriter(writer);

         root.Accept(reporter);
         string output = writer.ToString().Trim();

         Assert.True(string.IsNullOrWhiteSpace(output), "Expected no output for empty directory.");
      }

      [Fact]
      public void WritesNonEmptyDirectoryReport()
      {
         string baseDir = AppContext.BaseDirectory;
         string fullPath = Path.Combine(baseDir, @"ChecksumCalculator.Tests\..\..\..\..\resources\nonempty");
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

         Assert.Contains(lines, line => line.Contains("binary.bin") && line.EndsWith("bytes"));
         Assert.Contains(lines, line => line.Contains("hello.txt") && line.EndsWith("bytes"));
         Assert.Equal(2, lines.Length);
      }
   }
}
