public interface IUnit
{
    public int UnitId { get; }
    public int TeamId { get; }
    public void OnUnitDeath();
}