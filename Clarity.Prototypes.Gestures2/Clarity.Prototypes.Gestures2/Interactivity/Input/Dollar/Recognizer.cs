/*-------------------------------------------------
 *  $1 recognizer engine adapted for WP7 from c# one 
 *  posted at http://depts.washington.edu/aimgroup/proj/dollar/
----------------------------------------------------*/
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Clarity.Phone.Interactivity.Input.Dollar
{
	public class Recognizer
	{
		#region Members

		public const int NumPoints = 64;
		private const double Dx = 250.0;
		public static readonly SizeR SquareSize = new SizeR(Dx, Dx);
		public static readonly double Diagonal = Math.Sqrt(Dx * Dx + Dx * Dx);
		public static readonly double HalfDiagonal = 0.5 * Diagonal;
		public static readonly PointR Origin = new PointR(0, 0);
		private static readonly double Phi = 0.5 * (-1.0 + Math.Sqrt(5.0)); // Golden Ratio

		// batch testing
		private const int NumRandomTests = 100;
		public event ProgressEventHandler ProgressChangedEvent;

		//private Hashtable _gestures;
		private readonly Dictionary<string, Unistroke> _gestures;

		#endregion

		#region Constructor
	
		public Recognizer()
		{
			//_gestures = new Hashtable(256);
			_gestures = new Dictionary<string, Unistroke>(256);
		}

		#endregion

		#region Recognition

		//public NBestList Recognize(ArrayList points) // candidate points
		public NBestList Recognize(List<PointR> points) // candidate points
		{
			points = Utils.Resample(points, NumPoints);
			var radians = Utils.AngleInRadians(Utils.Centroid(points), points[0], false);
			points = Utils.RotateByRadians(points, -radians);
			points = Utils.ScaleTo(points, SquareSize);
			points = Utils.TranslateCentroidTo(points, Origin);

			var nbest = new NBestList();
			foreach (var u in _gestures.Values)
			{
				var best = GoldenSectionSearch(
					points,                 // to rotate
					u.Points,               // to match
					Utils.Deg2Rad(-45.0),   // lbound
					Utils.Deg2Rad(+45.0),   // ubound
					Utils.Deg2Rad(2.0)      // threshold
				);

				double score = 1.0 - best[0] / HalfDiagonal;
				nbest.AddResult(u.Name, score, best[0], best[1]); // name, score, distance, angle
			}
			nbest.SortDescending(); // sort so that nbest[0] is best result
			return nbest;
		}

		// From http://www.math.uic.edu/~jan/mcs471/Lec9/gss.pdf
		//private double[] GoldenSectionSearch(ArrayList pts1, ArrayList pts2, double a, double b, double threshold)
		private static double[] GoldenSectionSearch(List<PointR> pts1, List<PointR> pts2, double a, double b, double threshold)
		{
			var x1 = Phi * a + (1 - Phi) * b;
			//ArrayList newPoints = Utils.RotateByRadians(pts1, x1);
			var newPoints = Utils.RotateByRadians(pts1, x1);
			var fx1 = Utils.PathDistance(newPoints, pts2);

			var x2 = (1 - Phi) * a + Phi * b;
			newPoints = Utils.RotateByRadians(pts1, x2);
			var fx2 = Utils.PathDistance(newPoints, pts2);

			var i = 2.0; // calls
			while (Math.Abs(b - a) > threshold)
			{
				if (fx1 < fx2)
				{
					b = x2;
					x2 = x1;
					fx2 = fx1;
					x1 = Phi * a + (1 - Phi) * b;
					newPoints = Utils.RotateByRadians(pts1, x1);
					fx1 = Utils.PathDistance(newPoints, pts2);
				}
				else
				{
					a = x1;
					x1 = x2;
					fx1 = fx2;
					x2 = (1 - Phi) * a + Phi * b;
					newPoints = Utils.RotateByRadians(pts1, x2);
					fx2 = Utils.PathDistance(newPoints, pts2);
				}
				i++;
			}
			return new[] { Math.Min(fx1, fx2), Utils.Rad2Deg((b + a) / 2.0), i }; // distance, angle, calls to pathdist
		}

		// continues to rotate 'pts1' by 'step' degrees as long as points become ever-closer 
		// in path-distance to pts2. the initial distance is given by D. the best distance
		// is returned in array[0], while the angle at which it was achieved is in array[1].
		// array[3] contains the number of calls to PathDistance.
		//private double[] HillClimbSearch(ArrayList pts1, ArrayList pts2, double D, double step)
		private static double[] HillClimbSearch(List<PointR> pts1, List<PointR> pts2, double D, double step)
		{
			var i = 0.0;
			var theta = 0.0;
			var d = D;
			do
			{
				D = d; // the last angle tried was better still
				theta += step;
				//ArrayList newPoints = Utils.RotateByDegrees(pts1, theta);
				var newPoints = Utils.RotateByDegrees(pts1, theta);
				d = Utils.PathDistance(newPoints, pts2);
				i++;
			}
			while (d <= D);
			return new[] { D, theta - step, i }; // distance, angle, calls to pathdist
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pts1"></param>
		/// <param name="pts2"></param>
		/// <param name="writer"></param>
		/// <returns></returns>
		//private double[] FullSearch(ArrayList pts1, ArrayList pts2, StreamWriter writer)
		private static double[] FullSearch(List<PointR> pts1, List<PointR> pts2, TextWriter writer)
		{
			var bestA = 0d;
			var bestD = Utils.PathDistance(pts1, pts2);

			for (var i = -180; i <= +180; i++)
			{
				//ArrayList newPoints = Utils.RotateByDegrees(pts1, i);
				var newPoints = Utils.RotateByDegrees(pts1, i);
				var d = Utils.PathDistance(newPoints, pts2);
				if (writer != null)
				{
					writer.WriteLine("{0}\t{1:F3}", i, Math.Round(d, 3));
				}
				if (d >= bestD)
					continue;

				bestD = d;
				bestA = i;
			}
			if (writer != null)
				writer.WriteLine("\nFull Search (360 rotations)\n{0:F2}{1}\t{2:F3} px", Math.Round(bestA, 2), (char) 176, Math.Round(bestD, 3)); // calls, angle, distance
			return new[] { bestD, bestA, 360.0 }; // distance, angle, calls to pathdist
		}

		#endregion

		#region Gestures & Xml

		public int NumGestures
		{
			get
			{
				return _gestures.Count;
			}
		}

		//public ArrayList Gestures
		public List<Unistroke> Gestures
		{
			get
			{
				//ArrayList list = new ArrayList(_gestures.Values);
				var list = new List<Unistroke>(_gestures.Values);
				list.Sort();
				return list;
			}
		}

		public void ClearGestures()
		{
			_gestures.Clear();
		}

		//public bool SaveGesture(string filename, ArrayList points)
		//public bool SaveGesture(string filename, List<PointR> points)
		//{
		//    // add the new prototype with the name extracted from the filename.
		//    string name = Unistroke.ParseName(filename);
		//    if (_gestures.ContainsKey(name))
		//        _gestures.Remove(name);
		//    Unistroke newPrototype = new Unistroke(name, points);
		//    _gestures.Add(name, newPrototype);

		//    // figure out the duration of the gesture
		//    PointR p0 = (PointR) points[0];
		//    PointR pn = (PointR) points[points.Count - 1];

		//    // do the xml writing
		//    bool success = true;
		//    XmlTextWriter writer = null;
		//    try
		//    {
		//        // save the prototype as an Xml file
		//        writer = new XmlTextWriter(filename, Encoding.UTF8);
		//        writer.Formatting = Formatting.Indented;
		//        writer.WriteStartDocument(true);
		//        writer.WriteStartElement("Gesture");
		//        writer.WriteAttributeString("Name", name);
		//        writer.WriteAttributeString("NumPts", XmlConvert.ToString(points.Count));
		//        writer.WriteAttributeString("Millseconds", XmlConvert.ToString(pn.T - p0.T));
		//        writer.WriteAttributeString("AppName", Assembly.GetExecutingAssembly().GetName().Name);
		//        writer.WriteAttributeString("AppVer", Assembly.GetExecutingAssembly().GetName().Version.ToString());
		//        writer.WriteAttributeString("Date", DateTime.Now.ToLongDateString());
		//        writer.WriteAttributeString("TimeOfDay", DateTime.Now.ToLongTimeString());

		//        // write out the raw individual points
		//        foreach (PointR p in points)
		//        {
		//            writer.WriteStartElement("Point");
		//            writer.WriteAttributeString("X", XmlConvert.ToString(p.X));
		//            writer.WriteAttributeString("Y", XmlConvert.ToString(p.Y));
		//            writer.WriteAttributeString("T", XmlConvert.ToString(p.T));
		//            writer.WriteEndElement(); // <Point />
		//        }

		//        writer.WriteEndDocument(); // </Gesture>
		//    }
		//    catch (XmlException xex)
		//    {
		//        Console.Write(xex.Message);
		//        success = false;
		//    }
		//    catch (Exception ex)
		//    {
		//        Console.Write(ex.Message);
		//        success = false;
		//    }
		//    finally
		//    {
		//        if (writer != null)
		//            writer.Close();
		//    }
		//    return success; // Xml file successfully written (or not)
		//}

		public bool LoadGesture(string filename)
		{
			var success = true;
			//XmlTextReader reader = null;
			XmlReader reader = null;
			try
			{
				//reader = new XmlTextReader(filename);
				var settings = new XmlReaderSettings {XmlResolver = new XmlXapResolver()};
				reader = XmlReader.Create(filename);
				//reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();

				var p = ReadGesture(reader);

				// remove any with the same name and add the prototype gesture
				if (_gestures.ContainsKey(p.Name))
					_gestures.Remove(p.Name);
				_gestures.Add(p.Name, p);
			}
			catch (XmlException xex)
			{
				Console.Write(xex.Message);
				success = false;
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
				success = false;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
			return success;
		}

		// assumes the reader has been just moved to the head of the content.
		//private Unistroke ReadGesture(XmlTextReader reader)
		private static Unistroke ReadGesture(XmlReader reader)
		{
			Debug.Assert(reader.LocalName == "Gesture");
			var name = reader.GetAttribute("Name");

			//ArrayList points = new ArrayList(XmlConvert.ToInt32(reader.GetAttribute("NumPts")));
			var points = new List<PointR>(XmlConvert.ToInt32(reader.GetAttribute("NumPts")));

			//reader.Read(); // advance to the first Point
			//reader.ReadToFollowing("Point"); // advance to the first Point
			//Debug.Assert(reader.LocalName == "Point");


			while (reader.ReadToFollowing("Point"))
			{
				var p = PointR.Empty;
				p.X = XmlConvert.ToDouble(reader.GetAttribute("X"));
				p.Y = XmlConvert.ToDouble(reader.GetAttribute("Y"));
				p.T = XmlConvert.ToInt32(reader.GetAttribute("T"));
				points.Add(p);
				//reader.ReadStartElement("Point");
				//reader.ReadToFollowing("Point");
			}

			//while (reader.NodeType != XmlNodeType.EndElement)
			//{
			//    PointR p = PointR.Empty;
			//    p.X = XmlConvert.ToDouble(reader.GetAttribute("X"));
			//    p.Y = XmlConvert.ToDouble(reader.GetAttribute("Y"));
			//    p.T = XmlConvert.ToInt32(reader.GetAttribute("T"));
			//    points.Add(p);
			//    //reader.ReadStartElement("Point");
			//    reader.ReadToFollowing("Point");
			//}

			return new Unistroke(name, points);
		}

		#endregion

		#region Batch Processing

		/// <summary>
		/// Assemble the gesture filenames into categories that contain 
		/// potentially multiple examples of the same gesture.
		/// </summary>
		/// <param name="filenames"></param>
		/// <returns>A 1D arraylist of category instances that each
		/// contain the same number of examples, or <b>null</b> if an
		/// error occurs.</returns>
		/// <remarks>
		/// See the comments above MainForm.BatchProcess_Click.
		/// </remarks>
		//public ArrayList AssembleBatch(string[] filenames)
		public List<Category> AssembleBatch(string[] filenames)
		{
			//Hashtable categories = new Hashtable();
			var categories = new Dictionary<string, Category>();

			foreach (var filename in filenames)
			{
				//XmlTextReader reader = null;
				XmlReader reader = null;
				try
				{
					//reader = new XmlTextReader(filename);
					reader =  XmlReader.Create(filename);
					//reader.WhitespaceHandling = WhitespaceHandling.None;
					reader.MoveToContent();
					
					var p = ReadGesture(reader);
					var catName = Category.ParseName(p.Name);
					if (categories != null)
						if (categories.ContainsKey(catName))
						{
							var cat = categories[catName];
							cat.AddExample(p); // if the category has been made before, just add to it
						}
						else // create new category
						{
							categories.Add(catName, new Category(catName, p));
						}
				}
				catch (XmlException xex)
				{
					Console.Write(xex.Message);
					if (categories != null)
						categories.Clear();
					categories = null;
				}
				catch (Exception ex)
				{
					Console.Write(ex.Message);
					if (categories != null)
						categories.Clear();
					categories = null;
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}

			// now make sure that each category has the same number of elements in it
			//ArrayList list = null;
			List<Category> list = null;
			if (categories != null)
			{
				//list = new ArrayList(categories.Values);
				list = new List<Category>(categories.Values);
				var numExamples = list[0].NumExamples;
				if (list.Any(c => c.NumExamples != numExamples))
				{
					Console.WriteLine("Different number of examples in gesture categories.");
					list.Clear();
					list = null;
				}
			}
			return list;
		}

		/// <summary>
		/// Tests an entire batch of files. See comments atop MainForm.TestBatch_Click().
		/// </summary>
		/// <param name="subject">Subject identification.</param>
		/// <param name="speed">"fast", "medium", or "slow"</param>
		/// <param name="categories">A list of gesture categories that each contain lists of prototypes (examples) within that gesture category.</param>
		/// <param name="dir">The directory into which to write the output files.</param>
		/// <returns>The two filenames of the output file if successful; null otherwise. The main results are in string[0],
		/// while the detailed recognition results are in string[1].</returns>
		//public string[] TestBatch(string subject, string speed, ArrayList categories, string dir)
		public string[] TestBatch(string subject, string speed, List<Category> categories, string dir)
		{
			StreamWriter mainWriter = null;
			StreamWriter recWriter = null;
			var filenames = new string[2];
			try
			{
				//
				// set up a main results file and detailed recognition results file
				//
				var start = Environment.TickCount;
				filenames[0] = String.Format("{0}\\onedollar_main_{1}.txt", dir, start); // main results (small file)
				filenames[1] = String.Format("{0}\\onedollar_nbest_{1}.txt", dir, start); // recognition details (large file)

				mainWriter = new StreamWriter(filenames[0], false, Encoding.UTF8);
				mainWriter.WriteLine("Subject = {0}, Recognizer = onedollar, Speed = {1}, StartTime(ms) = {2}", subject, speed, start);
				mainWriter.WriteLine("Subject Recognizer Speed NumTraining GestureType RecognitionRate\n");

				recWriter = new StreamWriter(filenames[1], false, Encoding.UTF8);
				recWriter.WriteLine("Subject = {0}, Recognizer = onedollar, Speed = {1}, StartTime(ms) = {2}", subject, speed, start);
				recWriter.WriteLine("Correct? NumTrain Tested 1stCorrect Pts Ms Angle : (NBestNames) [NBestScores]\n");

				//
				// determine the number of gesture categories and the number of examples in each one
				//
				var numCategories = categories.Count;
				var numExamples = categories[0].NumExamples;
				double totalTests = (numExamples - 1) * NumRandomTests;

				//
				// outermost loop: trains on N=1..9, tests on 10-N (for e.g., numExamples = 10)
				//
				for (var n = 1; n <= numExamples - 1; n++)
				{
					// storage for the final avg results for each category for this N
					var results = new double[numCategories];

					//
					// run a number of tests at this particular N number of training examples
					//
					for (int r = 0; r < NumRandomTests; r++)
					{
						_gestures.Clear(); // clear any (old) loaded prototypes

						// load (train on) N randomly selected gestures in each category
						for (var i = 0; i < numCategories; i++)
						{
							var c = categories[i]; // the category to load N examples for
							var chosen = Utils.Random(0, numExamples - 1, n); // select N unique indices
							foreach (var t in chosen)
							{
								var p = c[t]; // get the prototype from this category at chosen[j]
								_gestures.Add(p.Name, p); // load the randomly selected test gestures into the recognizer
							}
						}

						//
						// testing loop on all unloaded gestures in each category. creates a recognition
						// rate (%) by averaging the binary outcomes (correct, incorrect) for each test.
						//
						for (int i = 0; i < numCategories; i++)
						{
							// pick a random unloaded gesture in this category for testing
							// instead of dumbly picking, first find out what indices aren't
							// loaded, and then randomly pick from those.
							var c = categories[i];
							var notLoaded = new int[numExamples - n];
							for (int j = 0, k = 0; j < numExamples; j++)
							{
								var g = c[j];
								if (!_gestures.ContainsKey(g.Name))
									notLoaded[k++] = j; // jth gesture in c is not loaded
							}
							var chosen = Utils.Random(0, notLoaded.Length - 1); // index
							var p = c[notLoaded[chosen]]; // gesture to test
							Debug.Assert(!_gestures.ContainsKey(p.Name));
							
							// do the recognition!
							//ArrayList testPts = Utils.RotateByDegrees(p.RawPoints, Utils.Random(0, 359));
							var testPts = Utils.RotateByDegrees(p.RawPoints, Utils.Random(0, 359));
							var result = Recognize(testPts);
							var category = Category.ParseName(result.Name);
							var correct = (c.Name == category) ? 1 : 0;

							recWriter.WriteLine("{0} {1} {2} {3} {4} {5} {6:F1}{7} : ({8}) [{9}]",
								correct,                            // Correct?
								n,                                  // NumTrain 
								p.Name,                             // Tested 
								FirstCorrect(p.Name, result.Names), // 1stCorrect
								p.RawPoints.Count,                  // Pts
								p.Duration,                         // Ms 
								Math.Round(result.Angle, 1), (char) 176, // Angle tweaking :
								result.NamesString,                 // (NBestNames)
								result.ScoresString);               // [NBestScores]

							results[i] += correct;
						}

						// provide feedback as to how many tests have been performed thus far.
						double testsSoFar = ((n - 1) * NumRandomTests) + r;
						ProgressChangedEvent(this, new ProgressEventArgs(testsSoFar / totalTests)); // callback
					}

					//
					// now create the final results for this N and write them to a file
					//
					for (int i = 0; i < numCategories; i++)
					{
						results[i] /= NumRandomTests; // normalize by the number of tests at this N
						var c = categories[i];
						// Subject Recognizer Speed NumTraining GestureType RecognitionRate
						mainWriter.WriteLine("{0} onedollar {1} {2} {3} {4:F3}", subject, speed, n, c.Name, Math.Round(results[i], 3));
					}
				}

				// time-stamp the end of the processing
				int end = Environment.TickCount;
				mainWriter.WriteLine("\nEndTime(ms) = {0}, Minutes = {1:F2}", end, Math.Round((end - start) / 60000.0, 2));
				recWriter.WriteLine("\nEndTime(ms) = {0}, Minutes = {1:F2}", end, Math.Round((end - start) / 60000.0, 2));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				filenames = null;
			}
			finally
			{
				if (mainWriter != null)
					mainWriter.Close();
				if (recWriter != null)
					recWriter.Close();
			}
			return filenames;
		}

		private static int FirstCorrect(string name, IList<string> names)
		{
			var category = Category.ParseName(name);
			for (var i = 0; i < names.Count; i++)
			{
				var c = Category.ParseName(names[i]);
				if (category == c)
				{
					return i + 1;
				}
			}
			return -1;
		}

		#endregion

		#region Rotation Graph

		public bool CreateRotationGraph(string file1, string file2, string dir, bool similar)
		{
			var success = true;
			StreamWriter writer = null;
			//XmlTextReader reader = null;
			XmlReader reader = null;
			try
			{
				// read gesture file #1
				//reader = new XmlTextReader(file1);
				reader = XmlReader.Create(file1);
				//reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();
				var g1 = ReadGesture(reader);
				reader.Close();

				// read gesture file #2
				//reader = new XmlTextReader(file2);
				reader = XmlReader.Create(file2);
				//reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.MoveToContent();
				var g2 = ReadGesture(reader);

				// create output file for results
				var outfile = String.Format("{0}\\{1}({2}, {3})_{4}.txt", dir, similar ? "o" : "x", g1.Name, g2.Name, Environment.TickCount);
				writer = new StreamWriter(outfile, false, Encoding.UTF8);
				writer.WriteLine("Rotated: {0} --> {1}. {2}, {3}\n", g1.Name, g2.Name, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());

				// do the full 360 degree rotations
				var full = FullSearch(g1.Points, g2.Points, writer);

				// use bidirectional hill climbing to do it again
				var init = Utils.PathDistance(g1.Points, g2.Points); // initial distance
				var pos = HillClimbSearch(g1.Points, g2.Points, init, 1d);
				var neg = HillClimbSearch(g1.Points, g2.Points, init, -1d);
				var best = (neg[0] < pos[0]) ? neg : pos;
				writer.WriteLine("\nHill Climb Search ({0} rotations)\n{1:F2}{2}\t{3:F3} px", pos[2] + neg[2] + 1, Math.Round(best[1], 2), (char) 176, Math.Round(best[0], 3)); // calls, angle, distance

				// use golden section search to do it yet again
				var gold = GoldenSectionSearch(
					g1.Points,              // to rotate
					g2.Points,              // to match
					Utils.Deg2Rad(-45.0),   // lbound
					Utils.Deg2Rad(+45.0),   // ubound
					Utils.Deg2Rad(2.0));    // threshold
				writer.WriteLine("\nGolden Section Search ({0} rotations)\n{1:F2}{2}\t{3:F3} px", gold[2], Math.Round(gold[1], 2), (char) 176, Math.Round(gold[0], 3)); // calls, angle, distance

				// for pasting into Excel
				writer.WriteLine("\n{0} {1} {2:F2} {3:F2} {4:F3} {5:F3} {6} {7:F2} {8:F2} {9:F3} {10} {11:F2} {12:F2} {13:F3} {14}",
					g1.Name,                    // rotated
					g2.Name,                    // into
					Math.Abs(Math.Round(full[1], 2)), // |angle|
					Math.Round(full[1], 2),     // Full Search angle
					Math.Round(full[0], 3),     // Full Search distance
					Math.Round(init, 3),        // Initial distance w/o any search
					full[2],                    // Full Search iterations
					Math.Abs(Math.Round(best[1], 2)), // |angle|
					Math.Round(best[1], 2),     // Bidirectional Hill Climb Search angle
					Math.Round(best[0], 3),     // Bidirectional Hill Climb Search distance
					pos[2] + neg[2] + 1,        // Bidirectional Hill Climb Search iterations
					Math.Abs(Math.Round(gold[1], 2)), // |angle|
					Math.Round(gold[1], 2),     // Golden Section Search angle
					Math.Round(gold[0], 3),     // Golden Section Search distance
					gold[2]);                   // Golden Section Search iterations
			}
			catch (XmlException xml)
			{
				Console.Write(xml.Message);
				success = false;
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
				success = false;
			}
			finally
			{
				if (reader != null)
					reader.Close();
				if (writer != null)
					writer.Close();
			}
			return success;
		}

		#endregion
	}
}