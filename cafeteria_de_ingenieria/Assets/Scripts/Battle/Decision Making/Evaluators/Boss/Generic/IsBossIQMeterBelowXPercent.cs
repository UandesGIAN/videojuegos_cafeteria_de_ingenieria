using UnityEngine;


public class IsBossIQMeterBelowXPercentEvaluator : BehaviorEvaluator
{
    // evaluador para saber si la barra de IQ del Jefe se encuentra bajo un porcentaje particular
    private float targetIQPercent;

    public override bool Evaluate(GameObject obj, GameObject world)
    {
        // obj = Game Object padre del FighterAction del Jefe (es decir, el GameObject del Jefe xd)
        // world esta vacio probablemente
        FighterStats ownerStats = obj.GetComponent<FighterStats>();

        float iQPercentage = ownerStats.IQ / ownerStats.startIQ;

        if (iQPercentage < targetIQPercent) {
            Debug.Log($"ALERTA!!! {ownerStats.fightername} TIENE IQ POR DEBAJO DE {targetIQPercent * 100}% ({iQPercentage * 100}%)");
            return true;
        }
        else return false;
    }

    // constructor
    public IsBossIQMeterBelowXPercentEvaluator(float targetIQPercent)
    {
        this.targetIQPercent = targetIQPercent;
    }
}
