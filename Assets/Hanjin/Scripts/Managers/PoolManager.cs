using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    class Pool<T> where T : UnityEngine.Component
    {
        // 풀링 대상 오브젝트
        public GameObject Original { get; private set; }

        // 오브젝트를 담아두는 부모 오브젝트
        public Transform Root { get; set; }

        // 실질적 오브젝트를 관리하는 데이터
        public Stack<T> _poolStack = new Stack<T>();


        // 오리지널 오브젝트 꺼내기
        public GameObject GetOriginal() { return Original; }


        /// <summary>
        /// 
        /// [기능]
        /// 
        /// Pool을 초기화하는 메서드
        /// 1. Original 오브젝트를 저장
        /// 2. 오브젝트를 담아둘 Parent 오브젝트 생성
        /// 3. Count만큼 오브젝트를 생성 후, Push
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="count"></param>
        public void Init(GameObject original, int count = 300)
        {
            Original = original;

            Root = new GameObject { name = $"{Original.name}_Root" }.transform;

            for(int i = 0; i < count; i++)
            {
                Push(Create());
            }
        }


        /// <summary>
        /// 
        /// [기능]
        /// 오브젝트를 생성하는 메서드
        /// 
        /// </summary>
        /// <returns></returns>
        T Create()
        {
            GameObject go = Instantiate(Original);
            go.name = Original.name;

            // 해당 타입으로 오브젝트를 장착
            // 만약 해당 컴포넌트가 없을 시 오브젝트를 장착
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }

            return component;
        }

        public void Push(T obj)
        {
            if (obj == null)
                return;

            obj.transform.parent = Root;
            obj.gameObject.SetActive(false);

            _poolStack.Push(obj);
        }

        public T Pop(Transform parent)
        {
            T obj = null;
            if (_poolStack.Count > 0)
                obj = _poolStack.Pop();
            else
                obj = Create();


            obj.gameObject.SetActive(true);

            obj.transform.parent = parent;
            return obj;
        }
    }

    // 최상위 부모
    private Dictionary<(string, Type), object> poolDict = new Dictionary<(string, Type), object>();

    // 데이터 전부 삭제
    public void Clear()
    {
        poolDict.Clear();
    }


    /// <summary>
    /// 
    /// [기능]
    /// Pool을 생성하는 메서드
    /// 
    /// 제네릭으로 다양한 타입의 풀을 자동으로 생성 가능
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"> 생성 대상 오브젝트 </param>
    /// <param name="count" : 초기 풀오브젝트 개수 ></param>
    public void CreatePool<T>(GameObject original, int count = 50) where T : Component
    {
        var key = (original.name, typeof(T));
        if (poolDict.ContainsKey(key))
        {
            return;
        }

        Pool<T> pool = new Pool<T>();
        pool.Init(original, count);

        pool.Root.SetParent(transform);
        poolDict.Add(key, pool);
    }

    /// <summary>
    /// 
    /// [기능]
    /// 사용완료한 풀 오브젝트를 반납하는 메서드
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public void Push<T>(T obj) where T : Component
    {
        var key = (obj.gameObject.name, typeof(T));
        if (!poolDict.ContainsKey(key))
        {
            Destroy(obj.gameObject);
            return;
        }

        ((Pool<T>)poolDict[key]).Push(obj);
    }



    /// <summary>
    /// 
    /// [기능]
    /// 사용할 오브젝트를 꺼내는 메서드
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <param name="parent"> 꺼내면서 부착할 부모 오브젝트 </param>
    /// <returns></returns>
    public T Pop<T>(GameObject original, Transform parent = null) where T : Component
    {
        var key = (original.name, typeof(T));
        if (!poolDict.ContainsKey(key))
        {
            CreatePool<T>(original);
        }
        return ((Pool<T>)poolDict[key]).Pop(parent);
    }

    // 풀링 오브젝트를 초기화하고 모든 오브젝트를 파괴
    // Scene을 전환할 때 사용
    public void AllReset()
    {
        poolDict.Clear();

        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}