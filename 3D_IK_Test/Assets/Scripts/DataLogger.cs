using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class DataLogger : MonoBehaviour
{
    // サンプリング周期[s] 60fps 16-20 msあたりが限界?
    //public float span = 0.020f;
    // 1フレームあたりの実行時間 [s]
    //private float deltaTime;
    // シーン開始からの経過時間 [s]
    //private float elapsedTime;
    //private float elapsedTime2;

    // 保存するデータ格納配列
    private double[] logData;
    // csvに書き込む文字列
    private string rowString;
    private StreamWriter sw;
    private FileInfo fi;
    private DateTime Today;
    private string fileName, filePath;
    private GameObject EventManager;

    // Start is called before the first frame update
    void Start()
    {
        EventManager = GameObject.Find("EventManager");
        int subjectNo = EventManager.GetComponent<EventManager>().subjectNo;

        // ファイル作成
        Today = DateTime.Now;
        fileName = Today.Year.ToString() + Today.Month.ToString("D2") + Today.Day.ToString("D2")
            + "_" + Today.Hour.ToString("D2") + Today.Minute.ToString("D2")
            +"_" + "sub" + subjectNo.ToString("D2") + ".csv";
        filePath = Application.dataPath + "/csv/" + fileName;
        // TODO: ファイル名が既にある場合の処理
        sw = new StreamWriter(@filePath, false, Encoding.GetEncoding("UTF-8"));
        
        //既存のファイルに追記
        //fi = new FileInfo(Application.dataPath + "/csv/" + fileName + ".csv");
        // 最終行へ移動
        //sw = fi.AppendText();

        // ヘッダー作成
        sw.WriteLine("Time.time, preTime");

        // コルーチン開始
        StartCoroutine("FuncCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        //deltaTime += Time.deltaTime;
        //elapsedTime = Time.realtimeSinceStartup;
        //elapsedTime2 += Time.deltaTime;
        //if (deltaTime >= span)
        //{
        //    //Debug.Log(elapsedTime + " , " + elapsedTime2 + " , " + deltaTime);
        //    deltaTime = 0.0f;
        //}
    }

    // --------------------------------------------------------------------
    // コルーチンを用いたタイマー 誤差:+15ms -0ms
    private float elapsed;
    private float timeOut = 0.005f;     // コルーチンの呼び出し秒数 [s]
    private float samplingTime = 0.05f; // 処理を行うサンプリング周期 [s]
    private float preTime = 0.0f;
    private float corDeltaTime;
    private IEnumerator FuncCoroutine()
    {
        while(true)
        {
            //Debug.Log(Time.time);
            corDeltaTime = Time.time - preTime;
            if (Time.time > this.elapsed)
            {
                SaveCsv(Time.time, preTime);
                this.elapsed = Time.time + samplingTime;
                //Debug.Log(this.elapsed);
            }
            preTime = Time.time;
            yield return new WaitForSeconds(timeOut);
        }
    }

    private void SaveCsv(float x, float y)
    {
        rowString = x + "," + y;
        sw.WriteLine(rowString);
    }

    private void OnApplicationQuit()
    {
        sw.Flush();
        sw.Close();
    }
}
