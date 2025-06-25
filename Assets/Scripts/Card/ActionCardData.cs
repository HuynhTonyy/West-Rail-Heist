using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionCardData", menuName = "West Rail Heist/ActionCard")]
public class ActionCardData : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    public ActionType actionType;
    [TextArea] public string description;
    
}
