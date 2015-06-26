using System;
using SFML.Window;
using System.Linq;
using System.Collections.Generic;

namespace MiraiEngine
{
    class CollisionCell
    {
        public static int Size { get { return 256; } }
        public List<IGameObject> ObjectsInside { get; private set; }

        public CollisionCell()
        {
            ObjectsInside = new List<IGameObject>();
        }

        public void Add(IGameObject obj)
        {
            ObjectsInside.Add(obj);
        }

        public void Clear()
        {
            ObjectsInside.Clear();
        }

        public void SortObjects()
        {
            ObjectsInside = ObjectsInside
                .OrderByDescending(x => x.Body.Position.Y)
                .ToList();
        }
    }
}
