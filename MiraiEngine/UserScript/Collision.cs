using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class Collision
    {
        public IGameObject Subject { get; internal set; }
        public Vector2f Eject { get; internal set; }
        public Vector2f Velocity { get; internal set; }
        
        public Vector2f Normal { get { return -Eject.Normalize(); } }
        public double Depth { get { return Eject.Length(); } }
        public Vector2f ContactPoint { get; internal set; }

        public Collision(IGameObject self, IGameObject contact, Vector2f resulVelocity, Vector2f eject)
        {
            Subject = contact;
            Eject = eject;
            Velocity = resulVelocity;
            ContactPoint = self.Body.Incircle.Center + Normal * self.Body.Incircle.Radius;
        }
    }
}
