using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hextris
{
	public partial class GamePanel
	{
		#region variables

		#region public variables

		public int PanelWidth;
		public int PanelHeight;
		public Board Board;

		#endregion

		#region private variables
		private readonly HexTile[,] _tiles;

		private readonly double _hexagonRadius;
		private readonly double _bh;
		private readonly double _hexHeight;
		private readonly double _hexWidth;
		#endregion
		
		#endregion

		public GamePanel(int width, int height, double hexagonRadius)
		{
			InitializeComponent();

			PanelWidth = width;
			PanelHeight = height;
			Board = new Board(width, height);

			_tiles = new HexTile[PanelWidth, PanelHeight];

			_hexagonRadius = hexagonRadius;

			_bh = Math.Round(2.0 * _hexagonRadius * Math.Cos(Math.PI / 6.0));

			_hexHeight = 2.0 * _bh;
			_hexWidth = 3.0 * _hexagonRadius;

			DrawBoard();
		}

		public void DrawBoard()
		{
			GamePanelCanvas.Children.Clear();

			for (var x = 0; x < Board.BoardWidth; x++)
			{
				var lineOffset = (x % 2.0) * (_hexHeight / 2.0);

				for (var y = 0; y < Board.BoardHeight; y++)
				{
					var a = new Point(x * _hexWidth - _hexWidth / 2.0 - _hexagonRadius, y * _hexHeight - _bh + lineOffset);
					_tiles[x, y] = new HexTile(_hexagonRadius);
					_tiles[x, y].SetValue(Canvas.LeftProperty, a.X);
					_tiles[x, y].SetValue(Canvas.TopProperty, a.Y);

					GamePanelCanvas.Children.Add(_tiles[x, y]);
				}
			}
		}

		public void Repaint()
		{
			for (var x = 0; x < Board.BoardWidth; x++)
			{
				for (var y = 0; y < Board.BoardHeight; y++)
				{
					var colorId = Board.GetField(x, y);

					if (colorId == 0)
					{
						_tiles[x, y].Fill = new SolidColorBrush(Colors.Black);
						_tiles[x, y].Stroke = new SolidColorBrush(Colors.White);
						_tiles[x, y].TileVisibility = Visibility.Collapsed;
					}
					else
					{
						switch (colorId)
						{
							case 1:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255,155,0,151));
								break;
							case 2:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 162, 0, 255));
								break;
							case 3:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 140, 191, 38));
								break;
							case 4:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 160, 80, 0));
								break;
							case 5:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 230, 113, 184));
								break;
							case 6:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
								break;
							case 7:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));
								break;
							case 8:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 229, 20, 0));
								break;
							case 9:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));
								break;
							case 10:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 155, 0, 151));
								break;
							case 11:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 140, 191, 38));
								break;
							case 12:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 230, 113, 184));
								break;
							case 13:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
								break;
							case 14:
								_tiles[x, y].Fill = new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));
								break;
							default:
								_tiles[x, y].Fill = new SolidColorBrush((Color) Resources["PhoneAccentColor"]);
								break;
						}
						
						_tiles[x, y].Stroke = new SolidColorBrush(Colors.Black);
						_tiles[x, y].TileVisibility = Visibility.Visible;
					}

				}
			}
		}

		public int RemoveFillLines()
		{
			var lines = 0;

			for (var y = Board.BoardHeight - 2; y > 0; y--)
			{
				if (!Board.IsLineFull(y))
					continue;

				Board.ClearLine(y);
				Repaint();
				Board.RemoveLine(y);
				Repaint();
				lines++;
				y++;
			}

			return lines;
		}
	}
}
