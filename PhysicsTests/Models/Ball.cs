using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsTests.Models
{
    public class Ball
    {
        public int Id { get; set; }
        public float Diameter { get; set; }
        public float Radius { get { return Diameter / 2; } }         
        public Vector2 Position { get; set; }
        public bool Selected { get; set; }
        public bool RightButtonSelected { get; set; }
        private Texture2D image;
        public Vector2 Acc { get; set; }
        public Vector2 Vel { get; set; }
        public float Mass { get; set; }


        public Ball(int id, float diameter, Vector2 position, Texture2D image)
        {
            Id = id;
            Diameter = diameter;
            Position = position;
            this.image = image;

            Mass = diameter / 2 * 10f;

            Acc = Vel = Vector2.Zero;
            Selected = false;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, new Vector2(Position.X - Radius, Position.Y - Radius), Color.White);
        }


    }
}
