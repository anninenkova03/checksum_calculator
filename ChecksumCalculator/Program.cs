using System.CommandLine;
using System.Text;
using ChecksumCalculator.calculators;
using ChecksumCalculator.fs;

namespace ChecksumCalculator
{
   internal class Program
   {
      public static void Main(string[] args)
      {
         if (args.Length == 0)
         {
            Console.WriteLine("Enter arguments (e.g. --path C:\\MyFolder --algorithm SHA512):");
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
               args = SplitArgs(input);
            }
         }

         var pathOption = new Option<string>(
            "--path");
         var algorithmOption = new Option<string>(
            "--algorithm");
         var followSymlinksOption = new Option<bool>(
            "--follow-symlinks");

         var rootCommand = new RootCommand("Checksum Calculator")
         {
            pathOption,
            algorithmOption,
            followSymlinksOption
         };

         try
         {
            var parseResult = rootCommand.Parse(args);

            string path = parseResult.GetValue(pathOption) ?? Directory.GetCurrentDirectory();
            string algorithm = parseResult.GetValue(algorithmOption) ?? "MD5";
            bool followSymlinks = parseResult.GetValue(followSymlinksOption);

            if (!File.Exists(path) && !Directory.Exists(path))
            {
               Console.Error.WriteLine($"Error: The specified path does not exist: {path}");
               Environment.Exit(1);
               return;
            }

            var calculator = ChecksumCalculatorFactory.Create(algorithm);

            Console.WriteLine($"Calculating {algorithm.ToUpper()} checksums for '{path}'...");

            FileSystemBuilder builder = followSymlinks
                ? new SymlinkFollowingBuilder()
                : new SymlinkIgnoringBuilder();

            var rootNode = builder.Build(path);

            if (rootNode == null)
            {
               Console.WriteLine("No files found to process.");
               return;
            }

            var hashWriter = new HashStreamWriter(calculator, Console.Out);
            rootNode.Accept(hashWriter);

         }
         catch (Exception ex)
         {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
         }
      }

      private static string[] SplitArgs(string commandLine)
      {
         var args = new List<string>();
         var current = new StringBuilder();
         bool inQuotes = false;

         foreach (char c in commandLine)
         {
            if (c == '\"')
            {
               inQuotes = !inQuotes;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
               if (current.Length > 0)
               {
                  args.Add(current.ToString());
                  current.Clear();
               }
            }
            else
            {
               current.Append(c);
            }
         }

         if (current.Length > 0)
         {
            args.Add(current.ToString());
         }

         return args.ToArray();
      }
   }
}
