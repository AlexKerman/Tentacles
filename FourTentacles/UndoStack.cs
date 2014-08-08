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
		private Vector3 move = Vector3.Zero;

		private Dictionary<Node, Vector3> nodePositions = new Dictionary<Node, Vector3>();

		public DoUndoMove(IEnumerable<Node> nodes)
		{
			foreach (var node in nodes)
				nodePositions[node] = node.Pos;
		}

		public void Move(Vector3 delta)
		{
			move += delta;
			Redo();
		}

		public void Undo()
		{
			foreach (var nodePosition in nodePositions)
				nodePosition.Key.Pos = nodePosition.Value;
		}

		public void Redo()
		{
			foreach (var nodePosition in nodePositions)
				nodePosition.Key.Pos = nodePosition.Value + move;
		}
	}

	class DoUndoWidth : IDoUndo
	{
		private readonly WidthController controller;
		private readonly float oldValue;
		private float newValue;

		public DoUndoWidth(WidthController controller)
		{
			this.controller = controller;
			oldValue = controller.Width;
		}

		public void SetWidth(float value)
		{
			newValue = value;
			Redo();
		}

		public void Undo()
		{
			controller.Width = oldValue;
		}

		public void Redo()
		{
			controller.Width = newValue;
		}
	}
}
