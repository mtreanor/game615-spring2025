using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Assertions;
using UnityEngine.Networking;

public class GoogleSheetSimpleConditionalConversation : MonoBehaviour {

	public string googleSheetDocID = "1wlWYeWrr-wAScjgJxtmpGl_1R6v3ONrYK5Kg9tPPuyM";
	private string url;

	void Start() 
	{
		url = "https://docs.google.com/spreadsheets/d/" + googleSheetDocID + "/export?format=csv";

		// This line starts the download of the google sheet.
		StartCoroutine(DownloadData());
	}

	IEnumerator DownloadData()
	{
		yield return new WaitForEndOfFrame();

		string downloadData = null;
		using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {

			yield return webRequest.SendWebRequest();

			if (webRequest.result == UnityWebRequest.Result.ConnectionError) {
				Debug.Log("Download Error: " + webRequest.error);
				yield break;
			} else {
				// Debug.Log("Download success");
				//Debug.Log("Data: " + webRequest.downloadHandler.text);
				downloadData = webRequest.downloadHandler.text;
			}
		}

		List<Dictionary<string, object>> dataAsList = CSVReader_GoogleSheet.Read(downloadData);
		DialogueManager.scc = new SimpleConditionalConversation(dataAsList);
		DialogueManager.LoadInitialSCCState();
	}
}