using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public Sprite sprite;

    [Header("Stats")]
    public int maxHP = 100;
    public int attack = 10;
    public int defense = 5;

    //[Header("Skills")]
    //public SkillData[] skills;
}
