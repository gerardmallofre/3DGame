using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxeState { STILL, ROTATING, HIT, AXEDOWN }

public class AxeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float rotCooldown = 2f;
    [SerializeField] float rotDuration = 0.3f;
    [SerializeField] float hitDelay = 0.5f;
    [SerializeField] float hitDuration = 0.2f;
    [SerializeField] float retractDuration = 0.5f;
    [SerializeField] GameObject axe;
    Quaternion initRot;
    Quaternion axeInitRot;
    Direction currentdir=Direction.RIGHT;
    [SerializeField] int rotDirection = 1;
    float time = 0f;
    AxeState state = AxeState.ROTATING;
    void Start()
    {
        initRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > rotCooldown)
        {
            time = 0;
            initRot = transform.rotation;
            state = AxeState.ROTATING;
        }
        else if (time > rotDuration + hitDelay + hitDuration + retractDuration && state == AxeState.AXEDOWN)
        {
            state = AxeState.STILL;
            axe.transform.localRotation = axeInitRot;
        }
        else if (time > rotDuration + hitDelay + hitDuration)
        {
            if (time > rotDuration + hitDelay + hitDuration + retractDuration/2) state = AxeState.AXEDOWN;
            axe.transform.Rotate(0, 0, Time.deltaTime / retractDuration * 90);
        }
        else if (time > rotDuration + hitDelay)
        {
            if (time > rotDuration + hitDelay + hitDuration/3) state = AxeState.HIT;
            axe.transform.Rotate(0, 0, -90 * Time.deltaTime / hitDuration);
        }
        else if (time >= rotDuration && state == AxeState.ROTATING)
        {
            transform.rotation = initRot;
            transform.Rotate(0, rotDirection * 90, 0);
            initRot = transform.rotation;
            state = AxeState.STILL;
            axeInitRot = axe.transform.localRotation;
            currentdir = rotate(currentdir, rotDirection);
        }
        else if (time < rotDuration)
        {
            transform.Rotate(0, rotDirection * Time.deltaTime / rotDuration * 90, 0);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (state == AxeState.HIT)
        {
            GameObject obj = other.gameObject;
            if (obj.tag == "Player")
            {
                obj.GetComponent<PlayerHandler>().takeDamage(1, currentdir);
            }
            IEnemy ie = obj.GetComponent<IEnemy>();
            if (ie != null)
            {
                ie.die(currentdir);
            }
        }
    }

    public Direction rotate(Direction d, int rd)
    {
        int tmp;
        if (d == Direction.UP) tmp = 0;
        else if (d == Direction.RIGHT) tmp = 1;
        else if (d == Direction.DOWN) tmp = 2;
        else tmp = 3;
        tmp += rd;
        if (tmp > 3) tmp = 0;
        else if (tmp < 0) tmp = 3;
        if (tmp == 0) return Direction.UP;
        else if (tmp == 1) return Direction.RIGHT;
        else if (tmp == 2) return Direction.DOWN;
        else return Direction.LEFT;
    }
}