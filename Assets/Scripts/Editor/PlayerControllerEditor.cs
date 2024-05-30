using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerEditor : EditorWindow
{
    private PlayerControllerData PCData;

    private string profileName = "NewProfile";
    private List<string> profileNames = new List<string>();
    private int selectedProfileIndex = 0;

    private Vector2 scrollPos = Vector2.zero;

    private bool showPlayerController = true;

    private bool showMovementSettings = true;
    private bool showShootingSettings = true;
    private bool showInputSettings = true;
    private bool showGizmosSettings = true;

    private bool showNormal = true;
    private bool showGravity = true;
    private bool showAcceleration = true;
    private bool showFriction = true;
    private bool showDirection = true;

    private GUIStyle header1Style;
    private GUIStyle header2Style;

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
        header1Style = new GUIStyle(EditorStyles.foldout);
        header1Style.fontStyle = FontStyle.Bold;
        header1Style.normal.textColor = Color.white;

        header2Style = new GUIStyle(EditorStyles.foldout);
        header2Style.normal.textColor = Color.white;

        float padding = 15;
        Rect area = new Rect(padding, padding, position.width - padding * 2f, position.height - padding * 2f);
        GUILayout.BeginArea(area);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));    
        showPlayerController = EditorGUILayout.Foldout(showPlayerController, "Player Controller", true, header1Style);
        if (showPlayerController)
        {
            
            EditorGUI.indentLevel++;
            showMovementSettings = EditorGUILayout.Foldout(showMovementSettings, "Movement Settings", true, header1Style);
            if (showMovementSettings)
            {
                EditorGUI.indentLevel++;
                PCData.moveSpeed = EditorGUILayout.FloatField("Acceleration", PCData.moveSpeed);
                PCData.maxSpeed = EditorGUILayout.FloatField("Speed", PCData.maxSpeed);
                PCData.airMultiplier = EditorGUILayout.FloatField("AirMultiplier", PCData.airMultiplier);
                PCData.rotationSpeed = EditorGUILayout.FloatField("RotationSpeed", PCData.rotationSpeed);
                PCData.gravityForce = EditorGUILayout.FloatField("Gravity", PCData.gravityForce);
                PCData.yCurve = EditorGUILayout.CurveField("Gravity Curve", PCData.yCurve);
                EditorGUI.indentLevel--;
            }

            showShootingSettings = EditorGUILayout.Foldout(showShootingSettings, "Shooting Settings", true, header1Style);
            if (showShootingSettings)
            {
                EditorGUI.indentLevel++;
                PCData.shootForce = EditorGUILayout.FloatField("Power", PCData.shootForce);

                PCData.shootCharges = EditorGUILayout.IntField("Charges", PCData.shootCharges);
                PCData.maxShootCharges = EditorGUILayout.IntField("Max Charges", PCData.maxShootCharges);
                EditorGUI.indentLevel--;
            }

            showInputSettings = EditorGUILayout.Foldout(showInputSettings, "Inputs", true, header1Style);
            if (showInputSettings)
            {
                EditorGUI.indentLevel++;
                PCData.aimingInput = (KeyCode)EditorGUILayout.EnumPopup("Aim", PCData.aimingInput);
                PCData.shootInput = (KeyCode)EditorGUILayout.EnumPopup("Shoot", PCData.shootInput);
                EditorGUI.indentLevel--;
            }

            showGizmosSettings = EditorGUILayout.Foldout(showGizmosSettings, "Gizmos Settings", true, header1Style);
            if (showGizmosSettings)
            {
                EditorGUI.indentLevel++;
                PCData.factor = EditorGUILayout.FloatField("Factor", PCData.factor);

                showNormal = EditorGUILayout.Foldout(showNormal, "Normal", true, header2Style);
                if (showNormal)
                {
                    EditorGUI.indentLevel++;
                    PCData.drawNormal = EditorGUILayout.Toggle("Draw", PCData.drawNormal);
                    PCData.normalColor = EditorGUILayout.ColorField("Color", PCData.normalColor);
                    EditorGUI.indentLevel--;
                }

                showGravity = EditorGUILayout.Foldout(showGravity, "Gravity", true, header2Style);
                if (showGravity)
                {
                    EditorGUI.indentLevel++;
                    PCData.drawGravity = EditorGUILayout.Toggle("Draw", PCData.drawGravity);
                    PCData.gravityColor = EditorGUILayout.ColorField("Color", PCData.gravityColor);
                    EditorGUI.indentLevel--;
                }

                showAcceleration = EditorGUILayout.Foldout(showAcceleration, "Acceleration", true, header2Style);
                if (showAcceleration)
                {
                    EditorGUI.indentLevel++;
                    PCData.drawAcceleration = EditorGUILayout.Toggle("Draw", PCData.drawAcceleration);
                    PCData.accelerationColor = EditorGUILayout.ColorField("Color", PCData.accelerationColor);
                    EditorGUI.indentLevel--;
                }

                showFriction = EditorGUILayout.Foldout(showFriction, "Friction", true, header2Style);
                if (showFriction)
                {
                    EditorGUI.indentLevel++;
                    PCData.drawFriction = EditorGUILayout.Toggle("Draw", PCData.drawFriction);
                    PCData.frictionColor = EditorGUILayout.ColorField("Color", PCData.frictionColor);
                    EditorGUI.indentLevel--;
                }

                showDirection = EditorGUILayout.Foldout(showDirection, "Direction", true, header2Style);
                if (showDirection)
                {
                    EditorGUI.indentLevel++;
                    PCData.drawDirection = EditorGUILayout.Toggle("Draw", PCData.drawDirection);
                    PCData.directionColor = EditorGUILayout.ColorField("Color", PCData.directionColor);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        if (PCData != null)
        {
            selectedProfileIndex = EditorGUILayout.Popup("Select Profile", selectedProfileIndex, profileNames.ToArray());

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Profile"))
            {
                LoadProfile(profileNames[selectedProfileIndex]);
            }
            if(GUILayout.Button("Delete Profile"))
            {
                DeleteProfile(profileNames[selectedProfileIndex]);
            }
            EditorGUILayout.EndHorizontal();

            profileName = EditorGUILayout.TextField("Profile Name", profileName);

            if (GUILayout.Button("Save Profile"))
            {
                SaveProfile(profileName, PCData);
                RefreshProfileNames();
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void SaveProfile(string profileName, PlayerControllerData settings)
    {
        string path = $"Assets/Profiles/{profileName}.asset";
        PlayerControllerData newProfile = ScriptableObject.CreateInstance<PlayerControllerData>();
        EditorUtility.CopySerialized(settings, newProfile);
        AssetDatabase.CreateAsset(newProfile, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadProfile(string profileName)
    {
        string path = $"Assets/Profiles/{profileName}.asset";
        PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>(path);
        if(PCData == null )
        {
            PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>("Assets/ScriptableObject/PlayerController/PlayerControllerData.asset");
        }
    }

    private void DeleteProfile(string profileName)
    {
        string path = $"Assets/Profiles/{profileName}.asset";
        AssetDatabase.DeleteAsset(path);
        RefreshProfileNames();
    }

    private void RefreshProfileNames()
    {
        profileNames.Clear();
        string[] guids = AssetDatabase.FindAssets("t:PlayerControllerData", new[] { "Assets/Profiles" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            profileNames.Add(Path.GetFileNameWithoutExtension(path));
        }

        if (profileNames.Count == 0)
        {
            profileNames.Add("No Profiles Available");
        }

        if (PCData == null)
        {
            PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>("Assets/ScriptableObject/PlayerController/PlayerControllerData.asset");
        }
    }
}