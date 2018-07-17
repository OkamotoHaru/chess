#if UNITY_DEBUG
	//#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	public enum FIELD_TYPE {
		NORMAL,			//練習用
		PAWN_GRADEUP,	//ポーン昇格、アンパッサン確認用
		CASTLING,		//キャスリング確認用
		END
	}
	const int B = 0;	//黒陣営
	const int W = 100;	//白陣営
	//*フィールド
	//
	// 1	ポーン
	// 2	ナイト
	// 3	ビショップ
	// 4	ルーク
	// 5	クイーン
	// 6	キング
	//
	//通常
	short[,] fieldData_Normal =
	{
		//練習用
		{ B+4, B+2, B+3, B+5, B+6, B+3, B+2, B+4, },
		{ B+1, B+1, B+1, B+1, B+1, B+1, B+1, B+1, },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ W+1, W+1, W+1, W+1, W+1, W+1, W+1, W+1, },
		{ W+4, W+2, W+3, W+5, W+6, W+3, W+2, W+4, },
	};
	short[,] fieldData_PawnGradeUp =
	{
		//練習用
		{ 0,   0,   0,   0  , B+6, 0,   0,   0,   },
		{ 0,   0,   0,   0,   B+1, B+1, B+1, B+1, },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ W+1, W+1, W+1, W+1, W+1, 0,   0,   0,   },
		{ 0,   0,   0,   0  , W+6, 0,   0,   0,   },
	};
	short[,] fieldData_Castling =
	{
		//練習用
		{ B+4, 0,   0,   0  , B+6, 0,   0,   B+4, },
		{ 0,   0,   0,   0,   0,   0,   B+5, 0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   0,   0,   0,   0,   0,   0,   0,   },
		{ 0,   W+5, 0,   0,   0,   0,   0,   0,   },
		{ W+4, 0,   0,   0  , W+6, 0,   0,   W+4, },
	};
	//ピースの幅
	int PIECE_SIZE_X = 9;
	int PIECE_SIZE_Z = 9;
	//ピースのスケール
	float PIECE_SCA_X = 9;
	//----------CLASS----------
	//----------PUBLIC----------
	public GameObject[] Pieces;
	public GameObject Pin;
	//----------PRIVATE----------
	//管理系
	int procNo = 0;
	FIELD_TYPE fieldType = FIELD_TYPE.NORMAL;
	//フラグ
	bool init = false;
	bool selected = false;
	bool pawnJumpMove = false;	//ポーンが２マス移動したなら
	//オブジェクト
	GameObject field = null;
	GameObject[,] mapchip;
	List<GameObject> pin = new List<GameObject>();
	List<GameObject> judgeTouch = new List<GameObject>();
	//フィールド
	short[,] useFieldData;
	//選択判定用
	List<int> canPutX_List = new List<int>();
	List<int> canPutY_List = new List<int>();
	int nowCheckPosX;	//マス選択判定中の選択開始座標
	int nowCheckPosY;	//マス選択判定中の選択開始座標
	int newPiecePosX;	//マスを選択した後の駒座標
	int newPiecePosY;	//マスを選択した後の駒座標
	//アンパッサン用
	int passantPosX;
	List<int> passantX_List = new List<int>();
	List<int> passantY_List = new List<int>();
	//キャスリング用
	List<int> castlingX_List = new List<int>();
	List<int> castlingY_List = new List<int>();
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//生成
	public static Field MakeField(
		GameObject prefab,
		GameObject parent,
		FIELD_TYPE fieldType )
	{
		GameObject obj = (GameObject)Instantiate(prefab);
		ObjectMng.SetParent( obj, parent );
		Field cmp = obj.GetComponent<Field>();
		cmp.fieldType = fieldType;
		return cmp;
	}
	public static Field MakeField_AddCmp( GameObject addCmp )
	{
		Field cmp = addCmp.AddComponent<Field>();
		return cmp;
	}
	//----------RELEASE----------
	//開放
	public static void Release( Field cmp ){
		//nullなら失敗
		if (cmp == null)
		{
			return;
		}
		cmp.Release();
	}
	//----------IS----------
	//初期化したか
	public static bool IsInit( Field cmp ){
		//nullなら失敗
		if ( cmp == null ){
			return false;
		}
		return cmp.IsInit();
	}
	//終了したか
	public static bool IsEnd(Field cmp)
	{
		//nullなら失敗
		if (cmp == null)
		{
			return false;
		}
		return cmp.IsEnd();
	}
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
	//ピン生成
	public void MakePin(
		ChessPieces cmp,
		ref List<int> xList,
		ref List<int> yList,
		ref List<int> passantXList,
		ref List<int> passantYList,
		ref List<ChessPieces> blackList,
		ref List<ChessPieces> whiteList,
		bool dummy = false )	//チェックのみ行う
	{
		//現在のフィールド座標取得
		int fieldX = 0;
		int fieldY = 0;
		cmp.GetFieldPos( out fieldX, out fieldY );
		//駒種類取得
		ChessPieces.PIECE_TYPE pieceType = cmp.GetPieceType();
		//リストクリア
		List<int> canPutDummyXList = new List<int>();
		List<int> canPutDummyYList = new List<int>();
		List<int> passantDummyXList = new List<int>();
		List<int> passantDummyYList = new List<int>();
		List<int> castlingDummyXList = new List<int>();
		List<int> castlingDummyYList = new List<int>();
		//何個生成するか計算
		switch ( pieceType )
		{
			case ChessPieces.PIECE_TYPE.PAWN:
				checkPawnCanPut(
					cmp,
					fieldX,
					fieldY,
					ref canPutDummyXList,
					ref canPutDummyYList,
					ref passantDummyXList,
					ref passantDummyYList );
				break;
			case ChessPieces.PIECE_TYPE.KNIGHT:
				checkPawnCanPut(
					cmp,
					fieldX,
					fieldY,
					ref canPutDummyXList,
					ref canPutDummyYList );
				break;
			case ChessPieces.PIECE_TYPE.BISHOP:
				checkBishopCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType() );
				break;
			case ChessPieces.PIECE_TYPE.ROOK:
				checkRookCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType() );
				break;
			case ChessPieces.PIECE_TYPE.QUEEN:
				checkBishopCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType() );
				checkRookCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType() );
				break;
			case ChessPieces.PIECE_TYPE.KING:
				checkBishopCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType(),
					true );
				checkRookCanPut(
					ref canPutDummyXList,
					ref canPutDummyYList,
					fieldX,
					fieldY,
					cmp.GetSideType(),
					true );
				checkKingCanPut(
					cmp,
					fieldX,
					fieldY,
					ref canPutDummyXList,
					ref canPutDummyYList,
					dummy,
					blackList,
					whiteList,
					ref castlingDummyXList,
					ref castlingDummyYList );
				break;
		}
		//１つも置けないなら
		if ( canPutDummyXList.Count <= 0 )
		{
			return;
		}
		//返答用リストに設定
		xList = canPutDummyXList;
		yList = canPutDummyYList;
		passantXList = passantX_List;
		passantYList = passantY_List;
		//チェックのみ行うなら
		if (dummy)
		{
			return;
		}
		//リストに設定
		canPutX_List.Clear();
		canPutX_List = canPutDummyXList;
		canPutY_List.Clear();
		canPutY_List = canPutDummyYList;
		passantX_List.Clear();
		passantX_List = passantDummyXList;
		passantY_List.Clear();
		passantY_List = passantDummyYList;
		castlingX_List.Clear();
		castlingX_List = castlingDummyXList;
		castlingY_List.Clear();
		castlingY_List = castlingDummyYList;
		//生成
		judgeTouch.Clear();
		pin.Clear();
		for (int i=0; i<canPutX_List.Count; i++)
		{
			//座標取得
			judgeTouch.Add( mapchip[canPutY_List[i], canPutX_List[i]] );
			Vector3 makePos = judgeTouch[judgeTouch .Count - 1].transform.localPosition;
			//生成
			GameObject obj = (GameObject)Instantiate(Pin);
			ObjectMng.SetParent(obj, gameObject);
			//座標設定
			obj.transform.localPosition = makePos;
			//リストに登録
			pin.Add( obj );
		}
		//移動予定の駒の座標設定
		nowCheckPosX = fieldX;
		nowCheckPosY = fieldY;

