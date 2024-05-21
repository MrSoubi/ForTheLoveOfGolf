using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static IEnumerator Delay(Action ev, float delay)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }
}
