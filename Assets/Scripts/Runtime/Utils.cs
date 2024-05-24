using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Pour appeler des fonctions avec un delai
    /// </summary>
    /// <param name="ev"> Passer la fonction à appeler de cette maniere ()=> ... </param>
    /// <param name="delay">Temps de delai </param>
    /// <returns></returns>
    public static IEnumerator Delay(Action ev, float delay)
    {
        yield return new WaitForSeconds(delay);
        ev?.Invoke();
    }


    /// <summary>
    /// Pour créer des singleton:
    /// 1) Déclarer une variable static de cette manière: public static T instance {  get; private set; } (T = type de la classe, ex: SoundManager)
    /// 2) Dans le Awake, copier coller la ligne suivante: instance = this.Singleton(instance, () => Destroy(gameObject));
    /// </summary>
    /// <typeparam name="T">Type de l'object</typeparam>
    /// <param name="object">L'object depuis lequel on appel la fonction = this</param>
    /// <param name="instance">L'instance du singleton </param>
    /// <param name="callbackDestroy">Function de callback pour detruire l'object</param>
    /// <returns></returns>
    public static T Singleton<T>(this T @object, T instance, Action callbackDestroy) where T : MonoBehaviour
    {
        if (instance != null && !@object.Equals(instance))
        {
            Debug.LogWarning(message: $"deleted, {@object} already exist in world!");
            callbackDestroy?.Invoke();
            return instance;
        }
        instance = @object;
        return instance;
    }
}
