using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public AudioClip hit, death, heal;
    public AudioSource src;

    public void PlaySound(string clip){
        switch(clip) {
            case "hit": 
                src.PlayOneShot(hit);
                break;
            case "death": 
                src.PlayOneShot(death);
                break;
            case "heal": 
                src.PlayOneShot(heal);
                break;
        }
    }
}
