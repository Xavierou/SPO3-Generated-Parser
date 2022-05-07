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
        public static Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

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
        public static string DefineVar(string id, Variable num)
        {
            if (!variables.TryAdd(id, num))
            {
                variables.Remove(id);
                variables.Add(id, num);
                return $"Re-defined variable {id} to {num.Value.TokenContent}.";
            }
            return $"Successfully defined variable: {id} = {num.Value.TokenContent}";
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
        /// <param name="args">Список подставляемых аргументов</param>
        /// <returns>true, если функцию можно посчитать.</returns>
        private static bool CanBeComputed(string funcName, List<Token> args)
        {
            bool output = true;
            Func func = GetFunc(funcName);
            foreach (Token arg in args)
            {
                if (!variables.ContainsKey(arg.TokenContent))
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
        /// <param name="variable">Перменная в теле функции</param>
        /// <param name="argument">Подставляемый аргумент</param>
        /// <excepton cref="Exception">В случае, если преобразование типов нельзя совершить</excepton>
        private static void ReplaceWithValue(TokenNode tree, Token variable, Token argument)
        {
            Variable argToken;
            variables.TryGetValue(argument.TokenContent, out argToken);

            if (tree.Value.TokenContent.Equals(variable.TokenContent))
            {
                string varType = variable.TokenType;
                string argType = argToken.Type;
                if (!varType.Equals(argType))
                {
                    if (varType == "double" && argType == "int")
                    {
                        tree.VarValue = argToken.Value;
                        tree.VarValue.TokenContent += ".0";
                    }
                    else
                    {
                        throw new Exception($"Cannot convert type '{argument.TokenType}' of {argument.TokenContent} to the required type 'int' of {variable.TokenContent}");
                    }
                }
                else
                {
                    tree.VarValue = argToken.Value;
                }
            }

            if (tree.Left != null)
            {
                ReplaceWithValue(tree.Left, variable, argument);
            }

            if (tree.Right != null)
            {
                ReplaceWithValue(tree.Right, variable, argument);
            }
        }

        /// <summary>
        /// Функция, отвечающая за вычисление значения функции.
        /// </summary>
        /// <param name="func">Имя вычисляемой функции</param>
        /// <param name="funcArgs">Список подставляемых аргументов</param>
        /// <returns>Результат выполнения функции</returns>
        /// <exception cref="Exception">В случае если не все переменные были объявлены или же было передано
        /// достаточное количество аргументов</exception>
        public static string ComputeFunc(string func, List<Token> funcArgs)
        {
            if (!CanBeComputed(func, funcArgs))
            {
                throw new Exception($"Function {func} cannot be computed because not all variables have been defined.");
            }

            Func function = GetFunc(func);
            Token[] funcVars = function.Args;
            if (funcArgs.Count != funcVars.Length)
            {
                throw new Exception($"Function {func} takes {funcVars.Length} elements, while {funcArgs.Count} elements were given.");
            }
            
            for (int i = 0; i < funcVars.Length; i++)
            {
                ReplaceWithValue(function.Body, funcVars[i], funcArgs[i]);
            }

            double output = TokenNode.ComputeTree(function.Body);
            return $"Successfully computed function {func}. Result: {output}";
        }
    }
}
