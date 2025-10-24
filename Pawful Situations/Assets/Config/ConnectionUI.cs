using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] TMP_InputField ipField;
    [SerializeField] string gameSceneName = "Game"; // must be in Build Settings

    void Awake()
    {
        // Default IP for quick local testing
        if (ipField != null && string.IsNullOrWhiteSpace(ipField.text))
            ipField.text = "127.0.0.1";
    }

    public void OnClickHost()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null)
        {
            Debug.LogError("No NetworkManager (Bootstrap must load first).");
            return;
        }

        // Start the host
        bool ok = nm.StartHost();
        if (!ok)
        {
            Debug.LogError("StartHost() failed.");
            return;
        }

        // Spawn SettingsSync network object (this is the replicated settings data)
        SettingsSyncSpawner.Instance?.EnsureSpawned();

        // Refresh the lobby UI now that we are host (enables sliders/toggles)
        FindObjectOfType<LobbySettingsBinder>(true)?.ForceRefreshNow();

        // Optional: feedback
        Debug.Log("Hosting. You can adjust settings; clients will see them.");
    }


    public void OnClickJoin()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) { Debug.LogError("No NetworkManager."); return; }

        var ip = (ipField != null && !string.IsNullOrWhiteSpace(ipField.text)) ? ipField.text.Trim() : "127.0.0.1";
        var transport = nm.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, 7777);

        nm.StartClient();
        Debug.Log($"Joining host at {ip}:7777 …");
    }

    // Host uses this to move everyone into the Game scene
    public void OnClickStartMatch()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsServer) { Debug.LogWarning("Only host can start the match."); return; }

        nm.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}
