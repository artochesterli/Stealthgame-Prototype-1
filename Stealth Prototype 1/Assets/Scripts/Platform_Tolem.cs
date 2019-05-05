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
    public bool moving;

    private bool At_First_Point;
    private const float LightAppearTime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        At_First_Point = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Move()
    {
        moving = true;
        StartCoroutine(LightChange(true));

        Vector2 direction = Vector2.zero;
        Vector3 EndPoint = Vector3.zero;
        float speed = ((Vector2)(SecondPoint - FirstPoint)).magnitude/move_period;
        if (At_First_Point)
        {
            direction = SecondPoint - FirstPoint;
            EndPoint = SecondPoint;
        }
        else
        {
            direction = FirstPoint - SecondPoint;
            EndPoint = FirstPoint;
        }
        direction.Normalize();
        yield return null;
        

        CurrentSpeed = speed * direction;
        if (Character_Manager.Main_Character.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Main_Character.GetComponent<CharacterMove>().PlatformSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y);
        }
        if (Character_Manager.Fairy.GetComponent<CharacterMove>().ConnectedMovingPlatform == gameObject)
        {
            Character_Manager.Fairy.GetComponent<CharacterMove>().PlatformSpeed = new Vector2(CurrentSpeed.x, CurrentSpeed.y);
        }
        while (Vector2.Dot(direction, transform.position - EndPoint) < 0)
        {
            transform.position += (Vector3)CurrentSpeed * Time.deltaTime;
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
        At_First_Point = !At_First_Point;
        StartCoroutine(LightChange(false));
        moving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject ob = collision.GetComponent<Collider2D>().gameObject;
        if (ob.CompareTag("Slash") &&!moving)
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
            timecount += Time.deltaTime;
            yield return null;
        }
    }

}
