using System;
using SFML.Audio;
using SFML.Window;

namespace MiraiEngine
{
    public static class SoundExtension
    {
        public static void Play(this Sound sound, Vector2f position, int distance)
        {
            sound.RelativeToListener = false;
            sound.PlayCommon(position, distance);
        }

        public static void PlayRelativeToListener(this Sound sound, Vector2f position, int distance)
        {
            sound.RelativeToListener = true;
            sound.PlayCommon(position,distance);
        }

        private static void PlayCommon(this Sound sound, Vector2f position, int distance)
        {
            sound.Position = new Vector3f(position.Y, 0, position.X);
            sound.MinDistance = distance;
            sound.Play();
        }
    }   
}
