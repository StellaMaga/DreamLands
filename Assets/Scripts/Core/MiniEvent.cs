using System;
using TextGame.Core;

namespace TextGame.Core
{
    public class MiniEvent
    {
        public string Text { get; set; }
        public Action<Player> OnEffect { get; set; }

        public MiniEvent(string text, Action<Player> onEffect = null)
        {
            this.Text = text;
            this.OnEffect = onEffect;
        }
    }
}