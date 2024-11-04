﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceRespwanArea : MonoBehaviour
{
    public ResourceFactory resourceFactory;
    public ResourcePool resourcePool;

    private List<string> resourceName;

    [Header("X")]
    public float minXPoint;
    public float maxXPoint;

    [Header("Z")]
    public float minZPoint;
    public float maxZPoint;

    private bool inHuman = false; //collision 안에 사람이 있는지
    public LayerMask groundLayer;
    public float inHumanDistance;

    private void Awake()
    {
        resourceFactory = GetComponent<ResourceFactory>();
        resourcePool = GetComponent<ResourcePool>();
    }

    private void Start()
    {
        resourceName = resourceFactory.GetName();
    }

    private void Update()
    {
        if (inHuman) return;
        CheckResourceSpawn();
    }

    private void CheckResourceSpawn()
    {
        if (resourceName == null) return;
        
        for (int i = 0; i < resourceName.Count; i++)
        {
            if (resourcePool.CheckQueueEmpty(resourceName[i])) //큐의 내용물을 확인하여 Spawning 호출 true 일 때 Dequeue 가능
            {
                Spawning(resourceName[i]);
            }
        }
    }
    private void Spawning(string resourceName) //자원 스폰
    {
        GameObject obj = resourcePool.GetResourceInPool(resourceName);
        //오브젝트 생성위치에 또다른 자원이 있을 경우 다시 재설정
        while (true)
        {
            Vector3 point = ResourceSpawnPoint();

            if (SpawnPointCheck(point))
            {
                obj.transform.position = point;
                break;
            }
        }

        obj.SetActive(true);
    }


    private Vector3 ResourceSpawnPoint() //스폰 지점 설정 로직
    {
        Vector3 point = new Vector3(Random.Range(minXPoint,maxXPoint),0,Random.Range(minZPoint,maxZPoint));
        return transform.position + point;
    }

    private bool SpawnPointCheck(Vector3 point) //스폰 지점을 확인
    {
        if(Physics.Raycast(point + Vector3.up, Vector3.down, out RaycastHit inHit, groundLayer))//Ground 레이어가 있는지 확인
        { 
            return true;
        } 
        return false;
    }

  
    private void OnCollisionStay(Collision collision) //콜라이더 충돌이 지속될시 실행
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, inHumanDistance); //오버랩은 콜라이더 충돌 발생시 해당 지점의 콜라이더 들을 모두 가져옴

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                inHuman = true;
            }     
        }
    }

    private void OnCollisionExit(Collision collision) //콜라이더가 충돌범위를 나갈시 실행 플레이어가 남아있는지 확인
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, inHumanDistance);

        foreach(Collider collider in colliders)
        {
            if (collider == null) 
            {
                inHuman = false;
            }
            else if(!collider.CompareTag("Player") )
            {
                inHuman = false;
            }
        }
    }
}