using UnityEngine;
using System.Collections;

public interface IRemovable
{
    void Remove();

    bool RemovalAllowed();
}
