using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    public Gamepad gamepad;
    private float rumbleTimer;

    private void Update()
    {
        // Check of rumble aanstaat en update timer
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
        if (gamepad == null) gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            rumbleTimer = duration;
        }
    }

    private void StopRumble()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}
