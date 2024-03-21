using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExtendForearm : MonoBehaviour
{
    // Forearm object ex:J_Bip_R_LowerArm (VRoid)
    public GameObject ForearmObj;
    public GameObject HandObj;
    private Vector3 forearmInitBend;
    private Vector3 HandInitPos;
    private Vector3 RHandInitPos;
    // Vive Tracker本体
    public GameObject RHandController;
    public GameObject RElbowController;
    // Final IKが参照するオブジェクト
    public GameObject RHand;  // 右手首
    public GameObject RElbow;  // 右肘
    // [DEBUG] VIVE Tracker3Dモデル
    public GameObject RHandModel;

    // 前腕セグメントの長さ [m]
    public float forearmLength = 0.30f;    //0.30 [m]くらい 固定
    // 仮想腕モデルの総重量(TCP送信)
    public float elbowTipWeight = 2.0f;
    // デフォルトの肩先の重量 [kg]
    private float upperArmWeight;
    public float forearmWeight;
    private float handWeight;
    // 被験者の体重 60 kg固定
    public float subWeight = 60.0f;
    // 全身に対する前腕と手の重さ比
    private const float forearmWeightRaio = 0.03f;
    private const float handWeightRaio = 0.01f;

    private Vector3 localScale;
    private Vector3 parentLossyScale;

    // サイズ拡張倍率
    public float extendRatio = 1.0f;
    // 拡張をストップする倍率
    public float maxExtendRatio = 4.0f;
    // 拡張中の手先位置のoffset 三角補正？
    private float offsetRHand = -0.0f;   // -0.1f?

    private bool isStarted = false;
    private float r;
    private float angle;
    private float initAngle;
    public GameObject StickObj;

    void Start()
    {
        HandInitPos = HandObj.transform.localPosition;
        StickObj = GameObject.Find("Stick");

        RHandInitPos = RHand.transform.localPosition;
        forearmWeight = subWeight * forearmWeightRaio;
        handWeight = subWeight * handWeightRaio;
        localScale = this.transform.localScale;
        parentLossyScale = this.transform.parent.lossyScale;
        // VIVE Trackerが座標連動してから実行
        // Start() function実行直後だとVIVE Trackerの座標が(0.0f, 0.0f, 0.0f)のまま
        // TODO: Trackerが連動したかどうかAPI探す
        StartCoroutine(DelayCoroutine(5.0f, () =>
        {
            //CalcForeArmSegment();
        }));
    }

    private void CalcForeArmSegment()
    {
        // 前腕セグメント長さ計算
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

        // タスク開始
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //forearmInitBend = ForearmObj.transform.eulerAngles;
            //Debug.Log("forearmBend.Y:  " + forearmInitBend.y);
            initAngle = GetAngleByTwoPoints(RElbow.transform.position, RHand.transform.position);

            isStarted = true;
        }

        if (isStarted)
        {
            // 肘のベンド角を直接使う方法(安定しない)
            //float theta = forearmInitBend.y - ForearmObj.transform.eulerAngles.y;
            //Debug.Log(theta);
            //extendRatio = 1.0f + 3.0f * theta / 30.0f;

            // トラッカーの位置関係から角度を推定 Arccos
            //float theta2 = Mathf.Acos(RHand.transform.position.z / r);
            //Debug.Log(theta2);

            angle = GetAngleByTwoPoints(RElbow.transform.position, RHand.transform.position);
            Debug.Log(angle - initAngle);
            // 60.0fで4倍の腕になるように調整
            extendRatio = 1.0f + maxExtendRatio * (angle - initAngle) / 60.0f;

            // 指定の角度になったらスティックを赤くする
            if (angle - initAngle > 60.0f)
            {
                StickObj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            } else if (angle - initAngle < 0.0f)
            {
                StickObj.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            }

            // saturation
            if (extendRatio < 1.0f)
            {
                extendRatio = 1.0f;
            } else if (extendRatio > maxExtendRatio) {
                extendRatio = maxExtendRatio;
            }

            // 身体の重さパラメータ変更
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
                localScale.z / parentLossyScale.z);
        }
    }

    /// <summary>
    /// 中心点から見た対象点の?軸からの角度を取得
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
