using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : FighterShared
{
    #region ComponentVariables
    private AudioSource aus;
    #endregion
    #region InitializeVariables
    public GameObject FighterInfoCircle;
    public GameObject FighterTempColor;
    public GameObject FighterTempText;
    public Slider HPSlider;
    public GameObject HPText;
    public Slider BRSlider;
    public GameObject BRText;
    public GameObject[] PowerAndConsCDImage;
    public Slider[] PowerAndConsDurationSlider;
    public GameObject[] PowerAndConsCDText;
    public GameObject[] ChargeImage;
    public GameObject[] ConsCountText;
    public AudioClip MovingSound;
    public AudioClip DashSound;
    public AudioClip OverheatWarning;
    public AudioClip Overheated;
    public AudioClip SpawnSoundEffect;
    public AudioSource DashAudioSource;
    public float TotalHealing;
    public GameplayInteriorController ControllerMain;
    #endregion
    #region NormalVariables
    private float[] PowerAndConsCD;
    private float[] PowerAndConsCDTimer;
    private float[] PowerAndConsDuration;
    private float[] PowerAndConsDurationTimer;
    private bool[] PowerAndConsActivation;
    private float[] ChargingPower;
    private float[] ChargingPowerReq;
    private int[] ConsCount;
    private bool isPausing;
    private float testTimer;
    public GameObject FirstPower;
    public GameObject SecondPower;
    public Dictionary<string, int> Consumables;
    public List<GameObject> ConsumableObject;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitializeFighter();
        aus = GetComponent<AudioSource>();
        PlaySpawnSound();
        HPSlider.maxValue = MaxHP;
        ShieldPassive();
        BRSlider.maxValue = MaxBarrier;
        // temp, will get real data later
        PowerAndConsCD = new float[6] {1f, 1f, 5f, 5f, 5f, 5f};
        PowerAndConsCDTimer = new float[6] {0f, 0f, 0f, 0f, 0f, 0f};
        PowerAndConsDuration = new float[6] {1f, 0.5f, 3f, 3f, 3f, 3f};
        PowerAndConsDurationTimer = new float[6];
        PowerAndConsActivation = new bool[6];
        ChargingPower = new float[2];
        ChargingPowerReq = new float[2] { 1.14f, 1.14f };
        ConsCount = new int[4] { 10, 5, 2, 1 };
        SetConsumableCount();
        SetCDandDuration();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFighter();
        // For thermal test only, increase temp by pressing N and decrease by pressing M
        // Will be deleted
        if (testTimer <= 0f)
        {
            if (Input.GetKey(KeyCode.M))
            {
                ReceiveThermalDamage(false);
                testTimer = 0.1f;
            }
            else if (Input.GetKey(KeyCode.N))
            {
                ReceiveThermalDamage(true);
                testTimer = 0.1f;
            }
        } else
        {
            testTimer -= Time.deltaTime;
        }
        if (!ControllerMain.IsInLoading)
        {
            // Power Usage
            // 1st Power
            if (FirstPower != null && PowerAndConsCDTimer[0] <= 0f && !isPausing && !isFrozen && !isSFBFreeze)
            {
                // check if power does not need charge
                //
                FirstPower.GetComponent<Powers>().Fighter = gameObject;
                if (!FirstPower.name.Contains("LaserBeam"))
                {
                    PowerAndConsDurationSlider[0].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                    if (Input.GetKeyDown(KeyCode.Q) && !PowerAndConsActivation[0])
                    {
                        Debug.Log("Activate 1st power");
                        PowerAndConsActivation[0] = true;
                        // void function to activate power
                        FirstPower.GetComponent<Powers>().ActivatePower(FirstPower.name.Replace("(clone)", ""));
                        //Duration
                        PowerAndConsDurationTimer[0] = PowerAndConsDuration[0];
                        PowerAndConsDurationSlider[0].maxValue = PowerAndConsDuration[0];
                        PowerAndConsDurationSlider[0].value = PowerAndConsDuration[0];

                        //Cooldown
                        PowerAndConsCDTimer[0] = PowerAndConsCD[0];
                    }
                }
                // if power need charge
                //
                else
                {
                    if (Input.GetKey(KeyCode.Q) && !PowerAndConsActivation[0])
                    {

                        PowerAndConsDurationSlider[0].maxValue = ChargingPowerReq[0];
                        if (ChargingPower[0] < ChargingPowerReq[0])
                        {
                            if (ChargingPower[0] == 0)
                            {
                                FirstPower.GetComponent<Powers>().BeforeActivating();
                            }
                            ChargingPower[0] += Time.deltaTime;
                            Color c = ChargeImage[0].GetComponent<SpriteRenderer>().color;
                            c.a += 190 * Time.deltaTime / (255f * ChargingPowerReq[0]);
                            ChargeImage[0].GetComponent<SpriteRenderer>().color = c;

                        }
                        else
                        {
                            Debug.Log("Activate 1st charging power");
                            PowerAndConsActivation[0] = true;
                            ChargingPower[0] = 0;
                            Color c = ChargeImage[0].GetComponent<SpriteRenderer>().color;
                            c.a = 0;
                            ChargeImage[0].GetComponent<SpriteRenderer>().color = c;
                            // Void function to activate power

                            FirstPower.GetComponent<Powers>().ActivatePower(FirstPower.name.Replace("(clone)", ""));

                            //Duration
                            PowerAndConsDurationTimer[0] = PowerAndConsDuration[0];
                            PowerAndConsDurationSlider[0].fillRect.GetComponentInChildren<Image>().color =
                                new Color(0, 95 / 255f, 1, 149 / 255f);
                            PowerAndConsDurationSlider[0].maxValue = PowerAndConsDuration[0];
                            PowerAndConsDurationSlider[0].value = PowerAndConsDuration[0];

                            //Cooldown
                            PowerAndConsCDTimer[0] = PowerAndConsCD[0];
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Q) && !PowerAndConsActivation[0])
                    {
                        ChargingPower[0] = 0;
                        Color c = ChargeImage[0].GetComponent<SpriteRenderer>().color;
                        c.a = 0;
                        ChargeImage[0].GetComponent<SpriteRenderer>().color = c;
                        FirstPower.GetComponent<Powers>().DestroyChargingAnimation();
                    }
                }
            }
            // 2nd Power
            if (SecondPower != null && PowerAndConsCDTimer[1] <= 0f && !isPausing && !isFrozen && !isSFBFreeze)
            {
                SecondPower.GetComponent<Powers>().Fighter = gameObject;
                // check if power does not need charge
                //
                if (!SecondPower.name.Contains("LaserBeam"))
                {
                    PowerAndConsDurationSlider[1].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                    if (Input.GetKeyDown(KeyCode.E) && !PowerAndConsActivation[1])
                    {
                        Debug.Log("Activate 2nd power");
                        PowerAndConsActivation[1] = true;
                        // void function to activate power
                        SecondPower.GetComponent<Powers>().ActivatePower(SecondPower.name.Replace("(clone)", ""));

                        //Duration
                        PowerAndConsDurationTimer[1] = PowerAndConsDuration[1];
                        PowerAndConsDurationSlider[1].maxValue = PowerAndConsDuration[1];
                        PowerAndConsDurationSlider[1].value = PowerAndConsDuration[1];

                        //Cooldown
                        PowerAndConsCDTimer[1] = PowerAndConsCD[1];
                    }
                }
                // if power need charge
                //
                else
                {
                    if (Input.GetKey(KeyCode.E) && !PowerAndConsActivation[1])
                    {
                        PowerAndConsDurationSlider[1].maxValue = ChargingPowerReq[1];
                        if (ChargingPower[1] < ChargingPowerReq[1])
                        {
                            if (ChargingPower[1] == 0)
                            {
                                SecondPower.GetComponent<Powers>().BeforeActivating();
                            }
                            ChargingPower[1] += Time.deltaTime;
                            Color c = ChargeImage[1].GetComponent<SpriteRenderer>().color;
                            c.a += 190 * Time.deltaTime / (255f * ChargingPowerReq[1]);
                            ChargeImage[1].GetComponent<SpriteRenderer>().color = c;
                        }
                        else
                        {
                            Debug.Log("Activate 1st charging power");
                            PowerAndConsActivation[1] = true;
                            ChargingPower[1] = 0;
                            Color c = ChargeImage[1].GetComponent<SpriteRenderer>().color;
                            c.a = 0;
                            ChargeImage[1].GetComponent<SpriteRenderer>().color = c;
                            // Void function to activate power
                            SecondPower.GetComponent<Powers>().ActivatePower(SecondPower.name.Replace("(clone)", ""));
                            //Duration
                            PowerAndConsDurationTimer[1] = PowerAndConsDuration[1];
                            PowerAndConsDurationSlider[1].fillRect.GetComponentInChildren<Image>().color =
                                new Color(0, 95 / 255f, 1, 149 / 255f);
                            PowerAndConsDurationSlider[1].maxValue = PowerAndConsDuration[1];
                            PowerAndConsDurationSlider[1].value = PowerAndConsDuration[1];

                            //Cooldown
                            PowerAndConsCDTimer[1] = PowerAndConsCD[1];
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.E) && !PowerAndConsActivation[1])
                    {
                        ChargingPower[1] = 0;
                        Color c = ChargeImage[1].GetComponent<SpriteRenderer>().color;
                        c.a = 0;
                        ChargeImage[1].GetComponent<SpriteRenderer>().color = c;
                        SecondPower.GetComponent<Powers>().DestroyChargingAnimation();
                    }
                }
            }
            // Consumable Usage
            // 1st Cons
            if (ConsumableObject.Count > 0 && ConsumableObject[0] != null && PowerAndConsCDTimer[2] <= 0f && ConsCount[0] > 0 && !isPausing)
            {
                PowerAndConsDurationSlider[2].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                if (Input.GetKeyDown(KeyCode.Alpha1) && !PowerAndConsActivation[2])
                {
                    Debug.Log("Activate 1st Consumable");
                    PowerAndConsActivation[2] = true;
                    ConsCount[0] -= 1;
                    ConsCountText[0].GetComponent<TextMeshPro>().text = ConsCount[0].ToString();
                    // void function to activate power
                    ConsumableObject[0].GetComponent<Consumable>().ActivateConsumable();

                    PowerAndConsDurationTimer[2] = PowerAndConsDuration[2];
                    PowerAndConsDurationSlider[2].maxValue = PowerAndConsDuration[2];
                    PowerAndConsDurationSlider[2].value = PowerAndConsDuration[2];
                }
            }
            // 2nd Cons
            if (ConsumableObject.Count > 1 && ConsumableObject[1] != null && PowerAndConsCDTimer[3] <= 0f && ConsCount[1] > 0 && !isPausing)
            {
                PowerAndConsDurationSlider[3].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                if (Input.GetKeyDown(KeyCode.Alpha2) && !PowerAndConsActivation[3])
                {
                    Debug.Log("Activate 1st Consumable");
                    PowerAndConsActivation[3] = true;
                    ConsCount[1] -= 1;
                    ConsCountText[1].GetComponent<TextMeshPro>().text = ConsCount[1].ToString();
                    // void function to activate power
                    ConsumableObject[1].GetComponent<Consumable>().ActivateConsumable();

                    PowerAndConsDurationTimer[3] = PowerAndConsDuration[3];
                    PowerAndConsDurationSlider[3].maxValue = PowerAndConsDuration[3];
                    PowerAndConsDurationSlider[3].value = PowerAndConsDuration[3];
                }
            }
            // 3rd Cons
            if (ConsumableObject.Count > 2 && ConsumableObject[2] != null && PowerAndConsCDTimer[4] <= 0f && ConsCount[2] > 0 && !isPausing)
            {
                PowerAndConsDurationSlider[4].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                if (Input.GetKeyDown(KeyCode.Alpha3) && !PowerAndConsActivation[4])
                {
                    Debug.Log("Activate 1st Consumable");
                    PowerAndConsActivation[4] = true;
                    ConsCount[2] -= 1;
                    ConsCountText[2].GetComponent<TextMeshPro>().text = ConsCount[2].ToString();
                    // void function to activate power
                    ConsumableObject[2].GetComponent<Consumable>().ActivateConsumable();

                    PowerAndConsDurationTimer[4] = PowerAndConsDuration[4];
                    PowerAndConsDurationSlider[4].maxValue = PowerAndConsDuration[4];
                    PowerAndConsDurationSlider[4].value = PowerAndConsDuration[4];
                }
            }
            // 4th Cons
            if (ConsumableObject.Count > 3 && ConsumableObject[3] != null && PowerAndConsCDTimer[5] <= 0f && ConsCount[3] > 0 && !isPausing)
            {
                PowerAndConsDurationSlider[5].fillRect.GetComponentInChildren<Image>().color =
                        new Color(0, 95 / 255f, 1, 149 / 255f);
                if (Input.GetKeyDown(KeyCode.Alpha4) && !PowerAndConsActivation[5])
                {
                    Debug.Log("Activate 1st Consumable");
                    PowerAndConsActivation[5] = true;
                    ConsCount[3] -= 1;
                    ConsCountText[3].GetComponent<TextMeshPro>().text = ConsCount[3].ToString();
                    // void function to activate power
                    ConsumableObject[3].GetComponent<Consumable>().ActivateConsumable();

                    PowerAndConsDurationTimer[5] = PowerAndConsDuration[5];
                    PowerAndConsDurationSlider[5].maxValue = PowerAndConsDuration[5];
                    PowerAndConsDurationSlider[5].value = PowerAndConsDuration[5];
                }
            }
            // Pause
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPausing = FindObjectOfType<CameraController>().PauseGame();
            }
        }
        // Timer Decrease
        for (int i=0;i<6;i++)
        {
            if (PowerAndConsDurationTimer[i]>0f)
            {
                PowerAndConsDurationTimer[i]-=Time.deltaTime;
                PowerAndConsDurationSlider[i].value -= Time.deltaTime;
            } else
            {
                if (PowerAndConsActivation[i])
                {
                    PowerAndConsActivation[i] = false;
                    PowerAndConsCDTimer[i] = PowerAndConsCD[i];
                    PowerAndConsCDImage[i].GetComponent<Image>().fillAmount = 1;
                    PowerAndConsCDText[i].GetComponent<TextMeshPro>().text = PowerAndConsCDTimer[i].ToString();
                    PowerAndConsCDText[i].SetActive(true);
                }
            }
            if (PowerAndConsCDTimer[i]>0f)
            {
                if (i>=2)
                {
                    if (ConsCount[i-2]>0)
                    {
                        PowerAndConsCDTimer[i] -= Time.deltaTime;
                        PowerAndConsCDText[i].GetComponent<TextMeshPro>().text = PowerAndConsCDTimer[i] >= 1 ?
                            ((int)PowerAndConsCDTimer[i]).ToString() : (Mathf.Round(PowerAndConsCDTimer[i] * 10) / 10) > 0 ?
                            (Mathf.Round(PowerAndConsCDTimer[i] * 10) / 10).ToString() : "";
                        PowerAndConsCDImage[i].GetComponent<Image>().fillAmount -= Time.deltaTime / PowerAndConsCD[i];
                    } else
                    {
                        PowerAndConsCDTimer[i] = 0f;
                        PowerAndConsCDText[i].GetComponent<TextMeshPro>().text = "";
                    }
                } else
                {
                    PowerAndConsCDTimer[i] -= Time.deltaTime;
                    PowerAndConsCDText[i].GetComponent<TextMeshPro>().text = PowerAndConsCDTimer[i] >= 1 ?
                        ((int)PowerAndConsCDTimer[i]).ToString() : (Mathf.Round(PowerAndConsCDTimer[i] * 10) / 10) > 0 ?
                        (Mathf.Round(PowerAndConsCDTimer[i] * 10) / 10).ToString() : "";
                    PowerAndConsCDImage[i].GetComponent<Image>().fillAmount -= Time.deltaTime / PowerAndConsCD[i];
                }
            } else
            {
                PowerAndConsCDText[i].SetActive(false);
            }
        }
        ShowInfo();
    }
    #endregion
    #region Play Sound
    public void PlayMovingSound(float volume)
    {
        if (aus.clip!=MovingSound)
        {
            aus.clip = MovingSound;
            aus.loop = true;
            aus.Play();
        }
        aus.volume = volume;
    }

    public void StopSound()
    {
        if (aus.clip!=SpawnSoundEffect)
        aus.clip = null;
    }

    public void PlayDashSound()
    {
        DashAudioSource.clip = DashSound;
        DashAudioSource.loop = false;
        DashAudioSource.volume = 0.75f;
        DashAudioSource.Play();
    }

    public void PlaySpawnSound()
    {
        aus.clip = SpawnSoundEffect;
        aus.volume = 1;
        aus.Play();
    }
    #endregion
    #region Show Information To HUD
    private void ShowInfo()
    {
        // Temp
        FighterTempText.GetComponent<TextMeshPro>().text = currentTemperature.ToString() + "°";
        FighterInfoCircle.GetComponent<HUDFighterInfoEffects>().CurrentTemp = currentTemperature;
        if (currentTemperature==50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 158 / 255f);
        } else if (currentTemperature>50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0,1,0, 158 / 255f), new Color(1, 0, 0, 200 / 255f), (currentTemperature-50)/50f);
        } else if (currentTemperature<50f)
        {
            FighterTempColor.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(0, 1, 1, 200 / 255f), new Color(0, 1, 0, 158 / 255f), (currentTemperature) / 50f);
        }
        // HP
        HPSlider.value = CurrentHP;
        HPText.GetComponent<TextMeshPro>().text = Mathf.Round(CurrentHP) + " / " + MaxHP;
        // BR
        BRSlider.maxValue = MaxBarrier;
        BRSlider.value = CurrentBarrier;
        BRText.GetComponent<TextMeshPro>().text = Mathf.Round(CurrentBarrier) + " / " + MaxBarrier;
    }

    private void SetConsumableCount()
    {
        for (int i=0;i< ConsumableObject.Count; i++)
        {
            ConsCountText[i].GetComponent<TextMeshPro>().text = Consumables[ConsumableObject[i].name.Replace("(Clone)","")].ToString();
            ConsCount[i] = int.Parse(Consumables[ConsumableObject[i].name.Replace("(Clone)", "")].ToString());

        }
    }

    private void SetCDandDuration()
    {
        if (FirstPower != null )
        {
            PowerAndConsCD[0] = FirstPower.GetComponent<Powers>().CD;

            if (FirstPower.GetComponent<Powers>().Duration == 0)
            {
                PowerAndConsDuration[0] = 0.5f;
            } else
            {
                PowerAndConsDuration[0] = FirstPower.GetComponent<Powers>().Duration;
            }
            
        }
        if (SecondPower != null)
        {
            PowerAndConsCD[1] = SecondPower.GetComponent<Powers>().CD;

            if (SecondPower.GetComponent<Powers>().Duration == 0)
            {
                PowerAndConsDuration[1] = 0.5f;
            }
            else
            {
                PowerAndConsDuration[1] = SecondPower.GetComponent<Powers>().Duration;
            }
        }

        if (Consumables != null && Consumables.Count > 0)
        {
            int index = 2;
            for (int i = 0; i < Consumables.Count; i++)
            {
                PowerAndConsCD[index] = ConsumableObject[i].GetComponent<Consumable>().Cooldown;
                PowerAndConsDuration[index] = ConsumableObject[i].GetComponent<Consumable>().Duration;
                ConsumableObject[i].GetComponent<Consumable>().Fighter = gameObject;
                index++;
            }
        }
    }
    #endregion
    #region Passive

    public void ShieldPassive()
    {
        float br = 0;
        if (FirstPower != null)
        {
            if (FirstPower.GetComponent<Powers>().BR > 0)
            {
                br = FirstPower.GetComponent<Powers>().BR;
            }       
        }
        if (SecondPower != null)
        {
            if (SecondPower.GetComponent<Powers>().BR > 0)
            {
                br = SecondPower.GetComponent<Powers>().BR;
            }
        }
        if (br == 0)
        {
            MaxBarrier = 5000;
        } else
        {
            MaxBarrier = MaxHP * br / 100;
        }
        
        CurrentBarrier = MaxBarrier;
    }
    #endregion
}
