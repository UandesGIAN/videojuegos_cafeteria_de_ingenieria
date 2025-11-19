using UnityEngine;


public abstract class BinaryDecisionNode : DecisionNode
{
    public Node YesNode;
    public Node NoNode;

    protected override Node GetBranch(GameObject obj, GameObject world)
    {
        if (Evaluate(obj, world))
        {
            return YesNode;
        }
        else
        {
            return NoNode;
        }
    }

    protected abstract bool Evaluate(GameObject obj, GameObject world);
}
