using System;
using System.Windows;
using System.Windows.Media;

namespace Hextris
{
	public partial class HexTile
	{
		public HexTile(double hexagonRadius)
		{
			InitializeComponent();

			var cy = Math.Round(2.0 * hexagonRadius * Math.Cos(Math.PI / 6.0));
			var cx = 2.0 * hexagonRadius + 3.0 * hexagonRadius;

			TileArea.Points.Add(new Point(cx + 2.0 * hexagonRadius, cy));
			TileArea.Points.Add(new Point(cx + hexagonRadius, cy + cy));
			TileArea.Points.Add(new Point(cx - hexagonRadius, cy + cy));
			TileArea.Points.Add(new Point(cx - 2.0 * hexagonRadius, cy));
			TileArea.Points.Add(new Point(cx - hexagonRadius, 0.0));
			TileArea.Points.Add(new Point(cx + hexagonRadius, 0.0));
			TileArea.Points.Add(new Point(cx + 2.0 * hexagonRadius, cy));
		}

		public Brush Fill
		{
			get { return TileArea.Fill; }
			set { TileArea.Fill = value; }
		}

		public Brush Stroke
		{
			get { return TileArea.Stroke; }
			set { TileArea.Stroke = value; }
		}

		public Visibility TileVisibility
		{
			get { return TileArea.Visibility; }
			set { TileArea.Visibility = value; }
		}
	}
}
