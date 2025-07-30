using System.CommandLine;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator
{
   internal class Program
   {
      static void Main(string[] args)
      {
         var pathOption = new Option<string>(
            "--path");

         var algorithmOption = new Option<string>(
            "--algorithm");

         var rootCommand = new RootCommand("Checksum Calculator")
         {
            pathOption,
            algorithmOption
         };

         try
         {
            var parseResult = rootCommand.Parse(args);

            string path = parseResult.GetValue(pathOption) ?? Directory.GetCurrentDirectory();
            string algorithm = parseResult.GetValue(algorithmOption) ?? "MD5";

            if (!File.Exists(path))
            {
               Console.WriteLine($"File not found: {path}");
               return;
            }

            var calculator = ChecksumCalculatorFactory.Create(algorithm);
            using var stream = File.OpenRead(path);
            var checksum = calculator.calculate(stream);
            Console.WriteLine($"Checksum ({algorithm.ToUpper()}): {checksum}");
         }
         catch (Exception ex)
         {
            Console.WriteLine($"An error occurred: {ex.Message}");
         }
      }
   }
}
