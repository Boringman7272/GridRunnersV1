using UnityEngine;
using System.Collections;

public class CameraBob : MonoBehaviour
{
    public Transform cameraTransform; // The transform of the camera
    public float jumpBobAmount = 0.1f; // The amount the camera moves up when jumping
    public float landBobAmount = 0.15f; // The amount the camera moves down when landing
    public float bobSpeed = 10f; // How fast the bob happens

    private Vector3 originalCameraPosition;
    private bool isJumping = false;

    void Start()
    {
        // Store the original position of the camera
        originalCameraPosition = cameraTransform.localPosition;
    }

    public void DoJumpBob()
    {
        isJumping = true;
        StopCoroutine(ApplyBobbingEffect(0));
        StartCoroutine(ApplyBobbingEffect(jumpBobAmount));
    }

    public void DoLandBob()
    {
        isJumping = false;
        StopCoroutine(ApplyBobbingEffect(0));
        StartCoroutine(ApplyBobbingEffect(-landBobAmount));
    }

    IEnumerator ApplyBobbingEffect(float bobAmount)
    {
        // Calculate the target position based on the bob amount
        Vector3 targetPosition = originalCameraPosition + new Vector3(0, bobAmount, 0);
        Vector3 startPosition = cameraTransform.localPosition;
        float t = 0f;

        // Move the camera to the target position with ease out effect
        while (t < 1f)
        {
            t += Time.deltaTime * bobSpeed;
            float easeValue = EaseOutQuad(t); // Use an easing function for a non-linear interpolation
            cameraTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, easeValue);
            yield return null;
        }

        // If it was a jump bob, return to the original position with ease in effect
        if (isJumping)
        {
            t = 0f;
            startPosition = cameraTransform.localPosition;
            while (t < 1f)
            {
                t += Time.deltaTime * bobSpeed;
                float easeValue = EaseInQuad(t); // Use an easing function for a non-linear interpolation
                cameraTransform.localPosition = Vector3.Lerp(startPosition, originalCameraPosition, easeValue);
                yield return null;
            }
        }
    }

    // Easing function for a quadratic ease-out
    float EaseOutQuad(float time)
    {
        return -1 * time * (time - 2);
    }

    // Easing function for a quadratic ease-in
    float EaseInQuad(float time)
    {
        return time * time;
    }
}
