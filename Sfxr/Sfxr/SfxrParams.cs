using System;

namespace Sfxr
{
	public class SfxrParams
	{
		public bool ParamsDirty;

		private uint _waveType;					// Shape of the wave (0:square, 1:saw, 2:sin or 3:noise)

		private double _masterVolume = 1.0;		// Overall volume of the sound (0 to 1)

		private double _attackTime;		// Length of the volume envelope attack (0 to 1)
		private double _sustainTime;	// Length of the volume envelope sustain (0 to 1)
		private double _sustainPunch;	// Tilts the sustain envelope for more 'pop' (0 to 1)
		private double _decayTime;		// Length of the volume envelope decay (yes, I know it's called release) (0 to 1)
		
		private double _startFrequency;	// Base note of the sound (0 to 1)
		private double _minFrequency;	// If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1)
		
		private double _slide;			// Slides the note up or down (-1 to 1)
		private double _deltaSlide;		// Accelerates the slide (-1 to 1)
		
		private double _vibratoDepth;	// Strength of the vibrato effect (0 to 1)
		private double _vibratoSpeed;	// Speed of the vibrato effect (i.e. frequency) (0 to 1)
		
		private double _changeAmount;	// Shift in note, either up or down (-1 to 1)
		private double _changeSpeed;	// How fast the note shift happens (only happens once) (0 to 1)
		
		private double _squareDuty;		// Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1)
		private double _dutySweep;		// Sweeps the duty up or down (-1 to 1)
		
		private double _repeatSpeed;	// Speed of the note repeating - certain variables are reset each time (0 to 1)
		
		private double _phaserOffset;	// Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1)
		private double _phaserSweep;	// Sweeps the phase up or down (-1 to 1)
		
		private double _lpFilterCutoff;			// Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1)
		private double _lpFilterCutoffSweep;	// Sweeps the low-pass cutoff up or down (-1 to 1)
		private double _lpFilterResonance;		// Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1)
		
		private double _hpFilterCutoff;			// Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1)
		private double _hpFilterCutoffSweep;	// Sweeps the high-pass cutoff up or down (-1 to 1)
		
		//--------------------------------------------------------------------------
		//
		//  Getters / Setters
		//
		//--------------------------------------------------------------------------


		/** Shape of the wave (0:square, 1:saw, 2:sin or 3:noise) */
		public uint WaveType
		{
			get { return _waveType; }
			set { _waveType = value > 3 ? 0 : value; ParamsDirty = true; }
		}

		/** Overall volume of the sound (0 to 1) */
		public double MasterVolume
		{
			get { return _masterVolume; }
			set {_masterVolume = Clamp1(value); ParamsDirty = true;}
		}

		/** Length of the volume envelope attack (0 to 1) */
		public double AttackTime
		{
			get { return _attackTime; }
			set {_attackTime = Clamp1(value); ParamsDirty = true;}
		}

		/** Length of the volume envelope sustain (0 to 1) */
		public double SustainTime
		{
			get { return _sustainTime; }
			set { _sustainTime = Clamp1(value); ParamsDirty = true; }
		}

		/** Tilts the sustain envelope for more 'pop' (0 to 1) */
		public double SustainPunch
		{
			get { return _sustainPunch; }
			set { _sustainPunch = Clamp1(value); ParamsDirty = true; }
		}

		
		/** Length of the volume envelope decay (yes, I know it's called release) (0 to 1) */
		public double DecayTime
		{
			get { return _decayTime; }
			set { _decayTime = Clamp1(value); ParamsDirty = true; }
		}

		/** Base note of the sound (0 to 1) */
		public double StartFrequency
		{
			get { return _startFrequency; }
			set { _startFrequency = Clamp1(value); ParamsDirty = true; }
		}

		/** If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1) */
		public double MinFrequency
		{
			get { return _minFrequency; }
			set { _minFrequency = Clamp1(value); ParamsDirty = true; }
		}

		/** Slides the note up or down (-1 to 1) */
		public double Slide
		{
			get { return _slide; }
			set { _slide = Clamp2(value); ParamsDirty = true; }
		}

		/** Accelerates the slide (-1 to 1) */
		public double DeltaSlide
		{
			get { return _deltaSlide; }
			set { _deltaSlide = Clamp2(value); ParamsDirty = true; }
		}

		/** Strength of the vibrato effect (0 to 1) */
		public double VibratoDepth
		{
			get { return _vibratoDepth; }
			set { _vibratoDepth = Clamp1(value); ParamsDirty = true; }
		}

