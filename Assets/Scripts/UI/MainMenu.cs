using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private Canvas menuCanvas;
    private GameObject mainPanel;
    private GameObject settingsPanel;
    private LevelSelectMenu levelSelectMenu;
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

            // muzica mai incet in pauza
            if (MusicManager.Instance != null)
                MusicManager.Instance.OnEnterMenu();
        }
        else
        {
            Time.timeScale = 1f;
            menuCanvas.gameObject.SetActive(false);
            settingsPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // muzica la volum plin
            if (MusicManager.Instance != null)
                MusicManager.Instance.OnEnterGameplay();
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

        // Animated climbing-themed background
        CreateClimbingBackground(canvasObj.transform);

        // Main Panel
        mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
        mainRect.anchorMin = new Vector2(0.5f, 0.5f);
        mainRect.anchorMax = new Vector2(0.5f, 0.5f);
        mainRect.sizeDelta = new Vector2(500, 620);

        // Game Title - "ASCENT"
        CreateTitle(mainPanel.transform, "ASCENT", new Vector2(0, 180));

        // Subtitle
        CreateSubtitle(mainPanel.transform, "A Climbing Adventure", new Vector2(0, 120));

        // Buttons
        CreateStyledButton(mainPanel.transform, "PlayButton", "PLAY", new Vector2(0, 40), OnPlayClicked);
        CreateStyledButton(mainPanel.transform, "LevelSelectButton", "LEVEL SELECT", new Vector2(0, -40), OnLevelSelectClicked);
        CreateStyledButton(mainPanel.transform, "SettingsButton", "SETTINGS", new Vector2(0, -120), OnSettingsClicked);
        CreateStyledButton(mainPanel.transform, "ExitButton", "EXIT", new Vector2(0, -200), OnExitClicked);

        // Footer hint
        CreateHintText(mainPanel.transform, "Press ESC to pause during game", new Vector2(0, -270));

        // Initialize GameSaveManager
        _ = GameSaveManager.Instance;

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

    void CreateClimbingBackground(Transform parent)
    {
        // Dark sky gradient background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(parent, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.02f, 0.05f, 0.12f, 1f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Mountain silhouettes in background
        CreateMountainLayer(parent, 0.15f, new Color(0.08f, 0.12f, 0.2f, 1f), 0);
        CreateMountainLayer(parent, 0.25f, new Color(0.05f, 0.08f, 0.15f, 1f), -100);
        CreateMountainLayer(parent, 0.35f, new Color(0.03f, 0.05f, 0.1f, 1f), -200);

        // Floating rocks/climbing holds
        for (int i = 0; i < 12; i++)
        {
            CreateFloatingRock(parent, i);
        }

        // Climbing rope on the side
        CreateClimbingRope(parent, true);
        CreateClimbingRope(parent, false);

        // Subtle fog/mist overlay
        GameObject mist = new GameObject("Mist");
        mist.transform.SetParent(parent, false);
        Image mistImg = mist.AddComponent<Image>();
        mistImg.color = new Color(0.1f, 0.15f, 0.25f, 0.3f);
        RectTransform mistRect = mist.GetComponent<RectTransform>();
        mistRect.anchorMin = Vector2.zero;
        mistRect.anchorMax = Vector2.one;
        mistRect.offsetMin = Vector2.zero;
        mistRect.offsetMax = Vector2.zero;
    }

    void CreateMountainLayer(Transform parent, float heightPercent, Color color, float offsetY)
    {
        GameObject mountain = new GameObject("Mountain");
        mountain.transform.SetParent(parent, false);

        // Create mountain peak shapes using text symbols
        TextMeshProUGUI peakText = mountain.AddComponent<TextMeshProUGUI>();
        peakText.text = "\u25B2  \u25B2  \u25B2  \u25B2  \u25B2  \u25B2  \u25B2"; // Triangle symbols
        peakText.fontSize = 200;
        peakText.alignment = TextAlignmentOptions.Bottom;
        peakText.color = color;
        peakText.characterSpacing = 50;

        RectTransform rect = mountain.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, heightPercent);
        rect.offsetMin = new Vector2(-200, offsetY);
        rect.offsetMax = new Vector2(200, 0);

        // Add slow parallax movement
        MountainParallax parallax = mountain.AddComponent<MountainParallax>();
        parallax.speed = 5f * (1f - heightPercent);
    }

    void CreateFloatingRock(Transform parent, int index)
    {
        GameObject rock = new GameObject($"Rock_{index}");
        rock.transform.SetParent(parent, false);

        TextMeshProUGUI rockText = rock.AddComponent<TextMeshProUGUI>();
        // Use different rock/stone symbols
        string[] rockSymbols = { "\u25C6", "\u25C7", "\u2B22", "\u2B23", "\u25CF" };
        rockText.text = rockSymbols[index % rockSymbols.Length];
        rockText.fontSize = Random.Range(20, 60);
        rockText.alignment = TextAlignmentOptions.Center;

        float gray = Random.Range(0.2f, 0.4f);
        rockText.color = new Color(gray, gray * 1.1f, gray * 1.2f, Random.Range(0.3f, 0.6f));

        RectTransform rect = rock.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(100, 100);
        rect.anchoredPosition = new Vector2(
            Random.Range(-800f, 800f),
            Random.Range(-400f, 400f)
        );

        // Add floating animation
        FloatingRock floater = rock.AddComponent<FloatingRock>();
        floater.Initialize(
            Random.Range(0.3f, 0.8f),
            Random.Range(15f, 40f),
            Random.Range(0.5f, 1.5f)
        );
    }

    void CreateClimbingRope(Transform parent, bool leftSide)
    {
        GameObject rope = new GameObject(leftSide ? "RopeLeft" : "RopeRight");
        rope.transform.SetParent(parent, false);

        // Create rope using vertical line of symbols
        TextMeshProUGUI ropeText = rope.AddComponent<TextMeshProUGUI>();
        ropeText.text = "|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|\n|";
        ropeText.fontSize = 40;
        ropeText.alignment = TextAlignmentOptions.Center;
        ropeText.color = new Color(0.6f, 0.45f, 0.3f, 0.4f);
        ropeText.lineSpacing = -20;

        RectTransform rect = rope.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(leftSide ? 0.05f : 0.95f, 0);
        rect.anchorMax = new Vector2(leftSide ? 0.08f : 0.98f, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Add swaying animation
        RopeSwayAnimation sway = rope.AddComponent<RopeSwayAnimation>();
        sway.Initialize(leftSide ? 1f : -1f);
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

        // Add button animation
        buttonObj.AddComponent<ButtonAnimator>();
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
        rect.sizeDelta = new Vector2(500, 480);

        // Settings Title
        CreateSettingsTitle(settingsPanel.transform, "SETTINGS", new Vector2(0, 190));

        // Master Volume Section
        CreateSettingsLabel(settingsPanel.transform, "Master Volume", new Vector2(-120, 110));
        CreateStyledSlider(settingsPanel.transform, "VolumeSlider", new Vector2(80, 110), AudioListener.volume, OnVolumeChanged);

        // Music Volume Section
        CreateSettingsLabel(settingsPanel.transform, "Music Volume", new Vector2(-120, 40));
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        CreateStyledSlider(settingsPanel.transform, "MusicSlider", new Vector2(80, 40), savedMusic, OnMusicVolumeChanged);

        // Sensitivity Section
        CreateSettingsLabel(settingsPanel.transform, "Mouse Sensitivity", new Vector2(-100, -30));
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 300f);
        CreateStyledSlider(settingsPanel.transform, "SensSlider", new Vector2(80, -30), (savedSens - 100f) / 500f, OnSensitivityChanged);

        // Back Button
        CreateStyledButton(settingsPanel.transform, "BackButton", "BACK", new Vector2(0, -150), OnBackClicked);

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

        // ascunde cursorul in gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // muzica la volum plin in gameplay
        if (MusicManager.Instance != null)
            MusicManager.Instance.OnEnterGameplay();

        mainScript.StartGame();
    }

    void OnLevelSelectClicked()
    {
        menuCanvas.gameObject.SetActive(false);

        if (levelSelectMenu == null)
        {
            GameObject levelSelectObj = new GameObject("LevelSelectMenu");
            levelSelectMenu = levelSelectObj.AddComponent<LevelSelectMenu>();
            levelSelectMenu.Initialize(mainScript, OnLevelSelectBack, OnLevelStarted);
        }
        else
        {
            levelSelectMenu.Show();
        }
    }

    void OnLevelSelectBack()
    {
        menuCanvas.gameObject.SetActive(true);
        mainPanel.SetActive(true);
        if (isGameStarted)
        {
            isPaused = true;
        }
    }

    void OnLevelStarted()
    {
        isGameStarted = true;
        isPaused = false;

        // ascunde cursorul in gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // muzica la volum plin in gameplay
        if (MusicManager.Instance != null)
            MusicManager.Instance.OnEnterGameplay();
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

    void OnMusicVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
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
