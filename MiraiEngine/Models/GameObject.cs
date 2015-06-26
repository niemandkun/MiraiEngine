using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;

namespace MiraiEngine
{
    public class GameObject : IGameObject
    {
        public uint ID { get; private set; }
        public PhysicalModel Body { get; set; }
        public List<Light> Lighting { get; private set; }
        public int DrawingPriority { get { return Body.IsStatic ? 1 : 0; } }

        List<IActionScript> myActionScripts;
        List<ICollisionScript> myCollisionScripts;
        List<IKeyboardScript> myKeyboardScripts;

        public IEnumerable<IActionScript> ActionScripts { get { return myActionScripts; } }
        public IEnumerable<ICollisionScript> CollisionScripts { get { return myCollisionScripts; } }
        public IEnumerable<IKeyboardScript> KeyboardScripts { get { return myKeyboardScripts; } }
        
        private void Init()
        {
            myActionScripts = new List<IActionScript>();
            myKeyboardScripts = new List<IKeyboardScript>();
            myCollisionScripts = new List<ICollisionScript>();
            Lighting = new List<Light>();
        }

        public GameObject(uint id, Vector2f position, Vector2f size, double weight, 
            Vector2f colliderOffset, bool isSolid, bool isStatic)
        {
            Init();
            ID = id;
            Body = new PhysicalModel(position, size, weight, colliderOffset, isSolid, isStatic);
        }

        public void SetID(uint id)
        {
            ID = id;
        }

        public void BindScript(IUserScript script)
        {
            if (script is IActionScript)
                myActionScripts.Add((IActionScript)script);
            if (script is IKeyboardScript)
                myKeyboardScripts.Add((IKeyboardScript)script);
            if (script is ICollisionScript)
                myCollisionScripts.Add((ICollisionScript)script);
        }

        public void UnbindScript(IUserScript script)
        {
            if (script is IActionScript)
                myActionScripts.Remove((IActionScript)script);
            if (script is IKeyboardScript)
                myKeyboardScripts.Remove((IKeyboardScript)script);
            if (script is ICollisionScript)
                myCollisionScripts.Remove((ICollisionScript)script);
        }

        public void UnbindAllScripts()
        {
            myActionScripts.Clear();
            myKeyboardScripts.Clear();
            myCollisionScripts.Clear();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}
