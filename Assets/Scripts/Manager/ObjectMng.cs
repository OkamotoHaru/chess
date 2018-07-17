#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//モード
	public enum MODE
	{
		MOVE,			//普通のフレーム移動
		MOVE_PHYSIC,	//物理移動
		ROTATE_X,		//X軸回転
		ROTATE_Y,		//Y軸回転
		ROTATE_Z,		//Z軸回転
		TEXT_ALPHA,		//テキストアルファ
		END
	}
	//----------CLASS----------
	//----------PUBLIC STATIC----------
	//----------PUBLIC----------
	//----------PRIVATE----------
	//管理系
	MODE mode = MODE.END;
	int procNo = 0;
	//オブジェクト
	GameObject main = null;
	//GameObject purposeObj = null;
	//移動
	float moveFrame = 0.0f;
	float frameMoveSpdX;
	float frameMoveSpdY;
	float frameMoveSpdZ;
	Vector3 purposePos;
	//物理的移動
	Rigidbody rigid = null;
	Vector3 addForce = Vector3.zero;
	ForceMode forceMode = ForceMode.Force;
	CollisionDetectionMode collisionDetection = CollisionDetectionMode.Discrete;
	UtilityMng timer = null;
	//回転
	int purposeAngle = 0;	//0 ~ 360, -1で無限回転
	float frameRotAngleX;
	float frameRotAngleY;
	float frameRotAngleZ;
	float frameRotSpdX;
	float frameRotSpdY;
	float frameRotSpdZ;
	bool plusRotWay = false;	//プラス方向に回転するか
	int rotResetNum = 0;		//何回0←→360リセットするか
	//テキストアルファ
	int alphaNum;
	float alpha;
	float alphaTime;			//255→0になるまでのフレーム
	float alphaSpd;
	Color32 originalMainColor;
	Color32 originalSubColor;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//フレーム移動生成
	public static ObjectMng MakeObjectMng_FrameMove(
		GameObject obj,
		Vector3 purposePos,
		float moveFrame )
	{
		ObjectMng cmp = obj.AddComponent<ObjectMng>();

		//オブジェクト設定
		cmp.main = obj;
		//モード設定
		cmp.mode = MODE.MOVE;
		//目標座標設定
		cmp.purposePos = purposePos;
		//動く時間設定
		cmp.moveFrame = moveFrame;

		return cmp;
	}
	//物理的移動生成
	public static ObjectMng MakeObjectMng_PhysicalMove(
		GameObject obj,
		Vector3 addForce,
		float moveFrame )
	{
		ObjectMng cmp = obj.AddComponent<ObjectMng>();

		//オブジェクト設定
		cmp.main = obj;
		//モード設定
		cmp.mode = MODE.MOVE_PHYSIC;
		//加える力設定
		cmp.addForce = addForce;
		//動く時間設定
		cmp.moveFrame = moveFrame;

		return cmp;
	}
	//回転生成
	public static ObjectMng MakeObjectMng_Rotate(
		MODE mode,
		GameObject obj,
		int purposeAngle,
		float moveFrame,
		bool plusRotWay )
	{
		//モード指定が回転以外なら失敗
		if ( mode != MODE.ROTATE_X &&
			mode != MODE.ROTATE_Y &&
			mode != MODE.ROTATE_Z )
		{
			return null;
		}
		//コンポーネント生成
		ObjectMng cmp = obj.AddComponent<ObjectMng>();
		//モード設定
		cmp.mode = mode;
		//オブジェクト設定
		cmp.main = obj;
		//目標角度設定
		cmp.purposeAngle = purposeAngle;
		//動く時間設定
		cmp.moveFrame = moveFrame;
		//+方向に回転するか
		cmp.plusRotWay = plusRotWay;
		return cmp;
	}
	//スプライト生成
	public static GameObject MakeSprite( Sprite sprite )
	{
		//名前設定
		string name = "" + sprite.name;
		//オブジェクト生成
		GameObject obj = null;
		obj = new GameObject(name);
		//SpriteRenderer生成
		SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
		//Sprite設定
		renderer.sprite = sprite;
		return obj;
	}
	//テキストアルファ生成
	public static ObjectMng MakeObjectMng_TextAlpha(
		GameObject obj,
		int alphaNum,			//-1で無限回数
		float alphaTime )
	{
		//コンポーネント生成
		ObjectMng cmp = obj.AddComponent<ObjectMng>();
		//モード設定
		cmp.mode = MODE.TEXT_ALPHA;
		//オブジェクト設定
		cmp.main = obj;
		//目標角度設定
		cmp.alphaNum = alphaNum;
		//アルファタイム設定
		cmp.alphaTime = alphaTime;
		return cmp;
	}
	//----------RELEASE----------
	//----------IS----------
	public static bool IsEnd( ObjectMng cmp ){
		//nullなら失敗
		if ( cmp == null ){
			return true;
		}
		return cmp.IsEnd();
	}
	//----------SET----------
	//親子付け
	public static void SetParent(
		GameObject obj,
		GameObject parent, 
		bool stayNowPos = false )
	{
		obj.transform.SetParent( parent.transform, stayNowPos);
	}
	//コライダー取得
	public static Collider GetCollider( GameObject obj )
	{
		Collider collider = obj.GetComponent<Collider>();
		return collider;
	}
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
	//このコンポーネントがついたオブジェクトを開放
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
	//rigidbodyのcollision detection(衝突検知モード)を設定
	public void SetCollisionDetection( CollisionDetectionMode mode ){
		collisionDetection = mode;
	}
	//----------GET----------
	//動作モード取得
	public MODE GetMode()
	{
		return mode;
	}
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
		//コライダーチェック
		switch (mode)
		{
			case MODE.MOVE_PHYSIC:
				//コライダーチェック
				Collider collider = GetCollider( main );
				if (collider == null)
				{
					Debug.Log( "コライダーを設定してください" + main );
				}
				break;
			case MODE.MOVE:
			default:
				break;
		}
	}
	//ずっと通る
	void Update()
	{

#if DEBUG_MODE
		DebugMng.ScreenLog( "mode: " + mode + " procNo: " + procNo);
#endif

		switch ( mode )
		{
			//フレーム移動
			case MODE.MOVE:
				MoveUpdate();
				break;
			//回転
			case MODE.ROTATE_X:
			case MODE.ROTATE_Y:
			case MODE.ROTATE_Z:
				RotateUpdate();
				break;
			//テキストアルファ
			case MODE.TEXT_ALPHA:
				TextAlphaUpdate();
				break;
			default:
				break;
		}
	}
	//ずっと通る(Rigidbodyがアタッチされている場合のみ)
	void FixedUpdate()
	{
		switch ( mode )
		{
			//物理移動
			case MODE.MOVE_PHYSIC:
#if DEBUG_MODE
				//DebugMng.ScreenLog("procNo: " + procNo);
#endif
				PhysicalMoveUpdate();
				break;
		}
	}
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	bool isRotate()
	{
		//無限回転なら
		if ( purposeAngle <= -1 )
		{
			return false;
		}
		//角度のリセット回数がクリアできていないなら
		if ( rotResetNum != 0 )
		{
			return false;
		}
		//回転方向別に超過判定
		if ( plusRotWay )
		{
			if ( ( mode == MODE.ROTATE_X &&frameRotAngleX < purposeAngle) ||
				(mode == MODE.ROTATE_Y && frameRotAngleY < purposeAngle) ||
				(mode == MODE.ROTATE_Z && frameRotAngleZ < purposeAngle) )
			{
				return false;
			}
		}
		else
		{
			if ( (mode == MODE.ROTATE_X && frameRotAngleX > purposeAngle) ||
				(mode == MODE.ROTATE_Y && frameRotAngleY > purposeAngle) ||
				(mode == MODE.ROTATE_Z && frameRotAngleZ > purposeAngle) )
			{
				return false;
			}
		}
		return true;
	}
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//移動モード時Update
	void MoveUpdate()
	{
		switch ( procNo )
		{
			//初期化
			case 0:
				//距離測定
				float disX = Mathf.Abs( Mathf.Abs( main.transform.localPosition.x ) - Mathf.Abs( purposePos.x ) );
				float disY = Mathf.Abs( Mathf.Abs( main.transform.localPosition.y ) - Mathf.Abs( purposePos.y ) );
				float disZ = Mathf.Abs( Mathf.Abs( main.transform.localPosition.z ) - Mathf.Abs( purposePos.z ) );
				//速度設定
				frameMoveSpdX = disX / moveFrame;
				frameMoveSpdY = disY / moveFrame;
				frameMoveSpdZ = disZ / moveFrame;
				//動く方向設定（移動量マイナスかどうか設定）
				if (main.transform.localPosition.x > purposePos.x)
				{
					frameMoveSpdX *= -1;
				}
				if (main.transform.localPosition.y > purposePos.y)
				{
					frameMoveSpdY *= -1;
				}
				if (main.transform.localPosition.z > purposePos.z)
				{
					frameMoveSpdZ *= -1;
				}
				//ケース切り替え
				procNo = 100;
				break;
			//移動
			case 100:
				//移動
				Vector3 nowPos = main.transform.localPosition;
				nowPos.x += frameMoveSpdX;
				nowPos.y += frameMoveSpdY;
				nowPos.z += frameMoveSpdZ;
				main.transform.localPosition = nowPos * UtilityMng.GetOneFrame();
				//目標座標を超えていないなら
				if ((frameMoveSpdX > 0 && main.transform.localPosition.x < purposePos.x) ||
					(frameMoveSpdX < 0 && main.transform.localPosition.x > purposePos.x) ||
					(frameMoveSpdY > 0 && main.transform.localPosition.y < purposePos.y) ||
					(frameMoveSpdY < 0 && main.transform.localPosition.y > purposePos.y) ||
					(frameMoveSpdZ > 0 && main.transform.localPosition.z < purposePos.z) ||
					(frameMoveSpdZ < 0 && main.transform.localPosition.z > purposePos.z) )
				{
					break;
				}
				//座標修正
				main.transform.localPosition = purposePos;
				//ケース切り替え
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//物理移動モード時Update
	void PhysicalMoveUpdate()
	{
		switch ( procNo )
		{
			//初期化
			case 0:
				rigid = main.GetComponent<Rigidbody>();
				if (rigid == null )
				{
					rigid = main.AddComponent<Rigidbody>();
				}
				//rigidbody初期設定
				rigid.collisionDetectionMode = collisionDetection;
#if DEBUG_MODE
				Debug.Log("物理移動：初期化完了");
#endif
				procNo = 100;
				break;
			//移動量加算
			case 100:
				rigid.AddForce( addForce, forceMode );
				//タイマー生成
				timer = UtilityMng.MakeUtilityMng_Timer( gameObject, moveFrame);
				procNo = 200;
				break;
			//終了待ち
			case 200:
				//タイマー終わっていないなら
				if ( !timer.IsEnd() )
				{
					break;
				}
				//タイマー開放
				timer.Release();
				timer = null;
#if DEBUG_MODE
				Debug.Log("物理移動：終了");
#endif
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//回転Update
	void RotateUpdate()
	{
		switch (procNo)
		{
			//初期化
			case 0:
				//現在の角度設定
				frameRotAngleX = Mathf.Abs( main.transform.localEulerAngles.x );
				frameRotAngleY = Mathf.Abs( main.transform.localEulerAngles.y );
				frameRotAngleZ = Mathf.Abs( main.transform.localEulerAngles.z );
				//無限回転以外なら
				if ( purposeAngle > -1)
				{
					float dis = 0.0f;
					if (mode == MODE.ROTATE_X)
					{
						//角度差分計算
						dis = Mathf.Abs( frameRotAngleX - purposeAngle );
						//回転速度計算
						frameRotSpdX = dis / moveFrame;
					}
					else if (mode == MODE.ROTATE_Y)
					{
						//角度差分計算
						dis = Mathf.Abs( frameRotAngleY - purposeAngle );
						//回転速度計算
						frameRotSpdY = dis / moveFrame;
					}
					else if (mode == MODE.ROTATE_Z)
					{
						//角度差分計算
						dis = Mathf.Abs( frameRotAngleZ - purposeAngle );
						//回転速度計算
						frameRotSpdZ = dis / moveFrame;
					}
				}
				else
				{
					frameRotSpdX = 0.0f;
					frameRotSpdY = 0.0f;
					frameRotSpdZ = 0.0f;
					//回転速度計算
					if ( mode == MODE.ROTATE_X )
					{
						frameRotSpdX = 360.0f / moveFrame;
					}
					else if (mode == MODE.ROTATE_Y)
					{
						frameRotSpdY = 360.0f / moveFrame;
					}
					else
					{
						frameRotSpdZ = 360.0f / moveFrame;
					}
				}
				//回転する方向に応じて終了判定用の変数設定
				if ( plusRotWay )
				{
					//現在の角度より目標角度が小さいなら
					if ( ( mode == MODE.ROTATE_X && purposeAngle < frameRotAngleX ) ||
						(mode == MODE.ROTATE_Y && purposeAngle < frameRotAngleY) ||
						(mode == MODE.ROTATE_Z && purposeAngle < frameRotAngleZ))
					{
						rotResetNum = 1;
					}
				}
				else
				{
					//現在の角度より目標角度が大きいなら
					if ((mode == MODE.ROTATE_X && purposeAngle > frameRotAngleX) ||
						(mode == MODE.ROTATE_Y && purposeAngle > frameRotAngleY) ||
						(mode == MODE.ROTATE_Z && purposeAngle > frameRotAngleZ))
					{
						rotResetNum = 1;
					}
					//回転速度反転
					frameRotSpdX *= -1;
					frameRotSpdY *= -1;
					frameRotSpdZ *= -1;
				}
				//ケース切り替え
				procNo = 100;
				break;
			//回転
			case 100:
				//現在の回転率取得
				Vector3 nowAngle = main.transform.localEulerAngles;
				//回転
				frameRotAngleX += frameRotSpdX * UtilityMng.GetOneFrame();
				frameRotAngleY += frameRotSpdY * UtilityMng.GetOneFrame();
				frameRotAngleZ += frameRotSpdZ * UtilityMng.GetOneFrame();
				nowAngle.x = frameRotAngleX;
				nowAngle.y = frameRotAngleY;
				nowAngle.z = frameRotAngleZ;
				//下限上限チェック
				bool change = false;
				if (nowAngle.x < 0.0f)
				{
					nowAngle.x += 360.0f;
					change = true;
				}
				else if (nowAngle.x >= 360.0f)
				{
					nowAngle.x -= 360.0f;
					change = true;
				}
				if (nowAngle.y < 0.0f)
				{
					nowAngle.y += 360.0f;
					change = true;
				}
				else if (nowAngle.y >= 360.0f)
				{
					nowAngle.y -= 360.0f;
					change = true;
				}
				if (nowAngle.z < 0.0f)
				{
					nowAngle.z += 360.0f;
					change = true;
				}
				else if (nowAngle.z >= 360.0f)
				{
					nowAngle.z -= 360.0f;
					change = true;
				}
				if ( change )
				{
					rotResetNum -= 1;
				}
				main.transform.localEulerAngles = nowAngle;
				//回転終了できないなら
				if ( !isRotate() )
				{
					break;
				}
				//ケース切り替え
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//テキストアルファUpdate
	void TextAlphaUpdate()
	{
		switch ( procNo )
		{
			//初期化
			case 0:
				//元の色取得
				UtilityMng.GetTextColor(
					main,
					out originalMainColor,
					out originalSubColor );
				//現在のアルファ値取得
				alpha = originalMainColor.a;
				//アルファ速度設定
				alphaSpd = alpha / alphaTime;
				alphaSpd *= -1;
				//ケース切り替え
				procNo = 100;
				break;
			//アルファ
			case 100:
				alpha += alphaSpd * UtilityMng.GetOneFrame();
				//下限上限チェック
				if ( alpha <= 0.0f )
				{
					alpha = 0.0f;
					alphaSpd *= -1;
				}
				else if ( alpha >= 255.0f )
				{
					alpha = 255.0f;
					alphaSpd *= -1;
					if (alphaNum > 0)
					{
						alphaNum -= 1;
					}
				}
				//アルファ
				originalMainColor.a = (byte)alpha;
				originalSubColor.a = (byte)alpha;
				UtilityMng.SetTextColor(
					main,
					originalMainColor,
					originalSubColor );
				//回数分アルファしたら
				if ( alphaNum != 0 )
				{
					break;
				}
				//ケース切り替え
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
}
