using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour
{
    public AudioSource audioSource;

    void OnCollisionEnter(Collision hit)
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log("A collision happened , sound must be played");
        audioSource.Play();
    }
}
