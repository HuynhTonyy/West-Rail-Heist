using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class PlayerController : MonoBehaviour

{
    public string playerName;
    public int currentCardIndex = 0;
    // public Train currentTrain = Train.inside;

    public void Move(int direction)
    {
        // Implement movement logic here
        Debug.Log($"{playerName} moves {direction}");
    }

    public void Climb()
    {
       
    }

    public void Loot()
    {
        
    }

    public void Shoot()
    {
        // Basic version — implement targeting later
        Debug.Log($"{playerName} tried to shoot!");
    }

    public void Punch()
    {
        // Placeholder for punching logic
        Debug.Log($"{playerName} threw a punch!");
    }

}
