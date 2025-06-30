using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayedCard : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text cardNameText, cardDescriptionText, currentTurnText, gameLogText;
    [SerializeField] private List<(PlayerController player, ActionCardData card)> cardQueue = new();
    private bool isResolving = false;
    private System.Action onFinishedAllCards;
    public static PlayedCard Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PlayedCard instances detected!");
        }
        Instance = this;
        gameObject.SetActive(false);
    }

    public void AddCardToQueue(PlayerController player, CardUI cardUI)
    {
        var cardData = cardUI.GetCardData();
        if (cardData == null) return;

        cardQueue.Add((player, cardData));

        // Optional: show card immediately
        cardImage.sprite = cardData.cardImage;
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.description;
        gameObject.SetActive(true);
    }

    public void StartResolvingCards(System.Action onComplete)
    {

        gameLogText.text = "Round ended - Beginning heist !";
        CardPreviewUI.Instance?.gameObject.SetActive(false);

        foreach (var player in GameManager.Instance.GetAllPlayers())
        {
            player.HidePlayerCanvas();
        }

        isResolving = false;
        onFinishedAllCards = onComplete;
        ResolveNextCard();
    }


    public void ResolveNextCard()
    {
        if (isResolving) return;

        if (cardQueue.Count == 0)
        {
            gameLogText.text = "Heist completed - A new round starts!";
            gameObject.SetActive(false);
            onFinishedAllCards?.Invoke();  // ✅ Notify GameManager here
            return;
        }

        var (player, card) = cardQueue[0];
        cardQueue.RemoveAt(0);

        // Show card info
        cardImage.sprite = card.cardImage;
        cardNameText.text = card.cardName;
        cardDescriptionText.text = card.description;
        currentTurnText.text = player.PlayerName + "'s turn";
        gameLogText.text = $"{player.PlayerName} use {card.cardName} Card";

        isResolving = true;

        // Card effect logic
        switch (card.cardName.ToLower())
        {
            case "move":
                Debug.Log($"{player.PlayerName} is trying to move.");

                var moveOptions = player.GetValidMoveCarriages();

                if (moveOptions.Count == 0)
                {
                    GameManager.Instance.LogAction($"{player.PlayerName} has nowhere to move.");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowCarriageSelection(player, moveOptions, destination =>
                    {
                        player.Move(destination);
                    });
                }
                break;
            case "shoot":
                Debug.Log($"{player.PlayerName} is trying to shoot.");

                if (player.GetBullets() <= 0) // <- Add this method or field if not public
                {
                    GameManager.Instance.LogAction($"{player.PlayerName} has no bullets left and cannot shoot.");
                    FinishCurrentCard();
                    break;
                }
                var targets = player.GetValidShootTargets();
                if (targets.Count == 0)
                {
                    GameManager.Instance.LogAction($"{player.PlayerName} tried to shoot something");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowTargetSelection(player, targets, target =>
                    {
                        player.Shoot(target); // 👈 Make sure this is the SHOOT action
                        FinishCurrentCard(); // You already call FinishCurrentCard() inside Shoot()
                    });
                }
                break;
            case "punch":
                Debug.Log($"{player.PlayerName} is trying to punch.");

                var punchTargets = player.GetValidPunchTargets();
                if (punchTargets.Count == 0)
                {
                    GameManager.Instance.LogAction($"{player.PlayerName} tried to punch but found no valid targets.");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowTargetSelection(player, punchTargets, target =>
                    {
                        player.Punch(target);
                    });
                }
                break;
            case "loot":
                Debug.Log($"{player.PlayerName} is trying to loot.");
                var treasures = player.GetValidTreasure();
                if (treasures.Count <= 0)
                {
                    GameManager.Instance.LogAction($"{player.PlayerName} tried to loot but found no treasure.");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowTreasureSelection(player, treasures, treasure =>
                    {
                        player.Loot(treasure);
                    });
                }
                break;
            case "climb":
                Debug.Log($"{player.PlayerName} is trying to climb.");
                player.Climb();
                FinishCurrentCard();
                break;
            case "marshal":
                Debug.Log($"{player.PlayerName} is trying to move the Marshal.");

                var marshal = GameManager.Instance.GetMarshal(); // Get marshal PlayerController
                var marshalIndex = GameManager.Instance.GetCarriageIndex(marshal.CurrentCarriage);
                var marshalMove = Utility.GetNearbyCarriages(marshalIndex, 1);

                if (marshalMove.Count == 0)
                {
                    GameManager.Instance.LogAction("Marshal cannot be moved. No adjacent carriage.");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowCarriageSelection(player, marshalMove, carriage =>
                    {
                        player.Marshal(carriage);
                    });
                }
                break;
            default: Debug.Log($"Unknown card: {card.cardName}"); FinishCurrentCard(); break;
        }
    }

    public void FinishCurrentCard()
    {
        isResolving = false;
        // Add delay or call immediately
        StartCoroutine(WaitThenNext());
    }

    private System.Collections.IEnumerator WaitThenNext()
    {
        yield return new WaitForSeconds(2f);
        ResolveNextCard();
    }

    public void ClearPlayedCards()
    {
        cardQueue.Clear();
        cardImage.sprite = null;
        cardNameText.text = "";
        cardDescriptionText.text = "";
        gameObject.SetActive(false);
    }
}