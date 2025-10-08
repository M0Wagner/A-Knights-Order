using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private UIInventoryDescription itemDescription;
    [SerializeField] private Health health;

    [System.Serializable]
    public class ItemCatalogEntry
    {
        public string itemId;
        public Sprite icon;
        public string title;
        public string description;
    }
    [SerializeField] private List<ItemCatalogEntry> itemCatalog;

    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [SerializeField] private KeyCode useItemKey = KeyCode.E;
    [SerializeField] private bool wrapSelection = true;
    [SerializeField] private bool skipEmptySlots = false;

    private readonly List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
    private int selectedIndex = -1;
    private const string MoveLeftKeyPref = "MoveLeftKey";
    private const string MoveRightKeyPref = "MoveRightKey";
    
    private Coroutine setupCoroutine;

    private void Awake()
    {
        InitializeVisualSlots(InventoryData.instance.slots.Count);
        
        if (itemDescription != null) itemDescription.ResetDescription();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryData.OnInventoryChanged += RefreshDisplay;
        LoadNavKeysFromPrefs();
    }

    private void OnDisable()
    {
        InventoryData.OnInventoryChanged -= RefreshDisplay;
    }

    private void Update()
    {
        if (!isActiveAndEnabled || listOfUIItems.Count == 0) return;

        if (Input.GetKeyDown(moveRightKey)) MoveSelection(+1);
        else if (Input.GetKeyDown(moveLeftKey)) MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(useItemKey))
        {
            UseSelectedItem();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        
        if (setupCoroutine != null)
        {
            StopCoroutine(setupCoroutine);
        }
        setupCoroutine = StartCoroutine(ShowAndSetupInventory());
    }
    
    private IEnumerator ShowAndSetupInventory()
    {
        // Wait for the end of the current frame. This gives the UI system time to initialize
        // all the components after the GameObject was activated.
        yield return null; 

        RefreshDisplay();
        AutoSelectOnOpen();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void InitializeVisualSlots(int inventorySize)
    {
        foreach (Transform child in contentPanel) Destroy(child.gameObject);
        listOfUIItems.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem uiItem = Instantiate(itemPrefab, contentPanel);
            uiItem.OnItemClicked += HandleItemSelection;
            listOfUIItems.Add(uiItem);
        }
    }

    public void RefreshDisplay()
    {
        for (int i = 0; i < InventoryData.instance.slots.Count; i++)
        {
            if (i >= listOfUIItems.Count) break;
            
            var slotData = InventoryData.instance.slots[i];
            var uiSlot = listOfUIItems[i];
            ItemCatalogEntry catalogEntry = itemCatalog.Find(entry => entry.itemId == slotData.itemId);

            if (catalogEntry != null)
            {
                uiSlot.SetData(catalogEntry.icon, slotData.quantity);
            }
            else
            {
                uiSlot.ResetData();
            }
        }
        
        if (selectedIndex != -1) UpdateDescriptionForIndex(selectedIndex);
    }

    private void UseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= InventoryData.instance.slots.Count) return;
        
        var selectedSlotData = InventoryData.instance.slots[selectedIndex];
        
        if (selectedSlotData.itemId == "potion" && selectedSlotData.quantity > 0)
        {
            if (health.currentHealth < health.StartingHealth)
            {
                InventoryData.instance.DecreaseItem("potion", 1);
                health.HealDamage(1);
            }
        }
    }

    private void HandleItemSelection(UIInventoryItem clickedItem)
    {
        int index = listOfUIItems.IndexOf(clickedItem);
        SelectIndex(index);
    }

    private void SelectIndex(int index)
    {
        if (index < 0 || index >= listOfUIItems.Count) return;

        selectedIndex = index;
        
        for (int i = 0; i < listOfUIItems.Count; i++)
        {
            if (i == selectedIndex) listOfUIItems[i].Select();
            else listOfUIItems[i].Deselect();
        }

        UpdateDescriptionForIndex(index);
    }

    private void AutoSelectOnOpen()
    {
        int firstIndexToSelect = 0;
        if (skipEmptySlots)
        {
            firstIndexToSelect = InventoryData.instance.slots.FindIndex(slot => !string.IsNullOrEmpty(slot.itemId) && slot.quantity > 0);
            if (firstIndexToSelect == -1) firstIndexToSelect = 0;
        }
        SelectIndex(firstIndexToSelect);
    }
    
    private void UpdateDescriptionForIndex(int index)
    {
        if (index < 0 || index >= InventoryData.instance.slots.Count)
        {
            itemDescription.ResetDescription();
            return;
        }

        var slotData = InventoryData.instance.slots[index];
        ItemCatalogEntry catalogEntry = itemCatalog.Find(entry => entry.itemId == slotData.itemId);

        if (catalogEntry != null)
        {
            itemDescription.SetDescription(catalogEntry.icon, catalogEntry.title, catalogEntry.description);
        }
        else
        {
            itemDescription.ResetDescription();
        }
    }

    private void MoveSelection(int delta)
    {
        if (selectedIndex == -1)
        {
            AutoSelectOnOpen();
            return;
        }

        int count = listOfUIItems.Count;
        int newIndex = selectedIndex;
        
        for (int i = 0; i < count; i++)
        {
            newIndex += delta;

            if (wrapSelection)
            {
                if (newIndex < 0) newIndex = count - 1;
                else if (newIndex >= count) newIndex = 0;
            }
            else
            {
                newIndex = Mathf.Clamp(newIndex, 0, count - 1);
            }

            if (!skipEmptySlots || (InventoryData.instance.slots[newIndex].quantity > 0))
            {
                if (newIndex != selectedIndex) break;
            }
            
            if (!wrapSelection && (newIndex == 0 || newIndex == count - 1) && newIndex == selectedIndex) break;
        }
        
        SelectIndex(newIndex);
    }

    private void LoadNavKeysFromPrefs()
    {
        var leftStr = PlayerPrefs.GetString(MoveLeftKeyPref, moveLeftKey.ToString());
        var rightStr = PlayerPrefs.GetString(MoveRightKeyPref, moveRightKey.ToString());
        if (Enum.TryParse(leftStr, true, out KeyCode left)) moveLeftKey = left;
        if (Enum.TryParse(rightStr, true, out KeyCode right)) moveRightKey = right;
    }
}