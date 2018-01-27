using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsTests.Models;
using System;
using System.Collections.Generic;

namespace PhysicsTests.Managers
{
    public class BallManager
    {
        private List<Ball> balls;
        private Vector2 mouseOldPosition;
        private bool mouseRightButtonPressed;
        private Vector2 dragStartPosition;

        public BallManager()
        {
        }

        public void LoadContent(ContentManager content)
        {
            balls = new List<Ball>();
            balls.Add(new Ball(1, 64, new Vector2(200, 178), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(2, 64, new Vector2(200, 240), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(3, 64, new Vector2(200, 304), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(4, 64, new Vector2(255.68f, 272), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(5, 64, new Vector2(255.68f, 208), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(6, 64, new Vector2(144.32f, 272), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(7, 64, new Vector2(144.32f, 208), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(8, 64, new Vector2(88.64f, 240), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(9, 64, new Vector2(311.36f, 240), content.Load<Texture2D>("ball")));
            balls.Add(new Ball(10, 64, new Vector2(600, 240), content.Load<Texture2D>("ball")));
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();
            Vector2 dragVector = Vector2.Zero;

            if (state.RightButton == ButtonState.Pressed && mouseRightButtonPressed == false)
            {
                mouseRightButtonPressed = true;
                RightButtonSelectBall(state.Position);
                dragStartPosition = new Vector2(state.Position.X, state.Position.Y);
            }   
            else if (state.RightButton == ButtonState.Released)
            {
                if(mouseRightButtonPressed == true)
                {
                    dragVector = new Vector2(dragStartPosition.X - state.Position.X, dragStartPosition.Y - state.Position.Y);
                    foreach (var ball in balls)
                    {
                        if (ball.RightButtonSelected)
                            ball.Vel += dragVector;
                    }
                }
                mouseRightButtonPressed = false;
            }   

            if (state.LeftButton == ButtonState.Pressed)
                SelectBall(state.Position);
            else
            {
                foreach (var ball in balls)
                {
                    ball.Selected = false;
                }
            }

            foreach (var ball in balls)
            {
                if (ball.Selected)
                {
                    ball.Position = new Vector2(state.Position.X, state.Position.Y);
                    mouseOldPosition.X = state.Position.X;
                    mouseOldPosition.Y = state.Position.Y;
                }
            }

            var collidingBalls = IndentifyAndResolveStaticCollission();
            ResolveDynamicCollission(collidingBalls);
            UpdateBallAccVelPos(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var ball in balls)
            {
                ball.Draw(spriteBatch);
            }
        }

        private List<KeyValuePair<Ball, Ball>> IndentifyAndResolveStaticCollission()
        {
            var collidingBalls = new List<KeyValuePair<Ball, Ball>>();
            foreach (var source in balls)
            {
                foreach (var target in balls)
                {
                    if (source.Id != target.Id)
                    {
                        var distance = Vector2.Distance(source.Position, target.Position);
                        if (distance < source.Radius + target.Radius)
                        {
                            var overlapDistance = distance - source.Radius - target.Radius;
                            var interssectionVector = source.Position - target.Position;
                            interssectionVector.Normalize();
                            source.Position -= (interssectionVector * overlapDistance) * 0.5f;
                            target.Position += (interssectionVector * overlapDistance) * 0.5f;
                            collidingBalls.Add(new KeyValuePair<Ball, Ball>(source, target));
                        }
                    }
                }
            }
            return collidingBalls;
        }

        private void ResolveDynamicCollission(List<KeyValuePair<Ball, Ball>> collidingBalls)
        {
            foreach (var collision in collidingBalls)
            {
                var ball1 = collision.Key;
                var ball2 = collision.Value;
                var distance = Vector2.Distance(ball1.Position, ball2.Position);

                var normal = new Vector2((ball2.Position.X - ball1.Position.X) / distance, (ball2.Position.Y - ball1.Position.Y) / distance);

                var tangent = new Vector2(-normal.Y, normal.X);

                var dotTangent1 = Vector2.Dot(tangent, ball1.Vel);
                var dotTangent2 = Vector2.Dot(tangent, ball2.Vel);

                var dotNormal1 = Vector2.Dot(normal, ball1.Vel);
                var dotNormal2 = Vector2.Dot(normal, ball2.Vel);

                // Conservation of momentum
                var m1 = (dotNormal1 * (ball1.Mass - ball2.Mass) + 2f * ball2.Mass * dotNormal2) / (ball1.Mass + ball2.Mass);
                var m2 = (dotNormal2 * (ball2.Mass - ball1.Mass) + 2f * ball1.Mass * dotNormal1) / (ball1.Mass + ball2.Mass);

                ball1.Vel = tangent * dotTangent1 + normal * m1;
                ball2.Vel = tangent * dotTangent2 + normal * m2;
            }
        }

        private void UpdateBallAccVelPos(GameTime gameTime)
        {
            foreach (var ball in balls)
            {
                ball.Acc = -ball.Vel * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                ball.Vel += ball.Acc * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                ball.Position += ball.Vel * gameTime.ElapsedGameTime.Milliseconds * 0.01f;

                if (ball.Position.X > 800)
                    ball.Position = new Vector2(0, ball.Position.Y);
                if (ball.Position.X < 0)
                    ball.Position = new Vector2(800, ball.Position.Y);
                if (ball.Position.Y > 480)
                    ball.Position = new Vector2(ball.Position.X, 0);
                if (ball.Position.Y < 0)
                    ball.Position = new Vector2(ball.Position.X, 480);

                if (Math.Abs(ball.Vel.X + ball.Vel.Y) < 0.01f)
                    ball.Vel = Vector2.Zero;
            }
        }

        private void SelectBall(Point mousePosition)
        {
            foreach (var ball in balls)
            {
                if (Vector2.Distance(ball.Position, new Vector2(mousePosition.X, mousePosition.Y)) < ball.Radius)
                    ball.Selected = true;
            }
        }

        private void RightButtonSelectBall(Point mousePosition)
        {
            foreach (var ball in balls)
            {
                if (Vector2.Distance(ball.Position, new Vector2(mousePosition.X, mousePosition.Y)) < ball.Radius)
                    ball.RightButtonSelected = true;
                else
                    ball.RightButtonSelected = false;
            }
        }
    }
}
