using UnityEngine;
using System.Collections;

public abstract class Skill : MonoBehaviour
{
    [Header("Base Skill")]
    public string skillName;
    public string description;
    public float animationDuration = 0.5f;
    
    [Tooltip("Delay antes de reproducir el efecto visual (en segundos)")]
    public float animationDelay = 0f;
    
    public ElementType type;
    public Sprite icon;

    public bool selfinflicted;

    [Header("Visual & Audio Effects")]
    [Tooltip("Prefab del efecto visual (partículas, animación, etc.)")]
    public GameObject effectPrefab;
    
    [Tooltip("Sonido que se reproduce al usar la habilidad")]
    public AudioClip skillSound;
    
    [Tooltip("Volumen del sonido (0-1)")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Tooltip("Impact frame (pausa breve) al impactar")]
    public bool useImpactFrame = false;
    
    [Tooltip("Duración del impact frame en segundos")]
    [Range(0f, 0.3f)]
    public float impactFrameDuration = 0.1f;

    protected FighterStats userStats;
    protected FighterStats targetStats;

    // Offsets fijos definidos en el código (Z negativo para que esté delante)
    private readonly Vector3 effectOffsetToEnemy = new Vector3(4.5f, 1f, -5f);
    private readonly Vector3 effectOffsetToPlayer = new Vector3(-4.5f, 1f, -5f);

    /// <summary>
    /// Reproduce el efecto visual de la habilidad
    /// </summary>
    private void PlayVisualEffect()
    {
        if (effectPrefab != null && targetStats != null)
        {
            // Determinar qué offset usar según quién ataca a quién
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
            
            Debug.Log($"🎨 Efecto visual '{effectPrefab.name}' reproducido en posición {selectedOffset} para {skillName} (Z={selectedOffset.z})");
        }
    }

    /// <summary>
    /// Determina qué offset usar según el atacante y el objetivo
    /// </summary>
    private Vector3 GetEffectOffset()
    {
        // Si es auto-infligida, usar offset del jugador
        if (selfinflicted)
        {
            Debug.Log($"📍 Habilidad auto-infligida, usando effectOffsetToPlayer");
            return effectOffsetToPlayer;
        }

        // Verificar si el usuario es el jugador
        bool isPlayerAttacking = userStats != null && userStats.CompareTag("Player");

        if (isPlayerAttacking)
        {
            // Jugador atacando enemigo: usar offset hacia enemigo
            Debug.Log($"📍 Jugador atacando enemigo, usando effectOffsetToEnemy");
            return effectOffsetToEnemy;
        }
        else
        {
            // Enemigo atacando jugador: usar offset hacia jugador
            Debug.Log($"📍 Enemigo atacando jugador, usando effectOffsetToPlayer");
            return effectOffsetToPlayer;
        }
    }

    /// <summary>
    /// Reproduce el sonido de la habilidad
    /// </summary>
    private void PlaySound()
    {
        if (skillSound != null)
        {
            // Buscar o crear un AudioSource temporal
            AudioSource.PlayClipAtPoint(skillSound, Camera.main.transform.position, soundVolume);
            
            Debug.Log($"🔊 Sonido '{skillSound.name}' reproducido para {skillName}");
        }
    }

    /// <summary>
    /// Crea un impact frame (pausa breve para enfatizar el impacto)
    /// </summary>
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
            
            Debug.Log($"⏸️ Impact frame de {impactFrameDuration}s aplicado");
        }
    }

    //private void Animate()
    //{
    //    var go = Instantiate(this.effectPrfb, this.targetStats.transform.position, Quaternion.identity);
    //    Destroy(go, this.animationDuration);
    //}

    public void Run()
    {
        Debug.Log($"🎯 SKILL RUN: {skillName} | User: {userStats?.fightername ?? "NULL"} | Target: {targetStats?.fightername ?? "NULL"} | SelfInflicted: {selfinflicted}");
        
        if (this.selfinflicted)
        {
            this.targetStats = this.userStats;
            Debug.Log($"   ↳ SelfInflicted activado, target cambiado a: {targetStats?.fightername}");
        }

        // Reproducir efectos audiovisuales
        StartCoroutine(ExecuteSkillWithEffects());
    }

    /// <summary>
    /// Ejecuta la habilidad con todos los efectos
    /// </summary>
    private IEnumerator ExecuteSkillWithEffects()
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
        
        // 4. Ejecutar la lógica de la habilidad
        this.onRun();
        
        // 5. Aplicar impact frame después del impacto
        if (useImpactFrame)
        {
            yield return StartCoroutine(ImpactFrame());
        }
    }

    public void SetTargetanduser(FighterStats _user, FighterStats _target)
    {
        this.userStats = _user;
        this.targetStats = _target;
    }

    public abstract void onRun();
}
