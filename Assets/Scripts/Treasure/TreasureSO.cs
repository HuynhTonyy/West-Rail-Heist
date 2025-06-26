using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasureData", menuName = "West Rail Heist/Treasure Data")]
public class TreasureSO : ScriptableObject
{
    public string treasureName;
    public TreasurePriority priority;
    public GameObject treasureObj;
    public Sprite sprite;
    public int value;
    
}
public enum TreasurePriority
{
    Coin,
    MoneyBag,
    Diamond
}