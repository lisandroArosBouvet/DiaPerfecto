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
    public int neederKeys = 10;
    private float _percentBlurRest = 1f;
    public TornBubbleKey bubbleKeyModel;
    public RectTransform bubbleSpawnerArea;

    private KeyCode[] _possibilityKeys = new KeyCode[] {
        KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.T,KeyCode.Y,KeyCode.U,KeyCode.I,KeyCode.O,KeyCode.P,
    KeyCode.A,KeyCode.S,KeyCode.D,KeyCode.F,KeyCode.G,KeyCode.H,KeyCode.J,KeyCode.K,KeyCode.L
    ,KeyCode.Z,KeyCode.X,KeyCode.C,KeyCode.V,KeyCode.B,KeyCode.N,KeyCode.M};
    private Dictionary<KeyCode, TornBubbleKey> _correctKeys = new();

    [Header("Eyelids")]
    public RectTransform upperEyelid;  // Referencia al RectTransform del p�rpado superior
    public RectTransform lowerEyelid;  // Referencia al RectTransform del p�rpado inferior

    public float openStep = 10f;       // Cantidad de p�xeles que se mueven los p�rpados al hacer clic
    public float closeForce = 20f;     // Fuerza que cierra los p�rpados por segundo
    public float maxOpenAmount = 100f; // M�xima distancia que pueden abrirse los p�rpados

    private float _initialUpperPosY;    // Posici�n inicial del p�rpado superior
    private float _initialLowerPosY;    // Posici�n inicial del p�rpado inferior
    private float _currentUpperPosY;    // Posici�n actual del p�rpado superior
    private float _currentLowerPosY;    // Posici�n actual del p�rpado inferior

    private void Start()
    {
        bubbleKeyModel.gameObject.SetActive(false);
        InitalConfiguration();
    }
    private void Update()
    {
        if (_startGame == false)
            return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            // Si se presiona una tecla y no es un bot�n del mouse
            if (Input.GetKeyDown(keyCode) && !IsMouseButton(keyCode))
            {
                PressKey(keyCode);
            }
        }
        cama.position = Vector3.LerpUnclamped(cama.position,camaDestiny.position, smoothTime);
        pared.position = Vector3.Lerp(pared.position,paredDestiny.position, smoothTime);

        // Aplicar la fuerza que cierra los p�rpados constantemente
        _currentUpperPosY -= closeForce * Time.deltaTime;
        _currentLowerPosY += closeForce * Time.deltaTime;

        // Limitar el cierre al estado inicial
        _currentUpperPosY = Mathf.Max(_currentUpperPosY, _initialUpperPosY);
        _currentLowerPosY = Mathf.Min(_currentLowerPosY, _initialLowerPosY);

        if (Input.GetMouseButtonDown(0))
        {
            // Al hacer clic, abrir los p�rpados un poco
            _currentUpperPosY += openStep;
            _currentLowerPosY -= openStep;

            // Limitar la apertura m�xima
            _currentUpperPosY = Mathf.Min(_currentUpperPosY, _initialUpperPosY + maxOpenAmount);
            _currentLowerPosY = Mathf.Max(_currentLowerPosY, _initialLowerPosY - maxOpenAmount);
        }

        // Aplicar el movimiento de los p�rpados
        upperEyelid.anchoredPosition = new Vector2(upperEyelid.anchoredPosition.x, _currentUpperPosY);
        lowerEyelid.anchoredPosition = new Vector2(lowerEyelid.anchoredPosition.x, _currentLowerPosY);
    }
    private bool IsMouseButton(KeyCode keyCode)
    {
        return keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2;
    }
    public void InitalConfiguration()
    {
        // Guardar las posiciones iniciales de los p�rpados
        _initialUpperPosY = upperEyelid.anchoredPosition.y;
        _initialLowerPosY = lowerEyelid.anchoredPosition.y;

        _distanceToRest = maxDistance / (float)neederKeys;
        // Inicializar las posiciones actuales como las iniciales
        _currentUpperPosY = _initialUpperPosY;
        _currentLowerPosY = _initialLowerPosY;
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.Initial, () =>
        {
            _startGame = true;
            watchCooldown.SetTimeAndFinishEvent(timeToFinishGame, () => LoseGame(SituationType.SeTerminoElTiempo));
        });

        for (int i = 0; i < neederKeys; i++)
        {
            var bubble = Instantiate(bubbleKeyModel, bubbleKeyModel.transform.parent);
            bubble.gameObject.SetActive(true);
            KeyCode key = _possibilityKeys[Random.Range(0,_possibilityKeys.Length)];
            while(_correctKeys.ContainsKey(key))
                key = _possibilityKeys[Random.Range(0, _possibilityKeys.Length)];
            _correctKeys.Add(key, bubble);

            bubble.SetKeyText(key);
            Vector2 randomPosition = GetRandomPositionInImage();
            bubble.GetComponent<RectTransform>().anchoredPosition = randomPosition;
        }
    }
    Vector2 GetRandomPositionInImage()
    {
        // Obtener el tama�o de la imagen
        float width = bubbleSpawnerArea.rect.width;
        float height = bubbleSpawnerArea.rect.height;

        // Generar coordenadas aleatorias dentro de la imagen
        float randomX = Random.Range(0, width) - (width * bubbleSpawnerArea.pivot.x);
        float randomY = Random.Range(0, height) - (height * bubbleSpawnerArea.pivot.y);

        return new Vector2(randomX, randomY);
    }
    public void PressKey(KeyCode key)
    {
        if(_correctKeys.ContainsKey(key))
        {
            _correctKeys[key].CorrectKey();
            _correctKeys.Remove(key);
            BlurOut();
            if (_correctKeys.Count <= 0) PreWin();
        }
        else
        {
            LoseGame(SituationType.Ninguna);
        }
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
        _percentBlurRest -= 1f / (float)neederKeys;
        foreach (var sprite in blurSprites)
        {
            sprite.color = new Color(1, 1, 1, _percentBlurRest);
        }
        camaDestiny.transform.position = new Vector2(camaDestiny.position.x, camaDestiny.position.y - _distanceToRest);
        paredDestiny.transform.position = new Vector2(paredDestiny.position.x, paredDestiny.position.y + _distanceToRest);
    }
}
