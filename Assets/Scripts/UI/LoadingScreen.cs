using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Manages loading screen and asynchronous level loading
public class LoadingScreen : MonoBehaviour
{
    // Loading screen constants
    private const float fadeSpeed = 2f;
    private const int canvasSortingOrder = 100;
    private const float levelLoadProgress = 0.5f;
    private const float playerLoadProgress = 0.3f;
    private const float uiLoadProgress = 0.2f;
    private const float completionDelay = 0.3f;
    private const float bgColorR = 0.1f;
    private const float bgColorG = 0.1f;
    private const float bgColorB = 0.15f;
    private const int textFontSize = 32;
    private const int percentFontSize = 20;

    private Canvas canvas;
    private Image backgroundImage;
    private Image progressBarFill;
    private TextMeshProUGUI loadingText;
    private TextMeshProUGUI percentText;
    private CanvasGroup canvasGroup;

    private float fadeSpeedValue = fadeSpeed;
    private bool isLoading = false;

    void Awake()
    {
        CreateUI();
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
    }

    void CreateUI()
    {
        // Main canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = canvasSortingOrder; // on top of everything

        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Black background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(transform, false);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = new Color(bgColorR, bgColorG, bgColorB, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Centered container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 100);

        // Loading text
        GameObject textObj = new GameObject("LoadingText");
        textObj.transform.SetParent(container.transform, false);
        loadingText = textObj.AddComponent<TextMeshProUGUI>();
        loadingText.text = "Loading...";
        loadingText.fontSize = textFontSize;
        loadingText.alignment = TextAlignmentOptions.Center;
        loadingText.color = Color.white;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.6f);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        // Progress bar background
        GameObject barBg = new GameObject("ProgressBarBg");
        barBg.transform.SetParent(container.transform, false);
        Image barBgImage = barBg.AddComponent<Image>();
        barBgImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);
        RectTransform barBgRect = barBg.GetComponent<RectTransform>();
        barBgRect.anchorMin = new Vector2(0, 0.2f);
        barBgRect.anchorMax = new Vector2(1, 0.4f);
        barBgRect.sizeDelta = Vector2.zero;
        barBgRect.anchoredPosition = Vector2.zero;

        // Progress bar fill
        GameObject barFill = new GameObject("ProgressBarFill");
        barFill.transform.SetParent(barBg.transform, false);
        progressBarFill = barFill.AddComponent<Image>();
        progressBarFill.color = new Color(0.3f, 0.7f, 1f, 1f);
        RectTransform barFillRect = barFill.GetComponent<RectTransform>();
        barFillRect.anchorMin = Vector2.zero;
        barFillRect.anchorMax = new Vector2(0, 1);
        barFillRect.sizeDelta = Vector2.zero;
        barFillRect.pivot = new Vector2(0, 0.5f);
        barFillRect.anchoredPosition = Vector2.zero;

        // Percent text
        GameObject percentObj = new GameObject("PercentText");
        percentObj.transform.SetParent(container.transform, false);
        percentText = percentObj.AddComponent<TextMeshProUGUI>();
        percentText.text = "0%";
        percentText.fontSize = percentFontSize;
        percentText.alignment = TextAlignmentOptions.Center;
        percentText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        RectTransform percentRect = percentObj.GetComponent<RectTransform>();
        percentRect.anchorMin = new Vector2(0, 0);
        percentRect.anchorMax = new Vector2(1, 0.2f);
        percentRect.sizeDelta = Vector2.zero;
        percentRect.anchoredPosition = Vector2.zero;
    }

    // Loads a level asynchronously with callback when complete
    public void LoadLevelAsync(string levelName, Action<GameObject, GameObject, GameObject> onComplete)
    {
        if (isLoading) return;
        StartCoroutine(LoadLevelCoroutine(levelName, onComplete));
    }

    // Coroutine that manages asynchronous resource loading with progress tracking
    IEnumerator LoadLevelCoroutine(string levelName, Action<GameObject, GameObject, GameObject> onComplete)
    {
        isLoading = true;

        // Fade in loading screen
        yield return StartCoroutine(FadeIn());

        loadingText.text = "Loading " + levelName + "...";
        SetProgress(0f);

        // Load level asynchronously
        ResourceRequest levelRequest = Resources.LoadAsync<GameObject>("Levels/" + levelName);
        while (!levelRequest.isDone)
        {
            SetProgress(levelRequest.progress * levelLoadProgress); // 0-50%
            yield return null;
        }
        GameObject levelPrefab = levelRequest.asset as GameObject;

        // Load player asynchronously
        loadingText.text = "Loading player...";
        ResourceRequest playerRequest = Resources.LoadAsync<GameObject>("Player");
        while (!playerRequest.isDone)
        {
            SetProgress(levelLoadProgress + playerRequest.progress * playerLoadProgress); // 50-80%
            yield return null;
        }
        GameObject playerPrefab = playerRequest.asset as GameObject;

        // Load UI asynchronously
        loadingText.text = "Loading interface...";
        ResourceRequest uiRequest = Resources.LoadAsync<GameObject>("UIs/UI");
        while (!uiRequest.isDone)
        {
            SetProgress((levelLoadProgress + playerLoadProgress) + uiRequest.progress * uiLoadProgress); // 80-100%
            yield return null;
        }
        GameObject uiPrefab = uiRequest.asset as GameObject;

        SetProgress(1f);
        loadingText.text = "Finalizing...";

        // Small delay for visual effect
        yield return new WaitForSeconds(completionDelay);

        // Invoke callback with loaded prefabs
        onComplete?.Invoke(levelPrefab, playerPrefab, uiPrefab);

        // Fade out loading screen
        yield return StartCoroutine(FadeOut());

        isLoading = false;
    }

    // Updates progress bar fill and percentage text
    void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        progressBarFill.rectTransform.anchorMax = new Vector2(progress, 1);
        percentText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    // Fades in the loading screen with smooth alpha transition
    IEnumerator FadeIn()
    {
        canvas.enabled = true;
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeedValue;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    // Fades out the loading screen with smooth alpha transition
    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeedValue;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
    }

    // Returns whether a level is currently loading
    public bool IsLoading()
    {
        return isLoading;
    }
}
