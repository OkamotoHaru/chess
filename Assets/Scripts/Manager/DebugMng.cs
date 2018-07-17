#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//画面テキストアンカー
	TextAnchor TEXT_ANCHOR = TextAnchor.UpperLeft;
	//----------CLASS----------
	//----------PUBLIC STATIC----------
	//デバッグマネージャ作成したか
	public static bool madeDebugMng = false;
	static int fontSize = 42;
	//----------PUBLIC----------
	//----------PRIVATE----------
	//オブジェクト
	GameObject canvas = null;
	GameObject text = null;
	//テキストリスト
	public static Queue<string> debugMngQueue = new Queue<string>();
	Text textSrc = null;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//デバッグマネージャ生成
	public static DebugMng MakeDebugMng ( GameObject prefab )
	{
		GameObject obj = (GameObject)Instantiate(prefab);
		DebugMng cmp = obj.AddComponent<DebugMng>();
		madeDebugMng = true;
		return cmp;
	}
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//画面ログ設定
	public static void ScreenLog(string mess)
	{
		//デバッグマネージャが存在するかチェック
		if ( !madeDebugMng )
		{
			Debug.Log("デバッグマネージャが存在しないため、この機能は利用できません。Title.sceneから起動するか、デバイスマネージャを生成してください。");
			return;
		}
		debugMngQueue.Enqueue( mess );
	}
	//コンソールログ出力
	public static void Log(string mess)
	{
		Debug.Log(mess);
	}
	//フォントサイズ設定
	public static void SetFontSize(int size)
	{
		fontSize = size;
	}
	//********************************************************************************
	//private static function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//********************************************************************************
	//public function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//********************************************************************************
	//private function
	//********************************************************************************
	//----------EVENT----------
	//起動直後１度のみ通る
	void Awake()
	{
	}
	//起動後初回のみ通る
	void Start ()
	{
		//canvas取得
		canvas = gameObject.transform.Find("Canvas").gameObject;
		//canvas取得チェック
		if (canvas == null)
		{
			Debug.Log("canvasの取得に失敗");
			return;
		}
		//プレファブ取得
		GameObject prefabText = UtilityMng.LoadPrefab("Debug/Text");
		//テキスト生成
		text = (GameObject)Instantiate(prefabText);
		ObjectMng.SetParent(text, canvas);
		//テキストスクリプト取得
		textSrc = text.GetComponent<Text>();
		//アンカー設定
		textSrc.alignment = TEXT_ANCHOR;
		//テキストフォントサイズ設定
		textSrc.fontSize = fontSize;
	}
	//ずっと通る
	void Update ()
	{
		//テキストないなら失敗
		if ( text == null )
		{
			return;
		}
        //キューが０のときもスルー
        if ( debugMngQueue.Count <= 0 )
        {
			//テキスト設定
			textSrc.text = "";
			return;
        }
		//テキスト設定
		string mess = "";
		foreach ( string queue in debugMngQueue )
		{
			//テキスト取り出し
			mess += queue + "\n";
		}
		//フォンtのサイズが変更されたら
		if ( textSrc.fontSize != fontSize )
		{
			textSrc.fontSize = fontSize;
		}
		//テキスト設定
		textSrc.text = mess;
		//キュークリア
		debugMngQueue.Clear();
	}
}
