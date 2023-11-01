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
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
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
    /// <param name="Type">EconomySpend / EconomyInsuff / LoadoutCons / LoadoutWeapon / LoadoutFighter</param>
    public void GenerateSound(string Type)
    {
        if (Type=="EconomySpend")
        {
            GetComponent<AudioSource>().clip = EconomySpend;
        } else if (Type== "EconomyInsuff")
        {
            GetComponent<AudioSource>().clip = EconomyInsuff;
        } else if (Type== "LoadoutCons")
        {
            GetComponent<AudioSource>().clip = LoadoutCons;
        } else if (Type== "LoadoutWeapon")
        {
            GetComponent<AudioSource>().clip = LoadoutWeapon;
        } else if (Type== "LoadoutFighter")
        {
            GetComponent<AudioSource>().clip = LoadoutFighter;
        }
        GetComponent<AudioSource>().Play();
    }
    #endregion
}
