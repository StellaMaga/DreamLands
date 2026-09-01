using System;
using UnityEngine;
using TextGame.Core;

namespace TextGame.Core
{
    public class EventStep
    {
        public string Title;
        public string BodyText;
        public string Option1Text;
        public string Option2Text;
        public string Option3Text; // 3번 선택지 추가

        public Action<Player, GameManager> OnOption1;
        public Action<Player, GameManager> OnOption2;
        public Action<Player, GameManager> OnOption3; // 3번 액션 추가

        public EventStep(string title, string bodyText,
                         string opt1 = "다음 ▶", string opt2 = null, string opt3 = null,
                         Action<Player, GameManager> onOption1 = null,
                         Action<Player, GameManager> onOption2 = null,
                         Action<Player, GameManager> onOption3 = null)
        {
            Title = title;
            BodyText = bodyText;
            Option1Text = opt1;
            Option2Text = opt2;
            Option3Text = opt3;
            OnOption1 = onOption1;
            OnOption2 = onOption2;
            OnOption3 = onOption3;
        }

        public static EventStep Multi_Step(string title, DialogueNode[] dialogues, Action<Player, GameManager> onComplete)
        {
            return Step_Helper(title, dialogues, 0, onComplete);
        }

        private static EventStep Step_Helper(string title, DialogueNode[] dialogues, int index, Action<Player, GameManager> onComplete)
        {
            var node = dialogues[index];
            bool isLast = index >= dialogues.Length - 1;

            string formattedBody = string.IsNullOrEmpty(node.Speaker)
                ? node.Text
                : $"{node.Speaker}\n{node.Text}";

            return new EventStep(
                title: title,
                bodyText: formattedBody,
                opt1: isLast ? "확인" : "다음 ▶",
                opt2: null,
                opt3: null,
                onOption1: (player, manager) =>
                {
                    if (isLast)
                    {
                        onComplete?.Invoke(player, manager);
                    }
                    else
                    {
                        manager.Execute_EventStep(Step_Helper(title, dialogues, index + 1, onComplete));
                    }
                }
            );
        }
    }
}

namespace TextGame.Events.Shallow_Dream
{
    // [서브 이벤트 메인 클래스 - partial]
    public partial class Sub_Events
    {
        private static System.Random rand = new System.Random();

        public static EventStep Random_Event(Player player)
        {
            int eventIndex = rand.Next(1, 14);

            switch (eventIndex)
            {
                case 1: return Sub_Event_01_Floret();
                case 2: return Sub_Event_02_Butterfly(player);
                case 3: return Sub_Event_03_Road_Of_Past(player);
                case 4: return Sub_Event_04_entireness_orb(player);
                case 5: return Sub_Event_05_Three_Trees(player);
                case 6: return Sub_Event_06_Subway(player);
                case 7: return Sub_Event_07_Lost_Name(player);
                case 8: return Sub_Event_08_Blood_Bathtub(player);
                case 9: return Sub_Event_09_Violin(player);
                case 10: return Sub_Event_10_Picture(player);
                case 11: return Sub_Event_11_Kraken(player);
                case 12: return Sub_Event_12_Smoker(player);
                case 13: return Sub_Event_13_Suicide_Theory();
                default: return Sub_Event_01_Floret();
            }
        }
    }
}