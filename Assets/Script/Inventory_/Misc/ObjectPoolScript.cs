using System.Collections; // 컬렉션 인터페이스를 사용
using System.Collections.Generic; // 제네릭 컬렉션을 사용
using UnityEngine; // Unity 엔진 기능을 사용

public class ObjectPoolScript : MonoBehaviour // ObjectPoolScript 클래스 정의, MonoBehaviour를 상속
{
    public GameObject prefab; // 생성할 프리팹을 저장할 변수
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>(); // 비활성화된 인스턴스를 저장할 스택

    public GameObject GetObject() // 오브젝트를 풀에서 가져오는 메서드
    {
        GameObject spawnedGameObject;
        if (inactiveInstances.Count > 0) // 비활성화된 인스턴스가 있을 경우
        {
            spawnedGameObject = inactiveInstances.Pop(); // 스택에서 하나를 꺼냄
        }
        else // 비활성화된 인스턴스가 없을 경우
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(prefab); // 새 오브젝트를 생성
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>(); // PooledObject 컴포넌트를 추가
            pooledObject.pool = this; // 풀을 설정
        }
        spawnedGameObject.transform.SetParent(null); // 부모를 설정하지 않음
        spawnedGameObject.SetActive(true); // 오브젝트를 활성화

        return spawnedGameObject; // 생성된 오브젝트를 반환
    }

    public void ReturnObject(GameObject toReturn) // 오브젝트를 풀에 반환하는 메서드
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>(); // PooledObject 컴포넌트를 가져옴

        if (pooledObject != null && pooledObject.pool == this) // 오브젝트가 이 풀에서 생성된 경우
        {
            toReturn.transform.SetParent(transform); // 부모를 설정
            toReturn.transform.position = this.transform.position; // 위치를 설정
            toReturn.SetActive(false); // 오브젝트를 비활성화
            inactiveInstances.Push(toReturn); // 스택에 추가
        }
        else // 오브젝트가 이 풀에서 생성되지 않은 경우
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying."); // 경고 메시지 출력
            Destroy(toReturn); // 오브젝트를 파괴
        }
    }
}

public class PooledObject : MonoBehaviour // PooledObject 클래스 정의, MonoBehaviour를 상속
{
    public ObjectPoolScript pool; // 오브젝트가 속한 풀을 저장할 변수
}
