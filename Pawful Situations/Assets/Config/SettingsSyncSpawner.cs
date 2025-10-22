using Unity.Netcode;
using UnityEngine;

public class SettingsSyncSpawner : MonoBehaviour
{
    [SerializeField] private SettingsSync settingsPrefab;
    public static SettingsSync Instance { get; private set; }

    void Awake() => DontDestroyOnLoad(gameObject);

    public void EnsureSpawned()
    {
        if (Instance != null) return;

        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            var go = Instantiate(settingsPrefab);
            var netObj = go.GetComponent<NetworkObject>();
            netObj.Spawn();
            Instance = go.GetComponent<SettingsSync>();
        }
    }
}
