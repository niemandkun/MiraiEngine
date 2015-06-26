using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFML.Window;
using SFML.Graphics;


namespace MiraiEngine
{
    public class Scene : IEnumerable<IGameObject>
    {
        public Vector2f Size { get; private set; }
        public uint ElapsedTicks { get; private set; }
        public Camera Camera { get; private set; }
        public ParticlesContainer Particles { get; private set; }
        public LightContainer Lights { get; private set; }

        private Dictionary<uint, HashSet<IGameObject>> objectsByID;
        private Queue<IGameObject> removeQueue;
        private Queue<IGameObject> addQueue;
       
        public Vector2f Gravity
        {
            get { return Physics.Gravity; }
            set { Physics.Gravity = value; }
        }
        
        public uint PhysicsQuality
        {
            get { return Physics.IterCount; }
            set { Physics.IterCount = value; }
        }

        internal PhysicalProcessor Physics;

		public Scene(Vector2f size)
		{
            Size = size;
            ElapsedTicks = 0;
            objectsByID = new Dictionary<uint, HashSet<IGameObject>>();
            removeQueue = new Queue<IGameObject>();
            addQueue = new Queue<IGameObject>();
            Physics = new PhysicalProcessor(this, new Vector2f(), 4, PhysicsMode.Incircle);
            Particles = new ParticlesContainer();
            Lights = new LightContainer();
        }

        public Scene(Vector2f size, Vector2f gravity)
            : this(size)
        {
            Physics.Gravity = gravity;
        }

        public void CreateCamera()
        {
            Camera = new Camera(new Vector2f());
            Add(Camera);
        }
        
        public void SetActiveCamera(Camera camera)
        {
            Camera = camera;
        }

        public void Add(IGameObject obj)
        {
            addQueue.Enqueue(obj);
        }

        public void Remove(IGameObject obj)
        {
            removeQueue.Enqueue(obj);
        }
        
        public void Commit()
        {
            while (removeQueue.Count > 0)
            {
                var obj = removeQueue.Dequeue();
                objectsByID[obj.ID].Remove(obj);
            }
            
            while (addQueue.Count > 0)
            {
                var obj = addQueue.Dequeue();
                if (objectsByID.ContainsKey(obj.ID))
                    objectsByID[obj.ID].Add(obj);
                else
                    objectsByID[obj.ID] = new HashSet<IGameObject>(new[] { obj });
            }
        }

        public IEnumerable<IGameObject> GetByID(uint id)
        {
            if (objectsByID.ContainsKey(id))
                return objectsByID[id]
                    .Union(addQueue)
                    .Where(obj => obj.ID == id);
            else
                return addQueue.Where(obj => obj.ID == id);
        }
        
        public void UpdateWorld(HashSet<Keyboard.Key> keysPressed)
        {
            Commit();
            UpdateLogic(new KeyboardState(keysPressed));
            Physics.Step();
            Particles.Update();
            Lights.Update();
            CheckOutOfRange();

            ElapsedTicks++;
        }

        private void CheckOutOfRange()
        {
            foreach (var obj in this)
                if (obj.Body.Position.Length() > Size.Length() * 2)
                    Remove(obj);
        }

        private void UpdateLogic(KeyboardState keys)
        {
            UpdateCamera();
            foreach (var obj in this)
                foreach (var script in obj.ActionScripts)
                    script.ProcessLogic(obj, this, keys);
        }

        public void ProcessKey(KeyEventArgs key)
        {
            foreach (var obj in this)
                foreach (var script in obj.KeyboardScripts)
                    script.ProcessKey(obj, this, key);
        }

        private void UpdateCamera()
        {
            if (Camera.IsLocked)
            {
                var target = Camera.Target.Body.Collider;
                var newPos = target.Position + target.Size / 2 + Camera.LockOffset;
                var delta = newPos - Camera.Body.Position;
                Camera.Body.Velocity = delta * Camera.ReactionSpeed;
            }
        }

        public IEnumerator<IGameObject> GetEnumerator()
        {
            foreach (var set in objectsByID.Values)
                foreach (var obj in set)
                    yield return obj;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
