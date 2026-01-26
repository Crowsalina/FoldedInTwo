using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int round, playerHealth, enemyHealth, playerDeckSize, enemyDeckSize, playerMaxPower, enemyMaxPower, playerCurrentPower, enemyCurrentPower;
    public float roundTime;
    public bool canPlayerInput;
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
        roundTime = 60f;
        round = 0; 
    }

    public void RoundEnd()
    {
        round += 1;
        StopCoroutine(RoundTimer());
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    { 
        yield return new WaitForSeconds(roundTime);
        RoundEnd();
    }
}