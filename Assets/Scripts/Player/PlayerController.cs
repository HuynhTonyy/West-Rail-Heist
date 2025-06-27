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

    [SerializeField] private bool isMarshal = false;
    public bool IsMarshal => isMarshal;

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
        if (!isMarshal)
        {
            if (playerCanvas == null)
                playerCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;

            if (cardHandManager == null)
                cardHandManager = GetComponentInChildren<CardHandManager>(true);

            if (playerCanvas != null)
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
        if (isMarshal) return;

        if (playerCanvas == null || cardHandManager == null)
            return;

        playerCanvas.SetActive(myTurn);
        cardHandManager.EnableInteraction(myTurn);
    }

    public CardHandManager GetHandManager() => cardHandManager;

    private void UpdateBulletUI()
    {
        if (!isMarshal && bulletText != null)
            bulletText.text = playerBullets.ToString();
    }

    public void PrepareDeckForNewRound()
    {
        if (isMarshal) return;

        cardHandManager.PrepareDeck(pendingBulletCards);
        pendingBulletCards = 0;
        cardHandManager.InitializeHand();
        UpdateBulletUI();
    }

    public void ReceiveBulletCard()
    {
        if (isMarshal) return;

        pendingBulletCards++;
        Debug.Log($"{PlayerName} received a Bullet Card. Total: {pendingBulletCards}");
    }

    public void GiveBulletCard()
    {
        if (playerBullets > 0)
        {
            playerBullets--;
            Debug.Log($"{PlayerName} gave a Bullet Card. Remaining: {playerBullets}");
        }
        else
        {
            Debug.LogWarning($"{PlayerName} has no pending Bullet Cards to give!");
        }
    }

    public void SetCardHandManager(CardHandManager manager)
    {
        if (!isMarshal)
            cardHandManager = manager;
    }

    public void ResetPlayer()
    {
        if (!isMarshal)
        {
            pendingBulletCards = 0;
            HidePlayerCanvas();
            cardHandManager.PrepareDeck(0);
        }
    }

    public void HidePlayerCanvas()
    {
        if (!isMarshal && playerCanvas != null)
            playerCanvas.SetActive(false);
    }

    public List<PlayerController> GetValidShootTargets()
    {
        var carriages = GameManager.Instance.GetCarriages();
        int myIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);
        List<PlayerController> validTargets = new();

        if (IsOnTop)
        {
            // Search left for nearest carriage with player on top
            for (int i = myIndex - 1; i >= 0; i--)
            {
                var players = Utility.GetPlayersAt(i, true)
                    .Where(p => p != this.gameObject)
                    .Select(p => p.GetComponent<PlayerController>())
                    .Where(p => !p.IsMarshal)
                    .ToList();

                if (players.Count > 0)
                {
                    validTargets.AddRange(players);
                    break; // Stop after the first non-empty top carriage on the left
                }
            }

            // Search right for nearest carriage with player on top
            for (int i = myIndex + 1; i < carriages.Count; i++)
            {
                var players = Utility.GetPlayersAt(i, true)
                    .Where(p => p != this.gameObject)
                    .Select(p => p.GetComponent<PlayerController>())
                    .Where(p => !p.IsMarshal)
                    .ToList();

                if (players.Count > 0)
                {
                    validTargets.AddRange(players);
                    break; // Stop after the first non-empty top carriage on the right
                }
            }
        }
        else
        {
            // Bottom: only adjacent carriages
            if (myIndex > 0)
            {
                validTargets.AddRange(
                    Utility.GetPlayersAt(myIndex - 1, false)
                        .Where(p => p != this.gameObject)
                        .Select(p => p.GetComponent<PlayerController>())
                        .Where(p => !p.IsMarshal)
                );
            }

            if (myIndex < carriages.Count - 1)
            {
                validTargets.AddRange(
                    Utility.GetPlayersAt(myIndex + 1, false)
                        .Where(p => p != this.gameObject)
                        .Select(p => p.GetComponent<PlayerController>())
                        .Where(p => !p.IsMarshal)
                );
            }
        }

        return validTargets;
    }

    public List<PlayerController> GetValidPunchTargets()
    {
        var players = IsOnTop
            ? CurrentCarriage.topCarriage.GetPlayers()
            : CurrentCarriage.bottomCarriage.GetPlayers();

        return players
            .Where(p => p != this.gameObject)
            .Select(p => p.GetComponent<PlayerController>())
            .Where(p => !p.IsMarshal)
            .ToList();
    }

    public List<Carriage> GetValidMoveCarriages()
    {
        var carriages = GameManager.Instance.GetCarriages();
        int myIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);
        int range = IsOnTop ? 2 : 1;

        return Utility.GetNearbyCarriages(myIndex, range);
    }

    public void Move(Carriage destination)
    {
        Utility.MovePlayerToCarriage(this, destination);
        GameManager.Instance.LogAction($"{PlayerName} moved to Carriage {GameManager.Instance.GetCarriageIndex(destination)}");
        GameManager.Instance.EnforceMarshalRules();
        PlayedCard.Instance.FinishCurrentCard();
    }

    public void Climb()
    {
        if (IsOnTop)
        {
            CurrentCarriage.topCarriage.RemovePlayer(gameObject);
            CurrentCarriage.bottomCarriage.AddPlayer(gameObject);
            GameManager.Instance.LogAction($"{PlayerName} dropped down inside the carriage.");
        }
        else
        {
            CurrentCarriage.bottomCarriage.RemovePlayer(gameObject);
            CurrentCarriage.topCarriage.AddPlayer(gameObject);
            GameManager.Instance.LogAction($"{PlayerName} climbed to the roof.");
        }

        IsOnTop = !IsOnTop;
        SetPosition(CurrentCarriage, IsOnTop);
        GameManager.Instance.EnforceMarshalRules();
        PlayedCard.Instance.FinishCurrentCard();
    }

    public void Loot() => Debug.Log($"{PlayerName} loots");

    public void Shoot(PlayerController target)
    {
        if (isMarshal) return;

        if (playerBullets <= 0)
        {
            GameManager.Instance.LogAction($"{PlayerName} has no bullets left!");
            return;
        }

        GiveBulletCard();
        UpdateBulletUI();
        target.ReceiveBulletCard();
        GameManager.Instance.LogAction($"{PlayerName} shot {target.PlayerName}!");

        PlayedCard.Instance.FinishCurrentCard();
    }

    public void Punch(PlayerController target)
    {
        GameManager.Instance.LogAction($"{PlayerName} punched {target.PlayerName}!");

        int currentIndex = GameManager.Instance.GetCarriageIndex(CurrentCarriage);
        var carriages = GameManager.Instance.GetCarriages();

        Dictionary<string, Carriage> moveOptions = new();

        if (currentIndex > 0)
            moveOptions["Left"] = carriages[currentIndex - 1];
        if (currentIndex < carriages.Count - 1)
            moveOptions["Right"] = carriages[currentIndex + 1];

        TargetSelectionUI.Instance.ShowDirectionSelection(
            $"Choose direction to punch {target.PlayerName}",
            moveOptions.Keys.ToList(),
            direction =>
            {
                if (moveOptions.TryGetValue(direction, out var destination))
                {
                    Utility.MovePlayerToCarriage(target, destination);
                    GameManager.Instance.LogAction($"{target.PlayerName} was punched to the {direction} carriage.");
                }
                GameManager.Instance.EnforceMarshalRules();
                PlayedCard.Instance.FinishCurrentCard();
            }
        );
    }

    public void Marshal(Carriage destination)
    {
        var marshal = GameManager.Instance.GetMarshal();
        GameManager.Instance.LogAction($"{PlayerName} moved the Marshal to carriage {GameManager.Instance.GetCarriageIndex(destination)}.");
        Utility.MovePlayerToCarriage(marshal, destination);
        GameManager.Instance.EnforceMarshalRules();
        PlayedCard.Instance.FinishCurrentCard();
    }
}
