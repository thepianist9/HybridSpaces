using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    private PlayerController PlayerController;

    private void Awake()
    {
        PlayerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(true);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == PlayerController.gameObject)
            return;
        PlayerController.SetGroundedState(true);
    }
}
