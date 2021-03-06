﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ShootArrow : MonoBehaviour
{
    public GameObject Connected_Arrow;
    public float ChargingTime;

    private Player player;
    private List<GameObject> Aim_Line;
    private float ChargingTimeCount;
    private bool Charging;
    private float ArrowAngle;


    private const float Velocity_Charge_Speed = 10;
    private const float Aim_offset = 0.8f;
    private const float mirroBounceStartPointOffset = 0.01f;
    private const float AimLineUnitPerMeter = 2;

    private const float AimDirectionSlowRotationSpeed = 2;
    private const float AimDirectionFastRotationSpeed = 60;
    
    private const float AimDirectionSlowChangeThreshold = 0.2f;
    private const float AimDirectionFastChangeThreshold = 0.9f;

    private const float AimRotationLimit = 150;

    //private const float mirrorTopDownOffset = 0.05f;

    
    // Start is called before the first frame update
    void Start()
    {
        ArrowAngle = 90;
        player = ControllerManager.Fairy;
        Aim_Line = new List<GameObject>();
        EventManager.instance.AddHandler<LoadLevel>(OnLoadLevel);
        EventManager.instance.AddHandler<CharacterHitSpineEdge>(OnHitSpineEdge);
    }

    private void OnDestroy()
    {
        EventManager.instance.RemoveHandler<LoadLevel>(OnLoadLevel);
        EventManager.instance.RemoveHandler<CharacterHitSpineEdge>(OnHitSpineEdge);
    }

    // Update is called once per frame
    void Update()
    {
        
        var status = GetComponent<Fairy_Status_Manager>();
        if (status.status!=FairyStatus.Aimed && status.status!=FairyStatus.KnockBack)
        {
            Check_Input();
        }
        CheckIfAimed();
    }

    private bool InputHolding()
    {
        if (ControllerManager.FairyJoystick != null)
        {
            return player.GetButton("RT");
        }
        else
        {
            return Input.GetKey(KeyCode.V);
        }
    }

    private bool InputUpFast()
    {
        if (ControllerManager.FairyJoystick != null)
        {
            return player.GetAxis("Right Stick Y")>AimDirectionFastChangeThreshold;
        }
        else
        {
            return Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S);
        }
    }

    private bool InputDownFast()
    {
        if (ControllerManager.FairyJoystick != null)
        {
            return player.GetAxis("Right Stick Y") < -AimDirectionFastChangeThreshold;
        }
        else
        {
            return Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W);
        }
    }

    private bool InputUpSlow()
    {
        if (ControllerManager.FairyJoystick != null)
        {
            return player.GetAxis("Right Stick Y") > AimDirectionSlowChangeThreshold;
        }
        else
        {
            return false;
        }
    }

    private bool InputDownSlow()
    {
        if (ControllerManager.FairyJoystick != null)
        {
            return player.GetAxis("Right Stick Y") < -AimDirectionSlowChangeThreshold;
        }
        else
        {
            return false;
        }
    }

    private void Check_Input()
    {
        var Fairy_Status = GetComponent<Fairy_Status_Manager>();
        if (Fairy_Status.status==FairyStatus.Normal && GetComponent<CharacterMove>().OnGround && InputHolding())
        {
            Fairy_Status.status = FairyStatus.Aiming;
            if (Connected_Arrow == null)
            {
                Charging = true;
                ChargingTimeCount = 0;
                Connected_Arrow = (GameObject)Instantiate(Resources.Load("Prefabs/GameObject/Arrow"));
                Connected_Arrow.transform.parent = transform;

                Vector2 direction;
                if (transform.right.x > 0)
                {
                    direction = Utility.Rotate(Vector2.up, -ArrowAngle);
                }
                else
                {
                    direction = Utility.Rotate(Vector2.up, ArrowAngle);
                }

                Connected_Arrow.transform.position = transform.position + (Vector3)direction * Aim_offset;
                ClearAimLine();
                CreateAimLIne(direction, Connected_Arrow.transform.position);
            }
        }

        if (Fairy_Status.status == FairyStatus.Aiming)
        {
            if (!GetComponent<CharacterMove>().OnGround)
            {
                Fairy_Status.status = FairyStatus.Normal;
                return;
            }

            if (Charging)
            {
                if (ChargingTimeCount > ChargingTime)
                {
                    Connected_Arrow.GetComponent<AudioSource>().Play();
                    Charging = false;
                }
                ChargingTimeCount += Time.deltaTime;
            }

            if (InputUpFast()||InputDownFast())
            {
                if (InputUpFast())
                {
                    ArrowAngle -= AimDirectionFastRotationSpeed * Time.deltaTime;
                    if (ArrowAngle < 0)
                    {
                        ArrowAngle = 0;
                    }
                }
                else
                {
                    ArrowAngle+= AimDirectionFastRotationSpeed * Time.deltaTime;
                    if (ArrowAngle > AimRotationLimit)
                    {
                        ArrowAngle = AimRotationLimit;
                    }
                }
            }
            else if (InputUpSlow()||InputDownSlow())
            {
                if (InputUpSlow())
                {
                    ArrowAngle -= AimDirectionSlowRotationSpeed * Time.deltaTime;
                    if (ArrowAngle < 0)
                    {
                        ArrowAngle = 0;
                    }
                }
                else
                {
                    ArrowAngle += AimDirectionSlowRotationSpeed * Time.deltaTime;
                    if (ArrowAngle > AimRotationLimit)
                    {
                        ArrowAngle = AimRotationLimit;
                    }
                }
            }

            Vector2 direction;
            if (transform.right.x > 0)
            {
                direction = Utility.Rotate(Vector2.up, -ArrowAngle);
            }
            else
            {
                direction = Utility.Rotate(Vector2.up, ArrowAngle);
            }
            ClearAimLine();
            CreateAimLIne(direction, Connected_Arrow.transform.position);
            Connected_Arrow.transform.position = transform.position + (Vector3)direction * Aim_offset;

            if (!InputHolding())
            {
                ClearAimLine();
                if (!Charging)
                {
                    Connected_Arrow.GetComponent<Arrow>().direction = direction;
                    Connected_Arrow.GetComponent<Arrow>().Emited = true;
                    Connected_Arrow.transform.parent = null;
                    Connected_Arrow = null;
                }
                else
                {
                    Destroy(Connected_Arrow);
                }
                Fairy_Status.status = FairyStatus.Normal;
            }
        }

    }

    private void CreateAimLIne(Vector2 direction, Vector2 StartPoint)
    {
        int layermask = 1 << LayerMask.NameToLayer("TutorialTrigger") | 1 << LayerMask.NameToLayer("Invisible_Object") | 1 << LayerMask.NameToLayer("Arrow") | 1 << LayerMask.NameToLayer("Portal") | 1 << LayerMask.NameToLayer("PlatformTotemTrigger") | 1<<LayerMask.NameToLayer("Path") | 1<<LayerMask.NameToLayer("Main_Character") | 1<<LayerMask.NameToLayer("Fairy");
        layermask = ~layermask;
        float mag = 100;
        RaycastHit2D hit= Physics2D.Raycast(StartPoint, direction, mag, layermask);
        int num = Mathf.FloorToInt((hit.point - StartPoint).magnitude*AimLineUnitPerMeter)+1;
        for(int i = 0; i < num; i++)
        {
            GameObject unit = (GameObject)Instantiate(Resources.Load("Prefabs/GameObject/AimLineUnit"), StartPoint + direction * (1.0f/AimLineUnitPerMeter)*i, Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.right ,direction)));
            Aim_Line.Add(unit);
        }
        if (hit&&hit.collider.gameObject.CompareTag("Mirror"))
        {
            StartPoint = hit.point;
            direction = Vector2.Reflect(direction, hit.normal);
            direction.Normalize();
            CreateAimLIne(direction, StartPoint);
        }

    }

    private void ClearAimLine()
    {
        for (int i = 0; i < Aim_Line.Count; i++)
        {
            Destroy(Aim_Line[i]);
        }
    }

    private void CheckIfAimed()
    {
        if (GetComponent<Fairy_Status_Manager>().status == FairyStatus.Aimed)
        {
            Destroy(Connected_Arrow);
            ClearAimLine();
        }
    }

    private void OnLoadLevel(LoadLevel L)
    {
        ChargingTimeCount = 0;
        ClearAimLine();
        Destroy(Connected_Arrow);
    }

    private void OnHitSpineEdge(CharacterHitSpineEdge C)
    {
        if (C.Character == gameObject)
        {
            ChargingTimeCount = 0;
            ClearAimLine();
            Destroy(Connected_Arrow);
        }
    }
}
