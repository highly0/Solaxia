using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class CoinManagement : MonoBehaviour{
    public Text coinsText;
    public Text warningText;
    public int numCoins = 10;
    public int earn = 0;

    void Start() {
        numCoins = PlayerPrefs.GetInt("coins");
        coinsText.text = "Coins: " + numCoins.ToString();
    }

    void Update(){
        coinsText.text = "Coins: " + numCoins.ToString(); 
    }

    // only enter the earn version if we have enough money
    public void Validate(){
        if(numCoins > 5) {
            numCoins -= 5;
            PlayerPrefs.SetInt("coins", numCoins);
            earn = 1;
            PlayerPrefs.SetInt("earn", earn);
            SceneManager.LoadScene("Main"); 
        } else {
            warningText.text = "Not enough coins for this mode!";
        }
    }

    public void addCoins(){
        numCoins += 10;
    }

    public void FreeMode(){
        earn = 0;
        SceneManager.LoadScene("Main");
    }
}
