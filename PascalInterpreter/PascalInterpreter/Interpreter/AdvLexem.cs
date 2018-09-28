using System;

namespace PascalInterpreter.Interpreter
{
	public class AdvLexem
	{
		public String value;
		public byte code;
		public byte lcategory; // lower category
		public byte hcategory; // higher category
		public byte ccategory; // custom category, if needed
		public int line;
		public int column;

		public AdvLexem(String value, byte code, byte lcategory,
						byte hcategory, int line, int column)
		{
			this.value = value;
			this.code = code;
			this.lcategory = lcategory;
			this.hcategory = hcategory;
			this.line = line;
			this.column = column;
		}
	}
}
