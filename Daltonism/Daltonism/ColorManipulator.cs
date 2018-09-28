using System;

namespace Daltonism
{
	public class ColorManipulator
	{

		#region colorDifference
		public static double CIELabOriginal(CIELab lab1, CIELab lab2)
		{
			var dl = lab2.L - lab1.L;
			var da = lab2.A - lab1.A;
			var db = lab2.B - lab1.B;

			return Math.Sqrt(dl * dl + da * da + db * db);
		}

		public static double CIELab94(CIELab lab1, CIELab lab2)
		{
			var sl = 1.0;
			var kl = 1.0;
			var kc = 1.0;
			var kh = 1.0;

			var c1 = Math.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
			var c2 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);

			var dl = lab2.L - lab1.L;
			var dc = c2 - c1;
			var de = CIELabOriginal(lab1, lab2);

			var dh = 0.0;
			if ((de * de) > (dl * dl + dc * dc))
			{
				dh = Math.Sqrt(de * de - dl * dl - dc * dc);
			}

			var sc = 1.0 + 0.045 * c1;
			var sh = 1.0 + 0.015 * c1;

			return Math.Sqrt((dl / (kl * sl)) * (dl / (kl * sl)) + (dc / (kc * sc)) * (dc / (kc * sc)) + (dh / (kh * sh)) * (dh / (kh * sh)));
		}

		//by http://www.ece.rochester.edu/~gsharma/ciede2000/ciede2000noteCRNA.pdf
		public static double CIELab2000(CIELab lab1, CIELab lab2)
		{
			//step 1
			var c1 = Math.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
			var c2 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);

			var averageC = (c1 + c2)/2.0;

			var g = 0.5*(1.0 - Math.Sqrt( Math.Pow(averageC, 7.0) / (Math.Pow(averageC, 7.0) + 6103515625.0) ));

			var a1 = (1.0 + g) * lab1.A;
			var a2 = (1.0 + g) * lab2.A;

			var c11 = Math.Sqrt(a1 * a1 + lab1.B * lab1.B);
			var c12 = Math.Sqrt(a2 * a2 + lab2.B * lab2.B);

			var h1 = 0.0;
			if (lab1.B != 0.0 || a1 != 0.0)
			{
				if (lab1.B >= 0)
				{
					h1 = Math.Atan2(lab1.B, a1) * 180.0 / Math.PI;
				}
				else
				{
					h1 = Math.Atan2(lab1.B, a1) * 180.0 / Math.PI + 360.0;
				}
			}

			var h2 = 0.0;
			if (lab2.B != 0.0 || a2 != 0.0)
			{
				if (lab2.B >= 0)
				{
					h2 = Math.Atan2(lab2.B, a2) * 180.0 / Math.PI;
				}
				else
				{
					h2 = Math.Atan2(lab2.B, a2) * 180.0 / Math.PI + 360.0;
				}
			}

			//step2

			var dh = 0.0;
			if(c11 * c12 != 0.0)
			{
				if ((h2 - h1 <= 180.0) && (h2 - h1) >= -180.0)
				{
					dh = h2 - h1;
				}
				else if ((h2 - h1) > 180.0)
				{
					dh = h2 - h1 - 360.0;
				}
				else if ((h2 - h1) < -180.0)
				{
					dh = h2 - h1 + 360.0;
				}

			}

			var dH = 2.0 * Math.Sqrt(c11*c12) * Math.Sin( (dh/2.0) * Math.PI / 180.0);

			//step 3

			var averageL = ( lab2.L + lab1.L ) / 2.0;
			var averageC1 = ( c11 + c12 ) / 2.0;

			var averageh =  h1 + h2;

			if(c11 * c12 != 0.0)
			{
				if ((h2 - h1) >= -180.0 && (h2 - h1) <= 180.0)
				{
					averageh = ( h1 + h2 ) / 2.0;
				}
				else if (Math.Abs(h2 - h1) > 180.0 && (h2 + h1) < 360.0)
				{
					averageh = ( h1 + h2 ) / 2.0 + 180.0;
				}
				else if (Math.Abs(h2 - h1) > 180.0 && (h2 + h1) >= 360.0)
				{
					averageh = (h1 + h2) / 2.0 - 180.0;
				}
			}


			var lave50 = (averageL - 50.0) * (averageL - 50.0);
			var sl = 1.0 + (0.015 * lave50) / (Math.Sqrt(20.0 + lave50) );

			var sc = 1.0 + 0.045 * averageC1;


			var t = 1.0 - 0.17 * Math.Cos( (averageh - 30.0) * Math.PI / 180.0)
				+ 0.24 * Math.Cos((2.0 * averageh) * Math.PI / 180.0)
				+ 0.32 * Math.Cos((3.0 * averageh + 6.0) * Math.PI / 180.0)
				- 0.2 * Math.Cos((4.0 * averageh - 63) * Math.PI / 180.0);

			var sh = 1.0 + 0.015 * averageC1 * t;

			var dteta = 30.0 * Math.Exp(-1 * ( Math.Pow( (averageh - 275.0) / 25.0 , 2.0) ) );

			var rc = 2.0 * Math.Sqrt(Math.Pow(averageC1, 7.0) / (Math.Pow(averageC1, 7.0) + 6103515625.0));

			var rt = - Math.Sin( (2.0 * dteta) * Math.PI / 180.0 ) * rc;

			var deltal = lab2.L - lab1.L;
			var deltac = c12 - c11;

			const double kl = 1.0;
			const double kc = 1.0;
			const double kh = 1.0;

			var first = deltal / kl / sl;
			var second = deltac / kc / sc;
			var third = dH / kh / sh;

