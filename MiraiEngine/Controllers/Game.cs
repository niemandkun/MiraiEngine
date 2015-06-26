using System;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using MiraiEngine;


namespace MiraiEngine
{
    public class Game : IApplicationState
    {
        public Application App { get; set; }
        public HashSet<Keyboard.Key> KeysPressed { get; private set; }
        public int TickInterval { get; private set; }

        private Mutex logicLock;
        private System.Timers.Timer timer;
        private Render render;

        public Scene Scene { get; private set; }
        public GameView View { get; private set; }
        public GameSettings Settings { get; set; }

        public Game(GameSettings settings, Scene scene, Application app)
        {
            App = app;

            Settings = settings;
            TickInterval = settings.TickInterval;

            Scene = scene;
            Scene.Camera.LockOffset = Settings.CameraLockOffset;
            Scene.PhysicsQuality = Settings.PhysicsQuality;
            Scene.Camera.ReactionSpeed = Settings.CameraSpeed;
            Scene.Gravity = Settings.Gravity;
            if (Settings.PhysicsStaticResistance != 0)
                Scene.Physics.StaticResistance = Settings.PhysicsStaticResistance;

            View = new GameView();
            if (!String.IsNullOrEmpty(Settings.Background))
            {
                var bgTexture = ResourceManager.GetTexture(Settings.Background);
                bgTexture.Repeated = true;
                View.Background = bgTexture;
            }
            
            View.SpriteSet = Settings.Textures
                .ToDictionary(kv => kv.Key, kv => new Sprite(ResourceManager.GetTexture(kv.Value)));
            View.SpriteSet[0] = new Sprite();

            logicLock = new Mutex();
            KeysPressed = new HashSet<Keyboard.Key>();

            timer = new System.Timers.Timer(TickInterval);
            timer.Elapsed += Act;
            timer.AutoReset = true;

            Listener.GlobalVolume = 100;
            Listener.Direction = new Vector3f(1, 0, 0);

            render = new Render(App.Window, App.Settings.EnableLights);
        }

        public void OnMainLoop()
        {
            logicLock.WaitOne();
            render.RenderScene(Scene, View);
            logicLock.ReleaseMutex();
        }

        private void Act(object sender, ElapsedEventArgs e)
        {
            Listener.Position = new Vector3f(Scene.Camera.Body.Position.Y, 0, Scene.Camera.Body.Position.X);

            logicLock.WaitOne();
            Scene.UpdateWorld(KeysPressed);
            logicLock.ReleaseMutex();
        }

        public void OnEnable()
        {
            App.Window.KeyPressed += KeyDown;
            App.Window.KeyReleased += KeyUp;
            timer.Start();
        }

        public void OnDisable()
        {
            timer.Stop();
            App.Window.KeyPressed -= KeyDown;
            App.Window.KeyReleased -= KeyUp;
            KeysPressed.Clear();
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            KeysPressed.Add(e.Code);
            if (Settings.Keybindings.ContainsKey(e.Code))
                Settings.Keybindings[e.Code](this);

            Scene.ProcessKey(e);
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            KeysPressed.Remove(e.Code);
        }
    }
}
