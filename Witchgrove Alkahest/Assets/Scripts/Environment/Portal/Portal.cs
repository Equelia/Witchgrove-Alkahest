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
        // чтобы в инспекторе всегда был триггер
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 1) Запоминаем, в какой портал хотим попасть
        PortalManager.NextPortalID = targetPortalID;

        // 2) Подписываемся на событие, чтобы очистить подписку сразу после загрузки
        SceneManager.sceneLoaded += PortalManager.OnSceneLoaded;

        // 3) Загружаем сцену
        SceneManager.LoadScene(targetSceneName);
    }

    private void Awake()
    {
        // Когда сцена загружена, все порталы Awake() отработают.
        // Мы проверяем, совпадает ли наш portalID с тем, что запомнили ранее.
        if (PortalManager.NextPortalID == portalID)
        {
            // найдём игрока и временно выключим его контроллер, чтобы избежать конфликтов при телепорте
            var player = GameObject.FindGameObjectWithTag("Player");
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // ставим позицию и ротацию
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

            // включаем контроллер обратно
            if (cc != null) cc.enabled = true;

            // сбрасываем, чтобы другие порталы не подхватили это значение
            PortalManager.NextPortalID = 0;
        }
    }
}
