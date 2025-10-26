using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class DisabilitasManager : MonoBehaviour
{
    [SerializeField] private GameObject wheelChair;
    [SerializeField] private Transform originalParentWheelChair; //xrorigin

    private XROrigin xrOrigin;

    private float playerDisabilityHeight = 0.8f; // Sitting height
    private float playerNormalHeight = 1.6f;     // Standing height

    private Vector3 previousLocalPosition;
    private Quaternion previousLocalRotation;

    private Vector3 lastPosition;
    private float movementThreshold = 0.01f; // Prevent tiny noise from triggering rotation

    [SerializeField] private float rotationYawOffset = 180f; // Default 180 to correct inverted models
    [SerializeField] private bool useSmoothRotation = false;
    [SerializeField] private float rotationSmoothSpeed = 10f;


    private void Start()
    {
        xrOrigin = GetComponent<XROrigin>();

        VisibilityWheelChair(false);
        EventManager.OnModeNormal += HandleModeDisability;

        if (wheelChair != null)
            lastPosition = wheelChair.transform.position;

        EventManager.OnBackToMainMenu += HandleBackToMainMenu;

    }
    private void HandleBackToMainMenu()
    {
        VisibilityWheelChair(false);
    }

    private void Update()
    {
        if (wheelChair != null && wheelChair.activeSelf)
        {
            UpdateWheelchairRotation();
        }
    }

    private void HandleModeDisability(bool isNormal)
    {
        if (!isNormal)
        {
            VisibilityWheelChair(true);
            xrOrigin.CameraYOffset = playerDisabilityHeight; // Lower head
            EventManager.OnDisabilitySit += HandleDisabilitySit;
        }
        else
        {
            VisibilityWheelChair(false);
            xrOrigin.CameraYOffset = playerNormalHeight; // Restore head height
            EventManager.OnDisabilitySit -= HandleDisabilitySit;
        }
    }
    private void HandleDisabilitySit(bool isDisabilitySit, GameObject newParent)
    {
        if (isDisabilitySit)
        {
            // Save current state so we can restore later
            previousLocalPosition = wheelChair.transform.localPosition;
            previousLocalRotation = wheelChair.transform.localRotation;

            // Reparent to the sit location
            wheelChair.transform.SetParent(newParent.transform, worldPositionStays: false);
            wheelChair.transform.localPosition = Vector3.zero;
            wheelChair.transform.localRotation = Quaternion.identity;

            lastPosition = wheelChair.transform.position; // Reset tracking
        }
        else
        {
            // Reparent back to original XR Origin
            wheelChair.transform.SetParent(originalParentWheelChair, worldPositionStays: false);
            wheelChair.transform.localPosition = previousLocalPosition;
            wheelChair.transform.localRotation = previousLocalRotation;
        }
    }

    private void UpdateWheelchairRotation()
    {
        Vector3 currentPosition = wheelChair.transform.position;
        Vector3 movement = currentPosition - lastPosition;

        if (movement.magnitude > movementThreshold)
        {
            Vector3 forward = movement.normalized;
            forward.y = 0f; // keep rotation on horizontal plane

            if (forward.sqrMagnitude > 0.0001f)
            {
                // Create target rotation from movement direction
                Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

                // Apply yaw offset (useful if model faces -Z in modeling tool)
                targetRotation *= Quaternion.Euler(0f, rotationYawOffset, 0f);

                if (useSmoothRotation)
                    wheelChair.transform.rotation = Quaternion.Slerp(wheelChair.transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
                else
                    wheelChair.transform.rotation = targetRotation;
            }
        }

        lastPosition = currentPosition;
    }

    private void VisibilityWheelChair(bool isVisible)
    {
        wheelChair.SetActive(isVisible);
    }
}
