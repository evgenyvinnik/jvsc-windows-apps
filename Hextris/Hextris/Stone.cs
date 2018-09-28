using System;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Hextris
{
	/// <summary>
	/// Models a stone in the Tetris game.
	/// 
	/// A stone can be place on the Board and taken from it.
	/// It can be rotated.
	/// 
	/// The Constant stones contains the standard hextris stones.
	/// 
	/// The moving of a stone is modeled inside the Tetris class as it
	/// has some important effects on the game.
	/// 
	/// </summary>
	public class Stone : Board
	{
		#region Variables

		public enum Movements
		{
			MoveDown,
			MoveLeft,
			MoveRight,
			RotateLeft,
			RotateRight
		}

		#region public variables
		public int PosX;
		public int PosY;
		public int Color; 
		#endregion

		#region private variables
		private readonly Board _board;
		private int[,] _severities;

		/**
		 * array with stones
		 */
		private int[, ,] _stones;  
		#endregion
		#endregion

		/**
		 * Creates a random stone according to severity.
		 * @param board
		 * @param severity
		 */
		public Stone(Board board, int severity):
			base (5,5)
		{
			InitReadOnlyVarialbles();
			var r = new Random();
			SetType(r.Next(_severities[severity, 1] - _severities[severity, 0]) + _severities[severity, 0]);
			_board = board;
			PosX = 0;
			PosY = 0;
		}

		/**
		 * Creates the stone from a given stone.
		 * @param stone
		 * @param board
		 */
		public Stone(Stone stone, Board board):
			base(5,5)
		{
			InitReadOnlyVarialbles();
			SetField(stone.GameField);
			Color = stone.Color;
			_board = board;
			PosX = 0;
			PosY = 0;
		}

		/**
		 * Creates the stone from stone index.
		 * @param i
		 */
		public Stone(int i):
			base(5,5)
		{
			InitReadOnlyVarialbles();
			SetField(new int[5, 5]);
			SetType(i);
			PosX = 0;
			PosY = 0;
		}


		void InitReadOnlyVarialbles()
		{
			_severities = new[,] { { 0, 7 }, { 3, 13 }, { 5, 16 } };

			_stones = new[, ,]
			         	{
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 1, 0, 1, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0},
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 1, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0},
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 1, 1, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 1, 1, 0, 0},
			         			{0, 1, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 1, 0},
			         			{0, 0, 0, 1, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 1, 1, 1, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 1, 0, 0, 0},
			         			{0, 1, 0, 1, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 1, 0, 1, 0},
			         			{0, 1, 1, 1, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 1, 1, 1, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 1, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		},
			         		{
			         			{0, 0, 0, 0, 0},
			         			{0, 1, 1, 1, 0},
			         			{0, 1, 0, 1, 0},
			         			{0, 0, 0, 0, 0},
			         			{0, 0, 0, 0, 0}
			         		}
					};

		}

		/**
		 * Initialize this stones board-field an color.
		 * @param nr
		 */
		private void SetType(int nr)
		{
			for (var y = 0; y < BoardHeight; y++)
			{
				for (var x = 0; x < BoardWidth; x++)
				{
					GameField[y, x] = _stones[nr, y, x];
				}
			}
			Color = nr + 2;
		}

		/**
		 * Sets stones position on board.
		 * @param x
		 * @param y
		 */
		public void SetPosition(int x, int y)
		{
			PosX = x;
			PosY = y;
		}

		/**
		 * Places or removes a stone from the board
		 * @param place true: place false: remove
		 */
		public void Place(bool place)
		{
			if (_board == null)
			{
				return;
			}

			for (var y = 0; y < BoardHeight; y++)
			{
				for (var x = 0; x < BoardWidth; x++)
				{
					var val = GameField[y, x];
					if (val == 0)
					{
						continue;
					}
					var bx = PosX + x;
					var by = PosY + y + Math.Abs((PosX % 2) * (x % 2));

					_board.SetField(bx, by, place ? Color : 0);
				}
			}
		}

		/**
		 * Tests if the stone can be placed at given position.
		 * @param nX
		 * @param nY
		 * @return
		 */
		public bool MayPlace(int nX, int nY)
		{
			if (_board == null)
			{
				return false;
			}

			for (var y = 0; y < BoardHeight; y++) 
			{
				for (var x = 0; x < BoardWidth; x++)
				{
					if ( GameField[y, x] == 0)
					{
						continue;
					}

					var bx = nX + x;
					var by = nY + y + Math.Abs((nX % 2) * (x % 2));

					if (by >= 0 && _board.GetField(bx, by) != 0) 
					{
						return false;
					}
				}
			}
			return true;
		}

		/**
		 * Moves the stone in specified way.
		 * It's synchronized so only one thread can move the stone at a time.
		 * @param type
		 * @return
		 */
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool MoveStone(Movements movement)
		{
			switch (movement)
			{
				case Movements.MoveDown:
					return MoveDown();
				case Movements.MoveLeft:
					return MoveHorizontal(Movements.MoveLeft);
				case Movements.MoveRight:
					return MoveHorizontal(Movements.MoveRight);
				case Movements.RotateLeft:
					return Rotate(true);
				case Movements.RotateRight:
					return Rotate(false);
				default:
					return false;
			}
		}


		/**
		 * Moves the stone over one position horizontally if possible.
		 * If stone can't be moved horizontally nor vertically it tries to move
		 * the stone horizontally+vertically together.
		 * @param direction to left or right
		 */
		private bool MoveHorizontal(Movements movement)
		{
			Place(false);

			var diffX = movement == Movements.MoveLeft ? -1 : 1;

			var diffY = 0;

			var mayPlace = MayPlace(PosX + diffX, PosY);

			if (!mayPlace && !MayPlace(PosX, PosY + 1)) 
			{
				diffY = 1;
				mayPlace = MayPlace(PosX + diffX, PosY + diffY);
			}

			if (mayPlace)
			{
				PosX += diffX;
				PosY += diffY;
			}

			Place(true);

			return mayPlace;
		}

		/**
		 * Move one position down.
		 * @return 
		 */
		public bool MoveDown()
		{
			Place(false);
			var mayPlace = MayPlace(PosX, PosY + 1);

			if (mayPlace)
			{
				PosY += 1;
			}

			Place(true);
			return mayPlace;
		}

		/**
		 * Rotates the stone on the board if possible.
		 * If stone can only be rotated by moving left or right, then it moves
		 * @param direction left or right
		 */
		private bool Rotate(bool direction)
		{
			var res = true;

			Place(false);
			var oldField = GameField;

			GameField = GetFieldRotate(2, 2, direction).GameField;

			//check posible x-positions
			if (MayPlace(PosX, PosY))
			{
				
			}
			else if (MayPlace(PosX + 1, PosY))
			{
				PosX += 1;
			}
			else if (MayPlace(PosX - 1, PosY))
			{
				PosX -= 1;
			}
			else if (MayPlace(PosX + 2, PosY))
			{
				PosX += 2;
			}
			else if (MayPlace(PosX - 2, PosY))
			{
				PosX -= 2;
			}
			else
			{
				GameField = oldField;
				res = false;
			}
			Place(true);

			return res;
		}

		/**
		 * 
		 * @return
		 */
		public int[] GetBestPosition()
		{
			var bestPos = new[]{-1, -1, -1, -1};
			var bestEval = -1;
			if (_board == null)
			{
				return bestPos;
			}

			var boardSurface = _board.GetSurface(true);

			for (var hpos = -2; hpos < _board.BoardWidth - 1; hpos++)
			{
				//horizontal positions
				var stoneBoard = new Board(GameField);
				for (var rots = 0; rots < 6; rots++)
				{

					//rotations
					var stoneSurface = stoneBoard.GetSurface(false);
					var valid = true;

					var diff = new int[stoneSurface.Length];
					var maxDiff = -1;
					var maxY = -1;
					for (var i = 0; i < diff.Length; i++)
					{
						var boardX = i + hpos;
						if (stoneSurface[i] == -1)
						{
							diff[i] = -1;
						}
						else if (boardX < 0 || i + hpos >= boardSurface.Length)
						{
							valid = false;
							break;
						}
						else
						{
							diff[i] = 5 + boardSurface[boardX] - stoneSurface[i];

							if (Math.Abs(hpos % 2) == 1 && i % 2 == 1)
							{
								diff[i]--;
							}
							if (diff[i] > maxDiff)
							{
								maxDiff = diff[i];
							}
							if (boardSurface[boardX] > maxY)
							{
								maxY = boardSurface[boardX];
							}
						}
					}

					if (valid)
					{

						var holeCount = diff.Where(t => t != -1).Sum(t => (maxDiff - t));

						//compare with best
						var eval = holeCount * 100 + (50 - maxY);
						if (bestEval == -1 || eval < bestEval)
						{
							bestPos[0] = hpos;
							bestPos[1] = rots;
							bestPos[2] = holeCount;
							bestPos[3] = maxDiff;
							bestEval = eval;
						}
					}

					//rotate for next test
					stoneBoard = stoneBoard.GetFieldRotate(2, 2, true);
				}

			}


			return bestPos;
		}
	}
}
