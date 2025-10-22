using Unity.Netcode;
using UnityEngine;

public class SettingsSync : NetworkBehaviour
{
    public NetworkVariable<int> CardsPerHand = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> Rounds      = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> Teams      = new(writePerm: NetworkVariableWritePermission.Server);

    public event System.Action OnSettingsChanged;

    public override void OnNetworkSpawn()
    {
        CardsPerHand.OnValueChanged += (_, __) => OnSettingsChanged?.Invoke();
        Rounds.OnValueChanged       += (_, __) => OnSettingsChanged?.Invoke();
        Teams.OnValueChanged        += (_, __) => OnSettingsChanged?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCardsPerHandServerRpc(int value) =>
        CardsPerHand.Value = Mathf.Clamp(value, 3, 12);

    [ServerRpc(RequireOwnership = false)]
    public void SetRoundsServerRpc(int value) =>
        Rounds.Value = Mathf.Clamp(value, 3, 30);

    [ServerRpc(RequireOwnership = false)]
    public void SetTeamsServerRpc(bool on) =>
        Teams.Value = on;
}
