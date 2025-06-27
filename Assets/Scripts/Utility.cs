using System.Collections.Generic;
using UnityEngine;
public static class Utility
{
    public static int GetRandom(int from, int to)
    {
        return Random.Range(from, to);
    }
    public static float GetRandom(float from, float to)
    {
        return Random.Range(from, to);
    }
    public static List<GameObject> GetPlayersAt(int carriageIndex, bool isTop)
    {
        var carriages = GameManager.Instance.GetCarriages();

        if (carriageIndex < 0 || carriageIndex >= carriages.Count)
        {
            Debug.LogWarning($"Invalid carriage index: {carriageIndex}");
            return new List<GameObject>();
        }

        return isTop
            ? carriages[carriageIndex].topCarriage.GetPlayers()
            : carriages[carriageIndex].bottomCarriage.GetPlayers();
    }

    public static List<Carriage> GetNearbyCarriages(int currentIndex, int range)
    {
        var carriages = GameManager.Instance.GetCarriages();
        List<Carriage> nearbyCarriages = new();

        for (int offset = 1; offset <= range; offset++)
        {
            int left = currentIndex - offset;
            int right = currentIndex + offset;

            if (left >= 0)
                nearbyCarriages.Add(carriages[left]);

            if (right < carriages.Count)
                nearbyCarriages.Add(carriages[right]);
        }

        return nearbyCarriages;
    }

    public static void MovePlayerToCarriage(PlayerController player, Carriage destination)
    {
        var current = player.CurrentCarriage;

        // Remove from current carriage
        if (player.IsOnTop)
            current.topCarriage.RemovePlayer(player.gameObject);
        else
            current.bottomCarriage.RemovePlayer(player.gameObject);

        // Add to destination carriage
        if (player.IsOnTop)
            destination.topCarriage.AddPlayer(player.gameObject);
        else
            destination.bottomCarriage.AddPlayer(player.gameObject);

        // Update the player's internal position
        player.SetPosition(destination, player.IsOnTop);
    }


}
