using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BalloonShootDemo
{
    public class BalloonShootGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        const int _displayWidth = 1280;
        const int _displayHeight = 720;
        const int _speedIncreaseRate = 30;

        Texture2D _bgSprite;
        Texture2D _crossSprite;
        Texture2D _balloonSprite;
        SpriteFont _gameFont;
        Texture2D _popSprite;

        Rectangle balloonPosition;
        const int balloonSize = 150;
        int balloonSpeed = 100;

        bool _balloonPopped = false;
        double _popDisplayDuration = 0.1;
        double _popTimer = 0;


        int _balloonX = 0;
        int _balloonY = 0;

        int _score = 0;

        MouseState _mouseState;
        bool _mouseUp = false;

        Random random;

        public BalloonShootGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = _displayWidth;
            _graphics.PreferredBackBufferHeight = _displayHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            random = new Random();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            SetRandomBalloonPosition();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bgSprite = Content.Load<Texture2D>("assets/bg-4");
            _crossSprite = Content.Load<Texture2D>("assets/crosshair");
            _balloonSprite = Content.Load<Texture2D>("assets/balloon");
            _gameFont = Content.Load<SpriteFont>("assets/myfont");
            _popSprite = Content.Load<Texture2D>("assets/explosion");

            // TODO: use this.Content to load your game content here
        }

        private void SetRandomBalloonPosition()
        {
            int maxX = _graphics.PreferredBackBufferWidth - balloonSize;

            // Set a random X position, but reset Y to the bottom of the screen
            _balloonX = random.Next(0, maxX);
            _balloonY = _graphics.PreferredBackBufferHeight;

            balloonPosition = new Rectangle(_balloonX, _balloonY, balloonSize, balloonSize);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            _balloonY -= (int)(balloonSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            balloonPosition.Y = _balloonY;
            _mouseState = Mouse.GetState();

            if (_balloonPopped)
            {
                _popTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if(_popTimer >= _popDisplayDuration)
                {
                    _balloonPopped = false;
                    SetRandomBalloonPosition();
                    _popTimer = 0;
                }
            }
            else
            {
                if(_balloonY + balloonSize < 0)
                {
                    _score--;
                    SetRandomBalloonPosition();
                }
            }

            if(_mouseState.LeftButton == ButtonState.Pressed && _mouseUp)
            {
                var targetCenter = new Vector2(balloonPosition.X + (balloonSize / 2), balloonPosition.Y + (balloonSize / 2));
                var distanceFromCenter = Math.Sqrt(Math.Pow((_mouseState.X - targetCenter.X), 2) + Math.Pow((_mouseState.Y - targetCenter.Y) , 2));

                if (distanceFromCenter <= (balloonSize / 2))
                {
                    _score++;
                    _balloonPopped = true;
                }

                _mouseUp = false;
            }

            if (_mouseState.LeftButton == ButtonState.Released) _mouseUp = true;

            if(_score > 0)
            {
                balloonSpeed = 100 + _score * _speedIncreaseRate;
            }
            else
            {
                balloonSpeed = 100;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _spriteBatch.Draw(_bgSprite, new Vector2(0, 0), Color.AliceBlue);

            if(_balloonPopped)
            {
                _spriteBatch.Draw(_popSprite, balloonPosition, Color.IndianRed);
            }
            else
            {
                _spriteBatch.Draw(_balloonSprite, balloonPosition, Color.IndianRed);
            }

            var crossWidth = 50;
            var crossHeight = 50;

            var centerX = _mouseState.X - (crossWidth / 2);
            var centerY = _mouseState.Y - (crossHeight / 2);

            _spriteBatch.Draw(_crossSprite, new Rectangle(centerX, centerY, crossWidth, crossHeight), Color.AliceBlue);

            _spriteBatch.DrawString(_gameFont, $"Score : {_score}", new Vector2(0, 0), Color.AliceBlue);

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
