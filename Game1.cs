using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace SimpleGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private SoundEffect _soundEffect;

        private int Height;
        private int Width;

        private Texture2D pixel;
        private Rectangle paddle;
        private Rectangle paddle2;
        private Rectangle ball;

        private bool hasReset = false;

        private double ballVelX = -25; // Initial horizontal velocity
        private double ballVelY = -25; // Initial vertical velocity

        private int points = 0;
        private int points2 = 0;
        private string score = string.Empty;
        private string score2 = string.Empty;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 800,
                PreferredBackBufferWidth = 1600,
                HardwareModeSwitch = true
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            score = "0 points";
            Height = _graphics.GraphicsDevice.Viewport.Height;
            Width = _graphics.GraphicsDevice.Viewport.Width;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Arial");
            _soundEffect = Content.Load<SoundEffect>("ping");

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            paddle = new Rectangle(0, 100, 20, 150); // Adjust paddle dimensions
            paddle2 = new Rectangle(Width - 20, 100, 20, 150); // Adjust paddle dimensions
            ball = new Rectangle(Width / 2 - 15, Height / 2 - 15, 30, 30);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Height = _graphics.GraphicsDevice.Viewport.Height;
            Width = _graphics.GraphicsDevice.Viewport.Width;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (points < 5 && points2 < 5)
            {
                score = "Player 1 has " + points + " points.";
                score2 = "Player 2 has " + points2 + " points.";

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    paddle2.Y -= 25;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    paddle2.Y += 25;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    paddle.Y -= 25;
                }

                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    paddle.Y += 25;
                }

                if (ball.X <= paddle.Right && ball.Intersects(paddle))
                {
                    _soundEffect.Play();
                    paddleReaction(true, 25);
                    ball.X = paddle.Right;
                }
                else if (ball.Right >= paddle2.Left && ball.Intersects(paddle2))
                {
                    _soundEffect.Play();
                    paddleReaction(false, 25);
                    ball.X = paddle2.Left - ball.Width;
                }

                ball.X += (int)Math.Floor(ballVelX);
                ball.Y += (int)Math.Floor(ballVelY);

                paddle.Y = MathHelper.Clamp(paddle.Y, 0, Height - paddle.Height);
                paddle2.Y = MathHelper.Clamp(paddle2.Y, 0, Height - paddle2.Height);

                if (ball.Y <= 0 || ball.Y + ball.Height >= Height)
                {
                    if (ball.Y < 0) { ball.Y = 0; }
                    if (ball.Y + ball.Height > Height) { ball.Y = Height - ball.Height; }
                    ballVelY = -ballVelY;
                }

                if (ball.X < 0)
                {
                    // Player 2 scores a point
                    points2++;
                    ResetBall();
                }
                else if (ball.X + ball.Width > Width)
                {
                    // Player 1 scores a point
                    points++;
                    ResetBall();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                resetGame();
            }

            base.Update(gameTime);
        }

        private void ResetBall()
        {
            ballVelX = 25;
            ballVelY = 25;
            ball.X = Width / 2 - ball.Width / 2;
            ball.Y = Height / 2 - ball.Height / 2;
        }

        private void paddleReaction(bool isLeftPaddle, int ballSpeed) {
            var relativeintersectY = (paddle.Y + (paddle.Height) / 2) - (ball.Y + ball.Height);
            var normalizedRelativeIntersectY = relativeintersectY / (paddle.Height / 2f);
            var bounceAngle = normalizedRelativeIntersectY * 35f;
            
            ballVelX = ballSpeed * Math.Cos(MathHelper.ToRadians(bounceAngle));

            if (isLeftPaddle)
            {
                ballVelY = ballSpeed * Math.Sin(MathHelper.ToRadians(bounceAngle));
                if (ballVelX < 0) { ballVelX = -ballVelX; }
            } else {
                ballVelY = ballSpeed * -Math.Sin(MathHelper.ToRadians(bounceAngle));
                if (ballVelX > 0) { ballVelX = -ballVelX; }
            }
        }

        private void resetGame() {
            points = 0;
            points2 = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (points == 5)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_spriteFont, "Player 1 has won!", new Vector2(600, 10), Color.White);
                _spriteBatch.End();
            } else if (points2 == 5) {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_spriteFont, "Player 2 has won!", new Vector2(600, 10), Color.White);
                _spriteBatch.End();
            }
            if (points < 5 && points2 < 5)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(pixel, paddle, Color.White);
                _spriteBatch.Draw(pixel, paddle2, Color.White);
                _spriteBatch.Draw(pixel, ball, Color.White);
                _spriteBatch.DrawString(_spriteFont, score, new Vector2(600, 10), Color.White);
                _spriteBatch.DrawString(_spriteFont, score2, new Vector2(1000, 10), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
            {
                game.Run();
            }
        }
    }
}
