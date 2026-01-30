using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int round;
    public bool canPlayerInput, isFullscreen;
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
        isFullscreen = true;
        yearManager.SetYearText();
        provinceManager.SpawnAllProvinces();
    }

    public void RoundEnd()
    {
        orderParser.StartParsingOrders();
    }
    public void ToggleFullScreen()
    {
        if(isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        isFullscreen = !isFullscreen;
    }
}