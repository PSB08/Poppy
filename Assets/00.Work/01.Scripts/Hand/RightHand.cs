using TMPro;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //������Ʈ�� �ְ�
    public Transform[] hands;

    private int currentHandIndex = 0;
    private Transform activeHand;

    private void Start()
    {
        text.gameObject.SetActive(false);
        ChangeHand(0);
    }

    private void Update()
    {
        //1 ������ �迭�� 0������ 2 ������ �迭�� 1��, 3 ������ �迭�� 2������ 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeHand(0);
            text.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeHand(1);
            text.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeHand(2);
            text.gameObject.SetActive(false);
        }
    }

    private void ChangeHand(int index)
    {
        if (index == currentHandIndex) return;

        foreach (Transform h in hands) h.gameObject.SetActive(false);

        hands[index].gameObject.SetActive(true);
        activeHand = hands[index];
        currentHandIndex = index;
    }

}
