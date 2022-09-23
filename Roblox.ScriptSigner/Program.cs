using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;

namespace Roblox.ScriptSigner
{
    class Program
    {
        /// <summary>
        /// Defines the application entrypoint.
        /// </summary>
        /// <param name="args">Application arguments</param>
        static void Main(string[] args)
        {
            string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string format = Properties.Settings.Default.NewSignatureFormat ? "--rbxsig%{0}%{1}" : "%{0}%{1}";
            byte[] blob = Encoding.Default.GetBytes(File.ReadAllText(Path.Combine(currentPath, Properties.Settings.Default.PrivateKeyBlobFileName)));

            if (Properties.Settings.Default.PrivateKeyBlobIsBase64)
            {
                blob = Convert.FromBase64String(Encoding.Default.GetString(blob));
            }

            var shaCSP = new SHA1CryptoServiceProvider();
            var rsaCSP = new RSACryptoServiceProvider();
            rsaCSP.ImportCspBlob(blob);
            
            foreach (string filename in args)
            {
                if (!File.Exists(filename) || !File.Exists(Path.Combine(currentPath, filename)))
                {
                    Console.WriteLine("File \"{0}\" does not exist, skipping...", filename);
                    return;
                }

                Console.Write("Signing \"{0}\"... ", filename);

                string output = Properties.Settings.Default.GenerateNewFiles ? Path.GetFileName(filename) + Properties.Settings.Default.GeneratedNewFileSuffix : filename;
                string script = "\r\n" + File.ReadAllText(filename);
                byte[] signature = rsaCSP.SignData(Encoding.Default.GetBytes(script), shaCSP);

                File.WriteAllText(output, String.Format(format, Convert.ToBase64String(signature), script));

                Console.WriteLine("Finished!");
            }

            Console.WriteLine("Finished signing scripts!");
            Console.ReadKey();
        }
    }
}
