using Unity.Netcode;
using UnityEngine;

public class HostOnlyEnabler : MonoBehaviour
{
    [SerializeField] GameObject target; // e.g., Btn_StartMatch

    void OnEnable()
    {
        bool isHost = NetworkManager.Singleton && NetworkManager.Singleton.IsServer;
        target.SetActive(isHost);
    }
}
