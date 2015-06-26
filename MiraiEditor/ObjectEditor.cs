using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Window;
using System.Windows.Forms;

namespace MiraiEditor
{
    class ObjectEditor
    {
        private EditorWindow window;
        private int cameraSpeed = 32;

        public GameObject EditingObject { get; private set; }
        public Camera Camera { get; private set; }
        public string TextureName { get; set; }
        public event Action<object, EventArgs> ObjectSaved;

        public ObjectEditor(EditorWindow window)
        {
            this.window = window;
            window.ModeSwitch += ModeSwitched;

            CreateNewObject();
        }

        public void CreateNewObject()
        {
            EditingObject = new GameObject(0, new Vector2f(), new Vector2f(128, 128), 1, new Vector2f(), false, false);
            
            var target = EditingObject.Body;
            var offset = new Vector2f((window.ClientSize.Width - 300) / 2, window.ClientSize.Height / 2);
            var cameraPosition = target.Position + target.Collider.Size / 2 - offset;

            var screenSize = new Vector2f(window.ClientSize.Width, window.ClientSize.Height);
            Camera = new Camera(cameraPosition, screenSize);
            
            TextureName = "";
        }

        public void SaveEditingObject(string filename, string textureName)
        {
            var id = EditingObject.ID;
            var size = EditingObject.Body.Size;
            var weight = EditingObject.Body.Weight;
            var colliderOffset = EditingObject.Body.Collider.Position - EditingObject.Body.Position;
            var isSolid = EditingObject.Body.IsSolid;
            var isStatic = EditingObject.Body.IsStatic;

            ObjectsManager.Register(id, size, weight, colliderOffset, isSolid, isStatic, textureName, filename);
            ObjectsManager.SaveToFile(filename, textureName, EditingObject);

            if (ObjectSaved != null) ObjectSaved(this, EventArgs.Empty);
        }

        #region EventHandlers

        public void ModeSwitched(object sender, EditorMode e)
        {
            if (e == EditorMode.EditObject)
                window.KeyDown += KeyPressed;
            else
                window.KeyDown -= KeyPressed;
        }
        
        private void KeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Camera.Body.Position += GetCameraVelocity(e.KeyCode);
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

        #endregion
    }
}
