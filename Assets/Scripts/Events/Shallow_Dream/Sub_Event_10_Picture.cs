using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_10_Picture(Player player)
        {
            string title = "[서브 이벤트 10: 박제된 영원한 순간]";
            string bodyText = "노래하는 새들과 피어나는 꽃, 자전거 타는 아이들.\n" +
                              "모든 것이 정지된 흑백의 도시.\n" +
                              "마치 가장 평화로웠던 한순간을 박제해 둔 듯한 풍경이다.";

            int reqDetection = 47;
            string opt2Text = Game_System.FormatOption("2. 공간의 중심으로 다가간다", "탐지", player.Detection, reqDetection);

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 고요한 도시를 둘러본다",
                opt2: opt2Text,
                onOption1: (p, mgr) =>
                {
                    p.Sanity += 4;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("과거 유럽의 한 도시 같은 정적 속을 거닌다.\n이 순간은 영원히 박제되어 잊히지 않을 것이다."),
                        new(Game_System.Green("[이성 +4]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    if (Game_System.Has_Stat(p.Detection, reqDetection))
                    {
                        if (!p.HasItem("FrozenMomentPhoto"))
                        {
                            p.ObtainItem(new Mystic("FrozenMomentPhoto", "박제된 영원한 순간", "시간이 멈춘 도시의 원형이 담긴 흑백 사진.", MysticType.Item)
                            {
                                BonusSanity = 4,
                                BonusWill = 3
                            });
                        }

                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new("박제되지 않은 유일한 기척을 쫓아 작은 오두막에 도달했다.\n그곳에는 이 도시의 원형으로 보이는 사진이 놓여있었다."),
                            new(Game_System.Green("[+ 아이템 획득: 박제된 영원한 순간]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                    else
                    {
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new("공간의 중심을 찾아보려 했으나 희미한 기척을 놓치고 말았다.")
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                }
            );
        }
    }
}