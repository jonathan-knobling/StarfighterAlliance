namespace StarfighterAlliance.Core;

public class Cycle<T>
{
	private Element? current;
	public int Count { get; private set; }

	/// <summary>
	///     Returns the value of the current element in the cycle or the default value if there is no element
	/// </summary>
	public T? Current
	{
		get
		{
			if (current != null)
			{
				return current.Value;
			}

			return default(T?);
		}
	}

	/// <summary>
	///     Returns the value of the next element in the cycle or the default value if there is no element
	/// </summary>
	public T? Next
	{
		get
		{
			if (current?.Next != null)
			{
				return current.Next.Value;
			}

			return default(T?);
		}
	}

	/// <summary>
	///     Returns the value of the previous element in the cycle or the default value if there is no element
	/// </summary>
	public T? Previous
	{
		get
		{
			if (current?.Previous != null)
			{
				return current.Previous.Value;
			}

			return default(T?);
		}
	}

	/// <summary>
	///     Adds a value to the cycle. The value will be added at the "last" position, which is the previous of the current
	///     element.
	/// </summary>
	/// <param name="value">The value to add</param>
	public void Add(T value)
	{
		var newNode = new Element(value);

		if (current == null)
		{
			newNode.Next = newNode;
			newNode.Previous = newNode;
			current = newNode;
		}
		else
		{
			Element? previous = current.Previous;

			newNode.Previous = previous;
			newNode.Next = current;

			if (previous != null)
			{
				previous.Next = newNode;
			}

			current.Previous = newNode;
		}

		Count++;
	}

	public void AddRange(IEnumerable<T> values)
	{
		foreach (T value in values)
		{
			Add(value);
		}
	}

	/// <summary>
	///     Moves the current position to the next element in the cycle.
	/// </summary>
	public void CycleRight()
	{
		if (current != null)
		{
			current = current.Next;
		}
	}

	/// <summary>
	///     Moves the current position to the previous element in the cycle.
	/// </summary>
	public void CycleLeft()
	{
		if (current != null)
		{
			current = current.Previous;
		}
	}

	public void Clear()
	{
		current = null;
		Count = 0;
	}

	private class Element
	{
		public Element(T value)
		{
			Value = value;
		}

		public T Value { get; }
		public Element? Previous { get; set; }
		public Element? Next { get; set; }
	}
}