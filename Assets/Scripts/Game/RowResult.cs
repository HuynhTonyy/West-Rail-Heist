using UnityEngine;
using TMPro;


public class RowResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI bonusText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void SetResult(int rank, string name, int cash, bool isBonus)
    {
        rankText.text = $"No.{rank}";
        nameText.text = name;
        cashText.text = $"{cash}$";
        Debug.Log(isBonus);
        bonusText.gameObject.SetActive(isBonus);
    }
}
