using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class GameSettings
    {
        public int TickInterval { get; set; }
        //public GUI HeadUpDisplay { get; set; }
        public bool DebugMode { get; set; }
        public float CameraSpeed { get; set; }
        public uint PhysicsQuality { get; set; }
        public float PhysicsStaticResistance { get; set; }
        public Vector2f Gravity { get; set; }
        public Vector2f CameraLockOffset { get; set; }
        public Dictionary<Keyboard.Key, Action<Game>> Keybindings { get; set; }
        public Dictionary<uint, string> Textures { get; set; }
        public string Background { get; set; }

        public GameSettings()
        {
            TickInterval = 15;
            PhysicsQuality = 4;
            CameraSpeed = 1;
            Keybindings = new Dictionary<Keyboard.Key, Action<Game>>();
            Textures = new Dictionary<uint, string>();
        }
    }
}
