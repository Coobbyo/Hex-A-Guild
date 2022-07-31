using UnityEngine;
using TMPro;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text menuLabel, actionButtonLabel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private RectTransform listContent;
	[SerializeField] private SaveLoadItem itemPrefab;

	[SerializeField] private HexGrid hexGrid;

	private const int mapFileVersion = 5;
    private bool saveMode;

    private string GetSelectedPath()
    {
		string mapName = nameInput.text; //TODO: Add ContentType
		if(mapName.Length == 0)
        {
			return null;
		}
		return Path.Combine(Application.persistentDataPath, mapName + ".map");
	}
    
    private void Save(string path)
	{
		//Debug.Log(Application.persistentDataPath);
		using(BinaryWriter writer =
			new BinaryWriter(File.Open(path, FileMode.Create)))
		{
			writer.Write(mapFileVersion);
			hexGrid.Save(writer);
		}
	}

	private void Load(string path)
	{
        if(!File.Exists(path))
        {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using(BinaryReader reader = new BinaryReader(File.OpenRead(path)))
		{
			int header = reader.ReadInt32();
			if(header <= mapFileVersion)
			{
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else
			{
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}

    private void FillList()
    {
        for(int i = 0; i < listContent.childCount; i++)
        {
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths =
			Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);
        for(int i = 0; i < paths.Length; i++)
        {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
		}
	}

	public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if(saveMode)
        {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else
        {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		FillList();
        gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close()
    {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

    public void Action()
    {
		string path = GetSelectedPath();
		if (path == null)
			return;

		if(saveMode)
        {
			Save(path);
		}
		else
        {
			Load(path);
		}

		Close();
	}

    public void SelectItem(string name)
    {
		nameInput.text = name;
	}

    public void Delete()
    {
		Debug.Log("Try to Delete");
		string path = GetSelectedPath();
		if (path == null)
			return;

		Debug.Log("Path isn't Null");
		if (File.Exists(path))
			File.Delete(path);
        
		Debug.Log("Should be Deleted?");

        nameInput.text = "";
		FillList();
	}
}
