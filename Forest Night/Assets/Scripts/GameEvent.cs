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

public class CharacterHitSpineEdge : GameEvent
{
    public GameObject Character;
    public GameObject Spine;
    public CharacterHitSpineEdge(GameObject C, GameObject S)
    {
        Character = C;
        Spine = S;
    }
}

public class FreezeGame : GameEvent
{
    public int CurrentLevel;
    public FreezeGame(int L)
    {
        CurrentLevel = L;
    }
}

public class UnFreezeGame : GameEvent
{
    public int CurrentLevel;
    public UnFreezeGame(int L)
    {
        CurrentLevel = L;
    }
}

public class SaveLevel : GameEvent
{
    public GameObject Memory;
    public SaveLevel(GameObject M)
    {
        Memory = M;
    }
}

public class LoadLevel : GameEvent { }

public class EnterMenu : GameEvent
{
    public GameObject Menu;
    public EnterMenu(GameObject M)
    {
        Menu = M;
    }
}

public class ExitMenu : GameEvent
{
    public GameObject Menu;
    public ExitMenu(GameObject M)
    {
        Menu = M;
    }
}

public class EnterLevel : GameEvent
{
    public int Level;
    public float TimeCount;
    public EnterLevel(int L,float timecount)
    {
        Level = L;
        TimeCount = timecount;
    }
}

public class QuitGame : GameEvent
{
    public bool finish;
    public QuitGame(bool b)
    {
        finish = b;
    }
}


public class ButtonClicked : GameEvent
{
    public GameObject Button;
    public ButtonClicked(GameObject B)
    {
        Button = B;
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

public class GameSceneMenuOpen: GameEvent
{

}

public class GameSceneMenuClose : GameEvent
{

}
