using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;
using TextGame.Events.Mini;

namespace TextGame.Events.Theme.City
{
    public static class City_Area
    {
        // --------------------------------------------------
        // 도시 테마 상태 관리 변수
        // --------------------------------------------------
        public static int TurnCount { get; private set; } = 0;       // 도시 체류 턴
        public static bool HasClearedSubEvent { get; set; } = false; // 서브 이벤트 클리어 여부
        public static bool HasCore { get; private set; } = false;    // [신비 : 도시의 핵] 보유 여부

        // 서브 이벤트 풀 (추후 카페 등의 서브 이벤트 등록)
        private static List<Func<Player, GameManager, EventStep>> subEventPool = new();

        /// <summary>
        /// 도시 최초 진입 시 초기화 및 프롤로그 단계 반환
        /// </summary>
        public static EventStep Enter_Area(Player player, GameManager mgr)
        {
            TurnCount = 0;
            HasClearedSubEvent = false;

            // GameManager 상태 변경
            mgr.currentState = ExplorationState.InTheme;
            mgr.currentTheme = ThemeType.City;

            // 도시 배경 로드 (Assets/Resources/Backgrounds/City_Dream)
            Sprite cityBg = Resources.Load<Sprite>("Backgrounds/City_Dream");
            if (cityBg != null)
            {
                mgr.Change_Background(cityBg);
            }

            // 프롤로그 EventStep 구성
            EventStep enterStep = new EventStep();
            enterStep.Title = "[테마 진입 : 고요한 회색 도시]";
            enterStep.BodyText = "평범하기에 이질적으로 보이는 도시가 당신을 반깁니다.\n" +
                                 "마치 시간이 멈춘 듯 차가운 정적이 내려앉은 마천루의 숲.\n" +
                                 "기괴한 안개 사이로 끝없이 이어진 빌딩들이 모습을 드러냅니다.";

            enterStep.Option1Text = "도시를 둘러본다";
            enterStep.OnOption1 = (p, m) =>
            {
                // 도시 탐색 루프 시작
                Show_Theme_Buttons(p, m);
            };

            return enterStep;
        }

        /// <summary>
        /// 도시 내부 턴 진행 (앞으로 나아간다 클릭 시)
        /// </summary>
        public static void Next_Turn(Player player, GameManager mgr)
        {
            TurnCount++;

            // 1. 체류 시간에 따른 오염도 페널티 (정신력 감소)
            Apply_Pollution_Penalty(player, mgr);
            if (mgr.CheckPlayerDeath()) return;

            // 2. 턴 및 조건에 따른 핵심(신비: 도시의 핵) 체크 (임시: 10턴 이상 생존 시 획득)
            Check_Core_Reward(player, mgr);

            // 3. 서브 이벤트 vs 미니 이벤트 분기 (서브 이벤트 25% 확률)
            int roll = UnityEngine.Random.Range(1, 101);
            if (roll <= 25 && subEventPool.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, subEventPool.Count);
                EventStep step = subEventPool[idx].Invoke(player, mgr);
                mgr.Execute_EventStep(step);
                return;
            }

            // 4. 도시 전용 미니 이벤트 출력
            Execute_City_Mini_Event(player, mgr);

            // 5. 버튼 갱신 (탈출 조건 체크 포함)
            Show_Theme_Buttons(player, mgr);
        }

        /// <summary>
        /// 도시 체류 시간에 비례한 정신 오염 페널티
        /// </summary>
        private static void Apply_Pollution_Penalty(Player player, GameManager mgr)
        {
            // 5턴 이하: 1 감소 / 6~10턴: 2 감소 / 11턴 이상: 3 감소
            int penalty = 1;
            if (TurnCount > 10) penalty = 3;
            else if (TurnCount > 5) penalty = 2;

            player.Mental -= penalty;
            mgr.Append_Mini_Log($"<color=#90CAF9>[도시 체류 {TurnCount}턴]</color> 스며드는 도시의 냉기에 정신이 갉아먹힙니다. {Game_System.Red($"[정신력 -{penalty}]")}");
        }

        /// <summary>
        /// 도시 핵심 획득 판정 (추후 조건 확장 가능)
        /// </summary>
        private static void Check_Core_Reward(Player player, GameManager mgr)
        {
            if (!HasCore && TurnCount >= 10)
            {
                HasCore = true;
                mgr.Append_Text($"\n<color=#FFD54F><b>[심상 획득] 마천루의 가장 깊은 적막 속에서 찬란하게 진동하는 [신비 : 도시의 핵]을 손에 넣었습니다!</b></color>\n");
            }
        }

        /// <summary>
        /// 도시 전용 미니 이벤트 임시 처리 (지문 풀 확장 전 기본 구조)
        /// </summary>
        private static void Execute_City_Mini_Event(Player player, GameManager mgr)
        {
            // 임시 도시 지문 목록 (다음 단계에서 City_Mini_Events 분리 가능)
            string[] cityLogs = new string[]
            {
                "매끄러운 통유리 빌딩 벽면에 비친 자신의 얼굴이 흐릿하게 뭉개져 보인다.",
                "신호등 없는 횡단보도를 건너는 사람들의 얼굴에 이목구비가 없다. 아무런 대답도 들려오지 않는다.",
                "깨진 보도블록 틈새로 잡초 대신 가느다란 구리 전선들이 자라나 있다.",
                "꺼진 네온사인 간판에 여러 나라의 알파벳과 한자가 뒤섞여 기괴한 단어를 이루고 있다.",
                "골목 어귀에서 메마른 배수관을 타고 낮게 울리는 물방울 소리가 귓전을 맴돈다."
            };

            int idx = UnityEngine.Random.Range(0, cityLogs.Length);
            mgr.Append_Mini_Log(cityLogs[idx]);
            mgr.Status_UI();
        }

        /// <summary>
        /// 탈출 조건 검사 (13턴 이상 경과 OR 서브 이벤트 1회 클리어)
        /// </summary>
        public static bool Can_Escape()
        {
            return (TurnCount >= 13 || HasClearedSubEvent);
        }

        /// <summary>
        /// 테마 전용 3개 버튼 구성
        /// </summary>
        public static void Show_Theme_Buttons(Player player, GameManager mgr)
        {
            string escapeBtnText = Can_Escape() ? "<color=#81C784>3. 도시를 빠져나간다</color>" : null;

            mgr.Set_Buttons(
                "1. 앞으로 나아간다",
                "2. 상태 확인",
                escapeBtnText,
                () => Next_Turn(player, mgr),
                mgr.Show_Status_Popup,
                Can_Escape() ? () => Exit_Area(player, mgr) : null
            );
        }

        /// <summary>
        /// 도시 탈출 및 안개(얕은 꿈) 복귀
        /// </summary>
        public static void Exit_Area(Player player, GameManager mgr)
        {
            mgr.currentState = ExplorationState.Normal;
            mgr.currentTheme = ThemeType.None;

            // 안개 배경 복구
            Sprite fogBg = Resources.Load<Sprite>("Backgrounds/Fog_Dream");
            mgr.Change_Background(fogBg);

            mgr.Append_Text("\n<b>[테마 탈출]</b>\n도시의 빌딩 숲이 흩어지며, 다시 끝없는 짙은 안개 속으로 발걸음이 이어집니다.\n");

            // 기본 탐험 대기 버튼 복귀
            mgr.Show_Main_UI();
        }
    }
}