using UnityEngine;

//Todos los tipos de escalados o calculos que se tienen para la modificacion de salud
public enum HealthModType
{
    Flat, Percentage
}

public class HealthModSkill : Skill
{
    [Header("Health Mod Skill")]
    public float amount;
    public float cost;

    public HealthModType modType;

    public override void onRun()
    {
        float amount = this.GetModification();

        if (this.userStats.IQ >= this.cost)
        {
            this.userStats.ModifyIQ(-this.cost);
            
            if(this.selfinflicted)
            {
                this.targetStats.Heal(amount);
            }
            else
            {
                this.targetStats.ReceiveDamage(amount);
            }
        }
        else
        {
            Debug.Log("No hay IQ suficiente para usar la skill: " + this.skillName);
        }
    }

    public float GetModification()
    {
        switch (this.modType)
        {
            case HealthModType.Flat:
                return this.amount;
            case HealthModType.Percentage:
                return this.targetStats.health * (this.amount / 100);
            default:
                return 0;
        }
    }
}
