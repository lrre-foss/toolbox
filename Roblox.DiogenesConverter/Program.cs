using System;
using System.Text;
using System.IO;
using System.Reflection;

namespace Roblox.DiogenesConverter
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
            
            foreach (string filename in args)
            {
                if (!File.Exists(filename) || !File.Exists(Path.Combine(currentPath, filename)))
                {
                    Console.WriteLine("File \"{0}\" does not exist, skipping...", filename);
                    return;
                }

                Console.Write("Converting \"{0}\"... ", filename);

                string output = Properties.Settings.Default.GenerateNewFiles ? Path.GetFileName(filename) + Properties.Settings.Default.GeneratedNewFileSuffix : filename;
                string input = File.ReadAllText(filename);
                string result = String.Empty;

                byte[] bytes = Encoding.Default.GetBytes(input);
                foreach (byte singular in bytes)
                {
                    result += Convert.ToChar(Properties.Settings.Default.XorBy ^ singular);
                }

                File.WriteAllText(output, result);
                Console.WriteLine("Finished!");
            }

            Console.WriteLine("Finished converting!");
            Console.ReadKey();
        }
    }
}
