#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class PlayFromScene : EditorWindow
{
    private static string[] sceneNames;
    private static EditorBuildSettingsScene[] scenes;
    [SerializeField] private bool hasPlayed;
    [SerializeField] private string lastScene = "";
    [SerializeField] private int targetScene;
    [SerializeField] private string waitScene;

    [MenuItem("Edit/Play From Scene %0")]
    public static void Run()
    {
        GetWindow<PlayFromScene>();
    }

    private void OnEnable()
    {
        scenes = EditorBuildSettings.scenes;
        sceneNames = scenes.Select(x => AsSpacedCamelCase(Path.GetFileNameWithoutExtension(x.path))).ToArray();
    }

    private void Update()
    {
        if (!EditorApplication.isPlaying)
            if (null == waitScene && !string.IsNullOrEmpty(lastScene))
            {
                EditorSceneManager.OpenScene(lastScene);
                lastScene = null;
            }
    }

    private void OnGUI()
    {
        if (EditorApplication.isPlaying)
        {
            if (EditorApplication.currentScene == waitScene) waitScene = null;
            return;
        }

        if (EditorApplication.currentScene == waitScene) EditorApplication.isPlaying = true;
        if (null == sceneNames) return;
        targetScene = EditorGUILayout.Popup(targetScene, sceneNames);
        if (GUILayout.Button("Play"))
        {
            lastScene = EditorApplication.currentScene;
            waitScene = scenes[targetScene].path;
            EditorApplication.SaveCurrentSceneIfUserWantsTo();
            EditorApplication.OpenScene(waitScene);
        }
    }

    public string AsSpacedCamelCase(string text)
    {
        var sb = new StringBuilder(text.Length * 2);
        sb.Append(char.ToUpper(text[0]));
        for (var i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                sb.Append(' ');
            sb.Append(text[i]);
        }

        return sb.ToString();
    }
}
#endif