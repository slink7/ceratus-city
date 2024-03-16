using System.Collections.Generic;
using Godot;

public partial class AutoTilemap : TileMap
{
	public AStar2D astar2d;

	bool debug = true;

	int MAX_FALL_HEIGHT = 16;
	int MAX_JUMP_HEIGHT = 6;
	Color DEBUG_COLOR = Colors.Blue;

	public override void _Ready()
	{
		astar2d = new AStar2D();
		CreateMap();
	}

	long CreateNode(Vector2I pos)
	{
		if (astar2d.GetPointCount() > 0 && astar2d.GetPointPosition(astar2d.GetClosestPoint(MapToLocal(pos))) == MapToLocal(pos))
			return astar2d.GetClosestPoint(MapToLocal(pos));
		long id = astar2d.GetAvailablePointId();
		astar2d.AddPoint(id, MapToLocal(pos));
		return id;
	}

	bool IsSolid(Vector2I pos)
	{
		return GetCellTileData(ZIndex, pos) != null;
	}

	bool IsSolid(int x, int y)
	{
		return GetCellTileData(ZIndex, new Vector2I(x, y)) != null;
	}

	Vector2I GetLocalTopo(Vector2I pos)
	{
		Vector2I o = Vector2I.Zero;

		if (IsSolid(pos.X, pos.Y - 1))
			return o;

		if (IsSolid(pos.X - 1, pos.Y - 1))
			o.X = 1;
		else if (!IsSolid(pos.X - 1, pos.Y))
			o.X = -1;
		
		if (IsSolid(pos.X + 1, pos.Y - 1))
			o.Y = 1;
		else if (!IsSolid(pos.X + 1, pos.Y))
			o.Y = -1;
		return o;
	}

	Vector2I CastDown(Vector2I from)
	{
		for (int k = 1; k < MAX_FALL_HEIGHT; k++)
			if (IsSolid(from.X, from.Y + k))
				return new Vector2I(from.X, from.Y + k - 1);
		return Vector2I.Zero;
	}

	public override void _Draw()
	{
		// if (!debug)
		// 	return ;

		// long[] ids = astar2d.GetPointIds();
		
		// foreach (long id in ids)
		// {
		// 	long[] ids_to = astar2d.GetPointConnections(id);
		// 	foreach (long to in ids_to)
		// 	{
		// 		if (astar2d.ArePointsConnected(to, id, false))
		// 			DrawLine(astar2d.GetPointPosition(id), astar2d.GetPointPosition(to), DEBUG_COLOR, 3.0f);
		// 		else
		// 		{
		// 			Vector2 d = astar2d.GetPointPosition(to) - astar2d.GetPointPosition(id);
		// 			float theta = Mathf.Atan2(d.Y, d.X);
		// 			DrawDashedLine(astar2d.GetPointPosition(id), astar2d.GetPointPosition(to), DEBUG_COLOR);
		// 			DrawArc((astar2d.GetPointPosition(id) + astar2d.GetPointPosition(to)) / 2.0f, 4.0f, theta - Mathf.Pi / 2.0f, theta + Mathf.Pi / 2.0f, 3, DEBUG_COLOR);
		// 		}
		// 	}
		// 	DrawCircle(astar2d.GetPointPosition(id), 2.0f, DEBUG_COLOR);
		// 	//DrawString(ThemeDB.FallbackFont, astar2d.GetPointPosition(id), ids_to.Length.ToString(), fontSize:8);
		// }
		
	}

	void CreateMap()
	{
		Godot.Collections.Array<Vector2I> cells = GetUsedCells(ZIndex);

		foreach (Vector2I cell in cells)
		{
			Vector2I local_topo = GetLocalTopo(cell);
			if (!IsSolid(cell.X, cell.Y - 1) && IsSolid(cell.X, cell.Y))
			{
				long id = CreateNode(cell + Vector2I.Up);
				if (local_topo.X == -1)
				{
					Vector2I pos = CastDown(new Vector2I(cell.X - 1, cell.Y));
					if (pos != Vector2I.Zero)
					{
						long id2 = CreateNode(new Vector2I(pos.X, cell.Y));
						long id3 = CreateNode(pos);
						astar2d.ConnectPoints(id, id2);
						if (id2 != id3)
							astar2d.ConnectPoints(id2, id3, pos.Y - cell.Y + 1 < MAX_JUMP_HEIGHT);
					}
				}
				if (local_topo.Y == -1)
				{
					Vector2I pos = CastDown(new Vector2I(cell.X + 1, cell.Y));
					if (pos != Vector2I.Zero)
					{
						long id2 = CreateNode(new Vector2I(pos.X, cell.Y));
						long id3 = CreateNode(pos);
						astar2d.ConnectPoints(id, id2);
						if (id2 != id3)
							astar2d.ConnectPoints(id2, id3, pos.Y - cell.Y + 1 < MAX_JUMP_HEIGHT);
					}
				}
			}
		}

		long[] ids = astar2d.GetPointIds();
		foreach (long id in ids)
		{
			long close_right = -1;
			Vector2 pos = astar2d.GetPointPosition(id);
			Vector2I topo = GetLocalTopo(LocalToMap(pos) + Vector2I.Down);
			foreach (long id2 in ids)
			{
				Vector2 pos2 = astar2d.GetPointPosition(id2);
				if (topo.Y == 0 && pos.Y == pos2.Y && pos.X < pos2.X)
					if (close_right < 0 || pos2.X < astar2d.GetPointPosition(close_right).X)
						close_right = id2;
			}
			if (close_right > 0)
				astar2d.ConnectPoints(id, close_right);
		}
	}

	public Vector2[] GetPointPath(Vector2 from, Vector2 to)
	{
		Vector2 SegFrom = astar2d.GetClosestPositionInSegment(from);
		long idFrom = astar2d.GetClosestPoint(SegFrom);

		Vector2 SegTo = astar2d.GetClosestPositionInSegment(to);
		long idTo = astar2d.GetClosestPoint(SegTo);

		Vector2[] a = astar2d.GetPointPath(idFrom, idTo);
		//if (a == null || a.Length < 1)
		List<Vector2> path = new List<Vector2>();
		path.Add(from);
		path.AddRange(a);
		path.Add(to);
		return path.ToArray();
	}
}
