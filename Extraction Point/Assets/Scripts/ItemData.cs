using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject {
    public int width = 1;
    public int height = 1;

    public int itemClass = 0; //loot type: 0 = default, 1 = food, 2 = weapon/ammo, 3 = etc and so on

    public float itemWeight = 0.0f; //weight of item in kg
    
    public bool isWeapon = false; //is the item a weapon
    public float fireRate = 0.0f; //if so then fire rate
    public int damage; //and damage 
    
    public bool isBackpack = false; // is the item a backpack
    public bool isArmor = false; // is the item an armor
    

    public Sprite itemIcon;
}