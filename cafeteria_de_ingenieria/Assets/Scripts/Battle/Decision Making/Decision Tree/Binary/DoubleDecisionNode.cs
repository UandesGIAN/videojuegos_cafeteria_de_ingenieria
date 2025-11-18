using UnityEngine;


public class DoubleDecisionNode : BinaryDecisionNode
{
    public double MinValue;
    public double MaxValue;
    public double TestValue;

    protected override bool Evaluate(GameObject obj, GameObject world)
    {
        return TestValue >= MinValue && TestValue <= MaxValue;
    }

    // constructor
    public DoubleDecisionNode(double MinValue, double MaxValue, double TestValue)
    {
        this.MinValue = MinValue;
        this.MaxValue = MaxValue;
        this.TestValue = TestValue;
    }
}
