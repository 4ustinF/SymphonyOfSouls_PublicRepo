using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public static IKController instance;
    private Animator _anim;

    [Header("Right Hand IK")]
    [Range(0, 1)] public float rightHandWeight = 0.0f;
    [Range(0, 1)] public float rightHandHintWeight = 0.0f;
    public Transform rightHandObj = null;
    public Transform rightHandHint = null;

    [Header("Left Hand IK")]
    [Range(0, 1)] public float leftHandWeight = 0.0f;
    [Range(0, 1)] public float leftHandHintWeight = 0.0f;
    public Transform leftHandObj = null;
    public Transform leftHandHint = null;

    [Header("Left Foot IK")]
    [Range(0, 1)] public float leftFootWeight = 0.0f;
    [Range(0, 1)] public float leftFootHintWeight = 0.0f;
    public Transform leftFootObj = null;
    public Transform leftFootHint = null;

    [Header("Right Foot IK")]
    [Range(0, 1)] public float rightFootWeight = 0.0f;
    [Range(0, 1)] public float rightFootHintWeight = 0.0f;
    public Transform rightFootObj = null;
    public Transform rightFootHint = null;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK()
    {
        if (_anim)
        {
            #region RIGHT HAND IK

            if (rightHandObj != null)
            {
                _anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                _anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                _anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                _anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
            if (rightHandHint != null)
            {
                _anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightHandHintWeight);
                _anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
            }

            #endregion

            #region LEFT HAND IK

            if (leftHandObj != null)
            {
                _anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                _anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                _anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                _anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }
            if (rightHandHint != null)
            {
                _anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandHintWeight);
                _anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
            }


            #endregion

            #region LEFT FOOT IK

            if (leftFootObj != null)
            {
                _anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                _anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                _anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
                _anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
            }

            if (leftFootHint != null)
            {
                _anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootHintWeight);
                _anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftFootHint.position);
            }
            #endregion

            #region Right FOOT IK
            if (rightFootObj != null)
            {
                _anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                _anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                _anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                _anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
            }

            if (rightFootHint != null)
            {
                _anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
                _anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightFootHint.position);
            }
            #endregion
        }
    }
    public void SetHandsTransforms(Transform leftHandObject, Transform rightHandObject)
    {
        leftHandObj = leftHandObject;
        rightHandObj = rightHandObject;
    }
    public void SetHintsPositions(Vector3 leftHandHintPos, Vector3 rightHandHintPos)
    {
        leftHandHint.localPosition = leftHandHintPos;
        rightHandHint.localPosition = rightHandHintPos;
    }
}