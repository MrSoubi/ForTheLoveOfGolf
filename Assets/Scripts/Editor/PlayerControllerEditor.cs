using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerEditor : EditorWindow
{
    private PlayerControllerData PCData;

    private bool showMovementSettings = true;
    private bool showShootingSettings = true;
    private bool showInputSettings = true;
    private bool showGizmosSettings = true;

    private bool showNormal = true;
    private bool showGravity = true;
    private bool showAcceleration = true;
    private bool showFriction = true;
    private bool showDirection = true;

    private GUIStyle headerStyle;

    [MenuItem("Tools/Player Controller")]
    static void Init()
    {
        var window = (PlayerControllerEditor)GetWindow(typeof(PlayerControllerEditor));
        window.Show();
    }

    private void OnEnable()
    {
        PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>("Assets/ScriptableObject/PlayerController/PlayerControllerData.asset");

        
    }

    private void OnGUI()
    {
        headerStyle = new GUIStyle(EditorStyles.foldout);
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;

        float padding = 15;
        Rect area = new Rect(padding, padding, position.width - padding * 2f, position.height - padding * 2f);

        GUILayout.BeginArea(area);
        showMovementSettings = EditorGUILayout.Foldout(showMovementSettings, "Movement Settings", true, headerStyle);
        if (showMovementSettings)
        {
            PCData.moveSpeed = EditorGUILayout.FloatField("Acceleration", PCData.moveSpeed);
            PCData.maxSpeed = EditorGUILayout.FloatField("Speed", PCData.maxSpeed);
            PCData.airMultiplier = EditorGUILayout.FloatField("AirMultiplier", PCData.airMultiplier);
            PCData.rotationSpeed = EditorGUILayout.FloatField("RotationSpeed", PCData.rotationSpeed);
            PCData.gravityForce = EditorGUILayout.FloatField("Gravity", PCData.gravityForce);
            PCData.yCurve = EditorGUILayout.CurveField("Gravity Curve", PCData.yCurve);
        }

        showShootingSettings = EditorGUILayout.Foldout(showShootingSettings, "Shooting Settings", true, headerStyle);
        if (showShootingSettings)
        {
            PCData.shootForce = EditorGUILayout.FloatField("Power", PCData.shootForce);

            PCData.shootCharges = EditorGUILayout.IntField("Charges", PCData.shootCharges);
            PCData.maxShootCharges = EditorGUILayout.IntField("Max Charges", PCData.maxShootCharges);
        }

        showInputSettings = EditorGUILayout.Foldout(showInputSettings, "Inputs", true, headerStyle);
        if (showInputSettings)
        {
            PCData.aimingInput = (KeyCode)EditorGUILayout.EnumPopup("Aim", PCData.aimingInput);
            PCData.shootInput = (KeyCode)EditorGUILayout.EnumPopup("Shoot", PCData.shootInput);
        }

        showGizmosSettings = EditorGUILayout.Foldout(showGizmosSettings, "Gizmos Settings", true, headerStyle);
        if (showGizmosSettings)
        {
            PCData.factor = EditorGUILayout.FloatField("Factor", PCData.factor);

            showNormal = EditorGUILayout.Foldout(showNormal, "Normal", true, headerStyle);
            if (showNormal)
            {
                PCData.drawNormal = EditorGUILayout.Toggle("Draw", PCData.drawNormal);
                PCData.normalColor = EditorGUILayout.ColorField("Color", PCData.normalColor);
            }

            showGravity = EditorGUILayout.Foldout(showGravity, "Gravity", true, headerStyle);
            if (showGravity)
            {
                PCData.drawGravity = EditorGUILayout.Toggle("Draw", PCData.drawGravity);
                PCData.gravityColor = EditorGUILayout.ColorField("Color", PCData.gravityColor);
            }

            showAcceleration = EditorGUILayout.Foldout(showAcceleration, "Acceleration", true, headerStyle);
            if (showAcceleration)
            {
                PCData.drawAcceleration = EditorGUILayout.Toggle("Draw", PCData.drawAcceleration);
                PCData.accelerationColor = EditorGUILayout.ColorField("Color", PCData.accelerationColor);
            }

            showFriction = EditorGUILayout.Foldout(showFriction, "Friction", true, headerStyle);
            if (showFriction)
            {
                PCData.drawFriction = EditorGUILayout.Toggle("Draw", PCData.drawFriction);
                PCData.frictionColor = EditorGUILayout.ColorField("Color", PCData.frictionColor);
            }

            showDirection = EditorGUILayout.Foldout(showDirection, "Direction", true, headerStyle);
            if (showDirection)
            {
                PCData.drawDirection = EditorGUILayout.Toggle("Draw", PCData.drawDirection);
                PCData.directionColor = EditorGUILayout.ColorField("Color", PCData.directionColor);
            }
        }

        GUILayout.EndArea();
    }


}
