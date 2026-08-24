using System;
using UnityEngine;
using TextGame.Core;
using TextGame.Events.Shallow_Dream;

namespace TextGame.Stages
{
    public class Shallow_Dream
    {
        // GameManager의 턴 진행 시 호출되는 메인 이벤트 추첨 함수
        public static EventStep Next_Event(Player player, ref int eventCount, ref int cheshire_Count, ref int cheshire_Chance, ref bool met_WhiteRabbit)
        {
            eventCount++; // 턴 수 증가

            // [우선순위 1] 14회차 : 스테이지 종료 (임시 클리어 엔딩)
            if (eventCount >= 14)
            {
                return new EventStep(
                    title: "[ 1장 탐험 종료 : 얕은 꿈의 경계 ]",
                    bodyText: "얕은 꿈의 끝자락에 다다랐습니다.\n" +
                              "안개 너머로 더 깊은 차원의 꿈(드림랜드)으로 향하는 문이 아른거립니다.",
                    opt1: "다음 단계로 ▶",
                    opt2: null,
                    onOption1: (p, manager) =>
                    {
                        // Ending 컴포넌트가 연결되어 있다면 클리어 엔딩 호출
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

            // [우선순위 3] 7의 배수 회차 확정 공용 이벤트: 화로와 여인
            if (eventCount % 7 == 0)
            {
                return Rest_Events.Hestia(player);
            }

            // [우선순위 4] 조건형 메인 이벤트: 체셔캣 등장
            if (cheshire_Count < 3 && UnityEngine.Random.Range(1, 101) <= cheshire_Chance)
            {
                cheshire_Count++;
                return Main_Events.Cheshire_Cat(player, cheshire_Count);
            }

            // [우선순위 5] 일반 무작위 이벤트 추첨
            int rolledChance = Game_System.Roll_D100();

            // [60% 확률]: 서브 이벤트 (1~13번 무작위 추첨)
            if (rolledChance <= 60)
            {
                return Sub_Events.Random_Event(player);
            }
            // [20% 확률]: 탐험 이벤트
            else if (rolledChance <= 80)
            {
                return new EventStep(
                    title: "[탐험 이벤트]",
                    bodyText: "안개 속에서 지형의 변화를 발견했습니다.",
                    opt1: "다음 ▶",
                    opt2: null,
                    onOption1: (p, manager) => manager.Show_Main_UI()
                );
            }
            // [10% 확률]: 실제 전투 이벤트 발생 (무작위 적 조우)
            else if (rolledChance <= 90)
            {
                int random_Enemy_Index = UnityEngine.Random.Range(0, Enum.GetValues(typeof(EnemyType)).Length);
                EnemyType randomEnemy = (EnemyType)random_Enemy_Index;

                Enemy tempEnemy = EnemyDatabase.GetEnemy(randomEnemy);

                return new EventStep(
                    title: $"[전투 조우: {tempEnemy.Name}]",
                    bodyText: $"{tempEnemy.Description}\n\n앞으로 나아가던 도중 오싹한 기분이 든다...",
                    opt1: "[탐지(DET) 판정 시도]",
                    opt2: null,
                    onOption1: (p, manager) => {
                        Battle_Event battle = new Battle_Event(p, randomEnemy, manager);
                        battle.Detection_Check();
                    }
                );
            }
            // [10% 확률]: 인연 이벤트
            else
            {
                return new EventStep(
                    title: "[인연 이벤트]",
                    bodyText: "꿈속을 방황하는 또 다른 방랑자의 흔적을 발견했습니다.",
                    opt1: "다음 ▶",
                    opt2: null,
                    onOption1: (p, manager) => manager.Show_Main_UI()
                );
            }
        }
    }
}