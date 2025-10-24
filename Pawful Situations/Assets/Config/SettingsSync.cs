using Unity.Netcode;
using UnityEngine;

public class SettingsSync : NetworkBehaviour
{
    // Everyone can read; only server writes
    public NetworkVariable<int> CardsPerHand = new(7, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> Rounds = new(8, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> Teams = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public event System.Action OnSettingsChanged;

    public override void OnNetworkSpawn()
    {
        CardsPerHand.OnValueChanged += HandleChanged;
        Rounds.OnValueChanged += HandleChanged;
        Teams.OnValueChanged += HandleChanged;
    }

    public override void OnNetworkDespawn()
    {
        CardsPerHand.OnValueChanged -= HandleChanged;
        Rounds.OnValueChanged -= HandleChanged;
        Teams.OnValueChanged -= HandleChanged;
    }

    void HandleChanged<T>(T _, T __) => OnSettingsChanged?.Invoke();

    // --- Host-only setters (called via UI on host) ---
    [ServerRpc(RequireOwnership = false)]
    public void SetCardsPerHandServerRpc(int v)
    {
        v = Mathf.Clamp(v, 3, 12);
        Debug.Log($"[Host] CardsPerHand = {v}");
        CardsPerHand.Value = v;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRoundsServerRpc(int v)
    {
        v = Mathf.Clamp(v, 3, 30);
        Debug.Log($"[Host] Rounds = {v}");
        Rounds.Value = v;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTeamsServerRpc(bool on)
    {
        Debug.Log($"[Host] Teams = {on}");
        Teams.Value = on;
    }
}
