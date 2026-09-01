using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Sub_Events
    {
        public static EventStep Sub_Event_13_Suicide_Theory()
        {
            return new EventStep(
                title: "[서브 이벤트 13: 에드윈 슈나이트만의 이론]",
                bodyText: """
                사방에 죽음이 널브러져 있다.
                충동적이고, 나태하며, 광적이고, 도전적이다.
                널브러져 있는 죽음 중 하나가 묻는다.
                
                '너는 우리 중 누구일까.'
                """,
                opt1: "[무언가 다가온다]",
                opt2: null,
                onOption1: (player, manager) => Process_Event_13_Choice(player, manager)
            );
        }

        private static void Process_Event_13_Choice(Player player, GameManager manager)
        {
            int roll = Game_System.Roll_D6();

            // 1. [죽음의 추구자] (의지 판정 44)
            if (roll == 1)
            {
                bool isSuccess = Game_System.Stat_Check("의지", 44, player.Will, out string diceMsg);
                if (isSuccess)
                {
                    player.Will += 4;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 추구하는 자가 말한다.
                            '너는 나다.'
                            '삶을 끝내려는 분명한 의도를 가지고 죽고자 하는 강렬한 욕망을 품고 있지.'
                            '지금도 이 지긋지긋한 세계에서 벗어나고 싶어하지 않는가?'

                            추구자가 말하는 것과 동시에 강렬한 죽음의 충동이 들이닥친다.

                            {diceMsg}
                            """),
                        new($"충동은 잠깐이다.\n" +
                        $"한 순간이라도 충동을 참아내면 그것은 힘없이 무너진다.\n" +
                        $"이를 악물고 충동을 참아내자 죽음이 물러난다.\n\n" +
                        $"{Game_System.Green("[의지 +4]")}")
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                }
                else
                {
                    player.Will -= 10; // 기획 원문: 의지 -10
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 추구하는 자가 말한다.
                            '너는 나다.'
                            '삶을 끝내려는 분명한 의도를 가지고 죽고자 하는 강렬한 욕망을 품고 있지.'
                            '지금도 이 지긋지긋한 세계에서 벗어나고 싶어하지 않는가?'

                            추구자가 말하는 것과 동시에 강렬한 죽음의 충동이 들이닥친다.

                            {diceMsg}
                            """),
                        new($"죽음을 추구하는 자는 확실한 죽음을 선호한다.\n나는 어느새 쥐어진 권총을 들어 턱 밑에 대었고, 그대로 방아쇠를 당겼다.\n\n탕!\n\n{Game_System.Red("[의지 -10]")}")
                    };

                    if (player.Will <= 0)
                    {
                        player.Die(DeathType.Suicide);
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.CheckPlayerDeath()));
                    }
                    else
                    {
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                    }
                }
            }
            // 2. [죽음의 개시자] (정신력 판정 44)
            else if (roll == 2)
            {
                bool isSuccess = Game_System.Stat_Check("정신력", 44, player.Mental, out string diceMsg);
                if (isSuccess)
                {
                    player.Mental += 4;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 개시하는 자가 말한다.
                            '너는 나다.'
                            '죽음은 이미 시작되었으며 우리는 그 순간을 앞당기는 것 뿐이다.'
                            '왜 굳이 필사적으로 발버둥을 치는가. 미래는 정해져 있을 터인데.'

                            개시자가 말하는 것과 동시에 끔찍한 무기력함이 들이닥친다.

                            {diceMsg}
                            """),
                        new($"죽음이라는 결과가 정해졌어도 과정은 스스로 정하는 것이다.\n언젠가는 죽음을 맞이하겠지만 그 순간이 지금은 아닐 것이다.\n\n{Game_System.Green("[정신력 +4]")}")
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                }
                else
                {
                    player.Mental -= 10;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 개시하는 자가 말한다.
                            '너는 나다.'
                            '죽음은 이미 시작되었으며 우리는 그 순간을 앞당기는 것 뿐이다.'
                            '왜 굳이 필사적으로 발버둥을 치는가. 미래는 정해져 있을 터인데.'

                            개시자가 말하는 것과 동시에 끔찍한 무기력함이 들이닥친다.

                            {diceMsg}
                            """),
                        new($"하루를 붙잡지 않은 자에게 시간은 냉혹하다.\n미래가 정해져 있다고 지금을 포기하자 시간이 거둬져간다.\n정신을 차리고 보니 당신은 폭삭 늙어있었다.\n\n{Game_System.Red("[정신력 -10]")}")
                    };

                    if (player.Mental <= 0)
                    {
                        player.Die(DeathType.Mental_Zero);
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.CheckPlayerDeath()));
                    }
                    else
                    {
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                    }
                }
            }
            // 3. [죽음의 무시자] (이성 판정 44)
            else if (roll == 3)
            {
                bool isSuccess = Game_System.Stat_Check("이성", 44, player.Sanity, out string diceMsg);
                if (isSuccess)
                {
                    player.Sanity += 4;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 무시하는 자가 말한다.
                            '너는 나다.'
                            '죽음 뒤에는 지금보다 더 나은 삶이 존재함을 알고 있다.'
                            '이 비현실적인 세상이 사후를 증명한다!'
                            '그렇다면 지금 죽음을 맞이하는 게 무엇이 나쁜가?'

                            무시자가 말하는 것과 동시에 현실에 대한 의문이 들기 시작한다.

                            {diceMsg}
                            """),
                        new($"당신은 지금 이 순간을 살아가고 있다.\n현실을 충실하게 살아가지 않으면 사후가 무슨 의미가 있는가?\n\n{Game_System.Green("[이성 +4]")}")
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                }
                else
                {
                    player.Sanity -= 4;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음을 무시하는 자가 말한다.
                            '너는 나다.'
                            '죽음 뒤에는 지금보다 더 나은 삶이 존재함을 알고 있다.'
                            '이 비현실적인 세상이 사후를 증명한다!'
                            '그렇다면 지금 죽음을 맞이하는 게 무엇이 나쁜가?'

                            무시자가 말하는 것과 동시에 현실에 대한 의문이 들기 시작한다.

                            {diceMsg}
                            """),
                        new($"이 비현실적인 세상도 존재하는데 사후가 존재하지 않을 리가 없다.\n비상식적인 믿음에 빠진 당신은 그대로 바닥에 있는 날카로운 단검을 주워 목을 긋는다.\n죽음을 맞이하지는 않았지만 그에 준하는 대가를 바쳐야 했다.\n\n{Game_System.Red("[이성 -4]")}")
                    };

                    if (player.Sanity <= 0)
                    {
                        player.Die(DeathType.Sanity_Zero);
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.CheckPlayerDeath()));
                    }
                    else
                    {
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                    }
                }
            }
            // 4. [죽음의 도전자] (D&D DC 14 판정)
            else if (roll == 4)
            {
                bool isSuccess = Game_System.DD_Check(14, "의지", player.Will, player, out string diceMsg);
                if (isSuccess)
                {
                    // 도전자의 주사위 유물 획득 (행운 +3)
                    if (!player.HasItem("ChallengerDice"))
                    {
                        Mystic dice = new Mystic(
                            id: "ChallengerDice",
                            name: "도전자의 주사위",
                            description: "죽음과의 내기에서 승리하고 얻은 주사위. (행운 +3)",
                            type: MysticType.Item
                        )
                        {
                            BonusLuck = 3
                        };
                        player.ObtainItem(dice);
                    }

                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음에 도전하는 자가 말한다.
                            '너는 나다.'
                            '목숨을 밖에 내놓고 이 비현실적인 세계를 즐기고 있다.'
                            '분명 죽음을 건 도전에 환장하는 것이 분명하다.'
                            '그러니... 나와 목숨을 건 내기를 하자.'

                            거절은 불가능해 보인다.

                            {diceMsg}
                            """),
                        new($"'이야, 졌구만 졌어.'\n'축하하네, 죽음과의 내기에서 이긴 것을.'\n\n정신을 차리고 보니 도전자가 있던 자리에는 내기에 사용한 주사위가 놓여있었다.\n\n{Game_System.Green("[+ 신비 획득: 도전자의 주사위 (행운 +3)]")}")
                    };
                    manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                }
                else
                {
                    player.Mental /= 2;
                    DialogueNode[] nodes = new DialogueNode[]
                    {
                        new($"""
                            죽음에 도전하는 자가 말한다.
                            '너는 나다.'
                            '목숨을 밖에 내놓고 이 비현실적인 세계를 즐기고 있다.'
                            '분명 죽음을 건 도전에 환장하는 것이 분명하다.'
                            '그러니... 나와 목숨을 건 내기를 하자.'

                            거절은 불가능해 보인다.

                            {diceMsg}
                            """),
                        new($"'이야, 아쉽게 됐군.'\n'뭐, 오랜만에 재미는 있었으니 죽이지는 않겠네.'\n'딱 절반만 가져가지.'\n\n{Game_System.Red($"[정신력 절반 감소 (현재: {player.Mental})]")}")
                    };

                    if (player.Mental <= 0)
                    {
                        player.Die(DeathType.Mental_Zero);
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.CheckPlayerDeath()));
                    }
                    else
                    {
                        manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
                    }
                }
            }
            // 5~6. [삶을 향한 의지] (극복)
            else
            {
                player.Will += 4;
                DialogueNode[] nodes = new DialogueNode[]
                {
                    new("""
                        '그 누구도 아니야.'

                        당신의 정신은 죽음이 아닌 삶을 향해 나아간다.
                        그렇기에 널브러진 그 어떤 죽음도 당신을 막지 못한다.

                        죽음은 멀어지는 당신을 보며 나지막이 말한다.

                        '다시 보자.'
                        """),
                    new(Game_System.Green("[의지 +4]"))
                };
                manager.Execute_EventStep(EventStep.Multi_Step("[서브 이벤트 13: 에드윈 슈나이트만의 이론]", nodes, (p, mgr) => mgr.Show_Main_UI()));
            }
        }
    }
}