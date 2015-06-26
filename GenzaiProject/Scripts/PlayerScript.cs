using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Window;
using SFML.Graphics;

namespace GenzaiProject
{
    class PlayerScript : IActionScript, IKeyboardScript, ICollisionScript
    {
        float throttle = 0.2f;
        Vector2f velocity;
        double turnSpeed = Math.PI / 32;

        int laserSpeed = 15;
        int laserSpriteSize = 15;
        int selfSpriteSize = 128;
        int rateOfFire = 3;
        uint startFire;
        bool leftGunIsShooting;

        public void ProcessLogic(IGameObject self, Scene world, KeyboardState keyboard)
        {
            if (keyboard.Keys.Contains(Keyboard.Key.Left))
                self.Body.Rotate(-turnSpeed);
            if (keyboard.Keys.Contains(Keyboard.Key.Right))
                self.Body.Rotate(turnSpeed);

            if (keyboard.Keys.Contains(Keyboard.Key.Down))
                Stop(self);
            else if (keyboard.Keys.Contains(Keyboard.Key.Up))
                velocity += new Vector2f(throttle, 0).Rotate(self.Body.Rotation.Angle());

            if (keyboard.Keys.Contains(Keyboard.Key.Space))
                if ((world.ElapsedTicks - startFire) % rateOfFire == 1)
                    SpawnLaser(self, world);

            self.Body.Velocity = velocity;
        }

        private void Stop(IGameObject self)
        {
            var rotationDirection = GetRotateSign(self.Body.Rotation, -velocity);
            if (rotationDirection == 0)
                if (velocity.Length() < 1)
                    velocity = new Vector2f();
                else
                    velocity += new Vector2f(Math.Min(throttle, velocity.Length()), 0).Rotate(self.Body.Rotation.Angle());
            else
                self.Body.Rotate(rotationDirection * turnSpeed);
        }

        private int GetRotateSign(Vector2f current, Vector2f desired)
        {
            if (current.ScalarMultiplicate(desired) < -1 + 1e-3) return 1;

            var threshold = Math.Sin(turnSpeed);
            var sin = current.Normalize().PseudoscalarMultiplicate(desired.Normalize());
            return Math.Sign(Math.Abs(sin) > threshold ? sin : 0);
        }

        private void SpawnLaser(IGameObject ship, Scene world)
        {
            var gunsPosition = ship.Body.Rotation;
            var sideOffset = gunsPosition.Orthogonal() * (leftGunIsShooting ? 1 : -1) * selfSpriteSize / 4;
            var laserOffset = new Vector2f(laserSpriteSize, laserSpriteSize) / 2;
            gunsPosition *= selfSpriteSize / 4;

            var shotPosition = ship.Body.Incircle.Center + gunsPosition + sideOffset - laserOffset;
            var laser = ObjectsManager.Build((uint)ObjectID.Laser, shotPosition);
            var laserScript = new LaserScript(ship.Body.Rotation * laserSpeed, ObjectID.Player);
            laser.BindScript(laserScript);
            laser.Lighting.Add(new Light(new Vector2f(0.1f, 0.1f), Color.Magenta, LightMode.Flashlight));
            leftGunIsShooting = !leftGunIsShooting;
            world.Add(laser);
        }

        public void ProcessKey(IGameObject self, Scene world, KeyEventArgs args)
        {
            if (args.Code == Keyboard.Key.G) 
                self.Body.IsSolid = !self.Body.IsSolid;
            if (args.Code == Keyboard.Key.Space)
                startFire = world.ElapsedTicks;
        }

        public void ProcessCollision(IGameObject self, Scene world, Collision collision)
        {
            velocity = collision.Velocity;
            if (collision.Subject.Body.IsSolid)
            {
                var sparks = new Sparks(collision.ContactPoint);
                world.Particles.SpawnEmitter(sparks.Emitter, sparks.Timeout);
            }
        }
    }
}
