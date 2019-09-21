using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityClientA;
using System.Threading;

public class Action : MonoBehaviour
{
    public GameObject[] roads;
    public Material oldMat;
    public Material newMat;

    const float DELTIME = 0.2f;
    private float timeVal = DELTIME;
    private int point = 0;

    private delegate void RightEvent();
    private delegate void LeftEvent();
    private delegate void UpEvent();

    RightEvent rEvent;
    LeftEvent lEvent;
    UpEvent uEvent;

    private UnityClient client;

    private void GetKey(RightEvent rightEvent, LeftEvent leftEvent, UpEvent upEvent)
    {
        timeVal -= Time.deltaTime;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h > 0)
        {
            if (timeVal <= 0)
            {
                timeVal = DELTIME;
                rightEvent();
            }
        }
        else if (h < 0)
        {
            if (timeVal <= 0)
            {
                timeVal = DELTIME;
                leftEvent();
            }
        }
        if (v > 0)
        {
            if (timeVal <= 0)
            {
                timeVal = DELTIME;
                upEvent();
            }
        }


    }

    private void GetStatus()
    {
        Debug.Log("Get status");
        byte[] res = client.GetBits(0);
        return;
    }

    private void Act()
    {

        client.SetBit(0, 11);
        Thread.Sleep(1000);
        Debug.Log("11 down");

        client.SetBit(0, 21);
        Thread.Sleep(1000);
        Debug.Log("21 down");


        client.ResetBit(0, 11);
        Thread.Sleep(1000);
        Debug.Log("11 up");

        client.ResetBit(0, 21);
        Thread.Sleep(1000);
        Debug.Log("21 up");


    }

    private void LastRoad()
    {
        roads[point].GetComponent<Renderer>().material = oldMat;
        client.SetBit(0, point);

        point--;
        if (point <= -1)
        {
            point = roads.Length - 1;
        }
        roads[point].GetComponent<Renderer>().material = newMat;
        client.SetBit(0, point);

    }

    private void NextRoad()
    {
        roads[point].GetComponent<Renderer>().material = oldMat;
        client.SetBit(0, point);

        point++;
        if (point >= roads.Length)
        {
            point = 0;
        }
        roads[point].GetComponent<Renderer>().material = newMat;
        client.SetBit(0, point);

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("running");
        rEvent = new RightEvent(this.NextRoad);
        lEvent = new LeftEvent(this.LastRoad);
        uEvent = new UpEvent(this.Act);
        client = new UnityClient();
        if (client.Init("192.168.16.112", 9100) < 0)
        {
            Debug.Log("Connect peer failed");
            return;
        }
        Debug.Log("Init ok");

    }

    // Update is called once per frame
    void Update()
    {
        GetKey(rEvent, lEvent, uEvent);
    }
}
