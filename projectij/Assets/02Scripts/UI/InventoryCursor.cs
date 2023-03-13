using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCursor : MonoBehaviour
{
    RectTransform rectTransform; 
    private int columnIndex = 0;
    private int rowIndex = 0;
    public int RowIndex
    {
        get { return rowIndex; }
        set { rowIndex = value; columnIndex = 0; SetCursorPosition(); }
    }
    public int ColumnIndex 
    {
        get { return columnIndex; } 
        set 
        { 
            columnIndex = value;
            if (columnIndex < 0) 
                columnIndex = 0; 
            if (rowIndex == 0 && columnIndex > 5)
                columnIndex = 5;
            else if (rowIndex == 1 && columnIndex > 11)
                columnIndex = 11;

            SetCursorPosition();
        } 
    }

    Transform InventoryTransform;
    public Transform currentTransform = null;

    private void Awake() {
        InventoryTransform = StageManager.Instance.weaponIconsTrans;
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    
    void OnEnable()
    {
        if (InventoryTransform == null)
            InventoryTransform = StageManager.Instance.weaponIconsTrans;
        currentTransform = InventoryTransform.GetChild(0).GetChild(0);
        SetCursorPosition();
    }

    public void SetCursorPosition()
    {
        if (rowIndex == 0)
        {
            currentTransform = InventoryTransform.GetChild(columnIndex / 2).GetChild(columnIndex % 2);
            rectTransform.position = currentTransform.position;
        }
        else
        {
            currentTransform = InventoryTransform.GetChild(3).GetChild(columnIndex);
            rectTransform.position = currentTransform.position;
        }
        currentTransform.GetComponent<InventorySlot>().SetWeaponInfoWindow();
    }
}
