using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_12_Smoker(Player player)
        {
            string title = "[서브 이벤트 12: 흡연자]";
            string bodyText = "담배 연기가 안개를 밀어낼 정도로 가득한 곳에서 한 남자가 담배를 피우고 있다.\n\n" +
                              "'여기선 암 걸릴 걱정 없이 피워도 된다는 걸 너무 늦게 알았지.'\n" +
                              "남자의 몸은 부분부분 재가 되어 부스러지고 있었다.\n\n" +
                              "'이것도 인연인데 한 대 피우겠나?'";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 돛대를 받아들인다",
                opt2: "2. 비흡연자이다",
                onOption1: (p, mgr) =>
                {
                    p.Sanity -= 4;
                    p.Mental += 8;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("불을 붙이고 연기를 들이마신다. 남자의 마지막 미련이 타들어 간다.\n담배가 다 타자 자리에는 재만이 남아있었다."),
                        new(Game_System.Red("[이성 -4]") + " | " + Game_System.Green("[정신력 +8]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    if (!p.HasItem("LastCigarette"))
                    {
                        p.ObtainItem(new Mystic("LastCigarette", "돛대", "남자가 남기고 간 마지막 담배 한 개비.", MysticType.Item)
                        {
                            BonusLuck = 2,
                            BonusMental = 3
                        });
                    }

                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("'아쉽구만. 이왕 이렇게 된 거 아래층으로 도전이나 가봐야겠어.'\n남자는 담뱃갑을 던져주고 안개 속으로 사라졌다."),
                        new(Game_System.Green("[+ 아이템 획득: 돛대]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }
    }
}