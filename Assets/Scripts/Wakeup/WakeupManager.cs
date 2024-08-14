using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WakeupManager : MonoBehaviour, IGameManager
{
    const GameType NAME_GAME = GameType.Wakeup;
    public SoundFX soundFxManager;
    public float timeToFinishGame = 20f;
    public WatchCooldown watchCooldown;
    public string
        NEXT_SCENE = "ToastGame",
        LOSE_SCENE = "WakeupGame"
        ;
    private bool _startGame = false;

    [Header("VisualEffects")]
    public SpriteRenderer[] blurSprites;
    public float smoothTime = 0.3f;  // Tiempo de suavizado
    public Transform
        pared,
        paredDestiny,
        cama,
        camaDestiny
        ;
    public float maxDistance;
    private float _distanceToRest;
    [Header("JustDance")]
    public int neederBubbles = 25;
    private float _percentBlurRest = 1f;
    public TornBubbleKey bubbleKeyModel;
    public RectTransform bubbleSpawnerArea;

    [Header("Eyelids")]
    public RectTransform upperEyelid;  // Referencia al RectTransform del párpado superior
    public RectTransform lowerEyelid;  // Referencia al RectTransform del párpado inferior
    public AnimationCurve openStepForce;
    public int maxClickForForce = 20;
    private float _forceInForStep;
    private float _openStepForce;       // Cantidad de píxeles que se mueven los párpados al hacer clic
    public float openStep;
    public float closeForce = 20f;     // Fuerza que cierra los párpados por segundo
    public float maxOpenAmount = 10; // Máxima distancia que pueden abrirse los párpados

    private float _initialUpperPosY;    // Posición inicial del párpado superior
    private float _initialLowerPosY;    // Posición inicial del párpado inferior
    private float _currentUpperPosY;    // Posición actual del párpado superior
    private float _currentLowerPosY;    // Posición actual del párpado inferior

    public Vector2 _pitchLimits = new Vector2(.5f,2f);
    private float _currentPitch;
    private float _addPitch;

    private void Start()
    {
        bubbleKeyModel.gameObject.SetActive(false);
        InitalConfiguration();
    }
    private void Update()
    {
        if (_startGame == false)
            return;

        cama.position = Vector3.LerpUnclamped(cama.position,camaDestiny.position, smoothTime);
        pared.position = Vector3.Lerp(pared.position,paredDestiny.position, smoothTime);

        // Aplicar la fuerza que cierra los párpados constantemente
        _currentUpperPosY -= closeForce * Time.deltaTime;
        _currentLowerPosY += closeForce * Time.deltaTime;

        // Limitar el cierre al estado inicial
        _currentUpperPosY = Mathf.Max(_currentUpperPosY, _initialUpperPosY);
        _currentLowerPosY = Mathf.Min(_currentLowerPosY, _initialLowerPosY);

        if (Input.GetMouseButtonDown(0))
        {
            // Al hacer clic, abrir los párpados un poco
            _openStepForce += _forceInForStep;
            _currentUpperPosY += openStepForce.Evaluate(_openStepForce)* openStep;
            _currentLowerPosY -= openStepForce.Evaluate(_openStepForce)*openStep;

            // Limitar la apertura máxima
            _currentUpperPosY = Mathf.Min(_currentUpperPosY, _initialUpperPosY + maxOpenAmount);
            _currentLowerPosY = Mathf.Max(_currentLowerPosY, _initialLowerPosY - maxOpenAmount);
        }

        // Aplicar el movimiento de los párpados
        upperEyelid.anchoredPosition = new Vector2(upperEyelid.anchoredPosition.x, _currentUpperPosY);
        lowerEyelid.anchoredPosition = new Vector2(lowerEyelid.anchoredPosition.x, _currentLowerPosY);
    }
    private bool IsMouseButton(KeyCode keyCode)
    {
        return keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2;
    }
    public void InitalConfiguration()
    {
        _forceInForStep = 1f / (float)maxClickForForce;
    // Guardar las posiciones iniciales de los párpados
    _initialUpperPosY = upperEyelid.anchoredPosition.y;
        _initialLowerPosY = lowerEyelid.anchoredPosition.y;

        _distanceToRest = maxDistance / (float)neederBubbles;
        // Inicializar las posiciones actuales como las iniciales
        _currentUpperPosY = _initialUpperPosY;
        _currentLowerPosY = _initialLowerPosY;
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.Initial, () =>
        {
            _startGame = true;
            watchCooldown.SetTimeAndFinishEvent(timeToFinishGame, () => LoseGame(SituationType.SeTerminoElTiempo));

            _currentUpperPosY += maxOpenAmount;
            _currentLowerPosY -= maxOpenAmount;
        });

        for (int i = 0; i < neederBubbles; i++)
        {
            var bubble = Instantiate(bubbleKeyModel, bubbleKeyModel.transform.parent);
            bubble.gameObject.SetActive(true);

            Vector2 randomPosition = GetRandomPositionInImage();
            bubble.GetComponent<RectTransform>().anchoredPosition = randomPosition;
        }
        _currentPitch = _pitchLimits.x;
        float maxPitch = _pitchLimits.y - _pitchLimits.x;
        _addPitch = maxPitch / (float)neederBubbles;
    }
    Vector2 GetRandomPositionInImage()
    {
        // Obtener el tamaño de la imagen
        float width = bubbleSpawnerArea.rect.width;
        float height = bubbleSpawnerArea.rect.height;

        // Generar coordenadas aleatorias dentro de la imagen
        float randomX = Random.Range(0, width) - (width * bubbleSpawnerArea.pivot.x);
        float randomY = Random.Range(0, height) - (height * bubbleSpawnerArea.pivot.y);

        return new Vector2(randomX, randomY);
    }
    public void PressKey()
    {
        soundFxManager.SetPitch(_currentPitch);
        soundFxManager.Burbuja();
        _currentPitch += _addPitch;
        neederBubbles--;
        if (neederBubbles <= 0)
            PreWin();
        BlurOut();
    }
    public void LoseGame(SituationType type)
    {
        soundFxManager.Fail();
        watchCooldown.StopWatch();
        _startGame = false;
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.LoseGame, type, () => SceneManager.LoadScene(LOSE_SCENE));
    }

    private void PreWin()
    {
        _startGame = false;
        upperEyelid.GetComponent<Image>().enabled = false;
        lowerEyelid.GetComponent<Image>().enabled = false;
        Invoke("WinGame",1.5f);
        watchCooldown.StopWatch();
        soundFxManager.Ganaste();
    }
    public void WinGame()
    {
        watchCooldown.StopWatch();
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene(NEXT_SCENE));
    }
    private void BlurOut()
    {
        _percentBlurRest -= 1f / (float)neederBubbles;
        foreach (var sprite in blurSprites)
        {
            sprite.color = new Color(1, 1, 1, _percentBlurRest);
        }
        camaDestiny.transform.position = new Vector2(camaDestiny.position.x, camaDestiny.position.y - _distanceToRest);
        paredDestiny.transform.position = new Vector2(paredDestiny.position.x, paredDestiny.position.y + _distanceToRest);
    }
}
