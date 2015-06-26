using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using MiraiEngine;
using SFML.Window;

namespace MiraiEditor
{
    partial class EditorWindow : Form
    {
        private SceneEditor sceneEditor;
        private ObjectEditor objectEditor;

        private Panel sceneControlPanel;
        private Panel objectControlPanel;
        private Label objectData;
        private ListBox objectsList;
        private Timer timer;

        private int sidePanelWidth = 300;

        public EditorMode Mode { get; private set; }
        public event Action<object, EditorMode> ModeSwitch;

        public EditorWindow()
        {
            InitializeComponent();
            ClientSize = new Size(1280, 600);
            DoubleBuffered = true;
            KeyPreview = true;

            InitSelfMenu();

            sceneEditor = new SceneEditor(this);
            objectEditor = new ObjectEditor(this);
            sceneControlPanel = new Panel();
            objectControlPanel = new Panel();
            FillSceneControlPanel(sceneControlPanel);
            FillObjectControlPanel(objectControlPanel);

            sceneEditor.ObjectChanged += UpdateObjectDataText;
            objectEditor.ObjectSaved += UpdateAvailableItemsList;

            SwitchMode(EditorMode.EditScene);
            
            SizeChanged += ResizeInterface;
            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);
            
            timer = new Timer();
            timer.Tick += (sender, e) => Invalidate();
            timer.Interval = 30;
            timer.Start();
        }

        private void UpdateObjectDataText(object sender, EventArgs e)
        {
            if (sceneEditor.GrippedObject != null)
            {
                var obj = sceneEditor.GrippedObject;
                var body = obj.Body;
                var template = "{0} ({1})" +
                    "\n\nWeight: {2}" +
                    "\n\nPosition: {3}, {4}" +
                    "\n\nCollider offset: {5}, {6}" +
                    "\n\nCollider position: {7}, {8}" +
                    "\n\nSolid: {9}" + "\n\nStatic: {10}";

                objectData.Text = String.Format(template,
                    ObjectsManager.Registered[obj.ID], ObjectsManager.BindedTextures[obj.ID],
                    body.Weight,
                    body.Position.X, body.Position.Y,
                    body.ColliderOffset.X, body.ColliderOffset.Y,
                    body.Collider.Position.X, body.Collider.Position.Y,
                    body.IsSolid, body.IsStatic
                );
            }
            else
            {
                objectData.Text = "";
            }
        }

        private void UpdateAvailableItemsList(object sender, EventArgs args)
        {
            objectsList.Items.Clear();
            foreach(var item in ObjectsManager.Registered)
                objectsList.Items.Add(Tuple.Create(item.Value, item.Key));
        }