		/** Speed of the vibrato effect (i.e. frequency) (0 to 1) */
		public double VibratoSpeed
		{
			get { return _vibratoSpeed; }
			set { _vibratoSpeed = Clamp1(value); ParamsDirty = true; }
		}

		/** Shift in note, either up or down (-1 to 1) */
		public double ChangeAmount
		{
			get { return _changeAmount; }
			set { _changeAmount = Clamp2(value); ParamsDirty = true; }
		}

		/** How fast the note shift happens (only happens once) (0 to 1) */
		public double ChangeSpeed
		{
			get { return _changeSpeed; }
			set { _changeSpeed = Clamp1(value); ParamsDirty = true; }
		}

		/** Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1) */
		public double SquareDuty
		{
			get { return _squareDuty; }
			set { _squareDuty = Clamp1(value); ParamsDirty = true; }
		}

		/** Sweeps the duty up or down (-1 to 1) */
		public double DutySweep
		{
			get { return _dutySweep; }
			set { _dutySweep = Clamp2(value); ParamsDirty = true; }
		}

		/** Speed of the note repeating - certain variables are reset each time (0 to 1) */
		public double RepeatSpeed
		{
			get { return _repeatSpeed; }
			set { _repeatSpeed = Clamp1(value); ParamsDirty = true; }
		}

		/** Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1) */
		public double PhaserOffset
		{
			get { return _phaserOffset; }
			set { _phaserOffset = Clamp2(value); ParamsDirty = true; }
		}

		/** Sweeps the phase up or down (-1 to 1) */
		public double PhaserSweep
		{
			get { return _phaserSweep; }
			set { _phaserSweep = Clamp2(value); ParamsDirty = true; }
		}

		/** Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1) */
		public double LpFilterCutoff
		{
			get { return _lpFilterCutoff; }
			set { _lpFilterCutoff = Clamp1(value); ParamsDirty = true; }
		}

		/** Sweeps the low-pass cutoff up or down (-1 to 1) */
		public double LpFilterCutoffSweep
		{
			get { return _lpFilterCutoffSweep; }
			set { _lpFilterCutoffSweep = Clamp2(value); ParamsDirty = true; }
		}

		/** Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1) */
		public double LpFilterResonance
		{
			get { return _lpFilterResonance; }
			set { _lpFilterResonance = Clamp1(value); ParamsDirty = true; }
		}

		/** Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1) */
		public double HpFilterCutoff
		{
			get { return _hpFilterCutoff; }
			set { _hpFilterCutoff = Clamp1(value); ParamsDirty = true; }
		}

		/** Sweeps the high-pass cutoff up or down (-1 to 1) */
		public double HpFilterCutoffSweep
		{
			get { return _hpFilterCutoffSweep; }
			set { _hpFilterCutoffSweep = Clamp2(value); ParamsDirty = true; }
		}


		private readonly Random _random;
		//--------------------------------------------------------------------------
		//
		//  Generator Methods
		//
		//--------------------------------------------------------------------------
	
		public SfxrParams()
		{
			_random = new Random();
		}

		/**
		 * Sets the parameters to generate a pickup/coin sound
		 */
		public void GeneratePickupCoin()
		{
			ResetParams();
			
			_startFrequency = 0.4 + _random.NextDouble() * 0.5;
			
			_sustainTime = _random.NextDouble() * 0.1;
			_decayTime = 0.1 + _random.NextDouble() * 0.4;
			_sustainPunch = 0.3 + _random.NextDouble() * 0.3;

			if (_random.NextDouble() >= 0.5)
				return;

			_changeSpeed = 0.5 + _random.NextDouble() * 0.2;
			_changeAmount = 0.2 + _random.NextDouble() * 0.4;
		}
		
