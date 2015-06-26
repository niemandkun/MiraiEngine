using System;
using SFML.Window;
using System.Linq;
using System.Collections.Generic;

namespace MiraiEngine
{
    enum PhysicsMode
    {
        AABB,
        Incircle,
        Сircumcircle,
    }

    class PhysicalProcessor
    {
        public Vector2f Gravity { get; set; }
        public uint IterCount { get; set; }
        public double StaticResistance { get; set; }

        internal CollisionCell[,] cellMap;
        private Scene master;

        private PhysicsMode mode;

        public PhysicalProcessor(Scene scene, Vector2f gravity, uint iterationsCount, PhysicsMode mode)
        {
            IterCount = iterationsCount;
            StaticResistance = 0.3;
            var sceneSize = scene.Size;
            Gravity = gravity;
            this.mode = mode;
            master = scene;

            var colonsCount = (int)(sceneSize.X / CollisionCell.Size) + 1;
            var rowsCount = (int)(sceneSize.Y / CollisionCell.Size) + 1;
            cellMap = new CollisionCell[colonsCount, rowsCount];
            
            for (var x = 0; x < cellMap.GetLength(0); x++)
                for (var y = 0; y < cellMap.GetLength(1); y++)
                    cellMap[x, y] = new CollisionCell();
        }

        public void Step()
        {   
            foreach (var obj in master)
            {
                var body = obj.Body;
                if (!body.IsStatic) body.Velocity += Gravity;                   
                UpdatePosition(body);
            }

            for (var i = 0; i < IterCount; ++i)
                ProcessCollisions(master, i);
        }

        private void UpdatePosition(PhysicalModel body)
        {
            body.Position += body.Velocity;
            
            // check if solid object is outside map borders
            if (body.IsSolid)
            {
                var target = body.Collider;
                var x = Math.Max(Math.Min(body.Collider.Position.X, master.Size.X - body.Collider.Size.X), 0);
                var y = Math.Max(Math.Min(body.Collider.Position.Y, master.Size.Y - body.Collider.Size.Y), 0);
                body.Position = new Vector2f(x, y) - body.ColliderOffset;
            }
        }

        private void SolveCollision(IGameObject first, IGameObject second, int iterationIndex, Vector2f delta)
        {
            if (second.Body.IsSolid && first.Body.IsSolid)
            {
                float weightRatio = (float)(second.Body.Weight / (first.Body.Weight + second.Body.Weight));
                if (Single.IsNaN(weightRatio)) weightRatio = 1;

                first.Body.Position += delta * weightRatio;
            }

            if (iterationIndex == 0 && !(second is Camera))
                foreach (var script in first.CollisionScripts)
                    script.ProcessCollision(first, master, new Collision(first, second, first.Body.Velocity, delta));
        }

        private void ChangeVelocity(PhysicalModel first, PhysicalModel second)
        {
            var ox = (first.Incircle.Center - second.Incircle.Center).Normalize();
            var oy = ox.Orthogonal();
            
            var v1x = first.Velocity.ProectionTo(ox).X / ox.X;
            var v1y = first.Velocity - first.Velocity.ProectionTo(ox);
            
            var v2x = second.Velocity.ProectionTo(ox).X / ox.X;
            var v2y = second.Velocity - second.Velocity.ProectionTo(ox);

            var m1 = first.Weight;
            var m2 = second.Weight;

            var newV1x = ((m1 - m2) * v1x + 2 * m2 * v2x) / (m1 + m2);
            var newV2x = ((m2 - m1) * v2x + 2 * m1 * v1x) / (m1 + m2);

            // if one of the objects is static
            // it will just reflect non-static object speed
            newV1x = Double.IsNaN(newV1x) ? -v1x * StaticResistance : newV1x;
            newV2x = Double.IsNaN(newV2x) ? -v2x * StaticResistance : newV2x;

            newV1x = Double.IsNaN(newV1x) ? 0 : newV1x;
            newV2x = Double.IsNaN(newV2x) ? 0 : newV2x;
            
            first.Velocity = ox.Normalize() * (float)newV1x + v1y; 
            second.Velocity = ox.Normalize() * (float)newV2x + v2y;
        }

        private Vector2f GetDelta(PhysicalModel first, PhysicalModel second)
        {
            switch (mode)
            {
                case PhysicsMode.Incircle:
                    return RoundShape.GetEjectingVector(first.Incircle, second.Incircle);
                case PhysicsMode.Сircumcircle:
                    return RoundShape.GetEjectingVector(first.Circumcircle, second.Circumcircle);
                default:
                    return AABB.GetEjectingVector(first.Collider, second.Collider);
            }
        }

        private bool AreIntersect(PhysicalModel first, PhysicalModel second)
        {
            switch (mode)
            {
                case PhysicsMode.Incircle:
                    return first.Incircle.Intersects(second.Incircle);
                case PhysicsMode.Сircumcircle:
                    return first.Circumcircle.Intersects(second.Circumcircle);
                default:
                    return first.Collider.Intersects(second.Collider);
            }
        }

        private void CheckCollision(IGameObject first, IGameObject second,
            Dictionary<IGameObject, HashSet<IGameObject>> done, int iterationIndex)
        {
            if (first == second || done[first].Contains(second) || done[second].Contains(first)) 
                return;

            if (AreIntersect(first.Body, second.Body))
            {
                var firstDelta = GetDelta(first.Body, second.Body);
                var secondDelta = GetDelta(second.Body, first.Body);

                if (iterationIndex == 0 && first.Body.IsSolid && second.Body.IsSolid) 
                    ChangeVelocity(first.Body, second.Body);

                SolveCollision(first, second, iterationIndex, firstDelta);
                SolveCollision(second, first, iterationIndex, secondDelta);
            }

            done[first].Add(second);
            done[second].Add(first);
        }

        private void ProcessCollisions(IEnumerable<IGameObject> scene, int iterationIndex)
        {
            UpdateCells(scene);
            var done = new Dictionary<IGameObject, HashSet<IGameObject>>();

            foreach (var cell in cellMap)
                foreach (var first in cell.ObjectsInside)
                    foreach (var second in cell.ObjectsInside)
                    {
                        if (!done.ContainsKey(first))
                            done.Add(first, new HashSet<IGameObject>());
                        if (!done.ContainsKey(second))
                            done.Add(second, new HashSet<IGameObject>());

                        CheckCollision(first, second, done, iterationIndex);
                    }
        }

        private void UpdateCells(IEnumerable<IGameObject> scene)
        {
            foreach (var cell in cellMap)
                cell.Clear();

            foreach (var obj in scene)
            {
                var collider = obj.Body.Collider;
                var cellSize = CollisionCell.Size;

                for (var x = (int)(collider.Left / cellSize); x <= collider.Right / cellSize; x++)
                    for (var y = (int)(collider.Top / cellSize); y <= collider.Bottom / cellSize; y++)
                        if (x < cellMap.GetLength(0) && y < cellMap.GetLength(1) && x >= 0 && y >= 0)
                            cellMap[x, y].Add(obj);
            }

            foreach (var cell in cellMap)
                cell.SortObjects();
        }
    }
}
