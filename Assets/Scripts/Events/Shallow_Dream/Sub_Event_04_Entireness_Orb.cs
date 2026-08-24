using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_04_entireness_orb(Player player)
        {
            string title = "[서브 이벤트 4: 완전무결한 구]";
            string bodyText = "무엇 하나 없는 새하얀 공간이다.\n" +
                              "안개도, 소음도, 티끌조차 존재하지 않는다.\n" +
                              "오직 공중에 떠 있는 하나의 구체만이 존재할 뿐이다.\n" +
                              "빛조차 흠을 내지 못하는 완전하고 무결한 구체다.";

            int reqInterference = 66;
            string opt3Text = Game_System.FormatOption("3. 그것은 완벽하지 않다", "간섭", player.Interference, reqInterference);

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 구체에 손을 올린다",
                opt2: "2. 바라만 본다",
                onOption1: (p, mgr) =>
                {
                    p.Mental -= 8;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("손이 닿자 파문이 일어나며 완전무결함이 깨져나갔다.\n완벽이 무너지자 남은 것은 혐오스러운 혼돈뿐이었고,\n혼돈은 완벽을 파괴한 당신을 용서하지 않는다."),
                        new(Game_System.Red("[정신력 -8]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    p.Sanity += 3;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("경이로운 예술품을 보듯 바라만 본다.\n예술을 완성하는 것은 관객의 시선이며, 이로써 구는 진정으로 완벽해졌다."),
                        new(Game_System.Green("[이성 +3]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }

        // 3번 선택지 전용 분기 (간섭 >= 66 해금)
        public static EventStep Sub_Event_04_Option3(Player player)
        {
            string title = "[서브 이벤트 4: 완전무결한 구]";
            if (Game_System.Has_Stat(player.Interference, 66))
            {
                if (!player.HasItem("TraceOfPerfection"))
                {
                    player.ObtainItem(new Mystic("TraceOfPerfection", "완벽의 흔적", "부정당한 완벽이 남긴 울퉁불퉁한 원석.", MysticType.Item)
                    {
                        BonusWill = 5
                    });
                }

                DialogueNode[] nodes = new DialogueNode[]
                {
                    new("완벽은 완성을 의미하며, 완성은 더 나아가지 못함을 의미한다.\n당신은 그것의 완벽을 단호히 부정했다."),
                    new("존재를 부정당한 구체가 사라지고, 그 자리에는 울퉁불퉁한 돌만이 남아있었다.\n\n" + Game_System.Green("[+ 아이템 획득: 완벽의 흔적]"))
                };
                return EventStep.Multi_Step(title, nodes, (p, mgr) => mgr.Show_Main_UI());
            }
            return null;
        }
    }
}