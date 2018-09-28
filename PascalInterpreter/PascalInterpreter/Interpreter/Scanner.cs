using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.GamerServices;

namespace PascalInterpreter.Interpreter
{
	public class Scanner
	{
		public List<Lexem> lexems;

		// variables for validation process
		List<string> vars = new List<string>();
		List<Int32> var_types = new List<Int32>();
		List<Object> var_vals = new List<Object>();
		// position in Advenced Lexems Array
		private int pos = 0;
		// position during instruction evaluating
		private int place = 0;
		private int main_begin = 0;

		public String stderr;
		public String stdout;


		public Scanner(String data)
		{
			try
			{
				stderr = "";
				stdout = "";
 
				StringReader filein = new StringReader(data);

				// array of 'lexem' objects
				lexems = new List<Lexem>();

				// current lexem
				StringBuilder value = new StringBuilder();

				// current type of lexem
				byte type = 0;

				// current line
				int line = 1;

				// current column
				int column = 1;

				// where the lexem began
				int began = 1;
		
				// where the String began
				int sline = 1;

				int bite = filein.Read();
				while (bite != -1) {
					// if bite is a quotation mark
					if (bite == 34) {
						if (type != 0) {
							lexems.Add(new Lexem(value, type, line, began));
							value = new StringBuilder();
						}
						if (type != 5) {
							type = 5;
							began = column;
							sline = line;
						}
						else
							type = 0;
					}
					else if (type == 5) {
						value.Append((char)bite);
						if (bite == 13) {
							line ++;
							column = -1;
						}
					}
					// if bite is a digit
					else if ((bite >= 48) && (bite <= 57)) {
						if (type == 0) {
							type = 1;
							began = column;
						}
						value.Append((char)bite);
					}
					else
						// if bite is a letter
						if (((bite >= 65) && (bite <= 90)) ||
							((bite >= 97) && (bite <= 122))) {
							if (type == 0) {
								type = 3;
								began = column;
								value.Append((char)bite);
							}
							else
								if (type == 3)
									value.Append((char)bite);
								else {
									type = 7;
									break;
								}
						}
						else
							// if bite is a sign
							if (((bite >= 40) && (bite <= 45)) ||
								 (bite == 47) ||
								((bite >= 58) && (bite <= 62))) {
								if (type != 0) {
									lexems.Add(new Lexem(value, type, line, began));
									type = 0;
									value = new StringBuilder();
								}
								lexems.Add(new Lexem(bite, 4, line, column));
							}
							else
								// if bite is a dot
								if (bite == 46)
									if (type != 0)
										if (type == 3)
											if (((value.ToString())
												  .ToUpper()).Equals("END")) {
												lexems.Add(new Lexem("END.",
														   (byte)3, line, began));
												type = 0;
												value = new StringBuilder();
											}
											else {
												type = 8;
												break;
											}
										else
											if (type == 1) {
												type = 2;
												value.Append('.');
											}
											else {
												type = 8;
												break;
											}
									else {
										type = 8;
										break;
									}
								else
									// if bite is a separator
									if ((bite ==  9) || (bite == 32) ||
										(bite == 13) || (bite == 10)) {
											if (type != 0) {
												lexems.Add(new Lexem(value,
														   type, line, began));
												type = 0;
												value = new StringBuilder();
											}
											if (bite == 13) {
												line ++;
												column = -1;
											}
										}
									// bite is not allowed character
									else {
										type = 9;
										break;
									}
					bite = filein.Read();
					if ((bite == -1) && (type == 5))
						type = 6;
					else if (bite == -1) 
						if (type != 0) {
							lexems.Add(new Lexem(value, type, line, began));
							type = 0;
							value = new StringBuilder();
						}
					column ++;
				}

				// error handling
				if (type == 6)
					err("ERROR: String not closed ! " + "line: " + sline + ", column: " + began);
				else if (type == 7)
					err("ERROR: letter joined with number ! " + "line: " + line + ", column: " + column);
				else if (type == 8)
					err("ERROR: dot in wrong place ! " + "line: " + line + ", column: " + column);
				else if (type == 9)
						err("ERROR: illegal character !: " + (char)bite + " line: " + line + ", column: " + column);
				else
				{
					// if everything is OK
					//printLexems(lexems);
					// advenced scunning (father, deeper clasification)

					List<AdvLexem> AdvL = advScanning(lexems);

					//	printAdvLexems(AdvL);
					// validation process
					String tmp_result = checkStructure(AdvL);
					Console.WriteLine(tmp_result);
					if (tmp_result.Equals("No errors !"))
					{
						Console.WriteLine("===================");
						// variables initialization
						initializeVars();
						pos = main_begin;
						// interpretation process
						List<ExecLexem> ExecL = interBody(AdvL);
						// execution
						execute(ExecL);
						

						/*
							Console.WriteLine("IAL Size = " + ExecL.Count);  
							for (int i = 0; i < ExecL.Count; i++) {
								Console.WriteLine(((ExecLexem)ExecL[i]).type);
								Console.WriteLine(((ExecLexem)ExecL[i]).variable);
								Console.WriteLine("If TRUE SIZE = " + ((ExecLexem)ExecL[i]).
																					iftrue.Count);
								for (int j = 0; j < ((ExecLexem)ExecL[i]).lexems.Length; j++)
									Console.WriteLine(((ExecLexem)ExecL[i]).lexems[j].value);
							}
						*/
	
					}
					else
					{
						err("ERROR!");
						err(tmp_result);
					}
				}
			}
			catch (Exception ex)
			{
				err(ex.ToString());
			}
		}// Scanner()

		public void printLexems(List<Lexem> lex)
		{
			Console.WriteLine("\nLEXEMS: " + lex.Count);
			Console.WriteLine("VALUE   TYPE   LINE   COLUMN");
			Object[] lexems = lex.ToArray();
			for (int i = 0; i < lexems.Length; i ++)
			{
				Lexem lexem = (Lexem)lexems[i];
				Console.WriteLine(lexem.value + "   " + lexem.type + "   "
								 + lexem.line + "   " + lexem.column);
			}
			Console.WriteLine(Environment.NewLine);
		}// printLexems(ArrayList)
	
		public void printAdvLexems(List<AdvLexem> AdvLex) {
			Console.WriteLine("\nLEXEMS: " + AdvLex.Count);
			Console.WriteLine("VALUE   CODE   LCATEGORY   "
							 + "HCATEGORY   LINE   COLUMN");
			Object[] AdvLexems = AdvLex.ToArray();
			for (int i = 0; i < AdvLexems.Length; i ++)
			{
				AdvLexem advlexem = (AdvLexem)AdvLexems[i];
				Console.WriteLine(advlexem.value + "   " + advlexem.code + "   "
								 + advlexem.lcategory + "   " 
								 + advlexem.hcategory + "   "
								 + advlexem.line + "   " + advlexem.column);
			}
			Console.WriteLine(Environment.NewLine);
		}// printLexems(ArrayList)

