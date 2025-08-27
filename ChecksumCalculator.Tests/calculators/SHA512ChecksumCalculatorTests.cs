using System.Text;
using ChecksumCalculator.calculators;

namespace ChecksumCalculator.Tests.calculators
{
   public class SHA512ChecksumCalculatorTests
   {
      [Theory]
      [InlineData("abc", "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
      [InlineData("Hello, World!", "374d794a95cdcfd8b35993185fef9ba368f160d8daf432d08ba9f1ed1e5abe6cc69291e0fa2fe0006a52570ef18c19def4e617c33ce52ef0a6e5fbe318cb0387")]
      public void Calculate_ReturnsCorrectSHA512(string input, string expectedHash)
      {
         var calculator = new SHA512ChecksumCalculator();
         using var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
         string actualHash = calculator.Calculate(stream);
         Assert.Equal(expectedHash, actualHash);
      }
   }
}