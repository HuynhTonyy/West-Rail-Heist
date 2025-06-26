using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CardHandManager : MonoBehaviour
{
    [Header("Card Setup")]
    public Transform cardHandContainer;
    public GameObject cardUIPrefab;
    [SerializeField] private ActionCardData[] defaultCards;
    [SerializeField] private ActionCardData bulletCardData;

    [Header("UI")]
    [SerializeField] private TMP_Text drawCountText;

    [Header("Runtime Data")]
    [SerializeField] private List<ActionCardData> fullDeck = new();
    [SerializeField] private List<ActionCardData> remainingCards = new();
    [SerializeField] private List<CardUI> currentCards = new();

    public void PrepareDeck(int bullets)
    {
        fullDeck = new List<ActionCardData>(defaultCards);

        for (int i = 0; i < bullets; i++)
        {
            fullDeck.Add(bulletCardData);
        }

        fullDeck = fullDeck.OrderBy(x => Random.value).ToList(); // Shuffle
        remainingCards = new List<ActionCardData>(fullDeck);
    }

    public void InitializeHand()
    {
        //Clear previous hand from UI
        foreach (var cardUI in currentCards)
        {
            Destroy(cardUI.gameObject);
        }
        currentCards.Clear();

        // Draw 6 new cards from deck
        var hand = remainingCards.Take(6).ToArray();
        DisplayHands(hand); // Removes drawn cards from remainingCards

        UpdateDrawButtonText();
    }
    public void SetDeck(List<ActionCardData> newDeck)
    {
        remainingCards = new List<ActionCardData>(newDeck);
    }

    public void AddBulletCards(ActionCardData bulletCard, int count)
    {
        for (int i = 0; i < count; i++)
        {
            remainingCards.Add(bulletCard);
        }
    }

    private void DisplayHands(ActionCardData[] cardsInHand)
    {
        foreach (ActionCardData card in cardsInHand)
        {
            remainingCards.Remove(card);
            AddCardToHand(card);
        }
        SortHandCards();
    }

    private void AddCardToHand(ActionCardData cardData)
    {
        GameObject cardUIObject = Instantiate(cardUIPrefab, cardHandContainer);
        CardUI cardUI = cardUIObject.GetComponent<CardUI>();
        cardUI.SetUp(cardData, this);
        currentCards.Add(cardUI);
    }

    private void SortHandCards()
    {
        currentCards = currentCards
            .Where(card => card != null && card.GetCardData() != null) // filter out bad cards
            .OrderBy(card => card.GetCardData().cardName)
            .ToList();

        for (int i = 0; i < currentCards.Count; i++)
        {
            currentCards[i].transform.SetSiblingIndex(i);
        }

        UpdateDrawButtonText();
    }

    public void DrawCard(int count = 2)
    {
        if (remainingCards.Count == 0) return;

        int drawCount = Mathf.Min(count, remainingCards.Count);
        var drawnCards = remainingCards.OrderBy(x => Random.value).Take(drawCount).ToList();

        foreach (var card in drawnCards)
        {
            remainingCards.Remove(card);
            AddCardToHand(card);
        }

        SortHandCards();
        UpdateDrawButtonText();
        CardPreviewUI.Instance?.gameObject.SetActive(false);

        var currentPlayer = GameManager.Instance.GetCurrentPlayer();
        GameManager.Instance.LogAction($"{currentPlayer.PlayerName} drew {drawCount} cards and skipped their turn.");
        GameManager.Instance.MoveToNextPlayer();
    }

    public void RemoveCard(CardUI cardUI)
    {
        currentCards.Remove(cardUI);
        Destroy(cardUI.gameObject);
    }

    public void PlayCard(CardUI cardUI)
    {
        var data = cardUI.GetCardData();

        if (data.cardName == "Bullet")
        {
            Debug.Log("Bullet card cannot be played.");
            return;
        }

        currentCards.Remove(cardUI);

        var currentPlayer = GameManager.Instance.GetCurrentPlayer();
        PlayedCard.Instance?.AddCardToQueue(currentPlayer, cardUI);

        Destroy(cardUI.gameObject);
        SortHandCards();

        GameManager.Instance.LogAction($"{currentPlayer.PlayerName} played {data.cardName}");
        GameManager.Instance.MoveToNextPlayer();
    }

    private void UpdateDrawButtonText()
    {
        if (drawCountText != null)
            drawCountText.text = $"Draw 2 Cards\n({remainingCards.Count} cards left)";
    }

    public void EnableInteraction(bool enable)
    {
        cardHandContainer.gameObject.SetActive(enable);
    }
}
