using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace MiraiEngine
{
    public class KeyboardState : EventArgs
    {
        public HashSet<Keyboard.Key> Keys { get; private set; }
        public readonly bool RShift;
        public readonly bool RCtrl;
        public readonly bool RAlt;
        public readonly bool LShift;
        public readonly bool LCtrl;
        public readonly bool LAlt;

        public KeyboardState(HashSet<Keyboard.Key> keys)
        {
            Keys = keys;
            if (keys.Contains(Keyboard.Key.RShift))
                RShift = true;
            if (keys.Contains(Keyboard.Key.RControl))
                RCtrl = true;
            if (keys.Contains(Keyboard.Key.RAlt))
                RAlt = true;
            if (keys.Contains(Keyboard.Key.LShift))
                LShift = true;
            if (keys.Contains(Keyboard.Key.LControl))
                LCtrl = true;
            if (keys.Contains(Keyboard.Key.LAlt))
                LAlt = true;
        }
    }
}
