using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneManager : MonoBehaviour
{
    public Text loadingText;

    [SerializeField] GameObject doorA;
    [SerializeField] GameObject doorB;

    void Start()
    {
        StartCoroutine(LoadBattleScene());
    }

    IEnumerator LoadBattleScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene");
        asyncLoad.allowSceneActivation = false;

        // 로딩 진행도 계산
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

        // DoorA의 회전 조정
        float doorARotation = Mathf.Lerp(0, -90, progress);
        doorA.transform.rotation = Quaternion.Euler(-90, 0, doorARotation);

        // DoorB의 회전 조정
        float doorBRotation = Mathf.Lerp(180, 270, progress);
        doorB.transform.rotation = Quaternion.Euler(-90, 0, doorBRotation);

        while (!asyncLoad.isDone)
        {
            loadingText.text = (asyncLoad.progress * 100).ToString("F0") + "%";

            // Scene이 거의 로드되었을 때 (progress가 0.9가 되었을 때)
            if (asyncLoad.progress >= 0.9f)
            {
                // DoorA와 DoorB를 최종 위치로 설정
                doorA.transform.rotation = Quaternion.Euler(-90, 0, -90);
                doorB.transform.rotation = Quaternion.Euler(-90, 0, 270);

                loadingText.text = "100%";
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
