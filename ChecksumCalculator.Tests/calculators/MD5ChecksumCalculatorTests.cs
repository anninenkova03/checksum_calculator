using System.Text;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class MD5ChecksumCalculatorTests
   {
      [Theory]
      [InlineData("abc", "900150983cd24fb0d6963f7d28e17f72")]
      [InlineData("Hello, World!", "65a8e27d8879283831b664bd8b7f0ad4")]
      public void Calculate_ReturnsCorrectMD5(string input, string expectedHash)
      {
         var calculator = new MD5ChecksumCalculator();
         using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
         string actualHash = calculator.Calculate(stream);
         Assert.Equal(expectedHash, actualHash);
      }
   }
}