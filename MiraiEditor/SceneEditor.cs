using System;
using System.Windows.Forms;
using System.Collections.Generic;

using MiraiEngine;
using SFML.Window;
using System.Drawing;


namespace MiraiEditor
{
    class SceneEditor
    {
        private EditorWindow window;
        private float cameraSpeed = 32;
        private IGameObject dragNDrop;
        private Vector2f dndOffset;
        private Vector2f dndStartCameraPosition;
        private Vector2f dndLastMousePosition;

        public Scene Scene { get; set; } 
        public IGameObject GrippedObject { get; private set; }
        public Dictionary<uint, Bitmap> SpriteSet { get; private set; }

        public event Action<object, EventArgs> ObjectChanged;

        public SceneEditor(EditorWindow window)
        {
            this.window = window;
            window.ModeSwitch += ModeSwitched;

            CreateNewScene(800, 600);
            SpriteSet = new Dictionary<uint, Bitmap>();
            SpriteSet.Add(0, new Bitmap(1, 1));
            ObjectsManager.NewTextureBinded += LoadBitmap;
            ObjectsManager.LoadDefaultFolder();
        }

        public void LoadBitmap(uint id, string filename)
        {
            SpriteSet.Add(id, ResourceManager.GetBitmap(filename));
        }

        public void CreateNewScene(int width, int height)
        {
            GrippedObject = null;
            var screenSize = new Vector2f(window.ClientSize.Width, window.ClientSize.Height);
            Scene = new Scene(new Vector2f(width, height), screenSize);
            Scene.CreateCamera();
        }

        public void GrepObject(IGameObject obj, Vector2f mousePosition)
        {
            dragNDrop = obj;
            GrippedObject = obj;
            dndOffset = obj.Body.Position - mousePosition;
            dndStartCameraPosition = Scene.Camera.Body.Position;

            if (ObjectChanged != null) ObjectChanged(this, EventArgs.Empty);
        }

        public void CleanHands()
        {
            GrippedObject = null;

            if (ObjectChanged != null) ObjectChanged(this, EventArgs.Empty);
        }

        public void DropObject()
        {
            dragNDrop = null;
        }

        #region EventHandlers

        public void ModeSwitched(object sender, EditorMode e)
        {
            if (e == EditorMode.EditObject)
            {
                window.KeyDown -= KeyPressed;
                window.MouseDown -= MouseButtonPressed;
                window.MouseUp -= MouseButtonReleased;
                window.MouseMove -= MouseMoved;
            }
            else
            {
                window.KeyDown += KeyPressed;
                window.MouseDown += MouseButtonPressed;
                window.MouseUp += MouseButtonReleased;
                window.MouseMove += MouseMoved;
            }
        }

        private void KeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Scene.Camera.Body.Position += GetCameraVelocity(e.KeyCode);
            if (dragNDrop != null) UpdateDnd(dndLastMousePosition);
        }
        
        private Vector2f GetCameraVelocity(Keys keycode)
        {
            switch(keycode)
            {
                case Keys.W:
                    return new Vector2f(0, -cameraSpeed);
                case Keys.S:
                    return new Vector2f(0, cameraSpeed);
                case Keys.A:
                    return new Vector2f(-cameraSpeed, 0);
                case Keys.D:
                    return new Vector2f(cameraSpeed, 0);
                default:
                    return new Vector2f();
            }
        }

        private void MouseButtonPressed(object sender, MouseEventArgs e)
        {
            var clickPosition = new Vector2f(e.X, e.Y);

            foreach (var obj in Scene)
                if (IsMouseAbove(obj, clickPosition))
                    GrepObject(obj, clickPosition);
  
            if (dragNDrop == null) CleanHands();
        }

        private void MouseButtonReleased(object sender, MouseEventArgs e)
        {
            DropObject();
        }

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            if (dragNDrop != null)
            {
                var mousePosition = new Vector2f(e.X, e.Y);
                dndLastMousePosition = mousePosition;
                UpdateDnd(mousePosition);
            }
        }

        private void UpdateDnd(Vector2f mousePosition)
        {
            var cameraDelta = dndStartCameraPosition - Scene.Camera.Body.Position;
            dragNDrop.Body.Position = mousePosition + dndOffset - cameraDelta;

            if (ObjectChanged != null) ObjectChanged(this, EventArgs.Empty);
        }

        private bool IsMouseAbove(IGameObject obj, Vector2f clickPosition)
        {
            var topLeft = obj.Body.Position - Scene.Camera.Body.Position;
            var bottomRight = topLeft + obj.Body.Size;
            return clickPosition.X > topLeft.X && clickPosition.X < bottomRight.X
                && clickPosition.Y > topLeft.Y && clickPosition.Y < bottomRight.Y;
        }

        #endregion
    }
}