		/**
		 * Sets the parameters to generate a laser/shoot sound
		 */
		public void GenerateLaserShoot()
		{
			ResetParams();
			
			_waveType = (uint)(_random.NextDouble() * 3);
			if(_waveType == 2 && _random.NextDouble() < 0.5) _waveType = (uint)(_random.NextDouble() * 2);
			
			_startFrequency = 0.5 + _random.NextDouble() * 0.5;
			_minFrequency = _startFrequency - 0.2 - _random.NextDouble() * 0.6;
			if(_minFrequency < 0.2) _minFrequency = 0.2;
			
			_slide = -0.15 - _random.NextDouble() * 0.2;
			
			if(_random.NextDouble() < 0.33)
			{
				_startFrequency = 0.3 + _random.NextDouble() * 0.6;
				_minFrequency = _random.NextDouble() * 0.1;
				_slide = -0.35 - _random.NextDouble() * 0.3;
			}
			
			if(_random.NextDouble() < 0.5) 
			{
				_squareDuty = _random.NextDouble() * 0.5;
				_dutySweep = _random.NextDouble() * 0.2;
			}
			else
			{
				_squareDuty = 0.4 + _random.NextDouble() * 0.5;
				_dutySweep =- _random.NextDouble() * 0.7;	
			}
			
			_sustainTime = 0.1 + _random.NextDouble() * 0.2;
			_decayTime = _random.NextDouble() * 0.4;
			if(_random.NextDouble() < 0.5) _sustainPunch = _random.NextDouble() * 0.3;
			
			if(_random.NextDouble() < 0.33)
			{
				_phaserOffset = _random.NextDouble() * 0.2;
				_phaserSweep = -_random.NextDouble() * 0.2;
			}
			
			if(_random.NextDouble() < 0.5) _hpFilterCutoff = _random.NextDouble() * 0.3;
		}
		
		/**
		 * Sets the parameters to generate an explosion sound
		 */
		public void GenerateExplosion()
		{
			ResetParams();
			_waveType = 3;
			
			if(_random.NextDouble() < 0.5)
			{
				_startFrequency = 0.1 + _random.NextDouble() * 0.4;
				_slide = -0.1 + _random.NextDouble() * 0.4;
			}
			else
			{
				_startFrequency = 0.2 + _random.NextDouble() * 0.7;
				_slide = -0.2 - _random.NextDouble() * 0.2;
			}
			
			_startFrequency *= _startFrequency;
			
			if(_random.NextDouble() < 0.2) _slide = 0.0;
			if(_random.NextDouble() < 0.33) _repeatSpeed = 0.3 + _random.NextDouble() * 0.5;
			
			_sustainTime = 0.1 + _random.NextDouble() * 0.3;
			_decayTime = _random.NextDouble() * 0.5;
			_sustainPunch = 0.2 + _random.NextDouble() * 0.6;
			
			if(_random.NextDouble() < 0.5)
			{
				_phaserOffset = -0.3 + _random.NextDouble() * 0.9;
				_phaserSweep = -_random.NextDouble() * 0.3;
			}
			
			if(_random.NextDouble() < 0.33)
			{
				_changeSpeed = 0.6 + _random.NextDouble() * 0.3;
				_changeAmount = 0.8 - _random.NextDouble() * 1.6;
			}
		}
		
		/**
		 * Sets the parameters to generate a powerup sound
		 */
		public void GeneratePowerup()
		{
			ResetParams();
			
			if(_random.NextDouble() < 0.5) _waveType = 1;
			else 					_squareDuty = _random.NextDouble() * 0.6;
			
			if(_random.NextDouble() < 0.5)
			{
				_startFrequency = 0.2 + _random.NextDouble() * 0.3;
				_slide = 0.1 + _random.NextDouble() * 0.4;
				_repeatSpeed = 0.4 + _random.NextDouble() * 0.4;
			}
			else
			{
				_startFrequency = 0.2 + _random.NextDouble() * 0.3;
				_slide = 0.05 + _random.NextDouble() * 0.2;
				
				if(_random.NextDouble() < 0.5)
				{
					_vibratoDepth = _random.NextDouble() * 0.7;
					_vibratoSpeed = _random.NextDouble() * 0.6;
				}
			}
			
			_sustainTime = _random.NextDouble() * 0.4;
			_decayTime = 0.1 + _random.NextDouble() * 0.4;
		}
		
		/**
		 * Sets the parameters to generate a hit/hurt sound
		 */
		public void GenerateHitHurt()
		{
			ResetParams();
			
			_waveType = (uint)(_random.NextDouble() * 3);
			if(_waveType == 2) _waveType = 3;
			else if(_waveType == 0) _squareDuty = _random.NextDouble() * 0.6;
			
			_startFrequency = 0.2 + _random.NextDouble() * 0.6;
			_slide = -0.3 - _random.NextDouble() * 0.4;
			
			_sustainTime = _random.NextDouble() * 0.1;
			_decayTime = 0.1 + _random.NextDouble() * 0.2;
			
			if(_random.NextDouble() < 0.5) _hpFilterCutoff = _random.NextDouble() * 0.3;
		}
		
