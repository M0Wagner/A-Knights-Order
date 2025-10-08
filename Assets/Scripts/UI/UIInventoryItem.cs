using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked, OnRightMouseBtnClick;

    private int currentQuantity;
    private Sprite currentIcon;

    // The IsEmpty state is now derived directly from the quantity. No more separate 'empty' bool!
    public bool IsEmpty => currentQuantity <= 0;
    public int Quantity => currentQuantity;

    private void Awake()
    {
        Deselect();
        ResetData();
    }

    public void ResetData()
    {
        currentQuantity = 0;
        currentIcon = null;
        UpdateVisuals();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        currentIcon = sprite;
        currentQuantity = Mathf.Max(0, quantity);
        UpdateVisuals();
    }

    public void IncreaseQuantity(int amount = 1)
    {
        currentQuantity += Mathf.Abs(amount);
        UpdateVisuals();
    }

    public void DecreaseQuantity(int amount = 1)
    {
        if (currentQuantity <= 0) return;
        currentQuantity = Mathf.Max(0, currentQuantity - Mathf.Abs(amount));
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (itemImage != null)
        {
            bool hasIcon = currentIcon != null;
            itemImage.gameObject.SetActive(hasIcon);
            itemImage.sprite = currentIcon;

            itemImage.color = IsEmpty ? new Color(1, 1, 1, 0.5f) : Color.white;
        }

        if (quantityText != null)
        {
            quantityText.text = currentQuantity.ToString();
        }
    }

    public void Select() => borderImage.enabled = true;
    public void Deselect() => borderImage.enabled = false;
    public void SimulateLeftClick() => OnItemClicked?.Invoke(this);

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (IsEmpty) return;

        if (pointerData.button == PointerEventData.InputButton.Right)
            OnRightMouseBtnClick?.Invoke(this);
        else
            OnItemClicked?.Invoke(this);
    }
}