using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RelinkFSMToolkit.Messages;

/// <summary>
/// Represents a message for when a FSM file has been loaded.
/// </summary>
public class FSMFileLoadStateChangedMessage : ValueChangedMessage<bool>
{
    public FSMFileLoadStateChangedMessage(bool value) : base(value)
    {

    }
}
