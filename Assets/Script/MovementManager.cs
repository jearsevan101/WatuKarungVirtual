using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementManager : MonoBehaviour
{
    [Header("References")]
    public ActionBasedContinuousMoveProvider continuousMoveProvider;
    public ActionBasedSnapTurnProvider snapTurnProvider;

    private bool isContinuous = true;

    void Start()
    {
        SetMovementMode(isContinuous);
    }

    void Update()
    {
        // Oculus Quest B button = "joystick button 1"
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            isContinuous = !isContinuous;
            SetMovementMode(isContinuous);
        }
    }

    private void SetMovementMode(bool continuous)
    {
        continuousMoveProvider.enabled = continuous;
        snapTurnProvider.enabled = !continuous;

        Debug.Log(continuous ? "Switched to Continuous Movement" : "Switched to Snap Turn");
    }
}
