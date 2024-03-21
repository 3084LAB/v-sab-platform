using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectControl : MonoBehaviour
{
    public GameObject ControlObj;
    public GameObject UpperArmObj;
    public GameObject ForearmObj;

    private bool isControlled = false;
    private Vector3 ControlObjPos;

    public float handControlAngVelo = 180.0f;    // �ڕW�Ƃ����摬�x [deg/s]

    // ��]�^���p�ϐ�
    [SerializeField] private Vector3 _center = Vector3.zero;   // ���S�_
    [SerializeField] private Vector3 _axis = Vector3.up;       // ��]��
    [SerializeField] private float _period = 4.0f;                // �~�^������ [s]

    // Start is called before the first frame update
    void Start()
    {
        //CalcPeriodByAngVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        // c�L�[�œ������[�hON/OFF�؂�ւ�
        if (Input.GetKeyDown("c"))
        {
            // �����I�u�W�F�N�g�̉�]���a�ƃX�^�[�gposition��ύX
            // ��]���鉔�������͌�or�I�̈ʒu�ɂ���
            ControlObj.transform.position = new Vector3(
                0.0f,
                UpperArmObj.transform.position.y,
                0.5f
            );
            _center = new Vector3(UpperArmObj.transform.position.x, UpperArmObj.transform.position.y, UpperArmObj.transform.position.z);

            isControlled = !isControlled;

            if (isControlled)
            {
                ControlObj.SetActive(true);
                Debug.Log("[event] subject control mode ON.");
            }
            else
            {
                ControlObj.SetActive(false);
                Debug.Log("[event] subject control mode OFF.");
            }
        }

        if (isControlled) {
            float sin = Mathf.Sin(Time.time);
            //ControlObjPos = ControlObj.transform.position;
            //ControlObj.transform.position = new Vector3(sin, ControlObjPos.y, ControlObjPos.z);
            ControlObj.transform.RotateAround(
                _center,                                // ���S�_
                _axis,                                  // ��]��
                (-1) * 360 / _period * Time.deltaTime   // ��]�p�x [deg]
            );
        }
    }

    // �����x ���x�f�U�C������������쐬
    void CalcPeriodByAngVelocity()
    {
        _period = handControlAngVelo / 90.0f;
    }
}
