using System;
using UnityEngine;

namespace TextGame.Core
{
    // 적 종류 열거형
    public enum EnemyType
    {
        Doppelganger,
        // 새로운 적이 추가될 때 여기에 열거형 추가
    }

    public class Enemy
    {
        public string Name { get; set; }            // 이름
        public string Description { get; set; }     // 기본 지문
        public string Ability { get; set; }         // 고유 능력
        public int Ego { get; set; }                // 에고 레벨
        public int Detection { get; set; }          // 탐지(DET) 판정 난이도
        public int HP { get; set; }                 // 체력
        public int Damage { get; set; }             // 공격력 (정신력 피해)
        public string DeathMessage { get; set; }    // 패배 엔딩 문구

        // 적 고유 기믹/특수 효과 가상 메서드 (자식 클래스에서 필요 시 override)
        public virtual void Doppelganger_Curse(Player player, CoC_Result result) { }
        public virtual void OnAttack(Player player) { }
    }
}