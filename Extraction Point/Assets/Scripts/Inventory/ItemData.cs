using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject {

    public int itemID;
    public int width = 1, height = 1, maxStackSize = 1, currentStackSize = 1, interactValue = 1;
    public float itemWeight = 0.0f; 
    public string itemName;
    public bool isWeapon = false, isBackpack = false, isArmor = false, unbox = false, med = false; 
    public WeaponScriptable weapon = null;
    public ItemData pairedItem;
    public Sprite itemIcon;
}