using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    private Gamepad currentGamepad;
    private float rumbleTimer;

    private void Update()
    {
        if (rumbleTimer > 0)
        {
            rumbleTimer -= Time.deltaTime;
            if (rumbleTimer <= 0)
            {
                StopRumble();
            }
        }
    }

    public void StartRumble(float lowFrequency, float highFrequency, float duration, Gamepad gamepad)
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
        }

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            rumbleTimer = duration;
            currentGamepad = gamepad;
        }
    }

    private void StopRumble()
    {
        if (currentGamepad != null)
        {
            currentGamepad.SetMotorSpeeds(0f, 0f);
            currentGamepad = null;
        }
    }
}