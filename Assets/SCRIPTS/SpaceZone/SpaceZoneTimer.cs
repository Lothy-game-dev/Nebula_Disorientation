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
    public GameplayInteriorController ControllerMain;
    #endregion
    #region NormalVariables
    public int Timer;
    public bool Countdown;
    public bool DoneSetupTimer;
    private float TimerDown;
    private bool AlreadyCallEnd;
    private bool GlowDown;
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
        if (DoneSetupTimer && !ControllerMain.IsInLoading)
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
        FindAnyObjectByType<StatisticController>().StageTime = timeInSecond;
        if (Timer>0)
        {
            Countdown = true;
        } else
        {
            Countdown = false;
        }
        TimerDown = 1f;
        SetTextTimer();
        DoneSetupTimer = true;
    }

    private void SetTextTimer()
    {
        int minute = (int)Timer / 60;
        int second = (int)Timer % 60;
        string Time = minute < 10 ? "0" + minute.ToString() + ":" + (second < 10 ? "0" + second : second.ToString()) : minute.ToString() + ":" + (second < 10 ? "0" + second : second.ToString());
        TimeText.text = Time;
    }

    private void CheckTimer()
    {
        if (Countdown)
        {
            if (Timer<=0f)
            {
                DoneSetupTimer = false;
                if (!AlreadyCallEnd)
                {
                    AlreadyCallEnd = true;
                    Color c = TimeText.color;
                    c.a = 1;
                    c.b = 1;
                    TimeText.color = c;
                    mission.TimerEnd();
                }
            }
            if (Timer<=10f)
            {
                Color c = TimeText.color;
                if (c.a>=1)
                {
                    GlowDown = true;
                } else if (c.a<=0)
                {
                    GlowDown = false;
                }
                if (GlowDown)
                {
                    c.a -= 2 * Time.deltaTime;
                } else
                {
                    c.a += 2 * Time.deltaTime;
                }
                c.r = 1;
                c.g = 1;
                c.b = 0;
                TimeText.color = c;
            }
        }
    }
    #endregion
}
