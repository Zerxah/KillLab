using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSfx : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private AudioClip hoverClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.8f;

    private AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHover();
    }

    void PlayHover()
    {
        if (!hoverClip) return;
        src.PlayOneShot(hoverClip, volume);
    }
}

