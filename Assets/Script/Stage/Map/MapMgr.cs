using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MapMgr
{

	private struct DIRPOINT
	{
		public int nX;
		public int nZ;
		public DIRPOINT(int nX,int nZ)
		{
			this.nX=nX;
			this.nZ=nZ;
		}
	}

	private static MapMgr m_Inst=null;
    public static MapMgr Inst { get { return m_Inst; } }

	private int m_nSizeX;
	private int m_nSizeZ;

	private GameObject m_goPanel;

	private float m_fPanelHeight;
	private float m_fPanelWidth;

	private Panel[][] m_arMap;

	private DIRPOINT[] m_dir;

	private List<C_Path> m_openPaths;
	private List<C_Path> m_closePaths;

    public bool IsCreated { get; private set; }

	private MapMgr()
	{
		m_nSizeX = 0;
		m_nSizeZ = 0;
		m_arMap = null;
		m_goPanel = null;
		m_fPanelHeight = 0.0f;
		m_fPanelWidth = 0.0f;
	}

	public static void InitInst(){
		if(m_Inst==null)
		{
			m_Inst=new MapMgr();
		}
	}

	public void Initialize()
	{
		m_goPanel = (GameObject)Resources.Load ("Prefebs/Map/Panel/Panel");
		SetSize ();
		InitDir ();
	}
	public void CreateMap()
	{
		m_arMap=new Panel[m_nSizeX][];
		GameObject gMap = new GameObject ("Map");
		for(int x = -m_nSizeX/2;x<m_nSizeX/2;x++){
			int nXtemp=x+m_nSizeX/2;
			m_arMap[nXtemp]=new Panel[m_nSizeZ];
			int nZRound=(int)(Mathf.Round((float)m_nSizeZ/2));
			for(int z=-m_nSizeZ/2;z<nZRound;z++){
				int nZTemp=z+m_nSizeZ/2;
				GameObject gTempPlat=GameObject.Instantiate(m_goPanel);
				gTempPlat.transform.parent=gMap.transform;
				m_arMap[nXtemp][nZTemp]=gTempPlat.GetComponent<Panel>();
				m_arMap[nXtemp][nZTemp].transform.position=GetWorldPos(x,z);
				m_arMap[nXtemp][nZTemp].SetPoint(x,z);
                bool isRed = false;
				Texture texTmp=(Texture)Resources.Load("Texture/PanelTexture/PanelBase_B");
                Texture texRev = (Texture)Resources.Load("Texture/PanelTexture/PanelBase_R");
                if (x>=0){
                    Texture texComp = texTmp;
                    texTmp = texRev;
                    texRev = texComp;
					isRed=true;
				}
				m_arMap[nXtemp][nZTemp].Texture = texTmp;
				m_arMap[nXtemp][nZTemp].OriTexture = texTmp;
                m_arMap[nXtemp][nZTemp].RevTexture = texRev;
				m_arMap[nXtemp][nZTemp].IsRed=isRed;
                m_arMap[nXtemp][nZTemp].OriRed = isRed;
                m_arMap[nXtemp][nZTemp].Passable = true;

            }
		}

        IsCreated = true;
	}

	public Panel GetMapPanel(int x,int z){

		if(x+(m_nSizeX/2)>=m_nSizeX)
		{
			return m_arMap [m_nSizeX- 1] [z + (m_nSizeZ / 2)];
		}
		else if(x+(m_nSizeX/2)<0)
		{
			return m_arMap[0][z+(m_nSizeZ/2)];
		}

		return m_arMap[x+(m_nSizeX/2)][z+(m_nSizeZ/2)];
	}

	public Panel GetMaxSidePanel()
	{
		return m_arMap [m_nSizeX - 1] [1];
	}
	public Panel GetMinSidePanel()
	{
		return m_arMap [0] [1];
	}

	public int GetSizeX()
	{
		return m_nSizeX;
	}
	public int GetSizeZ()
	{
		return m_nSizeZ;
	}

	private Vector3 GetWorldPos(float fX,float fZ){
		float fTempX,fTempZ;
		fTempX = fX * m_fPanelWidth;
		fTempZ = fZ * m_fPanelHeight;
		return new Vector3(fTempX,0,fTempZ);
	}

	private void SetSize()
	{
		Renderer renderer=m_goPanel.GetComponent<Renderer>();
		m_fPanelHeight = renderer.bounds.size.z;
		m_fPanelWidth = renderer.bounds.size.x;
		m_nSizeX = 6;
		m_nSizeZ = 3;
	}
		
	private void InitDir(){
		m_dir=new DIRPOINT[4];
		m_dir[0]=new DIRPOINT(1,0);
		m_dir[1]=new DIRPOINT(-1,0);
		m_dir[2]=new DIRPOINT(0,1);
		m_dir[3]=new DIRPOINT(0,-1);
	}

	public int GetDistance(Panel.Point point1,Panel.Point point2){
		return (Mathf.Abs (point1.nX-point2.nX) + Mathf.Abs (point1.nZ-point2.nZ));
	}
	
	private C_Path Recursive_find(C_Path ptParent,Panel plDest){

		if (ptParent.m_curPanel.GetPoint () == plDest.GetPoint ()) {
			return ptParent;
		}

		List<Panel> neighborList = GetNeighbor (ptParent.m_curPanel);
		for (int i = 0; i < neighborList.Count; i++) {
			C_Path tmpPath = new C_Path (ptParent,neighborList[i],(ptParent.m_fBDistance+1),GetDistance(neighborList[i].GetPoint(),plDest.GetPoint()));
			AddtoOpenList (tmpPath);
		}
		
		C_Path cBestPath=null;

		if (m_openPaths.Count == 0) {
			return null;
		}

		cBestPath = m_openPaths [0];

		for(int i=0;i<m_openPaths.Count;i++)
		{
			if(m_openPaths[i].m_fSum<cBestPath.m_fSum)
			{
				cBestPath = m_openPaths [i];
			}
		}
		
		m_openPaths.Remove (cBestPath);
		m_closePaths.Add (cBestPath);

		return Recursive_find (cBestPath,plDest);
	}
	
	private void AddtoOpenList(C_Path cPath){
		for(int i=0;i<m_closePaths.Count;i++){
			if(cPath.m_curPanel==m_closePaths[i].m_curPanel)
				return;
		}
		for(int i=0;i<m_openPaths.Count;i++)
		{
			if(cPath.m_curPanel==m_openPaths[i].m_curPanel)
			{
				if(cPath.m_fSum<m_openPaths[i].m_fSum)
				{
					m_openPaths.Remove (m_openPaths[i]);
					m_openPaths.Add (cPath);
					return;
				}
			}
		}

		m_openPaths.Add (cPath);
	}
	public List<Panel> GetPathPanel(Panel panelStart,Panel panelDest){

		if(m_openPaths!=null&&m_closePaths!=null)
		{
			m_openPaths = null;
			m_closePaths = null;
			System.GC.Collect();
		}
		m_openPaths = new List<C_Path> ();
		m_closePaths = new List<C_Path> ();

		List<Panel> rtnList = new List<Panel> ();

		int nDestDistance = GetDistance (panelStart.GetPoint(),panelDest.GetPoint());
		C_Path cPathTmp = new C_Path (null, panelStart, 0, nDestDistance);

		m_closePaths.Add (cPathTmp);

		C_Path cResult = Recursive_find (cPathTmp,panelDest);
		if (cResult == null) {
			Debug.Log ("Result null");
			return null;
		}
			
		while(cResult.m_parentPath!=null){
			rtnList.Insert(0,cResult.m_curPanel);
			cResult=cResult.m_parentPath;
		}
		return rtnList;
	}
	
	private List<Panel> GetNeighbor(Panel panel){
		List<Panel> rtnList = new List<Panel> ();

		if (panel.Passable==false) {
			return rtnList;
		}

		rtnList.Capacity = m_dir.Length;
		for (int i=0; i<m_dir.Length; i++) {
			DIRPOINT dirTmp=new DIRPOINT(m_dir[i].nX+panel.GetPoint().nX,
				m_dir[i].nZ+panel.GetPoint().nZ);
			if(dirTmp.nX>=-m_nSizeX/2&&dirTmp.nX<=m_nSizeX/2&&
				dirTmp.nZ>=-m_nSizeZ/2&&dirTmp.nZ<=m_nSizeZ/2)
				rtnList.Add(GetMapPanel(dirTmp.nX,dirTmp.nZ));
		}

		return rtnList;
	}
	
	public bool IsReachable(Panel panelStart,Panel panelDest){
		List<Panel> pathsTmp = GetPathPanel(panelStart,panelDest);
		if (pathsTmp == null) {
			Debug.Log ("Path null");
			return false;
		}
		if (pathsTmp.Count == 0) {
			return false;
		}
		return true;
	}
	public void resetMap(){
		for(int i=0;i<m_arMap.Length;i++){
			for(int j=0;i<m_arMap[i].Length;j++){
				m_arMap[i][j].GetComponent<Renderer>().material.SetTexture("_MainTex",m_arMap[i][j].OriTexture);
				m_arMap[i][j].IsRed=m_arMap[i][j].OriRed;
			}
		}
	}

	private class C_Path
	{
		public C_Path m_parentPath=null;
		public Panel m_curPanel;
		public float m_fSum,m_fBDistance,m_fEDistance;
		public C_Path(C_Path parentPath,Panel curPanel,float fBDistance,float fEDistance){
			m_parentPath=parentPath;
			m_curPanel=curPanel;
			m_fBDistance=fBDistance;
			m_fEDistance=fEDistance;
			m_fSum=m_fBDistance+m_fEDistance;	
		}
		public C_Path(){
			m_parentPath=null;
			m_curPanel=null;
			m_fBDistance=0.0f;
			m_fEDistance=0.0f;
		}
	}
}
