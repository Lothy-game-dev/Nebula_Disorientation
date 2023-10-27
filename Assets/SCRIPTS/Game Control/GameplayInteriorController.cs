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
    #endregion
    #region NormalVariables
    public bool IsInLoading;
    private float InitAPause1;
    private float InitAPause2;
    private float InitAPause3;
    private bool DoneLightUp;
    private bool DoneLightUp2;
    private float InitASummary;
    public float MasterVolumeScale;
    public float MusicVolumeScale;
    public float SFXVolumeScale;
    private Dictionary<string, object> OptionSetting;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    public int SessionID;
    void Start()
    {
        // Initialize variables
        GenerateBlackFadeOpen(transform.position, 5f, 1f);
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
        SoundVolume();

        Color c3 = SZSummary.GetComponent<SpriteRenderer>().color;
        InitASummary = c3.a;
        c3.a = 0;
        SZSummary.GetComponent<SpriteRenderer>().color = c3;
        
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

        if (SZSummary.activeSelf)
        {
            if (SZSummary.GetComponent<SpriteRenderer>().color.a < InitASummary)
            {
                Color c = SZSummary.GetComponent<SpriteRenderer>().color;
                c.a += InitASummary / 60;
                SZSummary.GetComponent<SpriteRenderer>().color = c;
            }
            else
            {
                if (!DoneLightUp2)
                {
                    DoneLightUp2 = true;
                    SZSummary.transform.GetChild(0).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(1).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(2).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(3).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(4).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(5).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(6).gameObject.SetActive(true);
                    SZSummary.transform.GetChild(7).gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion
    #region BlackFade
    public void SoundVolume()
    {
        aus.volume = MasterVolumeScale / 100f * MusicVolumeScale / 100f;
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
        Destroy(Fade);
    }
    #endregion
    #region Cash And Shard
    public void SetCashAndShard()
    {
        Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        Cash.text = "<sprite index='3'> " + (int)SessionData["SessionCash"];
        Shard.text = "<sprite index='0'> " + (int)SessionData["SessionTimelessShard"];
        SessionID = (int)SessionData["SessionID"];
    }

    public void AddCashAndShard(int Cash, int Shard)
    {
        FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShard(SessionID, true, (int)(Cash * LOTWEffect.LOTWCashIncScale), Shard);
        SetCashAndShard();
    }

    public void PauseMenuOn()
    {
        PauseMenu.SetActive(true);
        DoneLightUp = false;
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
    }
    #endregion
    #region SpaceZone Summary Menu
    public void SZSummaryOn()
    {
        SZSummary.SetActive(true);
        DoneLightUp2 = false;
        SZSummary.GetComponent<SpaceZoneSummary>().Summarize();
    }
    #endregion
}