		/**
		 * Sets the parameters to generate a jump sound
		 */
		public void GenerateJump()
		{
			ResetParams();
			
			_waveType = 0;
			_squareDuty = _random.NextDouble() * 0.6;
			_startFrequency = 0.3 + _random.NextDouble() * 0.3;
			_slide = 0.1 + _random.NextDouble() * 0.2;
			
			_sustainTime = 0.1 + _random.NextDouble() * 0.3;
			_decayTime = 0.1 + _random.NextDouble() * 0.2;
			
			if(_random.NextDouble() < 0.5) _hpFilterCutoff = _random.NextDouble() * 0.3;
			if(_random.NextDouble() < 0.5) _lpFilterCutoff = 1.0 - _random.NextDouble() * 0.6;
		}
		
		/**
		 * Sets the parameters to generate a blip/select sound
		 */
		public void GenerateBlipSelect()
		{
			ResetParams();
			
			_waveType = (uint)(_random.NextDouble() * 2);
			if(_waveType == 0) _squareDuty = _random.NextDouble() * 0.6;
			
			_startFrequency = 0.2 + _random.NextDouble() * 0.4;
			
			_sustainTime = 0.1 + _random.NextDouble() * 0.1;
			_decayTime = _random.NextDouble() * 0.2;
			_hpFilterCutoff = 0.1;
		}
		
		/**
		 * Resets the parameters, used at the start of each generate function
		 */
		protected void ResetParams()
		{
			ParamsDirty = true;
			
			_waveType = 0;
			_startFrequency = 0.3;
			_minFrequency = 0.0;
			_slide = 0.0;
			_deltaSlide = 0.0;
			_squareDuty = 0.0;
			_dutySweep = 0.0;
			
			_vibratoDepth = 0.0;
			_vibratoSpeed = 0.0;
			
			_attackTime = 0.0;
			_sustainTime = 0.3;
			_decayTime = 0.4;
			_sustainPunch = 0.0;
			
			_lpFilterResonance = 0.0;
			_lpFilterCutoff = 1.0;
			_lpFilterCutoffSweep = 0.0;
			_hpFilterCutoff = 0.0;
			_hpFilterCutoffSweep = 0.0;
			
			_phaserOffset = 0.0;
			_phaserSweep = 0.0;
			
			_repeatSpeed = 0.0;
			
			_changeSpeed = 0.0;
			_changeAmount = 0.0;
		}
		
		//--------------------------------------------------------------------------
		//
		//  Randomize Methods
		//
		//--------------------------------------------------------------------------
		
