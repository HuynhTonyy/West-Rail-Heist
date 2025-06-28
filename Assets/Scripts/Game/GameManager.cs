using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private List<Treasure> treasures = new List<Treasure>();


    [Header("Player Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject marshalPrefab;
    [SerializeField] private int numberOfPlayer;
    private List<PlayerController> allPlayers = new();

    [Header("Round Management")]
    private int currentRound = 1;
    private int currentPlayerIndex = 0;
    private int currentTurnInRound = 0;
    private int roundStartingPlayerIndex = 0;
    private PlayerController currentPlayer;
    private List<PlayerController> roundPlayerOrder;
    private PlayerController marshal;
    public PlayerController GetMarshal() => marshal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentRoundText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private TextMeshProUGUI gameLogText;

    [Header("Train")]

    [SerializeField] private List<Carriage> carriages = new();


    public List<Carriage> GetCarriages() => carriages;

    public int GetCarriageIndex(Carriage carriage)
    {
        return carriages.IndexOf(carriage);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeCarriages();
    }

    private void Start()
    {
        SpawnPlayers();
        SpawnMarshal();
        SpawnTreasures();
        StartNewRound();
    }

    // ------------------------- Initialization -------------------------

    private void InitializeCarriages()
    {
        carriages.Clear();

        var bottomObjects = GameObject.FindGameObjectsWithTag("Bottom Carriage")
                                      .OrderBy(obj => obj.transform.position.x).ToArray();

        var topObjects = GameObject.FindGameObjectsWithTag("Top Carriage")
                                   .OrderBy(obj => obj.transform.position.x).ToArray();

        for (int i = 0; i < bottomObjects.Length; i++)
        {
            Carriage carriage = new Carriage
            {
                bottomCarriage = new BottomCarriage { obj = bottomObjects[i] }
            };
            carriage.bottomCarriage.CalculateWidth();
            carriages.Add(carriage);
        }

        for (int i = 0; i < topObjects.Length && i < carriages.Count; i++)
        {
            carriages[i].topCarriage = new TopCarriage { obj = topObjects[i] };
            carriages[i].topCarriage.CalculateWidth();
        }

    }

    private void SpawnPlayers()
    {
        int half = numberOfPlayer / 2;

        for (int i = 0; i < numberOfPlayer; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab);
            PlayerController player = playerObj.GetComponent<PlayerController>();
            player.SetPlayerId(i + 1);

            // Set up hand manager
            var handManager = playerObj.GetComponentInChildren<CardHandManager>(true);
            player.SetCardHandManager(handManager);

            allPlayers.Add(player);
            player.CheckTurn(false); // Hide all player canvases initially

            int place = 1;
            //  place = UnityEngine.Random.Range(0, 1);
            if (i < half)
            {
                carriages[place].topCarriage.AddPlayer(playerObj);
                player.SetPosition(carriages[place], true);
            }
            else
            {
                carriages[place].bottomCarriage.AddPlayer(playerObj);
                player.SetPosition(carriages[place], false);
            }
        }
    }
    private void SpawnMarshal()
    {
        int last = carriages.Count - 1;
        int randomIndex = UnityEngine.Random.Range(last, last - 1);

        // Instantiate marshal
        GameObject marshalObj = Instantiate(marshalPrefab);
        marshal = marshalObj.GetComponent<PlayerController>(); // Assign directly to marshal field
        marshal.SetPlayerId(99);
        marshal.SetPosition(carriages[randomIndex], false); // bottom only
        carriages[randomIndex].bottomCarriage.AddPlayer(marshalObj);

        // Debug.Log($"Marshal spawned at carriage index {randomIndex} (bottom).");
    }

    // ------------------------- Rounds -------------------------

    public void StartNewRound()
    {
        Debug.Log($"--- Starting Round {currentRound} ---");

        currentTurnInRound = 0;

        // Rotate player order based on roundStartingPlayerIndex
        List<PlayerController> orderedPlayers = new();
        for (int i = 0; i < allPlayers.Count; i++)
        {
            int index = (roundStartingPlayerIndex + i) % allPlayers.Count;
            orderedPlayers.Add(allPlayers[index]);
        }
        roundPlayerOrder = orderedPlayers; // Store for this round

        currentPlayerIndex = 0;

        foreach (var player in allPlayers)
        {
            player.PrepareDeckForNewRound();
            player.CheckTurn(false);
        }

        SetCurrentPlayer(roundPlayerOrder[currentPlayerIndex]);
        UpdateRoundUI();
    }
    private void OnRoundEnd(System.Action onComplete)
    {
        Debug.Log("Round ended! Run summary, animations, score updates, etc.");
        StartCoroutine(WaitAndThen(2f, onComplete)); // Optional delay
    }

    private System.Collections.IEnumerator WaitAndThen(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }


    // ------------------------- Player Turn -------------------------

    public void SetCurrentPlayer(PlayerController player)
    {
        currentPlayer?.CheckTurn(false); // Hide previous canvas (null-safe)
        currentPlayer = player;
        currentPlayer.CheckTurn(true);
        UpdatePlayerUI(); // <- new: update current player text
    }

    public void MoveToNextPlayer()
    {
        currentPlayer.CheckTurn(false);
        currentPlayerIndex++;

        if (currentPlayerIndex >= roundPlayerOrder.Count)
        {
            currentTurnInRound++;
            roundStartingPlayerIndex = (roundStartingPlayerIndex + 1) % allPlayers.Count;

            OnRoundEnd(() =>
            {
                PlayedCard.Instance.StartResolvingCards(() =>
                {
                    currentRound++;
                    StartNewRound();
                });
            });
            return;
        }

        SetCurrentPlayer(roundPlayerOrder[currentPlayerIndex]);
    }


    // ------------------------- Accessors -------------------------

    public PlayerController GetCurrentPlayer() => currentPlayer;

    public List<PlayerController> GetAllPlayers() => allPlayers;

    private void SpawnTreasures()
    {
        foreach (var treasure in treasures)
        {
            for (int i = 0; i < treasure.amount; i++)
            {
                int from = 1;
                switch (treasure.treasureSO.priority)
                {
                    case TreasurePriority.Diamond:
                        from = carriages.Count - 2;
                        break;
                    case TreasurePriority.MoneyBag:
                        from = carriages.Count - 3;
                        break;
                    case TreasurePriority.Coin:
                        from = 1;
                        break;
                }
                int carriageIndex = Utility.GetRandom(from, carriages.Count);
                GameObject newTreasure = Instantiate(treasure.treasureSO.treasureObj);
                carriages[carriageIndex].bottomCarriage.AddTreasure(treasure.treasureSO, newTreasure);
            }
        }
    }


    public void EnforceMarshalRules()
    {
        var players = UnityEngine.Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None); // ✅ Updated API
        var marshal = GetMarshal();
        var marshalCarriage = marshal.CurrentCarriage;

        foreach (var player in players)
        {
            if (player == marshal || player.IsOnTop) continue;

            if (player.CurrentCarriage == marshalCarriage)
            {
                player.Climb(); // Force climb

                if (marshal.GetBullets() > 0)
                {
                    player.ReceiveBulletCard();
                    marshal.GiveBulletCard(); // ✅ Marshal loses 1 bullet
                    LogAction($"{player.PlayerName} was forced to climb by the Marshal and received a bullet card.");
                }
                else
                {
                    LogAction($"{player.PlayerName} was forced to climb by the Marshal.");
                }
            }
        }
    }

    private void UpdateRoundUI()
    {
        if (currentRoundText != null)
            currentRoundText.text = $"Round {currentRound} - Turn {currentTurnInRound + 1}";
    }

    private void UpdatePlayerUI()
    {
        if (currentPlayerText != null)
            currentPlayerText.text = $"Current player: {currentPlayer.PlayerName}";
    }

    public void LogAction(string message)
    {
        if (gameLogText != null)
            gameLogText.text = message;
    }
}






