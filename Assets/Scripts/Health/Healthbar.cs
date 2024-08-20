using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthba : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        // healthbar starts at full count
        totalHealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

    private void Update()
    {
        // refresh healthbar
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10;
    }
}
