using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Object/data")]
public class ObjectData : ScriptableObject
{
    public Sprite sprite;
    public GameObject groundItem;
    public string itemName;
    public int quantity;
    public float AttackDamage;
    public weaponType weaponType;    
}

public enum weaponType
{
    Rifle,
    Pistol,
    Grenade
}
