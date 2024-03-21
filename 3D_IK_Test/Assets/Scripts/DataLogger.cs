using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class DataLogger : MonoBehaviour
{
    // �T���v�����O����[s] 60fps 16-20 ms�����肪���E?
    //public float span = 0.020f;
    // 1�t���[��������̎��s���� [s]
    //private float deltaTime;
    // �V�[���J�n����̌o�ߎ��� [s]
    //private float elapsedTime;
    //private float elapsedTime2;

    // �ۑ�����f�[�^�i�[�z��
    private double[] logData;
    // csv�ɏ������ޕ�����
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

        // �t�@�C���쐬
        Today = DateTime.Now;
        fileName = Today.Year.ToString() + Today.Month.ToString("D2") + Today.Day.ToString("D2")
            + "_" + Today.Hour.ToString("D2") + Today.Minute.ToString("D2")
            +"_" + "sub" + subjectNo.ToString("D2") + ".csv";
        filePath = Application.dataPath + "/csv/" + fileName;
        // TODO: �t�@�C���������ɂ���ꍇ�̏���
        sw = new StreamWriter(@filePath, false, Encoding.GetEncoding("UTF-8"));
        
        //�����̃t�@�C���ɒǋL
        //fi = new FileInfo(Application.dataPath + "/csv/" + fileName + ".csv");
        // �ŏI�s�ֈړ�
        //sw = fi.AppendText();

        // �w�b�_�[�쐬
        sw.WriteLine("Time.time, preTime");

        // �R���[�`���J�n
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
    // �R���[�`����p�����^�C�}�[ �덷:+15ms -0ms
    private float elapsed;
    private float timeOut = 0.005f;     // �R���[�`���̌Ăяo���b�� [s]
    private float samplingTime = 0.05f; // �������s���T���v�����O���� [s]
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
