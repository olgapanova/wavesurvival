using System;
using UnityEngine;

public class InputInfo
{
    public Vector2 MoveDirInput;
    public bool CastSpellInput;
    public bool ChangeSpellPreviousInput;
    public bool ChangeSpellNextInput;

    public event Action CastSpell;
    public void TriggerCastSpell() => CastSpell?.Invoke();

    public event Action ChangeSpellPrevious;
    public void TriggerChangeSpellPrevious() => ChangeSpellPrevious?.Invoke();
    
    public event Action ChangeSpellNext;
    public void TriggerChangeSpellNext() => ChangeSpellNext?.Invoke();
}
