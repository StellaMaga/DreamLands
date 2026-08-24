using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_11_Kraken(Player player)
        {
            string title = "[서브 이벤트 11: 빙판 아래 무언가]";
            string bodyText = "바닥이 어느새 투명한 빙판으로 변해 있었다.\n" +
                              "빙판 아래 안개가 걷히자, 집채만 한 거대한 눈동자가 시선을 마주한다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 필사적으로 숨을 참는다 [이성 판정]",
                opt2: null,
                onOption1: (p, mgr) =>
                {
                    bool isSuccess = Game_System.Stat_Check("이성", 50, p.Sanity, out string checkMsg);
                    if (isSuccess)
                    {
                        p.Mental += 5;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"크게 심호흡을 하며 시선을 돌리고 걸음을 옮겼다.\n안개가 다시 짙어지며 평범한 길로 돌아왔다.\n\n{checkMsg}"),
                            new(Game_System.Green("[정신력 +5]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                    else
                    {
                        p.Mental -= 5;
                        p.Will -= 5;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"극심한 공포에 사로잡히자 거수가 몸을 일으켰다!\n빙판이 무너지며 아수라장이 되었고, 간신히 목숨만 건져 빠져나왔다.\n\n{checkMsg}"),
                            new(Game_System.Red("[정신력 -5, 의지 -5]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                }
            );
        }
    }
}