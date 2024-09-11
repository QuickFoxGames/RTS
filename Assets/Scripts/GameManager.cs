using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton_template<GameManager>
{
    [SerializeField] private List<Character> characterList;
    [SerializeField] private Character mainCharacter;
    public List<Character> CharacterList { get; private set; }
    public Character MainCharacter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        CharacterList = characterList;
        MainCharacter = mainCharacter;
    }
    void Update()
    {
        
    }
}
