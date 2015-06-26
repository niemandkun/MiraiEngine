using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using MiraiEngine;

namespace GenzaiProject
{
    class Sparks
    {
        public ParticlesFire Emitter;
        public int Timeout = 30;

        public Sparks(Vector2f position)
        {
            Emitter = new ParticlesFire(position, new Vector2f(0.01f, 0.01f), 2, 50, 30);
        }
    }

    class Bang
    {
        public ParticlesFire Emitter;
        public int Timeout = 60;

        public Bang(Vector2f position)
        {
            Emitter = new ParticlesFire(position, new Vector2f(0.1f, 0.1f), 2, 100, 60);
        }
    }
}
