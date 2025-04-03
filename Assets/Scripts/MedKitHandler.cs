using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKitHandler : MonoBehaviour
{
    public ParticleSystem DestroyParticle;

    public AudioSource DestroySound;

    public int HealthPointCount;

    private GameManager _GameManager;

    private void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_GameManager.Healing(HealthPointCount))
        {
            DestroyParticle.transform.parent = null;
            DestroySound.transform.parent = null;
            DestroyParticle.Play();
            DestroySound.Play();
            Destroy(gameObject);
        }
        
    }
}
