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

    public float handControlAngVelo = 180.0f;    // 目標とする手先速度 [deg/s]

    // 回転運動用変数
    [SerializeField] private Vector3 _center = Vector3.zero;   // 中心点
    [SerializeField] private Vector3 _axis = Vector3.up;       // 回転軸
    [SerializeField] private float _period = 4.0f;                // 円運動周期 [s]

    // Start is called before the first frame update
    void Start()
    {
        //CalcPeriodByAngVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        // cキーで統制モードON/OFF切り替え
        if (Input.GetKeyDown("c"))
        {
            // 統制オブジェクトの回転半径とスタートpositionを変更
            // 回転する鉛直高さは肩or肘の位置にする
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
                _center,                                // 中心点
                _axis,                                  // 回転軸
                (-1) * 360 / _period * Time.deltaTime   // 回転角度 [deg]
            );
        }
    }

    // 加速度 速度デザインから周期を作成
    void CalcPeriodByAngVelocity()
    {
        _period = handControlAngVelo / 90.0f;
    }
}
