using System.Collections.Generic;
using D2Hunt.App.Data.Models;

namespace D2Hunt.App.Abstraction.Data;

public interface IConnectionsProvider
{
    IAsyncEnumerable<ConnectionInfo> List();
}