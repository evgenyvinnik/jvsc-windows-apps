using System;
using System.Collections.Generic;

namespace Clarity.Phone.Interactivity.Input.Dollar
{
	public class Unistroke : IComparable
	{
		public string Name;
		//public ArrayList RawPoints; // raw points (for drawing) -- read in from XML
		public List<PointR> RawPoints; // raw points (for drawing) -- read in from XML
		//public ArrayList Points;    // pre-processed points (for matching) -- created when loaded
		public List<PointR> Points;    // pre-processed points (for matching) -- created when loaded

		public Unistroke()
		{
			Name = String.Empty;
			RawPoints = null;
			Points = null;
		}

		/// <summary>
		/// Constructor of a unistroke gesture. A unistroke is comprised of a set of points drawn
		/// out over time in a sequence.
		/// </summary>
		/// <param name="name">The name of the unistroke gesture.</param>
		/// <param name="points">The array of points supplied for this unistroke.</param>
		//public Unistroke(string name, ArrayList points)
		public Unistroke(string name, List<PointR> points)
		{
			Name = name;
			RawPoints = new List<PointR>(points); // copy (saved for drawing)

			Points = Utils.Resample(points, Recognizer.NumPoints);
			var radians = Utils.AngleInRadians(Utils.Centroid(Points), Points[0], false);
			Points = Utils.RotateByRadians(Points, -radians);
			Points = Utils.ScaleTo(Points, Recognizer.SquareSize);
			Points = Utils.TranslateCentroidTo(Points, Recognizer.Origin);
		}

		/// <summary>
		/// 
		/// </summary>
		public int Duration
		{
			get
			{
				if (RawPoints.Count >= 2)
				{
					var p0 = RawPoints[0];
					var pn = RawPoints[RawPoints.Count - 1];
					return pn.T - p0.T;
				}

				return 0;
			}
		}

		// sorts in descending order of Score
		public int CompareTo(object obj)
		{
			if (obj is Unistroke)
			{
				var g = (Unistroke) obj;
				return Name.CompareTo(g.Name);
			}

			throw new ArgumentException("object is not a Gesture");
		}

		/// <summary>
		/// Pulls the gesture name from the file name, e.g., "circle03" from "C:\gestures\circles\circle03.xml".
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ParseName(string filename)
		{
			var start = filename.LastIndexOf('\\');
			var end = filename.LastIndexOf('.');
			return filename.Substring(start + 1, end - start - 1);
		}

	}
}
