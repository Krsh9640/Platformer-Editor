using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCounter : MonoBehaviour
{
    public TMP_Text coinText;

    void Update()
    {
        if(coinText.text != Coin.totalCoin.ToString()){
            coinText.text = Coin.totalCoin.ToString();
        }
    }


}
