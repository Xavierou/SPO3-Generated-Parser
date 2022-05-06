﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpaca
{
    public class Func
    {
        public TokenNode Body { get; private set; }
        public string[] Args { get; private set; }
        public Func(TokenNode body, List<string>args)
        {
            Body = body;
            Args = args.ToArray();
        }
    }
}
