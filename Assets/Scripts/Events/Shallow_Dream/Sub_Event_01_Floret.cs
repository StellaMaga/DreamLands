using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_01_Floret()
        {
            return new EventStep(
                title: "[서브 이벤트 1: 작은 꽃]",
                bodyText: "메마른 대지, 그 어떤 식물도 이곳에서는 살아남지 못할 것이다.\n" +
                          "허나 예상을 비웃듯 대지 한가운데에는 자그마한 꽃 한 송이가 피어 있다.\n" +
                          "3장의 꽃잎을 가지고 있는 볼품없는 새하얀 꽃.\n" +
                          "이상하리만큼 눈길이 간다.",
                opt1: "[1. 인사를 한다]",
                opt2: "[2. 꽃을 따간다]",
                onOption1: (player, manager) =>
                {
                    player.Mental += 5;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            인사는 모든 예의의 시작이다.
                            인사를 하자 작은 꽃은 꽃잎을 살랑거리며 받아준다.,
                            이 장소에서 벗어나려면 어디로 가야 하냐 질문하자 꽃은 한쪽 방향을 가리킨다.
                            당신은 감사를 표하며 길을 마저 나아간다.

                            '예의는 짐승과 인간을 나누는 경계, 부디 언제나 예의를 잊지 마시길.'

                            {Game_System.Green("[정신력 +5]")}
                            """)
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 1: 작은 꽃]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                },
                onOption2: (player, manager) =>
                {
                    player.Die(DeathType.Flower_Trap);
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            호기심을 못 참고 꽃을 꺾으려 시도했다.
                            그러나 연약해 보였던 꽃은 아무리 힘을 주어도 꼼짝도 하지 않았다.
                            
                            '인사도 없이 꽃을 꺾으려 하다니, 당신 짐승이군요?'
                            
                            이 넓은 대지에 뿌리를 뻗은 꽃은 오늘 또 하나의 짐승을 집어삼켰다.
                            
                            {Game_System.Red("[사망: 대지에 뿌리를 내린자]")}
                            """)
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 1: 작은 꽃]", nodes, (p, mgr) => mgr.CheckPlayerDeath()));
                }
            );
        }
    }
}