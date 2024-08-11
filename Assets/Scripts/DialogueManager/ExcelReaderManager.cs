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
    string pathExcels = "Excels";
    private static ExcelReaderManager _instance;

    private int tries = 0;
    public DialogManager DialogManager;
    const string STAR_NAME = "Star";
    public Dictionary<string, List<RowDialogue>> excels = new();
    public static ExcelReaderManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ExcelReaderManager();
        }
        return _instance;
    }
    public void EnterDialogue(string nameGame,ConditionType condition, SituationType situation, UnityAction callback)
    {
        var dialogues = excels[nameGame];
        dialogues = dialogues.Where(d => d.Condition == condition.ToString() && CheckSituation(d.Situation, situation)).ToList();
        List<DialogData> dialogsData = new List<DialogData>();
        foreach (var d in dialogues)
        {
            dialogsData.Add(new DialogData(d.Message, STAR_NAME));
        }
        dialogsData[dialogsData.Count - 1].Callback = callback;
        DialogManager.Show(dialogsData);
    }
    public void EnterDialogue(string nameGame, ConditionType condition, UnityAction callback)
    {
        EnterDialogue(nameGame,condition, SituationType.Ninguna, callback);
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
        excels = new Dictionary<string, List<RowDialogue>>();

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
            excels[fileName] = rows;
        }
    }
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
    cChocasteObrero
}

public class RowDialogue
{
    public int Tries { get; set; }
    public string Condition { get; set; }
    public string Situation { get; set; }
    public string Message { get; set; }
    public string AudioName { get; set; }
}