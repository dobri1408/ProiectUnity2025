using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private GameObject player;
    private Player playerObj;
    private RectTransform rectTransform;
    private Image image;
    private float maxWidth;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        maxWidth = rectTransform.sizeDelta.x;
    }
    
    void FixedUpdate()
    {
        // Find player
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (playerObj == null && player != null)
        {
            playerObj = player.GetComponentInChildren<Player>();
        }
        
        // Update the bar
        if (playerObj != null)
        {
            float newWidth = maxWidth * (playerObj.stamina / playerObj.maxStamina);
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
            
            // Change color based on exhausted state
            image.color = playerObj.exhausted ? Color.red : Color.white;
        }
    }
}