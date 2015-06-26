using System;
using System.Collections.Generic;

using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public interface IGameObject
    {
        uint ID { get; }
        PhysicalModel Body { get; set; }
        List<Light> Lighting { get; }
        int DrawingPriority { get; }

        IEnumerable<IActionScript> ActionScripts { get; }
        IEnumerable<ICollisionScript> CollisionScripts { get; }
        IEnumerable<IKeyboardScript> KeyboardScripts { get; }

        void BindScript(IUserScript script);
        void UnbindScript(IUserScript script);
        void UnbindAllScripts();
    }
}
