using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    public void Start();
    public void Update();
    public void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection);
    public void OnRedCard();
}

