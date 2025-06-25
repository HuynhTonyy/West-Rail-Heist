using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

// A behaviour that is attached to a playable
public class PlayerController : MonoBehaviour

{
    [Header("Player Info")]
    [SerializeField] private int playerId;
    public int PlayerId => playerId;
    public string PlayerName => $"Player_{playerId}";

    [Header("References")]
    private TextMeshPro playerNameText;
    private CardHandManager cardHandManager;
    [SerializeField] private GameObject playerCanvas;

    private void Awake()
    {
        if (playerCanvas == null)
        {
            playerCanvas = GetComponentInChildren<Canvas>(true)?.gameObject;
        }

        if (playerCanvas != null)
        {
            playerCanvas.SetActive(false); // Always start hidden
        }
    }
    public void SetPlayerId(int id)
    {
        // Debug.Log($"Setting player ID: {id}");
        playerId = id;
        playerNameText = GetComponentInChildren<TextMeshPro>();
        playerNameText.text = PlayerName;
    }
    public void CheckTurn(bool myTurn)
    {
        if (myTurn)
        {
            // Debug.Log($"{PlayerName} is now active.");
            playerCanvas.SetActive(true);
            cardHandManager.EnableInteraction(true);
        }
        else
        {
            cardHandManager.EnableInteraction(false);
            playerCanvas.SetActive(false);
        }
    }

    public void SetCardHandManager(CardHandManager manager)
    {
        cardHandManager = manager;
    }
    public void HidePlayerCanvas()
    {
        if (playerCanvas != null)
            playerCanvas.SetActive(false);
    }

    public CardHandManager GetHandManager() => cardHandManager;
    public void Move()
    {
        // Implement movement logic here
        Debug.Log($"{PlayerName} moves");
    }
    public void Climb()
    {
        Debug.Log($"{PlayerName} climbs");
    }
    public void Loot()
    {
        Debug.Log($"{PlayerName} Loots");
    }
    public void Shoot()
    {
        Debug.Log($"{PlayerName}  Shoot!");
    }
    public void Punch()
    {
        Debug.Log($"{PlayerName} threw a punch!");
    }
    public void Marshal()
    {
        Debug.Log($"{PlayerName} move the Marshal!");
    }

}
