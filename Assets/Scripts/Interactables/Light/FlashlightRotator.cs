using UnityEngine;
public class FlashlightRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerBodyPartController playerBodyPartController;
    [SerializeField] private Transform groupFlashlightRotator;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private SpriteRenderer spritePlayer;
    [SerializeField] private SpriteRenderer spriteRendererArmFront;
    [SerializeField] private SpriteRenderer spriteRendererArmBack;
    [SerializeField] private SpriteRenderer spriteRendererHeadRight;
    [SerializeField] private SpriteRenderer spriteRendererHeadLeft;
    [SerializeField] private SpriteRenderer spriteRendererHeadCrawlRight;
    [SerializeField] private SpriteRenderer spriteRendererHeadCrawlLeft;
    [SerializeField] private Sprite spriteFrontLeftArm;
    [SerializeField] private Sprite spriteFrontRightArm;
    [SerializeField] private Sprite spriteBackLeftArm;
    [SerializeField] private Sprite spriteBackRightArm;
    [SerializeField] private Sprite spriteHeadNoEye;
    [SerializeField] private Sprite spriteHeadFullEye;
    [SerializeField] private Sprite spriteHeadCrawlNoEye;
    [SerializeField] private Sprite spriteHeadCrawlFullEye;
    [SerializeField] private Transform frontHandAnchor;
    [SerializeField] private Transform backHandAnchor;
    [SerializeField] private Transform headRightAnchor;
    [SerializeField] private Transform headLeftAnchor;
    [SerializeField] private Transform headLeftCrawlAnchor;
    [SerializeField] private Transform headRightCrawlAnchor;
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
        ToggleFlashlightFollow(false);
        currentHandAnchor = frontHandAnchor;
    }
    private void Update()
    {
        UpdateBodySprite();
        SetFacingDirection();
        float targetAngle = GetClampedAngleToMouse();

        currentAngle = rotationSpeed <= 0f
            ? targetAngle
            : Mathf.LerpAngle(currentAngle, targetAngle, 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime));

        // Safety clamp so smoothing can never overshoot the limits
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);

        currentHandAnchor.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
    public void ToggleFlashlightFollow(bool status)
    {
        groupFlashlightRotator.gameObject.SetActive(status);
    }
    public void UpdateBodySprite()
    {
        if(playerBodyPartController.Is1Arm == true && playerBodyPartController.Is1Leg == false && playerBodyPartController.Is2Leg == false)
        {
            spriteRendererArmFront.sprite = spriteBackLeftArm;
            spriteRendererArmFront.sortingOrder = 9;
            spriteRendererArmBack.sprite = spriteFrontLeftArm;
            spriteRendererArmBack.sortingOrder = 11;
        }
        if(playerBodyPartController.Is2Arm == true && playerBodyPartController.Is1Leg == false && playerBodyPartController.Is2Leg == false)
        {
            spriteRendererArmFront.sprite = spriteFrontRightArm;
            spriteRendererArmFront.sortingOrder = 11;
            spriteRendererArmBack.sprite = spriteFrontLeftArm;
            spriteRendererArmBack.sortingOrder = 11;
            currentHandAnchor = headRightAnchor;
            backHandAnchor.gameObject.SetActive(false);
            frontHandAnchor.gameObject.SetActive(false);
            headRightAnchor.gameObject.SetActive(true);
        }
        if (playerBodyPartController.Is2Eye == true)
        {
            spriteRendererHeadCrawlRight.sprite = spriteHeadNoEye;
            spriteRendererHeadCrawlLeft.sprite = spriteHeadNoEye;
            spriteRendererHeadRight.sprite = spriteHeadNoEye;
            spriteRendererHeadLeft.sprite = spriteHeadNoEye;
            spriteRendererHeadCrawlRight.sprite = spriteHeadCrawlNoEye;
            spriteRendererHeadCrawlLeft.sprite = spriteHeadCrawlNoEye;
        }
        if (playerBodyPartController.Is1Eye == true)
        {
            spriteRendererHeadCrawlRight.sprite = spriteHeadNoEye;
            spriteRendererHeadCrawlLeft.sprite = spriteHeadFullEye;
            spriteRendererHeadRight.sprite = spriteHeadNoEye;
            spriteRendererHeadLeft.sprite = spriteHeadFullEye;
            spriteRendererHeadCrawlRight.sprite = spriteHeadCrawlNoEye;
            spriteRendererHeadCrawlLeft.sprite = spriteHeadCrawlFullEye;
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
            
            if(playerBodyPartController.Is1Leg == true || playerBodyPartController.Is2Leg == true)
            {
                currentHandAnchor = headLeftCrawlAnchor;
                headLeftAnchor.gameObject.SetActive(false);
                headRightAnchor.gameObject.SetActive(false);
                headRightCrawlAnchor.gameObject.SetActive(false);
                headLeftCrawlAnchor.gameObject.SetActive(true);
                frontHandAnchor.gameObject.SetActive(false);
                backHandAnchor.gameObject.SetActive(false);
                return;
            }
            else
            {
                if (playerBodyPartController.Is2Arm == true)
                {
                    currentHandAnchor = headLeftAnchor;
                    headLeftAnchor.gameObject.SetActive(true);
                    headRightAnchor.gameObject.SetActive(false);
                    headRightCrawlAnchor.gameObject.SetActive(false);
                    headLeftCrawlAnchor.gameObject.SetActive(false);
                    frontHandAnchor.gameObject.SetActive(false);
                    backHandAnchor.gameObject.SetActive(false);
                    return;
                }
            }

            currentHandAnchor = backHandAnchor;
            backHandAnchor.gameObject.SetActive(true);
            frontHandAnchor.gameObject.SetActive(false);
        }
        else if(facingValue == 1)
        {
            minAngle = minAngleF;
            maxAngle = maxAngleF;

            if (playerBodyPartController.Is1Leg == true || playerBodyPartController.Is2Leg == true)
            {
                currentHandAnchor = headRightCrawlAnchor;
                headLeftAnchor.gameObject.SetActive(false);
                headRightAnchor.gameObject.SetActive(false);
                headRightCrawlAnchor.gameObject.SetActive(true);
                headLeftCrawlAnchor.gameObject.SetActive(false);
                frontHandAnchor.gameObject.SetActive(false);
                backHandAnchor.gameObject.SetActive(false);
                return;
            }
            else
            {
                if (playerBodyPartController.Is2Arm == true)
                {
                    currentHandAnchor = headRightAnchor;
                    headLeftAnchor.gameObject.SetActive(true);
                    headRightAnchor.gameObject.SetActive(false);
                    headRightCrawlAnchor.gameObject.SetActive(false);
                    headLeftCrawlAnchor.gameObject.SetActive(false);
                    frontHandAnchor.gameObject.SetActive(false);
                    backHandAnchor.gameObject.SetActive(false);
                    return;
                }
            }

            currentHandAnchor = frontHandAnchor;
            frontHandAnchor.gameObject.SetActive(true);
            backHandAnchor.gameObject.SetActive(false);
        }
    }
}