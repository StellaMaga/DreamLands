using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_03_Road_Of_Past(Player player)
        {
            string title = "[서브 이벤트 3: 과거로 돌아가는 길]";
            string bodyText = "익숙하고 그리운 길이 보인다.\n" +
                              "길 너머에는 추억이 담겨있는 과거의 풍경이 존재한다.\n" +
                              "모든 사람은 과거를 그리워하며, 그 어떤 기억도 시간이 지나면 미화된다.\n" +
                              "추억이 저 너머에 있다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 길을 따라 걸어가 본다 [DD 이성 판정]",
                opt2: "2. 길을 스쳐 지나간다",
                onOption1: (p, mgr) =>
                {
                    bool isSuccess = Game_System.DD_Check(12, "이성", p.Sanity, p, out string diceMsg);
                    if (isSuccess)
                    {
                        p.Mental += 4;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"걸음을 옮길 때마다 그리운 풍경들이 눈에 들어온다.\n보기만 해도 행복해지지만 더 나아가진 않는다.\n과거에는 그 누구도 존재하지 않음을 알기에.\n\n{diceMsg}"),
                            new(Game_System.Green("[정신력 +4]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                    else
                    {
                        p.Madness += 4;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"걸어갈수록 회의감만 늘어난다.\n뒤늦게 발길을 돌렸지만 너무 많은 시간을 소모했다.\n\n{diceMsg}"),
                            new(Game_System.Purple("[광기 +4]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                },
                onOption2: (p, mgr) =>
                {
                    if (!p.HasMystic("ColdHeart"))
                    {
                        p.ObtainMystic(new Mystic("ColdHeart", "냉혈한", "과거의 감상에 휘둘리지 않는 냉철한 심장.", MysticType.Mystic)
                        {
                            BonusWill = 3
                        });
                    }

                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("당신은 그저 스쳐 지나간다.\n추억이라 할 만한 것이 남지 않아서일까?\n저 길은 당신에게 그다지 매력적이지 않았다."),
                        new(Game_System.Green("[+ 신비 획득: 냉혈한 (의지 +3)]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }
    }
}