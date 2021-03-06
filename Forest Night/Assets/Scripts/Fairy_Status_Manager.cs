﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public enum FairyStatus
{
    Normal,
    Float,
    FloatPlatform,
    Aiming,
    Climbing,
    Aimed,
    KnockBack,
}

public class Fairy_Status_Manager : MonoBehaviour
{
    public FairyStatus status;

    private float AimedTimeCount;
    private Player player;

    private const float AimedDiedTime = 1;

    private const float AimedVibration = 0.1f;
    private const float DeadVibration = 1.0f;
    private const float DeadVibrationTime = 0.2f;
    private const float KnockBackVibration = 0.5f;
    private const float KnockBackVibrationTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        player = ControllerManager.Fairy;
        EventManager.instance.AddHandler<CharacterDied>(OnCharacterDied);
        EventManager.instance.AddHandler<CharacterHitSpineEdge>(OnCharacterHitSpineEdge);
        EventManager.instance.AddHandler<LoadLevel>(OnLoadLevel);
    }

    private void OnDestroy()
    {
        EventManager.instance.RemoveHandler<CharacterDied>(OnCharacterDied);
        EventManager.instance.RemoveHandler<CharacterHitSpineEdge>(OnCharacterHitSpineEdge);
        EventManager.instance.RemoveHandler<LoadLevel>(OnLoadLevel);
    }
    // Update is called once per frame
    void Update()
    {
        FloatGoingDown();
        CheckFloatPlatform();
        CheckAimed();
    }


    private void FloatGoingDown()
    {
        if (status == FairyStatus.Float&&!Freeze_Manager.Frozen)
        {
            GetComponent<CharacterMove>().speed.y = -GetComponent<Gravity_Data>().float_down_speed;
        }
    }

    private void CheckFloatPlatform()
    {
        if (status == FairyStatus.FloatPlatform)
        {
            GetComponent<CharacterMove>().speed = Vector2.zero;
        }
    }

    private void CheckAimed()
    {
        if (!Freeze_Manager.Frozen)
        {
            if (status == FairyStatus.Aimed)
            {
                GetComponent<CharacterMove>().speed = Vector2.zero;
                player.SetVibration(1, AimedVibration, Time.deltaTime);
                AimedTimeCount += Time.deltaTime;
                if (AimedTimeCount > AimedDiedTime)
                {
                    if (Character_Manager.Main_Character.activeSelf && Character_Manager.Fairy.activeSelf)
                    {
                        EventManager.instance.Fire(new CharacterDied(gameObject));

                    }
                    gameObject.SetActive(false);
                }
            }
            else
            {
                AimedTimeCount = 0;
            }
        }
    }

    private void OnCharacterHitSpineEdge(CharacterHitSpineEdge C)
    {
        if (C.Character == gameObject)
        {
            status = FairyStatus.KnockBack;
            player.SetVibration(1, KnockBackVibration, KnockBackVibrationTime);
        }
    }

    private void OnCharacterDied(CharacterDied C)
    {
        if (C.DeadCharacter == gameObject)
        {
            Instantiate(Resources.Load("Prefabs/VFX/FairyDeath"), transform.position, Quaternion.Euler(0, 0, 0));
            player.SetVibration(1, DeadVibration, DeadVibrationTime);
        }
    }

    private void OnLoadLevel(LoadLevel L)
    {
        AimedTimeCount = 0;
        status = FairyStatus.Normal;
    }
}
