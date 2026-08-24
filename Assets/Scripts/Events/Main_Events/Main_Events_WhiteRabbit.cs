using UnityEngine;
using System.Collections.Generic;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public partial class Main_Events
    {
        // 3. [메인 이벤트: 흰 토끼]
        public static EventStep White_Rabbit(Player player)
        {
            player.Mental += 5;
            player.Madness += 5;

            HashSet<int> askedIndices = new HashSet<int>();

            DialogueNode[] introDialogues = new DialogueNode[]
            {
                new("기묘한 세계 속에서 사건사고를 겪으며 한 치 앞도 안 보이는 안개마저 친숙해질 즈음\n" +
                "째깍거리는 소리와 함께 회중시계를 든 토끼 가면의 소년이 나타났다."),

                new("흰 토끼", "\"믿고 있었습니다. 당신이라면 꿈에 잡아먹히지 않고 살아남아 있을 거라고.\n\n" +
                Game_System.Green("[정신력 +5 | 광기 +5]"))
            };

            return EventStep.Multi_Step(
                title: "[ 메인 이벤트 : 흰 토끼 ]",
                dialogues: introDialogues,
                onComplete: (p, mgr) => ShowWhiteRabbitDialogueMenu(p, mgr, askedIndices)
            );
        }

        private static void ShowWhiteRabbitDialogueMenu(Player player, GameManager manager, HashSet<int> askedIndices)
        {
            if (askedIndices.Count >= 3)
            {
                ShowWhiteRabbitEnding(player, manager);
                return;
            }

            List<int> availableQuestions = new List<int>();
            for (int i = 1; i <= 3; i++)
            {
                if (!askedIndices.Contains(i))
                    availableQuestions.Add(i);
            }

            int q1Index = availableQuestions[0];
            string opt1Text = GetQuestionText(q1Index);

            string opt2Text = null;
            int q2Index = -1;
            if (availableQuestions.Count > 1)
            {
                q2Index = availableQuestions[1];
                opt2Text = GetQuestionText(q2Index);
            }

            manager.Execute_EventStep(new EventStep(
                title: "[ 흰 토끼와의 대화 ]",
                bodyText: Game_System.ColorSpeaker("흰 토끼") + "\n회중시계를 만지작거리며 당신의 질문을 기다립니다.\n무엇을 물어보시겠습니까?",
                opt1: opt1Text,
                opt2: opt2Text,
                onOption1: (p, mgr) => ProcessQuestion(p, mgr, q1Index, askedIndices),
                onOption2: (p, mgr) =>
                {
                    if (q2Index != -1)
                        ProcessQuestion(p, mgr, q2Index, askedIndices);
                }
            ));
        }

        private static string GetQuestionText(int index)
        {
            switch (index)
            {
                case 1: return "1. 원래 있던 곳으로 돌려보내줘";
                case 2: return "2. 당신의 정체는 무엇인가?";
                case 3: return "3. 이곳은 도대체 어디인가?";
                default: return "";
            }
        }

        private static void ProcessQuestion(Player player, GameManager manager, int questionIndex, HashSet<int> askedIndices)
        {
            askedIndices.Add(questionIndex);
            DialogueNode[] dialogues = null;

            switch (questionIndex)
            {
                case 1:
                    player.Sanity = Mathf.Max(0, player.Sanity - 5);
                    dialogues = new DialogueNode[]
                    {
                        new("흰 토끼", "안타깝게도 불가능합니다.\n" +
                        "당신을 억지로 각성시키느라 현실과 꿈의 연결고리가 끊어졌으니까요."),
                        new("무책임한 소리에 정신이 흔들리는 것을 느낍니다."),
                        new(Game_System.Red("[이성 -5]"))
                    };
                    break;

                case 2:
                    dialogues = new DialogueNode[]
                    {
                        new("흰 토끼", "저는 바쁘게 돌아다니며 늦장을 부리는 존재를 아브락삭스로 인도하는 자.\n그냥 흰 토끼라 부르시면 됩니다.")
                    };
                    break;

                case 3:
                    dialogues = new DialogueNode[]
                    {
                        new("흰 토끼", "\"이곳은 수많은 인류의 무의식이 만들어낸 세계. 저희는 흔히 얕은 꿈이라고 부르지요.\"")
                    };
                    break;
            }

            manager.Execute_EventStep(EventStep.Multi_Step(
                title: "[ 흰 토끼와의 대화 ]",
                dialogues: dialogues,
                onComplete: (p, mgr) => ShowWhiteRabbitDialogueMenu(p, mgr, askedIndices)
            ));
        }

        private static void ShowWhiteRabbitEnding(Player player, GameManager manager)
        {
            DialogueNode[] endingDialogues = new DialogueNode[]
            {
                new("흰 토끼", "\"안타깝게도 이제 시간이 다 되었군요. 만일 현실로 돌아가고 싶으면, 부디 드림랜드로 오시길...\""),
                new("흰 토끼는 다시 안개 속으로 사라져 버렸습니다."),
                new("원하는 답은 듣지 못했지만, [드림랜드]라는 분명한 목표가 생겼습니다."),
                new(Game_System.Green("[목표 명확화: 의지 +5]"))
            };

            manager.Execute_EventStep(EventStep.Multi_Step(
                title: "[ 메인 이벤트 : 흰 토끼 ]",
                dialogues: endingDialogues,
                onComplete: (p, mgr) =>
                {
                    p.Will += 5;
                    mgr.Show_Main_UI();
                }
            ));
        }
    }
}