namespace Hextris
{
	/// <summary>
	///
	/// Implements the Hextris gameboard (or stone board).
	/// 
	/// Supports clearing the field, setting and reading single fields.
	/// Removelines supports deleting full lines and moving the rest down.
	/// 
	/// </summary>
	public class Board
	{
		#region variables

		#region public variables
		public int[,] GameField;
		public int BoardWidth;
		public int BoardHeight;
		#endregion

		#endregion

		/// <summary>
		/// New board with given size..
		/// </summary>
		/// <param name="boardWidth">Width of the board.</param>
		/// <param name="boardHeight">Height of the board.</param>
		public Board(int boardWidth, int boardHeight)
		{
			BoardWidth = boardWidth;
			BoardHeight = boardHeight;

			GameField = new int[boardHeight, boardWidth];
		}

		/// <summary>
		/// New board with given field.
		/// </summary>
		/// <param name="gameField">The given game field.</param>
		public Board(int[,] gameField)
		{
			BoardHeight = gameField.GetLength(0);
			BoardWidth = gameField.GetLength(1);

			GameField = gameField;
		}

		/// <summary>
		/// Is the line full?
		/// </summary>
		public bool IsLineFull(int y)
		{
			for (var x = 1; x < BoardWidth - 1; x++)
			{
				if (GameField[y, x] == 0)
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// clears the line
		/// </summary>
		public void ClearLine(int y)
		{
			for (var x = 1; x < BoardWidth - 1; x++)
			{
				GameField[y, x] = 0;
			}
		}

		private void CopyLine(int srcY, int destY)
		{
			for (var x = 1; x < BoardWidth - 1; x++)
			{
				GameField[destY, x] = GameField[srcY, x];
			}
		}

		/// <summary>
		/// Moves all lines down
		/// </summary>
		public void RemoveLine(int y)
		{
			for (var cY = y; cY > 0; cY--)
			{
				CopyLine(cY - 1, cY);
			}

			for (var x = 1; x < BoardWidth - 1; x++)
			{
				GameField[0, x] = 0;
			}
		}


		public void DrawPlayField()
		{
			for (var y = 0; y < BoardHeight - 1; y++)
			{
				GameField[y, 0] = 1;
				for (var x = 1; x < BoardWidth - 1; x++)
				{
					GameField[y, x] = 0;
				}
				GameField[y, BoardWidth - 1] = 1;
			}

			for (var x = 0; x < BoardWidth; x++)
			{
				GameField[BoardHeight - 1, x] = 1;
			}
		}

		/**
		 *
		 * @param x
		 * @param y
		 * @param color
		 */
		public void SetField(int x, int y, int color)
		{
			if (x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight)
			{
				GameField[y, x] = color;
			}
		}

		/**
		 *
		 * @param x
		 * @param y
		 * @return
		 */
		public int GetField(int x, int y)
		{
			if (x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight)
			{
				return GameField[y, x];
			}
			return 1;
		}

		/**
		 *
		 * @param is
		 */
		public void SetField(int[,] newField)
		{
			GameField = newField;
		}

		/**
		 * Computes the rotated board.
		 * Tricky algorithm: position on the board is translated into a path of steps
		 * from the center hexagon to the rotated hexagon. thus one path component
		 * is the number of steps  that are either up/down, on a 60 degree line or
		 * 120 degree line starting. the path is easily rotated by shifting the
		 * components. finally the path is translated into the new coordinate.
		 * @param cx x-coordinate of center
		 * @param cy y-coordinate of center
		 * @param direction left or right
		 * @return a new board with the rotated field
		 */
		public Board GetFieldRotate(int cx, int cy, bool direction)
		{
			var newField = new int[BoardHeight, BoardWidth];


			for (var y = 0; y < BoardHeight; y++)
			{
				for (var x = 0; x < BoardWidth; x++)
				{
					if (GameField[y, x] == 0)
						continue;

					//compute path
					var path = new[] { 0, 0, 0 };
					var dx = x - cx;
					var dy = y - cy;
					path[0] += dy;
					path[1] += dx / 2;
					path[2] += dx / 2;

					if (dx % 2 > 0)
					{
						path[1] += 1;
					}
					if (dx % 2 < 0)
					{
						path[2] -= 1;
					}

					//rotate path
					var newPath = direction ? new[] { -path[2], path[0], path[1] } : new[] { path[1], path[2], -path[0] };

					//compute new coordinates one path component at a time
					var newx = cx;
					var newy = cy;

					//path[0]
					newy += newPath[0];

					//path[1]
					if (newx % 2 == 1 && newPath[1] % 2 == 1)
					{
						newy += 1;
					}

					if (newx % 2 == 0 && newPath[1] % 2 == -1)
					{
						newy -= 1;
					}

					newx += newPath[1];
					newy += newPath[1] / 2;

					//path[2]
					if (newx % 2 == 0 && newPath[2] % 2 == 1)
					{
						newy -= 1;
					}
					if (newx % 2 == 1 && newPath[2] % 2 == -1)
					{
						newy += 1;
					}

					newx += newPath[2];
					newy -= newPath[2] / 2;

					if (newx < BoardWidth && newx >= 0 && newy < BoardHeight && newy >= 0)
					{
						newField[newy, newx] = GameField[y, x];
					}
				}
			}

			return new Board(newField);
		}

		/**
		 *
		 * @param direction
		 * @return
		 */
		public int[] GetSurface(bool direction)
		{
			var res = new int[BoardWidth];

			for (var x = 0; x < BoardWidth; x++)
			{
				res[x] = -1;
				for (var y = direction ? 0 : BoardHeight - 1; y != (direction ? BoardHeight : -1); y += direction ? 1 : -1)
				{
					if (GameField[y, x] == 0)
						continue;

					res[x] = y;
					break;
				}
			}

			return res;
		}
	}
}
