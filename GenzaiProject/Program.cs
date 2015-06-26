using System;
using MiraiEngine;
using SFML.Window;
using SFML.Graphics;

namespace GenzaiProject
{
	class Program
	{
        const string NAME = "Genzai";
        const string VERSION = "v0.0";

        [MTAThread]
        static void Main()
        {
            var settings = new ApplicationSettings()
            {
                WindowCaption = String.Format("{0} {1}", NAME, VERSION),
                FramerateLimit = 200,
                VideoMode = new VideoMode(1024, 600),
                Fullscreen = false,
                VerticalSyncEnabled = true,
                MouseCursorVisible = true,
                EnableLights = true,
            };
            
            var app = new Application(settings, new Init());
            app.Run();
        }
	}
}
