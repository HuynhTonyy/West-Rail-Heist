using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayedCard : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text cardNameText, cardDescriptionText;
    [SerializeField] private List<ActionCardData> cardQueue = new List<ActionCardData>();
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
    public void AddCardToQueue(CardUI cardUI)
    {
        gameObject.SetActive(true);
        var cardData = cardUI.GetCardData();
        if (cardData == null)
        {
            Debug.LogWarning("CardData is null, cannot add to queue.");
            return;
        }

        // Add to queue
        cardQueue.Add(cardData);

        // Update UI to show the last played card
        cardImage.sprite = cardData.cardImage;
        cardNameText.text = cardData.cardName;
        cardDescriptionText.text = cardData.description;
    }

    public List<ActionCardData> GetPlayedCards()
    {
        return cardQueue;
    }

    public void ClearPlayedCards()
    {
        cardQueue.Clear();

        // Clear the display UI
        cardImage.sprite = null;
        cardNameText.text = "";
        cardDescriptionText.text = "";
    }
}
