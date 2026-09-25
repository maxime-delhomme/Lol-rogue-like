using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class QTEManager : MonoBehaviour
{
    [Header("input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerMap;
    private InputActionMap qteMap;
    private InputAction[] qteActions;

    [Header("Touche")]
    private string[] nomsTouches =
    {
        "A",
        "Z",
        "E",
        "R",
        "Q",
        "S",
        "D",
        "F"
    };

    [Header("QTE")]
    private int[] sequenceQTE = new int[3];
    [SerializeField] private float dureeQTE = 1f;
    [SerializeField] private float ralentissement = 0.15f;
    private int etapeActuelle = 0;
    private float tempsRestant;
    private bool qteActif = false;

    [SerializeField] private UIManager uiManager;
    private Sort sortActuel;



    private void Awake()
    {
        playerMap = inputActions.FindActionMap("Player");
        qteMap = inputActions.FindActionMap("UI");

        qteActions = new InputAction[]
        {
            qteMap.FindAction("qte_A"),
            qteMap.FindAction("qte_Z"),
            qteMap.FindAction("qte_E"),
            qteMap.FindAction("qte_R"),
            qteMap.FindAction("qte_Q"),
            qteMap.FindAction("qte_S"),
            qteMap.FindAction("qte_D"),
            qteMap.FindAction("qte_F")
        };
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

        GenererSequence();

        playerMap.Disable();
        qteMap.Enable();

        Time.timeScale = ralentissement;

        uiManager.AfficherUI(nomsTouches[sequenceQTE[0]], nomsTouches[sequenceQTE[1]], nomsTouches[sequenceQTE[2]]);
    }

    private void GenererSequence()
    {
        int premiereTouche = UnityEngine.Random.Range(0, qteActions.Length);

        int deuxiemeTouche = UnityEngine.Random.Range(0, qteActions.Length);

        while (deuxiemeTouche == premiereTouche)
        {
            deuxiemeTouche = UnityEngine.Random.Range(0, qteActions.Length);
        }

        int troisiemeTouche = UnityEngine.Random.Range(0, qteActions.Length);

        while (troisiemeTouche == premiereTouche || troisiemeTouche == deuxiemeTouche)
        {
            troisiemeTouche = UnityEngine.Random.Range(0, qteActions.Length);
        }

        sequenceQTE[0] = premiereTouche;
        sequenceQTE[1] = deuxiemeTouche;
        sequenceQTE[2] = troisiemeTouche;
    }

    private void VerifierTouche()
    {
        InputAction toucheActuelle = qteActions[sequenceQTE[etapeActuelle]];

        if (toucheActuelle.WasPressedThisFrame())
        {
            uiManager.CacherTouche(etapeActuelle);

            etapeActuelle++;

            if (etapeActuelle >= 3)
            {
                ReussirQTE();
            }

            return;
        }

        for (int i =0; i < qteActions.Length; i++)
        {
            if (i == sequenceQTE[etapeActuelle])
            {
                continue;
            }

            if (qteActions[i].WasPressedThisFrame())
            {
                EchouerQTE();
                return;
            }
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
