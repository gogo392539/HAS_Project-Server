using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_PuzzleTile_hanzo : MonoBehaviour
{
    // 현재 타일의 위치 저장을 위한 변수
    public Vector3 TargetPosition;

    // 타일 활성화
    public bool Active = true;

    // 타일이 올바른 위치에 있는지 확인하는 변수
    public bool CorrectLocation = false;

    // 타일의 위치 정보를 저장
    public Vector2 ArrayLocation = new Vector2();
    public Vector2 GridLocation = new Vector2();

    void Awake()
    {
        // 현재 타일의 위치 저장
        TargetPosition = this.transform.localPosition;

        // 새로운 목적지를 향해 이동(섞는 과정??)
        StartCoroutine(UpdatePosition());
    }

    public void LaunchPositionCoroutine(Vector3 newPosition)
    {
        // 새로운 목적지 위치 저장
        TargetPosition = newPosition;

        // 새로운 목적지를 향해 이동
        StartCoroutine(UpdatePosition());
    }

    public IEnumerator UpdatePosition()
    {
        // 목적지에 있지 않을 경우
        while (TargetPosition != this.transform.localPosition)
        {
            // 타일을 이동
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, TargetPosition, 10.0f * Time.deltaTime);
            yield return null;
        }

        // 올바른 위치로 이동을 완료한 후
        if (ArrayLocation == GridLocation)
            CorrectLocation = true;
        else
            CorrectLocation = false;

        // 활성화된 타일이 아니라면 랜더러와 콜라이더를 비활성화
        if (Active == false)
        {
            this.GetComponent<Renderer>().enabled = false;
            this.GetComponent<Collider>().enabled = false;
        }

        yield return null;
    }

    public void ExecuteAdditionalMove()
    {
        // 파일의 위치를 받아서 대상의 위치를 반환
        LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay_hanzo>().GetTargetLocation(this.GetComponent<ST_PuzzleTile_hanzo>()));
    }

    void OnMouseDown()
    {
        // 파일의 위치를 받아서 대상의 위치를 반환
        LaunchPositionCoroutine(this.transform.parent.GetComponent<ST_PuzzleDisplay_hanzo>().GetTargetLocation(this.GetComponent<ST_PuzzleTile_hanzo>()));
    }
}
