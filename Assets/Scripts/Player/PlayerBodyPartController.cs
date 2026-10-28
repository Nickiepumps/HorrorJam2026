using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBodyPartController : MonoBehaviour
{
    // For Debug only
    [SerializeField] private Toggle arm1Toggle;
    [SerializeField] private Toggle arm2Toggle;
    [SerializeField] private Toggle leg1Toggle;
    [SerializeField] private Toggle leg2Toggle;
    [SerializeField] private Toggle eyeLeftToggle;
    [SerializeField] private Toggle eyeRightToggle;
    [SerializeField] private Toggle ear1Toggle;
    [SerializeField] private Toggle ear2Toggle;
    [SerializeField] private Toggle flaslightToggle;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animatorPlayer;

    [SerializeField] private Image imageBlindLeft;
    [SerializeField] private Image imageBlindRight;

    private bool isLeftEye;
    private bool isRightEye;
    private bool is1Eye;
    private bool is2Eye;
    private bool is1Leg;
    private bool is2Leg;
    private bool is1Arm;
    private bool is2Arm;
    private bool is1Ear;
    private bool is2Ear;

    private bool isToggleFlashLight;
    public bool IsToggleFlashLight => isToggleFlashLight;
    public bool Is1Eye => is1Eye;
    public bool Is2Eye => is2Eye;
    public bool Is1Arm => is1Arm;
    public bool Is2Arm => is2Arm;
    public bool Is1Leg => is1Leg;
    public bool Is2Leg => is2Leg;

    private List<IBodyPart> removedBodyPartList = new List<IBodyPart>();
    private void Start()
    {
        arm1Toggle.onValueChanged.AddListener(Arm1);
        arm2Toggle.onValueChanged.AddListener(Arm2);
        leg1Toggle.onValueChanged.AddListener(Leg1);
        leg2Toggle.onValueChanged.AddListener(Leg2);
        eyeLeftToggle.onValueChanged.AddListener(BlindLeft);
        eyeRightToggle.onValueChanged.AddListener(BlindRight);
        //ear1Toggle.onValueChanged.AddListener(UpdateBodyStatus);
        //eye2Toggle.onValueChanged.AddListener(UpdateBodyStatus);
        flaslightToggle.onValueChanged.AddListener(ToggleFlashLightBodyPart);
    }
    public void ToggleFlashLightBodyPart(bool toggle)
    {
        isToggleFlashLight = toggle;
    }
    private void BlindLeft(bool toggle)
    {
        isLeftEye = toggle;
        imageBlindLeft.enabled = toggle;

        if (isRightEye == true)
        {
            is1Eye = false;
            is2Eye = true;
            if (is1Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 3f);
            }
            if (is2Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 6f);
            }
            animatorPlayer.SetFloat("BlendEye", 3f);
        }
        else
        {
            is1Eye = true;
            is2Eye = false;
            if (is1Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 2f);
            }
            if (is2Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 5f);
            }
            animatorPlayer.SetFloat("BlendEye", 2f);
        }
    }
    private void BlindRight(bool toggle)
    {
        isRightEye = toggle;
        imageBlindRight.enabled = toggle;
        if (isLeftEye == true)
        {
            is1Eye = false;
            is2Eye = true;
            if (is1Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 3f);
            }
            if (is2Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 6f);
            }
            animatorPlayer.SetFloat("BlendEye", 3f);
        }
        else
        {
            is1Eye = true;
            is2Eye = false;
            if (is1Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 2f);
            }
            if (is2Leg == true)
            {
                animatorPlayer.SetFloat("BlendY", 5f);
            }
            animatorPlayer.SetFloat("BlendEye", 2f);
        }
    }
    private void Arm1(bool toggle)
    {
        is1Arm = toggle;
        is2Arm = false;
        if (is1Leg == true)
        {
            animatorPlayer.SetFloat("BlendY", 1f);
            return;
        }
    }
    private void Arm2(bool toggle)
    {
        is2Arm = toggle;
        is1Arm = false;
    }
    private void Leg1(bool toggle)
    {
        playerController.isCrawl = toggle;
        is1Leg = toggle;
        is2Leg = false;
        if (toggle == true)
        {
            animatorPlayer.SetBool("IsCrawl", true);
            animatorPlayer.SetBool("isRun", false);
            animatorPlayer.SetBool("isWalk", false);
            animatorPlayer.SetBool("isIdle", false);
            animatorPlayer.SetFloat("BlendY", 0f);
        }
        else
        {
            animatorPlayer.SetBool("IsCrawl", false);
            animatorPlayer.SetFloat("BlendX", 0f);
            animatorPlayer.SetFloat("BlendY", 0f);
        }
    }
    private void Leg2(bool toggle)
    {
        playerController.isCrawl = toggle;
        is2Leg = toggle;
        is1Leg = false;
        if(toggle == true)
        {
            animatorPlayer.SetBool("IsCrawl", true);
            animatorPlayer.SetBool("isRun", false);
            animatorPlayer.SetBool("isWalk", false);
            animatorPlayer.SetBool("isIdle", false);
            animatorPlayer.SetFloat("BlendY", 3f);
        }
        else
        {
            animatorPlayer.SetBool("IsCrawl", false);
            animatorPlayer.SetFloat("BlendX", 0f);
            animatorPlayer.SetFloat("BlendY", 0f);
        }   
    }
    public void AddRemovedBodyPart(IBodyPart bodyPart)
    {
        removedBodyPartList.Add(bodyPart);
    }

}
