using SFML.Graphics;

namespace MiraiEngine
{
    public static class SpriteExtension
    {
        public static void ReflectHorizontally(this Sprite self)
        {
            var rect = self.TextureRect;
            self.TextureRect = new IntRect(rect.Left + rect.Width, rect.Top, -rect.Width, rect.Height);
        }
    }
}
