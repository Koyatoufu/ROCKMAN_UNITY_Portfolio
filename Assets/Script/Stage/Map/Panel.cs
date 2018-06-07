using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {

	private Point m_Point;
	
	public string m_szTagName;
	private bool m_bPassable;

	private Texture m_Texture;
	private Texture m_oriTex;
	private E_PENELSTATE m_PenelState;

	public Panel()
	{
		m_bPassable = true;
		m_Texture = null;
		m_oriTex = null;
		m_szTagName = null;
		m_PenelState = E_PENELSTATE.NONE;
		m_bPassable = true;
	}

	#if UNITY_ANDROID
	void FixedUpdate()
	{
		
		if(Input.touchCount>0)
		{
			Vector3 vTmp = new Vector3 (Input.GetTouch (0).position.x, Input.GetTouch (0).position.y);
			Ray ray = CamMgr.GetInst ().GetMainCameraComponent ().ScreenPointToRay (vTmp);
			RaycastHit castHit;
			if(Physics.Raycast(ray,out castHit,Mathf.Infinity))
			{
				if(castHit.transform.position==transform.position)
				{
					if (!StageMgr.GetInst ().GetPlay ())
						return;
					if(Time.timeScale==0.0f)
						return;
					UnitBase PlayerUnit = UnitMgr.GetInst ().GetPlayer ();
					UnitMgr.GetInst ().MoveUnit (PlayerUnit, PlayerUnit.GetCurPanel (), this);
				}
			}
		}
	}
	#endif

	public void SetPenelState(E_PENELSTATE penelState){
		m_PenelState = penelState;
	}
	public E_PENELSTATE GetPenelState(){
		return m_PenelState;
	}

	public void SetPoint(int nX,int nZ){
		m_Point = new Point(nX,nZ);
	}
	public Point GetPoint(){
		return m_Point;
	}

	public void SetTexture(Texture texture){
		m_Texture = texture;
		this.gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex",m_Texture);
	}
	public Texture GetTexture()
	{
		return m_Texture;
	}
	public void SetOriTexture(Texture texture){
		m_oriTex = texture;
	}
	public Texture GetOriTexture(){
		return m_oriTex;
	}

	public void SetPassable(bool bPassable){
		m_bPassable = bPassable;
	}

	public bool GetPassable(){
		return m_bPassable;
	}

	public void SetTagName(string szTagName)
	{
		m_szTagName = szTagName;
		//this.gameObject.tag = m_szTagName;
	}

	public string GetTagName()
	{
		return m_szTagName;
	}

	#if UNITY_EDITOR
	void OnMouseDown()
	{
		
		if (!StageMgr.GetInst ().GetPlay ())
			return;
		if(Time.timeScale==0.0f)
			return;
		UnitBase PlayerUnit = UnitMgr.GetInst ().GetPlayer ();
		UnitMgr.GetInst ().MoveUnit (PlayerUnit, PlayerUnit.GetCurPanel (), this);
		//UnitMgr.GetInst ().MoveUnitPath (PlayerUnit, PlayerUnit.GetCurPanel (), this);
	}
	#endif

	public void SetColor(Color color)
	{
		this.gameObject.GetComponent<Renderer>().material.SetColor("_Color",color);
	}

	public struct Point
	{
		public int nX;
		public int nZ;
		public Point(int nX,int nZ)
		{
			this.nX=nX;
			this.nZ=nZ;
		}
		public static bool operator ==(Point p1,Point p2)
		{
			return(p1.nX == p2.nX && p1.nZ == p2.nZ);
		}
		public static bool operator !=(Point p1,Point p2)
		{
			return(p1.nX != p2.nX && p1.nZ != p2.nZ);
		}
		public override bool Equals (object obj)
		{
			/*Point p1 = (Point)obj;
			if((Point)obj==p1)
			{
				return true;
			}
			return false;*/
			return base.Equals(obj);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
		
}
