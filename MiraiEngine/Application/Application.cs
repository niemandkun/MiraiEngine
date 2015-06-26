using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace MiraiEngine
{
    public class Application
    {
        public RenderWindow Window { get; private set; }
        public ApplicationSettings Settings { get; private set; }
        private Stack<IApplicationState> appStates;

        public Application(ApplicationSettings settings, IApplicationState startState)
        {
            Settings = settings;

            Window = CreateWindow(settings);
            Window.Closed += (sender, args) => Window.Close();

            appStates = new Stack<IApplicationState>();
            appStates.Push(startState);
            startState.App = this;
            startState.OnEnable();
        }

        private RenderWindow CreateWindow(ApplicationSettings settings)
        {
            var windowStyle = settings.Fullscreen ? Styles.Fullscreen : Styles.Close;
            var window = new RenderWindow(settings.VideoMode, settings.WindowCaption, windowStyle);
            window.SetMouseCursorVisible(settings.MouseCursorVisible);
            window.SetVerticalSyncEnabled(settings.VerticalSyncEnabled);
            if (settings.FramerateLimit > 0)
                window.SetFramerateLimit(settings.FramerateLimit);
            return window;
        }

        public void Run()
        {
            while (Window.IsOpen())
            {
                appStates.Peek().OnMainLoop();
                Window.DispatchEvents();
            }
        }
        
        public void PushState(IApplicationState newState)
        {
            appStates.Peek().OnDisable();
            newState.App = this;
            appStates.Push(newState);
            newState.OnEnable();
        }

        public void PopState()
        {
            appStates.Pop().OnDisable();
            appStates.Peek().OnEnable();
        }

        public void ResetState(IApplicationState newState)
        {
            appStates.Peek().OnDisable();
            appStates.Clear();
            newState.App = this;
            appStates.Push(newState);
            newState.OnEnable();
        }
    }
}
