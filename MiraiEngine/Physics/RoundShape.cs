using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class RoundShape
    {
        public Vector2f Center { get; internal set; }
        public float Radius { get; internal set; }
        
        public RoundShape(Vector2f center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Intersects(RoundShape second)
        {
            return (this.Center - second.Center).Length() < this.Radius + second.Radius;
        }

        public static Vector2f GetEjectingVector(RoundShape first, RoundShape second)
        {
            var direction = (first.Center - second.Center).Normalize();
            var delta = (first.Radius + second.Radius) - (second.Center - first.Center).Length() + 1;

            return direction * delta;
        }
    }
}
