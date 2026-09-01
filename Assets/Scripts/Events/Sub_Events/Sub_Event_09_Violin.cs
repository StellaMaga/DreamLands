using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_09_Violin(Player player)
        {
            string title = "[서브 이벤트 9: 바이올린]";
            string bodyText = "고요한 폐허 속, 듣기 좋은 바이올린 선율이 울려 퍼진다.\n" +
                              "소리의 중심에는 금이 가고 닳아있는 해골이 서 있다.\n" +
                              "금방이라도 무너질 듯한 모습이지만, 해골은 쉬지 않고 연주를 이어간다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 안식을 내려준다",
                opt2: "2. 훌륭한 음악에 감탄을 바친다",
                onOption1: (p, mgr) =>
                {
                    p.Starlight += 10;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("해골을 붙잡고 있던 지독한 미련을 보았다.\n당신은 손수 해골을 부수어 영원한 안식을 내려주었다."),
                        new(Game_System.Green("[별빛 +10]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    p.Sanity += 5;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("무너져가면서도 연주를 이어가는 집념과 환희에 경의를 표한다.\n문득 해골이 미소 짓고 있는 것처럼 느껴졌다."),
                        new(Game_System.Green("[이성 +5]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }
    }
}