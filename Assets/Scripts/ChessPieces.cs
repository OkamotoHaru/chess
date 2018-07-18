#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieces : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//駒種類
	public enum PIECE_TYPE
	{
		PAWN,   //ポーン
		KNIGHT, //ナイト
		BISHOP, //ビショップ
		ROOK,   //ルーク
		QUEEN,  //クイーン
		KING,   //キング
		END
	}
	//プレイヤーの駒か敵の駒か
	public enum SIDE_TYPE
	{
		WHITE,
		BLACK,
		END
	}
	//モード
	public enum MODE
	{
		PIECE,	//駒用
		SELECT,	//ポーン昇格時の選択用
		END
	}
	//効果音
	enum SOUND
	{
		PUT,
		END
	}
	//駒
	float PIECE_POS_Y_SELECTING = 5.0f;
	//色
	public static readonly Color32 PIECE_DEF_COLOR = new Color32( 255, 255, 255, 255 );
	public static readonly Color PIECE_LOSE_COLOR = new Color( 168, 168, 0, 0 );
	public static readonly Color PIECE_PAWN_PASSANT_COLOR = new Color( 168, 0, 168, 0 );
	//----------CLASS----------
	//----------PUBLIC----------
	public GameObject[] BlackPieces;
	public GameObject[] WhitePieces;
	public AudioClip[] sounds;
	//----------PRIVATE----------
	//管理系
	int procNo = 0;
	PIECE_TYPE pieceType;
	SIDE_TYPE sideType;
	MODE mode;
	//フラグ
	bool appeared = false;	//登場したか
	bool touched = false;	//タッチされたか
	bool firstMoved = false;	//初回移動
	bool moved = false;		//移動したか
	bool cancel = false;	//選択キャンセル
	bool pawnJumpMove = false;	//ポーン２マス移動したか
	bool castling = false;	//キングのキャスリングしたか
	bool castlingLeft = false;	//左側にキャスリングしたか
	//駒
	int pieceSize;		//駒と駒の間
	GameObject pieceParent;
	GameObject piece;
	Vector3 makePos;	//生成する座標
	Vector3 makeRot;	//生成したときの回転率
	Vector3 makeRealRot;	//生成したときの駒本体回転率
	int fieldPosX;		//フィールド用座標
	int fieldPosY;		//フィールド用座標
	int nextPosX;		//移動先フィールド座標
	int nextPosY;		//移動先フィールド座標
	Collider pieceCollider;	//コライダー
	//効果音
	AudioSource audioSource;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//生成
	public static ChessPieces MakeChessPieces(
		GameObject prefab,
		GameObject parent,
		PIECE_TYPE pieceType,
		SIDE_TYPE sideType,
		Vector3 makePos,
		int posX,
		int posY,
		int size,
		MODE mode )
	{
		//プレファブ生成
		GameObject obj = (GameObject)Instantiate( prefab );
		ObjectMng.SetParent( obj, parent );
		//コンポーネント取得
		ChessPieces cmp = obj.GetComponent<ChessPieces>();

		//駒種類設定
		cmp.pieceType = pieceType;
		//陣営設定
		cmp.sideType = sideType;
		//モード設定
		cmp.mode = mode;
		//駒を生成する座標
		cmp.makePos = makePos;
		//フィールド用座標設定
		cmp.fieldPosX = posX;
		cmp.fieldPosY = posY;
		//駒と駒の間
		cmp.pieceSize = size;

		return cmp;
	}
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
	public void Remake( PIECE_TYPE type )
	{
		pieceType = type;
		makePos = pieceParent.transform.localPosition;
		makePiece();
	}
	//----------RELEASE----------
	//このコンポーネントがついたオブジェクトを開放
	public void Release()
	{
		Destroy( this.gameObject );
	}
	//----------IS----------
	//終了したか
	public bool IsEnd()
	{
		if (procNo == 99999)
		{
			return true;
		}
		return false;
	}
	//登場したか
	public bool IsAppear()
	{
		return appeared;
	}
	//タッチされたか
	public bool IsTouch()
	{
		bool flag = touched;
		touched = false;
		return flag;
	}
	//初回移動済みなら
	public bool IsFirstMoved()
	{
		return firstMoved;
	}
	//移動したか
	public bool IsMoved()
	{
		bool flag = moved;
		moved = false;
		return flag;
	}
	//選択キャンセルしたか
	public bool IsCancel()
	{
		bool flag = cancel;
		cancel = false;
		return flag;
	}
	//相手側のフィールド端にいるか
	public bool IsFieldEdge( int fieldEdgeNo )
	{
		if ( sideType == SIDE_TYPE.BLACK &&
			fieldPosY == fieldEdgeNo )
		{
			return true;
		}
		else if (sideType == SIDE_TYPE.WHITE &&
			fieldPosY == 0)
		{
			return true;
		}
		return false;
	}
	//ポーン２マス移動したか
	public bool IsPawnJumpMove()
	{
		return pawnJumpMove;
	}
	//キングがキャスリングしたか
	public bool IsCastling()
	{
		bool flag = castling;
		castling = false;
		return flag;
	}
	//キングが左側にキャスリングしたか
	public bool IsCastlingLeft()
	{
		return castlingLeft;
	}
	//----------SET----------
	//タッチ判定のフェーズに移行
	public void SetTouchOK()
	{
		procNo = 300;
	}
	//タッチ判定のフェーズに移行
	public void SetWait()
	{
		procNo = 200;
	}
	//駒移動
	public void SetMove(
		int posX,
		int posY )
	{
		nextPosX = posX;
		nextPosY = posY;
		procNo = 400;
	}
	//色設定
	public void SetColor(Color color)
	{
		Renderer renderer = piece.GetComponent<Renderer>();
		if (renderer)
		{
			renderer.material.color = color;
		}
	}
	//コライダーのトリガー設定
	public void SetColliderTrriger(bool flag)
	{
		if (pieceCollider)
		{
			pieceCollider.isTrigger = flag;
		}
	}
	//生成時の回転率
	public void SetRotate( Vector3 rot )
	{
		makeRot = rot;
	}
	//生成時の駒本体の回転率
	public void SetRealRotate(Vector3 rot)
	{
		makeRealRot = rot;
	}
	//ポーン2マス移動フラグfalse
	public void SetJumpMoveFlagOff()
	{
		pawnJumpMove = false;
	}
	//----------GET----------
	//駒種類取得
	public PIECE_TYPE GetPieceType()
	{
		return pieceType;
	}
	//陣営取得
	public SIDE_TYPE GetSideType()
	{
		return sideType;
	}
	//フィールド内での座標取得
	public void GetFieldPos(
		out int posX,
		out int posY )
	{
		posX = fieldPosX;
		posY = fieldPosY;
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
	void Start ()
	{
	}
	//ずっと通る
	void Update ()
	{
#if DEBUG_MODE
#if skip
				//DebugMng.ScreenLog("ChessPieces() procNo: " + procNo);
				if ( piece )
				{
					UtilityMng.TouchInfo touchInfo;
					UtilityMng.IsTouch( piece, out touchInfo );
					DebugMng.SetFontSize( 30 );
					DebugMng.ScreenLog( "touch pos: " + touchInfo );
				}
#endif
#endif
		UtilityMng.TouchInfo touchInfo;

		switch ( procNo )
		{
			//初期化
			case 0:
				//駒生成
				makePiece();
				//ピースが取得できなかったら終了
				if ( piece == null )
				{
					//メインコマンド切り替え
					procNo = 99999;
				}
				//メインコマンド切り替え
				procNo = 100;
				break;
			//登場
			case 100:
				//登場完了フラグtrue
				appeared = true;
				//メインコマンド切り替え
				procNo = 200;
				break;
			//待機
			case 200:
				break;
			//タッチ待ち
			case 300:
				//タッチされていないなら
				bool touchFlag = UtilityMng.IsTouchEnded( piece, out touchInfo );
				if ( !touchFlag )
				{
					break;
				}
				//タッチフラグtrue
				touched = true;
				//モード別に遷移先設定
				if ( mode == MODE.SELECT )
				{
					//メインコマンド切り替え
					procNo = 200;
					break;
				}
				//選択中とわかるように駒を移動
				Vector3 nowPiecePos = pieceParent.transform.localPosition;
				nowPiecePos.y = PIECE_POS_Y_SELECTING;
				pieceParent.transform.localPosition = nowPiecePos;
				//メインコマンド切り替え
				procNo = 301;
				break;
			case 301:
				//選択解除待ち
				bool touchSameObj = UtilityMng.IsTouchEnded( piece, out touchInfo );
				if ( !touchSameObj )
				{
					break;
				}
				//キャンセルフラグtrue
				cancel = true;
				//座標修正
				Vector3 fixPos = pieceParent.transform.localPosition;
				fixPos.y = 0.0f;
				pieceParent.transform.localPosition = fixPos;
				//メインコマンド切り替え
				procNo = 300;
				break;
			//駒移動
			case 400:
				//今の場所との差分
				int subX = nextPosX - fieldPosX;
				int subY = fieldPosY - nextPosY;
				//実際の座標の差分
				float realSubPosX = subX * pieceSize;
				float realSubPosY = subY * pieceSize;
				//ポーンのみ２マス移動フラグ設定
				if ( pieceType == PIECE_TYPE.PAWN && Mathf.Abs(subY) > 1)
				{
					pawnJumpMove = true;
				}
				//キングのキャスリング判定
				if ( pieceType == PIECE_TYPE.KING && Mathf.Abs(subX) > 1 )
				{
					castling = true;
					if ( subX < 0 )
					{
						castlingLeft = true;
					}
				}
				//移動
				Vector3 nowPos = pieceParent.transform.localPosition;
				nowPos.x += realSubPosX;
				nowPos.y = 0.0f;
				nowPos.z += realSubPosY;
				pieceParent.transform.localPosition = nowPos;
				//フィールド座標設定
				fieldPosX = nextPosX;
				fieldPosY = nextPosY;
				//効果音
				audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.clip = sounds[(int)SOUND.PUT];
				audioSource.PlayOneShot( sounds[(int)SOUND.PUT] );
				//移動フラグtrue
				moved = true;
				//初回移動フラグtrue
				firstMoved = true;
				//メインコマンド切り替え
				procNo = 200;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//----------MAKE----------
	//駒生成
	void makePiece()
	{
		//既にあるなら一旦開放
		if ( pieceParent != null )
		{
			Destroy(pieceParent);
			pieceParent = null;
		}
		//生成
		//黒なら
		if ( sideType == SIDE_TYPE.BLACK )
		{
			pieceParent = (GameObject)Instantiate( BlackPieces[(int)pieceType] );
		}
		else
		{
			pieceParent = (GameObject)Instantiate( WhitePieces[(int)pieceType] );
		}
		ObjectMng.SetParent( pieceParent, gameObject );
		//座標設定
		pieceParent.transform.localPosition = makePos;
		pieceParent.transform.localEulerAngles = makeRot;
		//コライダーがついてるモデル取得
		piece = UtilityMng.GetObjectWithCollider( pieceParent );
		piece.transform.localEulerAngles = makeRealRot;
		//コライダー取得
		pieceCollider = UtilityMng.GetCollider( piece, true );
		//選択用なら
		if ( mode == MODE.SELECT )
		{
			//影off
			MeshRenderer meshRenderer = piece.GetComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			//Y回転させる
			ObjectMng.MakeObjectMng_Rotate(
				ObjectMng.MODE.ROTATE_Z,
				pieceParent,
				-1,
				120.0f,
				true );
		}
#if DEBUG_MODE
		if ( piece == null )
		{
			DebugMng.Log( "piece is null.(" + sideType + ", " + pieceType + ")" );
		}
		if (pieceCollider == null)
		{
			DebugMng.Log( "collider is null.(" + sideType + ", " + pieceType + ")" );
		}
#endif
	}
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
}