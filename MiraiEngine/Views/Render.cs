using System;
using System.Linq;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace MiraiEngine
{
    class Render
    {
        public RenderWindow Window { get; private set; }
        public bool LightEnabled { get; set; }
        private RenderTexture frame;

        public Render(RenderWindow window, bool lightEnabled)
        {
            Window = window;
            LightEnabled = lightEnabled;
            frame = new RenderTexture(window.Size.X, window.Size.Y);
        }

        public void RenderScene(Scene scene, GameView view)
        {
            if (LightEnabled)
                RenderWithEffects(scene, view);
            else
                RenderWithoutEffects(scene, view);
        }

        private void RenderWithEffects(Scene scene, GameView view)
        {
            var cameraOffset = scene.Camera.Body.Position - new Vector2f(Window.Size.X, Window.Size.Y) / 2;

            Window.Clear();            
            DrawLights(Window, scene, cameraOffset, LightMode.Backlight);
            
            frame.Clear();
            if (view.Background != ResourceManager.DefaultTexture)
                DrawBackground(frame, view.Background, scene.Camera.Body.Position);
            DrawObjects(frame, scene, view.SpriteSet, cameraOffset);
            frame.Display();

            var state = new RenderStates(BlendMode.Multiply);
            Window.Draw(new Sprite(frame.Texture), state);
            
            DrawLights(Window, scene, cameraOffset, LightMode.Flashlight);
            DrawParticles(Window, scene, cameraOffset);

            if (scene.Camera.IsAllSeeingEye)
                DrawGrid(Window, scene, cameraOffset);
            
            Window.Display();
        }

        private void RenderWithoutEffects(Scene scene, GameView view)
        {
            var cameraOffset = scene.Camera.Body.Position - new Vector2f(Window.Size.X, Window.Size.Y) / 2;

            Window.Clear();
            
            if (view.Background != ResourceManager.DefaultTexture)
                DrawBackground(Window, view.Background, scene.Camera.Body.Position);

            DrawObjects(Window, scene, view.SpriteSet, cameraOffset);
            
            if (scene.Camera.IsAllSeeingEye)
                DrawGrid(Window, scene, cameraOffset);
            
            Window.Display();
        }

        private void DrawParticles(RenderTarget target, Scene scene, Vector2f cameraPosition)
        {
            var transform = Transform.Identity;
            transform.Translate(-cameraPosition);
            var renderState = new RenderStates(transform);

            foreach (var particle in scene.Particles)
                target.Draw(particle, renderState);
        }

        private void DrawBackground(RenderTarget target, Texture background, Vector2f cameraPosition)
        {
            var renderState = new RenderStates(BlendMode.Add);
            var offset = new Vector2f(0.12f * cameraPosition.X % background.Size.X, 0.12f * cameraPosition.Y % background.Size.Y);
            renderState.Transform.Translate(-offset);
            var bgSprite = new Sprite(background, new IntRect(0, 0, (int)target.Size.X * 2, (int)target.Size.Y * 2));
            target.Draw(bgSprite, renderState);
        }

        private void DrawObjects(RenderTarget target, Scene scene, 
            Dictionary<uint, Sprite> sprites, Vector2f cameraOffset)
        {
            foreach (var obj in scene)
            {
                var sprite = sprites.ContainsKey(obj.ID) ? sprites[obj.ID] : new Sprite(ResourceManager.DefaultTexture);
                DrawSprite(target, obj, sprite, cameraOffset);

                if (scene.Camera.IsAllSeeingEye && IsOnScreen(obj, target, cameraOffset))
                {
                    DrawAABB(target, obj, cameraOffset);
                    DrawCircle(target, obj, cameraOffset);
                    DrawVelocity(target, obj, cameraOffset);
                }
            }
        }

        private void DrawSprite(RenderTarget target, IGameObject obj, Sprite sprite, Vector2f cameraOffset)
        {
            Transform transf = Transform.Identity;
            transf.Rotate(obj.Body.Rotation.Angle() / (float)Math.PI * 180, obj.Body.Incircle.Center - cameraOffset);
            transf.Translate(obj.Body.Position - cameraOffset);
            
            RenderStates state = new RenderStates(transf);
            target.Draw(sprite, state);
        }

        private void DrawLights(RenderTarget target, Scene scene, Vector2f cameraOffset, LightMode mode)
        {
            foreach (var obj in scene)
            {
                Transform transf = Transform.Identity;
                transf.Translate(obj.Body.Incircle.Center - cameraOffset);
                RenderStates state = new RenderStates(transf);
                state.BlendMode = BlendMode.Add;

                foreach (var light in obj.Lighting.Where(x => x.Mode == mode))
                    target.Draw(light, state);
            }

            foreach (var light in scene.Lights.Where(x => x.Mode == mode))
            {
                Transform transf = Transform.Identity;
                transf.Translate(-cameraOffset);
                RenderStates state = new RenderStates(transf);
                state.BlendMode = BlendMode.Add;
            
                target.Draw(light, state);
            }
        }

        private void DrawAABB(RenderTarget target, IGameObject obj, Vector2f cameraOffset)
        {
            Transform colliderTransf = Transform.Identity;
            colliderTransf.Translate(obj.Body.Collider.Position - cameraOffset);
            
            var shape = new RectangleShape(new Vector2f(obj.Body.Collider.Width, obj.Body.Collider.Height))
            {
                FillColor = Color.Transparent,
                OutlineThickness = 1.5f,
                OutlineColor = Color.Red,
            };
            
            target.Draw(shape, new RenderStates(colliderTransf));
        }
        
        private void DrawVelocity(RenderTarget target, IGameObject obj, Vector2f cameraOffset)
        {
            Transform colliderTransf = Transform.Identity;
            colliderTransf.Translate(obj.Body.Incircle.Center - cameraOffset);

            var shape = new RectangleShape(new Vector2f(obj.Body.Velocity.Length() * 10, 2))
            {
                FillColor = Color.Green,
                Rotation = obj.Body.Velocity.Angle() /(float)Math.PI * 180,
            };
            
            target.Draw(shape, new RenderStates(colliderTransf));
        }

        private void DrawCircle(RenderTarget target, IGameObject obj, Vector2f cameraOffset)
        {
            var radius = obj.Body.Incircle.Radius;

            Transform transf = Transform.Identity;
            transf.Translate(obj.Body.Incircle.Center - cameraOffset);
            transf.Translate(new Vector2f(-radius, -radius));
            
            var circleShape = new CircleShape(radius)
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.Green,
                OutlineThickness = 1.5f,
            };

            target.Draw(circleShape, new RenderStates(transf));
        }
        
        private bool IsOnScreen(IGameObject obj, RenderTarget target, Vector2f cameraOffset)
        {
            var body = obj.Body;
            var topLeft = body.Collider.Position - cameraOffset;
            var bottomRight = body.Collider.Position + body.Collider.Size - cameraOffset;
            
            // отрезок [a1, a2] пересекает отрезок [b1, b2]
            Func<float, float, float, float, bool> areIntersect = (a1, a2, b1, b2) =>
                a1 > b1 && a1 < b2 && a2 < b2 && a2 > b1;

            return areIntersect(topLeft.X, bottomRight.X, 0, target.Size.X) ||
                areIntersect(topLeft.Y, bottomRight.Y, 0, target.Size.Y);
        }

        private void DrawGrid(RenderTarget target, Scene scene, Vector2f cameraOffset)
        {
            var start = cameraOffset / CollisionCell.Size;
            var end = start + new Vector2f(target.Size.X, target.Size.Y) / CollisionCell.Size;
            var maxX = scene.Physics.cellMap.GetLength(0);
            var maxY = scene.Physics.cellMap.GetLength(1);

            for (var x = Math.Max((int)start.X, 0); x < Math.Min(end.X, maxX); x++)
                for (var y = Math.Max((int)start.Y, 0); y < Math.Min(end.Y, maxY); y++)
                    DrawCollisionCell(target, scene.Physics.cellMap[x, y], x, y, cameraOffset);
        }

        private void DrawCollisionCell(RenderTarget target, CollisionCell cell, int x, int y, Vector2f offset)
        {
            Transform transf = Transform.Identity;
            transf.Translate(new Vector2f(x * CollisionCell.Size, y * CollisionCell.Size) - offset);
            var shape = new RectangleShape(new Vector2f(CollisionCell.Size - 3, CollisionCell.Size - 3));
            
            shape.FillColor = Color.Transparent;
            shape.OutlineColor = cell.ObjectsInside.Count > 0 ? Color.Red : Color.Green;
            shape.OutlineThickness = 1.5f;

            target.Draw(shape, new RenderStates(transf));
        }
    }
}
