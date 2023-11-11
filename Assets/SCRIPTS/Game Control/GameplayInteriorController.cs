using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayInteriorController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    public GameObject BlackFade;
    public GameObject Intro;
    public TextMeshPro Cash;
    public TextMeshPro Shard;
    public LOTWEffect LOTWEffect;
    public GameObject PauseMenu;
    public GameObject SZSummary;
    public GameObject LoadingScene;
    public GameObject Template;
    public GameSavedText SaveText;
    public GameObject HUDMain;
    public GameObject PlayerHUD;
    #endregion
    #region NormalVariables
    public bool IsInLoading;
    private float InitAPause1;
    private float InitAPause2;
    private float InitAPause3;
    private bool DoneLightUp;
    public float MasterVolumeScale;
    public float MusicVolumeScale;
    public float SFXVolumeScale;
    public int CurrentStageCash;
    public int CurrentStageShard;
    private Dictionary<string, object> OptionSetting;
    public bool isEnding;
    public bool isLoweringSound;
    public bool isPausing;
    public bool isHidingHUD;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    public int SessionID;
    void Start()
    {
        // Initialize variables
        GenerateLoadingScene(5f);
        SaveText.gameObject.SetActive(true);
        SaveText.FadingCountDown = 5f;
        SaveText.AlreadySetCountDown = true;
        GenerateBlackFadeOpen(transform.position, 5f, 0.5f);
        CurrentStageCash = 0;
        CurrentStageShard = 0;
        SetCashAndShard();
        Color c = PauseMenu.GetComponent<SpriteRenderer>().color;
        InitAPause1 = c.a;
        c.a = 0;
        PauseMenu.GetComponent<SpriteRenderer>().color = c;
        Color c1 = PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        InitAPause2 = c1.a;
        c1.a = 0;
        PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c1;
        Color c2 = PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        InitAPause3 = c2.a;
        c2.a = 0;
        PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = c2;
        OptionSetting = FindAnyObjectByType<AccessDatabase>().GetOption();
        MasterVolumeScale = float.Parse(OptionSetting["MVolume"].ToString());
        MusicVolumeScale = float.Parse(OptionSetting["MuVolume"].ToString());
        SFXVolumeScale = float.Parse(OptionSetting["Sfx"].ToString());
        aus = GetComponent<AudioSource>();
        isLoweringSound = false;
        SoundVolume();
        Camera.main.transparencySortAxis = new Vector3(0, 1, 0);


    }

    // Update is called once per frame
    void Update()
    {   
        // Call function and timer only if possible
        if (PauseMenu.activeSelf)
        {
            if (PauseMenu.GetComponent<SpriteRenderer>().color.a < InitAPause1)
            {
                Color c = PauseMenu.GetComponent<SpriteRenderer>().color;
                c.a += InitAPause1/60;
                PauseMenu.GetComponent<SpriteRenderer>().color = c;
            } else
            {
                if (!DoneLightUp)
                {
                    DoneLightUp = true;
                    PauseMenu.transform.GetChild(5).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(6).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(1).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(2).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(3).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(4).gameObject.SetActive(true);
                    PauseMenu.transform.GetChild(7).gameObject.SetActive(true);
                }
            }
            if (PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a < InitAPause2)
            {
                Color c = PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                c.a += InitAPause2/60;
                PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c;
            }
            if (PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color.a < InitAPause3)
            {
                Color c = PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
                c.a += InitAPause3/60;
                PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = c;
            }
        }
       
    }
    #endregion
    #region BlackFade
    public void SoundVolume()
    {
        aus.volume = MasterVolumeScale / 100f * MusicVolumeScale / 100f;
    }

    public void GenerateLoadingScene(float duration)
    {
        GameObject Load = Instantiate(LoadingScene,
            new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y, LoadingScene.transform.position.z), Quaternion.identity);
        Load.GetComponent<SpriteRenderer>().sortingOrder = 1000;
        Load.transform.SetParent(Camera.main.transform);
        Load.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1002;
        Load.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 1001;
        Load.transform.GetChild(0).GetComponent<LoadingScene>().LoadingTime = duration;
        Load.SetActive(true);
    }


    public void GenerateBlackFadeOpen(Vector2 pos, float delay, float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.transform.SetParent(transform);
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf,delay, duration));
    }

    private IEnumerator BlackFadeOpen(GameObject Fade, float delay, float duration)
    {
        IsInLoading = true;
        yield return new WaitForSeconds(delay);
        IsInLoading = false;
        FindAnyObjectByType<StatisticController>().StartTime = System.DateTime.Now;
        FindAnyObjectByType<StatisticController>().isStart = true;
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Intro.SetActive(true);
        Intro.GetComponent<SpaceZoneIntro>().RunAnimation();
        Destroy(Fade);
    }

    public void GenerateBlackFadeOpenNoDelay(Vector2 pos,  float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.transform.SetParent(transform);
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen2(bf, duration));
    }

    private IEnumerator BlackFadeOpen2(GameObject Fade, float duration)
    {
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }

    public void GenerateBlackFadeClose(float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(transform.position.x, transform.position.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeClose(bf, duration));
    }

    private IEnumerator BlackFadeClose(GameObject Fade, float duration)
    {
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a += 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        yield return new WaitForSeconds(2f);
        Destroy(Fade);
    }
    #endregion
    #region Cash And Shard
    public void SetCashAndShard()
    {
        Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        Cash.text = "<sprite index='3'> " + CurrentStageCash;
        Shard.text = "<sprite index='0'> " + CurrentStageShard;
        SessionID = (int)SessionData["SessionID"];
    }

    public void AddCashAndShard(int Cash, int Shard, GameObject go)
    {
        CurrentStageCash += (int)(Cash * LOTWEffect.LOTWCashIncScale);
        CurrentStageShard += Shard;
        FindAnyObjectByType<StatisticController>().PriceCollected((int)(Cash * LOTWEffect.LOTWCashIncScale), Shard);
        ShowCashAndShardEffect((int)(Cash * LOTWEffect.LOTWCashIncScale), Shard, go);
        SetCashAndShard();
    }

    public void FinalUpdateCashShard()
    {
        FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShard(SessionID, true, CurrentStageCash, CurrentStageShard);
    }

    

    public void ShowCashAndShardEffect(int CashVal, int ShardVal, GameObject go)
    {
        if (CashVal > 0)
        {
            GameObject CashUp = Instantiate(Template, go.transform.position, Quaternion.identity);
            /*CashUp.transform.SetParent(Cash.transform);*/
            CashUp.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + CashVal + " <sprite index='3'/>" + (ShardVal > 0 ? " +" + ShardVal + " <sprite index='0'/>" : "");
            CashUp.transform.localScale = new Vector3(100, 100, 100f);
            CashUp.SetActive(true);
            CashUp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 20f);
            
            Destroy(CashUp, 2f);
        }
    }
    #endregion
    #region Pause
    public void PauseMenuOn()
    {
        PauseMenu.SetActive(true);
        DoneLightUp = false;
        isPausing = true;
    }

    public void PauseMenuOff()
    {
        PauseMenu.SetActive(false);
        PauseMenu.transform.GetChild(1).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(2).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(3).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(4).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(5).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(6).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(7).gameObject.SetActive(false);
        Color c = PauseMenu.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        PauseMenu.GetComponent<SpriteRenderer>().color = c;
        Color c1 = PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        c1.a = 0;
        PauseMenu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c1;
        Color c2 = PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
        c2.a = 0;
        PauseMenu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = c2;
        isPausing = false;
    }
    #endregion
    #region Hide HUD
    public void HideHUD()
    {
        if (isHidingHUD)
        {
            isHidingHUD = false;
            HUDMain.SetActive(true);
            PlayerHUD.SetActive(true);
        } else
        {
            isHidingHUD = true;
            HUDMain.SetActive(false);
            PlayerHUD.SetActive(false);
        }
    }
    #endregion
    #region SpaceZone Summary Menu
    public void SZSummaryOn()
    {      
        SZSummary.SetActive(true);
        SZSummary.GetComponent<SpaceZoneSummary>().DoneLightUp2 = false;
        SZSummary.GetComponent<SpaceZoneSummary>().Summarize();
    }
    #endregion
}
