using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    void Start()
    {
        // ��l�� �T������GoToMenu
        Invoke("GoToMenu", 3f);
    }
    void GoToMenu()
    {
        // ����������
        SceneManager.LoadScene("Lobby");
    }
}


