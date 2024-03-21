using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Linq;

public class CanvasOKBtnClicked : MonoBehaviour
{
    // csvに書き込む文字列
    private string rowString;
    private StreamWriter sw;
    private FileInfo fi;
    private DateTime Today;
    private string fileName, filePath;
    public int subjectNo = 1;

    public GameObject QuesionnaireCanvas;
    public ToggleGroup ToggleGroupQ1;
    private Toggle selectedToggle;

    // Start is called before the first frame update
    void Start()
    {
        Today = DateTime.Now;
        fileName = Today.Year.ToString() + Today.Month.ToString("D2") + Today.Day.ToString("D2")
            + "_" + Today.Hour.ToString("D2") + Today.Minute.ToString("D2")
            + "_" + "sub" + subjectNo.ToString("D2") + ".csv";
        filePath = Application.dataPath + "/csv/Quesionnaire/" + fileName;
        sw = new StreamWriter(@filePath, false, Encoding.GetEncoding("UTF-8"));

        // ヘッダー作成
        sw.WriteLine("Q1,Q2,Q3,Q4,Q5,Q6,Q7,Q8");

        QuesionnaireCanvas = GameObject.Find("DefaultCanvas");
        if (ToggleGroupQ1 == null)
        {
            //ToggleGroupQ1 = GetComponent <ToggleGroup>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClicked()
    {
        selectedToggle = ToggleGroupQ1.ActiveToggles().FirstOrDefault();
        Debug.Log(selectedToggle);
        rowString = selectedToggle.ToString();
        sw.WriteLine(rowString);
    }

    private void OnApplicationQuit()
    {
        //TODO: swが存在していたら実行
        sw.Flush();
        sw.Close();
    }
}
