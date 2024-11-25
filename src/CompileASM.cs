using System;
using System.Diagnostics;

namespace StarByte.Compiler
{
    public class Compile
    {
        public static void CompileASMToExecutable(string path, string name)
        {
            Console.Write("Compiling");

            Process process = new Process();
            process.StartInfo.FileName = "wsl.exe";

            process.StartInfo.Arguments = $"cd {path} && nasm -f elf64 -o {name}.o {name}.asm";
            process.Start();
            process.WaitForExit();
            Console.Write(".");

            process.StartInfo.Arguments = $"cd {path} && ld -o {name} {name}.o";
            process.Start();
            process.WaitForExit();
            Console.Write(".");

            process.StartInfo.Arguments = $"cd {path} && rm -rf {name}.o {name}.asm";
            process.Start();
            process.WaitForExit();
            Console.WriteLine(".");
            Console.WriteLine("Done");
        }
    }
}