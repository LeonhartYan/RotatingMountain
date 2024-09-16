using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributesTextManager : MonoBehaviour
{
    public static AttributesTextManager Instance;
    public List<string> ShowingText;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public float FadeInTimer = 1.5f;
    private float timer = 0;
    private Color oriColor;
    public string GetTextByAttributes(PlayerAttributes.ATTRIBUTES_TYPE type)
    {
        switch (type)
        {
            case PlayerAttributes.ATTRIBUTES_TYPE.SPEED:
                return "Speed";
            case PlayerAttributes.ATTRIBUTES_TYPE.PHSICAL_POWER:
                return "Physical Power";
            case PlayerAttributes.ATTRIBUTES_TYPE.BLOOD_OXYGEN:
                return "Blood Oxygen";
            case PlayerAttributes.ATTRIBUTES_TYPE.SAN:
                return "San";
        }
        return "";
    }

    public void HandleShowingNewLine(PlayerAttributes.ATTRIBUTES_TYPE type, float amount)
    {
        timer = FadeInTimer;
        this.GetComponent<Text>().color = oriColor;
        string ope = (amount > 0) ? "+" : "-";
        if (ShowingText.Count > 3) ShowingText.RemoveAt(0);
        ShowingText.Add(GetTextByAttributes(type) + ": " + ope + Mathf.Abs(amount));
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowingText = new List<string>();
        timer = 0;
        oriColor = this.GetComponent<Text>().color;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        this.GetComponent<Text>().text = "";
        if (timer>=0)
        {
            Color col = this.GetComponent<Text>().color;
            col.a -= Time.deltaTime / FadeInTimer;
            this.GetComponent<Text>().color = col;
            for (int i = 0; i < ShowingText.Count; i++)
            {
                this.GetComponent<Text>().text += ShowingText[i];
                this.GetComponent<Text>().text += "\n";
            }
        }
        else
        {
            ShowingText.Clear();
        }
    }
}
