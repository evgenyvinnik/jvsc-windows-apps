using System;
using System.Collections.Generic;

namespace PascalInterpreter.Interpreter
{
	public class Codes
	{
		// codes for keywords
		public static readonly byte PROGRAM   = 10;
		public static readonly byte BEGIN = 11;
		public static readonly byte END = 12;
		public static readonly byte END_DOT = 13; // END.
		public static readonly byte VAR = 14;
		public static readonly byte IF = 15;
		public static readonly byte THEN = 16;
		public static readonly byte ELSE = 17;
		public static readonly byte WHILE = 18;
		public static readonly byte DO = 19;
		public static readonly byte NOT = 20;
		public static readonly byte AND = 21;
		public static readonly byte OR = 22;
		public static readonly byte READLN = 23;
		public static readonly byte WRITELN = 24;
		public static readonly byte INTEGER = 25;
		public static readonly byte REAL = 26;
		public static readonly byte BOOLEAN = 27;
		public static readonly byte TRUE = 28;
		public static readonly byte FALSE = 29;

		// codes for signs
		public static readonly byte PLUS = 30; // '+'
		public static readonly byte MINUS = 31; // '-'
		public static readonly byte MULT = 32; // '*'
		public static readonly byte DIV = 33; // '/'
		public static readonly byte E = 34; // EQUAL
		public static readonly byte NE = 35; // NOTEQUAL
		public static readonly byte GT = 36; // GREATER THEN
		public static readonly byte GE = 37; // GREATER OR EQUAL
		public static readonly byte LT = 38; // LESS THEN
		public static readonly byte LE = 39; // LESS OR EQUAL
		public static readonly byte COLON = 40; // ':'
		public static readonly byte SEMICOLON = 41; // ';'
		public static readonly byte COMMA = 42; // ','
		public static readonly byte ASOCIATE = 43; // ':='
		public static readonly byte LBRACKET = 44; // LEFT BRACKET
		public static readonly byte RBRACKET = 45; // RIGHT BRACKET
		public static readonly byte QUOTATION = 46; // QUOTATION MARK

		// codes for lower categories
		public static readonly byte KEYWORD = 60;
		public static readonly byte VARIABLE = 61;
		public static readonly byte VTYPE = 62; // VARIABLE TYPE
		public static readonly byte LOPERATOR = 63; // LOGICAL OPERATOR
		public static readonly byte AOPERATOR = 64; // ARITHMETIC OPERATOR
		public static readonly byte ROPERATOR = 65; // RELATIONAL OPERATOR
		public static readonly byte OOPERATOR = 66; // OTHER OPERATORS
		public static readonly byte NUMBER = 67;

		// codes for higher categories
		public static readonly byte INUMBER = 1; // INTEGER NUMBER
		public static readonly byte RNUMBER = 2; // REAL NUMBER
		public static readonly byte WORD = 3;
		public static readonly byte SIGN = 4;
		public static readonly byte STRING = 5;

		public List<string> keywords = new List<string>();
		private List<string> signs = new List<string>();


		public void fillLists()
		{
			keywords.Add("PROGRAM");
			keywords.Add("BEGIN");
			keywords.Add("END");
			keywords.Add("END.");
			keywords.Add("VAR");
			keywords.Add("IF");
			keywords.Add("THEN");
			keywords.Add("ELSE");
			keywords.Add("WHILE");
			keywords.Add("DO");
			keywords.Add("NOT");
			keywords.Add("AND");
			keywords.Add("OR");
			keywords.Add("READLN");
			keywords.Add("WRITELN");
			keywords.Add("INTEGER");
			keywords.Add("REAL");
			keywords.Add("BOOLEAN");
			keywords.Add("TRUE");
			keywords.Add("FALSE");

			signs.Add("+");
			signs.Add("-");
			signs.Add("*");
			signs.Add("/");
			signs.Add("=");
			signs.Add("<>");
			signs.Add(">");
			signs.Add(">=");
			signs.Add("<");
			signs.Add("<=");
			signs.Add(":");
			signs.Add(";");
			signs.Add(",");
			signs.Add(":=");
			signs.Add("(");
			signs.Add(")");
			signs.Add("\"");
		}

		public byte getKeywordCode(String value)
		{
			var index = keywords.IndexOf(value);
			if (index == -1)
				return VARIABLE;

			return (byte)(10 + index);
		}

		public byte getSignCode(String value)
		{
			return (byte)(30 + signs.IndexOf(value));
		}
	}
}
