using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaca
{
    public static class FuncParser
    {
        //Словари, связывающие название функции/переменной и их значения.
        public static Dictionary<string, Func> functions = new Dictionary<string, Func>();
        public static Dictionary<string, Token> variables = new Dictionary<string, Token>();

        /// <summary>
        /// Функция, возвращающая класс функции из словаря функций.
        /// </summary>
        /// <param name="funcName">Имя функции</param>
        /// <returns>Класс функции, соответствующий его имени.</returns>
        /// <exception cref="Exception">Функция не была определена</exception>
        private static Func GetFunc(string funcName)
        {
            Func func;
            if (functions.TryGetValue(funcName, out func))
            {
                return func;
            }
            throw new Exception($"Function not defined: {funcName}");
        }

        /// <summary>
        /// Функция, отвечающая за добавление переменной.
        /// </summary>
        /// <param name="id">Имя переменной</param>
        /// <param name="num">Значение (токен) переменной</param>
        /// <returns>Сообщение об успехе.</returns>
        public static string DefineVar(string id, Token num)
        {
            if (!variables.TryAdd(id, num))
            {
                variables.Remove(id);
                variables.Add(id, num);
                return $"Re-defined variable {id} to {num.TokenContent}.";
            }
            return $"Successfully defined variable: {id} = {num.TokenContent}";
        }

        /// <summary>
        /// Функция, отвечающая за определение функций.
        /// </summary>
        /// <param name="id">Имя функции</param>
        /// <param name="func">Класс функции (передается из строки в виде тела функции и аргументов).</param>
        /// <returns>Сообщение об успехе.</returns>
        public static string DefineFunc(string id, Func func)
        {
            if (!functions.TryAdd(id, func))   
            {
                functions.Remove(id);
                functions.Add(id, func);
                return $"Re-defined function {id}";
            }
            return $"Successfully defined function {id}";
        }

        /// <summary>
        /// Функция, определяющая, можно ли посчитать сейчас функцию или нет.
        /// </summary>
        /// <param name="funcName">Имя функции</param>
        /// <returns>true, если функцию можно посчитать.</returns>
        private static bool CanBeComputed(string funcName)
        {
            bool output = true;
            Func func = GetFunc(funcName);

            string[] funcVars = func.Args;
            foreach (string variable in funcVars)
            {
                if (!variables.ContainsKey(variable))
                {
                    output = false;
                }
            }

            return output;
        }

        /// <summary>
        /// Функция, добавляющая в узел с переменной значение переменной.
        /// </summary>
        /// <param name="tree">Тело функции</param>
        /// <param name="variable">Имя переменной, значение которой надо добавить</param>
        private static void ReplaceWithValue(TokenNode tree, string variable)
        {
            Token varToken;
            variables.TryGetValue(variable, out varToken);
            if (tree.Value.TokenContent.Equals(variable))
            {
                tree.VarValue = varToken;
            }

            if (tree.Left != null)
            {
                ReplaceWithValue(tree.Left, variable);
            }

            if (tree.Right != null)
            {
                ReplaceWithValue(tree.Right, variable);
            }
        }

        /// <summary>
        /// Функция, отвечающая за вычисление значения функции.
        /// </summary>
        /// <param name="func">Имя вычисляемой функции</param>
        /// <returns>Результат выполнения функции</returns>
        /// <exception cref="Exception">В случае если не все переменные были объявленыы</exception>
        public static string ComputeFunc(string func)
        {
            if (!CanBeComputed(func))
            {
                throw new Exception($"Function {func} cannot be computed because not all variables have been defined.");
            }

            Func function = GetFunc(func);
            string[] funcVars = function.Args;
            foreach (string var in funcVars)
            {
                ReplaceWithValue(function.Body, var);
            }

            double output = TokenNode.ComputeTree(function.Body);
            return $"Successfully computed function {func}. Result: {output}";
        }
    }
}
