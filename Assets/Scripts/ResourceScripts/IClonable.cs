using UnityEngine;
using System.Collections;

public interface IClonable<T>
{
	T Clone();
}
