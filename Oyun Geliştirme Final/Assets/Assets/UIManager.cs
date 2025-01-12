using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Text scoreText; // Puan göstergesi
    [SerializeField] private Button resetButton; // Sıfırlama düğmesi
    [SerializeField] private Button doublePointsButton; // 2 kat puan düğmesi

    private int score = 0;
    private bool isDoublePointsActive = false; // 2 kat puan özelliği etkin mi?
    private float doublePointsDuration = 6f; // 2 kat puan özelliği süresi
    private float buttonCooldownDuration = 10f; // Düğmenin tekrar etkinleşme süresi

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Düğmelere tıklama olaylarını bağla
        resetButton.onClick.AddListener(ResetGame);
        doublePointsButton.onClick.AddListener(ActivateDoublePoints);

        // Başlangıçta puanı sıfırla
        UpdateScoreUI();
    }

    public void UpdateScore(int points)
    {
        // Eğer 2 kat puan özelliği etkinse, puanı iki katına çıkar
        if (isDoublePointsActive)
        {
            points *= 2;
        }

        // Puanı artır ve arayüzü güncelle
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // Puan metnini güncelle
        scoreText.text = "Score: " + score;
    }

    private void ResetGame()
    {
        // Puanı sıfırla
        score = 0;
        UpdateScoreUI();

        // MatchingArea script'ine ulaşıp oyunu sıfırla
        MatchingArea matchingArea = FindObjectOfType<MatchingArea>();
        if (matchingArea != null)
        {
            matchingArea.ResetGame();
        }
    }

    private void ActivateDoublePoints()
    {
        // Eğer 2 kat puan özelliği zaten aktifse veya düğme devre dışıysa, işlem yapma
        if (!isDoublePointsActive && doublePointsButton.interactable)
        {
            StartCoroutine(DoublePointsCoroutine());
        }
    }

    private IEnumerator DoublePointsCoroutine()
    {
        // 2 kat puan özelliğini etkinleştir
        isDoublePointsActive = true;
        doublePointsButton.interactable = false; // Düğmeyi devre dışı bırak

        float startTime = Time.time; // Başlangıç zamanını al
        while (Time.time < startTime + doublePointsDuration)
        {
            yield return null; // Özellik süresi boyunca bekle
        }

        // 2 kat puan özelliğini devre dışı bırak
        isDoublePointsActive = false;

        // Düğmenin devre dışı kalma süresini başlat
        yield return new WaitForSeconds(buttonCooldownDuration - doublePointsDuration);
        doublePointsButton.interactable = true; // Düğmeyi tekrar etkinleştir
    }
}
