namespace TextGame.Core
{
    public static class EnemyDatabase
    {
        public static Enemy GetEnemy(EnemyType type)
        {
            return type switch
            {
                EnemyType.Doppelganger => new Doppelganger(),
                _ => new Doppelganger()
            };
        }
    }
}