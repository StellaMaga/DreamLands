using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        private static EventStep Sub_Event_07_Lost_Name(Player player)
        {
            int reqDetection = 66; 
            string title = "[서브 이벤트 7: 잊혀지고 있는 것]"; 
            string bodyText = """
                무언가가 안개를 헤치고 다가온다.
                그것은 고래처럼 경이로우며, 사슴처럼 신성했다.
                현실에서는 잊혀졌지만 꿈이기에 볼 수 있는 존재.
                그것은 새로운 몽상가를 반긴다.
                """; 

            string opt2Text = Game_System.FormatOption("교감을 한다", "탐지", player.Detection, reqDetection); 

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "[축복을 받는다]",
                opt2: opt2Text,
                opt3: null,
                onOption1: (p, mgr) =>
                {
                    p.Mental += 10; 
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new("머러미쿠", "'이곳은 모두의 꿈.'"),
                        new("머러미쿠", "'무섭고 슬픈 꿈이 있어 상처받았을 수도 있지만,\n부디 아름답고 행복한 꿈도 있다는 것을 기억해 줘.'"),
                        new("신성한 온기가 당신의 지친 정신을 부드럽게 감쌉니다.\n\n" + Game_System.Green("[정신력 +10]"))
                    }; 
                    mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI())); 
                },
                onOption2: (p, mgr) =>
                {
                    if (Game_System.Has_Stat(p.Detection, reqDetection)) 
                    {
                        if (!p.HasItem("Murramiku")) 
                        {
                            Mystic murramiku = new Mystic(
                                id: "Murramiku",
                                name: "머러미쿠",
                                description: "손등에 선명하게 새겨진 잊혀진 신수의 이름. (정신력 +10, 이성 +5, 탐지 보정 +5)",
                                type: MysticType.Mystic // 올바른 Enum으로 수정
                            )
                            {
                                BonusMental = 10,
                                BonusSanity = 5,
                                DetectionBonus = 5
                            }; 
                            p.ObtainItem(murramiku); 
                        }

                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new("잊혀지고 흐릿하지만 당신은 강렬하게 느낄 수 있었다.\n꿈이기에, 그리고 자각한 몽상가이기에 당신은 그것의 이름을 입에 담는다."),
                            new(p.Name, "\"머러미쿠...\""),
                            new("머러미쿠", "'...맞아, 그것이 나의 이름.'\n'잊혀지고 있지만 여전히 기억되고 있는 나의 이름.'"),
                            new("이름은 아무것도 아니지만 동시에 모든 것이다.\n모든 것이 흐릿한 곳이기에 떠올릴 수 있는 이름.\n그것은 자신의 이름을 떠올려준 몽상가에 감사를 전한다."),
                            new("손등에 그것의 이름이 선명하게 새겨져 있다.\n\n" + Game_System.Green("[+ 신비 획득: 머러미쿠 (정신력 +10, 이성 +5)]"))
                        }; 
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI())); 
                    }
                    else
                    {
                        p.Mental += 5; 
                        DialogueNode[] nodes = new DialogueNode[]
                        {
                            new("존재의 기척이 너무나 희미하여 그 본질을 명확히 읽어낼 수 없다."),
                            new(Game_System.Red($"[탐지가 부족합니다. (필요: {reqDetection} / 현재: {p.Detection})]") + "\n\n아쉬운 대로 안개 속에서 미약한 위안을 얻습니다. (정신력 +5)")
                        }; 
                        mgr.Execute_EventStep(EventStep.Multi_Step(title, nodes, (p2, mgr2) => mgr2.Show_Main_UI())); 
                    }
                }
            );
        }
    }
}