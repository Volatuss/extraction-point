using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponScriptable : ScriptableObject
{
    public int itemID, rpm, magSize;
    public float accuracy; //0-1 accuracy for weapon bloom
    public Vector2 shellEjectPos, barrelTipPos, pivotPos;
}
