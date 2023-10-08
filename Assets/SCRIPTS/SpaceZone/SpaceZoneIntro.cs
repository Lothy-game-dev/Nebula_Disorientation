using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceZoneIntro : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NameText;
    public GameObject MissionText;
    public GameObject PressText;
    #endregion
    #region NormalVariables
    private float InitScaleX;
    private float InitScaleY;
    private bool alreadyClose;
    private float AutoCloseTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        NameText.SetActive(false);
        Color c = NameText.GetComponent<TextMeshPro>().color;
        c.a = 0;
        NameText.GetComponent<TextMeshPro>().color = c;

        MissionText.SetActive(false);
        Color c1 = MissionText.GetComponent<TextMeshPro>().color;
        c1.a = 0;
        MissionText.GetComponent<TextMeshPro>().color = c1;

        PressText.SetActive(false);
        Color c2 = PressText.GetComponent<TextMeshPro>().color;
        c2.a = 0;
        PressText.GetComponent<TextMeshPro>().color = c2;
        InitScaleX = transform.localScale.x;
        InitScaleY = transform.localScale.y;
        transform.localScale = new Vector2(InitScaleX / 10, InitScaleY / 100);
        alreadyClose = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (!alreadyClose)
        {
            if (Input.anyKey)
            {
                alreadyClose = true;
                StartCoroutine(CloseAnimation());
            }
            if (AutoCloseTimer>0f)
            {
                AutoCloseTimer -= Time.deltaTime;
            } else
            {
                alreadyClose = true;
                StartCoroutine(CloseAnimation());
            }
        }

    }
    #endregion
    #region Set Data
    // Group all function that serve the same algorithm
    public void SetData(int SpaceZoneNo, string Type, string Mission, string color)
    {
        NameText.GetComponent<TextMeshPro>().text = "<color=" + color + ">Space Zone No." + SpaceZoneNo + " - " + Type + "</color>";
        MissionText.GetComponent<TextMeshPro>().text = Mission;
    }

    public void RunAnimation()
    {
        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        for (int i=0;i<9;i++)
        {
            transform.localScale = new Vector2(transform.localScale.x + InitScaleX / 10, transform.localScale.y);
            yield return new WaitForSeconds(0.02f);
        }
        for (int i=0;i<9;i++)
        {
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + InitScaleY*11f/100);
            yield return new WaitForSeconds(0.05f);
        }
        NameText.SetActive(true);
        MissionText.SetActive(true);
        PressText.SetActive(true);
        for (int i=0;i<10;i++)
        {
            Color c = NameText.GetComponent<TextMeshPro>().color;
            c.a += 0.1f;
            NameText.GetComponent<TextMeshPro>().color = c;
            Color c1 = MissionText.GetComponent<TextMeshPro>().color;
            c1.a += 0.1f;
            MissionText.GetComponent<TextMeshPro>().color = c1;
            Color c2 = PressText.GetComponent<TextMeshPro>().color;
            c2.a += 0.1f;
            PressText.GetComponent<TextMeshPro>().color = c2;
            yield return new WaitForSeconds(0.05f);
        }
        AutoCloseTimer = 3f;
        alreadyClose = false;
    }

    private IEnumerator CloseAnimation()
    {
        NameText.SetActive(false);
        MissionText.SetActive(false);
        PressText.SetActive(false);
        for (int i = 0; i < 9; i++)
        {
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - InitScaleY * 11f / 100);
            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 0; i < 9; i++)
        {
            transform.localScale = new Vector2(transform.localScale.x - InitScaleX / 10, transform.localScale.y);
            yield return new WaitForSeconds(0.02f);
        }
        gameObject.SetActive(false);
    }
    #endregion
}
