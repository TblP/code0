using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class bar_foam : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage barBackground;
    [SerializeField] private RectTransform greenArea;
    [SerializeField] private RectTransform yellowArea;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image Cup1;
    [SerializeField] private Image Cup2;
    [SerializeField] private Image Cup3;

    [Header("Game Settings")]
    [SerializeField] private float yellowAreaSpeed = 100f;
    [SerializeField] private float MaxYellowAreaSpeed = 300f;
    [SerializeField] private bool moveRight = true;
    [SerializeField] private float intersectionThreshold = 20f;
    [SerializeField] private float gameTime = 30f;


    [Header("Obj References")]
    private GameObject gameplayCanvas;
    private GameObject CameraPanel;
    private Animator Foam;
    private ControllerComp controllerPlayer;
    public CoffeeShaderController controllerFoamTexture;

    public Transform MainCam;
    private float currentTime;
    private float barWidth;
    private RectTransform barRect;
    public int score;

    private void Start()
    {
        score = 0;
        controllerPlayer = GameObject.Find("Player").GetComponent<ControllerComp>();

        barBackground = GetComponent<RawImage>();
        greenArea = transform.GetChild(0).GetComponent<RectTransform>();
        yellowArea = transform.GetChild(1).GetComponent<RectTransform>();
        timerText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Cup1 = transform.GetChild(3).GetComponent<Image>();
        Cup2 = transform.GetChild(4).GetComponent<Image>();
        Cup3 = transform.GetChild(5).GetComponent<Image>();

        MainCam = GameObject.Find("Camera").transform;

        CameraPanel = transform.root.Find("CameraP").GetChild(0).gameObject;
        gameplayCanvas = CameraPanel.transform.GetChild(0).gameObject;
        Foam = transform.root.Find("foam").GetComponent<Animator>();

        barRect = barBackground.rectTransform;
        barWidth = barRect.rect.width;

        // Установка начального положения желтой области
        ResetYellowArea();

        // Установка случайного положения зеленой области
        RepositionGreenArea();

        currentTime = gameTime;

        controllerFoamTexture.StartMixing();
    }

    private void Update()
    {
        MoveYellowArea();
        CheckInput();
        UpdateTimer();
    }
    private void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0 || score >= 3)
        {
            currentTime = 0;
            GameOver();
        }

        UpdateUI();
    }
    private void GameOver()
    {
        gameplayCanvas.SetActive(false);
        CameraPanel.SetActive(false);
        MainCam.GetChild(0).gameObject.SetActive(true);
        controllerPlayer.MatorikaOn();
        if (score >= 3)
        {
            Foam.enabled = true;
        }
        

    }

    private void MoveYellowArea()
    {
        float currentX = yellowArea.anchoredPosition.x;

        if (moveRight)
        {
            currentX += yellowAreaSpeed * Time.deltaTime;
            if (currentX >= barWidth / 2)
            {
                moveRight = false;
            }
        }
        else
        {
            currentX -= yellowAreaSpeed * Time.deltaTime;
            if (currentX <= -barWidth / 2)
            {
                moveRight = true;
            }
        }

        yellowArea.anchoredPosition = new Vector2(currentX, yellowArea.anchoredPosition.y);
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsIntersecting())
            {
                score++;
                if (score == 1)
                {
                    Cup1.color = Color.white;
                }
                if (score == 2)
                {
                    Cup3.color = Color.white;
                }
                if (score == 3)
                {
                    Cup2.color = Color.white;
                }
                RepositionGreenArea();
                if (yellowAreaSpeed < MaxYellowAreaSpeed)
                {
                    yellowAreaSpeed += 100;
                }
            }
            else
            {
                RepositionGreenArea();
            }
        }
    }

    private bool IsIntersecting()
    {
        float yellowX = yellowArea.anchoredPosition.x;
        float greenX = greenArea.anchoredPosition.x;

        return Mathf.Abs(yellowX - greenX) < intersectionThreshold;
    }

    private void RepositionGreenArea()
    {
        float randomX = Random.Range((-barWidth+10) / 2, (barWidth-10) / 2);
        greenArea.anchoredPosition = new Vector2(randomX, greenArea.anchoredPosition.y);
    }

    private void ResetYellowArea()
    {
        yellowArea.anchoredPosition = new Vector2(-barWidth / 2, yellowArea.anchoredPosition.y);
        moveRight = true;
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = $"Time: {seconds}";
        }
    }
}
