using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using Doublsb.Dialog;
using System;
using UnityEngine.Events;
using UnityEngine.AI;

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

                    // Asegúrate de que el singleton no se destruya al cargar una nueva escena
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }


    string pathExcels = "Excels";

    private int tries = 0;
    private DialogManager _dialogManager;
    const string STAR_NAME = "Star";
    public Dictionary<GameType, List<RowDialogue>> excels;
    
    public void EnterDialogue(GameType gameType,ConditionType condition, SituationType situation, UnityAction callback)
    {
        if (_dialogManager == null)
            _dialogManager = FindAnyObjectByType<DialogManager>();
        var dialogues = excels[gameType];
        dialogues = dialogues.Where(d => d.Condition == condition.ToString() && CheckSituation(d.Situation, situation)).ToList();
        List<DialogData> dialogsData = new List<DialogData>();
        foreach (var d in dialogues)
        {
            dialogsData.Add(new DialogData(d.Message, STAR_NAME));
        }
        dialogsData[dialogsData.Count - 1].Callback = callback;
        _dialogManager.Show(dialogsData);
    }
    public void EnterDialogue(GameType gameType, ConditionType condition, UnityAction callback)
    {
        EnterDialogue(gameType, condition, SituationType.Ninguna, callback);
    }
    private bool CheckSituation(string comparation, SituationType situationType )
    {
        if (situationType == SituationType.Ninguna)
            return true;
        else
            return comparation == situationType.ToString();
    }
    public void ResetTries()
    {
        tries = 0;
    }
    public void AddTry()
    {
        tries++;
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
        // Cargar todos los textos que están en la carpeta Resources/CSV
        TextAsset[] csvFiles = Resources.LoadAll<TextAsset>(pathExcels);
        foreach (TextAsset csvFile in csvFiles)
        {
            var rows = new List<RowDialogue>();

            // Dividir el contenido del archivo en líneas
            string[] lines = csvFile.text.Split('\n');

            // Iterar sobre cada línea, comenzando desde la segunda (saltando los títulos)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue; // Saltar líneas vacías
                }

                // Dividir la línea en campos
                string[] values = line.Split(',');

                // Crear un nuevo objeto RowDialogue y asignar sus propiedades
                var rowDialogue = new RowDialogue
                {
                    Tries = int.Parse(values[0].Trim()),
                    Condition = values[1].Trim(),
                    Situation = values[2].Trim(),
                    Message = values[3].Trim(),
                    AudioName = values.Length > 4 ? values[4].Trim() : string.Empty // Manejar casos donde AudioName puede estar vacío
                };

                // Añadir el RowDialogue a la lista
                rows.Add(rowDialogue);
            }

            // Añadir la lista de RowDialogue al diccionario usando el nombre del archivo como clave
            string fileName = csvFile.name; // Usa el nombre del archivo sin la extensión
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
    CayoMesa
}

public class RowDialogue
{
    public int Tries { get; set; }
    public string Condition { get; set; }
    public string Situation { get; set; }
    public string Message { get; set; }
    public string AudioName { get; set; }
}