using UnityEngine;

public class InputSystem : ITickable
{
    private InputInfo _inputInfo;

    public InputSystem(InputInfo inputInfo) => _inputInfo = inputInfo;

    private void UpdateInputs()
    {
        _inputInfo.MoveDirInput.x = Input.GetAxis("Horizontal");
        _inputInfo.MoveDirInput.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Q))
            _inputInfo.TriggerChangeSpellPrevious();
        
        if (Input.GetKeyDown(KeyCode.W))
            _inputInfo.TriggerChangeSpellNext();
        
        if (Input.GetKeyDown(KeyCode.X))
            _inputInfo.TriggerCastSpell();
    }
    
    public void Update() => UpdateInputs();
}
