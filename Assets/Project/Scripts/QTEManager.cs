using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class QTEManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private float dureeQTE = 1f;
    [SerializeField] private float ralentissement = 0.15f;

    [SerializeField] private UIManager uiManager;

    private InputActionMap playerMap;
    private InputActionMap qteMap;

    private InputAction m_qAction;
    private InputAction m_sAction;
    private InputAction m_dAction;

    private Sort sortActuel;

    private int etapeActuelle = 0;
    private float tempsRestant;
    private bool qteActif = false;

    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player");
        qteMap = inputActions.FindActionMap("UI");

        m_qAction = qteMap.FindAction("qte_Q");
        m_sAction = qteMap.FindAction("qte_S");
        m_dAction = qteMap.FindAction("qte_D");
    }

    private void Update()
    {
        if (!qteActif)
            return;

        tempsRestant -= Time.unscaledDeltaTime;

        if (tempsRestant <= 0f)
        {
            EchouerQTE();
            return;
        }

        VerifierTouche();
    }

    public void DemarrerQTE(Sort sort)
    {
        sortActuel = sort;

        etapeActuelle = 0;
        tempsRestant = dureeQTE;
        qteActif = true;

        playerMap.Disable();
        qteMap.Enable();

        Time.timeScale = ralentissement;

        uiManager.AfficherUI();
    }

    private void VerifierTouche()
    {
        switch (etapeActuelle)
        {
            case 0:
                if (m_qAction.WasPressedThisFrame())
                {
                    etapeActuelle++;
                }

                break;

            case 1:
                if (m_sAction.WasPressedThisFrame())
                {
                    etapeActuelle++;
                }

                break;

            case 2:
                if (m_dAction.WasPressedThisFrame())
                {
                    ReussirQTE();
                }

                break;
        }
    }

    private void ReussirQTE()
    {
        qteActif = false;

        Time.timeScale = 1f;

        qteMap.Disable();
        playerMap.Enable();

        uiManager.CacherUI();

        sortActuel.Ameliorer();
    }

    private void EchouerQTE()
    {
        qteActif = false;

        Time.timeScale = 1f;

        qteMap.Disable();
        playerMap.Enable();

        uiManager.CacherUI();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        qteMap.Disable();
        playerMap.Enable();
    }
}
