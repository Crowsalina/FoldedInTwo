using UnityEngine;
using UnityEngine.UI;
public class PowerBarHandler : MonoBehaviour
{
    [SerializeField]
    private Image powerBar;

    public void UpdatePowerBar(int currentPower)
    {
        powerBar.fillAmount = currentPower / 10f;
    }
}
