using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextGame.Core
{
    public class Player
    {
        // [기초 스탯]
        private int mental;             // 정신력
        private int will;               // 의지
        private int sanity;             // 이성
        private int madness = 0;        // 광기
        private int charm = 0;          // 매력
        private int luck = 0;           // 행운
        private int starlight = 0;      // 별빛

        // [플레이어 기본 상태]
        public string Name { get; set; }            // 이름
        public string Alignment { get; set; }       // 성향  

        // [3대 기본 스탯 총합]
        public int TotalStats => Mental + Will + Sanity;

        // [시스템 및 전투 계산용 자아 단계 (Lv. 1 ~ EX)]
        public int Ego
        {
            get
            {
                int total = TotalStats;
                if (total >= 200) return 6;
                if (total >= 180) return 5;
                if (total >= 150) return 4;
                if (total >= 130) return 3;
                if (total >= 80) return 2;
                return 1;
            }
        }

        // [상태창 UI 표시용 등급]
        public string Ego_LV
        {
            get
            {
                int total = TotalStats;
                if (total >= 200) return "EX";
                if (total >= 180) return "Lv. 5";
                if (total >= 150) return "Lv. 4";
                if (total >= 130) return "Lv. 3";
                if (total >= 80) return "Lv. 2";
                return "Lv. 1";
            }
        }

        // [음수 방지 프로퍼티]
        public int Mental
        {
            get => mental;
            set => mental = Mathf.Max(0, value);
        }

        public int Will
        {
            get => will;
            set => will = Mathf.Max(0, value);
        }

        public int Sanity
        {
            get => sanity;
            set => sanity = Mathf.Max(0, value);
        }

        public int Madness
        {
            get => madness;
            set => madness = Mathf.Max(0, value);
        }

        public int Charm
        {
            get => charm;
            set => charm = Mathf.Max(0, value);
        }

        public int Luck
        {
            get => luck;
            set => luck = Mathf.Max(0, value);
        }

        public int Starlight
        {
            get => starlight;
            set => starlight = Mathf.Max(0, value);
        }

        // 사망 판정
        public bool IsDead { get; private set; } = false;
        public DeathType CauseOfDeath { get; private set; } = DeathType.None;

        public void Die(DeathType deathType)
        {
            IsDead = true;
            CauseOfDeath = deathType;
        }

        //  [파생 스탯]
        public int Interference => (Will + Mental) / 2;     // 간섭(IFN)
        public int Rejection => (Will + Sanity) / 2;        // 거부(REJ)
        public int Detection => (Mental + Sanity) / 2;      // 탐지(DET)


        // [신비 및 아이템 관리 (Mystic)]
        public List<Mystic> Mystics = new List<Mystic>();

        // 분류별 목록 조회
        public List<Mystic> ItemList => Mystics.FindAll(x => x.Type == MysticType.Item);
        public List<Mystic> MysticList => Mystics.FindAll(x => x.Type == MysticType.Mystic);

        // [보유 여부 검사 (ID 또는 Name)]
        public bool HasMystic(string idOrName)
        {
            return Mystics.Exists(x => x.ID == idOrName || x.Name == idOrName);
        }

        public bool HasItem(string idOrName) => HasMystic(idOrName);

        // 습득 및 스탯 반영
        public void ObtainMystic(Mystic mystic)
        {
            if (mystic == null || HasMystic(mystic.ID)) return;

            Mystics.Add(mystic);

            Will += mystic.BonusWill;
            Sanity += mystic.BonusSanity;
            Mental += mystic.BonusMental;
            Madness += mystic.BonusMadness;
            Charm += mystic.BonusCharm;
            Luck += mystic.BonusLuck;
            Starlight += mystic.BonusStarlight;

            string tag = mystic.Type == MysticType.Item ? "[아이템 획득]" : "[신비 습득]";
            Debug.Log(Game_System.Green($"{tag} {mystic.Name} : {mystic.Description}"));
        }

        public void ObtainItem(Mystic mystic) => ObtainMystic(mystic);



        // [지식 관리 (Knowledge)]
        public List<Knowledge> KnowledgeList = new List<Knowledge>();

        public bool HasKnowledge(string idOrName)
        {
            return KnowledgeList.Exists(x => x.ID == idOrName || x.Name == idOrName);
        }

        public int KnowledgeCount => KnowledgeList.Count;

        // 지식 습득 (위험 지식 판정 로직 포함)
        public void ObtainKnowledge(Knowledge knowledge, out string checkLog)
        {
            checkLog = "";
            if (knowledge == null || HasKnowledge(knowledge.ID)) return;

            KnowledgeList.Add(knowledge);

            // 스탯 변동치 합산
            Will += knowledge.BonusWill;
            Sanity += knowledge.BonusSanity;
            Mental += knowledge.BonusMental;
            Madness += knowledge.BonusMadness;
            Luck += knowledge.BonusLuck;

            // 위험 지식일 경우 이성 체크(CoC_Check) 진행
            if (knowledge.IsDangerous)
            {
                CoC_Result result = Game_System.CoC_Check(Sanity); 

                if (result == CoC_Result.Jackpot || result == CoC_Result.Success)
                {
                    checkLog = Game_System.Cyan($"\n[이성 방어 성공] 금단의 지식 [{knowledge.Name}]의 충격을 견뎌냈습니다.");
                }
                else if (result == CoC_Result.Failure)
                {
                    int loss = knowledge.SanityLossOnFail > 0 ? knowledge.SanityLossOnFail : Game_System.Roll_D6(); 
                    Sanity -= loss;
                    checkLog = Game_System.Red($"\n[이성 방어 실패] 진실의 무게를 감당하지 못했습니다. (이성 -{loss})");
                }
                else // Fumble
                {
                    int loss = knowledge.SanityLossOnFail > 0 ? knowledge.SanityLossOnFail : Game_System.Roll_D6(); 
                    int gain = knowledge.MadnessGainOnFail > 0 ? knowledge.MadnessGainOnFail : Game_System.Roll_D6(); 
                    Sanity -= loss;
                    Madness += gain;
                    checkLog = Game_System.Purple($"\n[이성 판정 펌블!] 금단의 지식에 잠식당합니다! (이성 -{loss}, 광기 +{gain})");
                }
            }

            Debug.Log(Game_System.Cyan($"[지식 습득: {knowledge.Name}]") + checkLog);
        }

        public void ObtainKnowledge(Knowledge knowledge)
        {
            ObtainKnowledge(knowledge, out _);
        }


        // [생성자 및 상태 출력]
        public Player(string name, DreamInfo dream)
        {
            Name = name;
            Alignment = dream.Title; 

            Mental = Game_System.Roll_3D6_Times5(); 
            Sanity = Game_System.Roll_3D6_Times5(); 
            Will = Game_System.Roll_3D6_Times5(); 

            Madness = 0;
            Charm = 0;
            Luck = 0;
            Starlight = 0;
        }

        public void ShowStatus()
        {
            Debug.Log($"[ 몽상가 {Name}({Alignment})의 상태 ]\n" +
                      $"- 자아(Ego) : {Ego_LV}\n" +
                      $"- 정신력(Mental) : {Mental}\n" +
                      $"- 의지(Will) : {Will}\n" +
                      $"- 이성(Sanity) : {Sanity}\n" +
                      $"- 광기(Madness) : {Madness}\n" +
                      $"- 소지 아이템/신비 : {Mystics.Count}개\n" +
                      $"- 보유 지식 : {KnowledgeCount}개");
        }

        public DialogueNode[] Status_Dialogues()
        {
            return new DialogueNode[]
            {
                new($"[ 몽상가 {Name}의 상태 ]\n성향: {Alignment} | 자아: {Ego_LV}\n" +
                $"정신력: {Mental} | 의지: {Will} | 이성: {Sanity}\n" +
                $"간섭: {Interference} | 거부: {Rejection} | 탐지: {Detection}\n" +
                $"아이템/신비: {Mystics.Count}개 | 보유 지식: {KnowledgeCount}개\n" +
                $"광기: {Madness} | 별빛: {Starlight}")
            };
        }
    }
}