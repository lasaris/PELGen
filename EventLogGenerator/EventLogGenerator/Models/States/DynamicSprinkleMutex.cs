using EventLogGenerator.Services;

namespace EventLogGenerator.Models.States;

// FIXME: In the future, this could be somehow generalized for all sprinkles/states
public class DynamicSprinkleMutex
{
    public DynamicSprinkleState FirstState;

    public DynamicSprinkleState SecondState;

    public float FirstChance;

    public DynamicSprinkleMutex(DynamicSprinkleState firstState, DynamicSprinkleState secondState, float firstChance)
    {
        FirstState = firstState;
        SecondState = secondState;
        FirstChance = firstChance;
        
        SprinkleService.LoadDynamicSrpinkleMutex(this);
    }

    public DynamicSprinkleState PickState()
    {
        float randomNumber = (float)RandomService.GetNextDouble();
        return (randomNumber <= FirstChance) ? FirstState : SecondState;
    }
}