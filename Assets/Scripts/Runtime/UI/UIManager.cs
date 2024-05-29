using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Animator circularFadeAnim;

    public void Fadein()
    {
        circularFadeAnim.SetTrigger("FadeIn");
    }
    public  void FadeOut()
    {
        circularFadeAnim.SetTrigger("FadeOut");
    }
}