using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using Doublsb.Dialog;
using System;
using UnityEngine.Events;

public class ExcelReaderManager : MonoBehaviour
{
    private static ExcelReaderManager _instance;

    public static ExcelReaderManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Busca la instancia en la escena si no ha sido asignada
                _instance = FindAnyObjectByType<ExcelReaderManager>();

                if (_instance == null)
                {
                    // Si no la encuentra, crea un nuevo GameObject con el Singleton
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ExcelReaderManager>();
                    singletonObject.name = typeof(ExcelReaderManager).ToString() + " (Singleton)";

                    // Aseg�rate de que el singleton no se destruya al cargar una nueva escena
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    private void Start()
    {
        if(FindObjectsByType< ExcelReaderManager >(FindObjectsInactive.Include,FindObjectsSortMode.InstanceID).Length>1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    const string PATH_EXCELS = "Excels";

    private int tries = 0;
    private DialogManager _dialogManager;
    const string STAR_NAME = "Star";
    public Dictionary<GameType, List<RowDialogue>> excels;

    public void EnterDialogue(GameType gameType, ConditionType condition, UnityAction callback)
    {
        EnterDialogue(gameType, condition, SituationType.Ninguna, callback);
    }
    public void EnterDialogue(GameType gameType,ConditionType condition, SituationType situation, UnityAction callback)
    {
        if (_dialogManager == null)
            _dialogManager = FindAnyObjectByType<DialogManager>();
        var dialogues = excels[gameType];
        dialogues = dialogues.Where(d => d.Condition == condition.ToString() && CheckSituation(d.Situation, situation)).ToList();
        int lastTry = dialogues.Max(d => d.Tries);
        int findTry = tries <= lastTry ? tries : lastTry;
        dialogues = dialogues.Where(d => d.Tries == findTry).ToList();
        List<DialogData> dialogsData = new List<DialogData>();
        foreach (var d in dialogues)
        {
            dialogsData.Add(new DialogData(d.Message, STAR_NAME));
        }
        if(dialogsData.Count == 0)
        {
            dialogues = excels[gameType];
            dialogues = dialogues.Where(d => d.Condition == condition.ToString()).ToList();
            dialogsData = new List<DialogData>();
            foreach (var d in dialogues)
            {
                dialogsData.Add(new DialogData(d.Message, STAR_NAME));
            }
        }
        dialogsData[dialogsData.Count - 1].Callback = callback;

        _dialogManager.Show(dialogsData);
        ConditionChangeTries(condition);
    }
    private bool CheckSituation(string comparation, SituationType situationType )
    {
        Debug.Log($"Comparation:  {comparation} - situationType: {situationType} - {comparation.ToLower() == situationType.ToString().ToLower()}");
        if (situationType == SituationType.Ninguna)
            return string.IsNullOrEmpty(comparation);
        else
            return comparation.ToLower() == situationType.ToString().ToLower();
    }
    private void ConditionChangeTries(ConditionType condition)
    {
        if (condition == ConditionType.LoseGame)
            tries++;
        else if (condition == ConditionType.WinGame)
            tries = 0;
    }

    void Awake()
    {
        // Inicializa el diccionario
        excels = new Dictionary<GameType, List<RowDialogue>>();

        // Carga todos los archivos CSV en la carpeta Resources/CSV
        LoadCSVFiles();
    }
    private void LoadCSVFiles()
    {
        // Cargar todos los textos que est�n en la carpeta Resources/CSV
        TextAsset[] csvFiles = Resources.LoadAll<TextAsset>(PATH_EXCELS);
        foreach (TextAsset csvFile in csvFiles)
        {
            var rows = new List<RowDialogue>();

            // Dividir el contenido del archivo en l�neas
            string[] lines = csvFile.text.Split('\n');

            // Iterar sobre cada l�nea, comenzando desde la segunda (saltando los t�tulos)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue; // Saltar l�neas vac�as
                }

                // Dividir la l�nea en campos
                string[] values = line.Split(',');

                Debug.Log($"Tries: {int.Parse(values[0].Trim())}");
                Debug.Log($"Condition: {values[1].Trim()}");
                Debug.Log($"Situation: {values[2].Trim()}");
                Debug.Log($"Message: {values[3].Trim()}");
                // Crear un nuevo objeto RowDialogue y asignar sus propiedades
                var rowDialogue = new RowDialogue
                {
                    Tries = int.Parse(values[0].Trim()),
                    Condition = values[1].Trim(),
                    Situation = values[2].Trim(),
                    Message = values[3].Trim(),
                    //AudioName = values.Length > 4 ? values[4].Trim() : string.Empty // Manejar casos donde AudioName puede estar vac�o
                };

                // A�adir el RowDialogue a la lista
                rows.Add(rowDialogue);
            }

            // A�adir la lista de RowDialogue al diccionario usando el nombre del archivo como clave
            string fileName = csvFile.name; // Usa el nombre del archivo sin la extensi�n
            excels.Add(ConvertStringToGame(fileName), rows);
        }
    }
    private GameType ConvertStringToGame(string nameGame)
    {
        switch (nameGame)
        {
            case "Toast":
                return GameType.Toast;
            case "GotoJob":
                return GameType.GotoJob;
            case "Wakeup":
                return GameType.Wakeup;
            default:
                return GameType.Toast;
        }
    }
}
public enum GameType
{
    Toast,
    GotoJob,
    Wakeup
}
public enum ConditionType
{
    Initial,
    WinGame,
    LoseGame
}
public enum SituationType
{
    Ninguna,
    ChocasteEdificio,
    SalisteDelLimites,
    ChocasteValla,
    ChocasteAncianita,
    ChocasteObrero,
    SobreLaMesa,
    CayoMesa,
    Semaforo,
    SeTerminoElTiempo
}

public class RowDialogue
{
    public int Tries { get; set; }
    public string Condition { get; set; }
    public string Situation { get; set; }
    public string Message { get; set; }
    public string AudioName { get; set; }
}