using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PilotContract : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject[] EULAList;
    public GameObject LeftButton;
    public GameObject RightButton;
    public GameObject TOSButton;
    public GameObject counter;
    #endregion
    #region NormalVariables
    public int currentPages;
    private float InitX;
    private float InitY;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        currentPages = 0;
        InitX = transform.localScale.x;
        InitY = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x / 5f, transform.localScale.y / 5f, transform.localScale.z);
        EULAList[0].SetActive(false);
        StartCoroutine(PlayAnim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region Pages
    public void NextPage()
    {
        if (currentPages >= 1 && currentPages <= EULAList.Length-2)
        {
            EULAList[currentPages].SetActive(false);
            currentPages++;
            EULAList[currentPages].SetActive(true);
        } else if (currentPages ==0)
        {
            EULAList[0].SetActive(false);
            currentPages=1;
            EULAList[1].SetActive(true);
        }
        CheckLeftRight();
    }
    public void PreviousPage()
    {
        if (currentPages >= 2 && currentPages <= EULAList.Length - 1)
        {
            EULAList[currentPages].SetActive(false);
            currentPages--;
            EULAList[currentPages].SetActive(true);
        }
        CheckLeftRight();
    }
    public void CheckLeftRight()
    {
        if (currentPages>=1 && currentPages <= EULAList.Length - 2)
        {
            RightButton.SetActive(true);
        } else
        {
            RightButton.SetActive(false);
        }
        if (currentPages >= 2 && currentPages <= EULAList.Length-1)
        {
            LeftButton.SetActive(true);
        }
        else
        {
            LeftButton.SetActive(false);
        }
        if (currentPages >= 2 && currentPages <= EULAList.Length - 2)
        {
            RightButton.GetComponent<PilotContractButton>().ResetPosLeftRight();
            LeftButton.GetComponent<PilotContractButton>().ResetPosLeftRight();
        }
        if (currentPages >= 1 && currentPages <= EULAList.Length-1)
        {
            if (!counter.activeSelf)
            {
                counter.SetActive(true);
            }
            TextMeshPro tm = counter.GetComponent<TextMeshPro>();
            if (tm!=null)
            {
                tm.text = currentPages.ToString() + "/" + (EULAList.Length - 1).ToString();
            }
        } else
        {
            if (counter.activeSelf)
            {
                counter.SetActive(false);
            }
        }
    }
    #endregion
    #region Animation
    private IEnumerator PlayAnim()
    {
        for (int i=0;i<4;i++)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + InitY / 5f, transform.localScale.z);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i <4; i++)
        {
            transform.localScale = new Vector3(transform.localScale.x + InitX / 5f, transform.localScale.y, transform.localScale.z);
            yield return new WaitForSeconds(0.1f);
        }
        EULAList[0].SetActive(true);
        TOSButton.SetActive(true);
    }
    #endregion
}
