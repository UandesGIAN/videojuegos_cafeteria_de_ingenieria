using UnityEngine;


public class ActionNode : Node
{
    public string name;

    public override Node Decide(GameObject obj, GameObject world)
    {
        return this;
    }

    // constructor
    public ActionNode(string name)
    {
        this.name = name;
    }
}
