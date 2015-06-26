using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public interface IUserScript
    {
        // MARKER
        // base interface
    }

    public interface IActionScript : IUserScript
    {
        void ProcessLogic(IGameObject self, Scene world, KeyboardState keyboard);
    }

    public interface IKeyboardScript : IUserScript
    {
        void ProcessKey(IGameObject self, Scene world, KeyEventArgs args);
    }

    public interface ICollisionScript : IUserScript
    {
        void ProcessCollision(IGameObject self, Scene world, Collision collision);
    }
}
