using System;
using SFML.Window;

namespace MiraiEngine
{
    public class PhysicalModel
    {
        public AABB Collider { get; private set; }
        public RoundShape Incircle { get; private set; }
        public RoundShape Circumcircle { get; private set; }
        public Vector2f Velocity { get; set; }
        public Vector2f Rotation { get; private set; }
        public Vector2f Center { get { return Incircle.Center; } }
        public bool IsSolid { get; set; }
        public bool IsStatic { get; set; }

        private double myWeight;
        public double Weight 
        {
            get { return IsStatic ? Double.PositiveInfinity : myWeight; }
            set { myWeight = value; }
        }
        
        private Vector2f myPosition;
        public Vector2f Position
        {
            get { return myPosition; }
            set
            {
                myPosition = value;
                Collider.Position = value + ColliderOffset;
                Incircle.Center = Collider.Position + Size / 2;
                Circumcircle.Center = Collider.Position + Size / 2;
            }
        }
        
        public Vector2f Size { 
            get { return Collider.Size; }
            set { Collider.Size = value; }
        }

        private Vector2f myColliderOffset;
        public Vector2f ColliderOffset
        {
            get { return myColliderOffset; }
            set
            {
                myColliderOffset = value;
                Collider.Position = Position + value;
                Incircle.Center = Collider.Position + Size / 2;
                Circumcircle.Center = Collider.Position + Size / 2;
            }
        }
        
        public PhysicalModel(Vector2f position, Vector2f size, double weight, bool isSolid, bool isStatic)
        {
            Collider = new AABB(position, size);
            Incircle = new RoundShape(position + size / 2, Math.Min(Size.X, Size.Y) / 2);
            Circumcircle = new RoundShape(position + size / 2, size.Length() / 2);
            Position = position;
            Velocity = new Vector2f();
            Rotation = new Vector2f(1, 0);
            IsSolid = isSolid;
            IsStatic = isStatic;
            ColliderOffset = new Vector2f();
            Weight = weight;
        }

        public PhysicalModel(Vector2f position, Vector2f size, double weight, 
            Vector2f colliderOffset, bool isSolid, bool isStatic)
            :this(position, size, weight, isSolid, isStatic)
        {
            ColliderOffset = colliderOffset;
        }


        public void ProcessCollision(IGameObject sender, Vector2f eject)
        {
            if (!sender.Body.IsStatic) 
                eject = new Vector2f(eject.X / 2, Math.Min(eject.Y, 0));
            Position += eject;
        }

        public void Rotate(double angle)
        {
            Rotation = Rotation.Rotate(angle).Normalize();
        }
    }
}
