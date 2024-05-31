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

    private bool hideNewProfileSettings = true;

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
        header1Style = new GUIStyle(EditorStyles.foldout);
        header1Style.fontStyle = FontStyle.Bold;
        header1Style.normal.textColor = Color.white;

        header2Style = new GUIStyle(EditorStyles.foldout);
        header2Style.normal.textColor = Color.white;

        float padding = 15;
        Rect area = new Rect(padding, padding, position.width - padding * 2f, position.height - padding * 2f);

        GUILayout.BeginArea(area);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

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