using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    [Header("Music Tracks")]
    [Tooltip("Música para el menú principal y selección de personaje")]
    public AudioClip menuMusic;
    
    [Tooltip("Música durante el combate")]
    public AudioClip battleMusic;
    
    [Tooltip("Música de Game Over")]
    public AudioClip gameOverMusic;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 0.7f;
    
    [Range(0f, 1f)]
    [Tooltip("Volumen de la música cuando el juego está pausado")]
    public float pausedVolume = 0.3f;
    
    [Tooltip("Duración del fade entre canciones (segundos)")]
    public float fadeDuration = 1f;
    
    [Tooltip("Duración del fade al pausar/despausar (segundos)")]
    public float pauseFadeDuration = 0.5f;
    
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private bool isPaused = false;
    
    void Awake()
    {
        // Singleton pattern - solo una instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.loop = true;
            audioSource.volume = volume;
            
            Debug.Log("MusicManager inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// Reproduce la música del menú principal y selección de personaje
    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, "Menu Music");
    }
    
    /// Reproduce la música de batalla
    public void PlayBattleMusic()
    {
        PlayMusic(battleMusic, "Battle Music");
    }
    
    /// Reproduce la música de Game Over
    public void PlayGameOverMusic()
    {
        PlayMusic(gameOverMusic, "Game Over Music");
    }

    public void PlayCustomMusic(AudioClip customClip, string name = "Custom Music")
    {
        PlayMusic(customClip, name);
    }
    
    /// Detiene la música con fade out
    public void StopMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        fadeCoroutine = StartCoroutine(FadeOut());
    }
    
    /// Cambia la música con fade
    private void PlayMusic(AudioClip newClip, string trackName)
    {
        if (newClip == null)
        {
            Debug.LogWarning($"MusicManager: {trackName} no está asignado");
            return;
        }
        
        // Si ya está sonando el mismo clip, no hacer nada
        if (audioSource != null && audioSource.clip == newClip && audioSource.isPlaying)
        {
            Debug.Log($"{trackName} ya está sonando (clip={newClip.name}, isPlaying={audioSource.isPlaying})");
            return;
        }
        
        Debug.Log($"Cambiando a: {trackName} (clip actual={audioSource?.clip?.name}, nuevo={newClip.name})");
        
        // Detener fade anterior si existe
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Iniciar crossfade
        fadeCoroutine = StartCoroutine(CrossFade(newClip, trackName));
    }
    
    /// Hace crossfade entre la música actual y la nueva
    private IEnumerator CrossFade(AudioClip newClip, string trackName)
    {
        float timer = 0f;
        float startVolume = audioSource.volume;
        
        // Fade out de la música actual
        if (audioSource.isPlaying)
        {
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }
        }
        
        // Cambiar clip
        audioSource.clip = newClip;
        audioSource.Play();
        
        Debug.Log($"Reproduciendo: {trackName}");
        
        // Fade in de la nueva música
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, timer / fadeDuration);
            yield return null;
        }
        
        audioSource.volume = volume;
        fadeCoroutine = null;
    }
    
    /// Hace fade out sin cambiar de canción
    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float startVolume = audioSource.volume;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = volume;
        fadeCoroutine = null;
        
        Debug.Log("Música detenida");
    }
    
    /// Cambia el volumen global de la música
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null && !isPaused)
        {
            audioSource.volume = volume;
        }
    }

    /// Baja el volumen de la música cuando el juego se pausa
    public void SetPausedState(bool paused)
    {
        if (isPaused == paused) return; // Ya está en ese estado
        
        isPaused = paused;
        
        // Detener fade anterior si existe
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        if (paused)
        {
            // Bajar volumen gradualmente
            fadeCoroutine = StartCoroutine(FadeToVolume(pausedVolume, pauseFadeDuration));
            Debug.Log($"Música pausada - bajando volumen a {pausedVolume}");
        }
        else
        {
            // Restaurar volumen normal
            fadeCoroutine = StartCoroutine(FadeToVolume(volume, pauseFadeDuration));
            Debug.Log($"Música reanudada - restaurando volumen a {volume}");
        }
    }
    
    /// Hace fade del volumen actual a un volumen objetivo
    private IEnumerator FadeToVolume(float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // Usar unscaledDeltaTime para que funcione durante la pausa
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }
        
        audioSource.volume = targetVolume;
        fadeCoroutine = null;
    }
}
