using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Main_Events
    {
        // 1. [메인 이벤트: 오프닝]
        public static EventStep Shallow_Dream(Player player)
        {
            DialogueNode[] dialogues = new DialogueNode[]
            {
                new($"""
                    당신은 한치 앞도 안 보이는 안개 속에서 깨어났다.
                    기억을 더듬어 보니 흰 토끼가 만든 구멍에 떨어지면서 정신을 잃었던 것이 기억난다.

                    무심코 하늘을 올려다 봤지만 아무것도 보이지 않았다. 
                    오직 안개만이 당신을 감싸고 있었다.

                    솔직히 아직도 이 모든 상황이 당황스럽다.
                    기묘한 꿈을 꾸다가 기묘한 세계로 떨어졌으니 말이다.
                    지금 이 순간도 현실인지 꿈인지 알 수 있는 방법이 없었다.
                    그러나 확실한 것은 바로 앞으로 나아가야 한다는 것이다.

                    {Game_System.Green("[의지 +5]")}
                    """)
            };

            return EventStep.Multi_Step(
                title: "[ 1부 오프닝 : 얕은 꿈의 경계 ]",
                dialogues: dialogues,
                onComplete: (p, mgr) =>
                {
                    p.Will += 5;
                    mgr.Show_Main_UI();
                }
            );
        }
    }
}