using UnityEngine;

// Gestor centralizado de sonidos para la interfaz de usuario
public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    
    [Header("UI Sound Effects")]
    [Tooltip("Sonido al hacer clic en un botón normal")]
    public AudioClip buttonClickSound;
    
    [Tooltip("Sonido al pasar el mouse sobre un botón")]
    public AudioClip buttonHoverSound;
    
    [Tooltip("Sonido para botones importantes (comenzar, confirmar)")]
    public AudioClip buttonConfirmSound;
    
    [Tooltip("Sonido para botones de cancelar o volver")]
    public AudioClip buttonCancelSound;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 0.5f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.playOnAwake = false;
            audioSource.volume = volume;
            
            Debug.Log("🔊 UIAudioManager inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Reproduce el sonido de clic normal
    public void PlayClickSound()
    {
        PlaySound(buttonClickSound);
    }
    
    // Reproduce el sonido de hover
    public void PlayHoverSound()
    {
        PlaySound(buttonHoverSound);
    }
    
    // Reproduce el sonido de confirmación
    public void PlayConfirmSound()
    {
        PlaySound(buttonConfirmSound);
    }
    
    // Reproduce el sonido de cancelación
    public void PlayCancelSound()
    {
        PlaySound(buttonCancelSound);
    }
    
    // Reproduce un sonido específico
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    // Cambia el volumen de los sonidos de UI
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
