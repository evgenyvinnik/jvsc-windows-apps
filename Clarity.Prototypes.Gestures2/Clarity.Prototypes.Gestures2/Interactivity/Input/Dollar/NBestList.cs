using System;
using System.Collections.Generic;
using System.Linq;

namespace Clarity.Phone.Interactivity.Input.Dollar
{
	public class NBestList
	{
		#region NBestResult Inner Class

		public class NBestResult : IComparable
		{
			public static NBestResult Empty = new NBestResult(String.Empty, -1d, -1d, 0d);

			private readonly string _name;
			private readonly double _score;
			private readonly double _distance;
			private readonly double _angle;

			// constructor
			public NBestResult(string name, double score, double distance, double angle)
			{
				_name = name;
				_score = score;
				_distance = distance;
				_angle = angle;
			}

			public string Name { get { return _name; } }
			public double Score { get { return _score; } }
			public double Distance { get { return _distance; } }
			public double Angle { get { return _angle; } }
			public bool IsEmpty { get { return _score == -1d; } }

			// sorts in descending order of Score
			public int CompareTo(object obj)
			{
				if (!(obj is NBestResult))
					throw new ArgumentException("object is not a Result");
				var r = (NBestResult) obj;

				if (_score < r._score)
					return 1;

				if (_score > r._score)
					return -1;
				return 0;
			}
		}

		#endregion

		#region Fields

		public static NBestList Empty = new NBestList();
		//private ArrayList _nBestList;
		private readonly List<NBestResult> _nBestList;

		#endregion

		#region Constructor & Methods

		public NBestList()
		{
			//_nBestList = new ArrayList();
			_nBestList = new List<NBestResult>();
		}

		public bool IsEmpty
		{
			get
			{
				return _nBestList.Count == 0;
			}
		}

		public void AddResult(string name, double score, double distance, double angle)
		{
			var r = new NBestResult(name, score, distance, angle);
			_nBestList.Add(r);
		}

		public void SortDescending()
		{
			_nBestList.Sort();
		}

		#endregion

		#region Top Result

		/// <summary>
		/// Gets the gesture name of the top result of the NBestList.
		/// </summary>
		public string Name
		{
			get
			{
				if (_nBestList.Count > 0)
				{
					var r = _nBestList[0];
					return r.Name;
				}
				return String.Empty;
			}
		}

		/// <summary>
		/// Gets the [0..1] matching score of the top result of the NBestList.
		/// </summary>
		public double Score
		{
			get
			{
				if (_nBestList.Count > 0)
				{
					var r = _nBestList[0];
					return r.Score;
				}
				return -1.0;
			}
		}

		/// <summary>
		/// Gets the average pixel distance of the top result of the NBestList.
		/// </summary>
		public double Distance
		{
			get
			{
				if (_nBestList.Count > 0)
				{
					var r = _nBestList[0];
					return r.Distance;
				}
				return -1.0;
			}
		}

		/// <summary>
		/// Gets the average pixel distance of the top result of the NBestList.
		/// </summary>
		public double Angle
		{
			get
			{
				if (_nBestList.Count > 0)
				{
					var r = _nBestList[0];
					return r.Angle;
				}
				return 0.0;
			}
		}

		#endregion

		#region All Results

		public NBestResult this[int index]
		{
			get
			{
				if (0 <= index && index < _nBestList.Count)
				{
					return _nBestList[index];
				}
				return null;
			}
		}

		public string[] Names
		{
			get
			{
				var s = new string[_nBestList.Count];
				if (_nBestList.Count > 0)
				{
					for (var i = 0; i < s.Length; i++)
					{
						s[i] = _nBestList[i].Name;
					}
				}
				return s;
			}
		}

		public string NamesString
		{
			get
			{
				var s = String.Empty;
				if (_nBestList.Count > 0)
				{
					s = _nBestList.Aggregate(s, (current, r) => current + String.Format("{0},", r.Name));
				}
				return s.TrimEnd(new[] { ',' });
			}
		}

		public double[] Scores
		{
			get
			{
				var s = new double[_nBestList.Count];
				if (_nBestList.Count > 0)
				{
					for (var i = 0; i < s.Length; i++)
					{
						s[i] = _nBestList[i].Score;
					}
				}
				return s;
			}
		}

		public string ScoresString
		{
			get
			{
				var s = String.Empty;
				if (_nBestList.Count > 0)
				{
					s = _nBestList.Aggregate(s, (current, r) => current + String.Format("{0:F3},", Math.Round(r.Score, 3)));
				}
				return s.TrimEnd(new[] { ',' });
			}
		}

		#endregion

	}
}
