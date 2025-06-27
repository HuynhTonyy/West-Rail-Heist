using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private int playerId;
    public int PlayerId => playerId;
    public string PlayerName => $"P{playerId}";

    [SerializeField] private int pendingBulletCards = 0;
    [SerializeField] private int playerBullets = 6;
    public Carriage CurrentCarriage { get; private set; }
    public bool IsOnTop { get; private set; }


    [Header("References")]
    private TextMeshPro playerNameText;
    private CardHandManager cardHandManager;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject playerCanvas;


    private void Awake()
    {
        // Auto-assign canvas if not set
        if (playerCanvas == null)
        {
            playerCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;
        }
        // Auto-assign CardHandManager if not manually injected
        if (cardHandManager == null)
        {
            cardHandManager = GetComponentInChildren<CardHandManager>(true);
        }
        if (playerCanvas != null)
        {
            playerCanvas.SetActive(false);
        }
    }

    public void SetPosition(Carriage carriage, bool isOnTop)
    {
        CurrentCarriage = carriage;
        IsOnTop = isOnTop;
    }
    public int GetBullets() => playerBullets;

    public void SetPlayerId(int id)
    {
        playerId = id;
        playerNameText = GetComponentInChildren<TextMeshPro>();

        if (playerNameText != null)
            playerNameText.text = PlayerName;
    }

    public void CheckTurn(bool myTurn)
    {
        if (playerCanvas == null || cardHandManager == null)
            return;

        if (myTurn)
        {
            playerCanvas.SetActive(true);
            cardHandManager.EnableInteraction(true);
        }
        else
        {
            cardHandManager.EnableInteraction(false);
            playerCanvas.SetActive(false);
        }
    }
    private void UpdateBulletUI()
    {
        if (bulletText != null)
            bulletText.text = playerBullets.ToString();
    }

    public void PrepareDeckForNewRound()
    {
        cardHandManager.PrepareDeck(pendingBulletCards); // Build deck with bullets
        pendingBulletCards = 0;                          // Reset bullets to 0 for next round
        cardHandManager.InitializeHand();                // Draw fresh 6 cards
        UpdateBulletUI();                                // Update UI if you have bullet count shown
    }

    public void ReceiveBulletCard()
    {
        pendingBulletCards++;
        Debug.Log($"{PlayerName} received a Bullet Card. Total: {pendingBulletCards}");
    }

    public void SetCardHandManager(CardHandManager manager)
    {
        cardHandManager = manager;
    }

    public void ResetPlayer()
    {
        pendingBulletCards = 0;
        HidePlayerCanvas();
        cardHandManager.PrepareDeck(0);
    }

    public CardHandManager GetHandManager() => cardHandManager;

    public void HidePlayerCanvas()
    {
        if (playerCanvas != null)
            playerCanvas.SetActive(false);
    }

    public List<PlayerController> GetValidShootTargets()
    {
        var carriages = GameManager.Instance.GetCarriages();
        int myIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);
        List<PlayerController> validTargets = new();

        if (IsOnTop)
        {
            List<PlayerController> leftTargets = null;
            List<PlayerController> rightTargets = null;
            int leftDist = int.MaxValue, rightDist = int.MaxValue;

            // Check left
            for (int i = myIndex - 1, dist = 1; i >= 0; i--, dist++)
            {
                var targets = carriages[i].topCarriage.GetPlayers()
                    .Where(p => p != this.gameObject)
                    .Select(p => p.GetComponent<PlayerController>())
                    .ToList();

                if (targets.Count > 0)
                {
                    leftTargets = targets;
                    leftDist = dist;
                    break;
                }
            }

            // Check right
            for (int i = myIndex + 1, dist = 1; i < carriages.Count; i++, dist++)
            {
                var targets = carriages[i].topCarriage.GetPlayers()
                    .Where(p => p != this.gameObject)
                    .Select(p => p.GetComponent<PlayerController>())
                    .ToList();

                if (targets.Count > 0)
                {
                    rightTargets = targets;
                    rightDist = dist;
                    break;
                }
            }

            // Determine which side is closer
            if (leftDist < rightDist)
            {
                validTargets = leftTargets;
            }
            else if (rightDist < leftDist)
            {
                validTargets = rightTargets;
            }
            else if (leftDist == rightDist && leftTargets != null && rightTargets != null)
            {
                // Optional: If both sides equal distance (e.g. middle player)
                validTargets = leftTargets.Concat(rightTargets).ToList();
            }
        }
        else
        {
            // Bottom floor: adjacent carriages only
            if (myIndex > 0)
            {
                validTargets.AddRange(
                    carriages[myIndex - 1].bottomCarriage.GetPlayers()
                        .Where(p => p != this.gameObject)
                        .Select(p => p.GetComponent<PlayerController>())
                );
            }

            if (myIndex < carriages.Count - 1)
            {
                validTargets.AddRange(
                    carriages[myIndex + 1].bottomCarriage.GetPlayers()
                        .Where(p => p != this.gameObject)
                        .Select(p => p.GetComponent<PlayerController>())
                );
            }
        }

        return validTargets;
    }
    public List<PlayerController> GetValidPunchTargets()
    {
        List<GameObject> players = IsOnTop
            ? CurrentCarriage.topCarriage.GetPlayers()
            : CurrentCarriage.bottomCarriage.GetPlayers();

        return players
            .Where(p => p != this.gameObject)
            .Select(p => p.GetComponent<PlayerController>())
            .ToList();
    }
    public List<Carriage> GetValidMoveCarriages()
    {
        var carriages = GameManager.Instance.GetCarriages();
        int myIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);
        List<Carriage> validMoves = new();

        // Bottom floor can move to immediate left or right
        if (!IsOnTop)
        {
            if (myIndex > 0)
                validMoves.Add(carriages[myIndex - 1]);
            if (myIndex < carriages.Count - 1)
                validMoves.Add(carriages[myIndex + 1]);
        }
        else
        {
            // Top floor can move 1 or 2 carriages left or right
            for (int offset = 1; offset <= 2; offset++)
            {
                if (myIndex - offset >= 0)
                    validMoves.Add(carriages[myIndex - offset]);
                if (myIndex + offset < carriages.Count)
                    validMoves.Add(carriages[myIndex + offset]);
            }
        }

        return validMoves;
    }
    private void MoveTargetToCarriage(PlayerController target, Carriage destination)
    {
        if (target.IsOnTop)
        {
            destination.topCarriage.AddPlayer(target.gameObject);
        }
        else
        {
            destination.bottomCarriage.AddPlayer(target.gameObject);
        }

        target.SetPosition(destination, target.IsOnTop);
    }
    private void MoveTargetToNearbyCarriage(PlayerController target, System.Action onComplete = null)
    {
        var carriages = GameManager.Instance.GetCarriages();
        int currentIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);

        Dictionary<string, Carriage> moveOptions = new();

        if (currentIndex > 0)
            moveOptions.Add("Left", carriages[currentIndex - 1]);

        if (currentIndex < carriages.Count - 1)
            moveOptions.Add("Right", carriages[currentIndex + 1]);

        if (moveOptions.Count == 0)
        {
            GameManager.Instance.LogAction($"{target.PlayerName} has no where to go after being punched!");
            onComplete?.Invoke();
            return;
        }

        if (moveOptions.Count == 1)
        {
            // Only one direction
            MoveTargetToCarriage(target, moveOptions.Values.First());
            onComplete?.Invoke();
        }
        else
        {
            // Let player choose Left or Right
            TargetSelectionUI.Instance.ShowDirectionSelection(
                $"Choose direction to punch {target.PlayerName}",
                moveOptions.Keys.ToList(),
                selected =>
                {
                    MoveTargetToCarriage(target, moveOptions[selected]);
                    onComplete?.Invoke();
                }
            );
        }
    }

    // Example actions
    public void Move() => Debug.Log($"{PlayerName} moves");
    public void Climb() => Debug.Log($"{PlayerName} climbs");
    public void Loot() => Debug.Log($"{PlayerName} loots");
    public void Shoot(PlayerController target)
    {
        if (playerBullets <= 0)
        {
            GameManager.Instance.LogAction($"{PlayerName} has no bullets left!");
            return;
        }
        playerBullets--;
        UpdateBulletUI();
        target.ReceiveBulletCard();
        GameManager.Instance.LogAction($"{PlayerName} shot {target.PlayerName}!");
        PlayedCard.Instance.FinishCurrentCard();
    }
    public void Punch(PlayerController target)
    {
        GameManager.Instance.LogAction($"{PlayerName} punched {target.PlayerName}!");
        MoveTargetToNearbyCarriage(target, () =>
        {
            GameManager.Instance.LogAction($"{target.PlayerName} was moved after the punch.");
            PlayedCard.Instance.FinishCurrentCard();
        });
    }
    public void Marshal() => Debug.Log($"{PlayerName} moves the Marshal");
}
