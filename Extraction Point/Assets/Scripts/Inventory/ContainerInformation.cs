using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInformation : MonoBehaviour
{
    public Dictionary<Vector2, ItemData> containerContents{get; set;} 
    public string ContainerType { get; set; } //medical, weapon, ammo, ground, corpse, etc
}
