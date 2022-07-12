using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    private static GameManger instance;

    public static GameManger Instance { get { return instance; } }

    
    private bool riverPuzzle = false;
    private bool safePuzzle = false;
    private bool griffinPuzzle = false;
    private bool lightPuzzle = false;
    private bool cipherPuzzle = false;
    private bool lazerPuzzle = false;
    private bool torchPuzzle = false;
    private bool controlPanelPuzzle = false;
    public float timer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public bool IsSafeComplete()
    {
        return safePuzzle;
    }
    public bool IsGriffinComplete()
    {
        return griffinPuzzle;
    }

    public bool IsRiverComplete()
    {
        return riverPuzzle;
    }

    public bool IsLightComplete()
    {
        return lightPuzzle;
    }

    public bool IsCipherComplete()
    {
        return cipherPuzzle;
    }

    public bool IsLazerComplete()
    {
        return lazerPuzzle;
    }

    public bool IsTorchComplete()
    {
        return torchPuzzle;
    }

    public bool IsControlPanelComplete()
    {
        return controlPanelPuzzle;
    }

    public void CompleteTorch()
    {
        torchPuzzle = true;
    }

    public void CompleteGriffin()
    {
        griffinPuzzle = true;
    }

    public void CompleteRiver()
    {
        riverPuzzle = true;
    }

    public void CompleteControlPanel()
    {
        controlPanelPuzzle = true;
    }

    public void CompleteSafe()
    {
        safePuzzle = true;
    }

    public void CompleteLazer()
    {
        lazerPuzzle = true;
    }

    public void CompleteCipher()
    {
        cipherPuzzle = true;
    }

    public void CompleteLight()
    {
        lightPuzzle = true;
    }


}
