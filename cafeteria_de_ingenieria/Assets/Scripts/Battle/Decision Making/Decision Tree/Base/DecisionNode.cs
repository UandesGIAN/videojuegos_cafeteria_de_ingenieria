using UnityEngine;


public abstract class DecisionNode : Node
{
    public override Node Decide(GameObject obj, GameObject world)
    {
        return GetBranch(obj, world).Decide(obj, world);
    }

    protected abstract Node GetBranch(GameObject obj, GameObject world);
}
