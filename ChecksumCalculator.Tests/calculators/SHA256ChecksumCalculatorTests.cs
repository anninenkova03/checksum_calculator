using System.Text;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class SHA256ChecksumCalculatorTests
   {
      [Theory]
      [InlineData("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
      [InlineData("Hello, World!", "dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f")]
      public void Calculate_ReturnsCorrectSHA256(string input, string expectedHash)
      {
         var calculator = new SHA256ChecksumCalculator();
         using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
         string actualHash = calculator.Calculate(stream);
         Assert.Equal(expectedHash, actualHash);
      }
   }
}