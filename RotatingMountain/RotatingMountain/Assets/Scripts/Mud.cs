using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    public bool CouldInteract;
    private bool hasInteracted;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        CouldInteract = false;
        hasInteracted = false;
        player = PlayerAttributes.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.transform.position,player.transform.position)<=6){
            CouldInteract = true;
        }
        if(CouldInteract && !hasInteracted)
        {
            print("Player interacting with Mud");
            PlayerAttributes.Instance.HandleAttributesValueChange(PlayerAttributes.ATTRIBUTES_TYPE.SPEED, -2f);
            CouldInteract = false;
            hasInteracted = true;
        }
    }
}
