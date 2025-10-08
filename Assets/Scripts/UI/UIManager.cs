using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text coinText; // Or TMP_Text

    private void Start()
    {
        if (InventoryData.instance != null)
        {
            UpdateCoinText(InventoryData.instance.coins);
        }
    }

    private void OnEnable()
    {
        InventoryData.OnCoinCountChanged += UpdateCoinText;
    }

    private void OnDisable()
    {
        InventoryData.OnCoinCountChanged -= UpdateCoinText;
    }

    private void UpdateCoinText(int newCoinCount)
    {
        if (coinText != null)
        {
            coinText.text = newCoinCount.ToString();
        }
    }
}