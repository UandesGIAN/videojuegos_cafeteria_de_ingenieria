using UnityEngine;


public class IsBossHealthBelowXPercentEvaluator : BehaviorEvaluator
{
    // evaluador para saber si la vida del Jefe se encuentra bajo un porcentaje particular
    private float targetHealthPercent;

    public override bool Evaluate(GameObject obj, GameObject world)
    {
        // obj = Game Object padre del FighterAction del Jefe (es decir, el GameObject del Jefe xd)
        // world esta vacio probablemente
        FighterStats ownerStats = obj.GetComponent<FighterStats>();

        float healthPercentage = ownerStats.health / ownerStats.startHealth;

        if (healthPercentage < targetHealthPercent) return true;
        else return false;
    }

    // constructor
    public IsBossHealthBelowXPercentEvaluator(float targetHealthPercent)
    {
        this.targetHealthPercent = targetHealthPercent;
    }
}
