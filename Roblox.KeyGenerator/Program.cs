using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Roblox.KeyGenerator
{
    class Program
    {
        const int PKCS_RSA_PRIVATE_KEY = 43;
        const int PKCS_7_ASN_ENCODING = 0x00010000;

        [DllImport("crypt32.dll", SetLastError = true)]
        static extern bool CryptEncodeObject(int dwCertEncodingType, int lpszStructType, byte[] pvStructInfo, byte[] pbEncoded, ref int pcbEncoded);
        
        /// <summary>
        /// Defines the application entrypoint.
        /// </summary>
        static void Main()
        {
            var rsaCSP = new RSACryptoServiceProvider(1024);

            byte[] privateKey = rsaCSP.ExportCspBlob(true);
            byte[] publicKey = rsaCSP.ExportCspBlob(false);

            rsaCSP.Dispose();

            // Save our blobs
            if (Properties.Settings.Default.GenerateBlobs)
            {
                bool encode = Properties.Settings.Default.Base64EncodedBlobs;

                File.WriteAllText(Properties.Settings.Default.PublicKeyBlobFileName, encode ? Convert.ToBase64String(publicKey) : Encoding.Default.GetString(publicKey));
                File.WriteAllText(Properties.Settings.Default.PrivateKeyBlobFileName, encode ? Convert.ToBase64String(privateKey) : Encoding.Default.GetString(privateKey));
            }

            // Generate a PEM
            if (Properties.Settings.Default.GeneratePem)
            {
                // Encode our private key
                int privateKeyEncodedSize = 0;
                if (!CryptEncodeObject(PKCS_7_ASN_ENCODING, PKCS_RSA_PRIVATE_KEY, privateKey, null, ref privateKeyEncodedSize))
                {
                    Console.WriteLine("CryptEncodeObject failed because \"{0}\"", Marshal.GetLastWin32Error());
                    Console.ReadKey();
                    return;
                }

                byte[] privateKeyEncoded = new byte[privateKeyEncodedSize];
                if (!CryptEncodeObject(PKCS_7_ASN_ENCODING, PKCS_RSA_PRIVATE_KEY, privateKey, privateKeyEncoded, ref privateKeyEncodedSize))
                {
                    Console.WriteLine("CryptEncodeObject failed because \"{0}\"", Marshal.GetLastWin32Error());
                    Console.ReadKey();
                    return;
                }

                // Save the pem
                string privateKeyPem = "-----BEGIN RSA PRIVATE KEY-----\r\n"              +
                                       Convert.ToBase64String(privateKeyEncoded) + "\r\n" +
                                       "-----END RSA PRIVATE KEY-----\r\n"                ;

                File.WriteAllText(Properties.Settings.Default.PrivateKeyPemFileName, privateKeyPem);
            }
        }
    }
}
