using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_05_Three_Trees(Player player)
        {
            string title = "[서브 이벤트 5: 조각배 위의 3그루의 나무]";
            string bodyText = "넓은 강 위에 조각배 하나가 천천히 다가온다.\n" +
                              "배 위에는 3그루의 나무가 서 있다.\n" +
                              "첫 번째는 메말라 있고, 두 번째는 뒤틀려 있으며, 세 번째는 병들어 있다.\n" +
                              "당신은 각각의 나무가 무엇을 원하는지 직감한다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 첫 번째 나뭇가지에 손을 올린다",
                opt2: "2. 두 번째 뿌리에 손을 올린다",
                onOption1: (p, mgr) =>
                {
                    p.Mental -= 4;
                    // 거부 파생치 상향을 위해 의지/이성 조정
                    p.Will += 7;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("가시가 손바닥을 꿰뚫고 피를 게걸스럽게 빨아간다.\n쓰러질 때쯤 첫 번째 나무가 열매를 내민다. 강인한 육신에 대한 대가다."),
                        new(Game_System.Red("[정신력 -4]") + " | " + Game_System.Green("[의지 +7 (거부 상승)]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    p.Will -= 4;
                    p.Mental += 7; // 간섭 파생치 증가
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("뿌리가 심장에 파고들어 따스한 온기를 흡수한다.\n입김이 나올 때쯤 두 번째 나무가 열매를 내민다. 따뜻한 마음에 대한 대가다."),
                        new(Game_System.Red("[의지 -4]") + " | " + Game_System.Green("[정신력 +7 (간섭 상승)]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }

        // 세 번째 나무 선택지
        public static EventStep Sub_Event_05_Option3(Player player)
        {
            string title = "[서브 이벤트 5: 세 번째 나무]";
            player.Sanity -= 4;
            player.Mental += 7; // 탐지 파생치 증가

            DialogueNode[] nodes = new DialogueNode[]
            {
                new("나무 몸통에 손을 올리자 정신이 이어진다.\n병든 나무의 내면을 필사적으로 치료하자 나무가 열매를 내민다. 올곧은 정신에 대한 대가다."),
                new(Game_System.Red("[이성 -4]") + " | " + Game_System.Green("[정신력 +7 (탐지 상승)]"))
            };
            return EventStep.Multi_Step(title, nodes, (p, mgr) => mgr.Show_Main_UI());
        }
    }
}