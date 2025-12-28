using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Adds smooth hover and click animations to buttons
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float hoverScale = 1.05f;
    private float pressScale = 0.95f;
    private float animationSpeed = 12f;

    private Image backgroundImage;
    private Image borderImage;
    private TextMeshProUGUI buttonText;

    private Color originalBgColor;
    private Color hoverBgColor;
    private Color pressBgColor;

    private Color originalBorderColor;
    private Color hoverBorderColor;

    private bool isHovered = false;
    private bool isPressed = false;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            originalBgColor = backgroundImage.color;
            hoverBgColor = new Color(
                Mathf.Min(originalBgColor.r + 0.1f, 1f),
                Mathf.Min(originalBgColor.g + 0.15f, 1f),
                Mathf.Min(originalBgColor.b + 0.2f, 1f),
                originalBgColor.a
            );
            pressBgColor = new Color(
                originalBgColor.r * 0.8f,
                originalBgColor.g * 0.8f,
                originalBgColor.b * 0.8f,
                originalBgColor.a
            );
        }

        // Find border
        Transform borderTransform = transform.Find("Border");
        if (borderTransform != null)
        {
            borderImage = borderTransform.GetComponent<Image>();
            if (borderImage != null)
            {
                originalBorderColor = borderImage.color;
                hoverBorderColor = new Color(0.5f, 0.7f, 1f, 0.7f);
            }
        }

        // Find text
        Transform textTransform = transform.Find("Text");
        if (textTransform != null)
        {
            buttonText = textTransform.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Smooth scale animation
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);

        // Color animations
        if (backgroundImage != null)
        {
            Color targetBg = isPressed ? pressBgColor : (isHovered ? hoverBgColor : originalBgColor);
            backgroundImage.color = Color.Lerp(backgroundImage.color, targetBg, Time.unscaledDeltaTime * animationSpeed);
        }

        if (borderImage != null)
        {
            Color targetBorder = isHovered ? hoverBorderColor : originalBorderColor;
            borderImage.color = Color.Lerp(borderImage.color, targetBorder, Time.unscaledDeltaTime * animationSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = originalScale * pressScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (isHovered)
        {
            targetScale = originalScale * hoverScale;
        }
        else
        {
            targetScale = originalScale;
        }
    }
}

// Pulsing glow effect for highlighted elements
public class PulsingGlow : MonoBehaviour
{
    private Image image;
    private Color baseColor;
    private float pulseSpeed = 2f;
    private float pulseAmount = 0.3f;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            baseColor = image.color;
        }
    }

    void Update()
    {
        if (image != null)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
            float alpha = baseColor.a + pulse * pulseAmount;
            image.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
    }
}

// Slide in animation for UI elements
public class SlideInAnimation : MonoBehaviour
{
    public enum SlideDirection { Left, Right, Top, Bottom }
    public SlideDirection direction = SlideDirection.Bottom;
    public float slideDistance = 100f;
    public float duration = 0.5f;
    public float delay = 0f;

    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private float elapsedTime = 0f;
    private bool animating = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetPosition = rectTransform.anchoredPosition;

        Vector2 offset = direction switch
        {
            SlideDirection.Left => new Vector2(-slideDistance, 0),
            SlideDirection.Right => new Vector2(slideDistance, 0),
            SlideDirection.Top => new Vector2(0, slideDistance),
            SlideDirection.Bottom => new Vector2(0, -slideDistance),
            _ => Vector2.zero
        };

        startPosition = targetPosition + offset;
        rectTransform.anchoredPosition = startPosition;
    }

    void OnEnable()
    {
        elapsedTime = -delay;
        animating = true;
        rectTransform.anchoredPosition = startPosition;
    }

    void Update()
    {
        if (!animating) return;

        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime < 0) return;

        float t = Mathf.Clamp01(elapsedTime / duration);
        // Ease out cubic
        float eased = 1 - Mathf.Pow(1 - t, 3);

        rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, eased);

        if (t >= 1f)
        {
            animating = false;
            rectTransform.anchoredPosition = targetPosition;
        }
    }
}

// Fade in animation
public class FadeInAnimation : MonoBehaviour
{
    public float duration = 0.5f;
    public float delay = 0f;

    private CanvasGroup canvasGroup;
    private float elapsedTime = 0f;
    private bool animating = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    void OnEnable()
    {
        elapsedTime = -delay;
        animating = true;
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!animating) return;

        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime < 0) return;

        float t = Mathf.Clamp01(elapsedTime / duration);
        canvasGroup.alpha = t;

        if (t >= 1f)
        {
            animating = false;
        }
    }
}

// Floating rock animation for climbing theme
public class FloatingRock : MonoBehaviour
{
    private float speed;
    private float amplitude;
    private float rotationSpeed;
    private Vector2 startPos;
    private float offset;
    private RectTransform rectTransform;

    public void Initialize(float spd, float amp, float rotSpd)
    {
        speed = spd;
        amplitude = amp;
        rotationSpeed = rotSpd;
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
        offset = Random.Range(0f, Mathf.PI * 2);
    }

    void Update()
    {
        if (rectTransform == null) return;

        float time = Time.unscaledTime * speed + offset;

        // Floating movement
        rectTransform.anchoredPosition = startPos + new Vector2(
            Mathf.Sin(time * 0.7f) * amplitude * 0.3f,
            Mathf.Sin(time) * amplitude
        );

        // Slow rotation
        rectTransform.Rotate(0, 0, rotationSpeed * Time.unscaledDeltaTime * 10f);
    }
}

// Mountain parallax effect
public class MountainParallax : MonoBehaviour
{
    public float speed = 5f;
    private RectTransform rectTransform;
    private Vector2 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (rectTransform == null) return;

        float offset = Mathf.Sin(Time.unscaledTime * 0.1f) * speed;
        rectTransform.anchoredPosition = startPos + new Vector2(offset, 0);
    }
}

// Rope swaying animation
public class RopeSwayAnimation : MonoBehaviour
{
    private float direction = 1f;
    private RectTransform rectTransform;
    private float swayAmount = 8f;
    private float swaySpeed = 1.5f;

    public void Initialize(float dir)
    {
        direction = dir;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rectTransform == null) return;

        float sway = Mathf.Sin(Time.unscaledTime * swaySpeed) * swayAmount * direction;
        rectTransform.localRotation = Quaternion.Euler(0, 0, sway);
    }
}

// Climbing character silhouette animation (bonus)
public class ClimberAnimation : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 startPos;
    private float climbSpeed = 30f;
    private float maxHeight = 200f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Climbing up and down movement
        float cycle = Mathf.PingPong(Time.unscaledTime * climbSpeed, maxHeight);
        rectTransform.anchoredPosition = startPos + new Vector2(
            Mathf.Sin(Time.unscaledTime * 2f) * 5f, // Slight horizontal sway
            cycle
        );
    }
}
