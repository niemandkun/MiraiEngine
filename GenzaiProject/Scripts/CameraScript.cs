using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Window;

namespace GenzaiProject
{
    class CameraScript : IKeyboardScript
    {
        public void ProcessKey(IGameObject self, Scene world, KeyEventArgs args)
        {
            var camera = self as Camera;

            if (args.Code == Keyboard.Key.F3)
                camera.IsAllSeeingEye = !camera.IsAllSeeingEye;

            if (args.Code == Keyboard.Key.L)
            {
                if (camera.IsLocked) camera.Unlock();
                else
                {
                    var player = world.GetByID((uint)ObjectID.Player).FirstOrDefault();
                    camera.Lock(player);
                }
            }
        }
    }
}