        private void FillSceneControlPanel(Panel sceneControlPanel)
        {
            objectsList = new ListBox() { Size = new Size(sidePanelWidth - 40, 150) };
            objectsList.Location = new Point(20, 20);
            foreach(var item in ObjectsManager.Registered)
                objectsList.Items.Add(Tuple.Create(item.Value, item.Key));
            
            objectData = new Label() { Size = new Size(sidePanelWidth - 40, 200) };
            objectData.Location = new Point(objectsList.Location.X, objectsList.Location.Y + 200);

            var addButton = new Button() { Text = "Add", Location = new Point(20, 170) };
            addButton.Click += (sender, args) =>
                {
                    var nameId = (Tuple<string, uint>)objectsList.SelectedItem;
                    if (nameId != null)
                    {
                        try
                        {
                            var id = nameId.Item2;
                            var obj = ObjectsManager.Build(id, sceneEditor.Scene.Camera.Body.Position);
                            sceneEditor.Scene.Add(obj);
                            sceneEditor.Scene.Commit();
                        }
                        catch
                        {
                            MessageBox.Show("Cannot add object.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };

            var createButton = new Button() { Location = new Point(100, 170), Text = "Create" };
            createButton.Click += (sender, args) =>
                {
                    objectEditor.CreateNewObject();
                    SwitchMode(EditorMode.EditObject);
                };

            var removeButton = new Button() { Location = new Point(120, 450), Text = "Remove" };
            removeButton.Click += (sender, args) =>
                {
                    if (sceneEditor.GrippedObject != null)
                    {
                        sceneEditor.Scene.Remove(sceneEditor.GrippedObject);
                        sceneEditor.Scene.Commit();
                        sceneEditor.CleanHands();
                    }
                };

            sceneControlPanel.Controls.Add(objectsList);
            sceneControlPanel.Controls.Add(objectData);
            sceneControlPanel.Controls.Add(addButton);
            sceneControlPanel.Controls.Add(removeButton);
            sceneControlPanel.Controls.Add(createButton);
        }

        private void FillObjectControlPanel(Panel objectControlPanel)
        {
            var nameField = new TextBox();
            var nameLabel = new Label() { Text = "Name" };

            var idField = new NumericUpDown() { Minimum = 0, Maximum = UInt32.MaxValue };
            idField.Value = objectEditor.EditingObject.ID;
            idField.ValueChanged += (sender, args) =>
                objectEditor.EditingObject.SetID((uint)idField.Value);

            var idLabel = new Label() { Text = "ID" };

            var weightField = new NumberTextBox();
            weightField.AppendText(objectEditor.EditingObject.Body.Weight.ToString());
            weightField.TextChanged += (sender, args) =>
                objectEditor.EditingObject.Body.Weight = weightField.ValueDouble;

            var weightLabel = new Label() { Text = "Weight" };

            var colliderWidth = new NumericUpDown() { Maximum = 1000 };
            colliderWidth.Value = (int)objectEditor.EditingObject.Body.Size.X;
            colliderWidth.ValueChanged += (sender, args) =>
                {
                    var body = objectEditor.EditingObject.Body;
                    body.Size = new Vector2f((float)colliderWidth.Value, body.Size.Y);
                };

            var colliderWidthLabel = new Label() { Text = "Collider width" };

            var colliderHeight = new NumericUpDown() { Maximum = 1000 };
            colliderHeight.Value = (int)objectEditor.EditingObject.Body.Size.Y;
            colliderHeight.ValueChanged += (sender, args) =>
                {
                    var body = objectEditor.EditingObject.Body;
                    body.Size = new Vector2f(body.Size.X, (float)colliderHeight.Value);
                };

            var colliderHeightLabel = new Label() { Text = "Collider height" };

            var colliderHOffset = new NumericUpDown() { Minimum = -1000, Maximum = 1000 };
            colliderHOffset.Value = (int)objectEditor.EditingObject.Body.ColliderOffset.X;
            colliderHOffset.ValueChanged += (sender, args) =>
                {
                    var body = objectEditor.EditingObject.Body;
                    body.ColliderOffset = new Vector2f((float)colliderHOffset.Value, body.ColliderOffset.Y);
                };

            var colliderHOffsetLabel = new Label() { Text = "Horizontal offset" };

            var colliderVOffset = new NumericUpDown() { Minimum = -1000, Maximum = 1000 };
            colliderVOffset.Value = (int)objectEditor.EditingObject.Body.ColliderOffset.Y;
            colliderVOffset.ValueChanged += (sender, args) =>
                {
                    var body = objectEditor.EditingObject.Body;
                    body.ColliderOffset = new Vector2f(body.ColliderOffset.X, (float)colliderVOffset.Value);
                };

            var colliderVOffsetLabel = new Label() { Text = "Vertical offset" };

            var textureField = new TextBox();
            textureField.TextChanged += (sender, args) =>
                objectEditor.TextureName = textureField.Text;

            var textureLabel = new Label() { Text = "Texture" };

            var isSolidCheckBox = new CheckBox() { Text = "Solid" };
            isSolidCheckBox.CheckedChanged += (sender, args) =>
                objectEditor.EditingObject.Body.IsSolid = isSolidCheckBox.Checked;

            var isStaticCheckBox = new CheckBox() { Text = "Static" };
            isStaticCheckBox.CheckedChanged += (sender, args) =>
                objectEditor.EditingObject.Body.IsStatic = isStaticCheckBox.Checked;
            
            var doneButton = new Button() { Location = new Point(0, 500), Text = "Done" };
            doneButton.Click += (sender, args) =>
                {
                    if (nameField.Text == "")
                    {
                        MessageBox.Show("Name cannot be an empty string.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var filename = nameField.Text + ".mob";
                    
                    try
                    {
                        objectEditor.SaveEditingObject(filename, objectEditor.TextureName);
                    }
                    catch(ArgumentException e)
                    {
                        MessageBox.Show(e.Message, "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    SwitchMode(EditorMode.EditScene);
                };

            var cancelButton = new Button() { Location = new Point(100, 500), Text = "Cancel" };
            cancelButton.Click += (sender, args) => SwitchMode(EditorMode.EditScene);

            var table = new TableLayoutPanel() 
            { 
                Location = new Point(20, 20),
                Size = new Size(sidePanelWidth - 40, ClientSize.Height),
            };

            var tableWidth = 2;
            var tableHeight = 9;
            
            for (var i = 0; i < tableWidth; ++i)
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            
            for (var i = 0; i < tableHeight; ++i)
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            var row = 0;
            table.Controls.Add(nameLabel, 0, row);
            table.Controls.Add(nameField, 1, row++);
            table.Controls.Add(idLabel, 0, row);
            table.Controls.Add(idField, 1, row++);
            table.Controls.Add(weightLabel, 0, row);
            table.Controls.Add(weightField, 1, row++);
            table.Controls.Add(colliderWidthLabel, 0, row);
            table.Controls.Add(colliderWidth, 1, row++);
            table.Controls.Add(colliderHeightLabel, 0, row);
            table.Controls.Add(colliderHeight, 1, row++);
            table.Controls.Add(colliderHOffsetLabel, 0, row);
            table.Controls.Add(colliderHOffset, 1, row++);
            table.Controls.Add(colliderVOffsetLabel, 0, row);
            table.Controls.Add(colliderVOffset, 1, row++);
            table.Controls.Add(textureLabel, 0, row);
            table.Controls.Add(textureField, 1, row++);
            table.Controls.Add(isSolidCheckBox, 0, row);
            table.Controls.Add(isStaticCheckBox, 1, row++);
            table.Controls.Add(doneButton, 0, row);
            table.Controls.Add(cancelButton, 1, row++);

            objectControlPanel.Controls.Add(table);
        }

        private void InitSelfMenu()
        {
            var newScene = new MenuItem("Scene");
            newScene.Click += (sender, args) =>
                {
                    var size = NumericPrompt.ShowDialog("Width", "Height", "Create scene");
                    sceneEditor.CreateNewScene((int)size.Item1, (int)size.Item2);
                    if (Mode != EditorMode.EditScene) SwitchMode(EditorMode.EditScene);
                };

            var newObject = new MenuItem("Object");
            newObject.Click += (sender, args) => 
                {
                    if (Mode != EditorMode.EditObject) SwitchMode(EditorMode.EditObject);
                    else objectEditor.CreateNewObject();
                };

            var newSubmenu = new MenuItem("New");
            newSubmenu.MenuItems.Add(newScene);
            newSubmenu.MenuItems.Add(newObject);

            var saveScene = new MenuItem("Save as...");
            saveScene.Click += (sender, args) =>
                {
            	    using (SaveFileDialog dialog = new SaveFileDialog())
            	    {
            	        dialog.Filter = "Mirai Scene files (*.mss)|*.mss|All files (*.*)|*.*";
            	        dialog.FilterIndex = 1;
                        dialog.RestoreDirectory = false;
                        dialog.InitialDirectory = SceneManager.PathToScenes;

            	        if (dialog.ShowDialog() == DialogResult.OK)
            	        {
            	            using (Stream stream = dialog.OpenFile())
            	            {
            	                SceneManager.SaveToStream(sceneEditor.Scene, stream);
            	            }
            	        }
            	    }
            	};

            var openScene = new MenuItem("Open...");
            openScene.Click += (sender, args) =>
                {
                    using (OpenFileDialog dialog = new OpenFileDialog())
                    {
            	        dialog.Filter = "Mirai Scene files (*.mss)|*.mss|All files (*.*)|*.*";
            	        dialog.FilterIndex = 1;
                        dialog.RestoreDirectory = false;
                        dialog.InitialDirectory = SceneManager.PathToScenes;
            	        
                        if (dialog.ShowDialog() == DialogResult.OK)
            	        {
            	            using (Stream stream = dialog.OpenFile())
            	            {
                                var screenSize = new Vector2f(ClientSize.Width, ClientSize.Height);
            	                var loadedScene = SceneManager.LoadFromStream(stream);
                                sceneEditor.CleanHands();
                                sceneEditor.Scene = loadedScene;
            	            }
            	        }
                    }
                };

            var fileMenu = new MenuItem[] 
            { 
                newSubmenu,
                openScene,
                saveScene,
            };

            var helpMenu = new MenuItem[]
            {
                new MenuItem("About..."),
            };

            Menu = new MainMenu();
            Menu.MenuItems.Add("File", fileMenu);
            Menu.MenuItems.Add("Help", helpMenu);
        }

        private void ResizeInterface(object sender, EventArgs e)
        {
            sceneControlPanel.Location = new Point(ClientSize.Width - sidePanelWidth, 0);
            sceneControlPanel.Size = new System.Drawing.Size(sidePanelWidth, ClientSize.Height);
            objectControlPanel.Location = new Point(ClientSize.Width - sidePanelWidth, 0);
            objectControlPanel.Size = new System.Drawing.Size(sidePanelWidth, ClientSize.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(System.Drawing.Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (Mode == EditorMode.EditObject) 
                DrawObject(g);
            else 
                DrawScene(g);
        }

        private void DrawScene(Graphics g)
        {
            if (sceneEditor.GrippedObject != null)
            {
                var obj = sceneEditor.GrippedObject;
                var location = obj.Body.Position - sceneEditor.Scene.Camera.Body.Position;
                var highlight = CreateHighlightRectangle(obj, location);
                var pen = new Pen(System.Drawing.Color.LightBlue, 10);
                g.DrawRectangle(pen, highlight);
            }
            
            g.DrawScene(sceneEditor.Scene, sceneEditor.SpriteSet);
            
            if (sceneEditor.GrippedObject != null)
            {
                var obj = sceneEditor.GrippedObject;
                var location = obj.Body.Position - sceneEditor.Scene.Camera.Body.Position;
                DrawCoordinates(g, location, sceneEditor.GrippedObject.Body.Position);
            }
        }
        
        private void DrawCoordinates(Graphics g, Vector2f location, Vector2f coordinates)
        {
            var text = String.Format("({0}, {1})", coordinates.X, coordinates.Y);
            var fontSize = 16;
            var locationPoint = new Point((int)location.X, (int)location.Y - fontSize - 20);
            g.DrawString(text, new System.Drawing.Font("Consolas", fontSize), Brushes.Black, locationPoint);
        }

        private void DrawObject(Graphics g)
        {
            var bitmap = ResourceManager.GetBitmap(objectEditor.TextureName);
            g.DrawObject(objectEditor.EditingObject, bitmap, objectEditor.Camera);
        }

        private Rectangle CreateHighlightRectangle(IGameObject obj, Vector2f location)
        {
            var locationPoint = new Point((int)location.X, (int)location.Y);
            var rectSize = obj.Body.Size;
            return new Rectangle(locationPoint, new Size((int)rectSize.X, (int)rectSize.Y));
        }
        
        public void SwitchMode(EditorMode newMode)
        {
            Mode = newMode;
            
            if (Mode == EditorMode.EditObject)
            {
                Controls.Remove(sceneControlPanel);
                Controls.Add(objectControlPanel);
            }
            else
            {
                Controls.Remove(objectControlPanel);
                Controls.Add(sceneControlPanel);
            }

            if (ModeSwitch != null) ModeSwitch(this, Mode);
        }
    }
}
