using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Image cardImage;
    public Text cardName, cardDescription;

    private ActionCardData cardData;

    public void SetUp(ActionCardData cardData)
    {
        if (cardData == null) return;

        cardImage.sprite = cardData.cardImage;
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.description;
    }
}
