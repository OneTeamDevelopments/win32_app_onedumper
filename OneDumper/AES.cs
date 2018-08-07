using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace OneDumper
{
    class AES
    {
        public static byte[] Enc(byte[] input,string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", keyBytes), new byte[16]));
            byte[] encryptedBytes = cipher.DoFinal(input);
            return encryptedBytes;
        }

        public static byte[] Dec(byte[] input, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", keyBytes), new byte[16]));
            byte[] decryptedBytes = cipher.DoFinal(input);
            return decryptedBytes;
        }
    }
}
