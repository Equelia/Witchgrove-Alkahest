// Portal.cs
// Повесьте этот скрипт на объект-портал в каждой сцене.
// Обязательно: на том же GameObject должен быть Collider с Is Trigger = true.

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Header("Level Requirements")]
    [SerializeField] private int levelToLoad;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private UIWindowGroup levelWarningUIWindowGroup;
    
    [Header("Scene Settings")]
    [Tooltip("Target Scene name to load")]
    [SerializeField] private string targetSceneName;
    [Tooltip("ID of this portal")]
    [SerializeField] private int portalID;
    [Tooltip("ID of the portal, where player should appear in next scene")]
    [SerializeField] private int targetPortalID;
    [Tooltip("Spawn point on scene")]
    [SerializeField] private Transform spawnPoint;
    

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleport(other);
    }

    private void Teleport(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playerData.Level < levelToLoad)
        {
            levelWarningUIWindowGroup?.Show();
            return;
        }
        
        PortalManager.NextPortalID = targetPortalID;
        SceneManager.sceneLoaded += PortalManager.OnSceneLoaded;
        SaveManager.Instance.SaveGame();
        SceneManager.LoadScene(targetSceneName);
    }

    private void Awake()
    {
        if (PortalManager.NextPortalID == portalID)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }
            else
            {
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
            }

            if (cc != null) cc.enabled = true;

            PortalManager.NextPortalID = 0;
        }
    }
}
