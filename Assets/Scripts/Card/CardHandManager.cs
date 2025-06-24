using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class CardHandManager : MonoBehaviour
{
    public Transform cardHandContainer;
    public GameObject cardUIPrefab;
    public ActionCardData[] defaultCards;
    private ActionCardData[] startingHandCard;
    [SerializeField] private List<ActionCardData> remainingCards;
    [SerializeField] private List<CardUI> currentCards = new List<CardUI>();
    [SerializeField] private TMP_Text drawCountText;
    [SerializeField] private PlayedCard playedCard;
    void Start()
    {
        startingHandCard = GetStartingHandCards();
        DisplayHands(startingHandCard);
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
    }

    public void DrawCard(int count = 2)
    {
        if (remainingCards.Count == 0)
        {
            Debug.Log("No more cards to draw.");
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
            Debug.LogError("PlayedCard.Instance is null!");
            return;
        }

        PlayedCard.Instance.AddCardToQueue(cardUI);
        Destroy(cardUI.gameObject);
        SortHandCards();
    }

    private void UpdateDrawButtonText()
    {
        drawCountText.text = $"Draw 2 Cards\n({remainingCards.Count})";
    }
}