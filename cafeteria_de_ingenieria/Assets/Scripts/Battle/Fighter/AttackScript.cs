using UnityEngine;
using System.Collections;

public class AttackScript : MonoBehaviour
{
    // define ataque actual y modificadores del player

    public GameObject owner;
    
    [Header("Melee Attack Visual & Audio")]
    [Tooltip("Prefab del efecto visual del ataque melee (partículas, animación, etc.)")]
    public GameObject meleeEffectPrefab;
    
    [Tooltip("Sonido del ataque melee")]
    public AudioClip meleeAttackSound;
    
    [Tooltip("Volumen del sonido (0-1)")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Tooltip("Duración de la animación del efecto")]
    public float animationDuration = 0.5f;
    
    [Tooltip("Delay antes de reproducir el efecto visual (en segundos)")]
    public float animationDelay = 0f;
    
    [Tooltip("Impact frame (pausa breve) al impactar")]
    public bool useImpactFrame = false;
    
    [Tooltip("Duración del impact frame en segundos")]
    [Range(0f, 0.3f)]
    public float impactFrameDuration = 0.1f;

    //[SerializeField]
    //private bool magicAttack; //debug para cachar si el magic attack funca .. deberia ser activado por default en habilidades!

    //[SerializeField]
    //private float magicCost;

    /*
    [SerializeField]
    private float minAttackMult = 0.8f;

    [SerializeField]
    private float maxAttackMult = 1.2f;

    [SerializeField]
    private float minDefenseMult = 0.9f;

    [SerializeField]
    private float maxDefenseMult = 1.1f;
    */

    private FighterStats attackerStats;
    private FighterStats targetStats;
    
    // Offsets para el efecto visual (similar a Skill.cs)
    private readonly Vector3 effectOffsetToEnemy = new Vector3(4.5f, 1f, -5f);
    private readonly Vector3 effectOffsetToPlayer = new Vector3(-4.5f, 1f, -5f);

    public void Attack(GameObject target)
    {
    	// obtener estadisticas de atacante y target desde el componente fighterstats de cada uno
    	attackerStats = owner.GetComponent<FighterStats>();
    	targetStats = target.GetComponent<FighterStats>();
    	
    	// Reproducir efectos visuales y de audio
    	StartCoroutine(ExecuteAttackWithEffects(target));

        Debug.Log("\t\t" + BattleConstants.MenuAttackOptions.Melee.ToString() + " attack made to " + target.tag);
    }
    
    /// <summary>
    /// Ejecuta el ataque melee con efectos visuales y de sonido
    /// </summary>
    private IEnumerator ExecuteAttackWithEffects(GameObject target)
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
        
        // 4. Ejecutar el daño del ataque
        targetStats.ReceiveDamageWithType(attackerStats, false); // false = ataque físico
        
        // 5. Aplicar impact frame después del impacto
        if (useImpactFrame)
        {
            yield return StartCoroutine(ImpactFrame());
        }
    }
    
    /// <summary>
    /// Reproduce el efecto visual del ataque melee
    /// </summary>
    private void PlayVisualEffect()
    {
        if (meleeEffectPrefab != null && targetStats != null)
        {
            // Determinar qué offset usar según quién ataca a quién
            Vector3 selectedOffset = GetEffectOffset();
            
            // Instanciar el efecto
            GameObject effectInstance = Instantiate(meleeEffectPrefab, selectedOffset, Quaternion.identity);
            
            // Configurar sorting layers para todos los Particle System Renderers
            ParticleSystemRenderer[] renderers = effectInstance.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 100; // Orden alto para estar por encima de todo
            }
            
            // Destruir después de la duración de la animación
            Destroy(effectInstance, animationDuration);
            
            Debug.Log($"🎨 Efecto visual melee '{meleeEffectPrefab.name}' reproducido en posición {selectedOffset}");
        }
    }
    
    /// <summary>
    /// Determina qué offset usar según el atacante
    /// </summary>
    private Vector3 GetEffectOffset()
    {
        // Verificar si el usuario es el jugador
        bool isPlayerAttacking = owner != null && owner.CompareTag("Player");

        if (isPlayerAttacking)
        {
            // Jugador atacando enemigo: usar offset hacia enemigo
            return effectOffsetToEnemy;
        }
        else
        {
            // Enemigo atacando jugador: usar offset hacia jugador
            return effectOffsetToPlayer;
        }
    }
    
    /// <summary>
    /// Reproduce el sonido del ataque melee
    /// </summary>
    private void PlaySound()
    {
        if (meleeAttackSound != null)
        {
            // Reproducir sonido en la posición de la cámara
            AudioSource.PlayClipAtPoint(meleeAttackSound, Camera.main.transform.position, soundVolume);
            
            Debug.Log($"🔊 Sonido melee '{meleeAttackSound.name}' reproducido");
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
            
            Debug.Log($"⏸️ Impact frame de {impactFrameDuration}s aplicado en ataque melee");
        }
    }

    public void UseSkillRandomly(GameObject target)
    {
        // obtener estadisticas de atacante y target desde el componente fighterstats de cada uno
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = target.GetComponent<FighterStats>();

        Skill[] attackSkills = attackerStats.GetSkills();

        // Si no hay skills, hacer ataque normal
        if (attackSkills.Length == 0)
        {
            Debug.LogWarning("No skills available, performing MELEE attack instead.");
            Attack(target);
            return;
        }

        // usar alguna skill aleatoria del atacante
        int skillIndex = Random.Range(0, attackSkills.Length);
        Skill skillToUse = attackSkills[skillIndex];

        UseSkill(skillToUse, target);
    }

    public void UseSkill(Skill skill, GameObject target)
    {
        // obtener estadisticas de atacante y target desde el componente fighterstats de cada uno
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = target.GetComponent<FighterStats>();

        // usar skill
        skill.SetTargetanduser(attackerStats, targetStats);
        skill.Run();
        
        Debug.Log("\t\tSkill: " + skill.skillName + " used on " + target.tag);
    }
}