			return Math.Sqrt( first*first + second*second + third*third + rt * second * third);
		}

		public static string Grade(double difference)
		{
			if (difference < 1)
				return "A+";
			if (difference < 3)
				return "A";
			if (difference < 5)
				return "A-";
			if (difference < 7)
				return "B+";
			if (difference < 10)
				return "B";
			if (difference < 13)
				return "B-";
			if (difference < 15)
				return "C+";
			if (difference < 17)
				return "C";
			if (difference < 20)
				return "C-";
			if (difference < 25)
				return "D+";
			if (difference < 27)
				return "D";
			if (difference < 30)
				return "D";

			return "F";
		}
		#endregion


		/// <summary>
		/// Converts RGB to CIELab.
		/// </summary>
		public static CIELab RGBtoLab(int red, int green, int blue)
		{
			return XYZtoLab(RGBtoXYZ(red, green, blue));
		}

		/// <summary>
		/// Converts RGB to CIE XYZ (CIE 1931 color space)
		/// </summary>
		/// <param name="red">Red must be in [0, 255].</param>
		/// <param name="green">Green must be in [0, 255].</param>
		/// <param name="blue">Blue must be in [0, 255].</param>
		public static CIEXYZ RGBtoXYZ(int red, int green, int blue)
		{
			// normalize red, green, blue values
			var rLinear = red / 255.0;
			var gLinear = green / 255.0;
			var bLinear = blue / 255.0;

			// convert to a sRGB form
			var r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.2) : (rLinear / 12.92);
			var g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.2) : (gLinear / 12.92);
			var b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.2) : (bLinear / 12.92);

			// converts
			return new CIEXYZ(
				(r * 0.4124 + g * 0.3576 + b * 0.1805),
				(r * 0.2126 + g * 0.7152 + b * 0.0722),
				(r * 0.0193 + g * 0.1192 + b * 0.9505)
				);
		}

		/// <summary>
		/// XYZ to L*a*b* transformation function.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private static double Fxyz(double t)
		{
			return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
		}

		/// <summary>
		/// Converts CIEXYZ to CIELab structure.
		/// </summary>
		public static CIELab XYZtoLab(double x, double y, double z)
		{
			var lab = CIELab.Empty;

			lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
			lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
			lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

			return lab;
		}

		/// <summary>
		/// Converts CIEXYZ to CIELab structure.
		/// </summary>
		public static CIELab XYZtoLab(CIEXYZ xyz)
		{
			return XYZtoLab(xyz.X, xyz.Y, xyz.Z);
		}
	}

	/// <summary>
	/// Structure to define CIE L*a*b*.
	/// </summary>
	public struct CIELab
	{
		/// <summary>
		/// Gets an empty CIELab structure.
		/// </summary>
		public static readonly CIELab Empty;

		#region Fields
		private double _l;
		private double _a;
		private double _b;

		#endregion

		#region Operators
		public static bool operator ==(CIELab item1, CIELab item2)
		{
			return (
				item1.L == item2.L
				&& item1.A == item2.A
				&& item1.B == item2.B
				);
		}

		public static bool operator !=(CIELab item1, CIELab item2)
		{
			return (
				item1.L != item2.L
				|| item1.A != item2.A
				|| item1.B != item2.B
				);
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets or sets L component.
		/// </summary>
		public double L
		{
			get
			{
				return _l;
			}
			set
			{
				_l = value;
			}
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double A
		{
			get
			{
				return _a;
			}
			set
			{
				_a = value;
			}
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double B
		{
			get
			{
				return _b;
			}
			set
			{
				_b = value;
			}
		}

		#endregion

		public CIELab(double l, double a, double b)
		{
			_l = l;
			_a = a;
			_b = b;
		}

		#region Methods
		public override bool Equals(Object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;

			return (this == (CIELab)obj);
		}

		public override int GetHashCode()
		{
			return L.GetHashCode() ^ _a.GetHashCode() ^ _b.GetHashCode();
		}

		#endregion
	}
	/// <summary>
	/// Structure to define CIE XYZ.
	/// </summary>
	public struct CIEXYZ
	{
		/// <summary>
		/// Gets the CIE D65 (white) structure.
		/// </summary>
		public static readonly CIEXYZ D65 = new CIEXYZ(0.9505, 1.0, 1.0890);

		#region Fields
		private double _x;
		private double _y;
		private double _z;

		#endregion

		#region Operators
		public static bool operator ==(CIEXYZ item1, CIEXYZ item2)
		{
			return (
				item1.X == item2.X
				&& item1.Y == item2.Y
				&& item1.Z == item2.Z
				);
		}

		public static bool operator !=(CIEXYZ item1, CIEXYZ item2)
		{
			return (
				item1.X != item2.X
				|| item1.Y != item2.Y
				|| item1.Z != item2.Z
				);
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets or sets X component.
		/// </summary>
		public double X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = (value > 0.9505) ? 0.9505 : ((value < 0) ? 0 : value);
			}
		}

		/// <summary>
		/// Gets or sets Y component.
		/// </summary>
		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = (value > 1.0) ? 1.0 : ((value < 0) ? 0 : value);
			}
		}

		/// <summary>
		/// Gets or sets Z component.
		/// </summary>
		public double Z
		{
			get
			{
				return _z;
			}
			set
			{
				_z = (value > 1.089) ? 1.089 : ((value < 0) ? 0 : value);
			}
		}

		#endregion

		public CIEXYZ(double x, double y, double z)
		{
			_x = (x > 0.9505) ? 0.9505 : ((x < 0) ? 0 : x);
			_y = (y > 1.0) ? 1.0 : ((y < 0) ? 0 : y);
			_z = (z > 1.089) ? 1.089 : ((z < 0) ? 0 : z);
		}

		#region Methods
		public override bool Equals(Object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;

			return (this == (CIEXYZ)obj);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		#endregion
	}
}
