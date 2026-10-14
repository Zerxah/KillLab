using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler
{
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip) AudioManager.Instance.PlaySFX(hoverClip);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (hoverClip) AudioManager.Instance.PlaySFX(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip) AudioManager.Instance.PlaySFX(clickClip);
    }
}
