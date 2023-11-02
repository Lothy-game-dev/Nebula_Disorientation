using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySoundSFXController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public AudioClip BarrierHit;
    public AudioClip FighterHit;
    public AudioClip WSSSHit;
    public AudioClip FighterExplo;
    public AudioClip WSExplo;
    public AudioClip SSExplo;
    public AudioClip OtherExplo;
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
    /// <param name="Type">BarrierHit / FighterHit / WSSSHit / FighterExplo / WSExplo / SSExplo / OtherExplo</param>
    public void GenerateSound(string Type, GameObject go)
    {
        if (Type == "BarrierHit")
        {
            go.GetComponent<AudioSource>().clip = BarrierHit;
        }
        else if (Type == "FighterHit")
        {
            go.GetComponent<AudioSource>().clip = FighterHit;
        }
        else if (Type == "WSSSHit")
        {
            go.GetComponent<AudioSource>().clip = WSSSHit;
        }
        else if (Type == "FighterExplo")
        {
            Debug.Log("ExploF");
            go.GetComponent<AudioSource>().clip = FighterExplo;
        }
        else if (Type == "WSExplo")
        {
            go.GetComponent<AudioSource>().clip = WSExplo;
        }
        else if (Type == "SSExplo")
        {
            go.GetComponent<AudioSource>().clip = SSExplo;
        }
        else if (Type == "OtherExplo")
        {
            go.GetComponent<AudioSource>().clip = OtherExplo;
        }
        go.GetComponent<AudioSource>().Play();
    }
    #endregion
}
