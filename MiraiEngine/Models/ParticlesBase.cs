using System;
using System.Collections.Generic;
using System.Linq;

using SFML.Window;
using SFML.Graphics;


namespace MiraiEngine
{
    public class Particle
    {
        public Vector2f Velocity;
        public int Lifetime;
    }

    public class ParticlesBase : Drawable
    {
        public Vector2f EmitterPosition { get; set; }

        public int Count { get { return ParticleArray.Length; } }
        public int Lifetime { get; set; }
        public float Speed { get; set; }
        protected Particle[] ParticleArray;

        VertexArray vertices;
        Random rand = new Random();

        public ParticlesBase(Vector2f position, int count, float speed, int lifetime)
        {
            Lifetime = lifetime;
            Speed = speed;
            EmitterPosition = position;

            vertices = new VertexArray(PrimitiveType.Points, (uint)count);
            ParticleArray = new Particle[count];

            for (var i = 0; i < count; ++i)
            {
                ParticleArray[i] = new Particle() { Lifetime = this.Lifetime };
                ResetParticle(i);
            }
        }

        public virtual void Update()
        {
            var rand = new Random();

            for (var i = 0; i < Count; ++i)
            {
                var particle = ParticleArray[i];

                if (--particle.Lifetime < 0) ResetParticle(i);

                var point = vertices[(uint)i];
                point.Position += particle.Velocity;
                point.Color.A = (byte)(particle.Lifetime / (double)Lifetime * 255);
                vertices[(uint)i] = point;
            }
        }

        protected virtual void ResetParticle(int index)
        {
            var particle = ParticleArray[index];

            var angle = (rand.Next() % 360) / 180.0 * Math.PI;
            var speed = (float)rand.NextDouble() * Speed;

            particle.Lifetime = Lifetime;
            particle.Velocity = new Vector2f(speed, speed).Rotate(angle);

            var point = vertices[(uint)index];
            point.Position = EmitterPosition;
            vertices[(uint)index] = point;
        }

        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(vertices, states);
        }
    }
}
