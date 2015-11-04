using System;
using System.IO;

namespace Markdown
{
    class Program
    {
        static void ShowUsage()
        {
            Console.WriteLine(
                "Simple converter of Markdown-like markup to HTML\n" +
                "\n" +
                "Usage:\n" +
                "    {0} FILENAME\n" +
                "\n" +
                "Arguments:\n" +
                "    FILENAME    Source file\n" +
                "\n" +
                "Converted file will be placed in the program directory.",
                AppDomain.CurrentDomain.FriendlyName
            );
        }

        static void ShowError(string message)
        {
            Console.Error.WriteLine("[-] Error: {0}", message);
        }

        static void Main(string[] args)
        {
            if (args.Length != 1 || args[0] == "/?")
            {
                ShowUsage();
                return;
            }
            
            try
            {
                var inputPath = args[0];
                var markupSource = File.ReadAllText(inputPath);
                
                var document = new DocumentProcessor(markupSource).Process();
                var htmlSource = new HtmlFormatter(document).FormatDocument();

                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    Path.GetFileNameWithoutExtension(inputPath) + ".html");
                File.WriteAllText(outputPath, htmlSource);

                Console.WriteLine($"[+] Converted from \"{inputPath}\" to \"{outputPath}\"");
            }
            catch (IOException e)
            {
                ShowError(e.Message);
            }
        }
    }
}
