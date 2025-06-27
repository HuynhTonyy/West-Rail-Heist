using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public static GameManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI currentPlayerText, gameLogText;
    [SerializeField] private List<Carriage> carriages = new List<Carriage>();
    [SerializeField] private List<Treasure> treasures = new List<Treasure>();

    [SerializeField] private int numberOfPlayer;
    private List<PlayerController> allPlayers = new List<PlayerController>();
    private int currentPlayerIndex = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        carriages.Clear();

        // Find all bottom carriages
        GameObject[] bottomObjects = GameObject.FindGameObjectsWithTag("Bottom Carriage");
        GameObject[] topObjects = GameObject.FindGameObjectsWithTag("Top Carriage");

        bottomObjects = bottomObjects.OrderBy(obj => obj.transform.position.x).ToArray();
        topObjects = topObjects.OrderBy(obj => obj.transform.position.x).ToArray();

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
        for (int i = 0; i < topObjects.Length; i++)
        {
            if (i < carriages.Count)
            {
                carriages[i].topCarriage = new TopCarriage { obj = topObjects[i] };
                carriages[i].topCarriage.CalculateWidth();
            }
        }

    }
    void Start()
    {
        SpawnPlayers();
        SpawnTreasures();
    }

    public void LogAction(string message)
    {
        if (gameLogText != null)
            gameLogText.text = message;
    }

    private PlayerController currentPlayer;

    public void SetCurrentPlayer(PlayerController player)
    {
        if (currentPlayer != null)
            currentPlayer.CheckTurn(false); // Disable previous

        currentPlayer = player;
        currentPlayerText.text = player.PlayerName;
        currentPlayer.CheckTurn(true); // Enable new
    }

    public void MoveToNextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % allPlayers.Count;
        SetCurrentPlayer(allPlayers[currentPlayerIndex]);
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public List<PlayerController> GetAllPlayers()
    {
        return allPlayers;
    }

    private void SpawnPlayers()
    {
        int half = numberOfPlayer / 2;

        for (int i = 0; i < numberOfPlayer; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab);
            PlayerController player = playerObj.GetComponent<PlayerController>();
            player.SetPlayerId(i + 1);

            // Find CardHandManager inside the child hierarchy
            CardHandManager handManager = playerObj.GetComponentInChildren<CardHandManager>(true);
            player.SetCardHandManager(handManager); // Set it in PlayerController
            handManager.InitializeHand();

            allPlayers.Add(player);
            foreach (var person in allPlayers)
            {
                person.CheckTurn(false); // Ensure all canvases are hidden
            }


            if (i < half)
                carriages[0].topCarriage.AddPlayer(playerObj);
            else
                carriages[0].bottomCarriage.AddPlayer(playerObj);
        }

        SetCurrentPlayer(allPlayers[0]);
    }
    private void SpawnTreasures()
    {
        foreach (var treasure in treasures)
        {
            for (int i = 0; i < treasure.amount; i++)
            {
                int carriageIndex = Utility.GetRandom(1, carriages.Count);
                GameObject newTreasure = Instantiate(treasure.treasureSO.treasureObj);
                carriages[carriageIndex].bottomCarriage.AddTreasure(treasure.treasureSO,newTreasure);
            }
        }
    }

}






