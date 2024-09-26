using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAttributes : MonoBehaviour
{
    public static PlayerAttributes Instance;

    public float SpeedMultiplier = 1;

    public GameObject[] UITexts;
    public GameObject[] UISliders;
    public enum ATTRIBUTES_TYPE
    {
        SPEED, 
        PHSICAL_POWER, 
        BLOOD_OXYGEN, 
        SAN,
    }

    public enum INTERACTION_ANIMATION_TYPE
    {
        IDLE,
        WALK,
        TOUCH,
        REST,
    }


    public float[] InitialAttributesValue;
    public float[] CurrentAttributesValue;

    public float speedAdjustmentAmount = 1f;
    public float BloodDown = 0;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentAttributesValue = new float[InitialAttributesValue.Length];
        for (int i = 0; i < InitialAttributesValue.Length; i++)
        {
            CurrentAttributesValue[i] = InitialAttributesValue[i];
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < CurrentAttributesValue.Length; i++)
        {
            UITexts[i].GetComponent<Text>().text = AttributesTextManager.Instance.GetTextByAttributes((ATTRIBUTES_TYPE)i);
            UISliders[i].GetComponent<Slider>().value = (ATTRIBUTES_TYPE)i == ATTRIBUTES_TYPE.SPEED ? CurrentAttributesValue[i] / 10f : CurrentAttributesValue[i] / 100f;
        }
    }

    public void HandleAttributesValueChange(ATTRIBUTES_TYPE type, float value, bool ShowText = true)
    {
        CurrentAttributesValue[(int)type] += value;
        if (type == ATTRIBUTES_TYPE.SPEED )
        {
            if(CurrentAttributesValue[(int)type] < 0) CurrentAttributesValue[(int)type] = 0;
            if (CurrentAttributesValue[(int)type] >10) CurrentAttributesValue[(int)type] = 10;
        }
        else
        {
            if (CurrentAttributesValue[(int)type] <= 0)
            {
                SceneManager.LoadScene(3);
                CurrentAttributesValue[(int)type] = 0;
            }
            if (CurrentAttributesValue[(int)type] > 100) CurrentAttributesValue[(int)type] = 100;
        }
        if(ShowText)
        {
            AttributesTextManager.Instance.HandleShowingNewLine(type, value);
        }
    }

    public void HandleAnimationStateChange(INTERACTION_ANIMATION_TYPE type)
    {
        switch(type)
        {
            case INTERACTION_ANIMATION_TYPE.IDLE:
                GetComponent<Animator>().SetBool("Touch", false);
                GetComponent<Animator>().SetBool("Rest", false);
                GetComponent<Animator>().SetBool("IsWalk", false);
                break;
            case INTERACTION_ANIMATION_TYPE.WALK:
                GetComponent<Animator>().SetBool("Touch", false);
                GetComponent<Animator>().SetBool("Rest", false);
                GetComponent<Animator>().SetBool("IsWalk", true);
                break;
            case INTERACTION_ANIMATION_TYPE.TOUCH:
                GetComponent<Animator>().SetBool("Touch", true);
                GetComponent<Animator>().SetBool("Rest", false);
                GetComponent<Animator>().SetBool("IsWalk", false);
                PlayerAttributes.Instance.SpeedMultiplier = 0f;
                StartCoroutine(ResetSpeed(2f,true));
                break;
            case INTERACTION_ANIMATION_TYPE.REST:
                GetComponent<Animator>().SetBool("Touch", false);
                GetComponent<Animator>().SetBool("Rest", true);
                GetComponent<Animator>().SetBool("IsWalk", false);
                PlayerAttributes.Instance.SpeedMultiplier = 0f;
                StartCoroutine(ResetSpeed(5f,true));
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] > 1) HandleAttributesValueChange(ATTRIBUTES_TYPE.SPEED, -speedAdjustmentAmount,false);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] < InitialAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] * 2) HandleAttributesValueChange(ATTRIBUTES_TYPE.SPEED, speedAdjustmentAmount, false);
        }
        float speedModifier = CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED];
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, 0);
        movementDirection.Normalize();
        transform.Translate(movementDirection * speedModifier * SpeedMultiplier * Time.deltaTime /2f, Space.World);

        if (horizontalInput != 0 && PlayerAttributes.Instance.SpeedMultiplier != 0f)
        {
            HandleAnimationStateChange(INTERACTION_ANIMATION_TYPE.WALK);

            //the quicker, the more pp cost
            float lerp = Mathf.Lerp(1, 2.5f, CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] * SpeedMultiplier / 10);
            HandleAttributesValueChange(ATTRIBUTES_TYPE.PHSICAL_POWER, -CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] / 10 * lerp * Time.deltaTime, false);
            float animationLerp = Mathf.Lerp(0.5f, 1.25f, CurrentAttributesValue[(int)ATTRIBUTES_TYPE.SPEED] * SpeedMultiplier / 10);
            this.GetComponent<Animator>().SetFloat("Speed", animationLerp);
            if (horizontalInput > 0)
            {
                this.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                this.transform.eulerAngles = new Vector3(0, -180, 0);
            }
        }
        else if(horizontalInput == 0 && PlayerAttributes.Instance.SpeedMultiplier != 0f)
        {
            HandleAnimationStateChange(INTERACTION_ANIMATION_TYPE.IDLE);
        }

        HandleAttributesValueChange(ATTRIBUTES_TYPE.BLOOD_OXYGEN, -BloodDown * Time.deltaTime, false);
        HandleAttributesValueChange(ATTRIBUTES_TYPE.SAN, -(100f/GameManager.Instance.LevelLength) * 2f * Time.deltaTime, false);
        UpdateUI();
    }
    IEnumerator ResetSpeed(float time, bool resetAnimator = false)
    {
        yield return new WaitForSeconds(time);
        PlayerAttributes.Instance.SpeedMultiplier = 1f;
        if(resetAnimator)
        {
            HandleAnimationStateChange(INTERACTION_ANIMATION_TYPE.IDLE);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "MUD")
        {
            SpeedMultiplier = 0.7f;
        }
        if(collision.tag == "SNOW")
        {
            SpeedMultiplier = 0.5f;
        }
        if(collision.tag == "ENDING")
        {
            SceneManager.LoadScene(2);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MUD")
        {
            SpeedMultiplier = 1f;
        }
        if (collision.tag == "SNOW")
        {
            SpeedMultiplier = 1f;
        }
        if(collision.tag == "L2")
        {
            if(this.transform.position.x>collision.transform.position.x)
            {
                BloodDown = 1f;
            }
            else
            {
                BloodDown = 0;
            }
        }
        if (collision.tag == "L3")
        {
            if (this.transform.position.x > collision.transform.position.x)
            {
                BloodDown = 2f;
            }
            else
            {
                BloodDown = 1f;
            }
        }
    }
}