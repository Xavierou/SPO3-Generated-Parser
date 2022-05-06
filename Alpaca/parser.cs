using lexer;
using System;
using System.Collections.Generic;

using Alpaca;

namespace parser {
public class Parser {
  private readonly bool debug;
  private Stack<(uint state, dynamic value)> stack = new Stack<(uint state, dynamic value)>();
  private static uint[,] Action = new uint[,] {
    {36,16,36,36,36,36,24,36,36,36,22,23,6},
    {37,36,36,36,36,36,36,36,36,36,36,36,36},
    {1,36,36,36,36,36,36,36,36,36,36,36,36},
    {38,36,36,36,27,36,26,36,36,36,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {39,7,36,36,36,36,36,36,11,36,36,36,36},
    {36,36,36,36,36,36,36,36,36,36,5,36,36},
    {36,36,36,36,36,36,36,36,36,36,19,36,36},
    {36,36,36,36,36,36,36,36,4,36,36,36,36},
    {36,36,8,36,36,36,36,36,36,36,36,36,36},
    {40,36,36,36,27,36,26,36,36,36,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {41,36,36,36,27,36,26,36,36,36,36,36,36},
    {42,36,42,33,42,36,42,32,36,36,36,36,36},
    {43,36,43,43,43,36,43,43,36,30,36,36,36},
    {44,36,44,44,44,36,44,44,36,36,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {45,36,45,45,45,36,45,45,36,45,36,36,36},
    {36,36,17,36,27,36,26,36,36,36,36,36,36},
    {36,36,46,36,36,20,36,36,36,36,36,36,36},
    {36,36,36,36,36,36,36,36,36,36,19,36,36},
    {36,36,47,36,36,36,36,36,36,36,36,36,36},
    {48,36,48,48,48,36,48,48,36,48,36,36,36},
    {49,36,49,49,49,36,49,49,36,49,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {50,36,50,50,50,36,50,50,36,50,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {51,36,51,33,51,36,51,32,36,36,36,36,36},
    {52,36,52,33,52,36,52,32,36,36,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {53,36,53,53,53,36,53,53,36,36,36,36,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {36,16,36,36,36,36,24,36,36,36,22,23,36},
    {54,36,54,54,54,36,54,54,36,36,36,36,36},
    {55,36,55,55,55,36,55,55,36,36,36,36,36}
  };
  private static uint[,] GOTO = new uint[,] {
    {3,15,2,13,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {10,15,0,13,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,9},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {12,15,0,13,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {18,15,0,13,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,21},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,25,0},
    {0,0,0,0,0,0},
    {0,15,0,28,14,0},
    {0,15,0,29,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,31,0,0,14,0},
    {0,0,0,0,0,0},
    {0,34,0,0,14,0},
    {0,35,0,0,14,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0}
  };
  private uint top() {
    return stack.Count == 0 ? 0 : stack.Peek().state;
  }
  static string[] stateNames = new string[] {".","%eof","S","E","assign","id","key","lparen","rparen","A","E","assign","E","T","V","F","lparen","rparen","E","id","comma","A","id","number","minus","V","minus","plus","T","T","pow","F","div","mul","F","F"};
  static string[] expectedSyms = new string[] {"S","%eof","%eof","%eof/minus/plus","E","lparen/assign/%eof","id/id/id","A","assign","rparen","%eof/minus/plus","E","%eof/minus/plus","%eof/minus/plus/rparen/div/mul","%eof/div/minus/mul/plus/rparen/pow","%eof/div/minus/mul/plus/rparen","E","%eof/div/minus/mul/plus/pow/rparen","rparen/minus/plus","rparen/comma","A","rparen","%eof/div/minus/mul/plus/pow/rparen","%eof/div/minus/mul/plus/pow/rparen","V","%eof/div/minus/mul/plus/pow/rparen","T","T","%eof/minus/plus/rparen/div/mul","%eof/minus/plus/rparen/div/mul","F","%eof/div/minus/mul/plus/rparen","F","F","%eof/div/minus/mul/plus/rparen","%eof/div/minus/mul/plus/rparen"};

  public Parser(bool debug = false) {
    this.debug = debug;
  }
  public dynamic parse(IEnumerable<(TokenType type, dynamic attr)> tokens) {
    stack.Clear();
    var iter = tokens.GetEnumerator();
    iter.MoveNext();
    var a = iter.Current;
    while (true) {
      var action = Action[top(), (int)a.type];
      switch (action) {
      case 37: {
          stack.Pop();
          return stack.Pop().value;
        }
      case 46: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id");
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<string>() { _1.TokenContent })));
          break;
        }
      case 47: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id comma A");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<string>(_3.TokenContent) { _1 })));
          break;
        }
      case 51: {
          if(debug) Console.Error.WriteLine("Reduce using E -> E minus T");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 0 /*E*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_2, _1, _3))));
          break;
        }
      case 52: {
          if(debug) Console.Error.WriteLine("Reduce using E -> E plus T");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 0 /*E*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_2, _1, _3))));
          break;
        }
      case 42: {
          if(debug) Console.Error.WriteLine("Reduce using E -> T");
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 0 /*E*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(_1)));
          break;
        }
      case 43: {
          if(debug) Console.Error.WriteLine("Reduce using F -> V");
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 1 /*F*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(_1)));
          break;
        }
      case 53: {
          if(debug) Console.Error.WriteLine("Reduce using F -> V pow F");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 1 /*F*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_2, _1, _3))));
          break;
        }
      case 39: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id");
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,((_1 == 'c') ? FuncParser.ComputeFunc(_2.TokenContent) : throw new Exception("':c' key should be used to call a function."))));
          break;
        }
      case 41: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id assign E");
          dynamic _4=stack.Pop().value;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,((_1 == 'a') ? FuncParser.DefineVar(_2.TokenContent, new Token("Number",TokenNode.ComputeTree(_4).ToString())) : throw new Exception("':a' key should be used to assign."))));
          break;
        }
      case 40: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id lparen A rparen assign E");
          dynamic _7=stack.Pop().value;
          var _6=stack.Pop().value.Item2;
          var _5=stack.Pop().value.Item2;
          dynamic _4=stack.Pop().value;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,((_1 == 'a') ? FuncParser.DefineFunc(_2.TokenContent, new Func(_7, _4)) : throw new Exception("':a' key should be used to assign."))));
          break;
        }
      case 38: {
          if(debug) Console.Error.WriteLine("Reduce using S -> E");
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,($"Tree: {_1}\nValue: {TokenNode.ComputeTree(_1)}")));
          break;
        }
      case 44: {
          if(debug) Console.Error.WriteLine("Reduce using T -> F");
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 3 /*T*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(_1)));
          break;
        }
      case 54: {
          if(debug) Console.Error.WriteLine("Reduce using T -> T div F");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 3 /*T*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_2, _1, _3))));
          break;
        }
      case 55: {
          if(debug) Console.Error.WriteLine("Reduce using T -> T mul F");
          dynamic _3=stack.Pop().value;
          var _2=stack.Pop().value.Item2;
          dynamic _1=stack.Pop().value;
          var gt = GOTO[top(), 3 /*T*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_2, _1, _3))));
          break;
        }
      case 48: {
          if(debug) Console.Error.WriteLine("Reduce using V -> id");
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 4 /*V*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_1))));
          break;
        }
      case 45: {
          if(debug) Console.Error.WriteLine("Reduce using V -> lparen E rparen");
          var _3=stack.Pop().value.Item2;
          dynamic _2=stack.Pop().value;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 4 /*V*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(_2)));
          break;
        }
      case 50: {
          if(debug) Console.Error.WriteLine("Reduce using V -> minus V");
          dynamic _2=stack.Pop().value;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 4 /*V*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_1, null, _2))));
          break;
        }
      case 49: {
          if(debug) Console.Error.WriteLine("Reduce using V -> number");
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 4 /*V*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_1))));
          break;
        }
      case 36: {
          string parsed=stateNames[top()];
          var lastSt = top();
          while(stack.Count > 0) { stack.Pop(); parsed = stateNames[top()] + " " + parsed; }
          throw new ApplicationException(
            $"Rejection state reached after parsing \"{parsed}\", when encoutered symbol \""
            + $"\"{a.type}\" in state {lastSt}. Expected \"{expectedSyms[lastSt]}\"");
        }
      default:
        if(debug) Console.Error.WriteLine($"Shift to {action}");
        stack.Push((action, a));
        iter.MoveNext();
        a=iter.Current;
        break;
      }
    }
  }
}
}