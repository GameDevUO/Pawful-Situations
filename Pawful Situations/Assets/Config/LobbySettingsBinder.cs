using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettingsBinder : MonoBehaviour
{
    [SerializeField] Slider cardsSlider;
    [SerializeField] Slider roundsSlider;
    [SerializeField] Toggle teamsToggle;
    [SerializeField] Slider volumeSlider;

    SettingsSync settings;

    void OnEnable()
    {
        TryBindSettings();            // <- do all binding here once
        InitLocalVolume();
    }

    void OnDisable()
    {
        if (settings != null) settings.OnSettingsChanged -= RefreshFromSettings;
    }

    void TryBindSettings()
    {
        settings = FindObjectOfType<SettingsSync>(true);
        RefreshInteractable();

        if (settings == null) return;

        // Avoid duplicate subscriptions if TryBindSettings gets called again
        settings.OnSettingsChanged -= RefreshFromSettings;
        settings.OnSettingsChanged += RefreshFromSettings;

        // Initialize UI from replicated values
        cardsSlider.SetValueWithoutNotify(
            Mathf.Clamp(settings.CardsPerHand.Value, (int)cardsSlider.minValue, (int)cardsSlider.maxValue));
        roundsSlider.SetValueWithoutNotify(
            Mathf.Clamp(settings.Rounds.Value, (int)roundsSlider.minValue, (int)roundsSlider.maxValue));
        teamsToggle.SetIsOnWithoutNotify(settings.Teams.Value);
    }

    void RefreshFromSettings()
    {
        if (settings == null) return;
        cardsSlider.SetValueWithoutNotify(
            Mathf.Clamp(settings.CardsPerHand.Value, (int)cardsSlider.minValue, (int)cardsSlider.maxValue));
        roundsSlider.SetValueWithoutNotify(
            Mathf.Clamp(settings.Rounds.Value, (int)roundsSlider.minValue, (int)roundsSlider.maxValue));
        teamsToggle.SetIsOnWithoutNotify(settings.Teams.Value);
    }

    void RefreshInteractable()
    {
        bool isHost = NetworkManager.Singleton &&
                      (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost);
        cardsSlider.interactable = isHost;
        roundsSlider.interactable = isHost;
        teamsToggle.interactable = isHost;
        // volume is always local
    }

    public void ForceRefreshNow()
    {
        TryBindSettings();
        RefreshInteractable();
        RefreshFromSettings();
        Debug.Log($"Binders in scene: {FindObjectsOfType<LobbySettingsBinder>(true).Length}");
    }

    public void OnCardsChanged(float v)
    {
        if (!(NetworkManager.Singleton && NetworkManager.Singleton.IsServer) || settings == null) return;
        settings.SetCardsPerHandServerRpc(Mathf.RoundToInt(v));
    }

    public void OnRoundsChanged(float v)
    {
        if (!(NetworkManager.Singleton && NetworkManager.Singleton.IsServer) || settings == null) return;
        settings.SetRoundsServerRpc(Mathf.RoundToInt(v));
    }

    public void OnTeamsChanged(bool on)
    {
        if (!(NetworkManager.Singleton && NetworkManager.Singleton.IsServer) || settings == null) return;
        settings.SetTeamsServerRpc(on);
    }

    public void OnVolumeChanged(float v)
    {
        v = Mathf.Clamp01(v);
        AudioListener.volume = v;
        PlayerPrefs.SetFloat("masterVolume", v);
    }

    void InitLocalVolume()
    {
        float v = PlayerPrefs.GetFloat("masterVolume", 0.8f);
        volumeSlider.SetValueWithoutNotify(v);
        AudioListener.volume = v;
    }
}
