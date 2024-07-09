using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPointReset : MonoBehaviour
{
    [SerializeField] BoxCollider BoxCollider;
    void Start()
    {
        BoxCollider = transform.GetComponent<BoxCollider>();
        BoxCollider.isTrigger = true;  // 콜라이더를 트리거로 설정
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject objectToDestroy = GameObject.Find("MapParent");
            GameObject objectToDestroy2 = GameObject.Find("GamePlayManager");
            GameObject objectToDestroy3 = GameObject.Find("InGameUIManager");
            Destroy(objectToDestroy);
            Destroy(objectToDestroy2);
            Destroy(objectToDestroy3);

            //GamePlayManager.Instance.ReFresh();
            //RefreshUI();

            SceneManager.LoadScene("LoadingScene");
        }
    }

}
