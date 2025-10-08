using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UIInventoryPage inventoryUI;
    [SerializeField] private UIPausePage pauseUI;

    public int inventorySize = 10;

    private void Start()
    {
        inventoryUI.InitializeVisualSlots(inventorySize);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false && pauseUI.isActiveAndEnabled == false)
            {
                Time.timeScale = 0f;
                inventoryUI.Show();
            }
            else
            {
                Time.timeScale = 1;
                inventoryUI.Hide();
            }
        }
    }
}
