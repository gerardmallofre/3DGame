using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Sprites")]
    [SerializeField] private Sprite heartFull, heartEmpty, coinSprite, doorSprite;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int totalRooms = 10;

    const float U = 64f, GAP = 10f, PAD = 14f, TEXT_W = 90f, MARGIN = 24f, FONT = 38f, RADIUS = 16f;

    static readonly Color C_HEARTS = new Color(0.75f, 0.08f, 0.08f, 0.55f);
    static readonly Color C_COINS = new Color(0.85f, 0.65f, 0.05f, 0.55f);
    static readonly Color C_ROOMS = new Color(0.08f, 0.38f, 0.80f, 0.55f);

    List<Image> heartImages = new List<Image>();
    TextMeshProUGUI coinText, roomText;
    RectTransform heartContainer;
    int currentHealth, currentCoins, currentRoom = 1;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Canvas setup
        var c = GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 10;
        var s = GetComponent<CanvasScaler>() ?? gameObject.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1920, 1080);
        s.matchWidthOrHeight = 0.5f;
        if (!GetComponent<GraphicRaycaster>()) gameObject.AddComponent<GraphicRaycaster>();

        // Hearts panel
        float heartsW = maxHealth * (U + GAP) - GAP + PAD * 2;
        var hp = Panel("HeartsPanel", C_HEARTS, heartsW, U + PAD * 2, new Vector2(0, 1), new Vector2(MARGIN, -MARGIN));
        heartContainer = Child<RectTransform>(hp, "HeartsContainer");
        heartContainer.anchorMin = heartContainer.anchorMax = new Vector2(0, 0.5f);
        heartContainer.pivot = new Vector2(0, 0.5f);
        heartContainer.anchoredPosition = new Vector2(PAD, 0);
        heartContainer.sizeDelta = new Vector2(heartsW - PAD * 2, U);
        var hlg = heartContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = GAP; hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childForceExpandWidth = hlg.childForceExpandHeight = false;
        hlg.childControlWidth = hlg.childControlHeight = false;

        // Coins panel
        float pw = PAD + TEXT_W + GAP + U + PAD;
        var cp = Panel("CoinsPanel", C_COINS, pw, U + PAD * 2, new Vector2(1, 1), new Vector2(-MARGIN, -MARGIN));
        coinText = LabelAndIcon(cp, "Coin", "00", coinSprite, new Color(1f, 0.88f, 0.12f, 1f));

        // Rooms panel
        float yOff = -(MARGIN + U + PAD * 2 + 10f);
        var rp = Panel("RoomsPanel", C_ROOMS, pw, U + PAD * 2, new Vector2(1, 1), new Vector2(-MARGIN, yOff));
        roomText = LabelAndIcon(rp, "Room", "1/" + totalRooms, doorSprite, new Color(0.65f, 0.88f, 1f, 1f));
    }

    void Start()
    {
        currentHealth = maxHealth; currentCoins = 0; currentRoom = 1;
        foreach (Transform t in heartContainer) Destroy(t.gameObject);
        heartImages.Clear();
        for (int i = 0; i < maxHealth; i++)
        {
            var go = new GameObject("Heart_" + i);
            go.transform.SetParent(heartContainer, false);
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(U, U);
            var img = go.AddComponent<Image>();
            img.sprite = heartFull; img.preserveAspect = true;
            heartImages.Add(img);
        }
        RefreshCoins(); RefreshRooms();
    }

    RectTransform Panel(string name, Color col, float w, float h, Vector2 anchor, Vector2 pos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.sizeDelta = new Vector2(w, h);
        rt.anchoredPosition = pos;
        var bg = go.AddComponent<RawImage>();
        bg.texture = RoundedTex(Mathf.RoundToInt(w), Mathf.RoundToInt(h), Mathf.RoundToInt(RADIUS), col);
        return rt;
    }

    TextMeshProUGUI LabelAndIcon(RectTransform panel, string id, string val, Sprite spr, Color fallback)
    {
        // Text
        var tGO = new GameObject(id + "Text");
        tGO.transform.SetParent(panel, false);
        var tRT = tGO.AddComponent<RectTransform>();
        tRT.anchorMin = tRT.anchorMax = tRT.pivot = new Vector2(0, 0.5f);
        tRT.anchoredPosition = new Vector2(PAD, 0);
        tRT.sizeDelta = new Vector2(TEXT_W, U);
        var tmp = tGO.AddComponent<TextMeshProUGUI>();
        tmp.text = val; tmp.fontSize = FONT; tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.MidlineRight;

        // Icon
        var iGO = new GameObject(id + "Icon");
        iGO.transform.SetParent(panel, false);
        var iRT = iGO.AddComponent<RectTransform>();
        iRT.anchorMin = iRT.anchorMax = iRT.pivot = new Vector2(0, 0.5f);
        iRT.anchoredPosition = new Vector2(PAD + TEXT_W + GAP, 0);
        iRT.sizeDelta = new Vector2(U, U);
        var img = iGO.AddComponent<Image>();
        if (spr != null) { img.sprite = spr; img.preserveAspect = true; }
        else img.color = fallback;

        return tmp;
    }

    // Creates a RectTransform child (used for heartContainer)
    T Child<T>(RectTransform parent, string name) where T : Component
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<T>();
    }

    static Texture2D RoundedTex(int w, int h, int r, Color fill)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        var clear = new Color(0, 0, 0, 0);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float dx = Mathf.Max(0, Mathf.Max(r - x, x - (w - 1 - r)));
                float dy = Mathf.Max(0, Mathf.Max(r - y, y - (h - 1 - r)));
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(r + 0.5f - dist);
                var c = fill; c.a = fill.a * alpha;
                tex.SetPixel(x, y, dist <= r ? c : clear);
            }
        tex.Apply();
        return tex;
    }

    void RefreshHearts() { for (int i = 0; i < heartImages.Count; i++) heartImages[i].sprite = i < currentHealth ? heartFull : heartEmpty; }
    void RefreshCoins() { if (coinText) coinText.text = currentCoins.ToString("D2"); }
    void RefreshRooms() { if (roomText) roomText.text = currentRoom + "/" + totalRooms; }

    public void SetHealth(int v) { currentHealth = Mathf.Clamp(v, 0, maxHealth); RefreshHearts(); }
    public void AddCoin() { currentCoins++; RefreshCoins(); }
    public void SetRoom(int v) { currentRoom = Mathf.Clamp(v, 1, totalRooms); RefreshRooms(); }
    public void ResetHUD() { currentHealth = maxHealth; currentCoins = 0; currentRoom = 1; Start(); }
}