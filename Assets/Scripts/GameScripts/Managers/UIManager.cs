using System;
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
    float yellowCardWidth;

    Dictionary<int, VisualElement> unitContainers; 
    Dictionary<int, Dictionary<int, VisualElement>> healthBarMap; // teamId -> unitId -> healthBar
    Dictionary<int, VisualElement> yellowCardContainers;

    VisualElement mainContainer;
    Label message;

    public UIManager(UIDocument document, int maxYellowCards)
    {
        yellowCardContainers = new Dictionary<int, VisualElement>();
        unitContainers = new Dictionary<int, VisualElement>();
        healthBarMap = new Dictionary<int, Dictionary<int, VisualElement>>();

        yellowCardWidth = (100f - (maxYellowCards * 3)) / (maxYellowCards + 1);

        VisualElement root = document.rootVisualElement;
        root.Clear();

        message = new Label("");
        message.AddToClassList("messageLabel");
        root.Add(message);

        mainContainer = new VisualElement();
        mainContainer.AddToClassList("mainContainer");
        root.Add(mainContainer);
    }

    public void SetMessage(string text)
    {
        message.text = text;
    }
    public void AddYellowCard(int teamId, string cardText, bool redCard)
    {
        var yellowCard = new VisualElement();
        yellowCard.Add(new Label(cardText));
        yellowCard.AddToClassList("yellowCard");
        yellowCard.style.width = new StyleLength(new Length(yellowCardWidth, LengthUnit.Percent));
        if (redCard) yellowCard.style.backgroundColor = new StyleColor(Color.red);
        yellowCardContainers[teamId].Add(yellowCard);
    }
    public void RemoveUnitContainer(int teamId, int unitId)
    {
        var unitContainer = healthBarMap[teamId][unitId].parent;
        unitContainers[teamId].Remove(unitContainer);
        healthBarMap[teamId].Remove(unitId);
        unitContainer.RemoveFromHierarchy();
    }
    public void UpdateHealthBar(int teamId, int unitId, float healthPercentage)
    {
        var healthBar = healthBarMap[teamId][unitId];
        healthBar.style.width = new StyleLength(new Length(healthPercentage, LengthUnit.Percent));
        healthBar.style.backgroundColor = healthGradient.Evaluate(healthPercentage / 100f);
    }
    public void CreateUnitElement(int teamId, int unitId, string unitName, Color unitColor)
    {
        var teamContainer = unitContainers[teamId];
        
        var unitElement = new VisualElement();
        unitElement.AddToClassList("unit");
        unitElement.style.backgroundColor = unitColor;

        var nameLabel = new Label(unitName);
        unitElement.Add(nameLabel);

        var healthBar = new VisualElement();
        unitElement.Add(healthBar);
        healthBarMap[teamId][unitId] = healthBar;

        teamContainer.Add(unitElement);
    }
    public void CreateTeamContainer(int teamId)
    {
        var teamContainer = new VisualElement();
        teamContainer.AddToClassList("teamContainer");
        mainContainer.Add(teamContainer);

        var yellowCardContainer = new VisualElement();
        yellowCardContainer.AddToClassList("yellowCardContainer");
        teamContainer.Add(yellowCardContainer);
        yellowCardContainers[teamId] = yellowCardContainer;

        var unitContainer = new VisualElement();
        unitContainer.AddToClassList("unitContainer");
        teamContainer.Add(unitContainer);
        unitContainers[teamId] = unitContainer;
        healthBarMap[teamId] = new Dictionary<int, VisualElement>();
    }
}
