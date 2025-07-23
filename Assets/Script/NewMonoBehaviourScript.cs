using UnityEngine;
using UnityEngine.EventSystems;


public class NewMonoBehaviourScript : ActiveSkill
{

}


public class ActiveSkill : Skill
{

}


public class Character : Skill
{
    protected int characterName;

    private int level;


    public int health;
    public int mana;

    public void Attack()
    {
        Debug.Log(characterName + " attacks!");
    }
}


public class Skill : ScriptableObject
{

}