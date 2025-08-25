using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Toggle fullscreenToggle;
    public Toggle muteToggle;
    public Slider volumeSlider; // Asigna tu slider de volumen

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowMainMenu();

        // Inicializar fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1; // 1 = default fullscreen
        Screen.fullScreen = isFullscreen;
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = isFullscreen;

        // Inicializar volumen/mute
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (muteToggle != null)
            muteToggle.isOn = savedVolume <= 0f;

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenuScene()
    {
        // Limpiar toda la escena
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }

        // Recargar el menú
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ExitGame()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }

    // ----- TOGGLES -----

    public void OnToggleFullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;
        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnToggleMute(bool isOn)
    {
        if (isOn)
        {
            if (volumeSlider != null) volumeSlider.value = 0f;
            AudioListener.volume = 0f;
            PlayerPrefs.SetFloat("MusicVolume", 0f);
        }
        else
        {
            if (volumeSlider != null)
            {
                float vol = Mathf.Max(volumeSlider.value, 0.01f); // evitar que quede en 0
                AudioListener.volume = vol;
                PlayerPrefs.SetFloat("MusicVolume", vol);
            }
        }
        PlayerPrefs.Save();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;

        if (muteToggle != null)
            muteToggle.isOn = value <= 0f;

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }
}
