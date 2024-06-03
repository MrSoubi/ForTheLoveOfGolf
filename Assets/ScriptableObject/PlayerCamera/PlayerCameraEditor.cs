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

public class PlayerCameraEditor : EditorWindow
{
    /*
    private PC_OrbitCamera PCOrbitCam;
    private PlayerCameraData CamData;

    private string profileName = "NewProfile";
    private List<string> profileNames = new List<string>();
    private int selectedProfileIndex = 0;

    private Vector2 scrollPos = Vector2.zero;

    private bool addProfilePopup = false;

    private GUIStyle header1Style;
    private GUIStyle header2Style;

    [MenuItem("Tools/Player Camera")]
    static void Init()
    {
        var window = (PlayerCameraEditor)GetWindow(typeof(PlayerCameraEditor));
        window.Show();
    }

    private void OnEnable()
    {
        GetProfileList();
    }

    private void OnGUI()
    {
       PCOrbitCam = FindFirstObjectByType<PC_OrbitCamera>();

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
                NewProfile(profileName, CamData);
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

        if (CamData != null)
        {
            if (!addProfilePopup && GUILayout.Button("New Profile"))
            {
                addProfilePopup = true;
            }

            EditorGUILayout.BeginHorizontal();
            if (addProfilePopup)
            {
                profileName = EditorGUILayout.TextField("Profile Name", profileName, GUILayout.MinWidth(250));
                if(GUILayout.Button("Add", GUILayout.Width(150), GUILayout.MinWidth(100)))
                {
                    addProfilePopup = false;
                    NewProfile(profileName, CamData);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            CamData.distance = EditorGUILayout.Slider("Distance", CamData.distance, 1f, 20f);
            PCOrbitCam.distance = CamData.distance;

            CamData.focusRadius = EditorGUILayout.Slider("Focus Radius", CamData.focusRadius, 0f, 90f);
            PCOrbitCam.focusRadius = CamData.focusRadius;

            CamData.focusCentering = EditorGUILayout.Slider("Focus Centering", CamData.focusCentering, 0f, 1f);
            PCOrbitCam.focusCentering = CamData.focusCentering;

            CamData.rotationSpeed = EditorGUILayout.Slider("Rotation Speed", CamData.rotationSpeed, 1f, 360f);
            PCOrbitCam.rotationSpeed = CamData.rotationSpeed;

            CamData.minVerticalAngle = EditorGUILayout.Slider("Min Vertical Angle", CamData.minVerticalAngle, -89f, 89f);
            PCOrbitCam.minVerticalAngle = CamData.minVerticalAngle;

            CamData.maxVerticalAngle = EditorGUILayout.Slider("Max Vertical Angle", CamData.maxVerticalAngle, -89f, 89f);
            PCOrbitCam.maxVerticalAngle = CamData.maxVerticalAngle;

            CamData.alignDelay = EditorGUILayout.Slider("Align Delay", CamData.alignDelay, 0f, 100f);
            PCOrbitCam.alignDelay = CamData.alignDelay;

            CamData.obstructionMask = EditorGUILayout.MaskField("Obstruction Mask", CamData.obstructionMask, InternalEditorUtility.layers);
            PCOrbitCam.obstructionMask = CamData.obstructionMask;

            CamData.orbitAngles = EditorGUILayout.Vector2Field("Orbit Angle", CamData.orbitAngles);
            PCOrbitCam.orbitAngles = CamData.orbitAngles;
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void GetProfileList()
    {
        profileNames.Clear();
        string[] guids = AssetDatabase.FindAssets("t:PlayerCameraData", new[] { "Assets/Profiles/Camera" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            profileNames.Add(Path.GetFileNameWithoutExtension(path));
        }

        if (CamData == null)
        {
            CamData = AssetDatabase.LoadAssetAtPath<PlayerCameraData>("Assets/ScriptableObject/PlayerCamera/PlayerCameraData.asset");
        }
    }

    private void LoadProfile(string profileName)
    {
        string path = $"Assets/Profiles/Camera/{profileName}.asset";
        CamData = AssetDatabase.LoadAssetAtPath<PlayerCameraData>(path);
    }

    private void NewProfile(string profileName, PlayerCameraData settings)
    {
        string path = $"Assets/Profiles/Camera/{profileName}.asset";
        PlayerCameraData newProfile = CreateInstance<PlayerCameraData>();
        if(settings != null) EditorUtility.CopySerialized(settings, newProfile);
        AssetDatabase.CreateAsset(newProfile, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void DeleteProfile(int index)
    {
        string path = $"Assets/Profiles/Camera/{profileNames[index]}.asset";
        AssetDatabase.DeleteAsset(path);
        selectedProfileIndex = index > 0 ? index - 1 : 0;
        GetProfileList();
    }*/
}