using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class YearManager : MonoBehaviour
{
    public int currentSeason;
    private int yearNum;
    private string seasonString;
    [SerializeField]
    private TextMeshProUGUI YearText;
    private GameManager gameManager;
    private OrderParser orderParser;
    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        orderParser = FindFirstObjectByType<OrderParser>();
        currentSeason = 2;
        yearNum = 1900;
    }
    public void SetYearText()
    {
        switch (currentSeason)
        {
            case 0:
                currentSeason = 1;
                break;
            case 1:
                if (orderParser.UnusedOwnedSupplies.Count > 0)
                {
                    currentSeason = 2;
                }
                else
                {
                    yearNum++;
                    currentSeason = 0;
                }
                    break;
            case 2:
                currentSeason = 0;
                yearNum++;
                break;
        }
        switch (currentSeason)
        {
            case 0:
                seasonString = "Spring";
                break;
            case 1:
                seasonString = "Autumn";
                break;
            case 2:
                seasonString = "Winter";
                break;
        }

        YearText.text = (seasonString + " - " + yearNum.ToString());
    }
}
