using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}

[ExecuteInEditMode]
public class Book : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField]
    private RectTransform BookPanel;
    public Sprite background;
    public Sprite[] bookPages;
    public bool interactable = true;
    public bool enableShadowEffect = true;
    public int currentPage = 0;

    public int TotalPageCount => bookPages.Length;
    public Vector3 EndBottomLeft => ebl;
    public Vector3 EndBottomRight => ebr;
    public float Height => BookPanel.rect.height;

    public Image ClippingPlane;
    public Image NextPageClip;
    public Image Shadow;
    public Image ShadowLTR;
    public Image Left;
    public Image LeftNext;
    public Image Right;
    public Image RightNext;
    public UnityEvent OnFlip;

    private float radius1, radius2;
    private Vector3 sb, st, c, ebr, ebl, f;
    private bool pageDragging = false;
    private FlipMode mode;

    void Start()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!canvas) Debug.LogError("Book should be a child to canvas");

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        CalcCurlCriticalPoints();

        float pageWidth = BookPanel.rect.width / 2f;
        float pageHeight = BookPanel.rect.height;
        NextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight * 3);
        ClippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight * 3);

        float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        float shadowPageHeight = pageWidth / 2f + hyp;

        Shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        Shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);
        ShadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        ShadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);
    }

    private void CalcCurlCriticalPoints()
    {
        sb = new Vector3(0, -BookPanel.rect.height / 2f);
        ebr = new Vector3(BookPanel.rect.width / 2f, -BookPanel.rect.height / 2f);
        ebl = new Vector3(-BookPanel.rect.width / 2f, -BookPanel.rect.height / 2f);
        st = new Vector3(0, BookPanel.rect.height / 2f);
        radius1 = Vector2.Distance(sb, ebr);
        float pageWidth = BookPanel.rect.width / 2f;
        float pageHeight = BookPanel.rect.height;
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }

    public Vector3 transformPoint(Vector3 mouseScreenPos)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
            return BookPanel.InverseTransformPoint(mouseWorldPos);
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
            Vector3 globalEBR = transform.TransformPoint(ebr);
            Vector3 globalEBL = transform.TransformPoint(ebl);
            Vector3 globalSt = transform.TransformPoint(st);
            Plane p = new Plane(globalEBR, globalEBL, globalSt);
            if (p.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                return BookPanel.InverseTransformPoint(worldPoint);
            }
        }
        return BookPanel.InverseTransformPoint(mouseScreenPos);
    }

    void Update()
    {
        if (pageDragging && interactable)
            UpdateBook();
    }

    public void UpdateBook()
    {
        f = Vector3.Lerp(f, transformPoint(Input.mousePosition), Time.deltaTime * 10f);
        if (mode == FlipMode.RightToLeft)
            UpdateBookRTLToPoint(f);
        else
            UpdateBookLTRToPoint(f);
    }

    public void UpdateBookLTRToPoint(Vector3 followLocation)
    {
        mode = FlipMode.LeftToRight;
        f = followLocation;
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = Vector3.zero;
        ShadowLTR.transform.localEulerAngles = Vector3.zero;
        Left.transform.SetParent(ClippingPlane.transform, true);

        Right.transform.SetParent(BookPanel.transform, true);
        Right.transform.localEulerAngles = Vector3.zero;
        LeftNext.transform.SetParent(BookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebl, out t1);
        clipAngle = (clipAngle + 180f) % 180f;

        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90f);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        Left.transform.position = BookPanel.TransformPoint(c);
        float angle = Mathf.Atan2(t1.y - c.y, t1.x - c.x) * Mathf.Rad2Deg;
        Left.transform.localEulerAngles = new Vector3(0, 0, angle - 90f - clipAngle);

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90f);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        Right.transform.SetParent(ClippingPlane.transform, true);
        Right.transform.SetAsFirstSibling();

        ShadowLTR.rectTransform.SetParent(Left.rectTransform, true);
    }

    public void UpdateBookRTLToPoint(Vector3 followLocation)
    {
        mode = FlipMode.RightToLeft;
        f = followLocation;
        Shadow.transform.SetParent(ClippingPlane.transform, true);
        Shadow.transform.localPosition = Vector3.zero;
        Shadow.transform.localEulerAngles = Vector3.zero;
        Right.transform.SetParent(ClippingPlane.transform, true);

        Left.transform.SetParent(BookPanel.transform, true);
        Left.transform.localEulerAngles = Vector3.zero;
        RightNext.transform.SetParent(BookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebr, out t1);
        if (clipAngle > -90f) clipAngle += 180f;

        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90f);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        Right.transform.position = BookPanel.TransformPoint(c);
        float angle = Mathf.Atan2(t1.y - c.y, t1.x - c.x) * Mathf.Rad2Deg;
        Right.transform.localEulerAngles = new Vector3(0, 0, angle - (clipAngle + 90f));

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90f);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        RightNext.transform.SetParent(NextPageClip.transform, true);
        Left.transform.SetParent(ClippingPlane.transform, true);
        Left.transform.SetAsFirstSibling();

        Shadow.rectTransform.SetParent(Right.rectTransform, true);
    }

    private float CalcClipAngle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2f;
        float angleCorner = Mathf.Atan2(bookCorner.y - t0.y, bookCorner.x - t0.x);
        float t0t1Angle = 90f - angleCorner;

        float t1x = t0.x - (bookCorner.y - t0.y) * Mathf.Tan(angleCorner);
        t1x = normalizeT1X(t1x, bookCorner, sb);
        t1 = new Vector3(t1x, sb.y, 0);

        t0t1Angle = Mathf.Atan2(t1.y - t0.y, t1.x - t0.x) * Mathf.Rad2Deg;
        return t0t1Angle;
    }

    private float normalizeT1X(float t1x, Vector3 corner, Vector3 sb)
    {
        if (t1x > sb.x && sb.x > corner.x) return sb.x;
        if (t1x < sb.x && sb.x < corner.x) return sb.x;
        return t1x;
    }

    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 cPos;
        float angle = Mathf.Atan2(followLocation.y - sb.y, followLocation.x - sb.x);
        Vector3 r1 = new Vector3(radius1 * Mathf.Cos(angle), radius1 * Mathf.Sin(angle)) + sb;
        cPos = Vector2.Distance(followLocation, sb) < radius1 ? followLocation : r1;

        float angle2 = Mathf.Atan2(cPos.y - st.y, cPos.x - st.x);
        Vector3 r2 = new Vector3(radius2 * Mathf.Cos(angle2), radius2 * Mathf.Sin(angle2)) + st;
        return Vector2.Distance(cPos, st) > radius2 ? r2 : cPos;
    }

    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;

        NextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(0, 0);
        Left.transform.position = RightNext.transform.position;
        Left.transform.eulerAngles = Vector3.zero;
        Left.sprite = currentPage < bookPages.Length ? bookPages[currentPage] : background;
        Left.transform.SetAsFirstSibling();

        Right.gameObject.SetActive(true);
        Right.transform.position = RightNext.transform.position;
        Right.transform.eulerAngles = Vector3.zero;
        Right.sprite = currentPage < bookPages.Length - 1 ? bookPages[currentPage + 1] : background;

        RightNext.sprite = currentPage < bookPages.Length - 2 ? bookPages[currentPage + 2] : background;
        LeftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) Shadow.gameObject.SetActive(true);
        UpdateBookRTLToPoint(f);
    }

    public void OnMouseDragRightPage()
    {
        if (interactable) DragRightPageToPoint(transformPoint(Input.mousePosition));
    }

    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        pageDragging = true;
        mode = FlipMode.LeftToRight;
        f = point;

        NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        Right.gameObject.SetActive(true);
        Right.transform.position = LeftNext.transform.position;
        Right.sprite = bookPages[currentPage - 1];
        Right.transform.eulerAngles = Vector3.zero;
        Right.transform.SetAsFirstSibling();

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(1, 0);
        Left.transform.position = LeftNext.transform.position;
        Left.transform.eulerAngles = Vector3.zero;
        Left.sprite = currentPage >= 2 ? bookPages[currentPage - 2] : background;

        LeftNext.sprite = currentPage >= 3 ? bookPages[currentPage - 3] : background;
        RightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
        UpdateBookLTRToPoint(f);
    }

    public void OnMouseDragLeftPage()
    {
        if (interactable) DragLeftPageToPoint(transformPoint(Input.mousePosition));
    }

    public void OnMouseRelease()
    {
        if (interactable) ReleasePage();
    }

    public void ReleasePage()
    {
        if (!pageDragging) return;
        pageDragging = false;
        float distLeft = Vector2.Distance(c, ebl);
        float distRight = Vector2.Distance(c, ebr);
        if ((distRight < distLeft && mode == FlipMode.RightToLeft) ||
            (distRight > distLeft && mode == FlipMode.LeftToRight))
            _ = TweenBackAsync();
        else
            _ = TweenForwardAsync();
    }

    private async UniTask TweenForwardAsync()
    {
        if (mode == FlipMode.RightToLeft)
            await TweenToAsync(ebl, 0.15f);
        else
            await TweenToAsync(ebr, 0.15f);
        Flip();
    }

    private async UniTask TweenBackAsync()
    {
        if (mode == FlipMode.RightToLeft)
            await TweenToAsync(ebr, 0.15f);
        else
            await TweenToAsync(ebl, 0.15f);
        RestoreAfterBack();
    }

    private void RestoreAfterBack()
    {
        UpdateSprites();
        if (mode == FlipMode.RightToLeft)
        {
            RightNext.transform.SetParent(BookPanel.transform, true);
            Right.transform.SetParent(BookPanel.transform, true);
        }
        else
        {
            LeftNext.transform.SetParent(BookPanel.transform, true);
            Left.transform.SetParent(BookPanel.transform, true);
        }
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        pageDragging = false;
    }

    private async UniTask TweenToAsync(Vector3 target, float duration)
    {
        int steps = Mathf.CeilToInt(duration / 0.025f);
        Vector3 start = f;
        Vector3 delta = (target - start) / steps;
        for (int i = 0; i < steps - 1; i++)
        {
            f += delta;
            if (mode == FlipMode.RightToLeft)
                UpdateBookRTLToPoint(f);
            else
                UpdateBookLTRToPoint(f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.025f));
        }
        f = target;
        await UniTask.Yield();
    }

    private void Flip()
    {
        currentPage += (mode == FlipMode.RightToLeft) ? 2 : -2;
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.transform.SetParent(BookPanel.transform, true);
        Right.transform.SetParent(BookPanel.transform, true);
        RightNext.transform.SetParent(BookPanel.transform, true);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);
        OnFlip?.Invoke();
    }

    private void UpdateSprites()
    {
        LeftNext.sprite = (currentPage > 0 && currentPage <= bookPages.Length) ? bookPages[currentPage - 1] : background;
        RightNext.sprite = (currentPage >= 0 && currentPage < bookPages.Length) ? bookPages[currentPage] : background;
    }
}
