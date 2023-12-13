using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    Animator anim;
    PlayerLogic thePlayer;
    void Start()
    {
        anim = GetComponent<Animator>();
        thePlayer = PlayerLogic.playerObject.GetComponent<PlayerLogic>();
    }

    void Update()
    {
        float speed = Input.GetAxis("Vertical");
        float direction = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.SetBool("IsCrouching", true);
            anim.SetBool("IsWalking", false);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            anim.SetBool("IsCrouching", false);
        }
        if (Input.GetButtonDown("Throw") && thePlayer.selectedItem != null && GameManager.gameManager.throwingPowerEnabled)
        {
            anim.SetTrigger("Throw");
        }
        anim.SetFloat("Speed", speed);
        anim.SetFloat("Direction", direction);
        anim.SetBool("IsWalking", speed != 0 || direction != 0);
    }
}
