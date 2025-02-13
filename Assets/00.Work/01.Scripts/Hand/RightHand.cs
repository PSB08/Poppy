using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //오브젝트를 넣고
    public List<Transform> hands;

    private int currentHandIndex = 0;
    private Transform activeHand;

    private void Start()
    {
        text.gameObject.SetActive(false);
        ChangeHand(0);
    }

    private void Update()
    {
        //1 누르면 배열의 0번으로 2 누르면 배열의 1번, 3 누르면 배열의 2번으로 
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeHandWithText(0, false);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeHandWithText(1, true);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeHandWithText(2, false);
    }

    private void ChangeHandWithText(int index, bool isTextActive)
    {
        if (index < 0 || index >= hands.Count)
        {
            return;
        }

        ChangeHand(index);

        text.gameObject.SetActive(isTextActive);
    }

    private void ChangeHand(int index)
    {
        if (index == currentHandIndex) return;

        if (index < 0 || index >= hands.Count) return;

        foreach (Transform h in hands) h.gameObject.SetActive(false);

        hands[index].gameObject.SetActive(true);
        activeHand = hands[index];
        currentHandIndex = index;
    }

}
