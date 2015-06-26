using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class ApplicationSettings
    {
        public string WindowCaption { get; set; }
        public VideoMode VideoMode { get; set; }
        public uint FramerateLimit { get; set; }

        public bool Fullscreen { get; set; }
        public bool MouseCursorVisible { get; set; }
        public bool VerticalSyncEnabled { get; set; }
        public bool EnableLights { get; set; }
    }
}
