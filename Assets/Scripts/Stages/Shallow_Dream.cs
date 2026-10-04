using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;
using TextGame.Events.Shallow_Dream;
using TextGame.Events.Bond_Events;
using TextGame.Events.Explore;

namespace TextGame.Stages
{
    public class Shallow_Dream
    {
        // 1회 등장한 이벤트 중복 등장 방지용 목록
        public static HashSet<string> completedEvents = new HashSet<string>();

        // GameManager의 턴 진행 시 호출되는 메인 이벤트 추첨 함수
        public static EventStep Next_Event(Player player, ref int eventCount, ref int cheshire_Count, ref int cheshire_Chance, ref bool met_WhiteRabbit)
        {
            eventCount++; // 턴 수 증가

            // [우선순위 1] 14회차 : 스테이지 종료 (임시 클리어 엔딩)
            if (eventCount >= 14)
            {
                return new EventStep(
                    title: "[ 1장 탐험 종료 : 얕은 꿈의 경계 ]",
                    bodyText: """
                        얕은 꿈의 끝자락에 다다랐습니다.
                        안개 너머로 더 깊은 차원의 꿈(드림랜드)으로 향하는 문이 아른거립니다.
                        """,
                    opt1: "다음 단계로 ▶",
                    opt2: null,
                    onOption1: (p, manager) =>
                    {
                        if (manager.endingManager != null)
                        {
                            manager.endingManager.Show_Ending(p, "Stage1_Clear");
                        }
                        else
                        {
                            manager.Show_Main_UI();
                        }
                    }
                );
            }

            // [우선순위 2] 13회차 확정 메인 이벤트: 흰 토끼
            if (eventCount == 13)
            {
                met_WhiteRabbit = true;
                return Main_Events.White_Rabbit(player);
            }

            // [우선순위 3] 7회차 공용 이벤트: 화로와 여인
            if (eventCount == 7)
            {
                return Rest_Events.Hestia(player);
            }

            // [우선순위 4] 조건형 메인 이벤트: 체셔캣 등장
            if (cheshire_Count < 3 && UnityEngine.Random.Range(1, 101) <= cheshire_Chance)
            {
                cheshire_Count++;
                return Main_Events.Cheshire_Cat(player, cheshire_Count);
            }

            // [우선순위 5] 신규 5대 확률 기반 일반 이벤트 추첨
            int rolledChance = UnityEngine.Random.Range(1, 101);

            // [1. 서브 이벤트 : 50% 확률 (1 ~ 50)]
            if (rolledChance <= 50)
            {
                return Sub_Events.Random_Event(player);
            }
            // [2. 탐험 이벤트 : 35% 확률 (51 ~ 85)]
            else if (rolledChance <= 85)
            {
                return Explore_Manager.Random_Explore(player);
            }
            // [3. 인연 이벤트 : 10% 확률 (86 ~ 95)]
            else if (rolledChance <= 95)
            {
                return Bond_Events.Random_Event(player);
            }
            // [4. 전투 이벤트 : 5% 확률 (96 ~ 100)]
            else
            {
                return Battle_Event.Random_Battle(player);
            }
        }
    }
}