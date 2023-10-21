using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusStatus : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject FirstPos;
    public GameObject TemplateCard;
    public GameObject Content;
    public GameObject ScrollRect;
    public GameObject NoCardText;
    public LOTWEffect LOTWEffect;
    #endregion
    #region NormalVariables
    private Dictionary<string, string> DictDataAllEffects;
    private Dictionary<string, string> EffectColor;
    public List<GameObject> EffectList;
    private bool alreadyGen;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize variables
        DictDataAllEffects = new Dictionary<string, string>();
        EffectColor = new Dictionary<string, string>();
        GetListDataEffect();
        Open();
    }

    private void OnEnable()
    {
        if (!alreadyGen)
        Open();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    public void GetListDataEffect()
    {
        DictDataAllEffects.Add("Maximum HP", StatsScaleToString(LOTWEffect.LOTWMaxHPScale, true));
        DictDataAllEffects.Add("All Repairing Effect", StatsScaleToString(LOTWEffect.LOTWRepairEffectScale, true));
        DictDataAllEffects.Add("All Damage Received", StatsScaleToString(LOTWEffect.LOTWAllDamageReceiveScale, true));
        DictDataAllEffects.Add("Weapon Damage Received", StatsScaleToString(LOTWEffect.LOTWWeaponDMGReceivedScale, false));

        DictDataAllEffects.Add("Maximum Moving Speed", StatsScaleToString(LOTWEffect.LOTWMoveSpeedScale, true));
        DictDataAllEffects.Add("All Cash gain", StatsScaleToString(LOTWEffect.LOTWCashIncScale,true));
        DictDataAllEffects.Add("Affection Of Hazard Environment", StatsBoolToString(LOTWEffect.LOTWAffectEnvironment));
        DictDataAllEffects.Add("Consumable Costs", StatsBoolToString(!LOTWEffect.LOTWConsNoCost));

        DictDataAllEffects.Add("Weapon Damage", StatsScaleToString(LOTWEffect.LOTWWeaponDMGIncScale,true));
        DictDataAllEffects.Add("Weapon Rate Of Fire", StatsScaleToString(LOTWEffect.LOTWWeaponROFScale,true));
        DictDataAllEffects.Add("Thermal Weapon Damage To Thermal Status", StatsScaleToString(LOTWEffect.LOTWThermalWeaponDMGScale,true));
        DictDataAllEffects.Add("Power Damage", StatsScaleToString(LOTWEffect.LOTWPowerDMGIncScale, true));
        DictDataAllEffects.Add("Power Cooldown", StatsScaleToString(LOTWEffect.LOTWPowerCDScale, false));
        DictDataAllEffects.Add("Damage To Barrier", StatsScaleToString(LOTWEffect.LOTWBarrierDMGScale, true));
        DictDataAllEffects.Add("Damage To Far Enemies", StatsScaleToString(LOTWEffect.LOTWFarDMGScale, true));

        EffectColor.Add("Maximum HP", "#0000ff");
        EffectColor.Add("All Repairing Effect", "#0000ff");
        EffectColor.Add("All Damage Received", "#0000ff");
        EffectColor.Add("Weapon Damage Received", "#0000ff");

        EffectColor.Add("Maximum Moving Speed", "#00ff00");
        EffectColor.Add("All Cash gain", "#00ff00");
        EffectColor.Add("Affection Of Hazard Environment", "#00ff00");
        EffectColor.Add("Consumable Costs", "#00ff00");

        EffectColor.Add("Weapon Damage", "#ff0000");
        EffectColor.Add("Weapon Rate Of Fire", "#ff0000");
        EffectColor.Add("Thermal Weapon Damage To Thermal Status", "#ff0000");
        EffectColor.Add("Power Damage", "#ff0000");
        EffectColor.Add("Power Cooldown", "#ff0000");
        EffectColor.Add("Damage To Barrier", "#ff0000");
        EffectColor.Add("Damage To Far Enemies", "#ff0000");
    }

    private string StatsScaleToString(float scale, bool isIncrease)
    {
        string final = "";
        if (isIncrease)
        {
            final += "+";
        } else
        {
            final += "-";
        }
        float scaleToPercent = (scale - 1) * 100;
        final += (Mathf.Round(scaleToPercent)).ToString() + "%";
        return final;
    }

    private string StatsBoolToString(bool boolValue)
    {
        return boolValue ? "100%" : "0%";
    }

    private void Open()
    {
        ScrollRect.GetComponent<ScrollRect>().vertical = false;
        EffectList = new List<GameObject>();
        GenerateAllCards();
    }

    private void GenerateAllCards()
    {
        alreadyGen = true;
        /*if (ListDataAllCard.Count == 0)
        {
            NoCardText.SetActive(true);
        }*/
        Vector2 Pos = new Vector2(FirstPos.transform.position.x, FirstPos.transform.position.y);
        foreach (var item in DictDataAllEffects)
        {
            string Effect = item.Key;
            string percent = item.Value;
            string color = EffectColor[Effect];
            GameObject Card = Instantiate(TemplateCard, new Vector3(Pos.x, Pos.y, TemplateCard.transform.position.z), Quaternion.identity);
            // Info
            Card.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Effect;
            Card.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = percent;
            Color TierColor;
            ColorUtility.TryParseHtmlString(color, out TierColor);
            Card.GetComponent<PauseMenuEffect>().Effect = Effect;
            Card.transform.GetChild(0).GetComponent<Image>().color = TierColor;
            Color c = Card.transform.GetChild(0).GetComponent<Image>().color;
            c.a = 30 / 255f;
            Card.transform.GetChild(0).GetComponent<Image>().color = c;
            Card.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(2).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(0).GetChild(4).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetComponent<Image>().color = TierColor;
            Color c2 = Card.transform.GetChild(1).GetComponent<Image>().color;
            c2.a = 30 / 255f;
            Card.transform.GetChild(1).GetComponent<Image>().color = c2;
            Card.transform.GetChild(1).GetChild(1).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(2).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(3).GetComponent<Image>().color = TierColor;
            Card.transform.GetChild(1).GetChild(4).GetComponent<Image>().color = TierColor;
            Card.transform.SetParent(Content.transform);
            Card.transform.localScale = TemplateCard.transform.localScale;
            Card.SetActive(true);
            EffectList.Add(Card);
        }
        ScrollRect.GetComponent<ScrollRect>().vertical = true;
    }

    private void Close()
    {
        int n = 0;
        while (n < EffectList.Count)
        {
            if (EffectList[n] != null)
            {
                GameObject temp = EffectList[n];
                EffectList.RemoveAt(n);
                Destroy(temp);
            }
            else
            {
                n++;
            }
        }
        alreadyGen = false;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Close();
    }
    #endregion
}
