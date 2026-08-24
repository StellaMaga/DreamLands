using System;
using UnityEngine;

namespace TextGame.Core
{
    public enum MysticType
    {
        Item,
        Mystic
    }

    public class Mystic
    {
        public string ID { get; set; }              // 고유 식별자 (예: "Murramiku", "ChallengerDice")
        public string Name { get; set; }            // 표시 이름 (예: "머러미쿠", "도전자의 주사위")
        public string Description { get; set; }     // 플레이버 텍스트 및 상세 설명
        public MysticType Type { get; set; }        // 분류 (Item, Mystic)


        // [스탯 보정치 (습득 시 자동 적용)]
        public int BonusWill { get; set; } = 0;
        public int BonusSanity { get; set; } = 0;
        public int BonusMental { get; set; } = 0;
        public int BonusMadness { get; set; } = 0;
        public int BonusCharm { get; set; } = 0;
        public int BonusLuck { get; set; } = 0;
        public int BonusStarlight { get; set; } = 0;
        public int DetectionBonus { get; set; } = 0;

        public Mystic(string id, string name, string description, MysticType type)
        {
            ID = id;
            Name = name;
            Description = description;
            Type = type;
        }
    }
}