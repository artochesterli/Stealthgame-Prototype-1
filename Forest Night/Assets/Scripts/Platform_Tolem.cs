﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Tolem : MonoBehaviour
{
    public GameObject Connected_Moving_Platform;
    public Vector3 FirstPoint;
    public Vector3 SecondPoint;
    public float move_period;
    public Vector2 CurrentSpeed;
    public bool At_First_Point;

    private bool moving;
    private const float LightAppearTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Move()
    {
        moving = true;
        

        Vector2 direction = Vector2.zero;
        Vector3 StartPoint = Vector3.zero;
        Vector3 EndPoint = Vector3.zero;
        float speed = ((Vector2)(SecondPoint - FirstPoint)).magnitude/move_period;
        if (At_First_Point)
        {
            direction = SecondPoint - FirstPoint;
            StartPoint = FirstPoint;
            EndPoint = SecondPoint;
        }
        else
        {
            direction = FirstPoint - SecondPoint;
            StartPoint = SecondPoint;
            EndPoint = FirstPoint;
        }
        direction.Normalize();

        At_First_Point = !At_First_Point;

        yield return StartCoroutine(LightChange(true));


        CurrentSpeed = speed * direction;
        if (Character_Manager.Main_Character.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Main_Character.GetComponent<CharacterMove>().PlatformSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y);
        }
        if (Character_Manager.Fairy.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Fairy.GetComponent<CharacterMove>().PlatformSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y);
        }

        float timecount = 0;
        while (timecount < move_period)
        {
            if (!Freeze_Manager.Frozen)
            {
                timecount += Time.deltaTime;
            }
            transform.position = Vector3.Lerp(StartPoint, EndPoint, timecount/move_period);
            
            yield return null;
        }

        if (Character_Manager.Main_Character.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Main_Character.transform.position += EndPoint - transform.position;
        }
        if (Character_Manager.Fairy.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Fairy.transform.position += EndPoint - transform.position;
        }
        transform.position = EndPoint;
        CurrentSpeed = Vector2.zero;
        if (Character_Manager.Main_Character.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Main_Character.GetComponent<CharacterMove>().PlatformSpeed = Vector2.zero;
        }
        if (Character_Manager.Fairy.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Fairy.GetComponent<CharacterMove>().PlatformSpeed = Vector2.zero;
        }

        yield return StartCoroutine(LightChange(false));
        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public void Go()
    {
        if (!moving)
        {
            StopAllCoroutines();
            StartCoroutine(Move());
            if (Connected_Moving_Platform != null)
            {
                Connected_Moving_Platform.GetComponent<Platform_Tolem>().StartCoroutine(Connected_Moving_Platform.GetComponent<Platform_Tolem>().Move());
            }
        }
    }

    private IEnumerator LightChange(bool appear)
    {
        GameObject Light = transform.Find("ActivatedLight").gameObject;
        float timecount = 0;
        while (timecount < LightAppearTime)
        {
            if (appear)
            {
                Light.GetComponent<SpriteRenderer>().color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, timecount / LightAppearTime);
            }
            else
            {
                Light.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timecount / LightAppearTime);
            }
            if (!Freeze_Manager.Frozen)
            {
                timecount += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void DisableSelf()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        transform.Find("ActivatedLight").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("Trigger").gameObject.SetActive(false);
        GameObject Totem_Platform = transform.Find("Totem_Platform").gameObject;
        Totem_Platform.SetActive(false);
        this.enabled = false;
    }

    public void EnableSelf()
    {
        CurrentSpeed = Vector2.zero;
        if (At_First_Point)
        {
            transform.position = FirstPoint;
        }
        else
        {
            transform.position = SecondPoint;
        }
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<PolygonCollider2D>().enabled = true;
        transform.Find("ActivatedLight").GetComponent<SpriteRenderer>().enabled = true;
        transform.Find("ActivatedLight").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        transform.Find("Trigger").gameObject.SetActive(true);
        GameObject Totem_Platform = transform.Find("Totem_Platform").gameObject;
        Totem_Platform.SetActive(true);
        this.enabled = true;
    }

}
