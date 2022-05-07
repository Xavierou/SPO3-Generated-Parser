using lexer;
using System;
using System.Collections.Generic;

using Alpaca;

namespace parser {
public class Parser {
  private readonly bool debug;
  private Stack<(uint state, dynamic value)> stack = new Stack<(uint state, dynamic value)>();
  private static uint[,] Action = new uint[,] {
    {48,22,48,48,48,48,36,48,48,48,34,35,6,48,48},
    {49,48,48,48,48,48,48,48,48,48,48,48,48,48,48},
    {1,48,48,48,48,48,48,48,48,48,48,48,48,48,48},
    {50,48,48,48,39,48,38,48,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {48,7,48,48,48,48,48,48,17,48,48,48,48,14,13},
    {48,48,48,48,48,48,48,48,48,48,5,48,48,48,48},
    {48,48,51,48,48,48,48,48,48,48,25,48,48,48,48},
    {52,48,48,48,48,48,48,48,4,48,48,48,48,48,48},
    {48,48,8,48,48,48,48,48,48,48,48,48,48,48,48},
    {53,48,48,48,39,48,38,48,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {48,48,48,48,48,48,48,48,11,48,48,48,48,48,48},
    {48,48,48,48,48,48,48,48,12,48,48,48,48,48,48},
    {54,48,48,48,39,48,38,48,48,48,48,48,48,48,48},
    {55,48,48,48,39,48,38,48,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {56,48,48,48,39,48,38,48,48,48,48,48,48,48,48},
    {57,48,57,45,57,48,57,44,48,48,48,48,48,48,48},
    {58,48,58,58,58,48,58,58,48,42,48,48,48,48,48},
    {59,48,59,59,59,48,59,59,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {60,48,60,60,60,48,60,60,48,60,48,48,48,48,48},
    {48,48,23,48,39,48,38,48,48,48,48,48,48,48,48},
    {48,48,61,48,48,28,48,48,48,48,48,48,48,27,26},
    {48,48,62,48,48,30,48,48,48,48,48,48,48,48,48},
    {48,48,63,48,48,31,48,48,48,48,48,48,48,48,48},
    {48,48,51,48,48,48,48,48,48,48,25,48,48,48,48},
    {48,48,64,48,48,48,48,48,48,48,48,48,48,48,48},
    {48,48,51,48,48,48,48,48,48,48,25,48,48,48,48},
    {48,48,51,48,48,48,48,48,48,48,25,48,48,48,48},
    {48,48,65,48,48,48,48,48,48,48,48,48,48,48,48},
    {48,48,66,48,48,48,48,48,48,48,48,48,48,48,48},
    {67,48,67,67,67,48,67,67,48,67,48,48,48,48,48},
    {68,48,68,68,68,48,68,68,48,68,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {69,48,69,69,69,48,69,69,48,69,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {70,48,70,45,70,48,70,44,48,48,48,48,48,48,48},
    {71,48,71,45,71,48,71,44,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {72,48,72,72,72,48,72,72,48,48,48,48,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {48,22,48,48,48,48,36,48,48,48,34,35,48,48,48},
    {73,48,73,73,73,48,73,73,48,48,48,48,48,48,48},
    {74,48,74,74,74,48,74,74,48,48,48,48,48,48,48}
  };
  private static uint[,] GOTO = new uint[,] {
    {3,21,2,19,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {10,21,0,19,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,9},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {15,21,0,19,20,0},
    {16,21,0,19,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {18,21,0,19,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {24,21,0,19,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,29},
    {0,0,0,0,0,0},
    {0,0,0,0,0,32},
    {0,0,0,0,0,33},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,0,0,0,37,0},
    {0,0,0,0,0,0},
    {0,21,0,40,20,0},
    {0,21,0,41,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0},
    {0,43,0,0,20,0},
    {0,0,0,0,0,0},
    {0,46,0,0,20,0},
    {0,47,0,0,20,0},
    {0,0,0,0,0,0},
    {0,0,0,0,0,0}
  };
  private uint top() {
    return stack.Count == 0 ? 0 : stack.Peek().state;
  }
  static string[] stateNames = new string[] {".","%eof","S","E","assign","id","key","lparen","rparen","A","E","assign","assign","typedouble","typeint","E","E","assign","E","T","V","F","lparen","rparen","E","id","typedouble","typeint","comma","A","comma","comma","A","A","id","number","minus","V","minus","plus","T","T","pow","F","div","mul","F","F"};
  static string[] expectedSyms = new string[] {"S","%eof","%eof","%eof/minus/plus","E","lparen/typedouble/typeint/assign/lparen","id/id/id/id/id","A/A","assign/%eof","rparen/rparen","%eof/minus/plus","E","E","assign","assign","%eof/minus/plus","%eof/minus/plus","E","%eof/minus/plus","%eof/minus/plus/rparen/div/mul","%eof/div/minus/mul/plus/rparen/pow","%eof/div/minus/mul/plus/rparen","E","%eof/div/minus/mul/plus/pow/rparen","rparen/minus/plus","rparen/typedouble/typeint/comma/typedouble/typeint","rparen/comma","rparen/comma","A","rparen","A","A","rparen","rparen","%eof/div/minus/mul/plus/pow/rparen","%eof/div/minus/mul/plus/pow/rparen","V","%eof/div/minus/mul/plus/pow/rparen","T","T","%eof/minus/plus/rparen/div/mul","%eof/minus/plus/rparen/div/mul","F","%eof/div/minus/mul/plus/rparen","F","F","%eof/div/minus/mul/plus/rparen","%eof/div/minus/mul/plus/rparen"};

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
      case 49: {
          stack.Pop();
          return stack.Pop().value;
        }
      case 51: {
          if(debug) Console.Error.WriteLine("Reduce using A -> ");
          
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(null)));
          break;
        }
      case 61: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id");
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<Token>() { new Token("double", _1.TokenContent) })));
          break;
        }
      case 64: {
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
          stack.Push((gt,(new List<Token>(_3) { new Token("double", _1.TokenContent) })));
          break;
        }
      case 62: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id typedouble");
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<Token>() { new Token(_2, _1.TokenContent) })));
          break;
        }
      case 65: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id typedouble comma A");
          dynamic _4=stack.Pop().value;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<Token>(_4) { new Token(_2, _1.TokenContent) })));
          break;
        }
      case 63: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id typeint");
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<Token>() { new Token(_2, _1.TokenContent) })));
          break;
        }
      case 66: {
          if(debug) Console.Error.WriteLine("Reduce using A -> id typeint comma A");
          dynamic _4=stack.Pop().value;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 5 /*A*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new List<Token>(_4) { new Token(_2, _1.TokenContent) })));
          break;
        }
      case 70: {
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
      case 71: {
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
      case 57: {
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
      case 58: {
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
      case 72: {
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
      case 56: {
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
          stack.Push((gt,((_1 == 'a') ? FuncParser.DefineVar(_2.TokenContent, new Variable(new Token("Number",TokenNode.ComputeTree(_4).ToString()))) : throw new Exception("':a' key should be used to assign."))));
          break;
        }
      case 52: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id lparen A rparen");
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
          stack.Push((gt,((_1 == 'c') ? FuncParser.ComputeFunc(_2.TokenContent, _4) : throw new Exception("':c' key should be used to call a function."))));
          break;
        }
      case 53: {
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
      case 54: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id typedouble assign E");
          dynamic _5=stack.Pop().value;
          var _4=stack.Pop().value.Item2;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,((_1 == 'a') ? FuncParser.DefineVar(_2.TokenContent, new Variable(_3, new Token("Number",TokenNode.ComputeTree(_5).ToString()))) : throw new Exception("':a' key should be used to assign."))));
          break;
        }
      case 55: {
          if(debug) Console.Error.WriteLine("Reduce using S -> key id typeint assign E");
          dynamic _5=stack.Pop().value;
          var _4=stack.Pop().value.Item2;
          var _3=stack.Pop().value.Item2;
          var _2=stack.Pop().value.Item2;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 2 /*S*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,((_1 == 'a') ? FuncParser.DefineVar(_2.TokenContent, new Variable(_3, new Token("Number",TokenNode.ComputeTree(_5).ToString()))) : throw new Exception("':a' key should be used to assign."))));
          break;
        }
      case 50: {
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
      case 59: {
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
      case 73: {
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
      case 74: {
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
      case 67: {
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
      case 60: {
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
      case 69: {
          if(debug) Console.Error.WriteLine("Reduce using V -> minus V");
          dynamic _2=stack.Pop().value;
          var _1=stack.Pop().value.Item2;
          var gt = GOTO[top(), 4 /*V*/];
          if(gt==0) throw new ApplicationException("No goto");
          if(debug) {
            Console.Error.WriteLine($"{top()} is now on top of the stack;");
            Console.Error.WriteLine($"{gt} will be placed on the stack");
          }
          stack.Push((gt,(new TokenNode(_1, new TokenNode(new Token("Number", "0")), _2))));
          break;
        }
      case 68: {
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
      case 48: {
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