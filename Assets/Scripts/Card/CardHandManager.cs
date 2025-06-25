using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CardHandManager : MonoBehaviour
{
    [Header("Card Setup")]
    public Transform cardHandContainer;
    public GameObject cardUIPrefab;
    public ActionCardData[] defaultCards;

    [Header("UI")]
    [SerializeField] private TMP_Text drawCountText;

    [Header("Reference")]
    [SerializeField] private List<ActionCardData> remainingCards = new List<ActionCardData>();
    [SerializeField] private List<CardUI> currentCards = new List<CardUI>();

    public void InitializeHand()
    {
        var startingHandCard = GetStartingHandCards();
        DisplayHands(startingHandCard);
        // SetCurrentPlayer(GameManager.Instance.GetCurrentPlayer());
        UpdateDrawButtonText();
    }

    public ActionCardData[] GetStartingHandCards()
    {
        remainingCards = defaultCards.ToList();

        var randomCards = remainingCards.OrderBy(x => Random.value).Take(6).ToList();
        var sortedCards = randomCards.OrderBy(card => card.cardName).ToArray();

        return sortedCards;
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
        // Sort card list
        currentCards = currentCards
            .OrderBy(card => card.GetCardData().cardName)
            .ToList();
        // Apply visual order in UI (layout)
        for (int i = 0; i < currentCards.Count; i++)
        {
            currentCards[i].transform.SetSiblingIndex(i);
        }
        UpdateDrawButtonText();
    }

    public void DrawCard(int count = 2)
    {
        if (remainingCards.Count == 0)
        {
            // Debug.Log("No more cards to draw.");
            return;
        }

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
        GameManager.Instance.LogAction(currentPlayer.PlayerName + $" drew {drawCount} cards and skipped their turn.");
        GameManager.Instance.MoveToNextPlayer();
    }

    public void RemoveCard(CardUI cardUI)
    {
        currentCards.Remove(cardUI);
        Destroy(cardUI.gameObject);
    }

    public void PlayCard(CardUI cardUI)
    {
        currentCards.Remove(cardUI);

        if (PlayedCard.Instance == null)
        {
            // Debug.LogError("PlayedCard.Instance is null!");
            return;
        }

        var currentPlayer = GameManager.Instance.GetCurrentPlayer();
        PlayedCard.Instance.AddCardToQueue(currentPlayer, cardUI);

        Destroy(cardUI.gameObject);
        SortHandCards();

        GameManager.Instance.LogAction(currentPlayer.PlayerName + $" played {cardUI.GetCardData().cardName}");
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