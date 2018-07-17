#if UNITY_DEBUG
	#define DEBUG_MODE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMng : MonoBehaviour
{
	//********************************************************************************
	//valiables
	//********************************************************************************
	//----------CONST----------
	//----------CLASS----------
	//----------PUBLIC----------
	//----------PRIVATE----------
	//フラグ
	public bool onCollisionEnter = false;
	//Rigidbody
	Rigidbody rigid = null;
	//フォース移動
	Vector3 forcePower = Vector3.zero;
	ForceMode forceMode = ForceMode.Force;
	//トルク移動
	Vector3 torquePower = Vector3.zero;
	ForceMode torqueForceMode = ForceMode.Force;
	//********************************************************************************
	//public static function
	//********************************************************************************
	//----------MAKE----------
	//生成
#if skip //2018.4.2 こっちいらない気がしたけど、弾の挙動に関わるので削除はしない
	public static RigidMng MakeRigidMng_AddCmp( GameObject addCmp )
	{
		RigidMng cmp = addCmp.AddComponent<RigidMng>();
		return cmp;
	}
#endif
	public static RigidMng MakeRigidMng_AddRigid( GameObject obj ){
		RigidMng cmp = obj.AddComponent<RigidMng>();
		cmp.rigid = obj.AddComponent<Rigidbody>();
		return cmp;
	}
	//----------RELEASE----------
	//開放
	public static void Release( RigidMng cmp ){
		if (cmp == null)
		{
			return;
		}
		cmp.Release();
	}
	//----------IS----------
	//onCollisionEnterが反応したか
	public static bool IsCollisionEnter( GameObject obj ){
		//nullなら失敗
		if ( obj == null ){
			return false;
		}
		RigidMng cmp = obj.GetComponent<RigidMng>();
		if (cmp == null)
		{
			return false;
		}
		bool flag = cmp.onCollisionEnter;
		cmp.onCollisionEnter = false;
		return flag;
	}
	public static bool IsCollisionEnter(RigidMng cmp)
	{
		//nullなら失敗
		if (cmp == null)
		{
			return false;
		}
		bool flag = cmp.onCollisionEnter;
		cmp.onCollisionEnter = false;
		return flag;
	}
	//----------SET----------
	//フォース移動
	public static void SetForceMove(
		RigidMng cmp,
		Vector3 power,
		ForceMode mode)
	{
		//nullなら失敗
		if (cmp == null)
		{
			return;
		}
		cmp.SetForceMove(power, mode);
	}
	//トルク移動
	public static void SetTorqueMove(
		RigidMng cmp,
		Vector3 power,
		ForceMode mode )
	{
		//nullなら失敗
		if (cmp == null){
			return;
		}
		cmp.SetMoveTorque( power, mode );
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
		Destroy(this);
	}
	//----------IS----------
	//----------SET----------
	//フォース移動
	public void SetForceMove(
		Vector3 power,
		ForceMode mode ){
		forcePower = power;
		forceMode = mode;
	}
	//トルク移動
	public void SetMoveTorque(
		Vector3 power,
		ForceMode forceMode ){
		torquePower = power;
		torqueForceMode = forceMode;
	}
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
	}
	//ずっと通る
	void Update ()
	{
#if DEBUG_MODE
		//DebugMng.ScreenLog("RigidMng() procNo: " + procNo);
#endif
	}
	void FixedUpdate()
	{
		//フォース移動
		rigid.AddForce( forcePower, forceMode );
		forcePower = Vector3.zero;
		//トルク移動
		rigid.AddTorque(torquePower, torqueForceMode);
		torquePower = Vector3.zero;
	}
	//物理判定したら１度だけ通る
	void OnCollisionEnter(Collision collision)
	{
#if DEBUG_MODE
		//Debug.Log("RigidMng() Hit!! " + collision.gameObject.name);
#endif
		onCollisionEnter = true;
	}
	//----------MAKE----------
	//----------RELEASE----------
	//----------IS----------
	//----------SET----------
	//----------GET----------
	//----------OTHER----------
}