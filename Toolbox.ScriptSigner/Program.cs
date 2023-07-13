using System.Text;
using System.Reflection;
using System.Security.Cryptography;

namespace Toolbox.ScriptSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            string format = "--rbxsig%{0}%{1}";

            using RSACryptoServiceProvider rsa = new();

            Console.WriteLine("status: loading private key from current working directory...");

            if (File.Exists(Path.Combine(path, "private.pem")))
            {
                rsa.ImportFromPem(File.ReadAllText(Path.Combine(path, "private.pem")));
            }
            else
            {
                Console.WriteLine("warn: private.pem not found -- looking for private key blob (private.bin)...");
                
                if (File.Exists(Path.Combine(path, "private.bin")))
                {
                    byte[] bytes = File.ReadAllBytes(Path.Combine(path, "private.bin"));

                    if (bytes[^1] == '=' && bytes[^2] == '=')
                    {
                        bytes = Convert.FromBase64String(Encoding.UTF8.GetString(bytes));
                    }

                    rsa.ImportCspBlob(bytes);
                }
                else
                {
                    Console.WriteLine("error: private key not found!");
                    Environment.Exit(1);
                }
            }

            Console.WriteLine("status: signing all scripts...");

            foreach (string filename in args)
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine($"warn: {filename} not found -- skipping...");
                    continue;
                }

                Console.Write($"status: signing {filename}...");

                string script = "\r\n" + File.ReadAllText(filename);
                byte[] signature = rsa.SignData(Encoding.Default.GetBytes(script), SHA1.Create());

                File.WriteAllText(filename, string.Format(format, Convert.ToBase64String(signature), script));

                Console.WriteLine("signed!");
            }

            Console.WriteLine("success: finished signing all scripts!");
        }
    }
}