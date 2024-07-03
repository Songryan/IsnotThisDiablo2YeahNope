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

        // �ε� ���൵ ���
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

        // DoorA�� ȸ�� ����
        float doorARotation = Mathf.Lerp(0, -90, progress);
        doorA.transform.rotation = Quaternion.Euler(-90, 0, doorARotation);

        // DoorB�� ȸ�� ����
        float doorBRotation = Mathf.Lerp(180, 270, progress);
        doorB.transform.rotation = Quaternion.Euler(-90, 0, doorBRotation);

        while (!asyncLoad.isDone)
        {
            loadingText.text = (asyncLoad.progress * 100).ToString("F0") + "%";

            // Scene�� ���� �ε�Ǿ��� �� (progress�� 0.9�� �Ǿ��� ��)
            if (asyncLoad.progress >= 0.9f)
            {
                // DoorA�� DoorB�� ���� ��ġ�� ����
                doorA.transform.rotation = Quaternion.Euler(-90, 0, -90);
                doorB.transform.rotation = Quaternion.Euler(-90, 0, 270);

                loadingText.text = "100%";
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
