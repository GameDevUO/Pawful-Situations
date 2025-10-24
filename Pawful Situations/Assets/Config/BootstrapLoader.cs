using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] string menuScene = "MainMenu";

    private void Awake()
    {
        Debug.Log($"[BootstrapLoader] Awake in scene: {SceneManager.GetActiveScene().name}");
    }

    private void Start()
    {
        Debug.Log($"[BootstrapLoader] Start: loading {menuScene}…");
        StartCoroutine(LoadNext());
    }

    private System.Collections.IEnumerator LoadNext()
    {
        // Wait one frame to let all Awake/Start (including DontDestroyOnLoad) run cleanly
        yield return null;

        if (SceneUtility.GetBuildIndexByScenePath(menuScene) < 0 &&
            SceneManager.GetSceneByName(menuScene).buildIndex < 0)
        {
            Debug.LogError($"[BootstrapLoader] Scene '{menuScene}' not in Build Settings.");
            yield break;
        }

        var op = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Single);
        if (op == null)
        {
            Debug.LogError("[BootstrapLoader] LoadSceneAsync returned null.");
            yield break;
        }

        while (!op.isDone) yield return null;

        Debug.Log("[BootstrapLoader] Loaded MainMenu.");
    }
}
