using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public enum ActionType
{
    Move,
    // Move to new train carriage.
    // - If a player is inside a train, they have to move to a near by carriage.
    // - If a player is on the roof, they can move from 1 - 2 nearest carriage.
    Climb,
    //Player will either climb to the roof or inside their current carriage which they are on.
    Punch,
    // Choose a person in the same carriage to punch.
    // They will drop a "Money bag" of your choice.
    // Then choose a near by carriage for them to be in.
    Shoot,
    // This card only usable when the player have bullets.
    // Player will shoot another player:
    // - When inside the carriage, you can only shoot the person in the next carriage. Can't shoot the person in the same carriage.
    // - When on the roof, shoot the nearest person you can see despite distance. If there are more then one person, choose one to shoot
    Loot,
    //Get a "Money bag" where your character is standing.
    // Characters in the carriage cannot get Money Bags on the roof and vice versa.
    Marshal
    // Move the Sheriff by 1 carriage.
    // When the Sheriff enters a car with a Robber, or vice versa. 
    // The Robber must escape to the roof of that car and each Robber seen will receive 1 Bullet (Neutral).

}
