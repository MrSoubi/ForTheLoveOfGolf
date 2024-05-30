using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Animator circularFadeAnim;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void FadeIn()
    {
        circularFadeAnim.SetTrigger("FadeIn");
    }
    public  void FadeOut()
    {
        circularFadeAnim.SetTrigger("FadeOut");
    }
}