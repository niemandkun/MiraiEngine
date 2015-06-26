using System;
using SFML.Audio;
using SFML.Window;

namespace MiraiEngine
{
    public static class MusicExtension
    {
        public static void Play(this Music music, Vector2f position, int distance)
        {
            music.RelativeToListener = false;
            music.PlayCommon(position, distance);
        }

        public static void PlayRelativeToListener(this Music music, Vector2f position, int distance)
        {
            music.RelativeToListener = false;
            music.PlayCommon(position, distance);
        }

        private static void PlayCommon(this Music music, Vector2f position, int distance)
        {
            music.Position = new Vector3f(position.Y, 0, position.X);
            music.MinDistance = distance;
            music.Play();
        }
    }
}
