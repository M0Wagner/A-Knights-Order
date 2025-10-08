using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlotData
{
    public string itemId;
    public int quantity;

    public InventorySlotData(string id, int qty)
    {
        itemId = id;
        quantity = qty;
    }
}

public class InventoryData : MonoBehaviour
{
    public static InventoryData instance;

    public int coins;

    public List<InventorySlotData> slots = new List<InventorySlotData>();
    private const int inventorySize = 10;

    public static event Action OnInventoryChanged;
    public static event Action<int> OnCoinCountChanged;

    private const string InventoryPrefsKey = "InventorySaveData";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInventory();
    }

    public void Start()
    {
        ResetInventory();
    }

    public void AddItem(string itemId, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.itemId == itemId)
            {
                slot.quantity += amount;
                SaveChangesAndNotifyUI();
                return;
            }
        }
        foreach (var slot in slots)
        {
            if (string.IsNullOrEmpty(slot.itemId))
            {
                slot.itemId = itemId;
                slot.quantity = amount;
                SaveChangesAndNotifyUI();
                return;
            }
        }
    }

    public void DecreaseItem(string itemId, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.itemId == itemId)
            {
                slot.quantity -= amount;
                if (slot.quantity <= 0)
                {
                    slot.quantity = 0;
                }
                SaveChangesAndNotifyUI();
                return;
            }
        }
    }

    public bool HasItem(string itemId, int quantity)
    {
        if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;

        // Check all slots to see if any stack has enough of the required item.
        foreach (var slot in slots)
        {
            if (slot.itemId == itemId && slot.quantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveChangesAndNotifyUI();
        OnCoinCountChanged?.Invoke(coins);
    }

    private void SaveChangesAndNotifyUI()
    {
        SaveInventory();
        OnInventoryChanged?.Invoke();
    }

    [System.Serializable]
    private class SaveDataWrapper {
        public int coinCount;
        public List<InventorySlotData> slotList;
    }

    public void SaveInventory()
    {
        var wrapper = new SaveDataWrapper {
            coinCount = this.coins,
            slotList = this.slots 
        };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(InventoryPrefsKey, json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey(InventoryPrefsKey))
        {
            string json = PlayerPrefs.GetString(InventoryPrefsKey);
            var wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);
            slots = wrapper.slotList;
            coins = wrapper.coinCount;
        }
        else
        {
            slots = new List<InventorySlotData>();
            for (int i = 0; i < inventorySize; i++)
            {
                slots.Add(new InventorySlotData("", 0));
            }
            slots[0] = new InventorySlotData("potion", 3);
            slots[1] = new InventorySlotData("key", 0);
            coins = 0;
        }

        while (slots.Count < inventorySize) slots.Add(new InventorySlotData("", 0));

        OnInventoryChanged?.Invoke();
        OnCoinCountChanged?.Invoke(coins);
    }
    
    public void ResetInventory()
    {
        Debug.LogWarning("INVENTORY RESET: Deleting PlayerPrefs save and reloading default items.");
        
        PlayerPrefs.DeleteKey(InventoryPrefsKey);
        PlayerPrefs.Save();

        slots.Clear();

        LoadInventory(); 
    }
}