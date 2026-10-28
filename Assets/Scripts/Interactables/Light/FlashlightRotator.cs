using UnityEngine;

public class FlashlightRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private Transform frontHandAnchor;
    [SerializeField] private Transform backHandAnchor;
    private Transform currentHandAnchor;

    [Header("Angle Limits (degrees)")]
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 180f;
    [SerializeField] private float minAngleF = 180f;
    [SerializeField] private float maxAngleF = 180f;
    [SerializeField] private float minAngleB = 180f;
    [SerializeField] private float maxAngleB = 180f;

    [Header("Smoothing")]
    [Tooltip("0 = snap instantly, higher = faster smoothing follow")]
    [SerializeField] private float rotationSpeed = 15f;

    private float currentAngle;

    private void Awake()
    {
        targetCamera = Camera.main;

        currentAngle = Mathf.Clamp(transform.eulerAngles.z, minAngle, maxAngle);
        //currentHandAnchor = frontHandAnchor;
    }

    private void Update()
    {
        float targetAngle = GetClampedAngleToMouse();

        currentAngle = rotationSpeed <= 0f
            ? targetAngle
            : Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime));

        // Safety clamp so smoothing can never overshoot the limits
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        currentHandAnchor.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    private float GetClampedAngleToMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        //mouseScreen.z = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(mouseScreen);

        Vector2 direction = mouseWorld - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        SetFacingDirection();

        if (angle < 0f)
        {
            angle = direction.x >= 0f ? minAngle : maxAngle;
            rotationSpeed = 0;
        }
        else
        {
            rotationSpeed = 15f;
        }       

        return Mathf.Clamp(angle, minAngle, maxAngle);
    }

    public void SetFacingDirection()
    {
        if (spritePlayer.flipX)
        {
            minAngle = minAngleB;
            maxAngle = maxAngleB;
            currentHandAnchor = backHandAnchor;
            backHandAnchor.gameObject.SetActive(true);
            frontHandAnchor.gameObject.SetActive(false);
        }
        else
        {
            minAngle = minAngleF;
            maxAngle = maxAngleF;
            currentHandAnchor = frontHandAnchor;
            frontHandAnchor.gameObject.SetActive(true);
            backHandAnchor.gameObject.SetActive(false);
        }
    }
}