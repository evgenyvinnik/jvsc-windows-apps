using System;
using System.Collections.Generic;

namespace PascalInterpreter.Interpreter
{
	public class ExecLexem
	{
		public String type;     // instruction type
		public String variable;  // name of variable in association instruction
		public AdvLexem[] lexems; // lexems of the given instruction
		public List<ExecLexem> iftrue;
		public List<ExecLexem> ifelse;

		public ExecLexem(String type, String variable, List<AdvLexem> AL,
				  int start, int finish,
				  List<ExecLexem> iftrue, List<ExecLexem> ifelse)
		{
			this.type = type;
			this.variable = variable;
			lexems = new AdvLexem[finish - start + 1];
			for (int i = start, j = 0; i <= finish; i++, j++)
				lexems[j] = (AdvLexem)AL[i];
			this.iftrue = iftrue;
			this.ifelse = ifelse;
		}// ExecLexem()
	}
}
