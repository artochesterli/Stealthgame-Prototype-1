﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Patrol : MonoBehaviour
{
    public float Patrol_Speed;
    public float Patrol_Right_pos_x;
    public float Patrol_Left_pos_x;
    public float Observation_Right_Time;
    public float Observation_Left_Time;
    public bool Patrol_Right;

    private float Observation_Time_Count;
    private bool IsObserving;
    private bool Observing_Right;
    // Start is called before the first frame update
    void Start()
    {
        
        Observation_Time_Count = 0;
        if (Patrol_Speed == 0)
        {
            IsObserving = true;
        }
        else
        {
            IsObserving = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        var Enemy_Status = GetComponent<Enemy_Status_Manager>();
        if (Enemy_Status.status==EnemyStatus.Patrol)
        {
            Patrol();
        }
        else
        {
            Observation_Time_Count = 0;
        }
    }

    private void Patrol()
    {
        GameObject Indicator = transform.Find("Indicator").gameObject;
        Indicator.GetComponent<SpriteRenderer>().enabled = false;
        if (!IsObserving)
        {
            if (Patrol_Right)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                transform.position += Patrol_Speed * Vector3.right * Time.deltaTime;
                if (transform.position.x > Patrol_Right_pos_x)
                {
                    transform.position = new Vector3(Patrol_Right_pos_x, transform.position.y, 0);
                    Patrol_Right = false;
                    if (Observation_Right_Time > 0)
                    {
                        Observing_Right = true;
                        IsObserving = true;
                    }
                }
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                transform.position += Patrol_Speed * Vector3.left * Time.deltaTime;
                if (transform.position.x < Patrol_Left_pos_x)
                {
                    transform.position = new Vector3(Patrol_Left_pos_x, transform.position.y, 0);
                    Patrol_Right = true;
                    if (Observation_Left_Time > 0)
                    {
                        Observing_Right = false;
                        IsObserving = true;
                    }
                }
            }

        }
        else
        {
            Observation_Time_Count += Time.deltaTime;
            if (Observing_Right)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                if (Observation_Time_Count > Observation_Right_Time)
                {
                    Observation_Time_Count = 0;
                    if (Patrol_Speed > 0)
                    {
                        IsObserving = false;
                    }
                    else
                    {
                        if (Observation_Left_Time > 0)
                        {
                            Observing_Right = false;
                        }
                    }
                    
                }
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                if (Observation_Time_Count > Observation_Left_Time)
                {
                    Observation_Time_Count = 0;
                    if (Patrol_Speed > 0)
                    {
                        IsObserving = false;
                    }
                    else
                    {
                        if (Observation_Right_Time > 0)
                        {
                            Observing_Right = true;
                        }
                    }
                }
            }
        }
    }

}
