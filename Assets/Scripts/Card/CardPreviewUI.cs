using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPreviewUI : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text cardNameText, CardDescriptionText;
    public Button cancelButton, playButton;
    private ActionCardData currentCard;
    public static CardPreviewUI Instance { get; private set; }

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

    public void ShowCard(ActionCardData actionCard, System.Action onConfirm)
    {
        gameObject.SetActive(true);

        currentCard = actionCard;
        cardImage.sprite = currentCard.cardImage;
        cardNameText.text = currentCard.cardName;
        CardDescriptionText.text = currentCard.description;

        gameObject.SetActive(true);

        playButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        playButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            gameObject.SetActive(false);
        });

        cancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });


    }

}
