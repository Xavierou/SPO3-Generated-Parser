using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaca
{
    public class Variable
    {
        public string Type { get; set; }
        public Token Value { get; set; }
        public Variable (Token value)
        {
            Value = value;
            Type = "double";
        }

        public Variable (string type, Token value)
        {
            Type = type;
            Value = value;
        }
    }
}
