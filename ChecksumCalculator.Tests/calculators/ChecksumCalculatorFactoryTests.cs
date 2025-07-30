using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class ChecksumCalculatorFactoryTests
   {
      [Theory]
      [InlineData("MD5", typeof(MD5ChecksumCalculator))]
      [InlineData("Sha1", typeof(SHA1ChecksumCalculator))]
      
      public void Create_KnownAlgorithm_ReturnsCorrectType(string algorithm, Type expectedType)
      {
         var calculator = ChecksumCalculatorFactory.Create(algorithm);
         Assert.NotNull(calculator);
         Assert.IsType(expectedType, calculator);
      }

      [Theory]
      [InlineData("nonexistent")]
      public void Create_UnknownAlgorithm_ThrowsArgumentException(string algorithm)
      {
         var ex = Assert.Throws<ArgumentException>(() => ChecksumCalculatorFactory.Create(algorithm));
         Assert.Contains("No checksum calculator found", ex.Message);
      }


      [Theory]
      [InlineData("")]
      [InlineData(null)]
      public void Create_EmptyOrNullAlgorithm_ThrowsArgumentException(string algorithm)
      {
         var ex = Assert.Throws<ArgumentException>(() => ChecksumCalculatorFactory.Create(algorithm));
         Assert.Contains("No checksum calculator found", ex.Message);
      }
   }
}
