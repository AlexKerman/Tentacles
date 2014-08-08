using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FourTentacles.Annotations;
using OpenTK;

namespace FourTentacles
{
	public interface IDoUndo
	{
		void Undo();
		void Redo();
	}

	static class UndoStack
	{
		public static event EventHandler StackChanged;

		private static readonly Stack<IDoUndo> StackUndo = new Stack<IDoUndo>();
		private static readonly Stack<IDoUndo> StackRedo = new Stack<IDoUndo>();

		public static void AddAction(IDoUndo action)
		{
			StackRedo.Clear();
			StackUndo.Push(action);
			StackChanged.Raise();
		}

		public static bool CanUndo { get { return StackUndo.Any(); }}
		public static bool CanRedo { get { return StackRedo.Any(); }}

		public static void Undo()
		{
			if (!CanUndo) return;
			var lastAction = StackUndo.Pop();
			lastAction.Undo();
			StackRedo.Push(lastAction);
			StackChanged.Raise();
		}

		public static void Redo()
		{
			if(!CanRedo) return;
			var nextAction = StackRedo.Pop();
			nextAction.Redo();
			StackUndo.Push(nextAction);
			StackChanged.Raise();
		}
	}

	class DoUndoMove : IDoUndo
	{
		class BeforeAfter
		{
			public BeforeAfter(Vector3 pos)
			{
				Before = pos;
				After = pos;
			}

			public Vector3 Before;
			public Vector3 After;
		}

		private Dictionary<Node, BeforeAfter> nodePositions = new Dictionary<Node, BeforeAfter>();

		public DoUndoMove(IEnumerable<Node> nodes)
		{
			foreach (var node in nodes)
				nodePositions[node] = new BeforeAfter(node.Pos);
		}

		public void Move(Vector3 delta)
		{
			foreach (var pos in nodePositions.Values)
				pos.After += delta;
			Redo();
		}

		public void Undo()
		{
			foreach (var nodePosition in nodePositions)
				nodePosition.Key.Pos = nodePosition.Value.Before;
		}

		public void Redo()
		{
			foreach (var nodePosition in nodePositions)
				nodePosition.Key.Pos = nodePosition.Value.After;
		}
	}
}
