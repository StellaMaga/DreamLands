using UnityEngine;

namespace TextGame.Core
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "TextGame/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("기본 정보")]
        public string enemyId;
        public string enemyName;

        [Header("스탯")]
        [Range(1, 100)] public int detection = 40;
        public int maxHp = 2;
        public int damage = 5;

        [Header("특수 기믹 (EnemyGimmick 에셋 등록)")]
        public EnemyGimmick gimmick;

        [Header("상황별 고유 지문")]
        [TextArea(2, 4)] public string encounterText;  // 최초 조우 지문
        [TextArea(2, 4)] public string ambushText;     // 탐지 실패(기습) 지문
        [TextArea(2, 4)] public string attackText;     // 적 공격 시 지문
        [TextArea(2, 4)] public string hitText;        // 적 피격 시 지문
        [TextArea(2, 4)] public string deathText;      // 적 처치(사망) 지문

        // 런타임용 복제본 생성
        public EnemyData CreateInstance()
        {
            return Instantiate(this);
        }

        // 해당 타이밍에 등록된 기믹이 있는지 검사하고 발동
        public string CheckAndTriggerGimmick(GimmickTriggerTime timing, Player player)
        {
            if (gimmick != null && gimmick.triggerTime == timing)
            {
                return gimmick.Execute(player);
            }
            return null;
        }
    }
}