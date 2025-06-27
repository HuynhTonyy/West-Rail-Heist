using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayedCard : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text cardNameText, cardDescriptionText, gameLogText;
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
            gameLogText.text = "All action completed - A new round starts!";
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
        gameLogText.text = $"{player.PlayerName} is now: {card.cardName}";

        isResolving = true;

        // Card effect logic
        switch (card.cardName.ToLower())
        {
            case "move":
                Debug.Log($"{player.PlayerName} is trying to move.");

                // var moveTargets = player.GetValidMoveCarriages();
                // if (moveTargets.Count == 0)
                // {
                //     GameManager.Instance.LogAction($"{player.PlayerName} tried to move but found no valid carriage.");
                //     FinishCurrentCard();
                // }
                // else
                // {
                //     TargetSelectionUI.Instance.ShowCarriageSelection(player, moveTargets, targetCarriage =>
                //     {
                //         player.Move(targetCarriage);
                //         FinishCurrentCard();
                //     });
                // }
                FinishCurrentCard();
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
                    GameManager.Instance.LogAction($"{player.PlayerName} tried to shoot but found no valid targets.");
                    FinishCurrentCard();
                }
                else
                {
                    TargetSelectionUI.Instance.ShowTargetSeletion(player, targets, target =>
                    {
                        player.Shoot(target);
                        // FinishCurrentCard();
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
                    TargetSelectionUI.Instance.ShowTargetSeletion(player, punchTargets, target =>
                    {
                        player.Punch(target);
                    });

                }
                break;
            case "loot": player.Loot(); FinishCurrentCard(); break;
            case "climb": player.Climb(); FinishCurrentCard(); break;
            case "marshal": player.Marshal(); FinishCurrentCard(); break;
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
        yield return new WaitForSeconds(1f);
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