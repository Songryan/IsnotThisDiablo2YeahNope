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
        // MapGenerator의 방 생성 로직 실행
        StartCoroutine(mapGenerator.InitializeAndGenerateRoomsAsync());

        // MapGenerator의 진행도를 모니터링하며 UI 업데이트
        while (mapGenerator.Progress < 1.0f)
        {
            UpdateLoadingUI(mapGenerator.Progress);
            yield return null;
        }

        // 방 생성이 완료되면 진행 상황을 100%로 설정
        UpdateLoadingUI(1.0f);

        // 몇 초 대기 후 BattleScene으로 전환
        yield return new WaitForSeconds(1.0f);

        // 생성된 방 오브젝트들을 DontDestroyOnLoad 설정
        foreach (var room in mapGenerator.GeneratedRooms)
        {
            DontDestroyOnLoad(room);
        }

        // BattleScene을 비동기로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene");
        asyncLoad.allowSceneActivation = false;

        // 씬 전환을 허용
        asyncLoad.allowSceneActivation = true;

        // 씬 전환 완료까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // BattleScene에서 MapGenerator의 방 오브젝트를 부모로 설정
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
        // DoorA의 회전 조정
        float doorARotation = Mathf.Lerp(0, -90, progress);
        doorA.transform.rotation = Quaternion.Euler(-90, 0, doorARotation);

        // DoorB의 회전 조정
        float doorBRotation = Mathf.Lerp(180, 270, progress);
        doorB.transform.rotation = Quaternion.Euler(-90, 0, doorBRotation);

        // 로딩 텍스트 업데이트
        loadingText.text = (progress * 100).ToString("F0") + "%";
    }
}
