using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Window;

namespace GenzaiProject
{
    class LaserScript : IActionScript, ICollisionScript
    {
        private Vector2f velocity;
        private ObjectID shooter;

        public void ProcessLogic(IGameObject self, Scene world, KeyboardState keyboard)
        {
            self.Body.Velocity = velocity;
        }

        public void ProcessCollision(IGameObject self, Scene world, Collision collision)
        {
            var victim = collision.Subject;
            if (victim.ID == (uint)ObjectID.Friendly) Explose(victim, world);
            
            if (victim.ID == (uint)ObjectID.Turret && shooter != ObjectID.Turret)
            {
                var turretScript = victim.ActionScripts.FirstOrDefault() as TurretScript;
                if (turretScript.Health-- == 0) Explose(victim, world);
            }

            if (victim.ID != (uint)ObjectID.Laser && victim.ID != (uint)shooter
                && victim.ID != (uint)ObjectID.Camera)
            {
                world.Remove(self);
                var sparks = new Sparks(self.Body.Position);
                world.Particles.SpawnEmitter(sparks.Emitter, sparks.Timeout);
            }
        }

        public void Explose(IGameObject victim, Scene world)
        {
            world.Remove(victim);
            var expCenter = victim.Body.Incircle.Center;
            var bang = new Bang(expCenter);
            world.Particles.SpawnEmitter(bang.Emitter, bang.Timeout);
            world.Lights.SpawnLight(new Light(expCenter, new Vector2f(1.1f, 1.1f), LightMode.Backlight), bang.Timeout);
        }

        public LaserScript(Vector2f velocity, ObjectID shooter)
        {
            this.velocity = velocity;
            this.shooter = shooter;
        }
    }
}
