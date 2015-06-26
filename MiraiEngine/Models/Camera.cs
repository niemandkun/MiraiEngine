using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public class Camera : IGameObject
    {
        public uint ID { get { return 0; } }
        public PhysicalModel Body { get; set; }
        public List<Light> Lighting { get; private set; }
        public int DrawingPriority { get { return 2; } }

        public bool IsLocked { get; private set; }
        public Vector2f LockOffset { get; set; }

        private float myReactionSpeed;
        public float ReactionSpeed {
            get { return myReactionSpeed; }
            set { myReactionSpeed = Math.Max(Math.Min(value, 1), 0); } 
        }

        List<IActionScript> myActionScripts;
        List<ICollisionScript> myCollisionScripts;
        List<IKeyboardScript> myKeyboardScripts;

        public IEnumerable<IActionScript> ActionScripts { get { return myActionScripts; } }
        public IEnumerable<ICollisionScript> CollisionScripts { get { return myCollisionScripts; } }
        public IEnumerable<IKeyboardScript> KeyboardScripts { get { return myKeyboardScripts; } }

        public IGameObject Target { get; private set; }

        public bool IsAllSeeingEye { get; set; }

        public Camera(Vector2f position)
        {
            Body = new PhysicalModel(position, new Vector2f(), 0, false, true);
            Lighting = new List<Light>();
            LockOffset = new Vector2f();
            ReactionSpeed = 1.0f;

            myActionScripts = new List<IActionScript>();
            myKeyboardScripts = new List<IKeyboardScript>();
            myCollisionScripts = new List<ICollisionScript>();
        }

        public Camera(Vector2f position, Vector2f lockOffset)
            :this(position)
        {
            LockOffset = lockOffset;
        }

        public void Lock(IGameObject obj)
        {
            Target = obj;
            IsLocked = true;
        }

        public void Unlock()
        {
            Target = null;
            IsLocked = false;
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
    }
}
