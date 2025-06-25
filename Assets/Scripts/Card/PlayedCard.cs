using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayedCard : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text cardNameText, cardDescriptionText, gameLogText;
    [SerializeField] private List<(PlayerController player, ActionCardData card)> cardQueue = new();
    public static PlayedCard Instance { get; private set; }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple CardPreviewUI detected!");
        }
        Instance = this;
    }
    public void AddCardToQueue(PlayerController player, CardUI cardUI)
    {
        gameObject.SetActive(true);
        var cardData = cardUI.GetCardData();
        if (cardData == null)
        {
            Debug.LogWarning("CardData is null, cannot add to queue.");
            return;
        }

        // Add to queue
        cardQueue.Add((player, cardData));

        // Update UI to show the last played card
        cardImage.sprite = cardData.cardImage;
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.description;

        // previousCardText.text = $"{player.PlayerName} play {cardData.cardName}";
    }

    public List<(PlayerController, ActionCardData)> GetPlayedCards()
    {
        return cardQueue;
    }

    public void ClearPlayedCards()
    {
        cardQueue.Clear();

        cardImage.sprite = null;
        cardNameText.text = "";
        cardDescriptionText.text = "";

    }
    public void StartResolvingCards()
    {
        CardPreviewUI.Instance?.gameObject.SetActive(false);
        // Disable all player UI panels before resolving
        foreach (var player in GameManager.Instance.GetAllPlayers())
        {
            player.HidePlayerCanvas();
        }
        StartCoroutine(ResolveAllCardsSequentially());
    }

    private IEnumerator<WaitForSeconds> ResolveAllCardsSequentially()
    {
        while (cardQueue.Count > 0)
        {
            var (player, card) = cardQueue[0];
            cardQueue.RemoveAt(0);

            // Show the card UI
            gameObject.SetActive(true);
            cardImage.sprite = card.cardImage;
            cardNameText.text = card.cardName;
            cardDescriptionText.text = card.description;
            gameLogText.text = $"{player.PlayerName} is now: {card.cardName}";

            // Wait 1 second before executing
            yield return new WaitForSeconds(1f);

            // Execute the card action
            switch (card.cardName.ToLower())
            {
                case "move":
                    player.Move();
                    break;
                case "shoot":
                    player.Shoot();
                    break;
                case "punch":
                    player.Punch();
                    break;
                case "loot":
                    player.Loot();
                    break;
                case "climb":
                    player.Climb();
                    break;
                case "marshal":
                    player.Marshal();
                    break;
                default:
                    Debug.Log($"{player.PlayerName} played unknown card: {card.cardName}");
                    break;
            }

            // Wait another 1 second before moving to the next card
            yield return new WaitForSeconds(1f);
        }

        // All cards processed, hide the UI
        gameLogText.text = "End of script";
        gameObject.SetActive(false);
    }

}
