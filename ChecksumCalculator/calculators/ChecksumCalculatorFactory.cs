using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.calculators
{
   public class ChecksumCalculatorFactory
   {
      /// <summary>  
      /// Uses reflection to enlist all available checksum calculator types and their corresponding algorithm names.  
      /// Creates an instance of a checksum calculator based on the specified algorithm name.  
      /// </summary>  
      /// <param name="algorithm"></param>  
      /// <returns></returns>  
      public static IChecksumCalculator Create(string algorithm)
      {
         // Get all types in the current assembly that implement IChecksumCalculator  
         var calculatorTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IChecksumCalculator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

         // Find the type that matches the algorithm name  
         var calculatorType = calculatorTypes.FirstOrDefault(type => type.Name.Equals(algorithm + "ChecksumCalculator", StringComparison.OrdinalIgnoreCase));
         if (calculatorType == null)
         {
            throw new ArgumentException($"No checksum calculator found for algorithm: {algorithm}");
         }

         // Create an instance of the found type  
         var instance = Activator.CreateInstance(calculatorType) as IChecksumCalculator;
         if (instance == null)
         {
            throw new InvalidOperationException($"Failed to create an instance of {calculatorType.FullName}.");
         }

         return instance;
      }
   }
}
