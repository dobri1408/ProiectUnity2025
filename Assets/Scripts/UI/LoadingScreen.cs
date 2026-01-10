using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Gestioneaza ecranul de incarcare si incarcarea asincrona a nivelurilor
public class LoadingScreen : MonoBehaviour
{
    private Canvas canvas;
    private Image backgroundImage;
    private Image progressBarFill;
    private TextMeshProUGUI loadingText;
    private TextMeshProUGUI percentText;
    private CanvasGroup canvasGroup;

    private float fadeSpeed = 2f;
    private bool isLoading = false;

    void Awake()
    {
        CreateUI();
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
    }

    void CreateUI()
    {
        // Canvas principal
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // deasupra tuturor

        gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Background negru
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(transform, false);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Container centrat
        GameObject container = new GameObject("Container");
        container.transform.SetParent(transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(400, 100);

        // Text "Loading..."
        GameObject textObj = new GameObject("LoadingText");
        textObj.transform.SetParent(container.transform, false);
        loadingText = textObj.AddComponent<TextMeshProUGUI>();
        loadingText.text = "Se incarca...";
        loadingText.fontSize = 32;
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

        // Procent text
        GameObject percentObj = new GameObject("PercentText");
        percentObj.transform.SetParent(container.transform, false);
        percentText = percentObj.AddComponent<TextMeshProUGUI>();
        percentText.text = "0%";
        percentText.fontSize = 20;
        percentText.alignment = TextAlignmentOptions.Center;
        percentText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        RectTransform percentRect = percentObj.GetComponent<RectTransform>();
        percentRect.anchorMin = new Vector2(0, 0);
        percentRect.anchorMax = new Vector2(1, 0.2f);
        percentRect.sizeDelta = Vector2.zero;
        percentRect.anchoredPosition = Vector2.zero;
    }

    // Incarca un nivel asincron cu callback la final
    public void LoadLevelAsync(string levelName, Action<GameObject, GameObject, GameObject> onComplete)
    {
        if (isLoading) return;
        StartCoroutine(LoadLevelCoroutine(levelName, onComplete));
    }

    IEnumerator LoadLevelCoroutine(string levelName, Action<GameObject, GameObject, GameObject> onComplete)
    {
        isLoading = true;

        // Fade in loading screen
        yield return StartCoroutine(FadeIn());

        loadingText.text = "Se incarca " + levelName + "...";
        SetProgress(0f);

        // Incarca nivelul async
        ResourceRequest levelRequest = Resources.LoadAsync<GameObject>("Levels/" + levelName);
        while (!levelRequest.isDone)
        {
            SetProgress(levelRequest.progress * 0.5f); // 0-50%
            yield return null;
        }
        GameObject levelPrefab = levelRequest.asset as GameObject;

        // Incarca player async
        loadingText.text = "Se incarca jucatorul...";
        ResourceRequest playerRequest = Resources.LoadAsync<GameObject>("Player");
        while (!playerRequest.isDone)
        {
            SetProgress(0.5f + playerRequest.progress * 0.3f); // 50-80%
            yield return null;
        }
        GameObject playerPrefab = playerRequest.asset as GameObject;

        // Incarca UI async
        loadingText.text = "Se incarca interfata...";
        ResourceRequest uiRequest = Resources.LoadAsync<GameObject>("UIs/UI");
        while (!uiRequest.isDone)
        {
            SetProgress(0.8f + uiRequest.progress * 0.2f); // 80-100%
            yield return null;
        }
        GameObject uiPrefab = uiRequest.asset as GameObject;

        SetProgress(1f);
        loadingText.text = "Finalizare...";

        // Mic delay pentru efect vizual
        yield return new WaitForSeconds(0.3f);

        // Callback cu prefab-urile incarcate
        onComplete?.Invoke(levelPrefab, playerPrefab, uiPrefab);

        // Fade out loading screen
        yield return StartCoroutine(FadeOut());

        isLoading = false;
    }

    void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        progressBarFill.rectTransform.anchorMax = new Vector2(progress, 1);
        percentText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    IEnumerator FadeIn()
    {
        canvas.enabled = true;
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
    }

    public bool IsLoading()
    {
        return isLoading;
    }
}
