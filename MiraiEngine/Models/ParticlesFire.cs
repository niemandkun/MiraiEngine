using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;

namespace MiraiEngine
{
    public class ParticlesFire : ParticlesBase
    {
        Light[] lights;

        public ParticlesFire(Vector2f position, Vector2f scale, float speed, int count, int lifetime)
            : base(position, count, speed, lifetime)
        {
            lights = new Light[count];
            for (var i = 0; i < count; ++i)
            {
                lights[i] = new Light(scale, Color.Yellow, LightMode.Flashlight);
                ResetParticle(i);
            }
        }

        public override void Update()
        {
            base.Update();

            var rand = new Random();

            for (var i = 0; i < Count; ++i)
            {
                var particle = ParticleArray[i];

                lights[i].Position += particle.Velocity;
                var percentage = particle.Lifetime / (double)Lifetime;
                var alpha = (byte)(percentage * 0xff);
                var green = alpha;
                var blue = (byte)Math.Max(alpha - 0x99, 0);
                lights[i].Color = new Color(0xff, green, blue, alpha);
            }
        }

        protected override void ResetParticle(int index)
        {
            base.ResetParticle(index);
            
            if (lights != null)
                lights[index].Position = EmitterPosition;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var light in lights)
                target.Draw(light, states);
        }
    }
}
