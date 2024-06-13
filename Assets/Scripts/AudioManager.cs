using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private bool musicState = true;
    public TextMeshProUGUI labelAudio;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void SetMusicState() {
        musicState = !musicState;
        if(musicState == true) {
            audioSource.Play();
            labelAudio.SetText("Music Off");
        }
        else {
            audioSource.Stop();
            labelAudio.SetText("Music On");
        }
    }
}
