using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicShowMessage : MonoBehaviour {

    public AudioSource audioManager;
    public bool CharSounds = true;
    public AudioClip CharSound;
    public bool EndMessageSound = true;
    public AudioClip EndSound;
    public float Volume = 1;
    public GameObject MessageUi;
    public Text messageText;
    public GameObject Continue;
    public string[] messages;
    char[] letters;
    public int currentMessage;
    public bool active;
    public bool button;
    public bool continueS;
    GameObject player;
    public GameObject ReadUI;
	
	// Update is called once per frame
	void Update () {
        button = Input.GetButton("Fire2");

	}


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            player = other.gameObject;
            if (button)
            {
                if (active == false)
                {
                    StartCoroutine(secuencia());
                    active = true;
                }
                if (continueS == true)
                {
                    if (active)
                    {
                        StartCoroutine(secuencia());
                    }
                }
            }
            else
            {
                if(active == false)
                {
                    ReadUI.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            ReadUI.SetActive(false);
        }
    }

    IEnumerator secuencia()
    {
        ReadUI.SetActive(false);
        Continue.SetActive(false);
        messageText.text = "";
        continueS = false;
        player.GetComponent<BasicController>().Pause = true;
        MessageUi.SetActive(true);
        letters = messages[currentMessage].ToCharArray();

        for(int i=0; i < letters.Length; i++)
        {
            messageText.text += letters[i];
            if (CharSounds)
            {
                audioManager.PlayOneShot(CharSound, Volume);
            }
            yield return new WaitForSeconds(0.04f);
        }

        if (EndMessageSound)
        {
            audioManager.PlayOneShot(EndSound, Volume);
        }
        Continue.SetActive(true);
        currentMessage += 1;
        
            if (currentMessage >= messages.Length)
        {
            MessageUi.SetActive(false);
            currentMessage = 0;
            Continue.SetActive(false);
            player.GetComponent<BasicController>().Pause = false;
            yield return new WaitForSeconds(2);
            
            active = false;
            continueS = false;
            yield break;
        }
        if (currentMessage < messages.Length)
        {
            continueS = true;
            yield break;
        }



    }
}
