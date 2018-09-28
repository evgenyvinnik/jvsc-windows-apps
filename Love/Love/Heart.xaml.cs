using System.Windows;
using System.Windows.Media;

namespace Love
{
	public partial class Heart
	{
		public Heart()
		{
			InitializeComponent();
			DrawHeart(48);
		}

		public Heart(int size)
		{
			InitializeComponent();
			DrawHeart(size);
		}

		void DrawHeart(int size)
		{
			//lower point
			HeartArea.Points.Add(new Point(0.500 * size, 1.0 * size));


			HeartArea.Points.Add(new Point(0.460 * size, 0.910 * size));
			HeartArea.Points.Add(new Point(0.382 * size, 0.792 * size));
			HeartArea.Points.Add(new Point(0.253 * size, 0.652 * size));
			HeartArea.Points.Add(new Point(0.110 * size, 0.493 * size));
			HeartArea.Points.Add(new Point(0.038 * size, 0.398 * size));
			HeartArea.Points.Add(new Point(0.008 * size, 0.305 * size));
			HeartArea.Points.Add(new Point(0.004 * size, 0.241 * size));
			HeartArea.Points.Add(new Point(0.018 * size, 0.167 * size));
			HeartArea.Points.Add(new Point(0.043 * size, 0.117 * size));
			HeartArea.Points.Add(new Point(0.091 * size, 0.063 * size));
			HeartArea.Points.Add(new Point(0.140 * size, 0.031 * size));
			HeartArea.Points.Add(new Point(0.198 * size, 0.010 * size));
			HeartArea.Points.Add(new Point(0.226 * size, 0.006 * size));
			HeartArea.Points.Add(new Point(0.280 * size, 0.010 * size));
			HeartArea.Points.Add(new Point(0.322 * size, 0.017 * size));
			HeartArea.Points.Add(new Point(0.385 * size, 0.043 * size));
			HeartArea.Points.Add(new Point(0.441 * size, 0.093 * size));
			HeartArea.Points.Add(new Point(0.471 * size, 0.137 * size));
			HeartArea.Points.Add(new Point(0.494 * size, 0.180 * size));

			HeartArea.Points.Add(new Point(0.500 * size, 0.208 * size));

			HeartArea.Points.Add(new Point((1 - 0.494) * size, 0.180 * size));
			HeartArea.Points.Add(new Point((1 - 0.471) * size, 0.137 * size));
			HeartArea.Points.Add(new Point((1 - 0.441) * size, 0.093 * size));
			HeartArea.Points.Add(new Point((1 - 0.385) * size, 0.043 * size));
			HeartArea.Points.Add(new Point((1 - 0.322) * size, 0.017 * size));
			HeartArea.Points.Add(new Point((1 - 0.280) * size, 0.010 * size));
			HeartArea.Points.Add(new Point((1 - 0.226) * size, 0.006 * size));
			HeartArea.Points.Add(new Point((1 - 0.198) * size, 0.010 * size));
			HeartArea.Points.Add(new Point((1 - 0.140) * size, 0.031 * size));
			HeartArea.Points.Add(new Point((1 - 0.091) * size, 0.063 * size));
			HeartArea.Points.Add(new Point((1 - 0.043) * size, 0.117 * size));
			HeartArea.Points.Add(new Point((1 - 0.018) * size, 0.167 * size));
			HeartArea.Points.Add(new Point((1 - 0.004) * size, 0.241 * size));
			HeartArea.Points.Add(new Point((1 - 0.008) * size, 0.305 * size));
			HeartArea.Points.Add(new Point((1 - 0.038) * size, 0.398 * size));
			HeartArea.Points.Add(new Point((1 - 0.110) * size, 0.493 * size));
			HeartArea.Points.Add(new Point((1 - 0.253) * size, 0.652 * size));
			HeartArea.Points.Add(new Point((1 - 0.382) * size, 0.792 * size));

			HeartArea.Points.Add(new Point(0.500 * size, 1.0 * size));
			HeartArea.Points.Add(new Point(0.500 * size, 1.0 * size));
		}

		public Brush Fill
		{
			get { return HeartArea.Fill; }
			set { HeartArea.Fill = value; }
		}
	}
}
