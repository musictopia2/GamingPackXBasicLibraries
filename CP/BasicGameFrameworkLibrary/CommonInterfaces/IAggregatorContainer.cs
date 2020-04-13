using CommonBasicStandardLibraries.Messenging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.CommonInterfaces
{
    public interface IAggregatorContainer
    {
        IEventAggregator Aggregator { get; } //this is needed because often times we need to send messages.

    }
}
