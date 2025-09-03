using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager
{
    Gradient healthGradient = new Gradient
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.green, 1f),
            new GradientColorKey(Color.yellow, 0.5f),
            new GradientColorKey(Color.red, 0f)
        },
        alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        }
    };

    Dictionary<int, VisualElement> teamContainers; 
    Dictionary<int, Dictionary<int, VisualElement>> healthBarMap; // teamId -> unitId -> healthBar

    VisualElement mainContainer;

    public UIManager(UIDocument document)
    {
        teamContainers = new Dictionary<int, VisualElement>();
        healthBarMap = new Dictionary<int, Dictionary<int, VisualElement>>();

        VisualElement root = document.rootVisualElement;

        mainContainer = new VisualElement();
        mainContainer.AddToClassList("mainContainer");
        root.Add(mainContainer);
    }

    public void RemoveUnitContainer(int teamId, int unitId)
    {
        var unitContainer = healthBarMap[teamId][unitId].parent;
        teamContainers[teamId].Remove(unitContainer);
        healthBarMap[teamId].Remove(unitId);
        unitContainer.RemoveFromHierarchy();
    }
    public void UpdateHealthBar(int teamId, int unitId, float healthPercentage)
    {
        var healthBar = healthBarMap[teamId][unitId];
        healthBar.style.width = new StyleLength(new Length(healthPercentage, LengthUnit.Percent));
        healthBar.style.backgroundColor = healthGradient.Evaluate(healthPercentage / 100f);
    }
    public void CreateUnitContainer(int teamId, int unitId, string unitName, Color unitColor)
    {
        var teamContainer = teamContainers[teamId];
        
        var unitContainer = new VisualElement();
        unitContainer.AddToClassList("unitContainer");
        unitContainer.style.backgroundColor = unitColor;

        var nameLabel = new Label(unitName);
        unitContainer.Add(nameLabel);

        var healthBar = new VisualElement();
        unitContainer.Add(healthBar);
        healthBarMap[teamId][unitId] = healthBar;

        teamContainer.Add(unitContainer);
    }
    public void CreateTeamContainer(int teamId)
    {
        var teamContainer = new VisualElement();
        teamContainer.AddToClassList("teamContainer");
        mainContainer.Add(teamContainer);

        teamContainers[teamId] = teamContainer;
        healthBarMap[teamId] = new Dictionary<int, VisualElement>();
    }
}
