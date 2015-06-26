using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using SFML.Window;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;

namespace MiraiEngine
{
    public static class GraphicsExtensions
    {
        public static void DrawScene(this Graphics g, Scene scene, Dictionary<uint, Bitmap> spriteSet)
        {
            for (var x = 0; x < scene.Physics.cellMap.GetLength(0); x++)
                for (var y = 0; y < scene.Physics.cellMap.GetLength(1); y++)
                    g.DrawCell(scene.Physics.cellMap[x, y], new Vector2f(x, y), scene.Camera);
            

            foreach (var obj in scene)
            {
                var bitmap = spriteSet.ContainsKey(obj.ID) ? spriteSet[obj.ID] : ResourceManager.DefaultBitmap;
                g.DrawObject(obj, bitmap, scene.Camera);
            }
        }

        private static void DrawCell(this Graphics g, CollisionCell cell, Vector2f indexes, Camera camera)
        {
            var location = indexes * CollisionCell.Size - camera.Body.Position;
            var locationPoint = new Point((int)location.X, (int)location.Y);
            var shape = new Rectangle(locationPoint, new Size(CollisionCell.Size, CollisionCell.Size));

            var pen = new Pen(cell.ObjectsInside.Count > 0 ? Color.DarkRed : Color.DarkCyan);
            pen.Width = 1.5f;

            g.DrawRectangle(pen, shape);
        }

        public static void DrawObject(this Graphics g, IGameObject obj, Bitmap bitmap, Camera camera)
        {
            var location = obj.Body.Position - camera.Body.Position;
            g.DrawImage(bitmap, location.X, location.Y);
            
            g.DrawAABB(obj, camera);
        }

        private static void DrawAABB(this Graphics g, IGameObject obj, Camera camera)
        {
            var location = obj.Body.Collider.Position - camera.Body.Position;
            var locationPoint = new Point((int)location.X, (int)location.Y);
            var size = obj.Body.Collider.Size;

            var shape = new Rectangle(locationPoint, new Size((int)size.X, (int)size.Y));
            var pen = new Pen(Color.Green, 2);

            g.DrawRectangle(pen, shape);
        }
    }
}
