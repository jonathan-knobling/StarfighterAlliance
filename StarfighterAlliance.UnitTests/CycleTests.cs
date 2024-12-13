using FluentAssertions;
using StarfighterAlliance.Core;

namespace StarfighterAlliance.UnitTests;

public class CycleTests
{
	[Fact]
	public void Add_SingleElement_ShouldSetCurrentToElement()
	{
		// Arrange
		var cycle = new Cycle<int>();

		// Act
		cycle.Add(1);

		// Assert
		cycle.Current.Should().Be(1);
		cycle.Count.Should().Be(1);
	}

	[Fact]
	public void Add_MultipleElements_ShouldMaintainCycleOrder()
	{
		// Arrange
		var cycle = new Cycle<int>();

		// Act
		cycle.Add(1);
		cycle.Add(2);
		cycle.Add(3);

		// Assert
		cycle.Current.Should().Be(1);
		cycle.Next.Should().Be(2);
		cycle.Previous.Should().Be(3);
		cycle.Count.Should().Be(3);
	}

	[Fact]
	public void AddRange_ShouldAddAllElementsInOrder()
	{
		// Arrange
		var cycle = new Cycle<int>();
		var values = new[] { 1, 2, 3, 4, 5 };

		// Act
		cycle.AddRange(values);

		// Assert
		cycle.Count.Should().Be(5);
		cycle.Current.Should().Be(1);
		cycle.Next.Should().Be(2);
		cycle.Previous.Should().Be(5);
	}

	[Fact]
	public void Add_ShouldLinkElementsProperly_WhenOnlyTwoElementsExist()
	{
		// Arrange
		var cycle = new Cycle<int>();

		// Act
		cycle.Add(10);
		cycle.Add(20);

		// Assert
		cycle.Current.Should().Be(10);
		cycle.Previous.Should().Be(20);
		cycle.Next.Should().Be(20);
		cycle.Count.Should().Be(2);
	}

	[Fact]
	public void Current_ShouldReturnDefault_WhenCycleIsEmpty()
	{
		// Arrange
		var cycle = new Cycle<string>();

		// Act
		string? current = cycle.Current;

		// Assert
		current.Should().BeNull();
		cycle.Count.Should().Be(0);
	}

	[Fact]
	public void Next_ShouldReturnDefault_WhenCycleIsEmpty()
	{
		// Arrange
		var cycle = new Cycle<string>();

		// Act
		string? next = cycle.Next;

		// Assert
		next.Should().BeNull();
	}

	[Fact]
	public void Previous_ShouldReturnDefault_WhenCycleIsEmpty()
	{
		// Arrange
		var cycle = new Cycle<string>();

		// Act
		string? previous = cycle.Previous;

		// Assert
		previous.Should().BeNull();
	}

	[Fact]
	public void Add_AfterClearing_ShouldStartNewCycle()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.Add(1);
		cycle.Add(2);

		// Act
		cycle.Clear();
		cycle.Add(3);

		// Assert
		cycle.Current.Should().Be(3);
		cycle.Next.Should().Be(3);
		cycle.Previous.Should().Be(3);
		cycle.Count.Should().Be(1);
	}

	[Fact]
	public void CycleRight_ShouldMoveToNextElement()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3 }); //312

		// Act
		cycle.CycleRight(); //123

		// Assert
		cycle.Current.Should().Be(2);
		cycle.Next.Should().Be(3);
		cycle.Previous.Should().Be(1);
	}

	[Fact]
	public void CycleRight_ShouldMoveToNextElement_WithMoreThanThreeElements()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3, 4 }); // 4123

		// Act
		cycle.CycleRight(); // 1234

		// Assert
		cycle.Current.Should().Be(2);
		cycle.Next.Should().Be(3);
		cycle.Previous.Should().Be(1);
	}

	[Fact]
	public void CycleRight_ShouldMoveToNextElement_WithTwoElements()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2 });

		// Act
		cycle.CycleRight();

		// Assert
		cycle.Current.Should().Be(2);
		cycle.Next.Should().Be(1);
		cycle.Previous.Should().Be(1);
	}

	[Fact]
	public void CycleRight_ShouldMoveRightByTwo_WhenCalledTwice()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3, 4, 5 }); // 512

		// Act
		cycle.CycleRight(); // 123
		cycle.CycleRight(); // 234

		// Assert
		cycle.Current.Should().Be(3);
		cycle.Next.Should().Be(4);
		cycle.Previous.Should().Be(2);
	}

	[Fact]
	public void CycleLeft_ShouldMoveToPreviousElement_WithTwoElements()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2 });

		// Act
		cycle.CycleLeft();

		// Assert
		cycle.Current.Should().Be(2);
		cycle.Next.Should().Be(1);
		cycle.Previous.Should().Be(1);
	}

	[Fact]
	public void CycleLeft_ShouldMoveToPreviousElement()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3 }); //312

		// Act
		cycle.CycleLeft(); //231

		// Assert
		cycle.Current.Should().Be(3);
		cycle.Next.Should().Be(1);
		cycle.Previous.Should().Be(2);
	}

	[Fact]
	public void CycleLeft_ShouldMoveToPreviousElement_WithMoreThanThreeElements()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3, 4 }); // 4123

		// Act
		cycle.CycleLeft(); // 3412

		// Assert
		cycle.Current.Should().Be(4);
		cycle.Next.Should().Be(1);
		cycle.Previous.Should().Be(3);
	}

	[Fact]
	public void CycleLeft_ShouldMoveLeftByTwo_WhenCalledTwice()
	{
		// Arrange
		var cycle = new Cycle<int>();
		cycle.AddRange(new[] { 1, 2, 3, 4, 5 }); // 512

		// Act
		cycle.CycleLeft(); // 451
		cycle.CycleLeft(); // 345

		// Assert
		cycle.Current.Should().Be(4);
		cycle.Next.Should().Be(5);
		cycle.Previous.Should().Be(3);
	}
}