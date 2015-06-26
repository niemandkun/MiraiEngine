using System;
using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public static class Vector2fExtension
    {
        public static float Length(this Vector2f vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static float Angle(this Vector2f vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float AngleInDegrees(this Vector2f vector)
        {
            return (float)(vector.Angle() / Math.PI * 180);
        }

        public static Vector2f Orthogonal(this Vector2f vector)
        {
            return new Vector2f(-vector.Y, vector.X);
        }

        public static Vector2f ProectionTo(this Vector2f v1, Vector2f v2)
        {
            return v2.Normalize() * v1.ScalarMultiplicate(v2) / v2.Length();
        }
        
        public static Vector2f Rotate(this Vector2f vector, double radians)
        {
            var vectorAngle = vector.Angle();
            var length = vector.Length();
            float x = (float)(length * Math.Cos(radians + vectorAngle));
            float y = (float)(length * Math.Sin(radians + vectorAngle));
            return new Vector2f(x, y);
        }
        
        public static Vector2f Normalize(this Vector2f vector)
        {
            var length = vector.Length();
            if (length == 0) return vector;
            return vector / length;
        }

        public static float ScalarMultiplicate(this Vector2f first, Vector2f second)
        {
            return first.X * second.X + first.Y * second.Y;
        }

        public static float PseudoscalarMultiplicate(this Vector2f first, Vector2f second)
        {
            return first.X * second.Y - first.Y * second.X;
        }
    }
}
