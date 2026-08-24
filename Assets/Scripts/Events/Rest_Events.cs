using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Shallow_Dream
{
    public class Rest_Events
    {
        // [휴식 이벤트 : 화로와 여인 메인 진입 함수]
        public static EventStep Hestia(Player player)
        {
            // 통합 신비 시스템(HasItem)으로 조건 체크
            bool sawOption1 = player.HasItem("화로_1번선택");
            bool sawOption2 = player.HasItem("화로_2번선택");
            bool canOffer = sawOption1 && sawOption2; // 1번, 2번을 모두 경험해야 공물 바치기 해금

            string title = "[휴식 이벤트 : 화로와 여인]";
            string body = "안개를 가로지르며 나아가던 중 희미한 불빛이 당신을 반긴다.\n" +
                         "불빛을 향해 다가가니 한 여인이 화로를 지키고 있다.\n" +
                         "여인은 아무 말도 하지 않고 있지만, 여기로 와서 쉬고 가라는 듯한 시선을 보낸다.";

            if (canOffer)
            {
                return new EventStep(
                    title: title,
                    bodyText: body + "\n\n<color=green>[여인과의 깊은 교감이 느껴집니다.]</color>",
                    opt1: "1. 화로 곁에서 휴식",
                    opt2: "2. 여인에게 공물 바치기",
                    opt3: null,
                    onOption1: (p, manager) => {
                        Action_Rest(p, manager);
                    },
                    onOption2: (p, manager) => {
                        Action_Offer(p, manager);
                    }
                );
            }
            else
            {
                return new EventStep(
                    title: title,
                    bodyText: body,
                    opt1: "1. 화로 곁에서 휴식",
                    opt2: "2. 여인과 대화 나누기",
                    opt3: null,
                    onOption1: (p, manager) => {
                        if (!sawOption1) p.ObtainItem(new Mystic("화로_1번선택", "화로_1번선택", "화로 대화 기록 1", MysticType.Item));
                        Action_Rest(p, manager);
                    },
                    onOption2: (p, manager) => {
                        if (!sawOption2) p.ObtainItem(new Mystic("화로_2번선택", "화로_2번선택", "화로 대화 기록 2", MysticType.Item));
                        Action_Talk(p, manager);
                    }
                );
            }
        }

        // [선택지 1: 화로 곁에서 휴식]
        private static void Action_Rest(Player player, GameManager manager)
        {
            player.Madness = Mathf.Max(0, player.Madness - 10);
            player.Mental += 10;

            EventStep resultStep = new EventStep(
                title: "[휴식 이벤트 : 화로와 여인]",
                bodyText: "감사히 여인의 화로 곁에 앉아 타오르는 불을 쬐며 휴식을 취합니다.\n" +
                          "타닥타닥 타오르는 따스한 불빛이 꿈속을 헤매며 쌓인 피로를 부드럽게 녹여줍니다.\n\n" +
                          "<color=green>[광기 -10 | 정신력 +10]</color>",
                opt1: "다음 ▶",
                opt2: null,
                opt3: null,
                onOption1: (p, mgr) => {
                    mgr.Show_Main_UI();
                }
            );

            manager.Execute_EventStep(resultStep);
        }

        // [선택지 2: 여인과 대화 나누기]
        private static void Action_Talk(Player player, GameManager manager)
        {
            // 따스한 숯 유물 획득 처리 (패시브: 정신력 +5)
            if (!player.HasItem("WarmCharcoal"))
            {
                Mystic charcoal = new Mystic("WarmCharcoal", "따스한 숯", "화로의 온기가 남아있는 숯. (정신력 +5)", MysticType.Item)
                {
                    BonusMental = 5
                };
                player.ObtainItem(charcoal);
            }

            EventStep resultStep = new EventStep(
                title: "[휴식 이벤트 : 화로와 여인]",
                bodyText: "조용히 앉아 화로를 관리하는 여인에게 말을 건네본다.\n" +
                          "여인이 대화에 응하지는 않았지만, 당신은 아랑곳하지 않고 말을 이어간다.\n" +
                          "당신은 그동안 겪은 여정을 하나씩 풀어나갔고, 떠날 시간이 되자 여인이 무언가를 조용히 건넨다.\n" +
                          "이야깃값인가 보다.\n\n" +
                          "<color=green>[+ 아이템 획득: 따스한 숯 (정신력 +5)]</color>",
                opt1: "다음 ▶",
                opt2: null,
                opt3: null,
                onOption1: (p, mgr) => {
                    mgr.Show_Main_UI();
                }
            );

            manager.Execute_EventStep(resultStep);
        }

        // [선택지 3: 공물 바치기 (1, 2번 경험 시 해금)]
        private static void Action_Offer(Player player, GameManager manager)
        {
            // 화로의 축복 신비 획득 (패시브: 의지 +5, 이성 +5)
            if (!player.HasItem("HestiaBlessing"))
            {
                Mystic blessing = new Mystic("HestiaBlessing", "화로의 축복", "손등에 새겨진 화로 문양. (의지 +5, 이성 +5)", MysticType.Mystic)
                {
                    BonusWill = 5,
                    BonusSanity = 5
                };
                player.ObtainMystic(blessing);
            }

            EventStep resultStep = new EventStep(
                title: "[휴식 이벤트 : 화로와 여인]",
                bodyText: "이 기묘한 세상에서 아무런 이유 없이 호의를 내비추는 여인에게 감사를 전하고 싶어졌습니다.\n" +
                          "당신은 가지고 있는 것 중 그나마 가치가 있어 보이는 [별빛]을 여인에게 건넵니다.\n" +
                          "여인은 선물을 받을 줄 예상하지 못했는지 잠깐 멈칫하다가, 희미하게 웃으며 받아들입니다.\n\n" +

                          "<i>'...공물을 받는 것은 무척 오랜만이네요.'</i>\n\n" +

                          "문득, 정신을 차리고 보니 여인도 화로도 깨끗이 사라져 있었습니다.\n" +
                          "어느새 손등에 새겨진 화로 문양의 스티그마에서 따스한 호의가 느껴집니다.\n\n" +

                          "<color=green>[+ 신비 해금: 화로의 축복 (의지 +5, 이성 +5)]</color>",
                opt1: "다음 ▶",
                opt2: null,
                opt3: null,
                onOption1: (p, mgr) => {
                    mgr.Show_Main_UI();
                }
            );

            manager.Execute_EventStep(resultStep);
        }
    }
}