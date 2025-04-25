using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBalloon : MonoBehaviour
{
    public Animator anim;

    public void SetCount(int pFailureNumber, int pCacheNumber)
    {
        anim.SetInteger("Number", pCacheNumber);
        anim.SetInteger("BreakNumber", pCacheNumber - pFailureNumber);
    }

    public void ReOpen()
    {
        anim.SetInteger("Number", 3);
        anim.SetInteger("BreakNumber", 0);
    }
}
