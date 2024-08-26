using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToastManager : MonoBehaviour, IGameManager
{
    [SerializeField] GameObject toast;
    GameObject copyToast;
    public Transform startPoint;   // Punto de inicio
    public Transform endPoint;     // Punto final
    public float speed = 1.0f;   // Velocidad de oscilación


    private Rigidbody2D[] rbs;
    private bool isHeld = false;
    private float timeToOscilian = 0;

    public float stillThreshold = 0.04f;

    [Header("WinConditions")]
    public SpriteRenderer[] toastInMachineGraphics;
    public int toastToWin = 2;
    private int _toastInMachine = 0;
    private bool _startGame = false;
    const GameType NAME_GAME =  GameType.Toast;

    public string
        NEXT_SCENE = "GotoJobGame",
        LOSE_SCENE = "ToastGame"
        ;

    void Start()
    {
        InitalConfiguration();
    }

    void Update()
    {
        if (_startGame == false)
            return;
        if (isHeld)
        {
            timeToOscilian += Time.deltaTime;
            foreach(var rb in rbs) rb.velocity = new Vector2(Mathf.Sin(timeToOscilian) *speed, 0);
        }else
        {
            if (rbs[0].velocity.magnitude < stillThreshold)
            {
                LoseGame(SituationType.SobreLaMesa);
            }
        }

        // Detectar si el botón del ratón está siendo presionado o soltado
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseBall();
        }
    }

    void ReleaseBall()
    {
        isHeld = false;
        foreach (var rb in rbs)
        {
            rb.gravityScale = 1;
            rb.simulated = true;
        }
        //rb.velocity = CalculateReleaseVelocity(); // Mantener la inercia en el momento de la liberación
    }

    public void LoseGame(SituationType loseType)
    {
        AudioManager.Instance.Fail();
        if (_startGame == false) return;
        _startGame = false;
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.LoseGame,loseType, () => SceneManager.LoadScene(LOSE_SCENE));
    }

    public void WinGame()
    {
        if (_startGame == false)
            return;
        _startGame = false;
        Debug.Log("Esta entrando en wingame");
        if (_toastInMachine < toastInMachineGraphics.Length)
            toastInMachineGraphics[_toastInMachine].enabled = true;
        _toastInMachine++;
        var miga = copyToast.transform.GetChild(4);
        miga.parent = null;
        miga.localScale = Vector3.one;
        Destroy(copyToast);
        if (_toastInMachine >= toastToWin)
        {
            AudioManager.Instance.Ganaste();
            ExcelReaderManager.Instance.EnterDialogue(NAME_GAME, ConditionType.WinGame, () => SceneManager.LoadScene(NEXT_SCENE));
        }
        else
        {
            ResetGame();
            AudioManager.Instance.Tostadora();
            _startGame = true;
        }
    }

    public void ResetGame()
    {
        copyToast = Instantiate(toast, toast.transform.parent);

        timeToOscilian = 0;
        isHeld = true;
        rbs = copyToast.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            rb.gravityScale = 0;
            rb.totalTorque = 0;
            rb.rotation = 0;
            rb.angularVelocity = 0;
            rb.Sleep();
        }
        for (var i = 0; i < rbs.Length; i++)
        {
            if (i == 0)
                rbs[i].position = startPoint.position;
            else
            {
                rbs[i].simulated = false;
            }
        }
    }

    public void InitalConfiguration()
    {
        ResetGame();
        FindAnyObjectByType<DeathZone>().SetLoseGame(LoseGame);
        FindAnyObjectByType<WinZone>().SetWinGame(WinGame);

        foreach (var graphic in toastInMachineGraphics)
        {
            graphic.enabled = false;
        }
        ExcelReaderManager.Instance.EnterDialogue(NAME_GAME,ConditionType.Initial,()=> _startGame = true);

    }
}
