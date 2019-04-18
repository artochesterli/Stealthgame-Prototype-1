﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent { }

public class CharacterDied : GameEvent
{
    public GameObject DeadCharacter;
    public CharacterDied(GameObject C)
    {
        DeadCharacter = C;
    }
}

public class CharacterMoveWithPlatform : GameEvent
{
    public int FrameCount;
    public GameObject Object;
    public CharacterMoveWithPlatform(int C, GameObject ob)
    {
        FrameCount = C;
        Object = ob;
    }
}

public class CharacterHitSpineEdge : GameEvent
{
    public GameObject Character;
    public CharacterHitSpineEdge(GameObject C)
    {
        Character = C;
    }
}

public class SaveLevel : GameEvent
{

}


public class LoadLevel : GameEvent
{

}

public class MemoryActivate : GameEvent
{

}

public class FinishClick : GameEvent
{
    public ButtonType type;
    public FinishClick(ButtonType t)
    {
        type = t;
    }
}

public class TutorialOpen : GameEvent
{
    public GameObject Tutorial;
    public TutorialOpen(GameObject T)
    {
        Tutorial = T;
    }
}

public class TutorialClose : GameEvent
{
    public GameObject Tutorial;
    public TutorialClose(GameObject T)
    {
        Tutorial = T;
    }
}

public class MenuOpen : GameEvent
{

}

public class MenuClose : GameEvent
{

}
