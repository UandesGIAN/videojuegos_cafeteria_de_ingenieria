using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [Header("Base Item")]
    public string itemName;
    public string description;
    public Sprite icon;
    public float animationDuration;
    public int amount;

    public void Run()
    {
        //this.Animate();

        this.onRun();
    }   
    public abstract void onRun();
}
