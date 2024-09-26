using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple : MonoBehaviour
{
    public bool CouldInteract;
    public string TipToShow;
    public GameObject Icon;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        CouldInteract = true;
        player = PlayerAttributes.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position,player.transform.position)<=3 && CouldInteract)
        {
            Icon.SetActive(true);
            //player is close
            if (Input.GetKeyDown(KeyCode.F))
            {
                CouldInteract = false;
                print("Player interacting with Temple");
                GameManager.Instance.HandleTimerChange(-10f);
                PlayerAttributes.Instance.HandleAttributesValueChange(PlayerAttributes.ATTRIBUTES_TYPE.SAN, 25f);
                PlayerAttributes.Instance.HandleAttributesValueChange(PlayerAttributes.ATTRIBUTES_TYPE.BLOOD_OXYGEN, 10f);
                GameManager.Instance.HandleShowTips(TipToShow);
                PlayerAttributes.Instance.HandleAnimationStateChange(PlayerAttributes.INTERACTION_ANIMATION_TYPE.TOUCH);
            }
        }
        else
        {
            Icon.SetActive(false);
        }
    }
}
