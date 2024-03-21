using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{

    // 1: test, 2: quesionnaire
    private int mode = 1;
    [SerializeField] public int subjectNo;
    public GameObject QuesionnaireCanvas;

    // Start is called before the first frame update
    void Start()
    {
        QuesionnaireCanvas = GameObject.Find("DefaultCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        // Reset the scene.
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Log("[event] Reset the scene.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown("q"))
        {
            mode = 2;
        }

        // Questionnaire mode.
        if (mode == 2) {
            Debug.Log("[event] Display quesionnaire.");
            QuesionnaireCanvas.SetActive(true);

            //if (OKƒ{ƒ^ƒ“‚ð‰Ÿ‚µ‚½‚ç)
            //{
            //    SaveQuesionnaireResult();
            //    QuesionnaireCanvas.SetActive(false);
            //}
        }
    }
}
