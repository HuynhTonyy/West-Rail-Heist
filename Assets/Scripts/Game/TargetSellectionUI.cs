using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectionUI : MonoBehaviour
{
    public static TargetSelectionUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform targetButtonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowTargetSelection(PlayerController actingPlayer, List<PlayerController> targets, Action<PlayerController> onSelected)
    {
        gameObject.SetActive(true);
        titleText.text = $"{actingPlayer.PlayerName}, choose a player to perform action";
        ClearButtons();

        foreach (var target in targets)
        {
            var btnObj = Instantiate(buttonPrefab, targetButtonContainer);
            var btn = btnObj.GetComponent<Button>();
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            btnText.text = target.PlayerName;

            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                onSelected?.Invoke(target);
            });
        }
    }

    public void ShowDirectionSelection(string title, List<string> labels, System.Action<string> onSelected)
    {
        gameObject.SetActive(true);
        titleText.text = title;
        ClearButtons();

        foreach (var label in labels)
        {
            var btnObj = Instantiate(buttonPrefab, targetButtonContainer);
            var btn = btnObj.GetComponent<Button>();
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            btnText.text = label;

            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                onSelected?.Invoke(label);
            });
        }
    }

    public void ShowCarriageSelection(PlayerController player, List<Carriage> carriages, System.Action<Carriage> onSelected)
    {
        gameObject.SetActive(true);
        titleText.text = $"Choose carriage to move";

        ClearButtons();

        foreach (var carriage in carriages)
        {
            var btnObj = Instantiate(buttonPrefab, targetButtonContainer);
            var btn = btnObj.GetComponent<Button>();
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            int index = GameManager.Instance.GetCarriageIndex(carriage);
            int reversedDisplayIndex = GameManager.Instance.GetCarriages().Count - index;

            btnText.text = $"Carriage {reversedDisplayIndex}";

            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                onSelected?.Invoke(carriage);
            });
        }
    }
    public void ShowTreasureSelection(PlayerController player, List<TreasureSO> treasureSOs, System.Action<TreasureSO> onSelected)
    {
        gameObject.SetActive(true);
        titleText.text = $"Choose treasure to loot";

        ClearButtons();

        foreach (var treasureSO in treasureSOs)
        {
            var btnObj = Instantiate(buttonPrefab, targetButtonContainer);
            var btn = btnObj.GetComponent<Button>();
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();


            btnText.text = $"{treasureSO.treasureName}: {treasureSO.value}$";

            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                onSelected?.Invoke(treasureSO);
            });
        }
    }
    private void ClearButtons()
    {
        foreach (Transform child in targetButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
