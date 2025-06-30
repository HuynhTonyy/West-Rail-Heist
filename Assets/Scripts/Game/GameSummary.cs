using UnityEngine;
using System.Collections.Generic;

public class GameSummary : MonoBehaviour
{
    [SerializeField] private GameObject playersResult;
    [SerializeField] private GameObject rank;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShowResults();
        }
    }
    public void ShowResults()
    {
        List<PlayerController> players = GameManager.Instance.GetAllPlayers();
        foreach (var player in players)
        {
            Instantiate(rank,playersResult.transform);
        }
    }

}
