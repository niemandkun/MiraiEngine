using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiraiEngine
{
    public class LightContainer : IEnumerable<Light>
    {
        private HashSet<Light> lights;
        private Dictionary<Light, int> timeleft;
        private Dictionary<Light, int> totalTime;

        private const int FADE_RATE = 8;

        public LightContainer()
        {
            lights = new HashSet<Light>();
            timeleft = new Dictionary<Light, int>();
            totalTime = new Dictionary<Light, int>();
        }

        public void SpawnLight(Light light, int timeout)
        {
            lights.Add(light);
            timeleft[light] = timeout - 1;
            totalTime[light] = timeout;
        }

        public void Update()
        {
            UpdateBrightness();

            var toRemove = new List<Light>();

            foreach(var light in lights)
            {
                if (timeleft[light]-- < 0)
                {
                    timeleft.Remove(light);
                    totalTime.Remove(light);
                    toRemove.Add(light);
                }
            }

            foreach (var light in toRemove)
                lights.Remove(light);
        }

        private void UpdateBrightness()
        {
            foreach(var light in lights)
            {
                var c = light.Color;
                var fadeOutRate = timeleft[light] / (double)totalTime[light] * FADE_RATE;
                var fadeInRate = FADE_RATE - fadeOutRate;
                var fadeIn = 0xff * fadeInRate;
                var fadeOut = 0xff * fadeOutRate;
                c.A = (byte)(Math.Min(Math.Min(fadeIn, fadeOut), 0xff));
                light.Color = c;
            }
        }

        public IEnumerator<Light> GetEnumerator()
        {
            foreach (var light in lights)
                yield return light;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
