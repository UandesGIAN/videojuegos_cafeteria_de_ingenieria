using UnityEngine;


public class ObjectDecisionNode : BinaryDecisionNode
{
    private BehaviorEvaluator evaluator;

    protected override bool Evaluate(GameObject obj, GameObject world)
    {
        return evaluator.Evaluate(obj, world);
    }
    
    // constructor
    public ObjectDecisionNode(BehaviorEvaluator evaluator)
    {
        this.evaluator = evaluator;
    }
}
