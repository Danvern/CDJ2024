using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class ImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private UnityEvent onClick;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        if (normalSprite != null)
        {
            image.sprite = normalSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            image.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null)
        {
            image.sprite = normalSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    public void SetNormalSprite(Sprite sprite)
    {
        normalSprite = sprite;
        image.sprite = normalSprite;
    }

    public void SetHoverSprite(Sprite sprite)
    {
        hoverSprite = sprite;
    }

    public void AddOnClickListener(UnityAction action)
    {
        onClick.AddListener(action);
    }

    public void RemoveOnClickListener(UnityAction action)
    {
        onClick.RemoveListener(action);
    }
}
