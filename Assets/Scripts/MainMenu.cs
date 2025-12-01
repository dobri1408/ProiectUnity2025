using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private Canvas menuCanvas;
    private GameObject mainPanel;
    private GameObject settingsPanel;
    private Main mainScript;
    private bool isGameStarted = false;
    private bool isPaused = false;

    void Start()
    {
        mainScript = FindFirstObjectByType<Main>();
        CreateMainMenu();
    }

    void Update()
    {
        if (isGameStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            menuCanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            menuCanvas.gameObject.SetActive(false);
            settingsPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void CreateMainMenu()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        menuCanvas = canvasObj.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Background with gradient effect
        GameObject background = CreatePanel(canvasObj.transform, "Background", new Color(0.05f, 0.08f, 0.12f, 1f));
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Overlay gradient
        GameObject overlay = CreatePanel(canvasObj.transform, "Overlay", new Color(0.1f, 0.15f, 0.25f, 0.5f));
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        // Main Panel
        mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
        mainRect.anchorMin = new Vector2(0.5f, 0.5f);
        mainRect.anchorMax = new Vector2(0.5f, 0.5f);
        mainRect.sizeDelta = new Vector2(500, 550);

        // Game Title - "ASCENT"
        CreateTitle(mainPanel.transform, "ASCENT", new Vector2(0, 180));

        // Subtitle
        CreateSubtitle(mainPanel.transform, "A Climbing Adventure", new Vector2(0, 120));

        // Buttons
        CreateStyledButton(mainPanel.transform, "PlayButton", "PLAY", new Vector2(0, 20), OnPlayClicked);
        CreateStyledButton(mainPanel.transform, "SettingsButton", "SETTINGS", new Vector2(0, -60), OnSettingsClicked);
        CreateStyledButton(mainPanel.transform, "ExitButton", "EXIT", new Vector2(0, -140), OnExitClicked);

        // Footer hint
        CreateHintText(mainPanel.transform, "Press ESC to pause during game", new Vector2(0, -220));

        // Create Settings Panel
        CreateSettingsPanel(canvasObj.transform);

        // Show cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    void CreateTitle(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject("Title");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 72;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.9f, 0.95f, 1f);
        tmp.enableVertexGradient = true;
        tmp.colorGradient = new VertexGradient(
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
            new Color(0.6f, 0.8f, 1f),
            new Color(0.6f, 0.8f, 1f)
        );

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(450, 100);
    }

    void CreateSubtitle(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject("Subtitle");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Italic;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.6f, 0.7f, 0.8f, 0.8f);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 40);
    }

    void CreateHintText(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject("Hint");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.5f, 0.5f, 0.6f, 0.6f);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 30);
    }

    void CreateStyledButton(Transform parent, string name, string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        // Button background
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.15f, 0.2f, 0.3f, 0.9f);

        Button btn = buttonObj.AddComponent<Button>();
        btn.onClick.AddListener(onClick);

        // Button hover/press colors
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.15f, 0.2f, 0.3f, 0.9f);
        colors.highlightedColor = new Color(0.25f, 0.4f, 0.6f, 1f);
        colors.pressedColor = new Color(0.1f, 0.25f, 0.4f, 1f);
        colors.selectedColor = new Color(0.2f, 0.35f, 0.5f, 1f);
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280, 55);

        // Border effect
        GameObject border = new GameObject("Border");
        border.transform.SetParent(buttonObj.transform, false);
        Image borderImg = border.AddComponent<Image>();
        borderImg.color = new Color(0.4f, 0.6f, 0.9f, 0.3f);
        RectTransform borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-2, -2);
        borderRect.offsetMax = new Vector2(2, 2);
        border.transform.SetAsFirstSibling();

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 26;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.9f, 0.95f, 1f);
        tmp.characterSpacing = 5;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    void CreateSettingsPanel(Transform parent)
    {
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(parent, false);

        Image bg = settingsPanel.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.1f, 0.15f, 0.98f);

        RectTransform rect = settingsPanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 400);

        // Settings Title
        CreateSettingsTitle(settingsPanel.transform, "SETTINGS", new Vector2(0, 150));

        // Volume Section
        CreateSettingsLabel(settingsPanel.transform, "Master Volume", new Vector2(-120, 70));
        CreateStyledSlider(settingsPanel.transform, "VolumeSlider", new Vector2(80, 70), AudioListener.volume, OnVolumeChanged);

        // Sensitivity Section
        CreateSettingsLabel(settingsPanel.transform, "Mouse Sensitivity", new Vector2(-100, 0));
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 300f);
        CreateStyledSlider(settingsPanel.transform, "SensSlider", new Vector2(80, 0), (savedSens - 100f) / 500f, OnSensitivityChanged);

        // Back Button
        CreateStyledButton(settingsPanel.transform, "BackButton", "BACK", new Vector2(0, -120), OnBackClicked);

        settingsPanel.SetActive(false);
    }

    void CreateSettingsTitle(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject("SettingsTitle");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 42;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.9f, 0.95f, 1f);
        tmp.characterSpacing = 8;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 60);
    }

    void CreateSettingsLabel(Transform parent, string text, Vector2 position)
    {
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = new Color(0.7f, 0.75f, 0.85f);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 35);
    }

    void CreateStyledSlider(Transform parent, string name, Vector2 position, float initialValue, UnityEngine.Events.UnityAction<float> onValueChanged)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = new Vector2(200, 25);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.22f, 0.28f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.35f);
        bgRect.anchorMax = new Vector2(1, 0.65f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.35f);
        fillAreaRect.anchorMax = new Vector2(1, 0.65f);
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.5f, 0.8f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Handle Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = new Color(0.9f, 0.95f, 1f);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(18, 28);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.value = initialValue;
        slider.onValueChanged.AddListener(onValueChanged);
    }

    void OnPlayClicked()
    {
        isGameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;
        menuCanvas.gameObject.SetActive(false);
        settingsPanel.SetActive(false);
        mainScript.StartGame();
    }

    void OnSettingsClicked()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    void OnBackClicked()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    void OnExitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    void OnSensitivityChanged(float value)
    {
        float sensitivity = value * 500f + 100f; // Range: 100-600
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);

        // Apply to current player if exists
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.mouseSens = sensitivity;
        }
    }

    public void ShowMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
