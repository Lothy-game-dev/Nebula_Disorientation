using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayInteriorController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject BlackFade;
    public GameObject Intro;
    public TextMeshPro Cash;
    public TextMeshPro Shard;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    public int SessionID;
    void Start()
    {
        // Initialize variables
        GenerateBlackFadeOpen(transform.position, 0f, 1f);
        SetCashAndShard();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region BlackFade
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
        yield return new WaitForSeconds(delay);
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
        FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShard(SessionID, true, Cash, Shard);
        SetCashAndShard();
    }
    #endregion
}
