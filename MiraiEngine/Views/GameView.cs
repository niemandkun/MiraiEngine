using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public class GameView
    {
        public Texture Background { get; set; }
        public View View { get; set; }
        public Dictionary<uint, Sprite> SpriteSet { get; internal set; }

        public GameView()
        {
            Background = ResourceManager.DefaultTexture;
            View = new View();
            SpriteSet = new Dictionary<uint, Sprite>();
        }
    }
}
