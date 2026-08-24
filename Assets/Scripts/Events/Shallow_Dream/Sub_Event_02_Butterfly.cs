using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_02_Butterfly(Player player)
        {
            string title = "[서브 이벤트 2: 나비]";
            string bodyText = "안개가 더 짙어지고 지형이 미로같이 변한다.\n" +
                              "몇 분째 같은 자리를 헤맸을까, 무언가 당신 앞에 나타난다.\n" +
                              "하나는 순백의 날개를, 하나는 칠흑의 날개를 가진 한 쌍의 나비들이다.\n" +
                              "자유롭게 주변을 누비던 나비들은 이내 양 갈래로 흩어진다.";

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 순백의 나비를 쫓아간다",
                opt2: "2. 칠흑의 나비를 쫓아간다",
                onOption1: (p, mgr) =>
                {
                    p.Mental -= 5;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("나비는 내가 자신을 따라오는 것을 확인하자마자 앞으로 빠르게 날아간다."),
                        new("정신없이 따라가던 와중 갑자기 지면이 사라졌다!\n낭떠러지에 떨어지기 전 가까스로 돌부리를 붙잡았다.\n\n'아깝다.'\n\n...나비는 혐오스러운 인간의 얼굴을 가지고 있었다."),
                        new(Game_System.Red("[정신력 -5]"))
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                },
                onOption2: (p, mgr) =>
                {
                    // 칠흑 나비: 무사 탈출 (3번 선택지로 연결하거나 메인 UI 복귀)
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("칠흑의 나비는 내가 잘 쫓아오는지 확인하듯 천천히 날아간다.\n왼쪽으로, 오른쪽으로, 아래로, 위로."),
                        new("이내 안개는 다시 옅어지고, 미로와도 같은 지형에서 무사히 빠져나왔다.")
                    };
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI()));
                }
            );
        }

        // 3번 선택지 (스스로 길 찾기 - DD 탐지 판정)
        public static EventStep Sub_Event_02_Option3(Player player)
        {
            string title = "[서브 이벤트 2: 나비 - 스스로 길찾기]";
            bool isSuccess = Game_System.DD_Check(12, "탐지", player.Detection, player, out string diceMsg);

            if (isSuccess)
            {
                if (!player.HasMystic("Wayfinding"))
                {
                    player.ObtainMystic(new Mystic("Wayfinding", "길찾기", "미로와 안개 속에서 길을 찾아내는 직감.", MysticType.Mystic)
                    {
                        DetectionBonus = 5
                    });
                }

                DialogueNode[] nodes = new DialogueNode[]
                {
                    new($"당신은 나비들을 지나쳐 앞으로 나아간다.\n이곳에서 믿을 수 있는 것은 결국 스스로뿐이다.\n\n{diceMsg}"),
                    new($"신중히 길을 가던 당신은 결국 미궁에서 홀로 빠져나오는 데 성공했다.\n\n{Game_System.Green("[+ 신비 획득: 길찾기]")}")
                };
                return EventStep.Multi_Step(title, nodes, (p, mgr) => mgr.Show_Main_UI());
            }
            else
            {
                player.Madness += 7;
                DialogueNode[] nodes = new DialogueNode[]
                {
                    new($"스스로 길을 찾을 거라 생각한 것은 오만한 판단이었다.\n아무리 나아가도 같은 자리가 반복되며 정신이 피폐해진다.\n\n{diceMsg}"),
                    new($"어떻게든 탈출에는 성공했지만 정신은 온전하지 못하다.\n\n{Game_System.Purple("[광기 +7]")}")
                };
                return EventStep.Multi_Step(title, nodes, (p, mgr) => mgr.Show_Main_UI());
            }
        }
    }
}