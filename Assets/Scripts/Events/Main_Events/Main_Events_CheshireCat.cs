using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Main_Events
    {
        // 2. [메인 이벤트: 체셔 캣 (1~3회차 만남)]
        public static EventStep Cheshire_Cat(Player player, int meetCount)
        {
            if (meetCount == 1)
            {
                // 통합 신비 시스템 적용 (꿈 탐험 신비 획득)
                if (!player.HasItem("DreamExplore"))
                {
                    Mystic dreamExplore = new Mystic("DreamExplore", "꿈 탐험", "타인의 꿈에 간섭하는 능력.", MysticType.Mystic);
                    player.ObtainItem(dreamExplore);
                }

                DialogueNode[] dialogues = new DialogueNode[]
                {
                    new("체셔 캣", "이곳은 거대한 꿈이야. 그리고 사람들은 꿈 속에서 꿈을 꾸지."),

                    new("어디선가 익살스러운 목소리가 들려왔다.\n" +
                    "주변을 둘러보자 얼굴 없는 입이 허공에 떠 있었다."),

                    new("체셔 캣", "대부분은 꿈이 꿈인지도 모르고 꿈꾸지만, 몇몇은 꿈이라는 것을 인지하고 꿈을 꾸지.\n" +
                    "꿈이 꿈이라는 것을 인지하면 꿈을 모험할 자격이 주어지는거야.\n" +
                    "이제 남은 것은 꿈이라는 알에서 벗어나는 것 뿐이지."),

                    new("쉴 새 없이 떠들던 입은 어느 순간 사라지고 적막만이 남았다.\n" +
                    "그나저나 아무래도 길에서 보이던 거대한 알 같은 구조물은 다른 누군가의 꿈이였나보다.\n" +
                    "문득 다른 사람의 꿈 속에 들어갈 수도 있을거 같다는 생각이 든다."),

                    new(Game_System.Green("[+ 신비 해금: 꿈 탐험]"))
                };

                return EventStep.Multi_Step(
                    title: "[ 메인 이벤트 : 체셔 캣 (1번째 만남) ]",
                    dialogues: dialogues,
                    onComplete: (p, mgr) => mgr.Show_Main_UI()
                );
            }
            else if (meetCount == 2)
            {
                DialogueNode[] dialogues = new DialogueNode[]
                {
                    new("체셔 캣", "지구가 깨어나고 앞면과 뒷면은 뭉개지며 가라앉은 것이 떠오르지.\n" +
                    "지구가 깨어나서 드림랜드가 움직이는 걸까, 아니면 드림랜드가 움직여서 지구가 깨어난 걸까."),

                    new("이번에는 입뿐만 아니라 고양이 특유의 세로 동공을 가진 눈이 밝게 빛나고 있었다."),

                    new("체셔 캣", "토끼는 알을 깨고, 거미는 다리를 짓고.\n" +
                    "늙은이들은 주책을 부리며, 가면은 늘 그렇듯 색다른 장난을 준비하지.\n" +
                    "우리 잠꾸러기 소녀는 거울 속에서 언제 나오려나."),

                    new("그 말을 끝으로 고양이는 사라졌다. 언젠가 한 번 더 만날 것 같은 예감이 든다.")
                };

                return EventStep.Multi_Step(
                    title: "[ 메인 이벤트 : 체셔 캣 (2번째 만남) ]",
                    dialogues: dialogues,
                    onComplete: (p, mgr) => mgr.Show_Main_UI()
                );
            }
            else // 3번째 만남
            {
                DialogueNode[] dialogues = new DialogueNode[]
                {
                    new("나뭇가지 위에 귀까지 입이 찢어지게 웃고 있는 고양이가 앉아 있다."),

                    new("체셔 캣", "첫 번째는 우연, 두 번째는 필연. 세 번째부터는 운명이지.\n" +
                    "너 따위가 대체 뭐라고 나랑 운명이 엮였을까."),

                    new("말해줄래, 제발, 난 어느 쪽으로 가야 되지?"),

                    new("체셔 캣", "그건 네가 어디로 가고 싶은지에 달렸지.\n" +
                    "...굳이 조언하자면 아래로 내려가.\n" +
                    "네가 무엇을 찾든 이 얄팍한 곳에는 네가 원하는 것이 없을 테니까.")
                };

                return EventStep.Multi_Step(
                    title: "[ 메인 이벤트 : 체셔 캣 (3번째 만남) ]",
                    dialogues: dialogues,
                    onComplete: (p, mgr) => mgr.Show_Main_UI()
                );
            }
        }
    }
}