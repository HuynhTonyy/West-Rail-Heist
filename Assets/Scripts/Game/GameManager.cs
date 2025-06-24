using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    [SerializeField] private List<Carriage> carriages = new List<Carriage>();
    [SerializeField] private int numberOfPlayer;

    private void Awake()
    {
        carriages.Clear();

        // Find all bottom carriages
        GameObject[] bottomObjects = GameObject.FindGameObjectsWithTag("Bottom Carriage");
        foreach (GameObject obj in bottomObjects)
        {
            Carriage carriage = new Carriage
            {
                bottomCarriage = new BottomCarriage { obj = obj }
            };
            carriage.bottomCarriage.CalculateWidth();
            carriages.Add(carriage);
        }

        // Optionally add top carriages if needed
        GameObject[] topObjects = GameObject.FindGameObjectsWithTag("Top Carriage");
        for (int i = 0; i < topObjects.Length; i++)
        {
            if (i < carriages.Count)
            {
                carriages[i].topCarriage = new TopCarriage { obj = topObjects[i] };
                carriages[i].topCarriage.CalculateWidth();
            }
        }

        Debug.Log($"Loaded {carriages.Count} carriages from scene.");
    }
    void Start()
    {
        // Example: Add 3 players to the first carriage top and bottom
        for (int i = 0; i < numberOfPlayer; i++)
        {
            // GameObject topPlayer = Instantiate(playerPrefab);
            GameObject bottomPlayer = Instantiate(playerPrefab);

            // carriages[carriages.Count-1].topCarriage.AddPlayer(topPlayer);
            carriages[carriages.Count-1].bottomCarriage.AddPlayer(bottomPlayer);
        }
    }
    private void Update()
    {
    }
}






