using System;
using System.Linq;
using System.Text;
using StarByte.Compiler;


public class CodeGenerator
{
    private StringBuilder _assemblyCode;
    private bool _exitEncountered;

    public CodeGenerator()
    {
        _assemblyCode = new StringBuilder();
        _exitEncountered = false;
    }

    public string GenerateAssembly(Parser parser)
    {
        _assemblyCode.AppendLine("section .bss");
        _assemblyCode.AppendLine("        buffer resb 64");
        _assemblyCode.AppendLine("section .data");
        _assemblyCode.AppendLine("        msg db 'Exited with exitcode: ', 0");
        _assemblyCode.AppendLine("        msg_len equ $ -msg");
        _assemblyCode.AppendLine("section .text");
        _assemblyCode.AppendLine("        global _start:");
        _assemblyCode.AppendLine("int_to_string:");
        _assemblyCode.AppendLine("        mov rcx, 10");
        _assemblyCode.AppendLine("        xor rbx, rbx");
        _assemblyCode.AppendLine(".next_digit:");
        _assemblyCode.AppendLine("        xor rdx, rdx");
        _assemblyCode.AppendLine("        div rcx");
        _assemblyCode.AppendLine("        add dl, '0'");
        _assemblyCode.AppendLine("        dec rdi");
        _assemblyCode.AppendLine("        mov [rdi], dl");
        _assemblyCode.AppendLine("        inc rbx");
        _assemblyCode.AppendLine("        test rax, rax");
        _assemblyCode.AppendLine("        jnz .next_digit");
        _assemblyCode.AppendLine("        ret");
        _assemblyCode.AppendLine("_start:");
        

        // exit-Anweisungen
        foreach (var exitStmt in parser.GetExitStatements())
        {
            var use_exitStmt = exitStmt;
            
            if (exitStmt is string)
            {
                var vars = parser.GetVariableDeclarations();
                var parts = vars.First(v => v.Split(";")[1] == exitStmt).Split(";");
                if (parts != null)
                {
                    if (parts[0] == "int" || Int32.TryParse(parts[2], out var i))
                    {
                        use_exitStmt = parts[2];
                    }
                    else
                    {
                        parser.HandleError($"Can not convert {parts[1]} to int.", 10);
                    }
                }   
                else
                {
                    parser.HandleError($"Unknown var {exitStmt}", 9);
                }
            }
            
            _exitEncountered = true;
            _assemblyCode.AppendLine("        mov rax, " + use_exitStmt + "\n");
            _assemblyCode.AppendLine("        lea rdi, [buffer]\n");
            _assemblyCode.AppendLine("        call int_to_string\n");
            _assemblyCode.AppendLine("        mov rax, 1\n");
            _assemblyCode.AppendLine("        mov rdi, 1\n");
            _assemblyCode.AppendLine("        lea rsi, [msg]\n");
            _assemblyCode.AppendLine("        mov rdx, msg_len\n");
            _assemblyCode.AppendLine("        syscall\n");
            _assemblyCode.AppendLine("        mov rax, 1\n");
            _assemblyCode.AppendLine("        mov rdi, 1\n");
            _assemblyCode.AppendLine("        lea rsi, [buffer]\n");
            _assemblyCode.AppendLine("        mov rdx, rbx\n");
            _assemblyCode.AppendLine("        syscall\n");
            _assemblyCode.AppendLine("        mov rax, 60\n");
            _assemblyCode.AppendLine("        mov rdi, " + use_exitStmt + "\n");
            _assemblyCode.AppendLine("        syscall\n");
            break;
        }

        if (_exitEncountered == false)
        {
            _assemblyCode.AppendLine("        mov rax, 0\n");
            _assemblyCode.AppendLine("        lea rdi, [buffer]\n");
            _assemblyCode.AppendLine("        call int_to_string\n");
            _assemblyCode.AppendLine("        mov rax, 1\n");
            _assemblyCode.AppendLine("        mov rdi, 1\n");
            _assemblyCode.AppendLine("        lea rsi, [msg]\n");
            _assemblyCode.AppendLine("        mov rdx, msg_len\n");
            _assemblyCode.AppendLine("        syscall\n");
            _assemblyCode.AppendLine("        mov rax, 1\n");
            _assemblyCode.AppendLine("        mov rdi, 1\n");
            _assemblyCode.AppendLine("        lea rsi, [buffer]\n");
            _assemblyCode.AppendLine("        mov rdx, rbx\n");
            _assemblyCode.AppendLine("        syscall\n");
            _assemblyCode.AppendLine("        mov rax, 60\n");
            _assemblyCode.AppendLine("        mov rdi, 0\n");
            _assemblyCode.AppendLine("        syscall\n");
        }

        return _assemblyCode.ToString();
    }
}
