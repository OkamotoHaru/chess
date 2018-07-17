#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	public enum MODE
	{
		TIMER,
		END
	}
	static int FPS = 60;
	//タッチ関連
	public enum TouchInfo
	{
		NONE,		//タッチなし
		BEGAN,		//タッチ開始
		MOVED,		//移動
		STATIONARY,	//静止
		ENDED,		//タッチ終了
		CANCELED,	//タッチキャンセル
	}
	//----------CLASS----------
	//----------PUBLIC STATIC----------
	//----------PUBLIC----------
	public static GameObject touchJudgeObj;
	//----------PRIVATE----------
	//管理系
	int procNo = 0;
	MODE mode = MODE.END;
	//タイマー
	float purposeFrame = 0.0f;
	float nowFrame = 0.0f;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//タイマーモードで生成
	public static UtilityMng MakeUtilityMng_Timer( GameObject parent, float frame )
	{
		UtilityMng cmp = parent.AddComponent<UtilityMng>();

		//モード設定
		cmp.mode = MODE.TIMER;
		//フレーム設定
		cmp.purposeFrame = frame;

		return cmp;
	}
	//----------RELEASE----------
	//このコンポーネントを開放
	public static void Release( UtilityMng cmp )
	{
		//nullなら失敗
		if ( cmp == null )
		{
			return;
		}
		cmp.Release();
	}
	//----------IS----------
	//終了したか
	public static bool IsEnd( UtilityMng cmp ){
		//nullなら失敗
		if ( cmp == null ){
			return true;
		}
		return cmp.IsEnd();
	}
	//画面タッチ取得
	public static TouchInfo IsTouch()
	{
		if (Application.isMobilePlatform)
		{
			if (Input.touchCount > 0)
			{
				return (TouchInfo)((int)Input.GetTouch( 0 ).phase);
			}
		}
		else
		{
			if (Input.GetMouseButtonDown( 0 ))
			{
				return TouchInfo.BEGAN;
			}
			if (Input.GetMouseButton( 0 ))
			{
				return TouchInfo.MOVED;
			}
			if (Input.GetMouseButtonUp( 0 ))
			{
				return TouchInfo.ENDED;
			}
		}
		return TouchInfo.NONE;
	}
	//画面タッチ取得
	public static bool IsTouchEnded()
	{
		TouchInfo info = IsTouch();
		if ( info == TouchInfo.ENDED )
		{
			return true;
		}
		return false;
	}
	//オブジェクトをタッチしたか
	public static bool IsTouch(
		GameObject judgeObj,
		float dis = 100.0f)
	{
		TouchInfo touchInfo;
		return IsTouch(
			judgeObj,
			out touchInfo,
			dis );
	}
	//オブジェクトをタッチしたか
	public static bool IsTouch(
		GameObject judgeObj,
		out TouchInfo touchInfo,
		float dis = 100.0f)
	{
		touchInfo = IsTouch();
		//画面タッチしてるなら
		if ( touchInfo != TouchInfo.NONE )
		{
			// タッチしたスクリーン座標をrayに変換
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			// オブジェクトにrayが当たった時
			RaycastHit hit = new RaycastHit();	//ray情報
			if (Physics.Raycast( ray, out hit, dis ))
			{
				//hitしたオブジェクトが指定されたものなら
				if (hit.collider.gameObject == judgeObj)
				{
#if DEBUG_MODE
					DebugMng.Log( "touch: " + hit.collider.gameObject.name );
#endif
					//判定するオブジェクト設定
					if (touchJudgeObj == null)
					{
						touchJudgeObj = judgeObj;
					}
					//反するオブジェクトが保存済のものと異なるなら
					if (touchJudgeObj != judgeObj)
					{
						return false;
					}
					return true;
				}
			}
		}
		return false;
	}
	//オブジェクトをタッチしたか
	public static bool IsTouchEnded(
		GameObject judgeObj,
		float dis = 100.0f)
	{
		TouchInfo touchInfo;
		return IsTouchEnded(
			judgeObj,
			out touchInfo,
			dis );
	}
	//オブジェクトをタッチしたか
	public static bool IsTouchEnded(
		GameObject judgeObj,
		out TouchInfo touchInfo,
		float dis = 100.0f)
	{
		touchInfo = IsTouch();
		//画面タッチしてないなら
		if (touchInfo == TouchInfo.NONE)
		{
			return false;
		}
		// タッチしたスクリーン座標をrayに変換
		Ray ray = Camera.main.ScreenPointToRay( GetTouchPosition() );
		//トリガー設定しているものはray判定しない
		Physics.queriesHitTriggers = false;
		// オブジェクトにrayが当たってないなら
		RaycastHit hit = new RaycastHit();  //ray情報
		if ( !Physics.Raycast( ray, out hit, dis ) )
		{
			return false;
		}
		//判定開始する時のobjを設定
		if (touchInfo == TouchInfo.BEGAN)
		{
			if (touchJudgeObj == null)
			{
				touchJudgeObj = hit.collider.gameObject;
			}
			return false;
		}
		//タッチ離したとき以外なら
		if (touchInfo != TouchInfo.ENDED)
		{
			return false;
		}
		//保存したオブジェクトとタッチ開始時のオブジェクトが異なるなら
		if (touchJudgeObj != hit.collider.gameObject)
		{
			touchJudgeObj = null;
			return false;
		}
		//オブジェクトが保存済のものと異なるなら
		if (hit.collider.gameObject != judgeObj)
		{
			return false;
		}
		touchJudgeObj = null;
		return true;
	}
	//オブジェクトをタッチしたか
	public static bool IsTouchEnded2D(
		GameObject judgeObj,
		float dis = 100.0f)
	{
		TouchInfo touchInfo;
		return IsTouchEnded2D(
			judgeObj,
			out touchInfo,
			dis );
	}
	//オブジェクトをタッチしたか
	public static bool IsTouchEnded2D(
		GameObject judgeObj,
		out TouchInfo touchInfo,
		float dis = 100.0f)
	{
		touchInfo = IsTouch();
		//画面タッチしてないなら
		if (touchInfo == TouchInfo.NONE)
		{
			return false;
		}
		// タッチしたスクリーン座標をrayに変換
		Vector2 ray = Camera.main.ScreenToWorldPoint( GetTouchPosition() );
		//トリガー設定しているものはray判定しない
		Physics.queriesHitTriggers = false;
		Collider2D collider2d = Physics2D.OverlapPoint( ray );
		//コライダーがなければ
		if ( collider2d == null )
		{
			return false;
		}
		// オブジェクトにrayが当たってないなら
		RaycastHit2D hit = Physics2D.Raycast( ray, -Vector2.up, dis );  //ray情報
		if ( !hit )
		{
			return false;
		}
		//判定開始する時のobjを設定
		if (touchInfo == TouchInfo.BEGAN)
		{
			if (touchJudgeObj == null)
			{
				touchJudgeObj = hit.collider.gameObject;
			}
			return false;
		}
		//タッチ離したとき以外なら
		if (touchInfo != TouchInfo.ENDED)
		{
			return false;
		}
		//保存したオブジェクトとタッチ開始時のオブジェクトが異なるなら
		if (touchJudgeObj != hit.collider.gameObject)
		{
			touchJudgeObj = null;
			return false;
		}
		//オブジェクトが保存済のものと異なるなら
		if (hit.collider.gameObject != judgeObj)
		{
			return false;
		}
		touchJudgeObj = null;
		DebugMng.Log("hit name: " + hit.collider.gameObject .name);
		return true;
	}
	//----------SET----------
	//テキスト設定
	public static void SetText(
		GameObject text,
		string mess )
	{
		foreach(Transform child in text.transform)
		{
			TextMesh textMesh = child.gameObject.GetComponent<TextMesh>();
			if ( textMesh )
			{
				textMesh.text = mess;
			}
		}
	}
	//テキスト色設定
	public static void SetTextColor(
		GameObject text,
		Color32 mainColor,
		Color32 subColor )
	{
		foreach (Transform child in text.transform)
		{
			TextMesh textMesh = child.gameObject.GetComponent<TextMesh>();
			if ( textMesh == null )
			{
				continue;
			}
			if ( child.gameObject.name == "3DText" )
			{
				textMesh.color = mainColor;
			}
			else if (child.gameObject.name == "Outline")
			{
				textMesh.color = subColor;
			}
		}
	}
	//スプライト色設定
	public static void SetSpriteColor(
		GameObject obj,
		Color32 color )
	{
		SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
		if ( renderer )
		{
			renderer.color = color;
		}
	}
	//----------GET----------
	//タッチ座標取得
	public static Vector3 GetTouchPosition()
	{
		if (Application.isMobilePlatform)
		{
			if (Input.touchCount > 0)
			{
				Vector3 TouchPosition = Vector3.zero;
				Touch touch = Input.GetTouch( 0 );
				TouchPosition.x = touch.position.x;
				TouchPosition.y = touch.position.y;
				return TouchPosition;
			}
		}
		else
		{
			TouchInfo touch = IsTouch();
			if (touch != TouchInfo.NONE)
			{
				return Input.mousePosition;
			}
		}
		return Vector3.zero;
	}
	//タッチ座標をワールド座標に変換して返す
	public static Vector3 GetTouchWorldPosition(Camera camera)
	{
		Vector3 pos = camera.ScreenToWorldPoint( GetTouchPosition() );
		return pos;
	}
	//最初にヒットしたコライダーがついてるobjを返す
	public static GameObject GetObjectWithCollider(GameObject obj)
	{
		Collider collider = UtilityMng.GetCollider(obj, true);
		if ( collider )
		{
			return collider.gameObject;
		}
		return null;
	}
	//コライダー取得
	public static Collider GetCollider(
		GameObject obj,
		bool checkChild = false )
	{
		Collider collider = null;
		collider = obj.GetComponent<Collider>();
		if ( collider == null && checkChild )
		{
			foreach (Transform child in obj.transform)
			{
				collider = child.gameObject.GetComponent<Collider>();
				if ( collider )
				{
					break;
				}
			}
		}
		return collider;
	}
	//１フレーム加算量を返す
	public static float GetOneFrame()
	{
		return Time.deltaTime * FPS;
	}
	//テキスト色返す
	public static void GetTextColor(
		GameObject text,
		out Color32 originalMainColor,
		out Color32 originalSubColor )
	{
		originalMainColor = new Color32( 0, 0, 0, 255 );
		originalSubColor = new Color32( 0, 0, 0, 255 );
		foreach (Transform child in text.transform)
		{
			TextMesh textMesh = child.gameObject.GetComponent<TextMesh>();
			if (textMesh == null)
			{
				continue;
			}
			if (child.gameObject.name == "3DText")
			{
				originalMainColor = textMesh.color;
			}
			else if (child.gameObject.name == "Outline")
			{
				originalSubColor = textMesh.color;
			}
		}
	}
	//----------OTHER----------
	//プレファブロード
	public static GameObject LoadPrefab(string prefabName)
	{
		GameObject prefab = null;
		prefab = (GameObject)Resources.Load( prefabName );
		return prefab;
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
	//このコンポーネントを開放
	public void Release()
	{
		Destroy( this );
	}
	//----------IS----------
	//終了したか
	public bool IsEnd()
	{
		if ( procNo == 99999 )
		{
			return true;
		}
		return false;
	}
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
	void Start()
	{
	}
	//ずっと通る
	void Update()
	{
#if DEBUG_MODE
		//DebugMng.ScreenLog("UtilityMng procNo: " + procNo);
		DebugMng.ScreenLog("UtilityMng nowFrame: " + nowFrame);
		DebugMng.ScreenLog("UtilityMng purposeFrame: " + purposeFrame);
#endif
		switch ( mode )
		{
			case MODE.TIMER:
				TimerUpdate();
				break;
			default:
				break;
		}
	}
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//タイマーモード時のUpdate
	void TimerUpdate()
	{
		switch ( procNo )
		{
			//初期化
			case 0:
#if skip //2018.03.31 uchida 物理フレームでタイマーする事がほぼ無い気がしたのでskip
				//物理フレームを見るかチェック
				Rigidbody rigid = main.GetComponent<Rigidbody>();
				if ( rigid == null )
				{
					procNo = 100;
				}
				else
				{
					procNo = 200;
				}
#else
				procNo = 100;
#endif
#if DEBUG_MODE
				if ( procNo == 100 )
				{
					Debug.Log("タイマー：初期化完了(deltaTime)");
				}
				else
				{
					Debug.Log("タイマー：初期化完了(fixedDeltaTime)");
				}
#endif
				break;
			//物理なしフレーム加算
			case 100:
				nowFrame += GetOneFrame();
				if ( nowFrame < purposeFrame)
				{
					break;
				}
#if DEBUG_MODE
				Debug.Log("タイマー：終了");
#endif
				procNo = 99999;
				break;
#if skip
			//物理ありフレーム加算
			case 200:
				nowFrame += Time.fixedDeltaTime;
				if (nowFrame < purposeFrame)
				{
					break;
				}
#if DEBUG_MODE
				Debug.Log("タイマー：終了");
#endif
				procNo = 99999;
				break;
#endif
			//終了
			case 99999:
				break;
		}
	}
}
