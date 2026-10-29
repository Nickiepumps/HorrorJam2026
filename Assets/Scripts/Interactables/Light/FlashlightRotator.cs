using UnityEngine;
public class FlashlightRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerBodyPartController playerBodyPartController;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private SpriteRenderer spriteRendererArmFront;
    [SerializeField] private SpriteRenderer spriteRendererArmBack;
    [SerializeField] private Sprite spriteFrontLeftArm;
    [SerializeField] private Sprite spriteFrontRightArm;
    [SerializeField] private Sprite spriteBackLeftArm;
    [SerializeField] private Sprite spriteBackRightArm;
    [SerializeField] private Transform frontHandAnchor;
    [SerializeField] private Transform backHandAnchor;
    [SerializeField] private Transform headAnchor;
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
    private float facingValue;

    private void Awake()
    {
        targetCamera = Camera.main;

        currentAngle = Mathf.Clamp(transform.eulerAngles.z, minAngle, maxAngle);
    }
    private void Start()
    {
        
        if (playerBodyPartController.Is2Arm == true)
        {
            currentHandAnchor = headAnchor;
        }
        else
        {
            currentHandAnchor = frontHandAnchor;
        }
    }

    private void Update()
    {
        UpdateBodySprite(); // Temp
        SetFacingDirection();
        float targetAngle = GetClampedAngleToMouse();

        currentAngle = rotationSpeed <= 0f
            ? targetAngle
            : Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime));

        // Safety clamp so smoothing can never overshoot the limits
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        currentHandAnchor.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    public void UpdateBodySprite()
    {
        if(playerBodyPartController.Is1Arm == true)
        {
            spriteRendererArmFront.sprite = spriteBackLeftArm;
            spriteRendererArmFront.sortingOrder = 9;
            spriteRendererArmBack.sprite = spriteFrontLeftArm;
            spriteRendererArmBack.sortingOrder = 11;
            return;
        }
        if(playerBodyPartController.Is2Arm == true)
        {
            spriteRendererArmFront.sprite = spriteFrontRightArm;
            spriteRendererArmFront.sortingOrder = 11;
            backHandAnchor.gameObject.SetActive(false);
            frontHandAnchor.gameObject.SetActive(false);
            headAnchor.gameObject.SetActive(true);
            return;
        }

    }
    private float GetClampedAngleToMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(mouseScreen);

        Vector2 direction = mouseWorld - transform.position;

        float angle = 0;
        if (facingValue == -1)
        {
            angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        }
        else if(facingValue == 1)
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        return ClampToArc(angle, minAngle, maxAngle);
    }
    private static float ClampToArc(float angle, float min, float max)
    {
        float center = (min + max) * 0.5f;
        float half = (max - min) * 0.5f;
        float delta = Mathf.DeltaAngle(center, angle);
        return center + Mathf.Clamp(delta, -half, half);
    }
    public void SetFacingDirection()
    {
        if (Input.GetAxisRaw("Horizontal") == 1) facingValue = 1;
        if (Input.GetAxisRaw("Horizontal") == -1) facingValue = -1;

        if (facingValue == -1)
        {
            minAngle = minAngleB;
            maxAngle = maxAngleB;
            currentHandAnchor = backHandAnchor;
            backHandAnchor.gameObject.SetActive(true);
            frontHandAnchor.gameObject.SetActive(false);
        }
        else if(facingValue == 1)
        {
            minAngle = minAngleF;
            maxAngle = maxAngleF;
            currentHandAnchor = frontHandAnchor;
            frontHandAnchor.gameObject.SetActive(true);
            backHandAnchor.gameObject.SetActive(false);
        }
    }
}