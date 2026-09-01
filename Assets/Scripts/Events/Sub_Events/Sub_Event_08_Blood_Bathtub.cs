using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_08_Blood_Bathtub(Player player)
        {
            string title = "[서브 이벤트 8: 피의 욕조]";
            string bodyText = "숨을 쉴 때마다 비릿한 쇠 맛이 사방에 진동한다.\n" +
                              "피로 물든 공간 한가운데에 놓인 욕조, 그 안에는 머리카락으로 얼굴을 가린 여인이 있다.\n" +
                              "붉은 핏물이 끝없이 넘쳐흐른다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 여인을 욕조 안으로 밀어 넣는다 [DD 간섭 판정]",
                opt2: "2. 여인을 밖으로 끄집어낸다 [CoC 간섭 판정]",
                onOption1: (p, mgr) =>
                {
                    bool isSuccess = Game_System.DD_Check(12, "간섭", p.Interference, p, out string diceMsg);
                    if (isSuccess)
                    {
                        if (!p.HasItem("BloodyRing"))
                        {
                            p.ObtainItem(new Mystic("BloodyRing", "피 묻은 반지", "욕조에 삼켜진 여인이 남긴 핏빛 반지.", MysticType.Item)
                            {
                                BonusWill = 4
                            });
                        }

                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"창백한 여인을 욕조 속 깊은 곳으로 밀어 넣었다.\n욕조가 만족하듯 핏물이 잦아들고 바닥에 반지가 남았다.\n\n{diceMsg}"),
                            new(Game_System.Green("[+ 아이템 획득: 피 묻은 반지]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                    else
                    {
                        p.Will -= 10;
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new($"여인이 살고 싶다는 듯 당신의 팔을 억세게 붙잡았다.\n간신히 뿌리치고 도망쳤으나 팔에 지워지지 않을 흔적이 남았다.\n\n{diceMsg}"),
                            new(Game_System.Red("[의지 -10]"))
                        };
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                    }
                },
                onOption2: (p, mgr) =>
                {
                CoC_Result result = Game_System.CoC_Check(p.Interference); 
                    if (result == CoC_Result.Jackpot || result == CoC_Result.Success)
            {
                if (!p.HasItem("RadiantRing"))
                {
                    p.ObtainItem(new Mystic("RadiantRing", "빛을 품은 반지", "구원받은 여인이 빛으로 흩어지며 남긴 유품.", MysticType.Item)
                    {
                        BonusSanity = 5,
                        BonusMental = 5
                    });
                }

                DialogueNode[] nodes = new DialogueNode[]
                {
                            new("창백한 손을 맞잡고 욕조 밖으로 끌어올렸다.\n\n'...아, 현실에서 당신을 만났더라면.'\n\n여인은 빛이 되어 흩어지고 핏물도 모두 사라졌다."),
                            new(Game_System.Green("[+ 아이템 획득: 빛을 품은 반지]"))
                };
                mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
            }
            else
            {
                p.Mental -= 10;
                DialogueNode[] nodes = new DialogueNode[]
                {
                            new("피로 이루어진 손아귀들이 여인을 다시 끌어당겼고, 당신은 손을 놓치고 말았다.\n절망적인 비명만이 귓가에 맴돈다."),
                            new(Game_System.Red("[정신력 -10]"))
                };
                mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
            }
        }
            );
        }
    }
}