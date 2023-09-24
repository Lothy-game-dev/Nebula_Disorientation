using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : FighterShared
{
    #region ComponentVariables
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    public GameObject FighterInfoCircle;
    public GameObject FighterTempColor;
    public GameObject FighterTempText;
    public Slider HPSlider;
    public GameObject HPText;
    public Slider BRSlider;
    public GameObject BRText;
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
        HPSlider.maxValue = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFighter();
        // For thermal test only, increase temp by pressing N and decrease by pressing M
        // Will be deleted
        if (testTimer <= 0f)
        {
            if (Input.GetKey(KeyCode.M))
            {
                ReceiveThermalDamage(false);
                testTimer = 0.1f;
            }
            else if (Input.GetKey(KeyCode.N))
            {
                ReceiveThermalDamage(true);
                testTimer = 0.1f;
            }
        } else
        {
            testTimer -= Time.deltaTime;
        }
        ShowInfo();
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
    #region Show Information To HUD
    private void ShowInfo()
    {
        // Temp
        FighterTempText.GetComponent<TextMeshPro>().text = currentTemperature.ToString() + "°";
        FighterInfoCircle.GetComponent<HUDFighterInfoEffects>().CurrentTemp = currentTemperature;
        if (currentTemperature==50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 158 / 255f);
        } else if (currentTemperature>50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0,1,0, 158 / 255f), new Color(1, 0, 0, 200 / 255f), (currentTemperature-50)/50f);
        } else if (currentTemperature<50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0, 1, 1, 200 / 255f), new Color(0, 1, 0, 158 / 255f), (currentTemperature) / 50f);
        }
        // HP
        HPSlider.value = CurrentHP;
        HPText.GetComponent<TextMeshPro>().text = CurrentHP + " / " + MaxHP;
        // BR Pending
    }
    #endregion
}
