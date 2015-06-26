using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiraiEngine;
using SFML.Graphics;

namespace GenzaiProject
{
    class Init : IApplicationState
    {
        public Application App {get; set;}

        public void OnMainLoop()
        {
            App.Window.Clear(Color.Black);
            App.Window.Display();
        }

        public void OnEnable()
        {
            ResourceManager.LoadFonts("BonvenoCF.otf");
            ObjectsManager.LoadDefaultFolder();
            App.PushState(new MainMenu());
        }

        public void OnDisable()
        {
            // pass
        }
    }
}
