using UnityEngine;
using TextGame.Core;

namespace TextGame.Events.Theme.City
{
    public static class City_Area
    {
        public static EventStep Enter_Area(Player player)
        {
            string title = "[테마 진입 : 고요한 회색 도시]";
            string bodyText = """
                마치 시간이 멈춘 듯 차가운 정적이 내려앉은 마천루의 숲.
                기괴한 안개 사이로 끝없이 이어진 빌딩들이 모습을 드러낸다.
                """;

            return new EventStep(
                title: title,
                bodyText: bodyText,
                opt1: "1. 도시 안으로 발걸음을 옮긴다",
                opt2: null,
                opt3: null,
                onOption1: (p, mgr) =>
                {
                    mgr.Append_Text("도시의 안개 속으로 들어섭니다.");
                    mgr.Show_Main_UI();
                }
            );
        }
    }
}