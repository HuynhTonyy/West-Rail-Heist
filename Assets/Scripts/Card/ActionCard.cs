using UnityEngine;

public class ActionCard
{
    public ActionType actionType;
    public PlayerController owner;
    public int moveDirection = 0; // -1 for left, +1 for right (only used for Move)

    public ActionCard(ActionType type, PlayerController player, int direction = 0)
    {
        actionType = type;
        owner = player;
        moveDirection = direction;
    }

    // public void Execute(GameManager gameManager)
    // {
    //     switch (actionType)
    //     {
    //         case ActionType.Move:
    //             owner.Move(moveDirection);
    //             break;
    //         case ActionType.Climb:
    //             owner.Climb();
    //             break;
    //         case ActionType.Loot:
    //             owner.Loot();
    //             break;
    //         case ActionType.Shoot:
    //             owner.Shoot();
    //             break;
    //         case ActionType.Punch:
    //             owner.Punch();
    //             break;
    //         case ActionType.Marshal:
    //             gameManager.Marshal();
    //             break;
    //     }
    // }
}
