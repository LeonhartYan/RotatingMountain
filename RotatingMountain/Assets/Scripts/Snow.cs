using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Snow : MonoBehaviour
{
    public float ReactionTime;
    public string TipToShow;
    public KeyCode ReactionKey;
    public GameObject ReactionUI;
    [Serializable]
    public struct PENALTY_DATA
    {
        public PlayerAttributes.ATTRIBUTES_TYPE Type;
        public float Duration;
        public float Amount;
    }
    public PENALTY_DATA[] Penalties;

    private bool startTimer;
    private bool hasTriggered;
    private bool goDown;
    // Start is called before the first frame update
    void Start()
    {
        startTimer = false;
        ReactionUI.SetActive(false);
        goDown = true;
    }


    IEnumerator ResetSpeed(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerAttributes.Instance.SpeedMultiplier = 1f;
    }

    public void HandlePenalty()
    {
        Debug.Log("Player failed to response! penalty!");
        GameManager.Instance.HandleShowTips(TipToShow);
        for (int i = 0;i<Penalties.Length;i++)
        {
            if(Penalties[i].Type == PlayerAttributes.ATTRIBUTES_TYPE.SPEED)
            {
                PlayerAttributes.Instance.SpeedMultiplier = Penalties[i].Amount;
                StartCoroutine(ResetSpeed(Penalties[i].Duration));
            }
            else
            {
                PlayerAttributes.Instance.HandleAttributesValueChange(Penalties[i].Type, Penalties[i].Amount);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(startTimer)
        {
            if(Input.GetKeyDown(ReactionKey))
            {
                ReactionUI.SetActive(false);
                startTimer = false;
                return;
            }
            ReactionTime -= Time.deltaTime;
            if(ReactionTime<=0)
            {
                GameManager.Instance.HandleTimerChange(-15f);
                HandlePenalty();
                ReactionUI.SetActive(false);
                startTimer = false;
            }
            if(goDown)
            {
                Color color = ReactionUI.GetComponent<Text>().color;
                color.a -= Time.deltaTime * 2;
                if(color.a<=0)
                {
                    goDown = false;
                }
                ReactionUI.GetComponent<Text>().color = color;
            }
            else
            {
                Color color = ReactionUI.GetComponent<Text>().color;
                color.a += Time.deltaTime * 2;
                if (color.a >= 1)
                {
                    goDown = true;
                }
                ReactionUI.GetComponent<Text>().color = color;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !hasTriggered)
        {
            Debug.Log("Player need attention! timer started!");
            startTimer = true;
            hasTriggered = true;
            ReactionUI.SetActive(true);
            ReactionUI.GetComponent<Text>().text = ReactionKey.ToString();
        }
    }
}
