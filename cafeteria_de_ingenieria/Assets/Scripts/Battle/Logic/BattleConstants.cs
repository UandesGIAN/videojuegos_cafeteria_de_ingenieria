public static class BattleConstants
{
    // para evitar magic strings xd
    public enum CharacterRole
    {
        Player,
        Enemy,
        Dead
    }

    // nombres que las subfunciones de BattleManager.ActivateOption() pasan a FighterAction
    public enum MenuAttackOptions
    {
        Melee,
        Skill,
        Item
    }
}