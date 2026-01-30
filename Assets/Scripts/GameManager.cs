using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int round;
    public bool canPlayerInput;
    public ProvinceSpawningManager provinceManager;
    public OrderParser orderParser;
    public YearManager yearManager;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static GameManager instance = null;

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        round = 0;
        yearManager.SetYearText();
        provinceManager.SpawnAllProvinces();
    }

    public void RoundEnd()
    {
        orderParser.StartParsingOrders();
    }
}