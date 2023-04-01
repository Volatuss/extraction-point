using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : MonoBehaviour
{
    
    public static ItemData primaryData = null, secondaryData = null, armorData = null, backpackData = null;

    public static void UpdatePrimary(ItemData itemData){
        primaryData = itemData;
    }
    public static void UpdateSecondary(ItemData itemData){
        secondaryData = itemData;
    }
    public static void UpdateArmor(ItemData itemData){
        armorData = itemData;
    }
    public static void UpdateBackpack(ItemData itemData){
        backpackData = itemData;
    }

    public static int GetRPM(int slot){
       if(slot == 1)
       { 
            return primaryData.weapon.rpm; 
       }
       else if(slot == 2)
       {
            return secondaryData.weapon.rpm;
       }
       return 0;
    }

    public static int GetMagSize(int slot){
       if(slot == 1)
       { 
            return primaryData.weapon.magSize; 
       }
       else if(slot == 2)
       {
            return secondaryData.weapon.magSize;
       }
       return 0;
    }

    public static Vector3 GetEjectPos(int slot){
        if(slot == 1)
        { 
            return primaryData.weapon.shellEjectPos; 
        }
        else if(slot == 2)
        {
            return secondaryData.weapon.shellEjectPos;
        }
        return Vector3.zero;
    }

    public static Vector3 GetBarrelPos(int slot){
        if(slot == 1)
        { 
            return primaryData.weapon.barrelTipPos; 
        }
        else if(slot == 2)
        {
            return secondaryData.weapon.barrelTipPos;
        }
        return Vector3.zero;
    }

    public static Vector3 GetPivotPos(int slot){
        if(slot == 1)
        { 
            return primaryData.weapon.pivotPos; 
        }
        else if(slot == 2)
        {
            return secondaryData.weapon.pivotPos;
        }
        return Vector3.zero;
    }

    public static Sprite GetEquipSprite( int slot ){
        if(slot == 1)
        { 
            return primaryData.itemIcon; 
        }
        else if(slot == 2)
        {
            return secondaryData.itemIcon;
        }
        return null;
    }



}
