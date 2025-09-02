using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [Header("Base Skill")]
    public string skillName;
    public string description;
    public float animationDuration;

    public bool selfinflicted;

    //public GameObject effectPrfb;
    //Para futuros efectos visuales

    protected FighterStats userStats;
    protected FighterStats targetStats;

    //private void Animate()
    //{
    //    var go = Instantiate(this.effectPrfb, this.targetStats.transform.position, Quaternion.identity);
    //    Destroy(go, this.animationDuration);
    //}

    public void Run()
    {
        if (this.selfinflicted)
        {
            this.targetStats = this.userStats;
        }

        //this.Animate();

        this.onRun();
    }

    public void SetTargetanduser(FighterStats _user, FighterStats _target)
    {
        this.userStats = _user;
        this.targetStats = _target;
    }

    public abstract void onRun();
}
