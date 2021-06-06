using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameJam2021.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI coinsText = null;
        [SerializeField] GameObject showCompletePanel = null;
        [SerializeField] TextMeshProUGUI coinsCollectedText = null;


        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnPickupCoin(int amount)
        {
            coinsText.text = amount.ToString();
        }

        public void ShowLevelComplete()
        {
            showCompletePanel.SetActive(true);
        }

        public void UpdateCoins(int amount, int total)
        {
            coinsCollectedText.text = "Coin " + amount + " / " + total;
        }

    }
}
