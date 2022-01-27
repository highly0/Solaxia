using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShuffle : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource src;
    // Start is called before the first frame update
    void Start(){
        src.loop = false;
    }

    private AudioClip RandomClip() {
        return clips[Random.Range(0, clips.Length)];
    }
    // Update is called once per frame
    void Update()
    {
        if(!src.isPlaying) {
            src.clip = RandomClip();
            src.Play();
        }
        
    }
}
