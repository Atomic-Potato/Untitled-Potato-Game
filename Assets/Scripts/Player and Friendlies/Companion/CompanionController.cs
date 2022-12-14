// NOTES:
// Companion rotation when shooting is handelled in the PivotController script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
#pragma warning disable 0649
    //Publics
    [SerializeField] float switchSpeedIdle;
    [SerializeField] float switchSpeedHorizontal;
    [SerializeField] float switchSpeedVertical;
    [SerializeField] float rotationSpeed;
    [SerializeField] float idleOffset;
    [SerializeField] float moveOffest;
    [SerializeField] float midAirOffset;

    [Space]
    [SerializeField] SpriteRenderer playerSprite;
    public SpriteRenderer companionSprite;

    [SerializeField] PlayerController playerScript;
    [SerializeField] PlayerAnimator playerAnimSp;

    //Privates
    [HideInInspector] public float mouseAngle;

    Vector3 localPos;
    Vector3 defaultPos;

    float rightOffIdle;//locked
    float leftOffIdle;
    float rightOffset;
    float leftOffset;
    float upOffset;
    float downOffset;

    private void Start()
    {
        localPos = transform.parent.InverseTransformPoint(transform.position);
        defaultPos.y = localPos.y;

        rightOffset = localPos.x + moveOffest;
        leftOffset = localPos.x - moveOffest;

        upOffset = localPos.y - midAirOffset;
        downOffset = (localPos.y + midAirOffset)/2;

        rightOffIdle = localPos.x + idleOffset;
        leftOffIdle = localPos.x - idleOffset;
    }

    void FixedUpdate() // ty colorfurrrrr for reminding me to use fixed instead uwu
    {
        HorizontalMovement();
        VerticalMovement();
    }

    void VerticalMovement()
    {
        //We used new Vector3 because the x and y are interfering eachother if we use Vector3.Lerp
        if (playerScript.rigidBody.velocity.y > 1f)
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, upOffset, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
        else if (playerScript.rigidBody.velocity.y < -1f)
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, downOffset, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
        else
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPos.y, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
        
    }

    private void HorizontalMovement()
    {
        if (playerScript.rigidBody.velocity.x < -1f)     //when going left
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, rightOffset, switchSpeedHorizontal * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
        else if (playerScript.rigidBody.velocity.x > 1f)      //when going right
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, leftOffset, switchSpeedHorizontal * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
        else // when idle
        {
            if (playerSprite.flipX == false) //if facing right
            {
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, leftOffIdle, switchSpeedIdle * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
                Rotation(0f);
                companionSprite.flipY = false;
            }
            else //if facing left
            {
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, rightOffIdle, switchSpeedIdle * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
                Rotation(180f);
                companionSprite.flipY = true;
            }
        }
        
    }

    public void Rotation(float angleRotation)
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //I tried using the weapon's rotation but its not actually the same angle between the comp and the mouse, thats because they are not on the same Y• 
        mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private bool VelocityNearZero(float offsetMargin)
    {
        return -offsetMargin >= playerScript.rigidBody.velocity.x && playerScript.rigidBody.velocity.x <= offsetMargin;
    }
}
