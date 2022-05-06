// See https://aka.ms/new-console-template for more information
using parser;
using lexer;

Console.WriteLine("Введите анализируемую строку. Для остановки ввода введите stop:\n=> ");
string input = Console.ReadLine();

do
{
    try
    {
        Parser parser = new();
        Console.WriteLine(parser.parse(Lexer.lex(input)) + "\n=> ");
        input = Console.ReadLine();
    }
    catch (Exception exc)
    {
        Console.WriteLine(exc.Message);
        input = Console.ReadLine();
    }
} while (!input.Equals("stop"));