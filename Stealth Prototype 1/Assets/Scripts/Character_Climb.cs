﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Character_Climb : MonoBehaviour
{

    public float ClimbSpeed;
    public Vector2 JumpOffSpeed;
    public float ThroughPathEndDuration;

    private Player player;
    private bool IsClimbing;
    public bool PathEndThrough;


    private bool InPathEnd;
    private bool AbleToClimb;
    public GameObject ConnectedPath;

    private const float climb_threshold = 0.7f;
    private const float climb_jump_threshold = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerId>().player;
        ConnectedPath = null;
        InPathEnd = false;
        IsClimbing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Fairy"))
        {
            var Status = GetComponent<Fairy_Status_Manager>();
            if (Status.status!=FairyStatus.KnockBack&& Status.status != FairyStatus.Aimed && !PathEndThrough)
            {
                Check_Input();
            }

        }
        else if (gameObject.CompareTag("Main_Character"))
        {
            var Status = GetComponent<Main_Character_Status_Manager>();
            if (Status.status!=MainCharacterStatus.KnockBack&& Status.status != MainCharacterStatus.Aimed && !PathEndThrough)
            {
                Check_Input();
            }
        }
        
        Check_Status();
    }




    private void Check_Input()
    {
        if ( AbleToClimb && !IsClimbing && (player.GetAxis("Left Stick Y") > climb_threshold && ConnectedPath.transform.position.y>transform.position.y|| player.GetAxis("Left Stick Y") < -climb_threshold && ConnectedPath.transform.position.y < transform.position.y))
        {
            IsClimbing = true;
            if (gameObject.CompareTag("Fairy"))
            {
                var Status = GetComponent<Fairy_Status_Manager>();
                if (player.GetAxis("Left Stick Y") > 0)
                {
                    transform.position = new Vector3(ConnectedPath.transform.position.x, transform.position.y, 0);
                }
                else
                {
                    transform.position = new Vector3(ConnectedPath.transform.position.x, transform.position.y, 0);
                }
                Status.status = FairyStatus.Climbing;

            }
            else if (gameObject.CompareTag("Main_Character"))
            {
                var Status = GetComponent<Main_Character_Status_Manager>();
                if (player.GetAxis("Left Stick Y") > 0)
                {
                    transform.position = new Vector3(ConnectedPath.transform.position.x, transform.position.y, 0);
                }
                else
                {
                    transform.position = new Vector3(ConnectedPath.transform.position.x, transform.position.y, 0);
                }
                Status.status = MainCharacterStatus.Climbing;
            }
            if (InPathEnd)
            {
                if(player.GetAxis("Left Stick Y") > 0)
                {
                    StartCoroutine(ThroughPathEnd(true));
                }
                else
                {
                    StartCoroutine(ThroughPathEnd(false));
                }
                return;
            }
        }
        if (!AbleToClimb)
        {
            IsClimbing = false;
        }

        if(IsClimbing)
        {
            transform.position = new Vector3(ConnectedPath.transform.position.x, transform.position.y, transform.position.z);
            GetComponent<CharacterMove>().speed.x = 0;
            if (Mathf.Abs(player.GetAxis("Left Stick Y")) > climb_threshold)
            {
                if (player.GetAxis("Left Stick Y") > 0)
                {
                    GetComponent<CharacterMove>().speed.y = ClimbSpeed;
                }
                else
                {
                    GetComponent<CharacterMove>().speed.y = -ClimbSpeed;
                }
                if (InPathEnd)
                {
                    if (player.GetAxis("Left Stick Y") > 0)
                    {
                        StartCoroutine(ThroughPathEnd(true));
                    }
                    else
                    {
                        StartCoroutine(ThroughPathEnd(false));
                    }
                    return;
                }
            }
            else
            {
                GetComponent<CharacterMove>().speed.y = 0;
            }
        }

        if (IsClimbing)
        {
            if(player.GetAxis("Left Stick X") > climb_jump_threshold)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            }
            else if(player.GetAxis("Left Stick X") < -climb_jump_threshold)
            {
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            }
            if (player.GetButtonDown("A"))
            {
                IsClimbing = false;
                if (transform.right.x > 0)
                {
                    GetComponent<CharacterMove>().speed = new Vector2(JumpOffSpeed.x, JumpOffSpeed.y);
                }
                else
                {
                    GetComponent<CharacterMove>().speed = new Vector2(-JumpOffSpeed.x, JumpOffSpeed.y);
                }
                
            }
        }
    }

    private void Check_Status()
    {
        if (IsClimbing)
        {


        }
        else
        {

            if (gameObject.CompareTag("Fairy"))
            {
                var Status = GetComponent<Fairy_Status_Manager>();
                if (Status.status == FairyStatus.Climbing)
                {
                    Status.status = FairyStatus.Normal;
                }

            }
            else if (gameObject.CompareTag("Main_Character"))
            {
                var Status = GetComponent<Main_Character_Status_Manager>();
                if (Status.status == MainCharacterStatus.Climbing)
                {
                    Status.status = MainCharacterStatus.Normal;
                }
            }
        }
        if (InPathEnd)
        {

        }
        else
        {
            
            if (GetComponent<CharacterMove>().OnGround)
            {
                IsClimbing = false;
            }
        }


    }

    private IEnumerator ThroughPathEnd(bool up)
    {
        GameObject PathEnd = ConnectedPath.transform.Find("Path_End").gameObject;
        if(up && PathEnd.transform.position.y<transform.position.y || !up && PathEnd.transform.position.y > transform.position.y)
        {
            yield break;
        }

        PathEndThrough = true;
        GetComponent<CharacterMove>().speed = Vector2.zero;
        Vector3 Target;
        if (up)
        {
            Target = PathEnd.transform.position + Vector3.up;
        }
        else
        {
            Target = PathEnd.transform.position + Vector3.down;
        }
        Vector3 speed = (Target - transform.position) / ThroughPathEndDuration;
        float timecount = 0;
        while (timecount < ThroughPathEndDuration)
        {
            transform.position += speed * Time.deltaTime;
            timecount += Time.deltaTime;
            yield return null;
        }
        transform.position = Target;
        if (up)
        {
            IsClimbing = false;
        }
        PathEndThrough = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject ob = collision.GetComponent<Collider2D>().gameObject;
        if (ob.CompareTag("Path"))
        {
            ConnectedPath = ob;
            AbleToClimb = true;
        }
        if (ob.name == "Path_End")
        {
            ConnectedPath = ob.transform.parent.gameObject;
            AbleToClimb = true;
            InPathEnd = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject ob = collision.GetComponent<Collider2D>().gameObject;
        if (ob.CompareTag("Path"))
        {
            ConnectedPath = null;
            AbleToClimb = false;
        }
        if (ob.name == "Path_End")
        {
            InPathEnd = false;
        }
    }
}
