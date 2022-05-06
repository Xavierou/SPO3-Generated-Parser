using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaca
{
    public class Token
    {
        public Token(string type, string content)
        {
            tokenType = type;
            tokenContent = content;
        }

        private string tokenType = "Invalid Type";
        private string tokenContent = "Invalid";

        public string TokenType
        {
            get { return tokenType; }
        }
        public string TokenContent
        {
            get { return tokenContent; }
        }

        public override string ToString()
        {
            return "(" + tokenType + "; " + tokenContent + ")";
        }
    }
}
