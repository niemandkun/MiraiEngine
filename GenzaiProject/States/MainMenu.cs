using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

using MiraiEngine;


namespace GenzaiProject
{
    class MainMenu : IApplicationState
    {
        public Application App { get; set; }
        private GameSettings gameSettings = new GameSettings()
        {
            DebugMode = true,
            TickInterval = 1000 / 65,
            Keybindings = new Dictionary<Keyboard.Key, Action<Game>>()
            {
                { Keyboard.Key.Escape, game => game.App.PushState(new PauseMenu()) },
            },
            CameraSpeed = 0.4f,
            Background = "classic_bg.jpg",
            Textures = new Dictionary<ObjectID, string>()
            {
                { ObjectID.Friendly, "friendly.png" },
                { ObjectID.GreenShip, "ship.png" },
                { ObjectID.Laser, "laser.png" },
                { ObjectID.Player, "player_ship.png" },
                { ObjectID.Turret, "turret.png" },
                { ObjectID.Wall, "wall.png" },
            }.ToDictionary(kv => (uint)kv.Key, kv => kv.Value),
        };

        string caption = "GENZAI PROJECT";
        string[] menu = new string[] { "EXTERIOR TEST", "FLIGHT TEST", "QUIT" };
        int currentSelection = 0;
        int textureIndex = 0;
        Random rand = new Random();

        public void OnMainLoop()
        {
            var captionSprite = new Text(caption, ResourceManager.GetFont("BonvenoCF.otf"), (uint)(App.Window.Size.Y * 0.07));
            captionSprite.Position = new Vector2f(200, App.Window.Size.Y * 0.35f);

            var text = currentSelection != 0 ? "^\n" : "\n";
            text += menu[currentSelection];
            text += currentSelection != menu.Length - 1 ? "\nv" : "\n";
            
            var textSprite = new Text(text, ResourceManager.GetFont("BonvenoCF.otf"), (uint)(App.Window.Size.Y * 0.03));
            textSprite.Position = new Vector2f(220, App.Window.Size.Y * 0.45f);

            var sprite = new Sprite(ResourceManager.GetTexture(gameSettings.Textures[(uint)textureIndex]));
            sprite.Position = new Vector2f(App.Window.Size.X * 0.7f, App.Window.Size.Y * 0.54f);
            sprite.Rotation = -90;
 
            App.Window.Clear(Color.Black);
            App.Window.Draw(captionSprite);
            App.Window.Draw(textSprite);
            App.Window.Draw(sprite);
            App.Window.Display();
        }

        public void OnEnable()
        {
            textureIndex = rand.Next(1, 6);
            App.Window.KeyPressed += KeyHandler;
        }

        public void OnDisable()
        {
            App.Window.KeyPressed -= KeyHandler;
        }

        private void KeyHandler(object sender, KeyEventArgs args)
        {
            if (args.Code == Keyboard.Key.Return)
                switch (currentSelection)
                {
                    case 0:
                        App.PushState(new Game(gameSettings, LoadExteriorTest(), App));
                        break;
                    case 1:
                        App.PushState(new Game(gameSettings, LoadFlightTest(), App));
                        break;
                    default:
                        App.Window.Close();
                        break;
                }

            if (args.Code == Keyboard.Key.Up && currentSelection > 0) 
                currentSelection--;
            if (args.Code == Keyboard.Key.Down && currentSelection < menu.Length - 1) 
                currentSelection++;
        }

        private Scene LoadExteriorTest()
        {
            var scene = SceneManager.LoadFromFile("testscene.mss");
            return PrepareScene(scene);
        }

        private Scene LoadFlightTest()
        {
            var scene = new Scene(new Vector2f(Int16.MaxValue * 2, 768));
            scene.CreateCamera();

            var player = ObjectsManager.Build((uint)ObjectID.Player, new Vector2f(256, 300));
            scene.Add(player);

            for (var x = 0; x < scene.Size.X; x += 512)
            {
                var upperWall = ObjectsManager.Build((uint)ObjectID.Wall, new Vector2f(x, 0));
                var bottomWall = ObjectsManager.Build((uint)ObjectID.Wall, new Vector2f(x, scene.Size.Y - 128));
                scene.Add(upperWall);
                scene.Add(bottomWall);
            }

            for (var x = 500; x < 2000; x += 256)
            {
                scene.Add(ObjectsManager.Build((uint)ObjectID.GreenShip, new Vector2f(x, 128 + 64)));
                scene.Add(ObjectsManager.Build((uint)ObjectID.GreenShip, new Vector2f(x, scene.Size.Y - 64 - 256)));
            }

            scene.Commit();
            return PrepareScene(scene);
        }

        private Scene PrepareScene(Scene scene)
        {
            var player = scene.GetByID((uint)ObjectID.Player).First();
            var playerScript = new PlayerScript();
            player.BindScript(playerScript);
            player.Lighting.Add(new Light(new Vector2f(2.0f, 2.0f), 
                new Color(0xff, 0xff, 0xff, 0xff), LightMode.Backlight));

            var camera = scene.Camera;
            camera.Lock(player);
            camera.BindScript(new CameraScript());

            var turrets = scene.GetByID((uint)ObjectID.Turret);
            foreach (var turret in turrets)
                turret.BindScript(new TurretScript());

            foreach (var obj in scene.Where(x => x.ID != (uint)ObjectID.Player))
            {
                var light = new Light(new Vector2f(0.6f, 0.6f), Color.White, LightMode.Backlight);
                obj.Lighting.Add(light);
            }

            return scene;
        }
    }
}