		public List<AdvLexem> advScanning(List<Lexem> lexems)
		{
			List<AdvLexem> AdvLexems = new List<AdvLexem>();
			String value = "";
			Codes codes = new Codes();
			codes.fillLists();
			Lexem lexem = null;
			byte code = 0;
			byte lcategory = 0;
			byte hcategory = 0;
		
			int size = lexems.Count;
			int i = 0;
			while (i < size) {
				lexem = (Lexem)lexems[i];
			
				// if lexem is a word
				if (lexem.type == Codes.STRING)
					AdvLexems.Add(new AdvLexem(lexem.value, Codes.STRING,
								  Codes.STRING, Codes.STRING,
								  lexem.line, lexem.column));
				else if (lexem.type == Codes.WORD) {
					hcategory = Codes.WORD;
					code = codes.getKeywordCode(lexem.value);

					if (code == Codes.VARIABLE)
						lcategory = Codes.VARIABLE;
					else if ((code >= Codes.AND) && (code <= Codes.OR))
						lcategory = Codes.LOPERATOR;
					else if ((code >= Codes.INTEGER) && (code <= Codes.BOOLEAN))
						lcategory = Codes.VTYPE;					
					else
						lcategory = Codes.KEYWORD;
				
					AdvLexems.Add(new AdvLexem(lexem.value, code,
								  lcategory, hcategory,
								  lexem.line, lexem.column));
				}
			
				// if lexem is a sign
				else if (lexem.type == Codes.SIGN)
				{
					value = lexem.value;;
					hcategory = Codes.SIGN;
					code = codes.getSignCode(lexem.value);

					if ((code >= Codes.COLON) && (code <= Codes.QUOTATION))
					{
						lcategory = Codes.OOPERATOR;
						if (lexem.value.Equals(":"))
							if ((i + 1) < size)
								if (((Lexem)lexems[i + 1]).value.Equals("=")) {
									code = Codes.ASOCIATE;
									value = ":=";
									i ++;
								}
					}
					else if ((code >= Codes.E) && (code <= Codes.LE))
					{
						lcategory = Codes.ROPERATOR;
						if (lexem.value.Equals("<"))
							if ((i + 1) < size) {
								if (((Lexem)lexems[i + 1]).value.Equals("=")) {
									code = Codes.LE;
									value = "<=";
									i ++;
								}
								if (((Lexem)lexems[i + 1]).value.Equals(">")) {
									code = Codes.NE;
									value = "<>";
									i ++;
								}
							}
						else if (lexem.value.Equals(">"))
							if ((i + 1) < size)
								if (((Lexem)lexems[i + 1]).value.Equals("=")) {
									code = Codes.GE;
									value = ">=";
									i ++;
								}
					}
					else if ((code >= Codes.PLUS) && (code <= Codes.DIV))
					{
						lcategory = Codes.AOPERATOR;
					}
					AdvLexems.Add(new AdvLexem(value, code,
								  lcategory, hcategory,
								  lexem.line, lexem.column));
				}
				else
					AdvLexems.Add(new AdvLexem(lexem.value, lexem.type,
								  Codes.NUMBER, Codes.WORD,
								  lexem.line, lexem.column));
				i ++;
			}
			return AdvLexems;
		}// advScanning()

		private String errorMess(String expected, AdvLexem found) {
			return ("expected: " + expected
				 + "\n   found: " + found.value
				 + "\n   line: " + found.line
				 + "\n   column: " + found.column);
		}// errorMess()

		private String errorMess(String expected) {
			return ("expected: " + expected
				 + "   nothing found");
		}// errorMess()
	
	
		// checks if there is a lexem of given Code
		private String checkC(String s, byte code, List<AdvLexem> AL)
		{
			if (pos < AL.Count) {
				AdvLexem al = (AdvLexem)AL[pos];
				if (al.code != code)
					return errorMess(s, al);
			}
			else
				return errorMess(s);
			return "";
		}// checkC()

		private String checkC(String s, byte code, List<AdvLexem> AL, int which)
		{
			if (which < AL.Count)
			{
				AdvLexem al = (AdvLexem)AL[which];
				if (al.code != code)
					return errorMess(s, al);
			}
			else
				return errorMess(s);
			return "";
		}// checkC()
	
		// checks if there is a lexem of given Lower category
		private String checkL(String s, byte code, List<AdvLexem> AL)
		{
			if (pos < AL.Count) {
				AdvLexem al = (AdvLexem)AL[pos];
				if (al.lcategory != code)
					return errorMess(s, al);
			}
			else
				return errorMess(s);
			return "";
		}// checkL()
	
	// mmmmmmmmmmmmmmmmmmm	VALIDATION	mmmmmmmmmmmmmmmmmmmmmmmmmmm

		public String checkStructure(List<AdvLexem> AL)
		{		
			Codes codes = new Codes();
			codes.fillLists();
			String result = "";
			AdvLexem al;
			int size = AL.Count;
			result = checkC("PROGRAM", Codes.PROGRAM, AL);
			if (!(result.Equals("")))
				return result;
			pos ++;
			result = checkC("VARIABLE", Codes.VARIABLE, AL);
			if (!(result.Equals("")))
				return result;
			pos ++;
			result = checkC("SEMICOLON", Codes.SEMICOLON, AL);
			if (!(result.Equals("")))
				return result;
			pos ++;
			result = checkC("VAR", Codes.VAR, AL);
			if (!(result.Equals(""))) {
				result = checkC("VAR or BEGIN", Codes.BEGIN, AL);
				if (!(result.Equals("")))
					return result;
			}
			if ((((AdvLexem)AL[pos]).value).Equals("VAR")) {
				pos ++;
				result = checkVAR(AL);
				if (!(result.Equals("")))
					return result;
			}

			// now we check if there is END. at the end of the program
			if (AL.Count - 1 > pos) {
				result = checkC("END.", Codes.END_DOT, AL, AL.Count - 1);
				if (!(result.Equals("")))
					return result;
			}
			else
				return "expected: END. at the end of program";

			// now we check if all VARIABLES are declared
			for (int i = pos + 1; i < AL.Count - 2; i ++) {
				al = (AdvLexem)(AL[i]);
				if (al.code == Codes.VARIABLE)
					if (!(vars.Contains(al.value)))
						return "cannot resolve symbol variable " + al.value
							 + "\n   line: " + al.line
							 + "\n   column: " + al.column;
			}
		
			pos ++;
			main_begin = pos;
			result = checkBody(AL);

	/*
	//testing...
	Console.WriteLine("Position = " + pos);
	al = (AdvLexem)AL[pos];
	Console.WriteLine("Lexem value = " + al.value);
	*/
		
			if (!(result.Equals("")))
				return result;

			return "No errors !";
		}// checkStructure

