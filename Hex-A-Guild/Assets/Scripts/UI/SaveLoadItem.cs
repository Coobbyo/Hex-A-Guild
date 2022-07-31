using UnityEngine;
using TMPro;

public class SaveLoadItem : MonoBehaviour
{
	public SaveLoadMenu menu;
	
	public string MapName
    {
		get
        {
			return mapName;
		}
		set
        {
			mapName = value;
			transform.GetChild(0).GetComponent<TMP_Text>().text = value;
		}
	}
	
	private string mapName;
	
	public void Select()
    {
		menu.SelectItem(mapName);
	}
}
