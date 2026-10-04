using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Explore.Desert
{
    public static class Desert_Area
    {
        private static List<Func<Player, EventStep>> desertEventPool = new();

        public static void Initialize_Pool()
        {
            desertEventPool.Clear();
            // 옮겨온 작은 꽃 이벤트 등록
            desertEventPool.Add(Desert_01_Floret.Get_Step);
        }

        public static EventStep Enter_Area(Player player)
        {
            // 배경 이미지가 있으면 로드 (없을 경우 null 처리되어 이전 배경 유지)
            Sprite desertBg = Resources.Load<Sprite>("Backgrounds/Desert_BG");

            string title = "[탐험 지역 : 별을 품은 사막]";
            string bodyText = """
                하늘에는 강렬한 태양과 은은한 별들이 오가고,
                땅에는 황금빛 모래가 아름답게 흐르는 장소.

                수많은 이야기를 품은 사막이 당신의 눈앞에 펼쳐진다.
                """;

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 앞으로 나아간다",
                opt2: null,
                opt3: null,
                onOption1: (p, mgr) =>
                {
                    mgr.Execute_EventStep(Get_Random_Desert_Event(p));
                },
                bgImage: desertBg
            );
        }

        private static EventStep Get_Random_Desert_Event(Player player)
        {
            if (desertEventPool.Count == 0)
            {
                Initialize_Pool();
            }

            int randomIndex = UnityEngine.Random.Range(0, desertEventPool.Count);
            var selectedFunc = desertEventPool[randomIndex];
            desertEventPool.RemoveAt(randomIndex); // 1회성 등장 보장

            return selectedFunc.Invoke(player);
        }
    }
}