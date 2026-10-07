using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private EnemySpawner _enemySpawner;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(_playerPrefab,new Vector3(0f, 1f, 0f),Quaternion.identity);

        Character player = playerObject.GetComponent<Character>();
        SpellCaster spellCaster = playerObject.GetComponent<SpellCaster>();

        _cameraController.SetTarget(playerObject.transform);

        _qteManager.SetPlayer(player);
        _qteManager.SetSpellCaster(spellCaster);

        spellCaster.SetQTEManager(_qteManager);
        spellCaster.SetUIManager(_uiManager);
        spellCaster.SetCamera(Camera.main);

        _uiManager.SetPlayer(player);

        _enemySpawner.SetPlayer(player);

    }
}
