using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }
    /// 
    public bool partitaIniziata = false;


    [SerializeField] private Animator animazioneMenu;
    [SerializeField] private GameObject bottoneAbilita;
    [SerializeField] private GameObject bottoneRematch;
    [SerializeField] private GameObject bottoneExit;
    [SerializeField] private GameObject paBianchi;
    [SerializeField] private GameObject paNeri;
    private void Awake()
    {
        Instance = this;
    }

    //Bottoni
    public void OnLocalGameButton()
    {
        animazioneMenu.SetTrigger("MenuInGame");
        ///
        partitaIniziata = true;
        bottoneAbilita.SetActive(true);
        bottoneRematch.SetActive(true);
        bottoneExit.SetActive(true);
        paBianchi.SetActive(true);
        //paBianchi.GetComponent<Renderer>().material.SetColor("_FaceColor", Color.white);
        //paBianchi.GetComponent<TextMeshProUGUI>().material.SetColor("_FaceColor", Color.white);
        //paBianchi.GetComponent<TextMeshProUGUI>().color = Color.white;
        paNeri.SetActive(true);
        //paNeri.GetComponent<Renderer>().material.SetColor("_FaceColor", Color.black);
    }
    public void OnOnlineGameButton()
    {
        animazioneMenu.SetTrigger("OnlineMenu");
    }
    public void OnOnlineHostButton()
    {
        animazioneMenu.SetTrigger("HostMenu");
    }
    public void OnOnlineConnettiButton()
    {
        Debug.Log("OnOnlineConnettiButton");
    }
    public void OnOnlineBackButton()
    {
        animazioneMenu.SetTrigger("StartMenu");
    }
    public void OnHostBackButton()
    {
        animazioneMenu.SetTrigger("OnlineMenu");
    }
}
