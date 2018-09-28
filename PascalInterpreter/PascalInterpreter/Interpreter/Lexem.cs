using System;
using System.Text;

namespace PascalInterpreter.Interpreter
{
	public class Lexem
	{
		public String value;
		public byte type;
		public int line;
		public int column;

		public Lexem(String value, byte type, int line, int column)
		{
			if (type != Codes.STRING)
				this.value = value.ToUpper();
			else
				this.value = value;
			this.type = type;
			this.line = line;
			this.column = column;
		}

		public Lexem(StringBuilder value, byte type, int line, int column)
		{
			if (type != Codes.STRING)
				this.value = value.ToString().ToUpper();
			else
				this.value = value.ToString();
			this.type = type;
			this.line = line;
			this.column = column;
		}

		public Lexem(int value, int type, int line, int column)
		{
			char[] sign = new char[1];
			sign[0] = (char)value;
			this.value = new String(sign);
			this.type = (byte)type;
			this.line = line;
			this.column = column;
		}
	}
}
