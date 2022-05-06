using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaca
{
    public class TokenNode
    {
        public TokenNode(Token token)
        {
            Value = token;
        }
        public TokenNode(Token token, TokenNode left, TokenNode right)
        {
            Value = token;
            Left = left;
            Right = right;
        }
        public Token Value { get; set; }
        
        //Поле для узла, в котором содержится переменная. В него попадает значение переменной.
        public Token VarValue { get; set; }
        public TokenNode Left { get; set; }
        public TokenNode Right { get; set; }

        public static double ComputeTree(TokenNode tree)
        {
            double output = 0;
            double left = 0;
            double right = 0;
            if (tree.Left != null)
            {
                left = ComputeTree(tree.Left);
            }
            else
            {
                if (tree.VarValue != null)
                {
                    output = double.Parse(tree.VarValue.TokenContent.Replace('.', ','));
                }
                else 
                {
                    output = double.Parse(tree.Value.TokenContent.Replace('.', ','));
                }
            }

            if (tree.Right != null)
            {
                right = ComputeTree(tree.Right);
            }

            if (tree.Value.TokenType.Equals("Operator"))
            {
                switch (tree.Value.TokenContent)
                {
                    case "+":
                        output = left + right;
                        break;
                    case "-":
                        if (tree.Right != null)
                        {
                            output = left - right;
                        }
                        else
                        {
                            output = -left;
                        }
                        break;
                    case "*":
                        output = left * right;
                        break;
                    case "/":
                        output = left / right;
                        break;
                    case "^":
                        output = Math.Pow(left, right);
                        break;
                    default:
                        throw new ArgumentException("Bad tree: " + tree + " at token " + tree.Value.ToString());
                }
            }

            return output;
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append(Value.TokenContent);

            if (Left != null || Right != null)
            {
                output.Append("(");
                if (Left != null)
                {
                    output.Append(Left);
                }
                if (Right != null)
                {
                    output.Append(", " + Right);
                }
                output.Append(")");
            }
            return output.ToString();
        }
    }
}
