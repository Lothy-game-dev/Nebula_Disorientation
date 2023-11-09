using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuOption : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public Slider MasterVolumnSlider;
    public GameObject FPSText;
    public GameObject ResolText;
    public Slider MusicVolumnSlider;
    public Slider SFXSlider;
    public GameplayInteriorController Interior;
    public SoundSFXGeneratorController SFXGen;
    public GameObject FuelCell;
    #endregion
    #region NormalVariables
    public string Fps;
    public string Resol;
    public string Master;
    public string Music;
    public string Sound;
    private Dictionary<string, object> OptionSetting;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        OptionSetting = FindAnyObjectByType<AccessDatabase>().GetOption();
        // Initialize variables
        MasterVolumnSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        MusicVolumnSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        SFXSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        FPSText.GetComponent<TextMeshPro>().text = OptionSetting["Fps"].ToString();
        Fps = FPSText.GetComponent<TextMeshPro>().text;

        MasterVolumnSlider.value = int.Parse(OptionSetting["MVolume"].ToString());
        Master = OptionSetting["MVolume"].ToString();

        ResolText.GetComponent<TextMeshPro>().text = OptionSetting["Resol"].ToString();
        Resol = OptionSetting["Resol"].ToString();

        MusicVolumnSlider.value = int.Parse(OptionSetting["MuVolume"].ToString());
        Music = OptionSetting["MuVolume"].ToString();

        SFXSlider.value = int.Parse(OptionSetting["Sfx"].ToString());
        Sound = OptionSetting["Sfx"].ToString();

        // Set data to fuel cell bar
        Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>()
            .GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)ListData["FuelCell"];
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region On Value Change
    // Group all function that serve the same algorithm
    public void ValueChangeCheck()
    {
        Master = MasterVolumnSlider.value.ToString();
        Interior.MasterVolumeScale = float.Parse(Master);
        Music = MusicVolumnSlider.value.ToString();
        Interior.MusicVolumeScale = float.Parse(Music);
        Sound = SFXSlider.value.ToString();
        Interior.SFXVolumeScale = float.Parse(Sound);
        Interior.SoundVolume();
        SFXGen.SoundScale = float.Parse(Master) / 100f * float.Parse(Sound) / 100f;
    }

    public void SaveData()
    {
        FindAnyObjectByType<AccessDatabase>().UpdateOptionSetting(Mathf.RoundToInt(float.Parse(Master)),
                                    Mathf.RoundToInt(float.Parse(Music)), Mathf.RoundToInt(float.Parse(Sound)), 
                                    Mathf.RoundToInt(float.Parse(Fps)), Resol);
        Application.targetFrameRate = int.Parse(Fps);
        if ("fullscreen".Equals(Resol.ToLower()))
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            if ("1920x1080".Equals(Resol))
            {
                Screen.SetResolution(int.Parse(Resol.Substring(0, 4)), int.Parse(Resol.Substring(5, 4)), true);
            }
            else
            {
                Screen.SetResolution(int.Parse(Resol.Substring(0, 4)), int.Parse(Resol.Substring(5, 3)), false);
            }
        }
    }
    #endregion
}
