using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.calculators
{
   public class SHA512ChecksumCalculator : IChecksumCalculator
   {
      /// <summary>
      /// Calculates the SHA512 checksum of the provided input stream.
      /// </summary>
      /// <param name="inputStream"></param>
      /// <returns></returns>
      public string Calculate(Stream inputStream)
      {
         using var hasher = SHA512.Create();
         {
            byte[] hashBytes = hasher.ComputeHash(inputStream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
         }
      }
   }
}
