// Portal.cs
// Повесьте этот скрипт на объект-портал в каждой сцене.
// Обязательно: на том же GameObject должен быть Collider с Is Trigger = true.

using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Tooltip("Имя сцены для загрузки")]
    [SerializeField] private string targetSceneName;

    [Tooltip("Уникальный ID этого портала в текущей сцене")]
    [SerializeField] private int portalID;

    [Tooltip("ID портала в целевой сцене, в который нужно появиться")]
    [SerializeField] private int targetPortalID;

    [Tooltip("Точка спавна игрока в целевой сцене")]
    [SerializeField] private Transform spawnPoint;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

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
