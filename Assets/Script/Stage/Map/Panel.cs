using UnityEngine;
using System.Collections;

public class Panel : Photon.MonoBehaviour {

	private Point m_Point;

    public bool IsRed { get; set; }
    public bool OriRed { get; set; }

	public bool Passable { get; set; }

	private Texture m_Texture = null;
    public Texture Texture {
        get { return m_Texture; }
        set {
            m_Texture = value;
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", m_Texture);
        }
    }
	private Texture m_oriTex = null;
    public Texture OriTexture { get { return m_oriTex; } set { m_oriTex = value; } }
    private Texture m_revTex = null;
    public Texture RevTexture { get { return m_revTex; } set { if (m_revTex != null) return; m_revTex = value; } }
	private E_PENELSTATE m_PenelState = E_PENELSTATE.NONE;
    	
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
					if (!StageMgr.Inst.IsPlay)
						return;
					if(Time.timeScale==0.0f)
						return;
					UnitBase PlayerUnit = UnitMgr.Inst.Player;
					UnitMgr.Inst.MoveUnit (PlayerUnit, PlayerUnit.GetCurPanel (), this);
				}
			}
		}
	}
    #endif
    #if UNITY_EDITOR
    void OnMouseDown()
    {
        if (!StageMgr.Inst.IsPlay)
            return;
        if (Time.timeScale == 0.0f)
            return;

        UnitBase PlayerUnit =UnitMgr.Inst.Player;

        if (PlayerUnit == null)
            return;

       UnitMgr.Inst.MoveUnit(PlayerUnit, PlayerUnit.GetCurPanel(), this);
    }
    #elif UNITY_WINDOWS
     void OnMouseDown()
    {
        if (!StageMgr.Inst.IsPlay)
            return;
        if (Time.timeScale == 0.0f)
            return;
        UnitBase PlayerUnit =UnitMgr.Inst.GetPlayer();
       UnitMgr.Inst.MoveUnit(PlayerUnit, PlayerUnit.GetCurPanel(), this);
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

    public void ReversePanel()
    {
        IsRed = !IsRed;
        //TODO: 색상 변환

        Texture temp = m_oriTex;
        Texture = m_revTex;
        m_oriTex = m_revTex;
        m_revTex = temp;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(IsRed);
            stream.SendNext(m_Texture);
            stream.SendNext(m_oriTex);
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            IsRed = (Panel)stream.ReceiveNext();
            m_Texture = (Texture)stream.ReceiveNext();
            m_oriTex = (Texture)stream.ReceiveNext();
            gameObject.SetActive((GameObject)stream.ReceiveNext());
        }
    }
}
