using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject button;

    // Start is called before the first frame update
    void Start(){
        button.SetActive(false);
    }

    public void appear(){
        button.SetActive(true);
    }

    public void backMainMenu() {
        Debug.Log("Here in button");
		SceneManager.LoadScene("Menu");
	}
}
