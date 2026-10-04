using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Explore.Desert
{
    public static class Desert_01_Floret
    {
        public static EventStep Get_Step(Player player)
        {
            Sprite floretSprite = Resources.Load<Sprite>("Event_Image/Event_01");

            string title = "[별을 품은 사막 : 작은 꽃]";

            string bodyText = """
                사막은 아름답지만 동시에 메마른 공간이다.
                그렇기에 평범한 식물은 이곳에서는 살아남지 못할 것이다.
                허나 예상을 비웃듯 사막 한가운데에는 자그마한 꽃 한 송이가 피어있다.
                """;

            string bottomText = """
                3장의 꽃잎을 가지고 있는 볼품없는 새하얀 꽃.
                당신은 이상하리만큼 눈길이 가는 것을 느낀다.
                """;

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 작은 꽃에게 인사를 한다",
                opt2: "2. 꽃을 따간다",
                opt3: null,
                onOption1: (p, manager) =>
                {
                    p.Mental += 5;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("""
                            인사는 모든 예의의 시작이다. 사람인지 아닌지는 중요하지 않다.
                            당신은 꽃을 향해 예의 바르게 인사를 했고, 놀랍게도 작은 꽃은 꽃잎을 살랑거리며 받아줬다.

                            '당신은 참으로 예의가 바르시군요.'
                            '이렇게 예의 바르신 분은 정말 오랜만이에요.'

                            작은 꽃은 기분이 좋은지 당신과 여러 대화를 나눈다.
                            대화는 무척 즐거웠고 헤어질 시간이 되자 작은 꽃은 사막을 벗어날 방법도 알려주었다.

                            '예의는 짐승과 인간을 나누는 경계, 부디 언제나 예의를 잊지 마시길.'
                            """),
                        new($"""
                            {Game_System.Green("[정신력 +5]")}
                            """)
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step(
                        title, nodes, (pl, mgr) => mgr.Show_Main_UI(),
                        floretSprite, IllustrationPosition.Top
                    ));
                },
                onOption2: (p, manager) =>
                {
                    p.Die(DeathType.Flower_Trap);
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("""
                            당신은 호기심을 못 참고 꽃을 꺾으려 시도했다.
                            그러나 연약해 보였던 꽃은 아무리 힘을 주어도 꼼짝하지 않았다.

                            '당신은 무척 무례하군요.'

                            메마른 사막에 작은 씨앗이 떨어지고, 드넓은 대지에 뿌리를 뻗었다.
                            뿌리는 끊임없이 뻗어져 사막의 반절을 차지했으니,
                            작은 꽃은 오늘 또 하나의 짐승을 잡아먹는다.
                            """),
                        new($"""
                            {Game_System.Red("[사망: 대지에 뿌리를 내린 자]")}
                            """)
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step(
                        title, nodes, (pl, mgr) => mgr.CheckPlayerDeath(),
                        floretSprite, IllustrationPosition.Top
                    ));
                },
                onOption3: null,
                illustration: floretSprite,
                imgPos: IllustrationPosition.Middle,
                bottomText: bottomText
            );
        }
    }
}