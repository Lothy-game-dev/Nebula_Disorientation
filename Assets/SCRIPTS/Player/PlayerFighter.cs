using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : FighterShared
{
    #region ComponentVariables
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    public AudioClip MovingSound;
    public AudioClip DashSound;
    public AudioClip OverheatWarning;
    public AudioClip Overheated;
    public AudioSource DashAudioSource;
    #endregion
    #region NormalVariables
    private float testTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitializeFighter();
        aus = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFighter();
        if (testTimer <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                ReceiveThermalDamage(false);
                testTimer = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                ReceiveThermalDamage(true);
                testTimer = 1f;
            }
        } else
        {
            testTimer -= Time.deltaTime;
        }
        
    }
    #endregion
    #region Play Sound
    public void PlayMovingSound(float volume)
    {
        if (aus.clip!=MovingSound)
        {
            aus.clip = MovingSound;
            aus.loop = true;
            aus.Play();
        }
        aus.volume = volume;
    }

    public void StopSound()
    {
        aus.clip = null;
    }

    public void PlayDashSound()
    {
        DashAudioSource.clip = DashSound;
        DashAudioSource.loop = false;
        DashAudioSource.volume = 0.75f;
        DashAudioSource.Play();
    }
    #endregion
}
