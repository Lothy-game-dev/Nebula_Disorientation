using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string SoundType;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize variables
        CheckSoundVolumeByDB();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check Sound Volume
    // Group all function that serve the same algorithm
    public void CheckSoundVolumeByDB()
    {
        Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetOption();
        if (SoundType == "Music")
        {
            GetComponent<AudioSource>().volume = (int)Data["MVolume"] / 100f * (int)Data["MuVolume"] / 100f;
        } else if (SoundType=="SFX")
        {
            GetComponent<AudioSource>().volume = (int)Data["MVolume"] / 100f * (int)Data["Sfx"] / 100f;
        }
    }

    public void CheckSoundVolumeByNumber(float MasterVolume, float MusicVolume, float SFXVolume)
    {
        if (SoundType == "Music")
        {
            GetComponent<AudioSource>().volume = MasterVolume / 100f * MusicVolume / 100f;
        }
        else if (SoundType == "SFX")
        {
            GetComponent<AudioSource>().volume = MasterVolume / 100f * SFXVolume / 100f;
        }
    }
    #endregion
}
