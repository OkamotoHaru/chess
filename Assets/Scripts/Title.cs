#if UNITY_DEBUG
	#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//ボタン
	Vector3 BUTTON_POS = new Vector3(0.0f, 0.0f, 9.0f);
	//テキスト
	Vector3 TEXT_TITLE_POS = new Vector3( 0.0f, 3.4f, 9.0f );
	Vector3 TEXT_TITLE_ROT = new Vector3( 0.0f, 0.0f, 0.0f );
	Vector3 TEXT_TITLE_SCA = new Vector3( 0.2f, 0.2f, 1.0f );
	Vector3 TEXT_POS = new Vector3( 0.0f, 0.0f, -1.0f );
	Vector3 TEXT_SCA = new Vector3(0.1f, 0.1f, 0.1f);
	//背景
	Vector3 BG_POS = new Vector3( 0.0f, 0.0f, 11.0f );
	Vector3 BG_SCA = new Vector3( 3.5f, 2.5f, 1.0f );
	//----------CLASS----------
	//----------PUBLIC----------
	public GameObject prefabDebugMng;
	public Sprite Button;
	public GameObject Text3d;
	public Sprite bgImage;
	//----------PRIVATE----------
	//管理系
	int procNo = 0;
	//デバッグ機能
	DebugMng debugMng = null;
	//ボタン
	GameObject button;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
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
		//背景生成
		makeBG();
		//デバッグ機能生成
		MakeDebugMng();
	}
	//ずっと通る
	void Update ()
	{
#if DEBUG_MODE
		//DebugMng.ScreenLog("procNo: " + procNo);
#endif
		switch ( procNo )
		{
			//初期化
			case 0:
				//タイトル生成
				makeTitle();
				//ボタン生成
				makeButton();
				//ゲームメインに移行
				procNo = 200;
				break;
			//ボタンタッチされたら
			case 200:
				if ( !UtilityMng.IsTouchEnded2D( button ) )
				{
					break;
				}
				//ゲームメインに移行
				procNo = 100;
				break;
			//ゲームメインに移行
			case 100:
				SceneMng.Change(SCENE.GAMEMAIN);
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//----------MAKE----------
	//背景生成
	void makeBG()
	{
		GameObject obj = ObjectMng.MakeSprite( bgImage );
		ObjectMng.SetParent( obj, gameObject );
		obj.transform.localPosition = BG_POS;
		obj.transform.localScale = BG_SCA;
	}
	//デバッグ機能生成
	void MakeDebugMng()
	{
		debugMng = DebugMng.MakeDebugMng(prefabDebugMng);
		//シーン切り替えしても破棄させない
		DontDestroyOnLoad( debugMng );
	}
	//タイトル生成
	void makeTitle()
	{
		//テキスト生成
		GameObject text = (GameObject)Instantiate( Text3d );
		ObjectMng.SetParent( text, gameObject );
		text.transform.localPosition = TEXT_TITLE_POS;
		text.transform.localEulerAngles = TEXT_TITLE_ROT;
		text.transform.localScale = TEXT_TITLE_SCA;
		//テキスト設定
		UtilityMng.SetText( text, "CHESS" );
	}
	//ボタン生成
	void makeButton()
	{
		button = ObjectMng.MakeSprite( Button );
		ObjectMng.SetParent( button, gameObject );
		button.transform.localPosition = BUTTON_POS;
		button.layer = LayerMask.NameToLayer( "UI" );
		//コライダーつける
		button.AddComponent<BoxCollider2D>();
		//テキスト生成
		GameObject text = (GameObject)Instantiate(Text3d);
		ObjectMng.SetParent( text, button );
		text.transform.localPosition = TEXT_POS;
		text.transform.localScale = TEXT_SCA;
		//テキスト設定
		UtilityMng.SetText( text, "2P対戦" );
	}
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
}
