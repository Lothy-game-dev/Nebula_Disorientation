using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MainMenuSceneShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public Slider MasterVolumnSlider;
    public GameObject FPSText;
    public GameObject ResolText;
    public Slider MusicVolumnSlider;
    public Slider SFXSlider;
    public GameObject Notification;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public bool IsClicked;
    public bool IsSaved;
    public string FpsCounter;
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
        FpsCounter = FPSText.GetComponent<TextMeshPro>().text;

        MasterVolumnSlider.value = int.Parse(OptionSetting["MVolume"].ToString());
        Master = OptionSetting["MVolume"].ToString();

        ResolText.GetComponent<TextMeshPro>().text = OptionSetting["Resol"].ToString();
        Resol = OptionSetting["Resol"].ToString();

        MusicVolumnSlider.value = int.Parse(OptionSetting["MuVolume"].ToString());
        Music = OptionSetting["MuVolume"].ToString();

        SFXSlider.value = int.Parse(OptionSetting["Sfx"].ToString());
        Sound = OptionSetting["Sfx"].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClicked)
        {
            FPSText.GetComponent<TextMeshPro>().text = FpsCounter;
            ResolText.GetComponent<TextMeshPro>().text = Resol;
            IsClicked = false;
            Debug.Log(Application.targetFrameRate);
        }
        if (IsSaved)
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Save Successfully!", 5f);
            Application.targetFrameRate = int.Parse(FpsCounter);
            if ("FullScreen".Equals(Resol))
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            } else
            {
                if ("1920x1080".Equals(Resol))
                {
                    Screen.SetResolution(int.Parse(Resol.Substring(0, 4)), int.Parse(Resol.Substring(5, 4)), true);
                } else
                {
                    Screen.SetResolution(int.Parse(Resol.Substring(0, 4)), int.Parse(Resol.Substring(5, 3)), false);
                }
            }
            IsSaved = false;
        }
    }
    #endregion
    #region
    // Group all function that serve the same algorithm
    public void ValueChangeCheck()
    {
        Master = MasterVolumnSlider.value.ToString();
        Music = MusicVolumnSlider.value.ToString();
        Sound = SFXSlider.value.ToString();
        MasterVolumnSlider.transform.GetChild(3).GetComponent<TextMeshPro>().text = Mathf.RoundToInt(float.Parse(Master)).ToString() + "%";
        MusicVolumnSlider.transform.GetChild(3).GetComponent<TextMeshPro>().text = Mathf.RoundToInt(float.Parse(Music)).ToString() + "%";
        SFXSlider.transform.GetChild(3).GetComponent<TextMeshPro>().text = Mathf.RoundToInt(float.Parse(Sound)).ToString() + "%";
    }

    #endregion
    #region Start Anim
    // Group all function that serve the same algorithm
    public override void StartAnimation()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
