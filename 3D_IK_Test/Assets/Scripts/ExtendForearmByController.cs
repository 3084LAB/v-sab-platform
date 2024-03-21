using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ExtendForearmByController : MonoBehaviour
{
    // Forearm object ex:J_Bip_R_LowerArm (VRoid)
    public GameObject ForearmObj;
    public GameObject HandObj;
    private Vector3 forearmInitBend;
    private Vector3 HandInitPos;
    private Vector3 RHandInitPos;
    // Vive Tracker�{��
    public GameObject RHandController;
    public GameObject RElbowController;
    // Final IK���Q�Ƃ���I�u�W�F�N�g
    public GameObject RHand;  // �E���
    public GameObject RElbow;  // �E�I
    // [DEBUG] VIVE Tracker3D���f��
    public GameObject RHandModel;

    // �O�r�Z�O�����g�̒��� [m]
    public float forearmLength = 0.30f;    //0.30 [m]���炢 �Œ�
    // ���z�r���f���̑��d��(TCP���M)
    public float elbowTipWeight = 2.0f;
    // �f�t�H���g�̌���̏d�� [kg]
    private float upperArmWeight;
    public float forearmWeight;
    private float handWeight;
    // �팱�҂̑̏d 60 kg�Œ�
    public float subWeight = 60.0f;
    // �S�g�ɑ΂���O�r�Ǝ�̏d����
    private const float forearmWeightRaio = 0.03f;
    private const float handWeightRaio = 0.01f;

    private Vector3 localScale;
    private Vector3 parentLossyScale;

    // �T�C�Y�g���{��
    public float extendRatio = 1.0f;
    // �g�����X�g�b�v����{��
    public float maxExtendRatio = 4.0f;
    // �g�����̎��ʒu��offset �O�p�␳�H
    private float offsetRHand = -0.0f;   // -0.1f?

    // �R���g���[���g�p
    // InteractUI�{�^����������Ă���̂����肷�邽�߂�Iui�֐���SteamVR_Actions.default_InteractUI���Œ�
    private SteamVR_Action_Boolean Iui = SteamVR_Actions.default_InteractUI;
    private SteamVR_Action_Boolean GrabG = SteamVR_Actions.default_GrabGrip;
    // ���ʂ̊i�[�p
    private Boolean isInteracted;
    private Boolean isGrabbedGrip;

    private bool isStarted = false;
    private float r;
    private float angle;
    private float initAngle;

    void Start()
    {
        HandInitPos = HandObj.transform.localPosition;

        RHandInitPos = RHand.transform.localPosition;
        forearmWeight = subWeight * forearmWeightRaio;
        handWeight = subWeight * handWeightRaio;
        localScale = this.transform.localScale;
        parentLossyScale = this.transform.parent.lossyScale;
        // VIVE Tracker�����W�A�����Ă�����s
        // Start() function���s���ゾ��VIVE Tracker�̍��W��(0.0f, 0.0f, 0.0f)�̂܂�
        // TODO: Tracker���A���������ǂ���API�T��
        StartCoroutine(DelayCoroutine(5.0f, () =>
        {
            //CalcForeArmSegment();
        }));
    }

    private void CalcForeArmSegment()
    {
        // �O�r�Z�O�����g�����v�Z
        forearmLength = Vector3.Distance(RHandController.transform.position, RElbowController.transform.position);
        Debug.Log("forearm legnth: " + forearmLength);
    }

    private IEnumerator DelayCoroutine(float seconds, UnityAction callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        //localScale = this.transform.localScale;
        //parentLossyScale = this.transform.parent.lossyScale;
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            extendRatio = extendRatio + 0.01f;
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            extendRatio = extendRatio - 0.01f;
        }

        isInteracted = Iui.GetState(SteamVR_Input_Sources.LeftHand);
        isGrabbedGrip = GrabG.GetState(SteamVR_Input_Sources.LeftHand);
        //Debug.Log(isInteracted);

        // �^�X�N�J�n
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //forearmInitBend = ForearmObj.transform.eulerAngles;
            //Debug.Log("forearmBend.Y:  " + forearmInitBend.y);
            initAngle = GetAngleByTwoPoints(RElbow.transform.position, RHand.transform.position);

            isStarted = true;
        }

        if (isStarted)
        {

            angle = GetAngleByTwoPoints(RElbow.transform.position, RHand.transform.position);
            Debug.Log(angle - initAngle);
            // 60.0f��4�{�̘r�ɂȂ�悤�ɒ���
            //extendRatio = 1.0f + maxExtendRatio * (angle - initAngle) / 60.0f;
            if (isInteracted)
            {
                extendRatio += 0.05f;
            }
            if (isGrabbedGrip)
            {
                extendRatio -= 0.05f;
            }
            

            // �w��̊p�x�ɂȂ�����X�e�B�b�N��Ԃ�����
            //if (angle - initAngle > 60.0f)
            //{
            //    StickObj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            //} else if (angle - initAngle < 0.0f)
            //{
            //    StickObj.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            //}

            // saturation
            if (extendRatio < 1.0f)
            {
                extendRatio = 1.0f;
            } else if (extendRatio > maxExtendRatio) {
                extendRatio = maxExtendRatio;
            }

            // �g�̂̏d���p�����[�^�ύX
            elbowTipWeight = forearmWeight * extendRatio + handWeight;

            this.transform.parent.localScale = new Vector3(extendRatio, 1.0f, 1.0f);
            //RHandModel.transform.localPosition = new Vector3(forearmLength * (extendRatio - 1.0f) + offsetRHand, 0.0f, 0.0f);
            RHand.transform.localPosition = new Vector3(forearmLength * (extendRatio - 1.0f) + offsetRHand, RHandInitPos.y, RHandInitPos.z);
            //RHand.transform.localPosition = new Vector3(HandInitPos.x, 0.0f, 0.0f);
            //Debug.Log(RHand.transform.localPosition);

            // Restores the scale of a child element(hand)
            // when the scale of the parent element (fore arm, upper arm ...) is changed.
            parentLossyScale = this.transform.parent.lossyScale;
            this.transform.localScale = new Vector3(
                localScale.x / parentLossyScale.x,
                localScale.y / parentLossyScale.y,
                localScale.z / parentLossyScale.z
            );
        }
    }

    /// <summary>
    /// ���S�_���猩���Ώۓ_��?������̊p�x���擾
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    float GetAngleByTwoPoints(Vector3 centerPos, Vector3 targetPos)
    {
        Vector3 dt = targetPos - centerPos;
        float rad = Mathf.Atan2(dt.z, dt.x);
        float degree = rad * Mathf.Rad2Deg;
        return degree;
    }
}
