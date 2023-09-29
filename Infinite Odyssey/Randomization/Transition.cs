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

    [JsonIgnore]
    private Room DestinationRoom;

    [JsonIgnore]
    public Transition DestinationTransition;

    [JsonProperty(PropertyName = "destination")]
    private string DestinationName => Template.Name;

    [JsonProperty(PropertyName = "destinationRoom")]
    private Guid DestinationRoomID => DestinationRoom.ID;

    [JsonConstructor]
    private Transition() { }

    public Transition(Room room, TransitionTemplate template)
    {
        Room = room;
        Template = template;
    }
}