		private String checkVAR(List<AdvLexem> AL)
		{
			byte var_count = 0;
			String result = "";
			AdvLexem al = null;
			// if BEGIN appears changes to TRUE
			bool finish_b = false;
			// if there is no COMMA changes to TRUE
			bool finish_c = false;

			while (!finish_b) {
				result = checkC("BEGIN", Codes.BEGIN, AL);
				// if there is no BEGIN yet
				if (!result.Equals("")) {
					result = checkC("VARIABLE", Codes.VARIABLE, AL);
					// if there is a VARIABLE
					if (!result.Equals(""))
						return result;
					al = (AdvLexem)AL[pos];
					if ((((AdvLexem)AL[1]).value).Equals(al.value))
						return "VARIABLE is already used as a neme of the program"
							 + "\n   line: " + al.line
							 + "\n   column: " + al.column;
					if (vars.Contains(al.value))
						return ("VARIABLE already declared: " + al.value
							 + "\n   line: " + al.line
							 + "\n   column: " + al.column);
					vars.Add(al.value);
					var_count ++;
					finish_c = false;
					while (!finish_c) {
						pos ++;
						result = checkC("COMMA", Codes.COMMA, AL);
						if (result.Equals("")) {
							pos ++;
							result = checkC("VARIABLE", Codes.VARIABLE, AL);
							if (!result.Equals(""))
								return result;
							al = (AdvLexem)AL[pos];
							if ((((AdvLexem)AL[1]).value).Equals(al.value))
								return "VARIABLE is already used as a neme "
									 + "of the program"
									 + "\n   line: " + al.line
									 + "\n   column: " + al.column;
							if (vars.Contains(al.value))
								return ("VARIABLE already declared: " + al.value
									 + "\n   line: " + al.line
									 + "\n   column: " + al.column);
							vars.Add(al.value);
							var_count ++;
						}
						else
							finish_c = true;
					}// while
					result = checkC("COLON", Codes.COLON, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					result = checkL("VARIABLE TYPE", Codes.VTYPE, AL);
					if (!result.Equals(""))
						return result;

					byte code = ((AdvLexem)AL[pos]).code;
					for (byte i = 0; i < var_count; i ++)
						var_types.Add(code);
					var_count = 0;
				
					pos ++;
					result = checkC("SEMICOLON", Codes.SEMICOLON, AL);
					if (!result.Equals(""))
						return result;	
					pos ++;	
				}
				else 
					finish_b = true;
			}// while
			return "";
		}// checkVAR()

		private String checkIASSOC(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			byte b_count = 0; // brackets count
			bool finish_b = false; // finish if no more brackets

			while (!finish_b) {
				result = checkC("VALUE", Codes.LBRACKET, AL);
				if (!result.Equals(""))
					finish_b = true;
				else {
					b_count ++;
					pos ++;
				}
			}
			result = checkC("VALUE", Codes.MINUS, AL);
			if (result.Equals("")) {
				pos ++;
				result = checkC("INTEGER NUMBER", Codes.INUMBER, AL);
				if (result.Equals("")) {
					al = (AdvLexem)AL[pos];
					AL[pos - 1] = new AdvLexem("-" + al.value, al.code,
												  al.lcategory, al.hcategory,
												  al.line, al.column);
					AL.RemoveAt(pos);
				}
				else
					return result;
			}
			else {
				result = checkC("INTEGER VALUE", Codes.INUMBER, AL);
				if (!result.Equals("")) {
					result = checkC("INTEGER VARIABLE", Codes.VARIABLE, AL);
					if (!result.Equals(""))
						return result;			
					else {
						al = (AdvLexem)AL[pos];
						int tmp = vars.IndexOf(al.value);
						if (!(((Int32)var_types[tmp]).Equals(
											Codes.INTEGER))) {
							al = (AdvLexem)AL[pos];
							return "INTEGER VARIABLE expected "
								 + "\n   found: " + al.value
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
						}
						else
							pos++;
					}
				}
				else
					pos ++;
			}
			finish_b = false;
			while (!finish_b) {
				result = checkC("OPERATOR", Codes.RBRACKET, AL);
				if (!result.Equals(""))
					finish_b = true;
				else {
					if (b_count > 0) {
						b_count --;
						pos ++;
					}
					else {
						al = (AdvLexem)AL[pos];
						return "RIGHT BRACKET not opened"
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
					}
				}
			}
			bool finish_o = false; // if there is no operator
			while (!finish_o){
				result = checkL("ARITHMETIC OPERATOR", Codes.AOPERATOR, AL);
				if (!result.Equals(""))
					finish_o = true;
				else {
					pos ++;
					finish_b = false;
					while (!finish_b) {
						result = checkC("VALUE", Codes.LBRACKET, AL);
						if (!result.Equals(""))
							finish_b = true;
						else {
							b_count ++;
							pos ++;
						}
					}
					result = checkC("VALUE", Codes.MINUS, AL);
					if (result.Equals("")) {
						pos ++;
						result = checkC("INTEGER NUMBER", Codes.INUMBER, AL);
						if (result.Equals("")) {
							al = (AdvLexem)AL[pos];
							AL[pos - 1] = new AdvLexem("-" + al.value, al.code,
														  al.lcategory,
														  al.hcategory,
														  al.line, al.column);
							AL.RemoveAt(pos);
						}
						else
							return result;
					}
					else {
						result = checkC("INTEGER VALUE", Codes.INUMBER, AL);
						if (!result.Equals("")) {
							result = checkC("INTEGER VARIABLE", Codes.VARIABLE, AL);
							if (!result.Equals(""))
								return result;			
							else {
								al = (AdvLexem)AL[pos];
								int tmp = vars.IndexOf(al.value);
								if (!(((Int32)var_types[tmp]).Equals(
													Codes.INTEGER))) {
									al = (AdvLexem)AL[pos];
									return "INTEGER VARIABLE expected "
										 + "\n   found: " + al.value
										 + "\n   line: " + al.line
										 + "\n   column: " + al.column;
								}
								else
									pos++;
							}
						}
						else
							pos ++;
					}
					finish_b = false;
					while (!finish_b) {
						result = checkC("OPERATOR", Codes.RBRACKET, AL);
						if (!result.Equals(""))
							finish_b = true;
						else {
							if (b_count > 0) {
								b_count --;
								pos ++;
							}
							else {
								al = (AdvLexem)AL[pos];
								return "RIGHT BRACKET not opened"
										 + "\n   line: " + al.line
										 + "\n   column: " + al.column;
							}
						}
					}				
				}
			}
		
			return "";
		}// checkIASSOC()
	
		private String checkRASSOC(List<AdvLexem> AL) {
			String result = "";
			AdvLexem al = null;
			byte b_count = 0; // brackets count
			bool finish_b = false; // finish if no more brackets

			while (!finish_b) {
				result = checkC("VALUE", Codes.LBRACKET, AL);
				if (!result.Equals(""))
					finish_b = true;
				else {
					b_count ++;
					pos ++;
				}
			}
			result = checkC("VALUE", Codes.MINUS, AL);
			if (result.Equals("")) {
				pos ++;
				result = checkC("INTEGER OR REAL NUMBER", Codes.INUMBER, AL);
				if ((result.Equals("")) ||
					(checkC("", Codes.RNUMBER, AL).Equals(""))) {
					al = (AdvLexem)AL[pos];
					AL[pos - 1] = new AdvLexem("-" + al.value, al.code,
												  al.lcategory, al.hcategory,
												  al.line, al.column);
					AL.RemoveAt(pos);
				}
				else
					return result;
			}
			else {
				result = checkC("", Codes.INUMBER, AL);
				if (!((result.Equals("")) ||
					(checkC("", Codes.RNUMBER, AL).Equals("")))) {
					result = checkC("INTEGER OR REAL VARIABLE",
									 Codes.VARIABLE, AL);
					if (!result.Equals(""))
						return result;
					else {
						al = (AdvLexem)AL[pos];
						int tmp = vars.IndexOf(al.value);
						int type = ((Int32)var_types[tmp]);
						if (!((type == Codes.INTEGER) || (type == Codes.REAL))) {
							al = (AdvLexem)AL[pos];
							return "INTEGER OR REAL VARIABLE expected "
								 + "\n   found: " + al.value
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
						}
						else
							pos++;
					}
				}
				else
					pos ++;
			}
			finish_b = false;
			while (!finish_b) {
				result = checkC("OPERATOR", Codes.RBRACKET, AL);
				if (!result.Equals(""))
					finish_b = true;
				else {
					if (b_count > 0) {
						b_count --;
						pos ++;
					}
					else {
						al = (AdvLexem)AL[pos];
						return "RIGHT BRACKET not opened"
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
					}
				}
			}
			bool finish_o = false; // if there is no operator
			while (!finish_o){
				result = checkL("closing BRACKET", Codes.AOPERATOR, AL);
				if (!result.Equals("")) {
					finish_o = true;
					if (b_count > 0)
					return result;
				}
				else {
					pos ++;
					finish_b = false;
					while (!finish_b) {
						result = checkC("VALUE", Codes.LBRACKET, AL);
						if (!result.Equals(""))
							finish_b = true;
						else {
							b_count ++;
							pos ++;
						}
					}
					result = checkC("VALUE", Codes.MINUS, AL);
					if (result.Equals("")) {
						pos ++;
						result = checkC("INTEGER OR REAL NUMBER",
										 Codes.INUMBER, AL);
						if ((result.Equals("")) ||
							(checkC("", Codes.RNUMBER, AL).Equals(""))) {
							al = (AdvLexem)AL[pos];
							AL[pos - 1] = new AdvLexem("-" + al.value, al.code,
														  al.lcategory,
														  al.hcategory,
														  al.line, al.column);
							AL.RemoveAt(pos);
						}
						else
							return result;
					}
					else {
						result = checkC("", Codes.INUMBER, AL);
						if (!((result.Equals("")) ||
							(checkC("", Codes.RNUMBER, AL).Equals("")))) {
							result = checkC("INTEGER OR REAL VARIABLE",
											 Codes.VARIABLE, AL);
							if (!result.Equals(""))
								return result;			
							else {
								al = (AdvLexem)AL[pos];
								int tmp = vars.IndexOf(al.value);
								int type = ((Int32)var_types[tmp]);
								if (!((type == Codes.INTEGER) ||
									 (type == Codes.REAL))) {
									al = (AdvLexem)AL[pos];
									return "INTEGER OR REAL VARIABLE expected "
										 + "\n   found: " + al.value
										 + "\n   line: " + al.line
										 + "\n   column: " + al.column;
								}
								else
									pos++;
							}
						}
						else
							pos ++;
					}
					finish_b = false;
					while (!finish_b) {
						result = checkC("OPERATOR", Codes.RBRACKET, AL);
						if (!result.Equals(""))
							finish_b = true;
						else {
							if (b_count > 0) {
								b_count --;
								pos ++;
							}
							else {
								al = (AdvLexem)AL[pos];
								return "RIGHT BRACKET not opened"
										 + "\n   line: " + al.line
										 + "\n   column: " + al.column;
							}
						}
					}				
				}
			}		
			return "";
		}// checkRASSOC()

		private String checkLEXPR(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			byte b_count = 0; // brackets count
			bool finish_o = false;
			bool finish_n = false;
			bool finish_b = false;		

			while (!finish_n) {
				finish_b = false;
				while (!finish_b) {
					result = checkC("VALUE", Codes.LBRACKET, AL);
					if (!result.Equals(""))
						finish_b = true;
					else {
						b_count ++;
						pos ++;
					}
				}
				result = checkC("NOT", Codes.NOT, AL);
				if (!result.Equals(""))
					finish_n = true;
				else
					pos ++;
			}
			result = checkLEXPRNElem(AL);
			if (result.Equals("")) {
				result = checkL("RELATION OPERATOR",
								 Codes.ROPERATOR, AL);
				if (!result.Equals(""))
					return result;
				else {
					pos ++;
					result = checkLEXPRNElem(AL);
					if (!result.Equals(""))
						return result;
				}
			}
			else {
				result = checkLEXPRLElem(AL);
				if (!result.Equals(""))
					return result;
			}
			finish_b = false;
			while (!finish_b) {
				result = checkC("OPERATOR", Codes.RBRACKET, AL);
				if (!result.Equals(""))
					finish_b = true;
				else {
					if (b_count > 0) {
						b_count --;
						pos ++;
					}
					else {
						al = (AdvLexem)AL[pos];
						return "RIGHT BRACKET not opened"
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
					}
				}
			}
			finish_o = false;
			while (!finish_o){
				result = checkL("closing BRACKET", Codes.LOPERATOR, AL);
				if (!result.Equals("")) {
					finish_o = true;
					if (b_count > 0)
					return result;
				}
				else {
					pos ++;
					finish_n = false;
					while (!finish_n) {
						finish_b = false;
						while (!finish_b) {
							result = checkC("VALUE", Codes.LBRACKET, AL);
							if (!result.Equals(""))
								finish_b = true;
							else {
								b_count ++;
								pos ++;
							}
						}
						result = checkC("NOT", Codes.NOT, AL);
						if (!result.Equals(""))
							finish_n = true;
						else
							pos ++;
					}
					result = checkLEXPRNElem(AL);
					if (result.Equals("")) {
						result = checkL("RELATION OPERATOR",
										 Codes.ROPERATOR, AL);
						if (!result.Equals(""))
							return result;
						else {
							pos ++;
							result = checkLEXPRNElem(AL);
							if (!result.Equals(""))
								return result;
						}
					}
					else {
						result = checkLEXPRLElem(AL);
						if (!result.Equals(""))
							return result;
					}
					finish_b = false;
					while (!finish_b) {
						result = checkC("OPERATOR", Codes.RBRACKET, AL);
						if (!result.Equals(""))
							finish_b = true;
						else {
							if (b_count > 0) {
								b_count --;
								pos ++;
							}
							else {
								al = (AdvLexem)AL[pos];
								return "RIGHT BRACKET not opened"
										 + "\n   line: " + al.line
										 + "\n   column: " + al.column;
							}
						}
					}
				}
			}
			return "";
		}// checkLEXPR()

		private String checkWRITE(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			result = checkC("LEFT BRACKET", Codes.LBRACKET, AL);
			if (!result.Equals(""))
				return result;
			pos ++;
			result = checkC("STRING or VARIABLE", Codes.STRING, AL);
			if (!(result.Equals("") ||
				(checkC("", Codes.VARIABLE, AL)).Equals("")))
				return result;
			pos ++;
			bool finish_c = false;
			while (!finish_c) {
				result = checkC("COMMA", Codes.COMMA, AL);
				if (!result.Equals(""))
					finish_c = true;
				else {
					pos ++;
					result = checkC("STRING or VARIABLE", Codes.STRING, AL);
					if (!(result.Equals("") ||
						(checkC("", Codes.VARIABLE, AL)).Equals("")))
						return result;
					else
						pos ++;			
				}
			}
			result = checkC("RIGHT BRACKET", Codes.RBRACKET, AL);
			if (!result.Equals(""))
				return result;
			pos ++;
			return "";
		}// checkWRITE()

		private String checkREAD(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			result = checkC("LEFT BRACKET", Codes.LBRACKET, AL);
			if (!result.Equals(""))
				return result;
			pos ++;
			result = checkC("VARIABLE", Codes.VARIABLE, AL);		
			if (!result.Equals(""))
				return result;
			pos ++;
			result = checkC("RIGHT BRACKET", Codes.RBRACKET, AL);
			if (!result.Equals(""))
				return result;
			pos ++;		
			return "";
		}// checkREAD()


	//mmmmmmmmmmmmmmmmmm	MAIN VALIDATOR   mmmmmmmmmmmmmmmmmmmmmmmmmmmmmm

		private String checkBody(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			int begins = 0;

			while (true) {
				al = (AdvLexem)AL[pos];
				if (al.code == Codes.VARIABLE) {
					pos ++;
					result = checkC("ASSOCIATE MARK", Codes.ASOCIATE, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					int tmp = vars.IndexOf(al.value);
					int type = ((Int32)var_types[tmp]);
					if (type == Codes.INTEGER) {
						result = checkIASSOC(AL);
						if (!result.Equals(""))
							return result;
					}
					else if (type == Codes.REAL) {
						result = checkRASSOC(AL);
						if (!result.Equals(""))
							return result;
					}
					else if (type == Codes.BOOLEAN) {
						result = checkLEXPR(AL);
						if (!result.Equals(""))
							return result;
					}
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.WRITELN) {
					pos ++;
					result = checkWRITE(AL);
					if (!result.Equals(""))
						return result;
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.READLN) {
					pos ++;
					result = checkREAD(AL);
					if (!result.Equals(""))
						return result;
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.WHILE) {
					pos ++;
					result = checkLEXPR(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword DO", Codes.DO, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					result = checkOne(AL);
					if (!result.Equals(""))
						return result;				
				}
				else if (al.code == Codes.IF) {
					pos ++;
					result = checkLEXPR(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword THEN", Codes.THEN, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					result = checkOne(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword ELSE", Codes.ELSE, AL);
					if (result.Equals("")) {
						pos ++;
						result = checkOne(AL);
						if (!result.Equals(""))
							return result;
					}
				}
				else if (al.code == Codes.BEGIN) {
					AL.RemoveAt(pos);
					result = checkBlock(AL);
					if (!result.Equals(""))
						return result;
					else {
						pos --;
						result = checkC("SEMICOLON", Codes.SEMICOLON, AL);
						if (result.Equals("")) {
							AL.RemoveAt(pos);
							pos --;
						}
						AL.RemoveAt(pos);

					}
				}
				else if (al.code == Codes.END) {
					return "END without coresponding BEGIN"
						 + "\n   found: " + al.value
						 + "\n   line: " + al.line
						 + "\n   column: " + al.column;	
				}
				else if (al.code == Codes.END_DOT) {
					return "";
				}
				else
					return "Not a statement"
						 + "\n   found: " + al.value
						 + "\n   line: " + al.line
						 + "\n   column: " + al.column;
			}
		}// checkBODY()	

		private String checkBlock(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			int begins = 0;

			while (true) {
				al = (AdvLexem)AL[pos];
				if (al.code == Codes.VARIABLE) {
					pos ++;
					result = checkC("ASSOCIATE MARK", Codes.ASOCIATE, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					int tmp = vars.IndexOf(al.value);
					int type = ((Int32)var_types[tmp]);
					if (type == Codes.INTEGER) {
						result = checkIASSOC(AL);
						if (!result.Equals(""))
							return result;
					}
					else if (type == Codes.REAL) {
						result = checkRASSOC(AL);
						if (!result.Equals(""))
							return result;
					}
					else if (type == Codes.BOOLEAN) {
						result = checkLEXPR(AL);
						if (!result.Equals(""))
							return result;
					}
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.WRITELN) {
					pos ++;
					result = checkWRITE(AL);
					if (!result.Equals(""))
						return result;
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.READLN) {
					pos ++;
					result = checkREAD(AL);
					if (!result.Equals(""))
						return result;
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.WHILE) {
					pos ++;
					result = checkLEXPR(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword DO", Codes.DO, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					result = checkOne(AL);
					if (!result.Equals(""))
						return result;				
				}
				else if (al.code == Codes.IF) {
					pos ++;
					result = checkLEXPR(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword THEN", Codes.THEN, AL);
					if (!result.Equals(""))
						return result;
					pos ++;
					result = checkOne(AL);
					if (!result.Equals(""))
						return result;
					result = checkC("keyword ELSE", Codes.ELSE, AL);
					if (result.Equals("")) {
						pos ++;
						result = checkOne(AL);
						if (!result.Equals(""))
							return result;
					}
				}
				else if (al.code == Codes.BEGIN) {
					pos ++;
					result = checkBlock(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (al.code == Codes.END) {
					pos ++;
					result = checkEnding(AL);
					if (!result.Equals(""))
						return result;
					else
						return "";
				}
				else if (al.code == Codes.END_DOT) {
					return "END expected"
						 + "\n   line: " + al.line
						 + "\n   column: " + al.column;
				}
				else
					return "Not a statement"
						 + "\n   found: " + al.value
						 + "\n   line: " + al.line
						 + "\n   column: " + al.column;
			}
		}// checkBlock()

		private String checkOne(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			int begins = 0;
			al = (AdvLexem)AL[pos];
			if (al.code == Codes.VARIABLE) {

				pos ++;
				result = checkC("ASSOCIATE MARK", Codes.ASOCIATE, AL);
				if (!result.Equals(""))
					return result;
				pos ++;
				int tmp = vars.IndexOf(al.value);
				int type = ((Int32)var_types[tmp]);

				if (type == Codes.INTEGER) {
					result = checkIASSOC(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (type == Codes.REAL) {
					result = checkRASSOC(AL);
					if (!result.Equals(""))
						return result;
				}
				else if (type == Codes.BOOLEAN) {
					result = checkLEXPR(AL);
					if (!result.Equals(""))
						return result;
				}
				return checkEnding(AL);
			}
			else if (al.code == Codes.WRITELN) {
				pos ++;
				result = checkWRITE(AL);
				if (!result.Equals(""))
					return result;
				return checkEnding(AL);
			}
			else if (al.code == Codes.READLN) {
				pos ++;
				result = checkREAD(AL);
				if (!result.Equals(""))
					return result;
				return checkEnding(AL);
			}
			else if (al.code == Codes.WHILE) {
				pos ++;
				result = checkLEXPR(AL);
				if (!result.Equals(""))
					return result;
				result = checkC("keyword DO", Codes.DO, AL);
				if (!result.Equals(""))
					return result;
				pos ++;
				return checkOne(AL);
			}
			else if (al.code == Codes.IF) { 
				pos ++;
				result = checkLEXPR(AL);
				if (!result.Equals(""))
					return result;
				result = checkC("keyword THEN", Codes.THEN, AL);
				if (!result.Equals(""))
					return result;
				pos ++;

				result = checkOne(AL);
				if (!result.Equals(""))
					return result;
				result = checkC("keyword ELSE", Codes.ELSE, AL);
				if (result.Equals("")) {
					pos ++;
					return checkOne(AL);
				}
				else
					return "";
			}
			else if (al.code == Codes.BEGIN) {
				pos ++;

				return checkBlock(AL);
			}
			else if (al.code == Codes.END) {
					return "END without coresponding BEGIN"
						 + "\n   found: " + al.value
						 + "\n   line: " + al.line
						 + "\n   column: " + al.column;					
			}
			else if ((al.code == Codes.END_DOT) || 
					 (al.code == Codes.ELSE)) {
				return "INSTRUCTION expected"
					 + "\n   line: " + al.line
					 + "\n   column: " + al.column;
			}
			else
				return "Not a statement"
					 + "\n   found: " + al.value
					 + "\n   line: " + al.line
					 + "\n   column: " + al.column;
		}// checkOne()

		private String checkEnding(List<AdvLexem> AL)
		{
			String result = "";
		
			result = checkC("SEMICOLON", Codes.SEMICOLON, AL);
			if (!result.Equals("")) {
				result = checkC("SEMICOLON", Codes.END, AL);
				if (!result.Equals("")) {
					result = checkC("SEMICOLON", Codes.ELSE, AL);
					if (!result.Equals("")) {
						result = checkC("SEMICOLON", Codes.END_DOT, AL);
						if (!result.Equals(""))
							return result;
					}
				}
			}
			else
				pos ++;
			return "";
		
		}// checkEnding()

		// checks Logical EXPRexion Number Element
		private String checkLEXPRNElem(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
		
			result = checkC("VALUE", Codes.MINUS, AL);
			if (result.Equals("")) {
				pos ++;
				result = checkC("INTEGER OR REAL NUMBER",
								 Codes.INUMBER, AL);
				if ((result.Equals("")) ||
					(checkC("", Codes.RNUMBER, AL).Equals(""))) {
					al = (AdvLexem)AL[pos];
					AL[pos - 1] = new AdvLexem("-" + al.value, al.code,
												  al.lcategory,
												  al.hcategory,
												  al.line, al.column);
					AL.RemoveAt(pos);
					return "";
				}
				else
					return result;
			}
			else {
				result = checkC("", Codes.INUMBER, AL);
				if (!((result.Equals("")) ||
					(checkC("", Codes.RNUMBER, AL).Equals("")))) {
					result = checkC("INTEGER OR REAL VARIABLE",
									 Codes.VARIABLE, AL);
					if (!result.Equals(""))
						return result;
					else {
						al = (AdvLexem)AL[pos];
						int tmp = vars.IndexOf(al.value);
						int type = ((Int32)var_types[tmp]);
						if (!((type == Codes.INTEGER) || (type == Codes.REAL))) {
							al = (AdvLexem)AL[pos];
							return "INTEGER OR REAL VARIABLE expected "
								 + "\n   found: " + al.value
								 + "\n   line: " + al.line
								 + "\n   column: " + al.column;
						}
						else {
							pos++;
							return "";
						}
					}
				}
				else {
					pos ++;
					return "";
				}
			}
		}// checkLEXPRNElem()
	
		// checks Logical EXPRexion Logic Element
		private String checkLEXPRLElem(List<AdvLexem> AL)
		{
			String result = "";
			AdvLexem al = null;
			result = checkC("", Codes.TRUE, AL);
			if (!((result.Equals("")) ||
				(checkC("", Codes.FALSE, AL).Equals("")))) {
				result = checkC("LOGICAL VALUE",
								 Codes.VARIABLE, AL);
				if (!result.Equals(""))
					return result;
				else {
					al = (AdvLexem)AL[pos];
					int tmp = vars.IndexOf(al.value);
					int type = ((Int32)var_types[tmp]);
					if (!(type == Codes.BOOLEAN)) {
						al = (AdvLexem)AL[pos];
						return "expected: LOGICAL VALUE"
							 + "\n   found: " + al.value
							 + "\n   line: " + al.line
							 + "\n   column: " + al.column;
					}
					else {
						pos++;
						return "";
					}
				}
			}
			else {
				pos ++;
				return "";
			}
		}// checkLEXPRLElem()

	//mmmmmmmmmmmmmmmmmmmmmm	INTERPRETER   mmmmmmmmmmmmmmmmmmmmmmmmmmm

		private List<ExecLexem> interBody(List<AdvLexem> AL)
		{
			List<ExecLexem> baze = new List<ExecLexem>();
			String itype = "";
			String variable = "";
			int start = 0;
			int finish = 0;
			List<ExecLexem> iftrue = new List<ExecLexem>();
			List<ExecLexem> ifelse = new List<ExecLexem>();

			AdvLexem al = null;

			while (true) {
				al = (AdvLexem)AL[pos];
				if (al.code == Codes.VARIABLE) {
					variable = al.value;
					pos ++;
					pos ++;
					start = pos;
					int tmp = vars.IndexOf(al.value);
					int type = ((Int32)var_types[tmp]);
	
					if (type == Codes.INTEGER) {
						checkIASSOC(AL);
						finish = pos - 1;
						itype = "IASSOC";
					}
					else if (type == Codes.REAL) {
						checkRASSOC(AL);
						finish = pos - 1;
						itype = "RASSOC";
					}
					else if (type == Codes.BOOLEAN) {
						checkLEXPR(AL);
						finish = pos - 1;
						itype = "BASSOC";
					}
					checkEnding(AL);
				}
				else if (al.code == Codes.WRITELN) {
					itype = "WRITE";
					pos ++;
					start = pos;
					checkWRITE(AL);
					finish = pos - 1;
					checkEnding(AL);
				}
				else if (al.code == Codes.READLN) {
					itype = "READ";
					pos ++;
					start = pos;
					checkREAD(AL);
					finish = pos - 1;
					checkEnding(AL);
				}
				else if (al.code == Codes.WHILE) {
					itype = "WHILE";
					pos ++;
					start = pos;
					checkLEXPR(AL);
					finish = pos - 1;
					pos ++;
					iftrue = interOne(AL);
				}
				else if (al.code == Codes.IF) {
					itype = "IF";
					pos ++;
					start = pos;
					checkLEXPR(AL);
					finish = pos - 1;
					pos ++;
					iftrue = interOne(AL);
					if (checkC("keyword ELSE", Codes.ELSE, AL).Equals("")) {
						pos ++;
						ifelse = interOne(AL);
					}
				}
				else if (al.code == Codes.BEGIN) {
					pos ++;
					return interBlock(AL);

				}
				else if (al.code == Codes.END_DOT) {
					pos ++;
					checkEnding(AL);
					return baze;
				}
				baze.Add(new ExecLexem(itype, variable, AL, start, finish,
									   iftrue, ifelse));
				iftrue = new List<ExecLexem>();
				ifelse = new List<ExecLexem>();
				variable = "";
			}
		}// interBODY()	

		private List<ExecLexem> interBlock(List<AdvLexem> AL)
		{
			List<ExecLexem> baze = new List<ExecLexem>();
			String itype = "";
			String variable = "";
			int start = 0;
			int finish = 0;
			List<ExecLexem> iftrue = new List<ExecLexem>();
			List<ExecLexem> ifelse = new List<ExecLexem>();

			AdvLexem al = null;

			while (true) {
				al = (AdvLexem)AL[pos];
				if (al.code == Codes.VARIABLE) {
					variable = al.value;
					pos ++;
					pos ++;
					start = pos;
					int tmp = vars.IndexOf(al.value);
					int type = ((Int32)var_types[tmp]);
	
					if (type == Codes.INTEGER) {
						checkIASSOC(AL);
						finish = pos - 1;
						itype = "IASSOC";
					}
					else if (type == Codes.REAL) {
						checkRASSOC(AL);
						finish = pos - 1;
						itype = "RASSOC";
					}
					else if (type == Codes.BOOLEAN) {
						checkLEXPR(AL);
						finish = pos - 1;
						itype = "BASSOC";
					}
					checkEnding(AL);
				}
				else if (al.code == Codes.WRITELN) {
					itype = "WRITE";
					pos ++;
					start = pos;
					checkWRITE(AL);
					finish = pos - 1;
					checkEnding(AL);
				}
				else if (al.code == Codes.READLN) {
					itype = "READ";
					pos ++;
					start = pos;
					checkREAD(AL);
					finish = pos - 1;
					checkEnding(AL);
				}
				else if (al.code == Codes.WHILE) {
					itype = "WHILE";
					pos ++;
					start = pos;
					checkLEXPR(AL);
					finish = pos - 1;
					pos ++;
					iftrue = interOne(AL);
				}
				else if (al.code == Codes.IF) {
					itype = "IF";
					pos ++;
					start = pos;
					checkLEXPR(AL);
					finish = pos - 1;
					pos ++;
					iftrue = interOne(AL);
					if (checkC("keyword ELSE", Codes.ELSE, AL).Equals("")) {
						pos ++;
						ifelse = interOne(AL);
					}
				}
				else if (al.code == Codes.BEGIN) {
					pos ++;
					return interBlock(AL);
				}
				else if (al.code == Codes.END) {
					pos ++;
					checkEnding(AL);
					return baze;
				}
				baze.Add(new ExecLexem(itype, variable, AL, start, finish,
									   iftrue, ifelse));
				iftrue = new List<ExecLexem>();
				ifelse = new List<ExecLexem>();
				variable = "";

			}
		}// interBlock()

		private List<ExecLexem> interOne(List<AdvLexem> AL)
		{
			List<ExecLexem> baze = new List<ExecLexem>();
			String itype = "";
			String variable = "";
			int start = 0;
			int finish = 0;
			List<ExecLexem> iftrue = new List<ExecLexem>();
			List<ExecLexem> ifelse = new List<ExecLexem>();

			AdvLexem al = null;
			al = (AdvLexem)AL[pos];
			if (al.code == Codes.VARIABLE) {
				variable = al.value;
				pos ++;
				pos ++;
				start = pos;
				int tmp = vars.IndexOf(al.value);
				int type = ((Int32)var_types[tmp]);

				if (type == Codes.INTEGER) {
					checkIASSOC(AL);
					finish = pos - 1;
					itype = "IASSOC";
				}
				else if (type == Codes.REAL) {
					checkRASSOC(AL);
					finish = pos - 1;
					itype = "RASSOC";
				}
				else if (type == Codes.BOOLEAN) {
					checkLEXPR(AL);
					finish = pos - 1;
					itype = "BASSOC";
				}
				checkEnding(AL);
			}
			else if (al.code == Codes.WRITELN) {
				itype = "WRITE";
				pos ++;
				start = pos;
				checkWRITE(AL);
				finish = pos - 1;
				checkEnding(AL);
			}
			else if (al.code == Codes.READLN) {
				itype = "READ";
				pos ++;
				start = pos;
				checkREAD(AL);
				finish = pos - 1;
				checkEnding(AL);
			}
			else if (al.code == Codes.WHILE) {
				itype = "WHILE";
				pos ++;
				start = pos;
				checkLEXPR(AL);
				finish = pos - 1;
				pos ++;
				iftrue = interOne(AL);
			}
			else if (al.code == Codes.IF) {
				itype = "IF";
				pos ++;
				start = pos;
				checkLEXPR(AL);
				finish = pos - 1;
				pos ++;
				iftrue = interOne(AL);
				if (checkC("keyword ELSE", Codes.ELSE, AL).Equals("")) {
					pos ++;
					ifelse = interOne(AL);
				}
			}
			else if (al.code == Codes.BEGIN) {
				pos ++;
				return interBlock(AL);
			}
			baze.Add(new ExecLexem(itype, variable, AL, start, finish,
								   iftrue, ifelse));
			return baze;

		}// checkOne()

	//mmmmmmmmmmmmmmmmmmmmmmm	EVALUATION	mmmmmmmmmmmmmmmmmmmmmmmmmmmmmm

		private void initializeVars()
		{
			for (int i = 0; i < var_types.Count; i++)
				if (((Int32)var_types[i]) == Codes.INTEGER)
					var_vals.Add(0);
				else if (((Int32)var_types[i]) == Codes.REAL)
					var_vals.Add(0);
				else
					var_vals.Add(false);
		}// initializeVars()

		private int evalInt(AdvLexem[] lex)
		{
			List<Int32> args = new List<Int32>();
			List<string> oper = new List<string>();

			while (place < lex.Length) {
				if (lex[place].code == Codes.RBRACKET) {
					break;
				}
				if (lex[place].code == Codes.INUMBER)
					args.Add(Int32.Parse(lex[place].value));
				else if (lex[place].code == Codes.VARIABLE) {
					int tmp = vars.IndexOf(lex[place].value);
					args.Add((Int32)var_vals[tmp]);
				}
				else if (lex[place].code == Codes.LBRACKET) {
					place ++;
					args.Add(evalInt(lex));	
				}
				else if (lex[place].lcategory == Codes.AOPERATOR)
					oper.Add(lex[place].value);
				place ++;
			}

			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("*")) {
					int tmp1 = ((Int32)args[i]);
					int tmp2 = ((Int32)args[i + 1]);
					args[i] = tmp1 * tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}
				else if (((String)oper[i]).Equals("/")) {
					int tmp1 = ((Int32)args[i]);
					int tmp2 = ((Int32)args[i + 1]);
					args[i] = tmp1 / tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}

			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("+")) {
					int tmp1 = ((Int32)args[i]);
					int tmp2 = ((Int32)args[i+1]);
					args[i] = tmp1 + tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}
				else if (((String)oper[i]).Equals("-")) {
					int tmp1 = ((Int32)args[i]);
					int tmp2 = ((Int32)args[i+1]);
					args[i] = tmp1 - tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}

			return ((Int32)args[0]);
		}// evalInt()
	
		private double evalReal(AdvLexem[] lex) {
			List<Double> args = new List<Double>();

			List<string> oper = new List<string>();

			while (place < lex.Length) {
				if (lex[place].code == Codes.RBRACKET) {
					break;
				}
				if (lex[place].lcategory == Codes.NUMBER)
					args.Add(Double.Parse(lex[place].value));
				else if (lex[place].code == Codes.VARIABLE) {
					int tmp = vars.IndexOf(lex[place].value);
					int type = ((Int32)var_types[tmp]);
					if (type == Codes.INTEGER) 
						args.Add( (Int32)var_vals[tmp]);
					else
						args.Add((Double)var_vals[tmp]);
				}
				else if (lex[place].code == Codes.LBRACKET) {
					place ++;
					args.Add( evalReal(lex));	
				}
				else if (lex[place].lcategory == Codes.AOPERATOR)
					oper.Add(lex[place].value);
				place ++;
			}

			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("*"))
				{
					double tmp1 = ((Double)args[i]);
					double tmp2 = ((Double)args[i+1]);
					args[i] = tmp1 * tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}
				else if (((String)oper[i]).Equals("/")) {
					double tmp1 = ((Double)args[i]);
					double tmp2 = ((Double)args[i+1]);
					args[i] = tmp1 / tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}

			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("+")) {
					double tmp1 = ((Double)args[i]);
					double tmp2 = ((Double)args[i+1]);
					args[i] = tmp1 + tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}
				else if (((String)oper[i]).Equals("-")) {
					double tmp1 = ((Double)args[i]);
					double tmp2 = ((Double)args[i + 1]);
					args[i] = tmp1 - tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}

			return ((Double)args[0]);
		}// evalReal()
	
		private bool evalBool(AdvLexem[] lex)
		{
			List<Boolean> args = new List<Boolean>();
			List<string> oper = new List<string>();
			List<Int32> nots = new List<Int32>();

			int pointer = 0;

			while (place < lex.Length) {
				if (lex[place].code == Codes.RBRACKET) {
					break;
				}
				if (lex[place].code == Codes.VARIABLE) {
					int tmp = vars.IndexOf(lex[place].value);
					int type = ((Int32)var_types[tmp]);
					if (type == Codes.BOOLEAN) 
						pointer = 1;
					else
						pointer = 2;
				}

				if ((lex[place].code == Codes.TRUE) ||
					(lex[place].code == Codes.FALSE))
					args.Add(Boolean.Parse(lex[place].value));
				else if (pointer == 1) {
					int tmp = vars.IndexOf(lex[place].value);
					args.Add((Boolean)var_vals[tmp]);
					pointer = 0;
				}
				else if (lex[place].code == Codes.LBRACKET) {
					place ++;
					args.Add(evalBool(lex) );	
				}
				else if ((lex[place].code == Codes.AND) ||
						 (lex[place].code == Codes.OR))
					oper.Add(lex[place].value);
				else if (lex[place].code == Codes.NOT) 
					nots.Add(args.Count);
				else {
					double tmp1 = 0;
					double tmp2 = 0;
					int rel = 0;
					if (lex[place].lcategory == Codes.NUMBER)
						tmp1 = (Double.Parse(lex[place].value));
					else if (pointer == 2) {
						pointer = 0;
						int tmp = vars.IndexOf(lex[place].value);
						int type = ((Int32)var_types[tmp]);
						if (type == Codes.INTEGER) 
							tmp1 = ((Int32)var_vals[tmp]);
						else
							tmp1 = ((Double)var_vals[tmp]);
					}
					place ++;
					rel = lex[place].code;
					place ++;
					if (lex[place].lcategory == Codes.NUMBER)
						tmp2 = (Double.Parse(lex[place].value));
					else if (lex[place].code == Codes.VARIABLE) {
						int tmp = vars.IndexOf(lex[place].value);
						int type = ((Int32)var_types[tmp]);
						if (type == Codes.INTEGER) 
							tmp2 = ((Int32)var_vals[tmp]);
						else
							tmp2 = ((Double)var_vals[tmp]);
					}
					bool boolean = false;
					if (rel == Codes.E)
						boolean = tmp1 == tmp2;
					else if (rel == Codes.NE)
						boolean = tmp1 != tmp2;
					else if (rel == Codes.GT) {
						boolean = tmp1 > tmp2;
					}
					else if (rel == Codes.GE)
						boolean = tmp1 >= tmp2;
					else if (rel == Codes.LT)
						boolean = tmp1 < tmp2;
					else if (rel == Codes.LE)
						boolean = tmp1 <= tmp2;

					args.Add(boolean);
				}
				
				place ++;
			}
		
			for (int i = 0; i < nots.Count; i++) {
				int neg = ((Int32)nots[i]);
				bool temp = ((Boolean)args[neg]);
				args[neg] = !temp;
			}

			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("AND")) {
					bool tmp1 = ((Boolean)args[i]);
					bool tmp2 = ((Boolean)args[i+1]);
					args[i] = tmp1 && tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}
			for (int i = 0; i < oper.Count; i++)
				if (((String)oper[i]).Equals("OR")) {
					bool tmp1 = ((Boolean)args[i]);
					bool tmp2 = ((Boolean)args[i+1]);
					args[i] = tmp1 || tmp2;
					args.RemoveAt(i + 1);
					oper.RemoveAt(i);
					i --;
				}

			return ((Boolean)args[0]);
		}// evalBool()

	//mmmmmmmmmmmmmmmmmmmmmmmmmmm	EXECUTOR	mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm

		private void execute(List<ExecLexem> al)
		{
			for (int i = 0; i < al.Count; i++)
				execLexem((ExecLexem)al[i]);
		}// execute()

		private void execLexem(ExecLexem el) {
			if (el.type.Equals("IASSOC"))
				execInt(el);
			else if (el.type.Equals("RASSOC"))
				execReal(el);
			else if (el.type.Equals("BASSOC"))
				execBool(el);
			else if (el.type.Equals("READ"))
				execRead(el);
			else if (el.type.Equals("WRITE"))
				execWrite(el);
			else if (el.type.Equals("WHILE"))
				execWhile(el);
			else if (el.type.Equals("IF"))
				execIf(el);

		}// execLexem()

		private void execInt(ExecLexem el) {
			int tmp = vars.IndexOf(el.variable);
			place = 0;
			var_vals[tmp] = evalInt(el.lexems);
		}// execInt()
	
		private void execReal(ExecLexem el) {
			int tmp = vars.IndexOf(el.variable);
			place = 0;
			var_vals[tmp] = evalReal(el.lexems);
		}// execReal()
	
		private void execBool(ExecLexem el) {
			int tmp = vars.IndexOf(el.variable);
			place = 0;
			var_vals[tmp] =evalBool(el.lexems);
		}// execBool()

		private void execWhile(ExecLexem el) {
			place = 0;
		
			while (evalBool(el.lexems)) {
				execute(el.iftrue);
				place = 0;
			}
		}// execWhile()

		private void execIf(ExecLexem el) {
			place = 0;
		
			if (evalBool(el.lexems))
				execute(el.iftrue);
			else
				execute(el.ifelse);
		}// execIf()


		private volatile bool _inputCompleted;
		private string _inputResult;

		private void execRead(ExecLexem el)
		{
			int tmp = vars.IndexOf(el.lexems[1].value);
			int type = ((Int32)var_types[tmp]);

			try
			{
				_inputCompleted = false;
				_inputResult = "";
				Guide.BeginShowKeyboardInput(Microsoft.Xna.Framework.PlayerIndex.One, "User input", "Enter data for readln() function", "", new AsyncCallback(GetReadlnString), null);

				while (!_inputCompleted)
				{
					Thread.Sleep(20);
				}

			}
			catch (Exception ex)
			{ 
				Console.WriteLine("Error during input operatiron !");
			}

			if (type == Codes.INTEGER)
				var_vals[tmp] = Int32.Parse(_inputResult);
			else if (type == Codes.REAL)
				var_vals[tmp] = Double.Parse(_inputResult);
			else
				var_vals[tmp] = Boolean.Parse(_inputResult);
 
		}

		void GetReadlnString(IAsyncResult res)
		{
			_inputResult = Guide.EndShowKeyboardInput(res);
			_inputCompleted = true;
		}
	
		private void execWrite(ExecLexem el) {
			for (int i = 1; i < el.lexems.Length; i = i + 2)
				if (el.lexems[i].code == Codes.STRING)
				{
					stdout += el.lexems[i].value;
					Console.WriteLine(el.lexems[i].value);

				}
				else
				{
					int tmp = vars.IndexOf(el.lexems[i].value);
					int type = ((Int32)var_types[tmp]);
					String s = "";

					if (type == Codes.INTEGER)
						s = ((Int32)var_vals[tmp]).ToString();
					else if (type == Codes.REAL)
						s = ((Double)var_vals[tmp]).ToString();
					else
						s = ((Boolean)var_vals[tmp]).ToString();
					stdout += s;
					Console.WriteLine(s);
				}
			stdout += Environment.NewLine;

		}// execWrite()

		private void err(String msg)
		{
			stderr += msg + Environment.NewLine;
			Console.WriteLine(msg);
		}
	
	}
}
