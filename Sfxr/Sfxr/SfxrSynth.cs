using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Sfxr
{
	public class SfxrSynth
	{
		//--------------------------------------------------------------------------
		//
		//  Sound Parameters
		//
		//--------------------------------------------------------------------------

		private SfxrParams _params;							// Params instance

		//--------------------------------------------------------------------------
		//
		//  Synth Variables
		//
		//--------------------------------------------------------------------------
		//For PCM
		const int Subchunk1Size = 16;
		//For no compression 
		const int AudioFormat = 1; 



		private bool _finished;						// If the sound has finished

		private double _masterVolume;					// masterVolume * masterVolume (for quick calculations)

		private uint _waveType;							// The type of wave to generate

		private double _envelopeVolume;					// Current volume of the envelope
		private int _envelopeStage;						// Current stage of the envelope (attack, sustain, decay, end)
		private double _envelopeTime;					// Current time through current enelope stage
		private double _envelopeLength;					// Length of the current envelope stage
		private double _envelopeLength0;				// Length of the attack stage
		private double _envelopeLength1;				// Length of the sustain stage
		private double _envelopeLength2;				// Length of the decay stage
		private double _envelopeOverLength0;			// 1 / _envelopeLength0 (for quick calculations)
		private double _envelopeOverLength1;			// 1 / _envelopeLength1 (for quick calculations)
		private double _envelopeOverLength2;			// 1 / _envelopeLength2 (for quick calculations)
		private double _envelopeFullLength;				// Full length of the volume envelop (and therefore sound)

		private double _sustainPunch;					// The punch factor (louder at begining of sustain)

		private int _phase;								// Phase through the wave
		private double _pos;							// Phase expresed as a double from 0-1, used for fast sin approx
		private double _period;							// Period of the wave
		private double _periodTemp;						// Period modified by vibrato
		private double _maxPeriod;						// Maximum period before sound stops (from minFrequency)

		private double _slide;							// Note slide
		private double _deltaSlide;						// Change in slide
		private double _minFreqency;					// Minimum frequency before stopping

		private double _vibratoPhase;					// Phase through the vibrato sine wave
		private double _vibratoSpeed;					// Speed at which the vibrato phase moves
		private double _vibratoAmplitude;				// Amount to change the period of the wave by at the peak of the vibrato wave

		private double _changeAmount;					// Amount to change the note by
		private int _changeTime;						// Counter for the note change
		private int _changeLimit;						// Once the time reaches this limit, the note changes

		private double _squareDuty;						// Offset of center switching point in the square wave
		private double _dutySweep;						// Amount to change the duty by

		private int _repeatTime;						// Counter for the repeats
		private int _repeatLimit;						// Once the time reaches this limit, some of the variables are reset

		private bool _phaser;						// If the phaser is active
		private double _phaserOffset;					// Phase offset for phaser effect
		private double _phaserDeltaOffset;				// Change in phase offset
		private int _phaserInt;							// Integer phaser offset, for bit maths
		private int _phaserPos;							// Position through the phaser buffer
		private double[] _phaserBuffer;			// Buffer of wave values used to create the out of phase second wave

		private bool _filters;						// If the filters are active
		private double _lpFilterPos;					// Adjusted wave position after low-pass filter
		private double _lpFilterOldPos;					// Previous low-pass wave position
		private double _lpFilterDeltaPos;				// Change in low-pass wave position, as allowed by the cutoff and damping
		private double _lpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
		private double _lpFilterDeltaCutoff;			// Speed of the low-pass cutoff multiplier
		private double _lpFilterDamping;				// Damping muliplier which restricts how fast the wave position can move
		private bool _lpFilterOn;					// If the low pass filter is active

		private double _hpFilterPos;					// Adjusted wave position after high-pass filter
		private double _hpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
		private double _hpFilterDeltaCutoff;			// Speed of the high-pass cutoff multiplier

		private double[] _noiseBuffer;			// Buffer of random values used to generate noise

		private double _superSample;					// Actual sample writen to the wave
		private double _sample;							// Sub-sample calculated 8 times per actual sample, averaged out to get the super sample
		private uint _sampleCount;						// double of samples added to the buffer sample
		private double _bufferSample;					// Another supersample used to create a 22050Hz wave

		//--------------------------------------------------------------------------
		//	
		//  Getters / Setters
		//
		//--------------------------------------------------------------------------

		/** The sound parameters */
		public SfxrParams Param
		{
			get { return _params; }
			set
			{
				_params = value;
				_params.ParamsDirty = true;
			}
		}

		private readonly Random _random;

		public SfxrSynth()
		{
			_random = new Random();
			Param = new SfxrParams();
		}

		//--------------------------------------------------------------------------
		//
		// Sound Methods
		//
		//--------------------------------------------------------------------------

		/**
		 * Plays the sound. If the parameters are dirty, synthesises sound as it plays, caching it for later. 
		 * If they're not, plays from the cached sound. 
		 * Won't play if caching asynchronously. 
		 */
		public void Play(int SampleRate = 44100, int BitsPerSample = 16, int NumChannels = 2)
		{
			var isfStream = new IsolatedStorageFileStream("sound.wav", FileMode.Create, FileAccess.Write,
											IsolatedStorageFile.GetUserStoreForApplication());

			Reset(true);

			var numSamples = (int)_envelopeFullLength;
			if (BitsPerSample == 16) numSamples *= 2;
			if (SampleRate == 22050) numSamples /= 2;


			var SubChunk2Size = numSamples * NumChannels * (BitsPerSample / 8);

			var ByteRate = SampleRate * NumChannels * (BitsPerSample / 8);
			var BlockAlign = NumChannels * (BitsPerSample / 8);

			var ChunkSize = 4 + (8 + Subchunk1Size) + (8 + SubChunk2Size);

			//write header

			// Chunk ID "RIFF"
			byte[] riff = { 0x52, 0x49, 0x46, 0x46 };
			isfStream.Write(riff, 0, riff.Length);

			// Chunk Data Size
			var chunkSize = BitConverter.GetBytes(ChunkSize);
			isfStream.Write(chunkSize, 0, chunkSize.Length);

			// RIFF Type "WAVE"
			byte[] wave = { 0x57, 0x41, 0x56, 0x45 };
			isfStream.Write(wave, 0, wave.Length);

			//write format

			//"fmt "
			byte[] subchunk1Id = { 0x66, 0x6d, 0x74, 0x20 };
			isfStream.Write(subchunk1Id, 0, subchunk1Id.Length);

			//Chunk Size
			var subchunk1Size = BitConverter.GetBytes(Subchunk1Size);
			isfStream.Write(subchunk1Size, 0, subchunk1Size.Length);

			//Audio Format (PCM)
			var audioFormat = BitConverter.GetBytes(AudioFormat);
			isfStream.Write(audioFormat, 0, 2);

			//Number of Channels (1 or 2)
			var numChannels = BitConverter.GetBytes(NumChannels);
			isfStream.Write(numChannels, 0, 2);

			//Sample Rate
			var sampleRate = BitConverter.GetBytes(SampleRate);
			isfStream.Write(sampleRate, 0, sampleRate.Length);

			//Byte Rate
			var byteRate = BitConverter.GetBytes(ByteRate);
			isfStream.Write(byteRate, 0, byteRate.Length);

			//Block Align
			var blockAlign = BitConverter.GetBytes(BlockAlign);
			isfStream.Write(blockAlign, 0, 2);

			//Bits Per Sample
			var bitsPerSample = BitConverter.GetBytes(BitsPerSample);
			isfStream.Write(bitsPerSample, 0, 2);

			//data

			//Chunk ID
			//"data"
			byte[] subChunk2Id = { 0x64, 0x61, 0x74, 0x61 };
			isfStream.Write(subChunk2Id, 0, subChunk2Id.Length);

			//Chunk Size
			var subChunk2Size = BitConverter.GetBytes(SubChunk2Size);
			isfStream.Write(subChunk2Size, 0, subChunk2Size.Length);

			//Wave Sound Data
			var data = new byte[NumChannels * (BitsPerSample / 8) * numSamples];
			SynthWave(ref data, numSamples, BitsPerSample);

			isfStream.Write(data, 0, data.Length);

			isfStream.Close();
			isfStream.Dispose();

			var str = new IsolatedStorageFileStream("sound.wav", FileMode.Open, FileAccess.Read,
															IsolatedStorageFile.GetUserStoreForApplication());


			Stream stream = str;
			var effect = SoundEffect.FromStream(stream);
			FrameworkDispatcher.Update();
			effect.Play();
			str.Close();

		}

		//--------------------------------------------------------------------------
		//	
		//  Synth Methods
		//
		//--------------------------------------------------------------------------

		/**
			* Resets the running variables from the params
			* Used once at the start (total reset) and for the repeat effect (partial reset)
			* @param	totalReset	If the reset is total
			*/
		private void Reset(bool totalReset)
		{
			// Shorter reference
			var p = _params;

			_period = 100.0 / (p.StartFrequency * p.StartFrequency + 0.001);
			_maxPeriod = 100.0 / (p.MinFrequency * p.MinFrequency + 0.001);

			_slide = 1.0 - p.Slide * p.Slide * p.Slide * 0.01;
			_deltaSlide = -p.DeltaSlide * p.DeltaSlide * p.DeltaSlide * 0.000001;

			if (p.WaveType == 0)
			{
				_squareDuty = 0.5 - p.SquareDuty * 0.5;
				_dutySweep = -p.DutySweep * 0.00005;
			}

			if (p.ChangeAmount > 0.0) 	_changeAmount = 1.0 - p.ChangeAmount * p.ChangeAmount * 0.9;
			else 						_changeAmount = 1.0 + p.ChangeAmount * p.ChangeAmount * 10.0;

			_changeTime = 0;

			if(p.ChangeSpeed == 1.0) 	_changeLimit = 0;
			else 						_changeLimit = (int) ( (1.0 - p.ChangeSpeed) * (1.0 - p.ChangeSpeed) * 20000 + 32 );

			if(totalReset)
			{
				p.ParamsDirty = false;

				_masterVolume = p.MasterVolume * p.MasterVolume;

				_waveType = p.WaveType;

				if (p.SustainTime < 0.01) p.SustainTime = 0.01;

				var totalTime = p.AttackTime + p.SustainTime + p.DecayTime;
				if (totalTime < 0.18) 
				{
					var multiplier = 0.18 / totalTime;
					p.AttackTime *= multiplier;
					p.SustainTime *= multiplier;
					p.DecayTime *= multiplier;
				}

				_sustainPunch = p.SustainPunch;

				_phase = 0;

				_minFreqency = p.MinFrequency;

				_filters = p.LpFilterCutoff != 1.0 || p.HpFilterCutoff != 0.0;

				_lpFilterPos = 0.0;
				_lpFilterDeltaPos = 0.0;
				_lpFilterCutoff = p.LpFilterCutoff * p.LpFilterCutoff * p.LpFilterCutoff * 0.1;
				_lpFilterDeltaCutoff = 1.0 + p.LpFilterCutoffSweep * 0.0001;
				_lpFilterDamping = 5.0 / (1.0 + p.LpFilterResonance * p.LpFilterResonance * 20.0) * (0.01 + _lpFilterCutoff);
				if (_lpFilterDamping > 0.8) _lpFilterDamping = 0.8;
				_lpFilterDamping = 1.0 - _lpFilterDamping;
				_lpFilterOn = p.LpFilterCutoff != 1.0;

				_hpFilterPos = 0.0;
				_hpFilterCutoff = p.HpFilterCutoff * p.HpFilterCutoff * 0.1;
				_hpFilterDeltaCutoff = 1.0 + p.HpFilterCutoffSweep * 0.0003;

				_vibratoPhase = 0.0;
				_vibratoSpeed = p.VibratoSpeed * p.VibratoSpeed * 0.01;
				_vibratoAmplitude = p.VibratoDepth * 0.5;

				_envelopeVolume = 0.0;
				_envelopeStage = 0;
				_envelopeTime = 0;
				_envelopeLength0 = p.AttackTime * p.AttackTime * 100000.0;
				_envelopeLength1 = p.SustainTime * p.SustainTime * 100000.0;
				_envelopeLength2 = p.DecayTime * p.DecayTime * 100000.0 + 10;
				_envelopeLength = _envelopeLength0;
				_envelopeFullLength = _envelopeLength0 + _envelopeLength1 + _envelopeLength2;

				_envelopeOverLength0 = 1.0 / _envelopeLength0;
				_envelopeOverLength1 = 1.0 / _envelopeLength1;
				_envelopeOverLength2 = 1.0 / _envelopeLength2;

				_phaser = p.PhaserOffset != 0.0 || p.PhaserSweep != 0.0;

				_phaserOffset = p.PhaserOffset * p.PhaserOffset * 1020.0;
				if(p.PhaserOffset < 0.0) _phaserOffset = -_phaserOffset;
				_phaserDeltaOffset = p.PhaserSweep * p.PhaserSweep * p.PhaserSweep * 0.2;
				_phaserPos = 0;

				if(_phaserBuffer == null)
					_phaserBuffer = new double[1024];
				if(_noiseBuffer == null)
					_noiseBuffer = new double[32];

				for(var i = 0; i < 1024; i++) _phaserBuffer[i] = 0.0;
				for(var i = 0; i < 32; i++) _noiseBuffer[i] = _random.NextDouble() * 2.0 - 1.0 ;

				_repeatTime = 0;

				if (p.RepeatSpeed == 0.0) 	_repeatLimit = 0;
				else 						_repeatLimit = (int)((1.0-p.RepeatSpeed) * (1.0-p.RepeatSpeed) * 20000) + 32;
			}
		}

		/**
		* Writes the wave to the supplied buffer ByteArray
		* @param	buffer		A ByteArray to write the wave to
		* @param	waveData	If the wave should be written for the waveData
		* @return				If the wave is finished
		*/
		private void SynthWave(ref byte[] buffer, int numSamples, int bitsPerSample = 16)
		{
			_finished = false;

			_sampleCount = 0;
			_bufferSample = 0.0;

			for (var i = 0; i < numSamples; i++)
			{
				if (_finished) return;

				// Repeats every _repeatLimit times, partially resetting the sound parameters
				if(_repeatLimit != 0)
				{
					if(++_repeatTime >= _repeatLimit)
					{
						_repeatTime = 0;
						Reset(false);
					}
				}

				// If _changeLimit is reached, shifts the pitch
				if(_changeLimit != 0)
				{
					if(++_changeTime >= _changeLimit)
					{
						_changeLimit = 0;
						_period *= _changeAmount;
					}
				}

				// Acccelerate and apply slide
				_slide += _deltaSlide;
				_period *= _slide;

				// Checks for frequency getting too low, and stops the sound if a minFrequency was set
				if(_period > _maxPeriod)
				{
					_period = _maxPeriod;
					if(_minFreqency > 0.0) _finished = true;
				}

				_periodTemp = _period;

				// Applies the vibrato effect
				if(_vibratoAmplitude > 0.0)
				{
					_vibratoPhase += _vibratoSpeed;
					_periodTemp = _period * (1.0 + Math.Sin(_vibratoPhase) * _vibratoAmplitude);
				}

				_periodTemp = (int)(_periodTemp);
				if(_periodTemp < 8) _periodTemp = 8;

				// Sweeps the square duty
				if (_waveType == 0)
				{
					_squareDuty += _dutySweep;
							if(_squareDuty < 0.0) _squareDuty = 0.0;
					else if (_squareDuty > 0.5) _squareDuty = 0.5;
				}

				// Moves through the different stages of the volume envelope
				if(++_envelopeTime > _envelopeLength)
				{
					_envelopeTime = 0;

					switch(++_envelopeStage)
					{
						case 1: _envelopeLength = _envelopeLength1; break;
						case 2: _envelopeLength = _envelopeLength2; break;
					}
				}

				// Sets the volume based on the position in the envelope
				switch(_envelopeStage)
				{
					case 0: _envelopeVolume = _envelopeTime * _envelopeOverLength0; 									break;
					case 1: _envelopeVolume = 1.0 + (1.0 - _envelopeTime * _envelopeOverLength1) * 2.0 * _sustainPunch; break;
					case 2: _envelopeVolume = 1.0 - _envelopeTime * _envelopeOverLength2; 								break;
					case 3: _envelopeVolume = 0.0; _finished = true; 													break;
				}

				// Moves the phaser offset
				if (_phaser)
				{
					_phaserOffset += _phaserDeltaOffset;
					_phaserInt = (int)(_phaserOffset);

					if(_phaserInt < 0) 	_phaserInt = -_phaserInt;
					else if (_phaserInt > 1023) _phaserInt = 1023;
				}

				// Moves the high-pass filter cutoff
				if(_filters && _hpFilterDeltaCutoff != 0.0)
				{
					_hpFilterCutoff *= _hpFilterDeltaCutoff;
							if(_hpFilterCutoff < 0.00001) 	_hpFilterCutoff = 0.00001;
					else if(_hpFilterCutoff > 0.1) 		_hpFilterCutoff = 0.1;
				}

				_superSample = 0.0;
				for(var j = 0; j < 8; j++)
				{
					// Cycles through the period
					_phase++;
					if(_phase >= _periodTemp)
					{
						_phase = (int) (_phase - _periodTemp);

						// Generates new random noise for this period
						if(_waveType == 3) 
						{ 
							for(var n = 0; n < 32; n++) _noiseBuffer[n] = _random.NextDouble() * 2.0 - 1.0;
						}
					}

					// Gets the sample from the oscillator
					switch(_waveType)
					{
						case 0: // Square wave
						{
							_sample = ((_phase / _periodTemp) < _squareDuty) ? 0.5 : -0.5;
							break;
						}
						case 1: // Saw wave
						{
							_sample = 1.0 - (_phase / _periodTemp) * 2.0;
							break;
						}
						case 2: // Sine wave (fast and accurate approx)
						{
							_pos = _phase / _periodTemp;
							_pos = _pos > 0.5 ? (_pos - 1.0) * 6.28318531 : _pos * 6.28318531;
							_sample = _pos < 0 ? 1.27323954 * _pos + .405284735 * _pos * _pos : 1.27323954 * _pos - 0.405284735 * _pos * _pos;
							_sample = _sample < 0 ? .225 * (_sample *-_sample - _sample) + _sample : .225 * (_sample * _sample - _sample) + _sample;

							break;
						}
						case 3: // Noise
						{
							if ((_phase * 32 / (int)(_periodTemp)) < _noiseBuffer.Length)
							{
								_sample = _noiseBuffer[_phase * 32 / (int)(_periodTemp)];
							}
							break;
						}
					}

					// Applies the low and high pass filters
					if (_filters)
					{
						_lpFilterOldPos = _lpFilterPos;
						_lpFilterCutoff *= _lpFilterDeltaCutoff;
								if(_lpFilterCutoff < 0.0) _lpFilterCutoff = 0.0;
						else if(_lpFilterCutoff > 0.1) _lpFilterCutoff = 0.1;

						if(_lpFilterOn)
						{
							_lpFilterDeltaPos += (_sample - _lpFilterPos) * _lpFilterCutoff;
							_lpFilterDeltaPos *= _lpFilterDamping;
						}
						else
						{
							_lpFilterPos = _sample;
							_lpFilterDeltaPos = 0.0;
						}

						_lpFilterPos += _lpFilterDeltaPos;

						_hpFilterPos += _lpFilterPos - _lpFilterOldPos;
						_hpFilterPos *= 1.0 - _hpFilterCutoff;
						_sample = _hpFilterPos;
					}

					// Applies the phaser effect
					if (_phaser)
					{
						_phaserBuffer[_phaserPos&1023] = _sample;
						_sample += _phaserBuffer[(_phaserPos - _phaserInt + 1024) & 1023];
						_phaserPos = (_phaserPos + 1) & 1023;
					}

					_superSample += _sample;
				}

				// Averages out the super samples and applies volumes
				_superSample = _masterVolume * _envelopeVolume * _superSample * 0.125;

				// Clipping if too loud
						if(_superSample > 1.0) 	_superSample = 1.0;
				else if(_superSample < -1.0) 	_superSample = -1.0;


				_bufferSample += _superSample;

				_sampleCount++;

				// Writes mono wave data to the .wav format
				{
					_bufferSample /= _sampleCount;
					_sampleCount = 0;

					if (bitsPerSample == 16)
					{
						byte[] bitDepth = BitConverter.GetBytes((short) ( 32000.0 * _bufferSample ));
						buffer[2 * i] = bitDepth[0];
						buffer[2 * i + 1] = bitDepth[1];
					}
					else
					{
						buffer[i] = (byte) (_bufferSample * 127 + 128);
					}

					_bufferSample = 0.0;
				}
			}

			return;
		}

	}
}
