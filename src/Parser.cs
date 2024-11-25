using System;
using System.Collections.Generic;

namespace StarByte.Compiler
{
    public class Parser
    {
        private Lexer _lexer;
        private Token _currentToken;
        private List<string> _variableDeclarations;
        private List<string> _assignments;
        private List<string> _ifStatements;
        private List<string> _exitStatements;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.NextToken();
            _variableDeclarations = new List<string>();
            _assignments = new List<string>();
            _ifStatements = new List<string>();
            _exitStatements = new List<string>();
        }

        private void Consume()
        {
            _currentToken = _lexer.NextToken();
        }

        public void Parse()
        {
            while (_currentToken.Type != TokenType.EndOfFile)
            {
                ParseStatement();
            }
        }

        private void ParseStatement()
        {
            switch (_currentToken.Type)
            {
                case TokenType.Keyword:
                    if (_currentToken.Value == "int" || _currentToken.Value == "float" || _currentToken.Value == "string")
                    {
                        ParseVariableDeclaration();
                    }
                    else if (_currentToken.Value == "if")
                    {
                        ParseIfStatement();
                    }
                    else if (_currentToken.Value == "exit")
                    {
                        ParseExitStatement();
                    }
                    else
                    {
                        HandleError($"Exitcode 3: Unknown command '{_currentToken.Value}'", 3);
                    }
                    break;

                case TokenType.Identifier:
                    ParseExpression();
                    break;

                default:
                    HandleError($"Exitcode 1: Unexpected token: {_currentToken.Value}", 1);
                    break;
            }
        }

        private void ParseExitStatement()
        {
            Consume();
            
            if (_currentToken.Type != TokenType.LeftParen)
            {
                HandleError("Exitcode 2: Expected '(' after keyword in exit statement.", 2);
            }
            
            Consume();

            if (_currentToken.Type != TokenType.Identifier)
            {
                HandleError("Exitcode 2: Expected integer after '(' in exit statement.", 2);
            }
    
            string exitValue = _currentToken.Value;
            _exitStatements.Add(exitValue);
            Consume();

            if (_currentToken.Type != TokenType.RightParen)
            {
                HandleError("Exitcode 2: Parenthesis ')' not closed.", 2);
            }

            Consume();
            
            if (_currentToken.Type != TokenType.Semicolon)
            {
                HandleError("Exitcode 2: Missing ';' at the end of the line.", 2);
            }
            
            Consume();
        }
        
        private void ParseVariableDeclaration()
        {
            string type = _currentToken.Value;
            Consume();
            string name = _currentToken.Value;
            Consume();

            int? value = null;

            if (_currentToken.Value == "=")
            {
                int? result = null;
                
                Consume();
                if (_currentToken.Type == TokenType.Quotation)
                {
                    Consume();
                    string buf = "";
                    while (_currentToken.Type != TokenType.Quotation)
                    {
                        buf += _currentToken.Value;
                        if (_currentToken.Type == TokenType.Quotation)
                        {
                            HandleError("Quotation was never closed.",1);
                        }
                        Consume();
                    }

                    Consume();

                    bool s = Int32.TryParse(buf, out int result_new);
                    
                    if (s == true)
                        result = result_new;
                }
                else
                {
                    bool s = Int32.TryParse(_currentToken.Value, out int result_new);
                    
                    if (s == true)
                        result = result_new;
                }

                if (result != null)
                {
                    value = result;
                }
                else
                {
                    HandleError("Exitcode 1: Expected '=' after variable declaration.", 3);
                }
            }
            
            if (_currentToken.Type != TokenType.Semicolon)
            {
                HandleError("Exitcode 2: Missing semicolon at the end of the statement.", 2);
            }
            
            Consume();
            _variableDeclarations.Add($"{type};{name};{value}");
        }

        private void ParseExpression()
        {
            string varName = _currentToken.Value;
            Consume();

            if (_currentToken.Type == TokenType.Assignment)
            {
                Consume();
                string value = _currentToken.Value;
                Consume();

                _assignments.Add($"Assign {value} to {varName}");
            }

            if (_currentToken.Type != TokenType.Semicolon)
            {
                HandleError("Exitcode 2: Missing semicolon after assignment.", 2);
            }
            Consume();
        }

        private void ParseIfStatement()
        {
            Consume();
            Consume();
            string condition = _currentToken.Value;
            Consume();
            Consume();

            _ifStatements.Add($"If statement with condition {condition}");
            ParseBlock();
        }

        private void ParseBlock()
        {
            Consume();
            while (_currentToken.Type != TokenType.RBrace)
            {
                if (_currentToken.Type == TokenType.EndOfFile)
                {
                    HandleError("Exitcode 2: Missing closing brace '}' for block.", 2);
                }
                ParseStatement();
            }
            Consume();
        }

        public void HandleError(string message, int exitCode)
        {
            Console.WriteLine(message);
            Environment.Exit(exitCode);
        }

        public List<string> GetVariableDeclarations() => _variableDeclarations;
        public List<string> GetAssignments() => _assignments;
        public List<string> GetIfStatements() => _ifStatements;
        public List<string> GetExitStatements() => _exitStatements;
    }
}
