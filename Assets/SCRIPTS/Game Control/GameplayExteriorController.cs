using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayExteriorController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject BlackFade;
    public GameObject LOTWScene;
    public GameObject UECScene;
    public GameObject LoadingScene;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        Time.timeScale = 1;
        InitTeleport();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate Black Fade
    public void GenerateBlackFadeOpen(Vector2 pos, float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf, duration));
    }

    public void GenerateLoadingScene(float duration)
    {
        GameObject Load = Instantiate(LoadingScene,
            new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, LoadingScene.transform.position.z), Quaternion.identity);
        Load.GetComponent<SpriteRenderer>().sortingOrder = 1000;
        Load.transform.SetParent(Camera.main.transform);
        Load.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1002;
        Load.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 1001;
        Load.transform.GetChild(0).GetComponent<LoadingScene>().LoadingTime = duration;
        Load.SetActive(true);
    }


    private IEnumerator BlackFadeOpen(GameObject Fade, float duration)
    {
        for (int i=0;i<50;i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1/50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }

    public void GenerateBlackFadeClose(float duration, float wait)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(transform.position.x, transform.position.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeClose(bf, duration, wait));
    }

    public void InitTeleport()
    {
        if (PlayerPrefs.GetString("InitTeleport")!="")
        {
            if (PlayerPrefs.GetString("InitTeleport")=="LOTW")
            {
                ChangeToScene(LOTWScene);
                GenerateBlackFadeOpen(LOTWScene.transform.position, 2f);
            } else if (PlayerPrefs.GetString("InitTeleport")=="UEC")
            {
                ChangeToScene(UECScene);
                GenerateLoadingScene(2f);
            }
            PlayerPrefs.SetString("InitTeleport","");
        }
    }

    private IEnumerator BlackFadeClose(GameObject Fade, float duration, float wait)
    {
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a += 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        yield return new WaitForSeconds(wait);
        Destroy(Fade);
    }

    public void ChangeToScene(GameObject Scene)
    {
        Camera.main.transform.position = new Vector3(Scene.transform.position.x,Scene.transform.position.y,Camera.main.transform.position.z);
        if (!Scene.activeSelf)
        {
            Scene.SetActive(true);
        }
        Scene.GetComponent<BackgroundBrieflyMoving>().enabled = true;
    }
    #endregion
}
