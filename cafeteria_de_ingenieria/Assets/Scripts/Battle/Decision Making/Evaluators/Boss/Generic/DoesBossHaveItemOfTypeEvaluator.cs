using UnityEngine;


public class DoesBossHaveItemOfTypeEvaluator : BehaviorEvaluator
{
    // evaluador para saber si el Jefe posee un item de un tipo particular
    public StatsType itemType;


    public override bool Evaluate(GameObject obj, GameObject world)
    {
        // obj = Game Object padre del FighterAction del Jefe (es decir, el GameObject del Jefe xd)
        // world esta vacio probablemente
        Item[] items = obj.GetComponent<FighterStats>().GetItems();

        foreach (Item item in items)
        {
            StatsModItems statModItem = item as StatsModItems;
            
            // si el item es de tipo StatsModItems y su statType coincide con el itemType buscado, se retorna true
            if (statModItem != null && statModItem.statType == itemType)
            {
                Debug.Log($"ALERTA!!! {obj.GetComponent<FighterStats>().fightername} POSEE UN ITEM DE TIPO {itemType}");
                return true;
            }
        }

        // e.o.c., no se encontro item del tipo buscado
        Debug.Log($"ALERTA!!!: {obj.GetComponent<FighterStats>().fightername} NO POSEE ITEMS DE TIPO {itemType}");
        return false;
    }

    // constructor
    public DoesBossHaveItemOfTypeEvaluator(StatsType itemType)
    {
        this.itemType = itemType;
    }
}