		/**
		 * Randomly adjusts the parameters ever so slightly
		 */
		public void Mutate(double mutation = 0.05)
		{
			if (_random.NextDouble() < 0.5) StartFrequency += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) MinFrequency += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) Slide += 				_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) DeltaSlide += 			_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) SquareDuty += 			_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) DutySweep += 			_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) VibratoDepth += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) VibratoSpeed += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) AttackTime += 			_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) SustainTime += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) DecayTime += 			_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) SustainPunch += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) LpFilterCutoff += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) LpFilterCutoffSweep += _random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) LpFilterResonance += 	_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) HpFilterCutoff += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) HpFilterCutoffSweep += _random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) PhaserOffset += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) PhaserSweep += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) RepeatSpeed += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) ChangeSpeed += 		_random.NextDouble() * mutation*2 - mutation;
			if (_random.NextDouble() < 0.5) ChangeAmount += 		_random.NextDouble() * mutation*2 - mutation;
		}
		
		/**
		 * Sets all parameters to random values
		 */
		public void Randomize()
		{
			ParamsDirty = true;
			
			_waveType = (uint)(_random.NextDouble() * 4);
			
			_attackTime =  		Pow(_random.NextDouble()*2-1, 4);
			_sustainTime =  	Pow(_random.NextDouble()*2-1, 2);
			_sustainPunch =  	Pow(_random.NextDouble()*0.8, 2);
			_decayTime =  		_random.NextDouble();

			_startFrequency =  	(_random.NextDouble() < 0.5) ? Pow(_random.NextDouble()*2-1, 2) : (Pow(_random.NextDouble() * 0.5, 3) + 0.5);
			_minFrequency =  	0.0;
			
			_slide =  			Pow(_random.NextDouble()*2-1, 5);
			_deltaSlide =  		Pow(_random.NextDouble()*2-1, 3);
			
			_vibratoDepth =  	Pow(_random.NextDouble()*2-1, 3);
			_vibratoSpeed =  	_random.NextDouble()*2-1;
			
			_changeAmount =  	_random.NextDouble()*2-1;
			_changeSpeed =  	_random.NextDouble()*2-1;
			
			_squareDuty =  		_random.NextDouble()*2-1;
			_dutySweep =  		Pow(_random.NextDouble()*2-1, 3);
			
			_repeatSpeed =  	_random.NextDouble()*2-1;
			
			_phaserOffset =  	Pow(_random.NextDouble()*2-1, 3);
			_phaserSweep =  	Pow(_random.NextDouble()*2-1, 3);
			
			_lpFilterCutoff =  		1 - Pow(_random.NextDouble(), 3);
			_lpFilterCutoffSweep = 	Pow(_random.NextDouble()*2-1, 3);
			_lpFilterResonance =  	_random.NextDouble()*2-1;
			
			_hpFilterCutoff =  		Pow(_random.NextDouble(), 5);
			_hpFilterCutoffSweep = 	Pow(_random.NextDouble()*2-1, 5);
			
			if(_attackTime + _sustainTime + _decayTime < 0.2)
			{
				_sustainTime = 0.2 + _random.NextDouble() * 0.3;
				_decayTime = 0.2 + _random.NextDouble() * 0.3;
			}
			
			if((_startFrequency > 0.7 && _slide > 0.2) || (_startFrequency < 0.2 && _slide < -0.05)) 
			{
				_slide = -_slide;
			}
			
			if(_lpFilterCutoff < 0.1 && _lpFilterCutoffSweep < -0.05) 
			{
				_lpFilterCutoffSweep = -_lpFilterCutoffSweep;
			}
		}
		
		//--------------------------------------------------------------------------
		//	
		//  Settings String Methods
		//
		//--------------------------------------------------------------------------
		
		/**
		 * Returns a string representation of the parameters for copy/paste sharing
		 * @return	A comma-delimited list of parameter values
		 */
		public string GetSettingsString()
		{
			var strings = _waveType.ToString();
			strings += "," + To4Dp(_attackTime) + 			"," + To4Dp(_sustainTime) 
					+ "," + To4Dp(_sustainPunch) + 			"," + To4Dp(_decayTime) 
					+ "," + To4Dp(_startFrequency) + 		"," + To4Dp(_minFrequency)
					+ "," + To4Dp(_slide) + 				"," + To4Dp(_deltaSlide)
					+ "," + To4Dp(_vibratoDepth) + 			"," + To4Dp(_vibratoSpeed)
					+ "," + To4Dp(_changeAmount) + 			"," + To4Dp(_changeSpeed)
					+ "," + To4Dp(_squareDuty) + 			"," + To4Dp(_dutySweep)
					+ "," + To4Dp(_repeatSpeed) + 			"," + To4Dp(_phaserOffset)
					+ "," + To4Dp(_phaserSweep) + 			"," + To4Dp(_lpFilterCutoff)
					+ "," + To4Dp(_lpFilterCutoffSweep) + 	"," + To4Dp(_lpFilterResonance)
					+ "," + To4Dp(_hpFilterCutoff)+ 		"," + To4Dp(_hpFilterCutoffSweep)
					+ "," + To4Dp(_masterVolume);		
			
			return strings;
		}
		
		/**
		 * Parses a settings string into the parameters
		 * @param	string	Settings string to parse
		 * @return			If the string successfully parsed
		 */
		public bool SetSettingsString(string strings)
		{
			var values = strings.Split(',');
			
			if (values.Length != 24) return false;
			
			WaveType = 				uint.Parse(values[0]) ;
			AttackTime =  			double.Parse(values[1]);
			SustainTime =  			double.Parse(values[2]);
			SustainPunch =			double.Parse(values[3]);
			DecayTime =				double.Parse(values[4]);
			StartFrequency =		double.Parse(values[5]);
			MinFrequency =  		double.Parse(values[6]);
			Slide =  				double.Parse(values[7]);
			DeltaSlide =  			double.Parse(values[8]);
			VibratoDepth =  		double.Parse(values[9]);
			VibratoSpeed =  		double.Parse(values[10]);
			ChangeAmount =  		double.Parse(values[11]);
			ChangeSpeed =  			double.Parse(values[12]);
			SquareDuty =  			double.Parse(values[13]);
			DutySweep =  			double.Parse(values[14]);
			RepeatSpeed =  			double.Parse(values[15]);
			PhaserOffset =  		double.Parse(values[16]);
			PhaserSweep =  			double.Parse(values[17]);
			LpFilterCutoff =  		double.Parse(values[18]);
			LpFilterCutoffSweep =  	double.Parse(values[19]);
			LpFilterResonance =  	double.Parse(values[20]);
			HpFilterCutoff =  		double.Parse(values[21]);
			HpFilterCutoffSweep =  	double.Parse(values[22]);
			MasterVolume =			double.Parse(values[23]);
			
			return true;
		}
		
		
		//--------------------------------------------------------------------------
		//	
		//  Copying Methods
		//
		//--------------------------------------------------------------------------
		
		/**
		 * Returns a copy of this SfxrParams with all settings duplicated
		 * @return	A copy of this SfxrParams
		 */
		public SfxrParams Clone()
		{
			var res = new SfxrParams();
			res.CopyFrom(this);
			
			return res;
		}
		
		/**
		 * Copies parameters from another instance
		 * @param	params	Instance to copy parameters from
		 */
		public void CopyFrom(SfxrParams param, bool makeDirty = false)
		{
			_waveType = 			param.WaveType;
			_attackTime =           param.AttackTime;
			_sustainTime =          param.SustainTime;
			_sustainPunch =         param.SustainPunch;
			_decayTime =			param.DecayTime;
			_startFrequency = 		param.StartFrequency;
			_minFrequency = 		param.MinFrequency;
			_slide = 				param.Slide;
			_deltaSlide = 			param.DeltaSlide;
			_vibratoDepth = 		param.VibratoDepth;
			_vibratoSpeed = 		param.VibratoSpeed;
			_changeAmount = 		param.ChangeAmount;
			_changeSpeed = 			param.ChangeSpeed;
			_squareDuty = 			param.SquareDuty;
			_dutySweep = 			param.DutySweep;
			_repeatSpeed = 			param.RepeatSpeed;
			_phaserOffset = 		param.PhaserOffset;
			_phaserSweep = 			param.PhaserSweep;
			_lpFilterCutoff = 		param.LpFilterCutoff;
			_lpFilterCutoffSweep = 	param.LpFilterCutoffSweep;
			_lpFilterResonance = 	param.LpFilterResonance;
			_hpFilterCutoff = 		param.HpFilterCutoff;
			_hpFilterCutoffSweep = 	param.HpFilterCutoffSweep;
			_masterVolume = 		param.MasterVolume;
			
			if (makeDirty) ParamsDirty = true;
		}
		
		
		//--------------------------------------------------------------------------
		//
		//  Util Methods
		//
		//--------------------------------------------------------------------------
		
		/**
		 * Clams a value to betwen 0 and 1
		 * @param	value	Input value
		 * @return			The value clamped between 0 and 1
		 */
		private static double Clamp1(double value) { return (value > 1.0) ? 1.0 : ((value < 0.0) ? 0.0 : value); }
		
		/**
		 * Clams a value to betwen -1 and 1
		 * @param	value	Input value
		 * @return			The value clamped between -1 and 1
		 */
		private static double Clamp2(double value) { return (value > 1.0) ? 1.0 : ((value < -1.0) ? -1.0 : value); }
		
		/**
		 * Quick power function
		 * @param	powbase		Base to raise to power
		 * @param	power		Power to raise base by
		 * @return				The calculated power
		 */
		private static double Pow( double powbase, int power)
		{
			switch(power)
			{
				case 2: return powbase*powbase;
				case 3: return powbase*powbase*powbase;
				case 4: return powbase*powbase*powbase*powbase;
				case 5: return powbase*powbase*powbase*powbase*powbase;
			}
			
			return 1.0;
		}
		
		
		/**
		 * Returns the number as a string to 4 decimal places
		 * @param	value	double to convert
		 * @return			double to 4dp as a string
		 */
		private static string To4Dp(double value)
		{
			if (value < 0.0001 && value > -0.0001) return "";
			
			var strings = value.ToString();
			var split = strings.Split('.');
			if (split.Length == 1)
			{
				return strings;
			}

			var res = split[0] + "." + split[1].Substring(0, 4);
			while (res.Substring(res.Length - 1, 1) == "0") res = res.Substring(0, res.Length - 1);
				
			return res;
		}
	}
}
