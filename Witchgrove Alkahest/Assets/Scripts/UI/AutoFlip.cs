using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour
{
    [SerializeField, Space(15f)] private Book controledBook;

    [Header("Settings")]
    [SerializeField] private float pageFlipTime = 1f;
    [SerializeField] private float timeBetweenPages = 1f;

    private bool isFlipping = false;

    private void Start()
    {
        if (!controledBook)
            controledBook = GetComponent<Book>();

        controledBook.OnFlip.AddListener(new UnityAction(PageFlipped));
    }

    private void PageFlipped()
    {
        isFlipping = false;
        controledBook.SetInteractable(true); // Включаем обратно
    }

    public async UniTaskVoid StartFlipping()
    {
        controledBook.SetInteractable(false);

        float xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2f;
        float xl = ((controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2f) * 0.9f;
        float h = Mathf.Abs(controledBook.EndBottomRight.y) * 0.9f;

        while (controledBook.currentPage < controledBook.TotalPageCount)
        {
            isFlipping = true;
            await FlipRTL(xc, xl, h, pageFlipTime);
            await UniTask.WaitForSeconds(timeBetweenPages);
        }

        controledBook.SetInteractable(true);
        isFlipping = false;
    }

    public void FlipRightPage()
    {
        if (isFlipping || controledBook.currentPage >= controledBook.TotalPageCount) return;

        isFlipping = true;
        controledBook.SetInteractable(false);

        float xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2f;
        float xl = ((controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2f) * 0.9f;
        float h = Mathf.Abs(controledBook.EndBottomRight.y) * 0.9f;

        FlipRTL(xc, xl, h, pageFlipTime).Forget();
    }

    public void FlipLeftPage()
    {
        if (isFlipping || controledBook.currentPage <= 0) return;

        isFlipping = true;
        controledBook.SetInteractable(false);

        float xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2f;
        float xl = ((controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2f) * 0.9f;
        float h = Mathf.Abs(controledBook.EndBottomRight.y) * 0.9f;

        FlipLTR(xc, xl, h, pageFlipTime).Forget();
    }

    private async UniTask FlipRTL(float xc, float xl, float h, float totalTime)
    {
        float elapsed = 0f;
        float startX = xc + xl;
        float endX = xc - xl;

        controledBook.DragRightPageToPoint(new Vector3(startX, GetParabolaY(startX, xc, xl, h), 0));

        while (elapsed < totalTime)
        {
            float t = elapsed / totalTime;
            float x = Mathf.Lerp(startX, endX, t);
            float y = GetParabolaY(x, xc, xl, h);
            controledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));

            await UniTask.Yield(PlayerLoopTiming.Update);
            elapsed += Time.deltaTime;
        }

        controledBook.ReleasePage();
    }

    private async UniTask FlipLTR(float xc, float xl, float h, float totalTime)
    {
        float elapsed = 0f;
        float startX = xc - xl;
        float endX = xc + xl;

        controledBook.DragLeftPageToPoint(new Vector3(startX, GetParabolaY(startX, xc, xl, h), 0));

        while (elapsed < totalTime)
        {
            float t = elapsed / totalTime;
            float x = Mathf.Lerp(startX, endX, t);
            float y = GetParabolaY(x, xc, xl, h);
            controledBook.UpdateBookLTRToPoint(new Vector3(x, y, 0));

            await UniTask.Yield(PlayerLoopTiming.Update);
            elapsed += Time.deltaTime;
        }

        controledBook.ReleasePage();
    }

    private float GetParabolaY(float x, float xc, float xl, float h)
    {
        return (-h / (xl * xl)) * (x - xc) * (x - xc);
    }
}
