using System.Text;
using System.Security.Cryptography;

namespace Toolbox.KeyGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int size = 1024;
            string path = Directory.GetCurrentDirectory();

            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                path = args[0];
            }

            if (args.Length > 1 && int.TryParse(args[1], out int result))
            {
                size = result;
            }

            Console.Write("status: generating keys...");

            using RSACryptoServiceProvider rsa = new(size);

            byte[] publicKeyBlob = rsa.ExportCspBlob(false);
            byte[] privateKeyBlob = rsa.ExportCspBlob(true);

            string publicKeyPem = rsa.ExportRSAPublicKeyPem();
            string privateKeyPem = rsa.ExportRSAPrivateKeyPem();

            Console.WriteLine("done!");
            
            File.WriteAllBytes(Path.Combine(path, "public.bin"), Encoding.UTF8.GetBytes(Convert.ToBase64String(publicKeyBlob)));
            File.WriteAllBytes(Path.Combine(path, "private.bin"), Encoding.UTF8.GetBytes(Convert.ToBase64String(privateKeyBlob)));

            File.WriteAllBytes(Path.Combine(path, "public.pem"), Encoding.UTF8.GetBytes(publicKeyPem));
            File.WriteAllBytes(Path.Combine(path, "private.pem"), Encoding.UTF8.GetBytes(privateKeyPem));

            Console.WriteLine("success: wrote all keys!");
        }
    }
}
