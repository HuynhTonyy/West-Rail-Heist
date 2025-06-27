using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Carriage
{
    public TopCarriage topCarriage = new TopCarriage();
    public BottomCarriage bottomCarriage = new BottomCarriage();
}

[System.Serializable]
public class TopCarriage
{
    public GameObject obj;
    public int width;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public List<TreasureSO> treasures = new List<TreasureSO>();


    public void CalculateWidth()
    {
        if (obj != null && obj.TryGetComponent(out BoxCollider2D collider))
        {
            width = (int)collider.size.x;
        }
    }
    public List<GameObject> GetPlayers()
    {
        return new List<GameObject>(players.Values);
    }

    public int MaxPlayers => width / 2;

    public bool AddPlayer(GameObject player)
    {
        if (players.Count >= MaxPlayers) return false;

        int slotIndex = GetNextAvailableSlot();
        if (slotIndex == -1) return false;

        players.Add(slotIndex, player);
        Vector3 pos = CalculatePlayerPosition(slotIndex);
        player.transform.position = pos;

        // Debug.Log($"Player added to TopCarriage at slot {slotIndex}, position {pos}");
        return true;
    }
    public bool RemovePlayer(GameObject player)
    {
        int keyToRemove = -1;

        foreach (var pair in players)
        {
            if (pair.Value == player)
            {
                keyToRemove = pair.Key;
                break;
            }
        }

        if (keyToRemove != -1)
        {
            players.Remove(keyToRemove);
            // Debug.Log($"Player removed from TopCarriage slot {keyToRemove}");
            return true;
        }

        return false;
    }
    public void AddTreasure(TreasureSO treasure, GameObject newTreasure)
    {
        treasures.Add(treasure);
        float x = Utility.GetRandom(obj.transform.position.x, obj.transform.position.x + width);
        Vector3 pos = new Vector3(x, obj.transform.position.y, 0);
        newTreasure.transform.position = pos;
    }
    private int GetNextAvailableSlot()
    {
        for (int i = 0; i < MaxPlayers; i++)
        {
            if (!players.ContainsKey(i)) return i;
        }
        return -1;
    }

    private Vector3 CalculatePlayerPosition(int slotIndex)
    {
        float startX = obj.transform.position.x + 1f; // 1f centers 2-unit wide slot
        float x = startX + slotIndex * 2f;
        float y = obj.transform.position.y;
        return new Vector3(x, y, 0);
    }
}

[System.Serializable]
public class BottomCarriage
{
    public GameObject obj;
    public int width;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public List<TreasureSO> treasures = new List<TreasureSO>();

    public void CalculateWidth()
    {
        if (obj != null && obj.TryGetComponent(out BoxCollider2D collider))
        {
            width = (int)collider.size.x;
        }
    }
    public List<GameObject> GetPlayers()
    {
        return new List<GameObject>(players.Values);
    }

    public int MaxPlayers => width / 2;

    public bool AddPlayer(GameObject player)
    {
        if (players.Count >= MaxPlayers) return false;

        int slotIndex = GetNextAvailableSlot();
        if (slotIndex == -1) return false;

        players.Add(slotIndex, player);
        Vector3 pos = CalculatePlayerPosition(slotIndex);
        player.transform.position = pos;

        // Debug.Log($"Player added to BottomCarriage at slot {slotIndex}, position {pos}");
        return true;
    }
    public bool RemovePlayer(GameObject player)
    {
        int keyToRemove = -1;

        foreach (var pair in players)
        {
            if (pair.Value == player)
            {
                keyToRemove = pair.Key;
                break;
            }
        }
        if (keyToRemove != -1)
        {
            players.Remove(keyToRemove);
            // Debug.Log($"Player removed from TopCarriage slot {keyToRemove}");
            return true;
        }
        return false;
    }
    public void AddTreasure(TreasureSO treasure, GameObject newTreasure)
    {
        treasures.Add(treasure);
        float x = Utility.GetRandom(obj.transform.position.x, obj.transform.position.x + width);
        Vector3 pos = new Vector3(x, obj.transform.position.y, 0);
        newTreasure.transform.position = pos;
    }
    private int GetNextAvailableSlot()
    {
        for (int i = 0; i < MaxPlayers; i++)
        {
            if (!players.ContainsKey(i)) return i;
        }
        return -1;
    }

    private Vector3 CalculatePlayerPosition(int slotIndex)
    {
        // Assuming center of carriage is at obj.transform.position.x
        float startX = obj.transform.position.x + 1f; // +1f to center player in 2-unit slot
        float x = startX + slotIndex * 2f;
        Vector3 pos = new Vector3(x, obj.transform.position.y, 0);
        return pos;
    }
}

