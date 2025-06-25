using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    public TMP_Text cardName, cardDescription;
    private ActionCardData cardData;
    private CardHandManager cardHandManager;

    public void SetUp(ActionCardData cardData, CardHandManager cardHandManager)
    {
        if (cardData == null) return;

        this.cardData = cardData;
        this.cardHandManager = cardHandManager;

        cardImage.sprite = cardData.cardImage;
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.description;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClick();
    }

    public void OnCardClick()
    {

        if (cardData == null)
        {
            Debug.LogWarning("CardData is null");
            return;
        }

        if (CardPreviewUI.Instance != null)
        {
            CardPreviewUI.Instance.ShowCard(cardData, () =>
            {
                Debug.Log($"Playing card: {cardData.cardName}");
                cardHandManager.PlayCard(this);                
            });
        }
        else
        {
            Debug.LogWarning("CardPreviewUI instance is missing from the scene.");
        }
    }

    public ActionCardData GetCardData()
    {
        return cardData;
    }
}
