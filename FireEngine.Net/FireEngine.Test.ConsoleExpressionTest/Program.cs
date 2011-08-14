using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireML.Library;
using FireEngine.FireML.Library.Compiler;
using FireEngine.FireML.Library.Expr;

namespace FireEngine.Test.ConsoleExpressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string exprStr;
            exprStr = Console.ReadLine();

            CompilerKernel kernel = new CompilerKernel(new string[]{}, new string[]{}, null);
            ExpressionParserKernel parser = new ExpressionParserKernel(kernel, exprStr, new FireEngine.FireML.Library.Location("", 0, 0));
            Expression expr = parser.Parse();

            Error[] errors = kernel.CheckPoint();

            foreach (Error e in errors)
            {
                Console.WriteLine("{0}\n{1},{2}\n{3}", e.Location.FileName, e.Location.Line, e.Location.Column, e.Message);
                Console.WriteLine();
            }
        }
    }
}
