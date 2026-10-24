using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;
using TextGame.Events.Mini;
using TextGame.Events.Theme.City;

namespace TextGame.Stages
{
    public static class Stage_ShallowDream
    {
        public static int TurnCount { get; private set; } = 0;

        private static List<Func<Player, EventStep>> oneTimeSubPool = new();

        public static void Initialize()
        {
            TurnCount = 0;
            oneTimeSubPool.Clear();
            // 추후 서브 이벤트 등록 예: oneTimeSubPool.Add(Sub_Events.Event_01);
        }

        public static void Next_Turn(Player player, GameManager mgr)
        {
            TurnCount++;

            if (TurnCount == 13)
            {
                // 13턴 메인 이벤트 자리
                return;
            }

            int roll = UnityEngine.Random.Range(1, 101);

            // 1. 테마 진입 (10% 이하: 91 ~ 100)
            if (roll > 90)
            {
                EventStep themeStep = City_Area.Enter_Area(player);
                mgr.Execute_EventStep(themeStep);
            }
            // 2. 1회성 서브 이벤트 (20%: 71 ~ 90) - 풀에 내용이 있을 때만
            else if (roll > 70 && oneTimeSubPool.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, oneTimeSubPool.Count);
                EventStep step = oneTimeSubPool[idx].Invoke(player);
                oneTimeSubPool.RemoveAt(idx);

                mgr.Execute_EventStep(step);
            }
            // 3. 미니 이벤트 (기본 70% + 서브풀이 없을 때 나머지 전부)
            else
            {
                MiniEvent mini = Common_Mini_Events.Get_Random(player);
                mini.OnEffect?.Invoke(player);

                // 줄바꿈(\n)을 덧붙여 로그 간 간격 확보
                mgr.Append_Mini_Log(mini.Text + "\n");
                mgr.Status_UI();
            }
        }
    }
}