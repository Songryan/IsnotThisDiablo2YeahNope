using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneManager : MonoBehaviour
{
    public Text loadingText;
    [SerializeField] private GameObject doorA;
    [SerializeField] private GameObject doorB;
    [SerializeField] private MapGenerator mapGenerator;

    private void Start()
    {
        StartCoroutine(LoadBattleScene());
    }

    private IEnumerator LoadBattleScene()
    {
        // MapGenerator�� �� ���� ���� ����
        StartCoroutine(mapGenerator.InitializeAndGenerateRoomsAsync());

        // MapGenerator�� ���൵�� ����͸��ϸ� UI ������Ʈ
        while (mapGenerator.Progress < 1.0f)
        {
            UpdateLoadingUI(mapGenerator.Progress);
            yield return null;
        }

        // �� ������ �Ϸ�Ǹ� ���� ��Ȳ�� 100%�� ����
        UpdateLoadingUI(1.0f);

        // �� �� ��� �� BattleScene���� ��ȯ
        yield return new WaitForSeconds(1.0f);

        // ������ �� ������Ʈ���� DontDestroyOnLoad ����
        foreach (var room in mapGenerator.GeneratedRooms)
        {
            DontDestroyOnLoad(room);
        }

        // BattleScene�� �񵿱�� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene");
        asyncLoad.allowSceneActivation = false;

        // �� ��ȯ�� ���
        asyncLoad.allowSceneActivation = true;

        // �� ��ȯ �Ϸ���� ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // BattleScene���� MapGenerator�� �� ������Ʈ�� �θ�� ����
        MoveGeneratedRoomsToBattleScene();
    }

    private void MoveGeneratedRoomsToBattleScene()
    {
        GameObject mapParent = new GameObject("MapParent");

        foreach (var room in mapGenerator.GeneratedRooms)
        {
            room.transform.SetParent(mapParent.transform);
        }
    }

    private void UpdateLoadingUI(float progress)
    {
        // DoorA�� ȸ�� ����
        float doorARotation = Mathf.Lerp(0, -90, progress);
        doorA.transform.rotation = Quaternion.Euler(-90, 0, doorARotation);

        // DoorB�� ȸ�� ����
        float doorBRotation = Mathf.Lerp(180, 270, progress);
        doorB.transform.rotation = Quaternion.Euler(-90, 0, doorBRotation);

        // �ε� �ؽ�Ʈ ������Ʈ
        loadingText.text = (progress * 100).ToString("F0") + "%";
    }
}
