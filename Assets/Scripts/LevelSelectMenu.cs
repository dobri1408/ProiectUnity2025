using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectMenu : MonoBehaviour
{
    private Canvas menuCanvas;
    private GameObject mainPanel;
    private Main mainScript;
    private List<GameObject> levelCards = new List<GameObject>();
    private System.Action onBackCallback;
    private System.Action onLevelSelectedCallback;

    // Animation properties
    private float cardAnimationDelay = 0.1f;
    private float cardAnimationDuration = 0.3f;

    public void Initialize(Main main, System.Action onBack, System.Action onLevelSelected = null)
    {
        mainScript = main;
        onBackCallback = onBack;
        onLevelSelectedCallback = onLevelSelected;
        CreateLevelSelectMenu();
    }

    void CreateLevelSelectMenu()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("LevelSelectCanvas");
        menuCanvas = canvasObj.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 101;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Animated Background
        CreateAnimatedBackground(canvasObj.transform);

        // Main Panel
        mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform mainRect = mainPanel.AddComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;

        // Header
        CreateHeader(mainPanel.transform);

        // Level Grid Container
        CreateLevelGrid(mainPanel.transform);

        // Footer with stats and back button
        CreateFooter(mainPanel.transform);

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start card animations
        StartCoroutine(AnimateCardsIn());
    }

    void CreateAnimatedBackground(Transform parent)
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
        CreateMountainLayer(parent, 0.12f, new Color(0.08f, 0.12f, 0.2f, 1f), 0);
        CreateMountainLayer(parent, 0.20f, new Color(0.05f, 0.08f, 0.15f, 1f), -80);

        // Floating rocks
        for (int i = 0; i < 10; i++)
        {
            CreateFloatingRock(parent, i);
        }

        // Subtle mist overlay
        GameObject mist = new GameObject("Mist");
        mist.transform.SetParent(parent, false);
        Image mistImg = mist.AddComponent<Image>();
        mistImg.color = new Color(0.1f, 0.15f, 0.25f, 0.25f);
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

        TextMeshProUGUI peakText = mountain.AddComponent<TextMeshProUGUI>();
        peakText.text = "\u25B2  \u25B2  \u25B2  \u25B2  \u25B2  \u25B2  \u25B2";
        peakText.fontSize = 180;
        peakText.alignment = TextAlignmentOptions.Bottom;
        peakText.color = color;
        peakText.characterSpacing = 40;

        RectTransform rect = mountain.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, heightPercent);
        rect.offsetMin = new Vector2(-200, offsetY);
        rect.offsetMax = new Vector2(200, 0);

        MountainParallax parallax = mountain.AddComponent<MountainParallax>();
        parallax.speed = 4f * (1f - heightPercent);
    }

    void CreateFloatingRock(Transform parent, int index)
    {
        GameObject rock = new GameObject($"Rock_{index}");
        rock.transform.SetParent(parent, false);

        TextMeshProUGUI rockText = rock.AddComponent<TextMeshProUGUI>();
        string[] rockSymbols = { "\u25C6", "\u25C7", "\u2B22", "\u25CF" };
        rockText.text = rockSymbols[index % rockSymbols.Length];
        rockText.fontSize = Random.Range(15, 45);
        rockText.alignment = TextAlignmentOptions.Center;

        float gray = Random.Range(0.2f, 0.35f);
        rockText.color = new Color(gray, gray * 1.1f, gray * 1.2f, Random.Range(0.2f, 0.5f));

        RectTransform rect = rock.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(80, 80);
        rect.anchoredPosition = new Vector2(
            Random.Range(-850f, 850f),
            Random.Range(-450f, 450f)
        );

        FloatingRock floater = rock.AddComponent<FloatingRock>();
        floater.Initialize(
            Random.Range(0.3f, 0.7f),
            Random.Range(12f, 30f),
            Random.Range(0.3f, 1.2f)
        );
    }

    void CreateHeader(Transform parent)
    {
        GameObject header = new GameObject("Header");
        header.transform.SetParent(parent, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.85f);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(header.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "SELECT LEVEL";
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.95f, 0.97f, 1f);
        titleText.characterSpacing = 10;
        titleText.enableVertexGradient = true;
        titleText.colorGradient = new VertexGradient(
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
            new Color(0.5f, 0.7f, 1f),
            new Color(0.5f, 0.7f, 1f)
        );

        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(0, 0);
        titleRect.offsetMax = new Vector2(0, -20);

        // Decorative line under title
        GameObject line = new GameObject("TitleLine");
        line.transform.SetParent(header.transform, false);
        Image lineImg = line.AddComponent<Image>();
        lineImg.color = new Color(0.4f, 0.6f, 1f, 0.5f);
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.3f, 0);
        lineRect.anchorMax = new Vector2(0.7f, 0);
        lineRect.sizeDelta = new Vector2(0, 3);
        lineRect.anchoredPosition = new Vector2(0, 10);
    }

    void CreateLevelGrid(Transform parent)
    {
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.SetParent(parent, false);
        RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.05f, 0.2f);
        gridRect.anchorMax = new Vector2(0.95f, 0.82f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        // Get available levels
        string[] levels = GameSaveManager.AvailableLevels;
        int columns = Mathf.Min(levels.Length, 4);
        int rows = Mathf.CeilToInt(levels.Length / (float)columns);

        float cardWidth = 350f;
        float cardHeight = 280f;
        float spacing = 40f;

        float totalWidth = columns * cardWidth + (columns - 1) * spacing;
        float startX = -totalWidth / 2 + cardWidth / 2;

        for (int i = 0; i < levels.Length; i++)
        {
            int col = i % columns;
            int row = i / columns;

            float x = startX + col * (cardWidth + spacing);
            float y = -row * (cardHeight + spacing);

            CreateLevelCard(gridContainer.transform, levels[i], i + 1, new Vector2(x, y), cardWidth, cardHeight);
        }
    }

    void CreateLevelCard(Transform parent, string levelName, int levelNumber, Vector2 position, float width, float height)
    {
        LevelData levelData = GameSaveManager.Instance.GetLevelData(levelName);
        bool isUnlocked = levelData != null && levelData.isUnlocked;
        bool isCompleted = levelData != null && levelData.isCompleted;

        // Card container
        GameObject card = new GameObject($"LevelCard_{levelName}");
        card.transform.SetParent(parent, false);
        levelCards.Add(card);

        // Set initial scale for animation
        card.transform.localScale = Vector3.zero;

        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(width, height);
        cardRect.anchoredPosition = position;

        // Card background with glow effect
        GameObject glowBg = new GameObject("Glow");
        glowBg.transform.SetParent(card.transform, false);
        Image glowImg = glowBg.AddComponent<Image>();
        glowImg.color = isUnlocked ?
            new Color(0.3f, 0.5f, 0.8f, 0.3f) :
            new Color(0.2f, 0.2f, 0.2f, 0.2f);
        RectTransform glowRect = glowBg.GetComponent<RectTransform>();
        glowRect.anchorMin = Vector2.zero;
        glowRect.anchorMax = Vector2.one;
        glowRect.offsetMin = new Vector2(-10, -10);
        glowRect.offsetMax = new Vector2(10, 10);

        // Main card background
        GameObject cardBg = new GameObject("CardBackground");
        cardBg.transform.SetParent(card.transform, false);
        Image cardBgImg = cardBg.AddComponent<Image>();
        cardBgImg.color = isUnlocked ?
            new Color(0.1f, 0.15f, 0.25f, 0.95f) :
            new Color(0.08f, 0.08f, 0.1f, 0.95f);
        RectTransform cardBgRect = cardBg.GetComponent<RectTransform>();
        cardBgRect.anchorMin = Vector2.zero;
        cardBgRect.anchorMax = Vector2.one;
        cardBgRect.offsetMin = Vector2.zero;
        cardBgRect.offsetMax = Vector2.zero;

        // Level number badge
        CreateLevelBadge(card.transform, levelNumber, isUnlocked);

        // Level name
        GameObject nameObj = new GameObject("LevelName");
        nameObj.transform.SetParent(card.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = levelName.ToUpper();
        nameText.fontSize = 28;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = isUnlocked ? new Color(0.9f, 0.95f, 1f) : new Color(0.4f, 0.4f, 0.45f);
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.55f);
        nameRect.anchorMax = new Vector2(1, 0.7f);
        nameRect.offsetMin = new Vector2(10, 0);
        nameRect.offsetMax = new Vector2(-10, 0);

        // Stars display
        CreateStarsDisplay(card.transform, levelData?.bestStars ?? 0, isUnlocked);

        // Best time display
        CreateBestTimeDisplay(card.transform, levelData, isUnlocked);

        // Status indicator
        CreateStatusIndicator(card.transform, isUnlocked, isCompleted);

        // Button functionality
        if (isUnlocked)
        {
            Button btn = card.AddComponent<Button>();
            btn.onClick.AddListener(() => OnLevelSelected(levelName));

            // Hover effect
            LevelCardHover hover = card.AddComponent<LevelCardHover>();
            hover.Initialize(glowImg, cardBgImg);
        }
    }

    void CreateLevelBadge(Transform parent, int number, bool isUnlocked)
    {
        GameObject badge = new GameObject("Badge");
        badge.transform.SetParent(parent, false);
        Image badgeImg = badge.AddComponent<Image>();
        badgeImg.color = isUnlocked ?
            new Color(0.3f, 0.5f, 0.8f, 1f) :
            new Color(0.2f, 0.2f, 0.25f, 1f);

        RectTransform badgeRect = badge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0.5f, 1f);
        badgeRect.anchorMax = new Vector2(0.5f, 1f);
        badgeRect.sizeDelta = new Vector2(60, 60);
        badgeRect.anchoredPosition = new Vector2(0, -30);

        // Number text
        GameObject numObj = new GameObject("Number");
        numObj.transform.SetParent(badge.transform, false);
        TextMeshProUGUI numText = numObj.AddComponent<TextMeshProUGUI>();
        numText.text = number.ToString();
        numText.fontSize = 32;
        numText.fontStyle = FontStyles.Bold;
        numText.alignment = TextAlignmentOptions.Center;
        numText.color = Color.white;
        RectTransform numRect = numObj.GetComponent<RectTransform>();
        numRect.anchorMin = Vector2.zero;
        numRect.anchorMax = Vector2.one;
        numRect.offsetMin = Vector2.zero;
        numRect.offsetMax = Vector2.zero;
    }

    void CreateStarsDisplay(Transform parent, int stars, bool isUnlocked)
    {
        GameObject starsContainer = new GameObject("Stars");
        starsContainer.transform.SetParent(parent, false);
        RectTransform starsRect = starsContainer.AddComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0, 0.35f);
        starsRect.anchorMax = new Vector2(1, 0.5f);
        starsRect.offsetMin = Vector2.zero;
        starsRect.offsetMax = Vector2.zero;

        float starSize = 35f;
        float spacing = 10f;
        float totalWidth = 4 * starSize + 3 * spacing;
        float startX = -totalWidth / 2 + starSize / 2;

        for (int i = 0; i < 4; i++)
        {
            GameObject star = new GameObject($"Star_{i}");
            star.transform.SetParent(starsContainer.transform, false);
            TextMeshProUGUI starText = star.AddComponent<TextMeshProUGUI>();
            starText.text = "\u2605"; // Star symbol
            starText.fontSize = 30;
            starText.alignment = TextAlignmentOptions.Center;

            if (!isUnlocked)
            {
                starText.color = new Color(0.25f, 0.25f, 0.3f);
            }
            else if (i < stars)
            {
                starText.color = new Color(1f, 0.85f, 0.2f); // Gold
            }
            else
            {
                starText.color = new Color(0.3f, 0.35f, 0.4f); // Gray
            }

            RectTransform starRect = star.GetComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0.5f, 0.5f);
            starRect.anchorMax = new Vector2(0.5f, 0.5f);
            starRect.sizeDelta = new Vector2(starSize, starSize);
            starRect.anchoredPosition = new Vector2(startX + i * (starSize + spacing), 0);
        }
    }

    void CreateBestTimeDisplay(Transform parent, LevelData levelData, bool isUnlocked)
    {
        GameObject timeContainer = new GameObject("BestTime");
        timeContainer.transform.SetParent(parent, false);
        RectTransform timeRect = timeContainer.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0, 0.15f);
        timeRect.anchorMax = new Vector2(1, 0.3f);
        timeRect.offsetMin = new Vector2(20, 0);
        timeRect.offsetMax = new Vector2(-20, 0);

        TextMeshProUGUI timeText = timeContainer.AddComponent<TextMeshProUGUI>();

        if (!isUnlocked)
        {
            timeText.text = "LOCKED";
            timeText.color = new Color(0.4f, 0.4f, 0.45f);
        }
        else if (levelData != null && levelData.bestTimeMs > 0)
        {
            timeText.text = "Best: " + GameSaveManager.Instance.FormatTime(levelData.bestTimeMs);
            timeText.color = new Color(0.6f, 0.8f, 0.6f);
        }
        else
        {
            timeText.text = "Not completed";
            timeText.color = new Color(0.5f, 0.55f, 0.6f);
        }

        timeText.fontSize = 18;
        timeText.alignment = TextAlignmentOptions.Center;
    }

    void CreateStatusIndicator(Transform parent, bool isUnlocked, bool isCompleted)
    {
        if (!isUnlocked)
        {
            // Lock icon
            GameObject lockIcon = new GameObject("LockIcon");
            lockIcon.transform.SetParent(parent, false);
            TextMeshProUGUI lockText = lockIcon.AddComponent<TextMeshProUGUI>();
            lockText.text = "\u26BF"; // Lock symbol (or use a custom texture)
            lockText.fontSize = 48;
            lockText.alignment = TextAlignmentOptions.Center;
            lockText.color = new Color(0.4f, 0.4f, 0.45f, 0.7f);

            RectTransform lockRect = lockIcon.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockRect.sizeDelta = new Vector2(80, 80);
            lockRect.anchoredPosition = Vector2.zero;
        }
        else if (isCompleted)
        {
            // Checkmark in corner
            GameObject check = new GameObject("Checkmark");
            check.transform.SetParent(parent, false);
            TextMeshProUGUI checkText = check.AddComponent<TextMeshProUGUI>();
            checkText.text = "\u2713"; // Checkmark
            checkText.fontSize = 24;
            checkText.alignment = TextAlignmentOptions.Center;
            checkText.color = new Color(0.4f, 0.9f, 0.4f);

            RectTransform checkRect = check.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(1, 1);
            checkRect.anchorMax = new Vector2(1, 1);
            checkRect.sizeDelta = new Vector2(40, 40);
            checkRect.anchoredPosition = new Vector2(-25, -25);
        }
    }

    void CreateFooter(Transform parent)
    {
        GameObject footer = new GameObject("Footer");
        footer.transform.SetParent(parent, false);
        RectTransform footerRect = footer.AddComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0.15f);
        footerRect.offsetMin = Vector2.zero;
        footerRect.offsetMax = Vector2.zero;

        // Total stars display
        GameObject statsObj = new GameObject("TotalStars");
        statsObj.transform.SetParent(footer.transform, false);
        TextMeshProUGUI statsText = statsObj.AddComponent<TextMeshProUGUI>();
        int totalStars = GameSaveManager.Instance.GetTotalStars();
        int maxStars = GameSaveManager.AvailableLevels.Length * 4;
        statsText.text = $"\u2605 {totalStars} / {maxStars}";
        statsText.fontSize = 28;
        statsText.alignment = TextAlignmentOptions.Left;
        statsText.color = new Color(1f, 0.85f, 0.2f);

        RectTransform statsRect = statsObj.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0, 0.3f);
        statsRect.anchorMax = new Vector2(0.3f, 0.7f);
        statsRect.offsetMin = new Vector2(50, 0);
        statsRect.offsetMax = new Vector2(0, 0);

        // Back button
        CreateBackButton(footer.transform);
    }

    void CreateBackButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("BackButton");
        buttonObj.transform.SetParent(parent, false);

        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.15f, 0.2f, 0.3f, 0.9f);

        Button btn = buttonObj.AddComponent<Button>();
        btn.onClick.AddListener(OnBackClicked);

        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.15f, 0.2f, 0.3f, 0.9f);
        colors.highlightedColor = new Color(0.25f, 0.4f, 0.6f, 1f);
        colors.pressedColor = new Color(0.1f, 0.25f, 0.4f, 1f);
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.2f);
        rect.anchorMax = new Vector2(1, 0.8f);
        rect.sizeDelta = new Vector2(180, 0);
        rect.anchoredPosition = new Vector2(-130, 0);

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "BACK";
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.9f, 0.95f, 1f);
        tmp.characterSpacing = 3;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Add button animation
        buttonObj.AddComponent<ButtonAnimator>();
    }

    IEnumerator AnimateCardsIn()
    {
        foreach (GameObject card in levelCards)
        {
            StartCoroutine(AnimateCardScale(card, Vector3.zero, Vector3.one, cardAnimationDuration));
            yield return new WaitForSecondsRealtime(cardAnimationDelay);
        }
    }

    IEnumerator AnimateCardScale(GameObject card, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Ease out back curve for bouncy effect
            float eased = 1 + 2.70158f * Mathf.Pow(t - 1, 3) + 1.70158f * Mathf.Pow(t - 1, 2);
            card.transform.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        card.transform.localScale = to;
    }

    void OnLevelSelected(string levelName)
    {
        Hide();
        mainScript.loadLevel(levelName);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        onLevelSelectedCallback?.Invoke();
    }

    void OnBackClicked()
    {
        Hide();
        onBackCallback?.Invoke();
    }

    public void Show()
    {
        menuCanvas.gameObject.SetActive(true);
        RefreshLevelCards();
        StartCoroutine(AnimateCardsIn());
    }

    public void Hide()
    {
        menuCanvas.gameObject.SetActive(false);
    }

    void RefreshLevelCards()
    {
        // Rebuild cards with updated data
        if (mainPanel != null)
        {
            Transform gridContainer = mainPanel.transform.Find("GridContainer");
            if (gridContainer != null)
            {
                foreach (Transform child in gridContainer)
                {
                    Destroy(child.gameObject);
                }
                levelCards.Clear();
                CreateLevelGrid(mainPanel.transform);
            }
        }
    }
}

