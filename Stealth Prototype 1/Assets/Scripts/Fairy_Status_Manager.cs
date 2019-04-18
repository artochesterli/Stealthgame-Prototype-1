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
    Transporting,
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
        player = GetComponent<PlayerId>().player;
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
        SetInvisibility();
        FloatGoingDown();
        set_status();
        check_aimed();
    }

    private void SetInvisibility()
    {
        if (status == FairyStatus.Normal)
        {
            GetComponent<Invisible>().AbleToInvisible = true;
        }
        else if (status == FairyStatus.Float || status == FairyStatus.FloatPlatform || status == FairyStatus.Climbing || status == FairyStatus.Aiming || status == FairyStatus.Transporting || status == FairyStatus.Aimed)
        {
            GetComponent<Invisible>().AbleToInvisible = false;
        }
    }

    private void FloatGoingDown()
    {
        if (status == FairyStatus.Float&&!Freeze_Manager.freeze)
        {
            GetComponent<CharacterMove>().speed.y = -GetComponent<Gravity_Data>().float_down_speed;
        }
    }

    private void set_status()
    {
        var FairySprite = GetComponent<SpriteRenderer>();
        if (status == FairyStatus.Normal)
        {
            if (GetComponent<CharacterMove>().OnGround)
            {
                FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalGround", typeof(Sprite)) as Sprite;
            }
            else
            {
                FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalJump", typeof(Sprite)) as Sprite;
            }
        }
        else if (status == FairyStatus.Float)
        {
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyGlide", typeof(Sprite)) as Sprite;
        }
        else if (status == FairyStatus.Aiming)
        {
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalGround", typeof(Sprite)) as Sprite;
        }
        else if (status == FairyStatus.Climbing)
        {
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalGround", typeof(Sprite)) as Sprite;
        }
        else if (status == FairyStatus.FloatPlatform)
        {
            GetComponent<CharacterMove>().speed = Vector2.zero;
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyFloat", typeof(Sprite)) as Sprite;
        }
        else if (status == FairyStatus.Transporting)
        {
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalGround", typeof(Sprite)) as Sprite;
        }
        else if (status == FairyStatus.Aimed)
        {
            FairySprite.sprite = Resources.Load("Sprite/CharacterSprite/FairyNormalGround", typeof(Sprite)) as Sprite;
        }
    }

    private void check_aimed()
    {
        if (status == FairyStatus.Aimed)
        {
            GetComponent<CharacterMove>().speed = Vector2.zero;
            player.SetVibration(1, AimedVibration, Time.deltaTime);
            AimedTimeCount += Time.deltaTime;
            if (AimedTimeCount > AimedDiedTime)
            {
                EventManager.instance.Fire(new CharacterDied(gameObject));
                gameObject.SetActive(false);
            }
        }
        else
        {
            AimedTimeCount = 0;
        }
    }

    private void OnCharacterHitSpineEdge(CharacterHitSpineEdge C)
    {
        if (C.Character == gameObject)
        {
            player.SetVibration(1, KnockBackVibration, KnockBackVibrationTime);
        }
    }

    private void OnCharacterDied(CharacterDied C)
    {
        if (C.DeadCharacter == gameObject)
        {
            player.SetVibration(1, DeadVibration, DeadVibrationTime);
        }
    }

    private void OnLoadLevel(LoadLevel L)
    {
        status = FairyStatus.Normal;
    }
}
