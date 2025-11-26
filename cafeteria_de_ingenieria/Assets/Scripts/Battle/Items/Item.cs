using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{
    [Header("Base Item")]
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Visual & Audio Effects")]
    [Tooltip("Prefab del efecto visual (partículas, animación, etc.)")]
    public GameObject effectPrefab;
    
    [Tooltip("Sonido que se reproduce al usar el item")]
    public AudioClip itemSound;
    
    [Tooltip("Volumen del sonido (0-1)")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Tooltip("Duración de la animación del efecto")]
    public float animationDuration = 0.5f;
    
    [Tooltip("Delay antes de reproducir el efecto visual (en segundos)")]
    public float animationDelay = 0f;
    
    [Tooltip("Impact frame (pausa breve) al usar el item")]
    public bool useImpactFrame = false;
    
    [Tooltip("Duración del impact frame en segundos")]
    [Range(0f, 0.3f)]
    public float impactFrameDuration = 0.1f;

    protected FighterStats userStats;

    // Offsets para items (items siempre son auto-infligidos, la posición depende de quién lo usa)
    private readonly Vector3 effectOffsetPlayer = new Vector3(-4.5f, 1f, -5f);  // Jugador usando item
    private readonly Vector3 effectOffsetEnemy = new Vector3(4.5f, 1f, -5f);    // Enemigo usando item


    /// Reproduce el efecto visual del item
    private void PlayVisualEffect()
    {
        if (effectPrefab != null && userStats != null)
        {
            // Determinar qué offset usar según quién usa el item
            Vector3 selectedOffset = GetEffectOffset();
            
            // Instanciar el efecto
            GameObject effectInstance = Instantiate(effectPrefab, selectedOffset, Quaternion.identity);
            
            // Configurar sorting layers para todos los Particle System Renderers
            ParticleSystemRenderer[] renderers = effectInstance.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 100; // Orden alto para estar por encima de todo
            }
            
            // Destruir después de la duración de la animación
            Destroy(effectInstance, animationDuration);
            
            Debug.Log($"Efecto visual '{effectPrefab.name}' reproducido en posición {selectedOffset} para item {itemName}");
        }
    }

    /// Determina qué offset usar según quién usa el item
    private Vector3 GetEffectOffset()
    {
        // Verificar si el usuario es el jugador
        bool isPlayer = userStats != null && userStats.CompareTag("Player");

        if (isPlayer)
        {
            // Jugador usando item: efecto en posición del jugador
            Debug.Log($"Jugador usando item, usando effectOffsetPlayer");
            return effectOffsetPlayer;
        }
        else
        {
            // Enemigo usando item: efecto en posición del enemigo
            Debug.Log($"Enemigo usando item, usando effectOffsetEnemy");
            return effectOffsetEnemy;
        }
    }

    /// Reproduce el sonido del item
    private void PlaySound()
    {
        if (itemSound != null)
        {
            // Buscar o crear un AudioSource temporal
            AudioSource.PlayClipAtPoint(itemSound, Camera.main.transform.position, soundVolume);
            
            Debug.Log($"Sonido '{itemSound.name}' reproducido para item {itemName}");
        }
    }

    /// Crea un impact frame (pausa breve para enfatizar el uso del item)
    private IEnumerator ImpactFrame()
    {
        if (useImpactFrame && impactFrameDuration > 0)
        {
            // Pausar el tiempo
            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            
            // Esperar en tiempo real (no afectado por timeScale)
            yield return new WaitForSecondsRealtime(impactFrameDuration);
            
            // Restaurar el tiempo
            Time.timeScale = originalTimeScale;
            
            Debug.Log($"Impact frame de {impactFrameDuration}s aplicado al usar item");
        }
    }

    /// Establece quién usa el item
    public void SetUser(FighterStats user)
    {
        this.userStats = user;
    }

    public void Run()
    {
        Debug.Log($"ITEM RUN: {itemName} | User: {userStats?.fightername ?? "NULL"}");
        
        // Reproducir efectos audiovisuales
        StartCoroutine(ExecuteItemWithEffects());
    }

    /// Ejecuta el item con todos los efectos
    private IEnumerator ExecuteItemWithEffects()
    {
        // 1. Reproducir sonido al inicio
        PlaySound();
        
        // 2. Esperar el delay antes del efecto visual
        if (animationDelay > 0)
        {
            yield return new WaitForSeconds(animationDelay);
        }
        
        // 3. Reproducir efecto visual
        PlayVisualEffect();
        
        // 4. Ejecutar la lógica del item
        this.onRun();
        
        // 5. Aplicar impact frame después del efecto
        if (useImpactFrame)
        {
            yield return StartCoroutine(ImpactFrame());
        }
    }
    
    public abstract void onRun();
}
