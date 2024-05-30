using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerEditor : EditorWindow
{
    private PC_MovingSphere PCSphere;
    private PlayerControllerData PCData;

    private string profileName = "NewProfile";
    private List<string> profileNames = new List<string>();
    private int selectedProfileIndex = 0;

    private Vector2 scrollPos = Vector2.zero;

    private bool showMovementSettings = true;
    private bool showProbeSettings = true;
    private bool showMaterialSettings = true;
    private bool showBallSettings = true;

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
        GetProfileList();
    }

    private void OnGUI()
    {
        PCSphere = FindFirstObjectByType<PC_MovingSphere>();

        header1Style = new GUIStyle(EditorStyles.foldout);
        header1Style.fontStyle = FontStyle.Bold;
        header1Style.normal.textColor = Color.white;

        header2Style = new GUIStyle(EditorStyles.foldout);
        header2Style.normal.textColor = Color.white;

        float padding = 15;
        Rect area = new Rect(padding, padding, position.width - padding * 2f, position.height - padding * 2f);
        GUILayout.BeginArea(area);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        EditorGUILayout.BeginHorizontal();
        
        if(profileNames.Count == 0)
        {
            profileName = EditorGUILayout.TextField("Profile Name", profileName, GUILayout.MinWidth(250));
            if(GUILayout.Button("Add Profile", GUILayout.Width(150), GUILayout.MinWidth(100)))
            {
                NewProfile(profileName, PCData);
            }
        }
        else
        {
            selectedProfileIndex = EditorGUILayout.Popup("Select Profile", selectedProfileIndex, profileNames.ToArray(), GUILayout.MinWidth(250));
        }

        if (profileNames.Count > 0 && GUILayout.Button("Delete Profile", GUILayout.Width(150), GUILayout.MinWidth(100)))
        {
            DeleteProfile(selectedProfileIndex);
        }

        EditorGUILayout.EndHorizontal();

        GetProfileList();

        if(profileNames.Count > 0) LoadProfile(profileNames[selectedProfileIndex]);

        if (PCData != null)
        {
            EditorGUILayout.BeginHorizontal();
            profileName = EditorGUILayout.TextField("Profile Name", profileName, GUILayout.MinWidth(250));

            if (GUILayout.Button("Add Profile", GUILayout.Width(150), GUILayout.MinWidth(100)))
            {
                NewProfile(profileName, PCData);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            showMovementSettings = EditorGUILayout.Foldout(showMovementSettings, "Movement Settings", true, header2Style);
            if (showMovementSettings)
            {
                EditorGUI.indentLevel++;

                PCData.maxSpeed = EditorGUILayout.Slider("Max Speed", PCData.maxSpeed, 0f, 100f);
                PCSphere.maxSpeed = PCData.maxSpeed;

                PCData.maxAcceleration = EditorGUILayout.Slider("Max Acceleration", PCData.maxAcceleration, 0f, 100f);
                PCSphere.maxAcceleration = PCData.maxAcceleration;

                PCData.maxAirAcceleration = EditorGUILayout.Slider("Max Air Acceleration", PCData.maxAirAcceleration, 0f, 10f);
                PCSphere.maxAirAcceleration = PCData.maxAirAcceleration;

                PCData.maxGroundAngle = EditorGUILayout.Slider("Max Ground Angle", PCData.maxGroundAngle, 0f, 90f);
                PCSphere.maxGroundAngle = PCData.maxGroundAngle;

                PCData.maxSnapSpeed = EditorGUILayout.Slider("Max Snap Speed", PCData.maxSnapSpeed, 0, 100);
                PCSphere.maxSnapSpeed = PCData.maxSnapSpeed;

                EditorGUI.indentLevel--;
            }

            showProbeSettings = EditorGUILayout.Foldout(showProbeSettings, "Probe Settings", true, header2Style);
            if (showProbeSettings)
            {
                EditorGUI.indentLevel++;

                PCData.probeDistance = EditorGUILayout.Slider("Probe Distance", PCData.probeDistance, 0f, 1f);
                PCSphere.probeDistance = PCData.probeDistance;

                PCData.probeMask = EditorGUILayout.MaskField("Probe Mask", PCData.probeMask, InternalEditorUtility.layers);
                PCSphere.probeMask = PCData.probeMask;

                EditorGUI.indentLevel--;
            }

            showMaterialSettings = EditorGUILayout.Foldout(showMaterialSettings, "Probe Settings", true, header2Style);
            if (showMaterialSettings)
            {
                EditorGUI.indentLevel++;

                PCData.normalMaterial = (Material)EditorGUILayout.ObjectField(PCData.normalMaterial, typeof(Material), true);
                PCSphere.normalMaterial = PCData.normalMaterial;

                EditorGUI.indentLevel--;
            }

            showBallSettings = EditorGUILayout.Foldout(showMaterialSettings, "Probe Settings", true, header2Style);
            if(showBallSettings)
            {
                EditorGUI.indentLevel++;

                PCData.ballRadius = EditorGUILayout.Slider("Ball Radius", PCData.ballRadius, 0f, 1f);
                PCSphere.ballRadius = PCData.ballRadius;

                PCData.ballAlignSpeed = EditorGUILayout.Slider("Ball Align Speed ", PCData.ballAlignSpeed, 0f, 360f);
                PCSphere.ballAlignSpeed = PCData.ballAlignSpeed;

                PCData.ballAirRotation = EditorGUILayout.Slider("Ball Air rotation", PCData.ballAirRotation, 0f, 1f);
                PCSphere.ballAirRotation = PCData.ballAirRotation;

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void GetProfileList()
    {
        profileNames.Clear();
        string[] guids = AssetDatabase.FindAssets("t:PlayerControllerData", new[] { "Assets/Profiles" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            profileNames.Add(Path.GetFileNameWithoutExtension(path));
        }

        if (PCData == null)
        {
            PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>("Assets/ScriptableObject/PlayerController/PlayerControllerData.asset");
        }
    }

    private void LoadProfile(string profileName)
    {
        string path = $"Assets/Profiles/{profileName}.asset";
        PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>(path);
    }

    private void NewProfile(string profileName, PlayerControllerData settings)
    {
        string path = $"Assets/Profiles/{profileName}.asset";
        PlayerControllerData newProfile = CreateInstance<PlayerControllerData>();
        if(settings != null) EditorUtility.CopySerialized(settings, newProfile);
        AssetDatabase.CreateAsset(newProfile, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void DeleteProfile(int index)
    {
        string path = $"Assets/Profiles/{profileNames[index]}.asset";
        AssetDatabase.DeleteAsset(path);
        selectedProfileIndex = index > 0 ? index - 1 : 0;
        GetProfileList();
    }
}