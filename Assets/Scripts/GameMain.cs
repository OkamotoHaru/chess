#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//背景
	Vector3 BG_ROT = new Vector3( 90.0f, 0.0f, 0.0f );
	Vector3 BG_SCA = new Vector3( 28.0f, 14.0f, 1.0f );
	//カメラ
	float CAM_MOVE_FRAME = 30.0f;
	Vector3 CAM_PLAING_POS = new Vector3( 0.0f, 70.0f, -8.0f );
	int CAM_PLAING_ANGLE = 84;
	//駒
	Vector3 PIECE_SELECT_ROT = new Vector3( 90.0f, 0.0f, -10.0f );
	//*テキスト
	//勝利文字
	Vector3 TEXT_GAMEOVER_POS = new Vector3( 0.0f, 18.0f, 0.0f );
	Vector3 TEXT_GAMEOVER_ROT = new Vector3(90.0f, 0.0f, 0.0f);
	Color32 TEXT_GAMEOVER_MAIN_COLOR = new Color32( 255, 255, 255, 255 );
	Color32 TEXT_GAMEOVER_SUB_COLOR = new Color32( 255, 0, 0, 255 );
	//タッチしてください
	Vector3 TEXT_TOUCH_POS = new Vector3( 0.0f, 18.0f, -15.0f );
	Vector3 TEXT_TOUCH_ROT = new Vector3( 90.0f, 0.0f, 0.0f );
	Vector3 TEXT_TOUCH_SCA = new Vector3( 0.25f, 0.25f, 1.0f );
	//チェック
	Vector3 TEXT_CHECK_POS = new Vector3( 0.0f, 18.0f, 0.0f );
	Vector3 TEXT_CHECK_ROT = new Vector3( 90.0f, 0.0f, 0.0f );
	Vector3 TEXT_CHECK_SCA = new Vector3( 1.0f, 1.0f, 1.0f );
	float CHECK_WAIT_TIME = 60.0f;
	//*タイル
	Vector3 TILE_POS = new Vector3( 0.0f, 16.0f, -4.0f );
	Vector3 TILE_ROT = new Vector3( 90.0f, 0.0f, 0.0f );
	Vector3 TILE_SCA = new Vector3( 11.0f, 9.5f, 1.0f );
	Color32 TILE_BLACK_COLOR = new Color32( 0, 0, 0, 128 );
	//タイマー
	float GAMEOVER_WAIT_TIME = 60.0f * 3;
	//テキスト
	float TEXT_TIME = 60.0f;
	//----------CLASS----------
	//----------PUBLIC----------
	public GameObject mainCamera;
	public GameObject chessPiece;
	public Sprite bgImage;
	public GameObject Text3D;
	public Sprite Tile;
	//----------PRIVATE----------
	//管理系
	int procNo = 0;
	//フラグ
	bool gamover = false;
	//フィールド
	GameObject fieldPrefab = null;
	Field field = null;
	//カメラ
	ObjectMng cmpMove;
	ObjectMng cmpRotate;
	//駒
	List<ChessPieces> piecesWhite_List = new List<ChessPieces>();
	List<ChessPieces> piecesBlack_List = new List<ChessPieces>();
	List<ChessPieces> piecesSelect_List = new List<ChessPieces>();
	ChessPieces selectPiece;
	List<int> passantXList = new List<int>();
	List<int> passantYList = new List<int>();
	//ターン
	ChessPieces.SIDE_TYPE nowSideType = ChessPieces.SIDE_TYPE.WHITE;
	int touchPieceNo = -1;
	UtilityMng timer;
	//テキスト
	GameObject textTouch;
	GameObject textCheck;
	//タイル
	GameObject tile;
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
		//プレファブ読み込み
		fieldPrefab = UtilityMng.LoadPrefab("Field/Field");
	}
	//ずっと通る
	void Update ()
	{
#if DEBUG_MODE
		DebugMng.ScreenLog( "GameMain.cs procNo: " + procNo);
		DebugMng.ScreenLog( "now side: " + nowSideType );
		DebugMng.ScreenLog( "UtilityMng.touchJudgeObj: " + UtilityMng.touchJudgeObj );
		/*
		if ( field && selectPiece )
		{
			List<ChessPieces> dagerList = new List<ChessPieces>();
			bool dbg_check = field.IsCheck(
				selectPiece ,
				piecesBlack_List,
				piecesWhite_List,
				out dagerList );
			DebugMng.ScreenLog( "select piece lose: " + dbg_check );
		}
		*/
		//DebugMng.ScreenLog( "delta time: " + UtilityMng.GetOneFrame() );
#endif
		int piecePosX;
		int piecePosY;

		switch ( procNo )
		{
			//初期化
			case 0:
				//背景生成
				makeBG();
				//ケース切り替え
				procNo = 100;
				break;
			//フィールド生成
			case 100:
				field = Field.MakeField( fieldPrefab, gameObject, Field.FIELD_TYPE.NORMAL );
				//ケース切り替え
				procNo = 101;
				break;
			case 101:
				//初期化が終わっていないなら
				if ( !Field.IsInit(field) ){
					break;
				}
				//ケース切り替え
				procNo = 400;
				break;
			//駒生成
			case 400:
				makePieces();
				//ケース切り替え
				procNo = 200;
				break;
			//カメラ移動&回転
			case 200:
				//移動タスク生成
				cmpMove = ObjectMng.MakeObjectMng_FrameMove(
					mainCamera,
					CAM_PLAING_POS,
					CAM_MOVE_FRAME );
				//回転タスク生成
				cmpRotate = ObjectMng.MakeObjectMng_Rotate(
					ObjectMng.MODE.ROTATE_X,
					mainCamera,
					CAM_PLAING_ANGLE,
					CAM_MOVE_FRAME,
					false );
				//ケース切り替え
				procNo = 201;
				break;
			case 201:
				//移動終わっていないなら
				if ( !ObjectMng.IsEnd(cmpMove) )
				{
					break;
				}
				//回転終わっていないなら
				if (!ObjectMng.IsEnd( cmpRotate ))
				{
					break;
				}
				//開放
				Destroy(cmpMove);
				cmpMove = null;
				Destroy( cmpRotate );
				cmpRotate = null;
#if DEBUG_MODE
				//DebugMng.Log("カメラ移動終了");
#endif
				procNo = 500;
				break;
			//タッチ待ち
			case 500:
				//置ける場所にある駒はisTrrigerをtrue
				setPieceColliderTrriger( nowSideType );
				//タッチ番号初期化
				touchPieceNo = -1;
				//駒タッチ判定開始
				if ( nowSideType == ChessPieces.SIDE_TYPE.BLACK )
				{
					pieceSetStart( ref piecesBlack_List );
				}
				else
				{
					pieceSetStart( ref piecesWhite_List );
				}
				//ケース切り替え
				procNo = 501;
				break;
			case 501:
				//どれかがタッチされたら
				//駒タッチ判定開始
				if (nowSideType == ChessPieces.SIDE_TYPE.BLACK)
				{
					touchPieceNo = getPieceTouchedNo( ref piecesBlack_List );
				}
				else
				{
					touchPieceNo = getPieceTouchedNo( ref piecesWhite_List );
				}
				if (touchPieceNo == -1 )
				{
					break;
				}
#if DEBUG_MODE
				DebugMng.Log( "touched no: " + touchPieceNo );
#endif
				//駒に応じて置ける場所にピン生成
				List<int> canPutX = new List<int>();
				List<int> canPutY = new List<int>();
				passantXList = new List<int>();
				passantYList = new List<int>();
				selectPiece = getPieceCmp( nowSideType, touchPieceNo );
				field.MakePin(
					selectPiece,
					ref canPutX,
					ref canPutY,
					ref passantXList,
					ref passantYList,
					ref piecesBlack_List,
					ref piecesWhite_List );
				//タッチされたもの以外は待機状態に設定
				setPieceWait( touchPieceNo );
				//置ける場所にある駒は色変更
				setPieceColor(
					ChessPieces.PIECE_LOSE_COLOR,
					nowSideType,
					canPutX,
					canPutY );
				//ポーンアンパッサンによって消えるものには色変更
				setPieceColor(
					ChessPieces.PIECE_PAWN_PASSANT_COLOR,
					nowSideType,
					passantXList,
					passantYList );
				//フィールドタッチ判定開始
				field.SetSelect();
				//ケース切り替え
				procNo = 502;
				break;
			case 502:
				//選択中の駒が解除された
				if ( isPieceCanselSelect( nowSideType, touchPieceNo ) )
				{
					//駒色デフォルト設定
					setPieceDefColor();
					//ピン削除
					field.ReleasePins();
					//フィールド待機
					field.SetWait();
					//ケース切り替え
					procNo = 500;
					break;
				}
				//移動先のマスが選択された
				if ( field.IsSelected() )
				{
					//駒色デフォルト設定
					setPieceDefColor();
					//ピン削除
					field.ReleasePins();
					//フィールド待機
					field.SetWait();
					//ケース切り替え
					procNo = 600;
					break;
				}
				break;
			//駒移動
			case 600:
				//駒のフィールド座標取得
				field.GetPieceFieldPos( out piecePosX, out piecePosY );
				//選択場所にある駒削除
				gamover = releasePiece(
					nowSideType,
					piecePosX,
					piecePosY );
				//ポーンアンパッサンしたならそれも削除
				releasePiecePassant(
					nowSideType,
					piecePosX,
					piecePosY,
					passantXList,
					passantYList );
				//駒移動
				setPieceMove(
					nowSideType,
					touchPieceNo,
					piecePosX,
					piecePosY );
				//ケース切り替え
				procNo = 601;
				break;
			case 601:
				//移動終わってないなら
				if ( !isPieceMoved( nowSideType, touchPieceNo ) )
				{
					break;
				}
#if DEBUG_MODE
				DebugMng.Log( "piece moving end." );
#endif
				//キングがとられたら
				if (gamover)
				{
					//ケース切り替え
					procNo = 800;
					break;
				}
				//移動したのがポーンなら
				//相手側フィールドの端まで到達したら
				if (isPawnGradeUp( selectPiece ))
				{
					//ケース切り替え
					procNo = 900;
					break;
				}
				//キャスリング判定
				if ( isCastling( selectPiece ) )
				{
					//ルーク分のフィールドデータ設定
					field.SetFieldDataOnlyRook( selectPiece );
					//ケース切り替え
					procNo = 1000;
					break;
				}
				//相手のキングをチェックするか
				if ( isCheck( selectPiece ) )
				{
					//ケース切り替え
					procNo = 1100;
					break;
				}
				//アンパッサン判定用設定(次のターンで使用される)
				if ( selectPiece.IsPawnJumpMove() )
				{
					//駒のフィールド座標取得
					field.GetPieceFieldPos( out piecePosX, out piecePosY );
					//フィールドにアンパッサン用情報わたす
					field.SetPawnJumpMove( piecePosX, piecePosY );
				}
				//ケース切り替え
				procNo = 700;
				break;
			//ターン切り替え
			case 700:
				//ターン切り替え
				if ( nowSideType == ChessPieces.SIDE_TYPE.BLACK )
				{
					nowSideType = ChessPieces.SIDE_TYPE.WHITE;
				}
				else
				{
					nowSideType = ChessPieces.SIDE_TYPE.BLACK;
				}
				//ケース切り替え
				procNo = 500;
				break;
			//ゲームオーバー
			case 800:
				//タイル生成
				makeTile( TILE_BLACK_COLOR );
				//ゲームオーバーテキスト生成
				makeTextGameOver( nowSideType );
				//タイマー生成
				timer = UtilityMng.MakeUtilityMng_Timer(
					gameObject,
					GAMEOVER_WAIT_TIME );
				//ケース切り替え
				procNo = 801;
				break;
			case 801:
				//タイマー終わっていないなら
				if ( !timer.IsEnd() )
				{
					break;
				}
				//開放
				UtilityMng.Release( timer );
				timer = null;
				//「タッチしてください」テキスト生成
				makeTextTouch();
				//テキストアルファ生成
				ObjectMng.MakeObjectMng_TextAlpha(
					textTouch,
					-1,
					TEXT_TIME );
				//ケース切り替え
				procNo = 802;
				break;
			case 802:
				//画面タッチされたら
				if ( !UtilityMng.IsTouchEnded() )
				{
					break;
				}
				//タイトルへ移動
				SceneMng.Change( SCENE.TITLE );
				//ケース切り替え
				procNo = 99999;
				break;
			//ポーン昇格待ち
			case 900:
				//タイル生成
				makeTile( TILE_BLACK_COLOR );
				//昇格選択用駒生成
				makeOnlySelectPiece( nowSideType );
				//ケース切り替え
				procNo = 901;
				break;
			case 901:
				//登場おわったか
				if ( !isPieceAppeared( ref piecesSelect_List ) )
				{
					break;
				}
				//タッチ判定開始
				pieceSetStart( ref piecesSelect_List );
				//ケース切り替え
				procNo = 902;
				break;
			case 902:
				//タッチ判定
				touchPieceNo = getPieceTouchedNo( ref piecesSelect_List );
				if ( touchPieceNo == -1 )
				{
					break;
				}
#if DEBUG_MODE
				//DebugMng.Log( "Select type: " + (ChessPieces.PIECE_TYPE)touchPieceNo );
#endif
				//開放
				Destroy(tile);
				tile = null;
				releasePiece( ref piecesSelect_List );
				//駒リメイク
				selectPiece.Remake( (ChessPieces.PIECE_TYPE)touchPieceNo );
				//ケース切り替え
				procNo = 700;
				break;
			//キャスリング
			case 1000:
				//移動させるルークを取得
				ChessPieces rook = getRookByCastling(
					selectPiece,
					piecesBlack_List,
					piecesWhite_List,
					ref touchPieceNo );
				//キングの座標設定
				int kingX;
				int kingY;
				selectPiece.GetFieldPos( out kingX, out kingY );
				//キングキャスリングが左側に行われたなら
				if ( selectPiece.IsCastlingLeft() )
				{
					kingX += 1;
				}
				else
				{
					kingX -= 1;
				}
				//ルーク移動
				rook.SetMove(
					kingX,
					kingY );
				//ケース切り替え
				procNo = 1001;
				break;
			case 1001:
				//移動終わってないなら
				if (!isPieceMoved( nowSideType, touchPieceNo ))
				{
					break;
				}
				//ケース切り替え
				procNo = 700;
				break;
			//チェック
			case 1100:
				//タイル生成
				makeTile( TILE_BLACK_COLOR );
				//テキスト生成
				makeTextCheck();
				//タイマー生成
				timer = UtilityMng.MakeUtilityMng_Timer(
					gameObject,
					CHECK_WAIT_TIME );
				//ケース切り替え
				procNo = 1101;
				break;
			case 1101:
				//タイマー終わっていないなら
				if (!timer.IsEnd())
				{
					break;
				}
				//開放
				UtilityMng.Release( timer );
				timer = null;
				Destroy( tile );
				tile = null;
				Destroy( textCheck );
				textCheck = null;
				//ケース切り替え
				procNo = 700;
				break;
			//終了待ち
			case 300:
				procNo = 99999;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//----------MAKE----------
	//駒生成
	void makePieces()
	{
		//フィールドデータ取得
		short[,] fieldData = field.GetFieldData();
		//フィールド数取得
		int fieldXNum = 0;
		int fieldYNum = 0;
		field.GetFieldLength( out fieldXNum, out fieldYNum );
		//フィールドobj取得
		GameObject fieldObj = field.GetObject();
		//フィールドのマップチップ取得
		GameObject[,] fieldMapChip = field.GetMapChip();
		//生成
		for (int i=0; i< fieldYNum; i++)
		{
			for (int j = 0; j < fieldXNum; j++)
			{
				//0以外なら駒
				if ( fieldData[i,j] != 0 )
				{
					//陣営判定
					bool white = field.IsWhite( fieldData[i, j] );
					int sideType = 0;
					if ( !white )
					{
						sideType = 1;
					}
					//駒種類設定
					ChessPieces.PIECE_TYPE pieceType = field.GetPieceType( j, i );
					//ピースの生成間隔取得
					int pieceDis = field.GetPieceDis();
					//生成
					ChessPieces cmpPiece = ChessPieces.MakeChessPieces(
						chessPiece,
						fieldObj,
						pieceType,
						(ChessPieces.SIDE_TYPE)sideType,
						fieldMapChip[i,j].transform.localPosition,
						j,
						i,
						pieceDis,
						ChessPieces.MODE.PIECE );
					//リストに登録
					if (sideType == (int)ChessPieces.SIDE_TYPE.BLACK)
					{
						piecesBlack_List.Add( cmpPiece );
					}
					else
					{
						piecesWhite_List.Add( cmpPiece );
					}
				}
			}
		}
	}
	//背景生成
	void makeBG()
	{
		//生成
		GameObject obj = ObjectMng.MakeSprite( bgImage );
		ObjectMng.SetParent( obj, gameObject );
		obj.transform.localEulerAngles = BG_ROT;
		obj.transform.localScale = BG_SCA;
	}
	//ゲームオーバーテキスト生成
	void makeTextGameOver(
		ChessPieces.SIDE_TYPE sideType)
	{
		GameObject text = (GameObject)Instantiate(Text3D);
		ObjectMng.SetParent( text, gameObject );
		text.transform.localPosition = TEXT_GAMEOVER_POS;
		text.transform.localEulerAngles = TEXT_GAMEOVER_ROT;
		//テキスト設定
		string mess = "";
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			mess += "黒 が 勝利!";
		}
		else
		{
			mess += "白 が 勝利!";
		}
		UtilityMng.SetText( text, mess );
		//テキスト色設定
		UtilityMng.SetTextColor(
			text,
			TEXT_GAMEOVER_MAIN_COLOR,
			TEXT_GAMEOVER_SUB_COLOR );
	}
	//タイル生成
	void makeTile( Color32 color )
	{
		tile = ObjectMng.MakeSprite( Tile );
		ObjectMng.SetParent( tile, gameObject );
		tile.transform.localPosition = TILE_POS;
		tile.transform.localEulerAngles = TILE_ROT;
		tile.transform.localScale = TILE_SCA;
		//色設定
		UtilityMng.SetSpriteColor(
			tile,
			color );
	}
	//タッチしてくださいテキスト生成
	void makeTextTouch()
	{
		textTouch = (GameObject)Instantiate( Text3D );
		ObjectMng.SetParent( textTouch, gameObject );
		textTouch.transform.localPosition = TEXT_TOUCH_POS;
		textTouch.transform.localEulerAngles = TEXT_TOUCH_ROT;
		textTouch.transform.localScale = TEXT_TOUCH_SCA;
		//テキスト設定
		string mess = "タッチしてください";
		UtilityMng.SetText( textTouch, mess );
		//テキスト色設定
		UtilityMng.SetTextColor(
			textTouch,
			TEXT_GAMEOVER_MAIN_COLOR,
			TEXT_GAMEOVER_SUB_COLOR );
	}
	//タッチしてくださいテキスト生成
	void makeTextCheck()
	{
		textCheck = (GameObject)Instantiate( Text3D );
		ObjectMng.SetParent( textCheck, gameObject );
		textCheck.transform.localPosition = TEXT_CHECK_POS;
		textCheck.transform.localEulerAngles = TEXT_CHECK_ROT;
		textCheck.transform.localScale = TEXT_CHECK_SCA;
		//テキスト設定
		string mess = "チェック";
		UtilityMng.SetText( textCheck, mess );
		//テキスト色設定
		UtilityMng.SetTextColor(
			textCheck,
			TEXT_GAMEOVER_MAIN_COLOR,
			TEXT_GAMEOVER_SUB_COLOR );
	}
	//ポーン昇格用駒生成
	void makeOnlySelectPiece(
		ChessPieces.SIDE_TYPE sideType )
	{
		for (int i=0; i<(int)ChessPieces.PIECE_TYPE.END; i++)
		{
			//キングは作らない
			if ( (ChessPieces.PIECE_TYPE)i == ChessPieces.PIECE_TYPE.KING )
			{
				continue;
			}
			//座標設定
			Vector3 pos = new Vector3( 0.0f, 23.0f, 0.0f  );
			pos.x = -20.0f + 20.0f * (i % ((int)ChessPieces.PIECE_TYPE.END / 2));
			pos.z = 5.0f - 25.0f * (i / ((int)ChessPieces.PIECE_TYPE.END / 2));
			DebugMng.Log( "i % 3 = " + (i%3) );
			//生成
			ChessPieces cmp = ChessPieces.MakeChessPieces(
				chessPiece,
				gameObject,
				(ChessPieces.PIECE_TYPE)i,
				sideType,
				pos,
				0,
				0,
				0,
				ChessPieces.MODE.SELECT );
			cmp.SetRealRotate( PIECE_SELECT_ROT );
			//リストに登録
			piecesSelect_List.Add( cmp );
		}
	}
	//----------RELEASE----------
	//駒削除
	bool releasePiece(
		ChessPieces.SIDE_TYPE sideType,
		int posX,
		int posY )
	{
		//黒ターンなら
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			for ( int i=0; i<piecesWhite_List.Count; i++ )
			{
				int fieldX;
				int fieldY;
				piecesWhite_List[i].GetFieldPos(out fieldX, out fieldY);
				//座標が同じなら
				if ( fieldX == posX &&
					fieldY == posY )
				{
					//駒種類取得
					ChessPieces.PIECE_TYPE pieceType = piecesWhite_List[i].GetPieceType();
					//開放
					piecesWhite_List[i].Release();
					piecesWhite_List.RemoveAt(i);
					//キングなら
					if ( pieceType == ChessPieces.PIECE_TYPE.KING )
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}
		else
		{
			for (int i = 0; i < piecesBlack_List.Count; i++)
			{
				int fieldX;
				int fieldY;
				piecesBlack_List[i].GetFieldPos( out fieldX, out fieldY );
				//座標が同じなら
				if (fieldX == posX &&
					fieldY == posY)
				{
					//駒種類取得
					ChessPieces.PIECE_TYPE pieceType = piecesBlack_List[i].GetPieceType();
					//開放
					piecesBlack_List[i].Release();
					piecesBlack_List.RemoveAt( i );
					//キングなら
					if (pieceType == ChessPieces.PIECE_TYPE.KING)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}
		return false;
	}
	//駒開放（リストver）
	void releasePiece( ref List<ChessPieces> pieceList )
	{
		for (int i = 0; i < pieceList.Count; i++)
		{
			pieceList[i].Release();
			pieceList.RemoveAt( i );
			i--;
		}
		piecesSelect_List.Clear();
	}
	//ポーンアンパッサンされたポーン駒削除
	void releasePiecePassant(
		ChessPieces.SIDE_TYPE sideType,
		int posX,
		int posY,
		List<int> xList,
		List<int> yList )
	{
		//リストがないなら失敗
		if ( xList.Count <= 0 )
		{
			return;
		}
		//黒ターンなら
		if (sideType == ChessPieces.SIDE_TYPE.BLACK)
		{
			for (int i = 0; i < piecesWhite_List.Count; i++)
			{
				//駒種類取得
				ChessPieces.PIECE_TYPE pieceType = piecesWhite_List[i].GetPieceType();
				//ポーン以外ならスルー
				if ( pieceType != ChessPieces.PIECE_TYPE.PAWN )
				{
					continue;
				}
				int fieldX;
				int fieldY;
				piecesWhite_List[i].GetFieldPos( out fieldX, out fieldY );
				//座標が同じなら
				if (fieldX == posX &&
					fieldY == posY - 1 )
				{
					//開放
					piecesWhite_List[i].Release();
					piecesWhite_List.RemoveAt( i );
					return;
				}
			}
		}
		else
		{
			for (int i = 0; i < piecesBlack_List.Count; i++)
			{
				//駒種類取得
				ChessPieces.PIECE_TYPE pieceType = piecesBlack_List[i].GetPieceType();
				//ポーン以外ならスルー
				if (pieceType != ChessPieces.PIECE_TYPE.PAWN)
				{
					continue;
				}
				int fieldX;
				int fieldY;
				piecesBlack_List[i].GetFieldPos( out fieldX, out fieldY );
				//座標が同じなら
				if (fieldX == posX &&
					fieldY == posY + 1)
				{
					//開放
					piecesBlack_List[i].Release();
					piecesBlack_List.RemoveAt( i );
					return;
				}
			}
		}
	}
	//----------IS----------
	//駒登場終わったか
	bool isPieceAppeared( ref List<ChessPieces> pieceList )
	{
		for (int i=0; i< pieceList.Count; i++)
		{
			if( !pieceList[i].IsAppear() )
			{
				return false;
			}
		}
		return true;
	}
	//選択キャンセルされたか
	bool isPieceCanselSelect(
		ChessPieces.SIDE_TYPE sideType,
		int touchNo )
	{
		if (sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			return piecesBlack_List[touchNo].IsCancel();
		}
		else
		{
			return piecesWhite_List[touchNo].IsCancel();
		}
	}
	//駒移動終わったか
	bool isPieceMoved(
		ChessPieces.SIDE_TYPE sideType,
		int touchNo )
	{
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			return piecesBlack_List[touchNo].IsMoved();
		}
		else
		{
			return piecesWhite_List[touchNo].IsMoved();
		}
	}
	//ポーン昇格タイミングチェック
	bool isPawnGradeUp( ChessPieces cmp)
	{
		//ポーンじゃないなら
		ChessPieces.PIECE_TYPE pieceType = cmp.GetPieceType();
		if ( pieceType != ChessPieces.PIECE_TYPE.PAWN )
		{
			return false;
		}
		//昇格できるフィールド座標か
		short[,] fieldData = field.GetNowFieldData();
		if (cmp.IsFieldEdge( fieldData.GetLength(0) - 1 ) )
		{
			return true;
		}
		return false;
	}
	//キングのキャスリング判定
	bool isCastling( ChessPieces cmp )
	{
		//キングじゃないなら
		ChessPieces.PIECE_TYPE pieceType = cmp.GetPieceType();
		if (pieceType != ChessPieces.PIECE_TYPE.KING)
		{
			return false;
		}
		//キャスリングしてるなら
		if ( cmp.IsCastling() )
		{
			return true;
		}
		return false;
	}
	//相手のキングをチェックするか
	bool isCheck( ChessPieces cmp )
	{
		//陣営設定
		ChessPieces.SIDE_TYPE sideType = cmp.GetSideType();
		//キング取得
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			for (int i=0; i<piecesWhite_List.Count; i++)
			{
				ChessPieces.PIECE_TYPE pieceType = piecesWhite_List[i].GetPieceType();
				//キングではないなら
				if ( pieceType != ChessPieces.PIECE_TYPE.KING )
				{
					continue;
				}
				return field.IsCheck( piecesWhite_List[i], piecesBlack_List, piecesWhite_List );
			}
		}
		else
		{
			for (int i = 0; i < piecesBlack_List.Count; i++)
			{
				ChessPieces.PIECE_TYPE pieceType = piecesBlack_List[i].GetPieceType();
				//キングではないなら
				if (pieceType != ChessPieces.PIECE_TYPE.KING)
				{
					continue;
				}
				return field.IsCheck( piecesBlack_List[i], piecesBlack_List, piecesWhite_List );
			}
		}
		return false;
	}
	//----------SET----------
	//駒のタッチ判定開始
	void pieceSetStart( ref List<ChessPieces> pieceList)
	{
		for (int i=0; i< pieceList.Count; i++)
		{
			pieceList[i].SetTouchOK();
		}
	}
	//引数番号以外の駒は待機状態に設定
	void setPieceWait( int touchNo )
	{
		for (int i=0; i<piecesBlack_List.Count; i++)
		{
			//ターン中で番号が同じなら
			if ( nowSideType == ChessPieces.SIDE_TYPE.BLACK &&
				i == touchNo )
			{
				continue;
			}
			piecesBlack_List[i].SetWait();
		}
		for (int i = 0; i < piecesWhite_List.Count; i++)
		{
			//ターン中で番号が同じなら
			if (nowSideType == ChessPieces.SIDE_TYPE.WHITE &&
				i == touchNo)
			{
				continue;
			}
			piecesWhite_List[i].SetWait();
		}
	}
	//駒移動設定
	void setPieceMove(
		ChessPieces.SIDE_TYPE sideType,
		int moveNo,
		int movePosX,
		int movePosY )
	{
		//黒番なら
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			piecesBlack_List[moveNo].SetMove( movePosX, movePosY );
		}
		else
		{
			piecesWhite_List[moveNo].SetMove( movePosX, movePosY );
		}
	}
	//駒の色設定
	void setPieceColor(
		Color color,
		ChessPieces.SIDE_TYPE sideType,
		List<int> canPutX,
		List<int> canPutY )
	{
		for (int i=0; i<canPutX.Count; i++)
		{
			if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
			{
				//白陣営
				for (int j = 0; j < piecesWhite_List.Count; j++)
				{
					//フィールド座標取得
					int fieldX = 0;
					int fieldY = 0;
					piecesWhite_List[j].GetFieldPos( out fieldX, out fieldY );
					//座標が同じ駒なら
					if (canPutX[i] == fieldX &&
						canPutY[i] == fieldY)
					{
						piecesWhite_List[j].SetColor( color );
						break;
					}
				}
			}
			else
			{
				//黒陣営
				for (int j = 0; j < piecesBlack_List.Count; j++)
				{
					//フィールド座標取得
					int fieldX = 0;
					int fieldY = 0;
					piecesBlack_List[j].GetFieldPos( out fieldX, out fieldY );
					//座標が同じ駒なら
					if (canPutX[i] == fieldX &&
						canPutY[i] == fieldY)
					{
						piecesBlack_List[j].SetColor( color );
						break;
					}
				}
			}
		}
	}
	//駒の色をデフォルト色に設定
	void setPieceDefColor()
	{
		//白陣営
		for (int j = 0; j < piecesWhite_List.Count; j++)
		{
			piecesWhite_List[j].SetColor( ChessPieces.PIECE_DEF_COLOR );
		}
		//黒陣営
		for (int j = 0; j < piecesBlack_List.Count; j++)
		{
			piecesBlack_List[j].SetColor( ChessPieces.PIECE_DEF_COLOR );
		}
	}
	//駒のコライダートリガー設定
	void setPieceColliderTrriger( ChessPieces.SIDE_TYPE sideType )
	{
		//トリガーフラグ設定
		bool whiteTrigger = false;
		if (sideType == ChessPieces.SIDE_TYPE.BLACK)
		{
			whiteTrigger = true;
		}
		else
		{
			whiteTrigger = false;
		}
		//白陣営
		for (int j = 0; j < piecesWhite_List.Count; j++)
		{
			piecesWhite_List[j].SetColliderTrriger( whiteTrigger );
		}
		//黒陣営
		for (int j = 0; j < piecesBlack_List.Count; j++)
		{
			piecesBlack_List[j].SetColliderTrriger( !whiteTrigger );
		}
	}
	//----------GET----------
	//タッチ許可
	int getPieceTouchedNo( ref List<ChessPieces> pieceList )
	{
		//判定
		int touchNo = -1;
		for (int i=0; i< pieceList.Count; i++)
		{
			//タッチされているのがあるなら
			if (pieceList[i].IsTouch() )
			{
				touchNo = i;
				break;
			}
		}
		return touchNo;
	}
	//選択中の駒種類取得
	ChessPieces.PIECE_TYPE getPieceType(
		ChessPieces.SIDE_TYPE sideType,
		int touchNo )
	{
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			return piecesBlack_List[touchNo].GetPieceType();
		}
		else
		{
			return piecesWhite_List[touchNo].GetPieceType();
		}
	}
	//フィールド内での座標取得
	void getPieceFieldPos(
		ChessPieces.SIDE_TYPE sideType,
		int touchNo,
		out int fieldPosX,
		out int fieldPosY )
	{
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			piecesBlack_List[touchNo].GetFieldPos( out fieldPosX, out fieldPosY );
		}
		else
		{
			piecesWhite_List[touchNo].GetFieldPos( out fieldPosX, out fieldPosY );
		}
	}
	//駒ポンポーネントを取得
	ChessPieces getPieceCmp(
		ChessPieces.SIDE_TYPE sideType,
		int touchNo )
	{
		if (sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			return piecesBlack_List[touchNo];
		}
		else
		{
			return piecesWhite_List[touchNo];
		}
	}
	//ポーン昇格時の駒選択判定
	int getSelectPieceNo()
	{
		for (int i=0; i<piecesSelect_List.Count; i++)
		{
			//if ()
			//{

			//}
		}
		return -1;
	}
	//キングのキャスリングにより移動させるルークを返す
	ChessPieces getRookByCastling(
		ChessPieces cmp,
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList,
		ref int touchNo )
	{
		//左側に移動したか
		bool left = cmp.IsCastlingLeft();
		//キングの現在の座標取得
		int kingX;
		int kingY;
		cmp.GetFieldPos( out kingX, out kingY );
		//判定
		ChessPieces.SIDE_TYPE sideType = cmp.GetSideType();
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			for (int i=0; i<blackList.Count; i++)
			{
				//ルーク以外ならスルー
				ChessPieces.PIECE_TYPE pieceType = blackList[i].GetPieceType();
				if ( pieceType != ChessPieces.PIECE_TYPE.ROOK )
				{
					continue;
				}
				//移動してるならスルー
				if ( blackList[i].IsFirstMoved() )
				{
					continue;
				}
				//座標が左側なら
				int rookX;
				int rookY;
				blackList[i].GetFieldPos(out rookX, out rookY);
				//Yがキングと異なるなら
				if ( rookY != kingY )
				{
					continue;
				}
				//Xがキングより小さいなら
				if ( left && rookX < kingX )
				{
					touchNo = i;
					return blackList[i];
				}
				else if (!left && rookX > kingX)
				{
					touchNo = i;
					return blackList[i];
				}
			}
		}
		else
		{
			for (int i = 0; i < whiteList.Count; i++)
			{
				//ルーク以外ならスルー
				ChessPieces.PIECE_TYPE pieceType = whiteList[i].GetPieceType();
				if (pieceType != ChessPieces.PIECE_TYPE.ROOK)
				{
					continue;
				}
				//移動してるならスルー
				if (whiteList[i].IsFirstMoved())
				{
					continue;
				}
				//座標が左側なら
				int rookX;
				int rookY;
				whiteList[i].GetFieldPos( out rookX, out rookY );
				//Yがキングと異なるなら
				if (rookY != kingY)
				{
					continue;
				}
				//Xがキングより小さいなら
				if (left && rookX < kingX)
				{
					touchNo = i;
					return whiteList[i];
				}
				else if (!left && rookX > kingX)
				{
					touchNo = i;
					return whiteList[i];
				}
			}
		}
		return null;
	}
	//----------OTHER----------
}
