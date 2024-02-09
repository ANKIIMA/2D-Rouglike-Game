using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallAudio : MonoBehaviour
{

    public AudioSource aud;
    void Start()
    {
        aud = GetComponent<AudioSource>();
        aud.Play();
    }
}
