using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Componente que agrega sonidos automáticamente a un botón de Unity UI
/// Añádelo a cualquier GameObject con un componente Button
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Tipo de Botón")]
    [Tooltip("Tipo de sonido que reproducirá este botón")]
    public ButtonSoundType soundType = ButtonSoundType.Normal;
    
    [Header("Configuración")]
    [Tooltip("Reproducir sonido al pasar el mouse")]
    public bool playHoverSound = true;
    
    [Tooltip("Reproducir sonido al hacer clic")]
    public bool playClickSound = true;
    
    [Header("Sonidos Personalizados (Opcional)")]
    [Tooltip("Si se asigna, sobreescribe el sonido de clic del tipo")]
    public AudioClip customClickSound;
    
    [Tooltip("Si se asigna, sobreescribe el sonido de hover")]
    public AudioClip customHoverSound;
    
    private Button button;
    
    public enum ButtonSoundType
    {
        Normal,      // Botón normal
        Confirm,     // Comenzar, Confirmar, Seleccionar
        Cancel,      // Cancelar, Volver, Salir
        Skill        // Botones de habilidades
    }
    
    void Awake()
    {
        button = GetComponent<Button>();
    }
    
    /// <summary>
    /// Llamado cuando el mouse entra en el área del botón
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || !button.interactable) return;
        
        if (UIAudioManager.Instance != null)
        {
            if (customHoverSound != null)
            {
                UIAudioManager.Instance.PlaySound(customHoverSound);
            }
            else
            {
                UIAudioManager.Instance.PlayHoverSound();
            }
        }
    }
    
    /// <summary>
    /// Llamado cuando se hace clic en el botón
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playClickSound || !button.interactable) return;
        
        if (UIAudioManager.Instance != null)
        {
            if (customClickSound != null)
            {
                UIAudioManager.Instance.PlaySound(customClickSound);
            }
            else
            {
                switch (soundType)
                {
                    case ButtonSoundType.Normal:
                    case ButtonSoundType.Skill:
                        UIAudioManager.Instance.PlayClickSound();
                        break;
                    case ButtonSoundType.Confirm:
                        UIAudioManager.Instance.PlayConfirmSound();
                        break;
                    case ButtonSoundType.Cancel:
                        UIAudioManager.Instance.PlayCancelSound();
                        break;
                }
            }
        }
    }
    
    /// <summary>
    /// Método público para reproducir el sonido de clic manualmente
    /// Útil cuando el botón se activa por código
    /// </summary>
    public void PlayClickSoundManually()
    {
        OnPointerClick(null);
    }
}
