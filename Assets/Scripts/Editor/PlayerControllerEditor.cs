using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControllerEditor : EditorWindow
{
    [MenuItem("Tools/Player Controller")]
    static void Init()
    {
        var window = (PlayerControllerEditor)GetWindow(typeof(PlayerControllerEditor));
        window.Show();
    }

    PlayerControllerData PCData;

    private void OnEnable()
    {
        PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>("Assets/ScriptableObject/PlayerController/PlayerControllerData.asset");
        Debug.Log(PCData.name);
    }

    private void OnGUI()
    {
        GUILayout.Label("PLAYER CONTROLLER EDITOR", EditorStyles.boldLabel);
        PCData.moveSpeed = EditorGUILayout.FloatField("Acceleration :", PCData.moveSpeed);
    }


}
