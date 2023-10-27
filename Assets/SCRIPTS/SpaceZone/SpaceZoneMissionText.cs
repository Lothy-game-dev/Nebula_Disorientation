using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceZoneMissionText : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Black1;
    public GameObject Black2;
    public GameObject Black3;
    public GameObject Black4;
    public GameObject Text;
    #endregion
    #region NormalVariables
    private float Black1InitA;
    private float Black2InitA;
    private float Black3InitA;
    private float Black4InitA;
    private float TextInitA;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Black1InitA = Black1.GetComponent<SpriteRenderer>().color.a;
        Black2InitA = Black2.GetComponent<SpriteRenderer>().color.a;
        Black3InitA = Black3.GetComponent<SpriteRenderer>().color.a;
        Black4InitA = Black4.GetComponent<SpriteRenderer>().color.a;
        TextInitA = Text.GetComponent<TextMeshPro>().color.a;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Create Text
    // Group all function that serve the same algorithm
    public void CreateText(string strText, Color color)
    {
        Black2.SetActive(false);
        Black3.SetActive(false);
        Black4.SetActive(false);
        Text.SetActive(false);
        Text.GetComponent<TextMeshPro>().text = strText;
        Text.GetComponent<TextMeshPro>().color = color;
        Color c = Black1.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        Black1.GetComponent<SpriteRenderer>().color = c;
        Color c2 = Black2.GetComponent<SpriteRenderer>().color;
        c2.a = 0;
        Black2.GetComponent<SpriteRenderer>().color = c2;
        Color c3 = Black3.GetComponent<SpriteRenderer>().color;
        c3.a = 0;
        Black3.GetComponent<SpriteRenderer>().color = c3;
        Color c4 = Black4.GetComponent<SpriteRenderer>().color;
        c4.a = 0;
        Black4.GetComponent<SpriteRenderer>().color = c4;
        Color c5 = Text.GetComponent<TextMeshPro>().color;
        c5.a = 0;
        Text.GetComponent<TextMeshPro>().color = c5;
        Black1.SetActive(true);
        StartCoroutine(PlayText(strText));
              
    }

    private IEnumerator PlayText(string text)
    {
        for (int i=0;i<10;i++)
        {
            Color c = Black1.GetComponent<SpriteRenderer>().color;
            c.a += Black1InitA/10f;
            Black1.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        Black2.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            Color c = Black2.GetComponent<SpriteRenderer>().color;
            c.a += Black2InitA / 10f;
            Black2.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        Black3.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            Color c = Black3.GetComponent<SpriteRenderer>().color;
            c.a += Black3InitA / 10f;
            Black3.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        Black4.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            Color c = Black4.GetComponent<SpriteRenderer>().color;
            c.a += Black4InitA / 10f;
            Black4.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.02f);
        }
        Text.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            Color c = Text.GetComponent<TextMeshPro>().color;
            c.a += TextInitA / 10f;
            Text.GetComponent<TextMeshPro>().color = c;
            yield return new WaitForSeconds(0.02f);
        }

        if (text == "Mission Accomplished!")
        {
            FindAnyObjectByType<RankController>().CheckToRankUp();
            yield return new WaitForSeconds(2f);
            Black2.SetActive(false);
            Black3.SetActive(false);
            Black4.SetActive(false);
            Text.SetActive(false);
            FindAnyObjectByType<GameplayInteriorController>().SZSummaryOn();
        } else
        {
            yield return new WaitForSeconds(2f);
            PlayerPrefs.SetString("isFailed", "T");
            SceneManager.LoadSceneAsync("SessionSummary");
            SceneManager.UnloadSceneAsync("GameplayInterior");
        }
    }
    #endregion
}
