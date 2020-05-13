/*
 * BSS의 후보 Spot 입니다.
 * 각 Spot들은 이 스크립트를 가지며, BSS계산시 점수 지정, 반환을 담당합니다.
 */ 
using UnityEngine;

public class SupportSpot : MonoBehaviour
{
    [SerializeField]
    private Vector2 pos;
    [SerializeField]
    private float score;

    private void Awake()
    {
        pos = transform.position;
        score = 0f;
    }

    // 점수 반환.
    public float GetScore()
    {
        return score;
    }

    // 점수 지정.
    public void SetScore(float value)
    {
        score = value;
    }

}
