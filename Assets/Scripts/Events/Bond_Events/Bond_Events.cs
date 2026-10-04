using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Bond_Events
{
    public static class Bond_Events
    {
        // 1회성 인연 이벤트 풀
        private static List<Func<Player, EventStep>> bondEventPool = new();

        /// <summary>
        /// 인연 이벤트 풀 초기화
        /// </summary>
        public static void Initialize_Pool()
        {
            bondEventPool.Clear();
            // 새로 분리한 1번 흡연자 이벤트 등록
            bondEventPool.Add(Bond_Events_01_Smoker.Get_Step);

            // 추후 2번, 3번 인연 이벤트 추가 시 여기에 등록:
            // bondEventPool.Add(Bond_Events_02_XXX.Get_Step);
        }

        /// <summary>
        /// 10% 확률로 호출되는 인연 이벤트 무작위 추첨 (중복 제거)
        /// </summary>
        public static EventStep Random_Event(Player player)
        {
            // 풀이 비어있다면 초기화
            if (bondEventPool.Count == 0)
            {
                Initialize_Pool();
            }

            // 남아있는 인연 이벤트 중 무작위 추첨
            int randomIndex = UnityEngine.Random.Range(0, bondEventPool.Count);
            var selectedFunc = bondEventPool[randomIndex];

            // 1회 등장한 인연 이벤트는 풀에서 영구 제거 (중복 방지)
            bondEventPool.RemoveAt(randomIndex);

            return selectedFunc.Invoke(player);
        }
    }
}