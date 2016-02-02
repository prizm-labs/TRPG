using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	private bool loadScene = false;
	[SerializeField]
	private int scene;
	[SerializeField]
	private Text loadingText;
	[SerializeField]
	private Image ProgressBar;

	private AsyncOperation asyncProg = null;
	public bool startGame = false;

	void Update () {
		if (startGame && loadScene == false) {
			loadScene = true;
			loadingText.text = "LOADING...";
			StartCoroutine(LoadNewScene());
		}

		if(loadScene){
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
			if (asyncProg != null) {
				ProgressBar.fillAmount = asyncProg.progress;
			}
		}

	}
	public void beginGame(){
		startGame = true;
	}

	public IEnumerator LoadNewScene(){
		
		asyncProg = SceneManager.LoadSceneAsync (scene);

		while (!asyncProg.isDone) {
			yield return asyncProg;
		}
	}
}
