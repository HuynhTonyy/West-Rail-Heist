using System;
using System.Linq;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    public Transform cardHandContainer;
    public GameObject cardUIPrefab;
    public ActionCardData[] actionCards;

    void Start()
    {
        actionCards = actionCards.OrderBy(card => card.cardName).ToArray();
        DisplayHands();
    }

    private void DisplayHands()
    {
        foreach (ActionCardData card in actionCards)
        {
            GameObject cardUIObject = Instantiate(cardUIPrefab, cardHandContainer);
            CardUI cardUI = cardUIObject.GetComponent<CardUI>();
            cardUI.SetUp(card);
        }
    }
}
