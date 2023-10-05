using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceZoneTimer : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public SpaceZoneMission mission;
    public TextMeshPro TimeText;
    #endregion
    #region NormalVariables
    private int Timer;
    private bool Countdown;
    private bool DoneSetupTimer;
    private float TimerDown;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        DoneSetupTimer = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (DoneSetupTimer)
        {
            if (TimerDown<=0f)
            {
                TimerDown = 1f;
                if (Countdown)
                {
                    Timer -= 1;
                } else
                {
                    Timer += 1;
                }
                SetTextTimer();
            } else
            {
                TimerDown -= Time.deltaTime;
            }
            CheckTimer();
        }
    }
    #endregion
    #region Set Timer
    public void SetTimer(int timeInSecond)
    {
        Timer = timeInSecond;
        if (Timer>0)
        {
            Countdown = true;
        } else
        {
            Countdown = false;
        }
        TimerDown = 1f;
        DoneSetupTimer = true;
    }

    private void SetTextTimer()
    {
        int minute = (int)Timer / 60;
        int second = (int)Timer % 60;
        string Time = minute < 10 ? "0" + minute.ToString() + ":" + second.ToString() : minute.ToString() + ":" + second.ToString();
        TimeText.text = Time;
    }

    private void CheckTimer()
    {
        if (Countdown)
        {
            if (Timer==0f)
            {
                DoneSetupTimer = false;
                // Fail
            }
        }
    }
    #endregion
}
