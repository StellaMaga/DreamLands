using System;

namespace TextGame.Core
{
    public class Knowledge
    {
        public string ID { get; set; }          // 고유 식별자 (예: "Dreamer", "ForbiddenTome")
        public string Name { get; set; }        // 표시 이름 (예: "몽상가", "금단의 고서")
        public string Category { get; set; }    // 분류 (예: "인물", "세계관", "금기")
        public string Content { get; set; }     // 상세 설명

        // [습득 시 스탯 변동치]
        public int BonusWill { get; set; } = 0;       // 의지 변동
        public int BonusSanity { get; set; } = 0;     // 이성 변동
        public int BonusMental { get; set; } = 0;     // 정신력 변동
        public int BonusMadness { get; set; } = 0;    // 광기 변동
        public int BonusLuck { get; set; } = 0;       // 행운 변동

        // [위험 지식(금기) 설정]
        public bool IsDangerous { get; set; } = false; // 습득 시 이성 체크 필요 여부
        public int SanityLossOnFail { get; set; } = 0; // 이성 체크 실패 시 이성 손실 (0이면 1D6 굴림)
        public int MadnessGainOnFail { get; set; } = 0;// 이성 체크 실패 시 광기 증가 (0이면 1D3 굴림)

        public Knowledge(string id, string name, string category, string content, bool isDangerous = false)
        {
            ID = id;
            Name = name;
            Category = category;
            Content = content;
            IsDangerous = isDangerous;
        }
    }
}