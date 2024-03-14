using Neu.Animations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private DoTweenActions moneyAnim;
    [SerializeField] private int startingMoney;
    private int currentMoney;

    void Start()
    {
        currentMoney = startingMoney;
        moneyText.SetText($"${currentMoney}");
    }

    public void AddMoney(int moneyToAdd)
    {
        currentMoney += moneyToAdd;
        moneyText.SetText($"${currentMoney}");
        moneyAnim.OneLoop();
    }

    public int GetMoney()
    {
        return currentMoney;
    }
}
