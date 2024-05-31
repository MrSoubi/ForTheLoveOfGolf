using Cinemachine.Editor;
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
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class PlayerControllerEditor : EditorWindow
{
    private PC_MovingSphere PCSphere;
    private PlayerControllerData PCData;

    private AnimationCurve shootCurve;

    private bool hideNewProfileSettings = true;

    private bool showMovementSettings = true;
    private bool showSpeedLimit = true;

    private string baseProfileName = "NewProfile";
    private string profileName;
    private List<string> profileList = new List<string>();
    private int selectedProfileIndex = 0;

    public string profilePath = "Assets/Profiles/Player/";

    private Vector2 scrollPos = Vector2.zero;

    private GUIStyle header1Style;
    private GUIStyle header2Style;

    [MenuItem("Tools/Player Controller")]
    public static void ShowWindow()
    {
        GetWindow<PlayerControllerEditor>("Player Controller Editor");
    }

    private void OnEnable()
    {
        GetProfileList();
    }

    private void OnGUI()
    {
        PCSphere = FindAnyObjectByType<PC_MovingSphere>();

        header1Style = new GUIStyle(EditorStyles.foldout);
        header1Style.fontStyle = FontStyle.Bold;
        header1Style.normal.textColor = Color.white;

        header2Style = new GUIStyle(EditorStyles.foldout);
        header2Style.normal.textColor = Color.white;

        float padding = 15;
        Rect area = new Rect(padding, padding, position.width - padding * 2f, position.height - padding * 2f);

        GUILayout.BeginArea(area);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        if (PCSphere != null)
        {
            if (profileList.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();

                profileName = EditorGUILayout.TextField("Profile Name", profileName, GUILayout.MinWidth(250));
                if (GUILayout.Button("Add Profile", GUILayout.Width(150), GUILayout.MinWidth(100)))
                {
                    NewProfile(profileName);
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                selectedProfileIndex = EditorGUILayout.Popup("Select Profile", selectedProfileIndex, profileList.ToArray(), GUILayout.MinWidth(250));

                if (profileList.Count > 0 && GUILayout.Button("Delete Profile", GUILayout.Width(150), GUILayout.MinWidth(100)))
                {
                    DeleteProfile(selectedProfileIndex);
                }

                EditorGUILayout.EndHorizontal();

                if (hideNewProfileSettings)
                {
                    if (GUILayout.Button("New Profile"))
                    {
                        profileName = baseProfileName;
                        hideNewProfileSettings = false;
                    }
                }

                if (!hideNewProfileSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    profileName = EditorGUILayout.TextField("Profile Name", profileName, GUILayout.MinWidth(250));
                    if (GUILayout.Button("Add", GUILayout.Width(150), GUILayout.MinWidth(100)))
                    {
                        hideNewProfileSettings = true;
                        NewProfile(profileName);
                        selectedProfileIndex = GetProfileIndex(profileName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (profileList.Count > 0) LoadProfile(profileList[selectedProfileIndex]);

            if (PCData != null)
            {
                PCSphere.UpdatePCData();

                showMovementSettings = EditorGUILayout.Foldout(showMovementSettings, "Movement Settings");
                if (showMovementSettings)
                {
                    PCData.maxAcceleration = EditorGUILayout.Slider("Max Acceleration", PCData.maxAcceleration, 1f, 100f);
                    PCData.maxAirAcceleration = EditorGUILayout.Slider("Max Air Acceleration", PCData.maxAirAcceleration, 1f, 100f);

                    PCData.shootHeight = EditorGUILayout.Slider("shoot Height", PCData.shootHeight, 0f, 100f);
                    PCData.maxShoots = EditorGUILayout.IntField("Max Shoots", PCData.maxShoots);

                    PCData.maxGroundAngle = EditorGUILayout.Slider("Max Ground Angle", PCData.maxGroundAngle, 0f, 90f);
                    PCData.maxSnapSpeed = EditorGUILayout.Slider("Max Snap Speed", PCData.maxSnapSpeed, 0f, 100f);
                    PCData.probeDistance = EditorGUILayout.Slider("Probe Distance", PCData.probeDistance, 0f, 100f);

                    EditorGUILayout.BeginHorizontal();
                    showSpeedLimit = EditorGUILayout.Foldout(showSpeedLimit, "Speed Limit");

                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("", PCData.speedLimits.Count, GUILayout.MaxWidth(100)));

                    EditorGUILayout.EndHorizontal();

                    while (newCount < PCData.speedLimits.Count)
                    {
                        PCData.speedLimits.RemoveAt(PCData.speedLimits.Count - 1);
                    }
                    while (newCount > PCData.speedLimits.Count)
                    {
                        PCData.speedLimits.Add(0);
                    }

                    if (showSpeedLimit)
                    {
                        for (int i = 0; i < PCData.speedLimits.Count; i++)
                        {
                            EditorGUI.indentLevel++;
                            PCData.speedLimits[i] = EditorGUILayout.FloatField("Limits " + i, PCData.speedLimits[i]);
                            EditorGUI.indentLevel--;
                        }
                    }

                    PCData.speedLimitMargin = EditorGUILayout.FloatField("Speed Limit Margin", PCData.speedLimitMargin);

                    PCData.rollingMaterial = (Material)EditorGUILayout.ObjectField(PCData.rollingMaterial, typeof(Material), true);
                    PCData.aimingMaterial = (Material)EditorGUILayout.ObjectField(PCData.aimingMaterial, typeof(Material), true);

                    PCData.shootingAngle = EditorGUILayout.FloatField("Shooting Angle", PCData.shootingAngle);

                    shootCurve = EditorGUILayout.CurveField("Animation Curve", shootCurve);
                    PCData.shootCurve = shootCurve;
                }
            }


        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void GetProfileList()
    {
        profileList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:PlayerControllerData", new[] {profilePath});
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            profileList.Add(Path.GetFileNameWithoutExtension(path));
        }
        if (profileList.Count == 0)
        {
            profileName = baseProfileName;
        }
    }

    private void LoadProfile(string profileName)
    {
        string path = profilePath + profileName + ".asset";
        PCData = AssetDatabase.LoadAssetAtPath<PlayerControllerData>(path);
        PCSphere.SetPCData(PCData);
        GetProfileList();
    }

    private void NewProfile(string profileName)
    {
        string path = profilePath + profileName + ".asset";

        PlayerControllerData newProfile = CreateInstance<PlayerControllerData>();

        if (PCData != null) EditorUtility.CopySerialized(PCData, newProfile);

        AssetDatabase.CreateAsset(newProfile, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GetProfileList();
    }
    private void DeleteProfile(int index)
    {
        string path = profilePath + profileList[index]+ ".asset";
        AssetDatabase.DeleteAsset(path);
        selectedProfileIndex = index > 0 ? index - 1 : 0;
        GetProfileList();
    }

    private int GetProfileIndex(string profileName)
    {
        for (int i = 0; i < profileList.Count; i++)
        {
            if (profileList[i] == profileName)
            {
                return i;
            }
        }
        return 0;
    }
}