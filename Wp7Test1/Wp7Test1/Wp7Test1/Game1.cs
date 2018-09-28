using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wp7Test1
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		readonly GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		private const int SpriteNumber = 4;

		private Texture2D[] _textures;
		private Vector2[] _spritePositions;
		private Vector2[] _spriteSpeeds;
		private int[] _spriteHeight;
		private int[] _spriteWidth;
		private Rectangle[] _spriteRect;

		SoundEffect _soundEffect;




		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			           	{
			           		PreferredBackBufferWidth = 480,
			           		PreferredBackBufferHeight = 800,
			           		IsFullScreen = true
			           	};

			// Full-screen on Windows Phone prevents the battery notification bar
			// from drawing on top of the game
			Content.RootDirectory = "Content";

			// Frame rate is 30 fps by default for Windows Phone.
			TargetElapsedTime = TimeSpan.FromTicks(333333);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			//get random number
			var rand = new Random();

			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_textures = new Texture2D[SpriteNumber];
			_spritePositions = new Vector2[SpriteNumber];
			_spriteSpeeds = new Vector2[SpriteNumber];
			_spriteHeight = new int[SpriteNumber];
			_spriteWidth = new int[SpriteNumber];
			_spriteRect = new Rectangle[SpriteNumber];
			for(var i = 0; i < SpriteNumber; i++)
			{
				//Load the GameThumbnail graphic into a texture
				_textures[i] = Content.Load<Texture2D>("GameThumbnail");

				_spriteSpeeds[i] = new Vector2(rand.Next(50,200), rand.Next(50,200));

				//Set the width and height for the sprites
				_spriteHeight[i] = _textures[i].Bounds.Height;
				_spriteWidth[i] = _textures[i].Bounds.Width;

				//Set the position of the sprite
				_spritePositions[i].X =rand.Next(_graphics.GraphicsDevice.Viewport.Width);
				_spritePositions[i].Y = rand.Next(_graphics.GraphicsDevice.Viewport.Height);

				//Set the initial positions for the collision rectangles
				_spriteRect[i]= new Rectangle((int)_spritePositions[i].X,
					(int)_spritePositions[i].Y,
					_spriteWidth[i],
					_spriteHeight[i]);
			}

			//Load the Windows Ding sound into a sound effect
			_soundEffect = Content.Load<SoundEffect>("Windows Ding");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Tells the game to exit when the back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				Exit();

			// Move the sprite around.
			for (var i = 0; i < SpriteNumber; i++)
			{
				UpdateSprite(gameTime, ref _spritePositions[i], ref _spriteSpeeds[i]);
			}

			//Check to see if the sprites collided
			CheckForCollision();

			base.Update(gameTime);
		}

		void UpdateSprite(GameTime gameTime, ref Vector2 spritePosition, ref Vector2 spriteSpeed)
		{
			// Move the sprite by speed, scaled by elapsed time.
			spritePosition +=
				spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


			// Check to see if the sprite has passed the edge of the screen
			// If so, change direction and place it at the edge of the screen
			if (spritePosition.X > _graphics.GraphicsDevice.Viewport.Width)
			{
				spriteSpeed.X *= -1;
				spritePosition.X = _graphics.GraphicsDevice.Viewport.Width;
			}

			else if (spritePosition.X < 0)
			{
				spriteSpeed.X *= -1;
				spritePosition.X = 0;
			}

			if (spritePosition.Y > _graphics.GraphicsDevice.Viewport.Height)
			{
				spriteSpeed.Y *= -1;
				spritePosition.Y = _graphics.GraphicsDevice.Viewport.Height;
			}

			else if (spritePosition.Y < 0)
			{
				spriteSpeed.Y *= -1;
				spritePosition.Y = 0;
			}

		}

		void CheckForCollision()
		{
			for (var i = 0; i < SpriteNumber; i++)
			{
				for (var j = 0; j < SpriteNumber; j++)
				{
					if (i != j)
					{
						//Set the collision rects to the current position of the sprites
						//The width and height data was set in LoadContent()
						_spriteRect[i].X = (int)_spritePositions[i].X;
						_spriteRect[i].Y = (int)_spritePositions[i].Y;

						_spriteRect[j].X = (int)_spritePositions[j].X;
						_spriteRect[j].Y = (int)_spritePositions[j].Y;

						//If the sprites rectangles intersect, play the soundEffect
						if (_spriteRect[i].Intersects(_spriteRect[j]))
						{
							_soundEffect.Play();
						}
					}
				}
			}

		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			//Clear the background color
			_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			// Draw the sprites.
			_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque);
			for (var i = 0; i < SpriteNumber; i++)
			{
				_spriteBatch.Draw(_textures[i], _spritePositions[i], Color.White);
			}
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
