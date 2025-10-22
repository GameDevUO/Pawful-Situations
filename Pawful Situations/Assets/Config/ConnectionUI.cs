using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] InputField ipField;

    void Awake()
    {
        // Bootstrap scene persists, so you can just find this when menu loads
        if (FindObjectOfType<SettingsSyncSpawner>() == null)
            Debug.LogWarning("ConnectionUI: couldn't find SettingsSyncSpawner yet");
    }

    public void OnClickHost()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null)
        {
            Debug.LogError("NetworkManager not found. Did Bootstrap load first?");
            return;
        }

        nm.StartHost();

        // Automatically find the persistent spawner
        var spawner = FindObjectOfType<SettingsSyncSpawner>(true);
        if (spawner != null)
            spawner.EnsureSpawned();
        else
            Debug.LogError("SettingsSyncSpawner not found in scene hierarchy!");
    }

    public void OnClickJoin()
    {
        var ip = string.IsNullOrWhiteSpace(ipField.text) ? "127.0.0.1" : ipField.text.Trim();
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, 7777);
        NetworkManager.Singleton.StartClient();
    }
}
