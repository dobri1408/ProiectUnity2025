using UnityEngine;

// Gestioneaza muzica de fundal in joc
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float musicVolume = 0.5f;
    private float targetVolume = 0.5f;
    private bool isInMenu = true;

    private const float MENU_VOLUME_MULTIPLIER = 0.4f; // volumul in meniu e 40% din gameplay
    private const float FADE_SPEED = 2f;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudio();
    }

    void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound

        // incarca muzica din Resources
        AudioClip music = Resources.Load<AudioClip>("Music/Vibing Over Venus");
        if (music != null)
        {
            audioSource.clip = music;
            LoadVolume();
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("MusicManager: Nu am gasit fisierul muzical in Resources/Music/");
        }
    }

    void Update()
    {
        // fade smooth intre volume
        float currentTarget = isInMenu ? targetVolume * MENU_VOLUME_MULTIPLIER : targetVolume;
        audioSource.volume = Mathf.Lerp(audioSource.volume, currentTarget, Time.unscaledDeltaTime * FADE_SPEED);
    }

    // seteaza volumul muzicii (0-1)
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        targetVolume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    // returneaza volumul curent
    public float GetVolume()
    {
        return musicVolume;
    }

    // apeleaza cand intri in gameplay
    public void OnEnterGameplay()
    {
        isInMenu = false;
    }

    // apeleaza cand intri in meniu/pauza
    public void OnEnterMenu()
    {
        isInMenu = true;
    }

    void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        targetVolume = musicVolume;
        audioSource.volume = musicVolume * MENU_VOLUME_MULTIPLIER;
    }
}
