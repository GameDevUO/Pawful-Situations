using Unity.Netcode;
using UnityEngine;

public class SettingsSyncSpawner : MonoBehaviour
{
    [SerializeField] private SettingsSync settingsPrefab;
    public static SettingsSyncSpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnsureSpawned()
    {
        if (!NetworkManager.Singleton || (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer))
            return;

        if (settingsPrefab == null)
        {
            Debug.LogError("SettingsSyncSpawner: settingsPrefab not assigned!");
            return;
        }

        var existing = FindObjectOfType<SettingsSync>();
        if (existing != null) return;

        var go = Instantiate(settingsPrefab.gameObject);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.Spawn();
        Debug.Log("SettingsSync spawned on host.");
    }
}
