using System;
using System.IO;
using System.Collections.Generic;

using SFML.Audio;
using SFML.Graphics;


namespace MiraiEngine
{
    public static class ResourceManager
    {        
        private static Dictionary<string, Font> fontsDict;
        private static Dictionary<string, Texture> texturesDict;
        private static Dictionary<string, Sound> soundsDict;
        private static Dictionary<string, Music> musicDict;
        private static Dictionary<string, System.Drawing.Bitmap> bitmapDict;

        public static System.Drawing.Bitmap DefaultBitmap { get; private set; }
        public static Texture DefaultTexture { get; private set; }

        public static string PathToFonts { get; set; }
        public static string PathToTextures { get; set; }
        public static string PathToSounds { get; set; }
        public static string PathToMusic { get; set; }

        static ResourceManager()
        {
            PathToFonts = @"Resources\Fonts\";
            PathToMusic = @"Resources\Music\";
            PathToSounds = @"Resources\Sounds\";
            PathToTextures = @"Resources\Textures\";

            CleanResources();

            DefaultTexture = new Texture(new Image(128, 128, Color.Magenta));
            DefaultBitmap = ConvertTextureToBitmap(DefaultTexture);
        }

        public static void CleanResources()
        {
            bitmapDict = new Dictionary<string, System.Drawing.Bitmap>();
            fontsDict = new Dictionary<string, Font>();
            texturesDict = new Dictionary<string, Texture>();
            soundsDict = new Dictionary<string, Sound>();
            musicDict = new Dictionary<string, Music>();
        }

        public static Font GetFont(string name)
        {
            return GetOrException(name, fontsDict);
        }

        public static Texture GetTexture(string name)
        {
            Action<string> loader = n => LoadTextures(n);
            return GetOrLoad(name, texturesDict, loader, DefaultTexture);
        }

        public static System.Drawing.Bitmap GetBitmap(string name)
        {
            Action<string> loader = n => LoadBitmaps(n);
            return GetOrLoad(name, bitmapDict, loader, DefaultBitmap);
        }

        public static Sound GetSound(string name)
        {
            return GetOrException(name, soundsDict);
        }

        public static Music GetMusic(string name)
        {
            return GetOrException(name, musicDict);
        }

        private static T GetOrException<T>(string name, Dictionary<string, T> dict)
        {
            if (dict.ContainsKey(name))
                return dict[name];
            else
                throw new KeyNotFoundException("Resource is not loaded.");
        }

        private static T GetOrLoad<T>(string name, Dictionary<string, T> dict, Action<string> load, T defaultRes)
        {
            try
            {
                return GetOrException(name, dict);
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    load(name);
                    return GetOrException(name, dict);
                }
                catch
                {
                    return defaultRes;
                }
            }
        }

        public static void LoadTextures(params string[] textures)
        {
            Func<string, Texture> build = s => new Texture(s);
            LoadResource(PathToTextures, textures, texturesDict, build);
        }

        public static void LoadBitmaps(params string[] bitmaps)
        {
            Func<string, System.Drawing.Bitmap> build = s =>
                {
                    var image = System.Drawing.Image.FromFile(s);
                    return new System.Drawing.Bitmap(image);
                };
            LoadResource(PathToTextures, bitmaps, bitmapDict, build);
        }

        public static void LoadMusic(params string[] music)
        {
            Func<string, Music> build = s => new Music(s);
            LoadResource(PathToMusic, music, musicDict, build);
        }

        public static void LoadSounds(params string[] sounds)
        {
            Func<string, Sound> build = s => new Sound(new SoundBuffer(s));
            LoadResource(PathToSounds, sounds, soundsDict, build);
        }
        
        public static void LoadFonts(params string[] fonts)
        {
            Func<string, Font> build = s => new Font(s);
            LoadResource(PathToFonts, fonts, fontsDict, build);
        }

        private static void LoadResource<T>(string path, IEnumerable<string> names, 
            Dictionary<string, T> destination, Func<string, T> build)
        {
            foreach(var name in names)
            {
                var fullname = path + name;
                var buildRes = build(fullname);
                
                if (typeof(T) == typeof(Texture))
                {
                    var texture = buildRes as Texture;
                    texture.Smooth = true;
                }

                destination.Add(name, buildRes);
            }
        }

        public static System.Drawing.Bitmap ConvertTextureToBitmap(SFML.Graphics.Texture texture)
        {
            var image = texture.CopyToImage();
            var size = image.Size;
            var bitmap = new System.Drawing.Bitmap((int)size.X, (int)size.Y, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            for (var x = 0; x < size.X; ++x)
                for (var y = 0; y < size.Y; ++y)
                {
                    var p = image.GetPixel((uint)x, (uint)y);
                    bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(p.A, p.R, p.G, p.B));
                }

            return bitmap;
        }
    }
}
