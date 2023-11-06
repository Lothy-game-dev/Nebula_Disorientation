using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSFXGeneratorController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public AudioClip EconomySpend;
    public AudioClip EconomyInsuff;
    public AudioClip LoadoutCons;
    public AudioClip LoadoutWeapon;
    public AudioClip LoadoutFighter;
    public AudioClip Repair;
    public AudioClip ButtonClick;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Dictionary<string, object> Data;
    public float SoundScale;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize variables
        SoundScale = 1f;
        GetComponent<SoundController>().CheckSoundVolumeByDB();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    /// <summary>
    /// Generate Sound
    /// </summary>
    /// <param name="Type">EconomySpend / EconomyInsuff / LoadoutCons / LoadoutWeapon / LoadoutFighter / Repair / ButtonClick</param>
    public void GenerateSound(string Type)
    {
        if (Type=="EconomySpend")
        {
            GetComponent<AudioSource>().volume = SoundScale;
            GetComponent<AudioSource>().clip = EconomySpend;
        } else if (Type== "EconomyInsuff")
        {
            GetComponent<AudioSource>().volume = SoundScale;
            GetComponent<AudioSource>().clip = EconomyInsuff;
        } else if (Type== "LoadoutCons")
        {
            GetComponent<AudioSource>().volume = SoundScale;
            GetComponent<AudioSource>().clip = LoadoutCons;
        } else if (Type== "LoadoutWeapon")
        {
            GetComponent<AudioSource>().volume = SoundScale;
            GetComponent<AudioSource>().clip = LoadoutWeapon;
        } else if (Type== "LoadoutFighter")
        {
            GetComponent<AudioSource>().volume = SoundScale;
            GetComponent<AudioSource>().clip = LoadoutFighter;
        } else if (Type=="Repair")
        {
            GetComponent<AudioSource>().volume = 0.5f * SoundScale;
            GetComponent<AudioSource>().clip = Repair;
        } else if (Type=="ButtonClick")
        {
            GetComponent<AudioSource>().volume = 0.5f * SoundScale;
            GetComponent<AudioSource>().clip = ButtonClick;
        }
        GetComponent<AudioSource>().Play();
    }

    public void SetSoundScaleInit(float scale)
    {
        SoundScale = scale;
    }
    #endregion
}
