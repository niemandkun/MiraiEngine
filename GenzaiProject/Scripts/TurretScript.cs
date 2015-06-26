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
    class TurretScript : IActionScript
    {
        int rateOfFire = 16;
        public int Health = 10;

        public void ProcessLogic(IGameObject self, Scene world, KeyboardState keyboard)
        {
            var playerShip = world.GetByID((uint)ObjectID.Player).FirstOrDefault();
            var toPlayer = playerShip.Body.Position - self.Body.Position;
            var distanceToPlayer = toPlayer.Length();
            var laserSpeed = toPlayer.Normalize() * 5;

            if (distanceToPlayer < 800)
            {
                self.Body.Rotate(toPlayer.Angle() - self.Body.Rotation.Angle());

                if (world.ElapsedTicks % rateOfFire == 0)
                    SpawnLaser(self, world, laserSpeed);
            }
        }
        
        private void SpawnLaser(IGameObject turret, Scene world, Vector2f laserSpeed)
        {
            var gunPosition = turret.Body.Incircle.Radius * turret.Body.Rotation.Normalize();
            var shotPosition = turret.Body.Incircle.Center + gunPosition;
            var laser = ObjectsManager.Build((uint)ObjectID.Laser, shotPosition);
            var laserScript = new LaserScript(laserSpeed, ObjectID.Turret);
            laser.BindScript(laserScript);
            laser.Lighting.Add(new Light(new Vector2f(0.1f, 0.1f), Color.Magenta, LightMode.Flashlight));
            world.Add(laser);
        }
    }
}
