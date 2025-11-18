using UnityEngine;


public class BoolDecisionNode : BinaryDecisionNode
{
    public bool TestValue;

    protected override bool Evaluate(GameObject obj, GameObject world)
    {
        return TestValue;
    }

    // constructor
    public BoolDecisionNode(bool TestValue)
    {
        this.TestValue = TestValue;
    }
}
