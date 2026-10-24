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
        [HideInInspector] public int currentHp; // 런타임 현재 체력
        public int damage = 5;

        [Header("특수 기믹 (EnemyGimmick 에셋 등록)")]
        public EnemyGimmick gimmick;

        [Header("상황별 고유 지문")]
        [TextArea(2, 4)] public string encounterText;
        [TextArea(2, 4)] public string ambushText;
        [TextArea(2, 4)] public string attackText;
        [TextArea(2, 4)] public string hitText;
        [TextArea(2, 4)] public string deathText;

        // 런타임용 인스턴스 생성 시 현재 체력을 최대 체력으로 동기화
        public EnemyData CreateInstance()
        {
            EnemyData clone = Instantiate(this);
            clone.currentHp = clone.maxHp;
            return clone;
        }

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