#if DEBUG_MODE
		for (int i=0; i< canPutX_List.Count; i++)
		{
			DebugMng.Log( "can pos num: X" + canPutX_List[i] + ",Y" + canPutY_List[i] );
		}
#endif
	}
	//----------RELEASE----------
	//このコンポーネントがついたオブジェクトを開放
	public void Release()
	{
		Destroy(this.gameObject);
	}
	//ピン開放
	public void ReleasePins()
	{
		//リストがないなら何もしない
		if ( pin.Count <= 0 )
		{
			return;
		}
		for (int i=0; i<pin.Count; i++)
		{
			Destroy(pin[i]);
			pin.RemoveAt(i);
			i--;
		}
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
	//初期化したか
	public bool IsInit(){
		return init;
	}
	//白陣営か
	public bool IsWhite(int num)
	{
		if ( num >= W )
		{
			return true;
		}
		return false;
	}
	//選択したか
	public bool IsSelected()
	{
		return selected;
	}
	//"チェック"状態にあるなら
	public bool IsCheck(
		ChessPieces me,						//チェックする駒
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList )
	{
		List<ChessPieces> dangerList;
		return IsCheck( me, blackList, whiteList, out dangerList );
	}
	public bool IsCheck(
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList,
		ChessPieces.SIDE_TYPE sideType,
		int posX,
		int posY )
	{
		List<ChessPieces> dangerList;
		return IsCheck( blackList, whiteList, out dangerList, sideType, posX, posY );
	}
	public bool IsCheck(
		ChessPieces me,						//チェックする駒
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList,
		out List<ChessPieces> dangerList )	//とられる駒
	{
		dangerList = new List<ChessPieces>();
		//自分の座標設定
		int meX;
		int meY;
		me.GetFieldPos( out meX, out meY );
		//陣営設定
		ChessPieces.SIDE_TYPE sideType = me.GetSideType();
		//判定
		IsCheck(
			blackList,
			whiteList,
			out dangerList,
			sideType,
			meX,
			meY );
		//１つ以上あるなら
		if ( dangerList.Count > 0 )
		{
			return true;
		}
		return false;
	}
	public bool IsCheck(
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList,
		out List<ChessPieces> dangerList,	//とられる駒
		ChessPieces.SIDE_TYPE sideType,
		int posX,
		int posY )
	{
		dangerList = new List<ChessPieces>();
		//陣営設定
		if ( sideType == ChessPieces.SIDE_TYPE.BLACK )
		{
			for (int i = 0; i < whiteList.Count; i++)
			{
				//ピンが置ける場所調べる
				List<int> canPutXList = new List<int>();
				List<int> canPutYList = new List<int>();
				List<int> passantXList = new List<int>();
				List<int> passantYList = new List<int>();
				MakePin( whiteList[i], ref canPutXList, ref canPutYList, ref passantXList, ref passantYList, ref whiteList, ref blackList, true );
				//１つでもとられる場所があるなら
				for (int j = 0; j < canPutXList.Count; j++)
				{
					if (canPutXList[j] == posX && canPutYList[j] == posY)
					{
						dangerList.Add( whiteList[i] );
					}
				}
			}
		}
		else
		{
			for ( int i=0; i< blackList.Count; i++ )
			{
				//ピンが置ける場所調べる
				List<int> canPutXList = new List<int>();
				List<int> canPutYList = new List<int>();
				List<int> passantXList = new List<int>();
				List<int> passantYList = new List<int>();
				MakePin( blackList[i], ref canPutXList, ref canPutYList, ref passantXList, ref passantYList, ref blackList, ref whiteList, true );
				//１つでもとられる場所があるなら
				for (int j=0; j<canPutXList.Count; j++)
				{
					if (canPutXList[j] == posX && canPutYList[j] == posY)
					{
						dangerList.Add( blackList[i] );
					}
				}
			}
		}
		//１つ以上あるなら
		if ( dangerList.Count > 0 )
		{
			return true;
		}
		return false;
	}
	//----------SET----------
	//マスタッチ判定開始
	public void SetSelect()
	{
		procNo = 200;
	}
	//フィールド待機設定
	public void SetWait()
	{
		procNo = 100;
	}
	//ポーン２マス移動設定
	public void SetPawnJumpMove(
		int posX,
		int posY )
	{
		pawnJumpMove = true;
		passantPosX = posX;
		//passantPosY = posY;
	}
	//キャスリングされたルークのフィールドデータ設定
	public void SetFieldDataOnlyRook( ChessPieces cmp )
	{
		ChessPieces.PIECE_TYPE pieceType = cmp.GetPieceType();
		//動いていたのがキング以外なら
		if ( pieceType != ChessPieces.PIECE_TYPE.KING )
		{
			return;
		}
		//左右どっちに動いたか判定
		//左側に動いたなら
		if ( newPiecePosX < nowCheckPosX )
		{
			//キャスリングできるどのルークを使うか
			int useRookNo = -1;
			for (int i = 0; i < castlingX_List.Count; i++)
			{
				if ( castlingX_List[i] < newPiecePosX )
				{
					useRookNo = i;
					break;
				}
			}
			//スワップ
			short data = useFieldData[castlingY_List[useRookNo], castlingX_List[useRookNo]];
			useFieldData[castlingY_List[useRookNo], castlingX_List[useRookNo]] = 0;
			useFieldData[newPiecePosY, newPiecePosX + 1] = data;
		}
		else
		{
			//キャスリングできるどのルークを使うか
			int useRookNo = -1;
			for (int i = 0; i < castlingX_List.Count; i++)
			{
				if (castlingX_List[i] > newPiecePosX)
				{
					useRookNo = i;
					break;
				}
			}
			//スワップ
			short data = useFieldData[castlingY_List[useRookNo], castlingX_List[useRookNo]];
			useFieldData[castlingY_List[useRookNo], castlingX_List[useRookNo]] = 0;
			useFieldData[newPiecePosY, newPiecePosX - 1] = data;
		}
	}
	//----------GET----------
	//使用するマップの配列数を返す
	public short[,] GetFieldData()
	{
		switch (fieldType)
		{
			//通常
			case FIELD_TYPE.NORMAL:
				return fieldData_Normal;
			//ポーン昇格確認用
			case FIELD_TYPE.PAWN_GRADEUP:
				return fieldData_PawnGradeUp;
			//キャスリング確認用
			case FIELD_TYPE.CASTLING:
				return fieldData_Castling;
			//それ以外なら
			case FIELD_TYPE.END:
			default:
				DebugMng.Log( "field type is FIELD_TYPE.END or undefined." );
				return fieldData_Normal;
		}
	}
	//現在のフィールドデータ
	public short[,] GetNowFieldData()
	{
		return useFieldData;
	}
	//使用するマップの配列数を返す
	public void GetFieldLength(out int lengthX, out int lengthY)
	{
		lengthX = useFieldData.GetLength( 1 );
		lengthY = useFieldData.GetLength( 0 );
	}
	//フィールドを返す
	public GameObject GetObject()
	{
		return field;
	}
	//マップチップを全て返す
	public GameObject[,] GetMapChip()
	{
		return mapchip;
	}
	//駒種類返す
	public ChessPieces.PIECE_TYPE GetPieceType(
		int posX,
		int posY )
	{
		//白陣営なら
		if (useFieldData[posY, posX] >= W)
		{
			return (ChessPieces.PIECE_TYPE)(useFieldData[posY, posX] - W - 1);
		}
		return (ChessPieces.PIECE_TYPE)(useFieldData[posY, posX] - 1);
	}
	//マス選択後の駒フィールド座標を返す
	public void GetPieceFieldPos(
		out int posX,
		out int posY )
	{
		posX = newPiecePosX;
		posY = newPiecePosY;
	}
	//ピースの間隔を返す
	public int GetPieceDis()
	{
		return PIECE_SIZE_X;
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
		//フィールド生成
		makeField();
		//初期化フラグtrue
		init = true;
	}
	//ずっと通る
	void Update ()
	{
#if DEBUG_MODE
		//DebugMng.ScreenLog("Field() procNo: " + procNo);
#endif
		switch ( procNo )
		{
			//初期化
			case 0:
				//ケース切り替え
				procNo = 100;
				break;
			//待機
			case 100:
				break;
			//タッチ判定
			case 200:
				//選択フラグfalse
				selected = false;
				//選択可能マスがないなら
				if ( judgeTouch.Count <= 0 )
				{
					//ケース切り替え
					procNo = 100;
					break;
				}
				//ケース切り替え
				procNo = 201;
				break;
			case 201:
				//タッチされていないなら
				int touchNo = -1;
				touchNo = isTouchField();
				if (touchNo == -1)
				{
					break;
				}
				//選択したフラグtrue
				selected = true;
				//フィールド状態更新
				short data = useFieldData[nowCheckPosY, nowCheckPosX];
				useFieldData[nowCheckPosY, nowCheckPosX] = 0;
				useFieldData[canPutY_List[touchNo], canPutX_List[touchNo]] = data;
				//駒座標設定
				newPiecePosX = canPutX_List[touchNo];
				newPiecePosY = canPutY_List[touchNo];
				//ケース切り替え
				procNo = 100;
				break;
			//終了
			case 99999:
				break;
		}
	}
	//----------MAKE----------
	//フィールド生成
	 void makeField()
	 {
		//親生成
		field = new GameObject("field");
		ObjectMng.SetParent( field, gameObject);
		//モードから使用するデータ設定
		useFieldData = GetFieldData();
		//配列数取得
		int arrayX = useFieldData.GetLength( 1 );
		int arrayZ = useFieldData.GetLength( 0 );
#if DEBUG_MODE
		Debug.Log( "field x length : " + arrayX + " odd:" + (arrayX % 2) );
		Debug.Log( "field z length : " + arrayZ + " odd:" + (arrayZ % 2));
		//Debug.Log("" + (-((float)PIECE_SIZE_X / 2) * (1 - (arrayX % 2))) );
#endif
		//ピース置き始める座標設定
		Vector3 startPos = new Vector3();
		startPos.x = -((float)PIECE_SIZE_X / 2 * ( 1 - arrayX % 2)) - (arrayX / 2 - 1) * PIECE_SIZE_X;
		startPos.z = ((float)PIECE_SIZE_Z / 2 * (1 - arrayZ % 2)) + (arrayZ / 2 - 1) * PIECE_SIZE_Z;
		//配列分マップチップ配列生成
		mapchip = new GameObject[arrayZ, arrayX];
		//配列分のピース生成
		short pieceNo = 0;
		for (int i=0; i<arrayZ; i++)
		{
			for (int j = 0; j < arrayX; j++)
			{
				mapchip[i,j] = (GameObject)Instantiate( Pieces[pieceNo] );
				ObjectMng.SetParent( mapchip[i, j], field );
				//座標計算
				Vector3 piecePos = new Vector3();
				piecePos.x = startPos.x + PIECE_SIZE_X * j;
				piecePos.z = startPos.z - PIECE_SIZE_Z * i;
				//座標設定
				mapchip[i, j].transform.localPosition = piecePos;
				//スケール設定
				mapchip[i, j].transform.localScale = new Vector3( PIECE_SCA_X, 1.0f, PIECE_SCA_X );
				//使用するピース反転（黒　←→　白）
				pieceNo = ReverseUsePiece( pieceNo );
			}
			//使用するピース反転（黒　←→　白）
			pieceNo = ReverseUsePiece( pieceNo );
		}
	}
	//----------RELEASE----------
	//----------IS----------
	//何もないマスか
	bool isCanPutEmpty(
		int checkPosX,
		int checkPosY,
		ChessPieces.SIDE_TYPE sideType)
	{
		if (useFieldData[checkPosY, checkPosX] == 0)
		{
			return true;
		}
		return false;
	}
	//敵の場所におけるか
	bool isCanPutEnemy(
		int checkPosX,
		int checkPosY,
		ChessPieces.SIDE_TYPE sideType)
	{
		//白番の時置き場所が黒なら
		//黒番の時置き場所が白なら
		if ((IsWhite( useFieldData[checkPosY, checkPosX] ) && sideType == ChessPieces.SIDE_TYPE.BLACK) ||
			(!IsWhite( useFieldData[checkPosY, checkPosX] ) && sideType == ChessPieces.SIDE_TYPE.WHITE))
		{
			return true;
		}
		return false;
	}
	//選択可能なマスをタッチしたら
	int isTouchField()
	{
		for (int i=0; i<judgeTouch.Count; i++)
		{
			if ( UtilityMng.IsTouchEnded(judgeTouch[i]) )
			{
				return i;
			}
		}
		return -1;
	}
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
	//使用するピース反転
	short ReverseUsePiece(short no)
	{
		no += 1;
		if (no >= 2)
		{
			no = 0;
		}
		return no;
	}
	//フィールド配列外参照
	bool checkOutOfRangeField(
		int posX,
		int posY,
		out int fixPosX,
		out int fixPosY )
	{
		//初期値設定
		fixPosX = posX;
		fixPosY = posY;
		//配列外なら
		if (posX < 0 || posX >= useFieldData.GetLength( 1 ))
		{
			fixPosX = 0;
			return false;
		}
		if (posY < 0 || posY >= useFieldData.GetLength( 0 ))
		{
			fixPosY = 0;
			return false;
		}
		return true;
	}
	//ポーンの置ける場所設定
	void checkPawnCanPut(
		ChessPieces cmp,
		int fieldX,
		int fieldY,
		ref List<int> canPutDummyXList,
		ref List<int> canPutDummyYList,
		ref List<int> passantDummyXList,
		ref List<int> passantDummyYList)
	{
		//チェックする座標
		int checkPosX = fieldX;
		int checkPosY = fieldY;
		if (cmp.GetSideType() == ChessPieces.SIDE_TYPE.BLACK)
		{
			checkPosY += 1;
		}
		else
		{
			checkPosY -= 1;
		}
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//アンパッサンルールチェック(左斜め)
		checkPosX = fieldX - 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//敵なら
			if (!isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) &&
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//アンパッサンルールチェック(右斜め)
		checkPosX = fieldX + 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//敵なら
			if (!isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) &&
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//敵ポーン限定
		if (cmp.IsFirstMoved() && pawnJumpMove) //初回移動してない時はポーンアンパッサンできる座標にいない
		{
			//チェックしたらフラグfalse
			//pawnJumpMove = false;
			checkPosX = -1;
			checkPosY = fieldY;
			//隣なら
			if (passantPosX == fieldX - 1)
			{
				checkPosX = fieldX - 1;
			}
			else if (passantPosX == fieldX + 1)
			{
				checkPosX = fieldX + 1;
			}
			//配列外チェック
			if (checkOutOfRangeField(
				checkPosX,
				checkPosY,
				out checkPosX,
				out checkPosY ))
			{
				//白側判定
				if (IsWhite( useFieldData[fieldY, fieldX] ) && fieldY == 3 &&
					!IsWhite( useFieldData[checkPosY, checkPosX] ))
				{
					//駒的にアンパッサンできるなら
					if (GetPieceType( checkPosX, checkPosY ) == ChessPieces.PIECE_TYPE.PAWN)
					{
						canPutDummyXList.Add( checkPosX );
						canPutDummyYList.Add( checkPosY - 1 );
						//ポーンアッパッサン用
						passantDummyXList.Add( checkPosX );
						passantDummyYList.Add( checkPosY );
					}
				}
				//黒側判定
				else if (!IsWhite( useFieldData[fieldY, fieldX] ) && fieldY == useFieldData.GetLength( 0 ) - 4 &&
					IsWhite( useFieldData[checkPosY, checkPosX] ))
				{
					//駒的にアンパッサンできるなら
					if (GetPieceType( checkPosX, checkPosY ) == ChessPieces.PIECE_TYPE.PAWN)
					{
						canPutDummyXList.Add( checkPosX );
						canPutDummyYList.Add( checkPosY + 1 );
						//ポーンアッパッサン用
						passantDummyXList.Add( checkPosX );
						passantDummyYList.Add( checkPosY );
					}
				}
			}
		}
		//ポーン初回移動なら
		if (!cmp.IsFirstMoved())
		{
			checkPosX = fieldX;
			//チェックする座標設定
			if (cmp.GetSideType() == ChessPieces.SIDE_TYPE.BLACK)
			{
				checkPosY += 1;
			}
			else
			{
				checkPosY -= 1;
			}
			//配列外チェック
			if (checkOutOfRangeField(
				checkPosX,
				checkPosY,
				out checkPosX,
				out checkPosY ))
			{
				//チェックするフィールド座標に何もない or 敵なら
				if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ))
				{
					canPutDummyXList.Add( checkPosX );
					canPutDummyYList.Add( checkPosY );
				}
			}
		}
	}
	//ナイト置ける場所設定
	void checkPawnCanPut(
		ChessPieces cmp,
		int fieldX,
		int fieldY,
		ref List<int> canPutDummyXList,
		ref List<int> canPutDummyYList )
	{
		//左上（上）チェック
		int checkPosX = fieldX - 1;
		int checkPosY = fieldY - 2;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//右上（上）チェック
		checkPosX = fieldX + 1;
		checkPosY = fieldY - 2;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//左上（左）チェック
		checkPosX = fieldX - 2;
		checkPosY = fieldY - 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//左下（左）チェック
		checkPosX = fieldX - 2;
		checkPosY = fieldY + 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//左下（下）チェック
		checkPosX = fieldX - 1;
		checkPosY = fieldY + 2;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//右下（下）チェック
		checkPosX = fieldX + 1;
		checkPosY = fieldY + 2;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//右下（右）チェック
		checkPosX = fieldX + 2;
		checkPosY = fieldY + 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
		//右上（右）チェック
		checkPosX = fieldX + 2;
		checkPosY = fieldY - 1;
		//配列外チェック
		if (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, cmp.GetSideType() ) ||
				isCanPutEnemy( checkPosX, checkPosY, cmp.GetSideType() ))
			{
				canPutDummyXList.Add( checkPosX );
				canPutDummyYList.Add( checkPosY );
			}
		}
	}
	//ビショップの置ける場所設定
	void checkBishopCanPut(
		ref List<int> xList,
		ref List<int> yList,
		int fieldX,
		int fieldY,
		ChessPieces.SIDE_TYPE sideType,
		bool king = false )
	{
		//左上斜め判定
		int checkPosX = fieldX - 1;
		int checkPosY = fieldY - 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if ( king )
			{
				break;
			}
			checkPosX -= 1;
			checkPosY -= 1;
		}
		//左下斜め判定
		checkPosX = fieldX - 1;
		checkPosY = fieldY + 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosX -= 1;
			checkPosY += 1;
		}
		//右上斜め判定
		checkPosX = fieldX + 1;
		checkPosY = fieldY - 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosX += 1;
			checkPosY -= 1;
		}
		//右下斜め判定
		checkPosX = fieldX + 1;
		checkPosY = fieldY + 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない or 敵なら
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosX += 1;
			checkPosY += 1;
		}
	}
	//ルークの置ける場所設定
	void checkRookCanPut(
		ref List<int> xList,
		ref List<int> yList,
		int fieldX,
		int fieldY,
		ChessPieces.SIDE_TYPE sideType,
		bool king = false )
	{
		//上判定
		int checkPosX = fieldX;
		int checkPosY = fieldY - 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosY -= 1;
		}
		//下判定
		checkPosX = fieldX;
		checkPosY = fieldY + 1;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosY += 1;
		}
		//左判定
		checkPosX = fieldX - 1;
		checkPosY = fieldY;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosX -= 1;
		}
		//右判定
		checkPosX = fieldX + 1;
		checkPosY = fieldY;
		while (checkOutOfRangeField(
			checkPosX,
			checkPosY,
			out checkPosX,
			out checkPosY ))
		{
			//チェックするフィールド座標に何もない
			if (isCanPutEmpty( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
			}
			else if (isCanPutEnemy( checkPosX, checkPosY, sideType ))
			{
				xList.Add( checkPosX );
				yList.Add( checkPosY );
				break;
			}
			else
			{
				break;
			}
			//キングなら１回で終了
			if (king)
			{
				break;
			}
			checkPosX += 1;
		}
	}
	//キングの置ける場所設定
	void checkKingCanPut(
		ChessPieces cmp,
		int fieldX,
		int fieldY,
		ref List<int> canPutDummyXList,
		ref List<int> canPutDummyYList,
		bool dummy,
		List<ChessPieces> blackList,
		List<ChessPieces> whiteList,
		ref List<int> castlingDummyXList,
		ref List<int> castlingDummyYList )
	{
		//初回移動なら
		if (!cmp.IsFirstMoved() && !dummy)  //生成なしチェックのときは”とれるかどうか”の判定だけでで、キャスリングはキングとルークの間に駒あるとできないので、相手を考慮しないため
		{
			//チェックする駒リスト設定
			ChessPieces.SIDE_TYPE sideType = cmp.GetSideType();
			List<ChessPieces> checkList;
			if (sideType == ChessPieces.SIDE_TYPE.BLACK)
			{
				checkList = blackList;
			}
			else
			{
				checkList = whiteList;
			}
			//動かすキングがチェックされてないなら
			bool check = false;
			if (IsCheck( cmp, blackList, whiteList ))
			{
				check = true;
			}
			if (!check)
			{
				//置けるか判定
				for (int i = 0; i < checkList.Count; i++)
				{
					//ルークじゃないならスルー
					if (checkList[i].GetPieceType() != ChessPieces.PIECE_TYPE.ROOK)
					{
						continue;
					}
					//既に移動済ならスルー
					if (checkList[i].IsFirstMoved())
					{
						continue;
					}
					//ルークの座標取得
					int rookX;
					int rookY;
					checkList[i].GetFieldPos( out rookX, out rookY );
					//キングの座標取得
					int kingX;
					int kingY;
					cmp.GetFieldPos( out kingX, out kingY );
					//Y座標が異なるなら
					if (kingY != rookY)
					{
						continue;
					}
					//間に他の駒がないか
					int checkPosX = kingX;
					int checkPosY = kingY;
					if (kingX > rookX)
					{
						//キャスリングの道中がチェックされているか
						check = false;
						if ( IsCheck( blackList, whiteList, sideType, kingX - 1, kingY ) ||
							IsCheck( blackList, whiteList, sideType, kingX - 2, kingY ))
						{
							check = true;
						}
						if ( check )
						{
							continue;
						}
						//間に他の駒がないか
						checkPosX = kingX;
						int sub = kingX - rookX - 1;
						bool allOK = true;
						for (int j = 0; j < sub; j++)
						{
							checkPosX -= 1;
							//駒があるなら
							if (!isCanPutEmpty( checkPosX, checkPosY, sideType ))
							{
								allOK = false;
								break;
							}
						}
						//全てループしたら
						if (allOK)
						{
							canPutDummyXList.Add( kingX - 2 );
							canPutDummyYList.Add( checkPosY );
							castlingDummyXList.Add( rookX );
							castlingDummyYList.Add( rookY );
						}
					}
					else if (kingX < rookX)
					{
						//キャスリングの道中がチェックされているか
						check = false;
						if (IsCheck( blackList, whiteList, sideType, kingX + 1, kingY ) ||
							IsCheck( blackList, whiteList, sideType, kingX + 2, kingY ))
						{
							check = true;
						}
						if (check)
						{
							continue;
						}
						//間に他の駒がないか
						checkPosX = kingX;
						int sub = kingX - rookX + 1;
						bool allOK = true;
						for (int j = 0; j < sub; j++)
						{
							checkPosX += 1;
							//駒があるなら
							if (!isCanPutEmpty( checkPosX, checkPosY, sideType ))
							{
								allOK = false;
								break;
							}
						}
						//全てループしたら
						if (allOK)
						{
							canPutDummyXList.Add( kingX + 2 );
							canPutDummyYList.Add( checkPosY );
							castlingDummyXList.Add( rookX );
							castlingDummyYList.Add( rookY );
						}
					}
				}
			}
		}
	}
}