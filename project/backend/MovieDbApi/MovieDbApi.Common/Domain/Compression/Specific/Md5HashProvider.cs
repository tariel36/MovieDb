using System.Security.Cryptography;
using System.Text;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Utility;

namespace MovieDbApi.Common.Domain.Compression.Specific
{
    public class Md5HashProvider
        : IHashProvider
    {
        private readonly MD5 _md5;

        public Md5HashProvider()
        {
            _md5 = MD5.Create();
        }

        public void Dispose()
        {
            Extensions.SafeDispose(_md5);
        }

        public string Get(string value)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(value);
            byte[] hashBytes = _md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}
