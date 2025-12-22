using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemData
{
    public int itemIdx; // 아이템 Idx
    public int itemGroup; // 아이템 그룹.. / 1..장비 / 901..골드 / 902..보석
    public bool itemType; // true = 1 , false = 0 / true..논스택형 / false = 스택형
    public int itemGrade; // 아이템 등급 1 - 일반, 2 - 고급, 3 - 희귀, 4 - 영웅, 5 - 전설, 6 - 신화, 7 - 신성, 8 - 지옥, 9 - 불멸
    public bool itemGradeSpine; //등급 이미지, Spine 사용 여부 판단
    
    public string itemRecordCd; // 아이템 이름 번역 코드
    public string descRecordCd; // 아이템 설명 번역 코드
    public string iconImage; // 아이템 이미지 Name    
}
