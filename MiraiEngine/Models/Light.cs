using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;


namespace MiraiEngine
{
    public enum LightMode
    {
        Flashlight,
        Backlight,
    }

    public class Light : Drawable
    {
        private Sprite light;
        public LightMode Mode { get; set; }

        public Vector2f Position 
        {
            get { return light.Position; }
            set { light.Position = value; }
        }
        
        public Vector2f Scale 
        {
            get { return light.Scale; }
            set { light.Scale = value; }
        }
        
        public Color Color
        {
            get { return light.Color; }
            set { light.Color = value; }
        }

        private Vector2u textureSize
        {
            get { return light.Texture.Size; }
        }

        public Light(Vector2f scale, Color color, LightMode mode)
        {
            light = new Sprite(ResourceManager.GetTexture("light.png"));
            light.Texture.Smooth = true;
            Scale = scale;
            Color = color;
            Position = new Vector2f();
            Mode = mode;
        }

        public Light(Vector2f position, Vector2f scale, Color color, LightMode mode)
            : this(scale, color, mode)
        {
            Position = position;
        }

        public Light(Vector2f scale, LightMode mode) : this(scale, Color.White, mode) { }
        public Light(Vector2f position, Vector2f scale, LightMode mode)
            : this(position, scale, Color.White, mode) { }
 
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.BlendMode = BlendMode.Add;
            var offset = new Vector2f(textureSize.X * Scale.X, textureSize.Y * Scale.Y) / 2;
            states.Transform.Translate(-offset);
            target.Draw(light, states);
        }
    }
}
