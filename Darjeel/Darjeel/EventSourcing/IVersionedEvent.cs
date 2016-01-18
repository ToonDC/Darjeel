﻿using Darjeel.Messaging;

namespace Darjeel.EventSourcing
{
    public interface IVersionedEvent : IEvent
    {
        int Version { get; }
        IEvent Event { get; }
    }
}