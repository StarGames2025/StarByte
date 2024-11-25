using System;
using System.IO;
using StarByte.Compiler;

namespace StarByte
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide the path to the input file.");
                return;
            }

            string inputFilePath = args[0];
            string[] parts = inputFilePath.Replace("\\", "/").Split('/');
            string path = "";
            string name = parts[parts.Length-1].Replace(".sb","");
                
            for (int i = 0; i < parts.Length - 1; i++)
            {
                path += parts[i]+  "/";
            }

            
            if (path == "" || path.Contains("./"))
            {
                path = Directory.GetCurrentDirectory() + "/";    
            }
            
            string[] distros = new string[] { "Arch", "RHEL", "Debian", "openSUSE", "Alpine" };
            foreach (var distro in distros)
            {
                path = path.Replace($"//wsl.localhost/{distro}", "");
            }
            
            string inputCode = File.ReadAllText(inputFilePath);

            Lexer lexer = new Lexer(inputCode);

            Parser parser = new Parser(lexer);
            parser.Parse();

            CodeGenerator codeGenerator = new CodeGenerator();
            string assemblyCode = codeGenerator.GenerateAssembly(parser);

            string outputFilePath = $"{path}{name}.asm";
            File.WriteAllText(outputFilePath, assemblyCode);
            
            Compile.CompileASMToExecutable(path, name);
            
            Console.WriteLine($"{name} has been compiled.");
        }
    }
}