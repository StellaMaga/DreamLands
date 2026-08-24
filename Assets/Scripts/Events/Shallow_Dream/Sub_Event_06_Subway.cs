using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_06_Subway(Player player)
        {
            string title = "[서브 이벤트 6: 저승으로 가는 지하철]";
            string bodyText = "안개가 짙게 낀 지하철역, 흐릿한 형체들의 사람들이 서 있다.\n" +
                              "열차가 들어오자 사람들이 밀려 들어가며 거대한 인파의 흐름을 만든다.\n" +
                              "그 흐름은 너무나 강렬하여 당신마저 휩쓸어버릴 것 같다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 인파에서 벗어난다 [DD 거부 판정]",
                opt2: null,
                onOption1: (p, mgr) =>
                {
                    bool isSuccess = Game_System.DD_Check(13, "거부", p.Rejection, p, out string diceMsg);
                    if (isSuccess)
                    {
                        p.Will += 5;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"강을 거스르는 연어처럼 사람들을 밀쳐내고 벗어났다.\n지하철은 다음 정거장을 향해 어둠 속으로 출발했다.\n\n{diceMsg}"),
                            new(Game_System.Green("[의지 +5 (거부 증가)]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                    else
                    {
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"흐름에 저항하지 못하고 지하철 속으로 빨려 들어갔다.\n열차는 그대로 종착역인 저승을 향해 달린다...\n\n{diceMsg}"),
                            new(Game_System.Red("[사망: 저승행 열차에 탑승했습니다]"))
                        };
                        p.Die(DeathType.Give_Up);
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.CheckPlayerDeath()));
                    }
                }
            );
        }
    }
}