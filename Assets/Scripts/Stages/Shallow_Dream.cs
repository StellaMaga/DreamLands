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

            if (roll <= 70)
            {
                MiniEvent mini = Common_Mini_Events.Get_Random(player);
                mini.OnEffect?.Invoke(player);
                mgr.Append_Mini_Log(mini.Text);
                mgr.Status_UI();
            }
            else if (roll <= 90 && oneTimeSubPool.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, oneTimeSubPool.Count);
                EventStep step = oneTimeSubPool[idx].Invoke(player);
                oneTimeSubPool.RemoveAt(idx);

                mgr.Execute_EventStep(step);
            }
            else
            {
                EventStep themeStep = City_Area.Enter_Area(player);
                mgr.Execute_EventStep(themeStep);
            }
        }
    }
}