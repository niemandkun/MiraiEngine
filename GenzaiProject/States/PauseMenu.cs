using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Graphics;
using SFML.Window;

namespace GenzaiProject
{
    class PauseMenu : IApplicationState
    {
        public Application App { get; set; }

        string caption = "PAUSE";
        string[] menu = new string[] { "CONTINUE", "QUIT" };
        int currentSelection = 0;

        public void OnMainLoop()
        {
            var captionSprite = new Text(caption, ResourceManager.GetFont("BonvenoCF.otf"), (uint)(App.Window.Size.Y * 0.07));
            captionSprite.Position = new Vector2f(200, App.Window.Size.Y * 0.35f);

            var text = currentSelection != 0 ? "...\n" : "\n";
            text += menu[currentSelection];
            text += currentSelection != menu.Length - 1 ? "\n..." : "\n";
            
            var textSprite = new Text(text, ResourceManager.GetFont("BonvenoCF.otf"), (uint)(App.Window.Size.Y * 0.03));
            textSprite.Position = new Vector2f(220, App.Window.Size.Y * 0.45f);

            App.Window.Clear(Color.Black);
            App.Window.Draw(captionSprite);
            App.Window.Draw(textSprite);
            App.Window.Display();
        }

        public void OnEnable()
        {
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
                        App.PopState();
                        break;
                    case 1:
                        App.ResetState(new MainMenu());
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
    }
}
