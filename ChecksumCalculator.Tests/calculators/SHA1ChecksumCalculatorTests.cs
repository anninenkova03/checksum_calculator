using System.Text;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class SHA1ChecksumCalculatorTests
   {
      [Theory]
      [InlineData("abc", "a9993e364706816aba3e25717850c26c9cd0d89d")]
      [InlineData("Hello, World!", "0a0a9f2a6772942557ab5355d76af442f8f65e01")]
      public void Calculate_ReturnsCorrectSHA1(string input, string expectedHash)
      {
         var calculator = new SHA1ChecksumCalculator();
         using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
         string actualHash = calculator.Calculate(stream);
         Assert.Equal(expectedHash, actualHash);
      }
   }
}