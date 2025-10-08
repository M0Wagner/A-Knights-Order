using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private UIPausePage pauseUI;
    [SerializeField] private UIInventoryPage inventoryUI;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseUI.isActiveAndEnabled == false && inventoryUI.isActiveAndEnabled == false)
            {
                Time.timeScale = 0f;
                pauseUI.Show();
            }
            else
            {
                Time.timeScale = 1;
                pauseUI.Hide();
            }
        }
    }
}