// Helper component for level card hover effect
public class LevelCardHover : MonoBehaviour, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
{
    private Image glowImage;
    private Image cardImage;
    private Color originalGlowColor;
    private Color originalCardColor;
    private Color hoverGlowColor;
    private Color hoverCardColor;
    private bool isHovered = false;
    private float transitionSpeed = 8f;

    public void Initialize(Image glow, Image card)
    {
        glowImage = glow;
        cardImage = card;
        originalGlowColor = glow.color;
        originalCardColor = card.color;
        hoverGlowColor = new Color(0.4f, 0.6f, 1f, 0.6f);
        hoverCardColor = new Color(0.15f, 0.22f, 0.35f, 1f);
    }

    void Update()
    {
        if (glowImage == null || cardImage == null) return;

        Color targetGlow = isHovered ? hoverGlowColor : originalGlowColor;
        Color targetCard = isHovered ? hoverCardColor : originalCardColor;

        glowImage.color = Color.Lerp(glowImage.color, targetGlow, Time.unscaledDeltaTime * transitionSpeed);
        cardImage.color = Color.Lerp(cardImage.color, targetCard, Time.unscaledDeltaTime * transitionSpeed);

        // Scale effect
        Vector3 targetScale = isHovered ? new Vector3(1.05f, 1.05f, 1f) : Vector3.one;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isHovered = false;
    }
}
