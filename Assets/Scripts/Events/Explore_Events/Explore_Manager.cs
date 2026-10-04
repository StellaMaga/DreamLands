using System;
using System.Collections.Generic;
using UnityEngine;
using TextGame.Core;
using TextGame.Events.Explore.Desert;

namespace TextGame.Events.Explore
{
    public static class Explore_Manager
    {
        private static List<Func<Player, EventStep>> areaList = new();

        private static void Initialize_Areas()
        {
            areaList.Clear();
            areaList.Add(Desert_Area.Enter_Area);
        }

        public static EventStep Random_Explore(Player player)
        {
            if (areaList.Count == 0)
            {
                Initialize_Areas();
            }

            int randomIndex = UnityEngine.Random.Range(0, areaList.Count);
            return areaList[randomIndex].Invoke(player);
        }
    }
}