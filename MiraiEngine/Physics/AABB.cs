using System;
using SFML.Window;
using System.Linq;
using System.Collections.Generic;

namespace MiraiEngine
{
    public class AABB
    {
        public Vector2f Position { get; set; } // top left angle
        public Vector2f Size { get; set; }
        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }
        public float Top { get { return Position.Y; } }
        public float Bottom { get { return Position.Y + Height; } }
        public float Left { get { return Position.X; } }
        public float Right { get { return Position.X + Width; } }

        public AABB(Vector2f position, float width, float height)
        {
            Position = position;
            Size = new Vector2f(width, height);
        }

        public AABB(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;
        }

        public bool Intersects(AABB collider)
        {
            bool verticalIntersect = this.Top < collider.Bottom && this.Bottom > collider.Top;
            bool horizontalIntersect = this.Left < collider.Right && this.Right > collider.Left;

            return verticalIntersect && horizontalIntersect;
        }

        public static Vector2f GetEjectingVector(AABB first, AABB second)
        {
            var right = second.Right - first.Left + 1;
            var left = second.Left - first.Right - 1;
            var up = second.Top - first.Bottom - 1;
            var down = second.Bottom - first.Top + 1;
            var dx = right * right > left * left ? left : right;
            var dy = down * down > up * up ? up : down;
            if (dx * dx > dy * dy) dx = 0; 
            else dy = 0;

            return new Vector2f(dx, dy);
        }

        public void Transform(Vector2f distance)
        {
            Position = new Vector2f(Position.X + distance.X, Position.Y + distance.Y);
        }

        public void Move(Vector2f position)
        {
            Position = position;
        }
    }
}
