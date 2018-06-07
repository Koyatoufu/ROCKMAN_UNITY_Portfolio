
public enum E_ACT
{
	IDLE,//0
	ATK,//1
	HIT,//2
	DIE,//3
	COUNTERHIT,//4
	MOVE,//5
	GUARD,//6
	MAX//7
}

public enum E_TYPE
{
	NONE,
	FIRE,
	WATER,
	ELEC,
	LEAF,
	AIR,
	SWORD,
	BREAK,
	CURSOR,
	MAX
}

public enum E_PlAnimState
{
	IDLE,//0
	NormalShot,//1
	RecoilShot,//2
	Swing,//3
	Throw,//4
	Punch,//5
	Pick,//6
	Hit,//7
	Freeze,//8
	Cross,//9
	FallDown,//10
	MAX//11
}

public enum E_PENELSTATE
{
	NONE,
	NORAML,
	CRACK,
	BROKEN,
	END
}
	
public enum E_CHIPLABEL
{
	E_STANDARD,
	E_MEGA,
	E_GIGA,
	E_MAX
}

public enum E_CHIPTYPE
{
	WEAPON,//0
	ATKEFFECT,//1
	SUMMON,//2
	AREA,//3
	SUPPORT,//4
	ATTACH,//5
	RECOVERY,//6
	NONE,
	MAX
}

public enum E_EFFECTID
{
	SHIELD,
	LOCKON,
	EXPLOSION,
	IMPACTEXPLOSION,
	POOF,
	DEATH,
	MAX
}

public enum E_ATKELEMENT
{
	NONE,
	BULLET,
	CHARGEBULLET,
	WAVE,
	MAX
}