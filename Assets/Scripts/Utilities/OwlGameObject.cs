using UnityEngine;

public static class OwlGameObject
{
	/// <summary>
	/// Returns the object itself if it exists, null otherwise.
	/// </summary>
	/// <typeparam name="T">The type of the object.</typeparam>
	/// <param name="obj">The object being checked.</param>
	/// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
	public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;
}