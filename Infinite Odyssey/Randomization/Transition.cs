using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public class Transition
{
    [JsonIgnore] public Room Room;

    [JsonProperty(PropertyName = "room")]
    private Guid RoomID => Room.ID;

    [JsonIgnore]
    public TransitionTemplate Template;

    [JsonProperty(PropertyName = "transition")]
    private string TemplateName => Template.Name;

    [JsonProperty(PropertyName = "exitType")]

    private ExitType? m_exitType;

    [JsonIgnore]
    public ExitType ExitType
    {
        get => m_exitType ?? Template.ExitType;
        set => m_exitType = value;
    }

    [JsonIgnore]
    private Room DestinationRoom;

    [JsonIgnore]
    public Transition DestinationTransition;

    [JsonProperty(PropertyName = "destination")]
    private string DestinationName => Template.Name;

    [JsonProperty(PropertyName = "destinationRoom")]
    private Guid DestinationRoomID => DestinationRoom.ID;

    [JsonProperty(PropertyName = "state")]
    public TransitionState State;

    [JsonConstructor]
    private Transition() { }

    public Transition(Room room, TransitionTemplate template)
    {
        Room = room;
        Template = template;
        State = TransitionState.Unbound;
    }

    public void Connect(Transition t2)
    {
        State = t2.State = TransitionState.Open;
        t2.ExitType = ExitType;
        DestinationTransition = t2;
        t2.DestinationTransition = this;
    }
}