using TMPro;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //오브젝트를 넣고
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
        //1 누르면 배열의 0번으로 2 누르면 배열의 1번, 3 누르면 배열의 2번으로 
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
