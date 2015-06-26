using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class ParticlesContainer : IEnumerable<ParticlesBase>
    {
        private HashSet<ParticlesBase> emitters;
        private Dictionary<ParticlesBase, int> timeleft;

        public ParticlesContainer()
        {
            emitters = new HashSet<ParticlesBase>();
            timeleft = new Dictionary<ParticlesBase, int>();
        }

        public void SpawnEmitter(ParticlesBase emitter, int timeout)
        {
            emitters.Add(emitter);
            timeleft[emitter] = timeout - 1;
        }

        public void Update()
        {
            var toRemove = new List<ParticlesBase>();

            foreach(var emitter in emitters)
            {
                emitter.Update();
                if (timeleft[emitter]-- < 0)
                {
                    timeleft.Remove(emitter);
                    toRemove.Add(emitter);
                }
            }

            foreach (var emitter in toRemove)
                emitters.Remove(emitter);
        }

        public IEnumerator<ParticlesBase> GetEnumerator()
        {
            foreach (var emitter in emitters)
                yield return emitter;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
