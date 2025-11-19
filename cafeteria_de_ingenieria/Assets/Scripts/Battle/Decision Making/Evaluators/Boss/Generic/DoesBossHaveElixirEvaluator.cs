using System.Linq;
using UnityEngine;


public class DoesBossHaveElixirEvaluator : BehaviorEvaluator
{
    // evaluador para saber is el Jefe tiene por lo menos un elixir

    public override bool Evaluate(GameObject obj, GameObject world)
    {
        /*
            // codigo original:

            float healthPercentage = enemyStats.health / enemyStats.startHealth;

            if (healthPercentage < 0.3f)
            {
                Item elixirToUse = enemyStats.GetItems().FirstOrDefault(item => item.itemName == "Elixir");
                
                if (elixirToUse != null)
                {
                    Debug.Log($"¡JEFE {enemyStats.name} está bajo de vida! Usando Elixir.");
                    
                    elixirToUse.Run(); 
                    enemyStats.RemoveItem(elixirToUse);

                    turnUsed = true;
                }
            }
        */

        // obj = Game Object padre del FighterAction del Jefe (es decir, el GameObject del Jefe xd)
        // world esta vacio probablemente

        FighterStats ownerStats = obj.GetComponent<FighterStats>();

        Item elixirToUse = ownerStats.GetItems().FirstOrDefault(item => item.itemName == "Elixir");

        if (elixirToUse) return true;
        else return false;
    }
}
