using UnityEngine;
using System.Collections;

public interface ITickable
{
	void Tick (float delta);
}

public class TickablePart<T> : ITickable 
{
    public void Init(T parent) { }
    public void Tick(float delta) { }
}
