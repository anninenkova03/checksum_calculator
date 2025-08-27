using System.Text;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class SHA384ChecksumCalculatorTests
   {
      [Theory]
      [InlineData("abc", "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7")]
      [InlineData("Hello, World!", "5485cc9b3365b4305dfb4e8337e0a598a574f8242bf17289e0dd6c20a3cd44a089de16ab4ab308f63e44b1170eb5f515")]
      public void Calculate_ReturnsCorrectSHA384(string input, string expectedHash)
      {
         var calculator = new SHA384ChecksumCalculator();
         using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
         string actualHash = calculator.Calculate(stream);
         Assert.Equal(expectedHash, actualHash);
      }
   }
}