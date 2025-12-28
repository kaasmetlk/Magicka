using System;
using System.Collections.Generic;
using JigLibX.Collision;
using JigLibX.Geometry;
using Magicka.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Magicka.PathFinding
{
	// Token: 0x02000099 RID: 153
	internal class NavMesh
	{
		// Token: 0x0600047F RID: 1151 RVA: 0x000166AC File Offset: 0x000148AC
		public NavMesh(LevelModel iLevel, ContentReader iInput)
		{
			this.mLevel = iLevel;
			this.mVertices = new Vector3[(int)iInput.ReadUInt16()];
			for (int i = 0; i < this.mVertices.Length; i++)
			{
				this.mVertices[i] = iInput.ReadVector3();
			}
			this.mTriangles = new Triangle[(int)iInput.ReadUInt16()];
			for (int j = 0; j < this.mTriangles.Length; j++)
			{
				this.mTriangles[j] = new Triangle(iInput);
			}
			TriangleVertexIndices[] array = new TriangleVertexIndices[this.mTriangles.Length];
			MovementProperties[] array2 = new MovementProperties[this.mTriangles.Length];
			for (int k = 0; k < this.mTriangles.Length; k++)
			{
				array[k].I0 = (int)this.mTriangles[k].VertexA;
				array[k].I1 = (int)this.mTriangles[k].VertexB;
				array[k].I2 = (int)this.mTriangles[k].VertexC;
				if ((this.mTriangles[k].Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic)
				{
					this.mDynamicNeighbours.Add((ushort)k, new List<NeighbourInfo>(10));
				}
				array2[k] = this.mTriangles[k].Properties;
			}
			this.mMesh = new NavMeshOctree(this.mVertices, array, array2);
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001688C File Offset: 0x00014A8C
		public float GetNearestPosition(ref Vector3 iPoint, out Vector3 oPoint, MovementProperties iProperties)
		{
			ushort num;
			ushort num2;
			return this.GetNearestPosition(ref iPoint, out oPoint, iProperties, out num, out num2);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x000168A8 File Offset: 0x00014AA8
		public float GetNearestPosition(ref Vector3 iPoint, out Vector3 oPoint, MovementProperties iProperties, out ushort oTriangle, out ushort oMesh)
		{
			oMesh = ushort.MaxValue;
			int num2;
			float num = this.mMesh.FindClosestPoint(ref iPoint, out oPoint, out num2, iProperties);
			for (int i = 0; i < this.mAnimatedParts.Count; i++)
			{
				Vector3 vector;
				Vector3.Transform(ref iPoint, ref this.mAnimatedParts[i].mInvTransform, out vector);
				Vector3 vector2;
				int num4;
				float num3 = this.mAnimatedParts[i].Mesh.FindClosestPoint(ref vector, out vector2, out num4, iProperties, num);
				if (num3 < num)
				{
					num = num3;
					Vector3.Transform(ref vector2, ref this.mAnimatedParts[i].mTransform, out oPoint);
					num2 = num4;
					oMesh = (ushort)i;
				}
			}
			oTriangle = (ushort)num2;
			return num;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001694C File Offset: 0x00014B4C
		public bool FindShortestPath(ref Vector3 iStartPos, ref Vector3 iEndPos, List<PathNode> iPath, MovementProperties iMoveAbilities)
		{
			bool result = this.FindPath(ref iStartPos, ref iEndPos, this.mTrianglePath, iMoveAbilities);
			this.StraightenPath(ref iStartPos, ref iEndPos, this.mTrianglePath, iPath, iMoveAbilities);
			return result;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001697C File Offset: 0x00014B7C
		public void UpdateNeighbours()
		{
			foreach (List<NeighbourInfo> list in this.mDynamicNeighbours.Values)
			{
				list.Clear();
			}
			for (int i = 0; i < this.mAnimatedParts.Count; i++)
			{
				this.mAnimatedParts[i].ClearNeighbours();
			}
			Vector3 vector = default(Vector3);
			vector.X = (vector.Y = (vector.Z = 0.25f));
			foreach (KeyValuePair<ushort, List<NeighbourInfo>> keyValuePair in this.mDynamicNeighbours)
			{
				Vector3 vector2 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexA];
				Vector3 vector3 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexB];
				Vector3 vector4 = this.mVertices[(int)this.mTriangles[(int)keyValuePair.Key].VertexC];
				for (int j = 0; j < this.mAnimatedParts.Count; j++)
				{
					AnimatedNavMesh animatedNavMesh = this.mAnimatedParts[j];
					Vector3 vector5;
					Vector3.Transform(ref vector2, ref animatedNavMesh.mInvTransform, out vector5);
					Vector3 vector6;
					Vector3.Transform(ref vector3, ref animatedNavMesh.mInvTransform, out vector6);
					Vector3 vector7;
					Vector3.Transform(ref vector4, ref animatedNavMesh.mInvTransform, out vector7);
					this.FindNeighbours(keyValuePair.Key, ref vector5, ref vector6, ref vector7, keyValuePair.Value, animatedNavMesh, (ushort)j);
				}
			}
			for (int k = 0; k < this.mAnimatedParts.Count; k++)
			{
				this.mAnimatedParts[k].FindNeighbours((ushort)k, this.mAnimatedParts);
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00016BAC File Offset: 0x00014DAC
		private unsafe void FindNeighbours(ushort iTri, ref Vector3 iA, ref Vector3 iB, ref Vector3 iC, List<NeighbourInfo> iNeighbours, AnimatedNavMesh iMesh, ushort iMeshID)
		{
			BoundingBox boundingBox;
			Vector3.Max(ref iA, ref iB, out boundingBox.Max);
			Vector3.Max(ref iC, ref boundingBox.Max, out boundingBox.Max);
			Vector3.Add(ref boundingBox.Max, ref NavMesh.BBBias, out boundingBox.Max);
			Vector3.Min(ref iA, ref iB, out boundingBox.Min);
			Vector3.Min(ref iC, ref boundingBox.Min, out boundingBox.Min);
			Vector3.Subtract(ref boundingBox.Min, ref NavMesh.BBBias, out boundingBox.Min);
			Triangle triangle = new Triangle(ref iA, ref iB, ref iC);
			int[] array = DetectFunctor.IntStackAlloc();
			int dynamicTrianglesIntersectingtAABox;
			fixed (int* ptr = array)
			{
				dynamicTrianglesIntersectingtAABox = iMesh.Mesh.GetDynamicTrianglesIntersectingtAABox(ptr, 2048, ref boundingBox);
			}
			Vector3 vector;
			Vector3.Add(ref iA, ref iB, out vector);
			Vector3.Add(ref iC, ref vector, out vector);
			Vector3.Multiply(ref vector, 0.33333334f, out vector);
			for (int i = 0; i < dynamicTrianglesIntersectingtAABox; i++)
			{
				int iVertex;
				int iVertex2;
				int iVertex3;
				iMesh.Mesh.GetTriangle(array[i]).GetVertexIndices(out iVertex, out iVertex2, out iVertex3);
				Vector3 vector2;
				iMesh.Mesh.GetVertex(iVertex, out vector2);
				Vector3 vector3;
				iMesh.Mesh.GetVertex(iVertex2, out vector3);
				Vector3 vector4;
				iMesh.Mesh.GetVertex(iVertex3, out vector4);
				Triangle triangle2 = new Triangle(ref vector2, ref vector3, ref vector4);
				Vector3 vector5;
				Vector3.Add(ref vector2, ref vector3, out vector5);
				Vector3.Add(ref vector4, ref vector5, out vector5);
				Vector3.Multiply(ref vector5, 0.33333334f, out vector5);
				float num = Distance.TriangleTriangleDistanceSq(ref triangle2, ref triangle);
				if (num <= 0.0625f)
				{
					NeighbourInfo item;
					item.Mesh = iMeshID;
					item.Triangle = (ushort)array[i];
					Vector3.Distance(ref vector, ref vector5, out item.Cost);
					Vector3.Add(ref vector2, ref vector3, out item.EdgePos);
					Vector3.Add(ref vector4, ref item.EdgePos, out item.EdgePos);
					Vector3.Multiply(ref item.EdgePos, 0.33333334f, out item.EdgePos);
					Vector3.Transform(ref item.EdgePos, ref iMesh.mTransform, out item.EdgePos);
					iNeighbours.Add(item);
					item.Triangle = iTri;
					item.Mesh = ushort.MaxValue;
					iMesh.DynamicNeighbours[(ushort)array[i]].Add(item);
				}
			}
			DetectFunctor.FreeStackAlloc(array);
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00016E08 File Offset: 0x00015008
		public unsafe bool FindPath(ref Vector3 iStartPos, ref Vector3 iEndPos, List<uint> iPath, MovementProperties iMoveAbilities)
		{
			if (float.IsNaN(iStartPos.X + iStartPos.Y + iStartPos.Z))
			{
				return false;
			}
			bool flag = false;
			int num = 0;
			while (num < this.mAnimatedParts.Count & !flag)
			{
				flag = this.mAnimatedParts[num].Dirty;
				num++;
			}
			this.UpdateNeighbours();
			iMoveAbilities |= MovementProperties.Dynamic;
			iMoveAbilities |= MovementProperties.Water;
			this.mClosedList.Clear();
			this.mCostList.Clear();
			this.mOpenList.Clear();
			iPath.Clear();
			Vector3 vector;
			ushort id;
			ushort num2;
			this.GetNearestPosition(ref iStartPos, out vector, iMoveAbilities, out id, out num2);
			ushort maxValue = ushort.MaxValue;
			Vector3 vector2;
			ushort num3;
			this.GetNearestPosition(ref iEndPos, out vector2, iMoveAbilities, out num3, out maxValue);
			TriangleNode triangleNode;
			triangleNode.Cost = 0f;
			triangleNode.TotalCost = this.ManhattanDistance(ref vector, ref vector2);
			triangleNode.Id = id;
			triangleNode.Mesh = num2;
			triangleNode.ParentId = (uint)((int)num2 << 16 | 65535);
			this.mOpenList.Push(triangleNode);
			float totalCost = triangleNode.TotalCost;
			TriangleNode triangleNode2 = triangleNode;
			bool result = false;
			while (!this.mOpenList.IsEmpty)
			{
				TriangleNode triangleNode3 = this.mOpenList.Pop();
				if (!this.mClosedList.ContainsKey(triangleNode3.LongId))
				{
					this.mClosedList.Add(triangleNode3.LongId, triangleNode3);
					if (triangleNode3.Id == num3 & triangleNode3.Mesh == maxValue)
					{
						triangleNode2 = triangleNode3;
						result = true;
						break;
					}
					Triangle[] triangles;
					Vector3[] vertices;
					if (triangleNode3.Mesh == 65535)
					{
						triangles = this.mTriangles;
						vertices = this.mVertices;
					}
					else
					{
						triangles = this.mAnimatedParts[(int)triangleNode3.Mesh].Triangles;
						vertices = this.mAnimatedParts[(int)triangleNode3.Mesh].Vertices;
					}
					Triangle triangle = triangles[(int)triangleNode3.Id];
					ushort* ptr = &triangle.NeighbourA;
					for (byte b = 0; b < 3; b += 1)
					{
						ushort num4 = ptr[b];
						uint key = (uint)((int)triangleNode3.Mesh << 16 | (int)num4);
						if (!(num4 == 65535 | this.mClosedList.ContainsKey(key)))
						{
							MovementProperties properties = triangles[(int)num4].Properties;
							if (!(properties != MovementProperties.Default & (properties & iMoveAbilities) == MovementProperties.Default))
							{
								if ((properties & MovementProperties.Water) == MovementProperties.Water)
								{
									Segment segment = default(Segment);
									triangles[(int)num4].GetCenter(vertices, out segment.Origin);
									segment.Origin.Y = segment.Origin.Y + 1f;
									segment.Delta.Y = -4f;
									bool flag2 = false;
									Liquid[] waters = this.mLevel.Waters;
									for (int i = 0; i < waters.Length; i++)
									{
										float num5;
										Vector3 vector3;
										Vector3 vector4;
										if (waters[i].SegmentIntersect(out num5, out vector3, out vector4, ref segment, false, true, false))
										{
											flag2 = true;
											break;
										}
									}
									if (!flag2)
									{
										goto IL_463;
									}
								}
								Vector3 vector5;
								Vector3.Add(ref vertices[(int)(&triangle.VertexA)[b]], ref vertices[(int)(&triangle.VertexA)[(b + 1) % 3]], out vector5);
								Vector3.Multiply(ref vector5, 0.5f, out vector5);
								if (triangleNode3.Mesh != 65535)
								{
									Vector3.Transform(ref vector5, ref this.mAnimatedParts[(int)triangleNode3.Mesh].mTransform, out vector5);
								}
								float num6;
								if ((triangleNode3.ParentId & 65535U) == 65535U)
								{
									Vector3.Distance(ref vector, ref vector5, out num6);
								}
								else if ((ushort)(triangleNode3.ParentId >> 16) == triangleNode3.Mesh)
								{
									num6 = triangle.GetCostFrom((ushort)triangleNode3.ParentId, b);
								}
								else
								{
									num6 = 0.5f;
								}
								if (num4 == num3 & triangleNode3.Mesh == maxValue)
								{
									float num7;
									Vector3.Distance(ref vector5, ref vector2, out num7);
									num6 += num7;
								}
								num6 += triangleNode3.Cost;
								float num8;
								if (!this.mCostList.TryGetValue(key, out num8) | num8 > num6)
								{
									this.mCostList[key] = num6;
									TriangleNode triangleNode4;
									triangleNode4.Mesh = triangleNode3.Mesh;
									triangleNode4.Id = num4;
									triangleNode4.ParentId = triangleNode3.LongId;
									triangleNode4.Cost = num6;
									triangleNode4.TotalCost = num6 + this.ManhattanDistance(ref vector5, ref vector2);
									this.mOpenList.Push(triangleNode4);
									if (triangleNode4.TotalCost < totalCost)
									{
										totalCost = triangleNode4.TotalCost;
										triangleNode2 = triangleNode4;
									}
								}
							}
						}
						IL_463:;
					}
					if ((triangle.Properties & MovementProperties.Dynamic) == MovementProperties.Dynamic)
					{
						List<NeighbourInfo> list;
						if (triangleNode3.Mesh == 65535)
						{
							list = this.mDynamicNeighbours[triangleNode3.Id];
						}
						else
						{
							list = this.mAnimatedParts[(int)triangleNode3.Mesh].DynamicNeighbours[triangleNode3.Id];
						}
						byte b2 = 0;
						while ((int)b2 < list.Count)
						{
							NeighbourInfo neighbourInfo = list[(int)b2];
							if (neighbourInfo.Mesh == 65535)
							{
								triangles = this.mTriangles;
							}
							else
							{
								triangles = this.mAnimatedParts[(int)neighbourInfo.Mesh].Triangles;
							}
							ushort triangle2 = neighbourInfo.Triangle;
							uint key2 = (uint)((int)neighbourInfo.Mesh << 16 | (int)triangle2);
							if (!(triangle2 == 65535 | this.mClosedList.ContainsKey(key2)))
							{
								MovementProperties properties2 = triangles[(int)triangle2].Properties;
								if (!(properties2 != MovementProperties.Default & (properties2 & iMoveAbilities) == MovementProperties.Default))
								{
									float num9;
									if ((triangleNode3.ParentId & 65535U) == 65535U)
									{
										Vector3.Distance(ref vector, ref neighbourInfo.EdgePos, out num9);
									}
									else
									{
										num9 = neighbourInfo.Cost;
									}
									if (triangle2 == num3 & neighbourInfo.Mesh == maxValue)
									{
										float num10;
										Vector3.Distance(ref neighbourInfo.EdgePos, ref vector2, out num10);
										num9 += num10;
									}
									num9 += neighbourInfo.Cost;
									float num11;
									if (!this.mCostList.TryGetValue(key2, out num11) | num11 > num9)
									{
										this.mCostList[key2] = num9;
										TriangleNode triangleNode5;
										triangleNode5.Mesh = neighbourInfo.Mesh;
										triangleNode5.Id = triangle2;
										triangleNode5.ParentId = triangleNode3.LongId;
										triangleNode5.Cost = num9;
										triangleNode5.TotalCost = num9 + this.ManhattanDistance(ref neighbourInfo.EdgePos, ref vector2);
										this.mOpenList.Push(triangleNode5);
										if (triangleNode5.TotalCost < totalCost)
										{
											totalCost = triangleNode5.TotalCost;
											triangleNode2 = triangleNode5;
										}
									}
								}
							}
							b2 += 1;
						}
					}
				}
			}
			uint num12 = triangleNode2.LongId;
			while ((ushort)num12 != 65535)
			{
				iPath.Add(num12);
				num12 = this.mClosedList[num12].ParentId;
			}
			return result;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x000174D9 File Offset: 0x000156D9
		public float ManhattanDistance(ref Vector3 startPos, ref Vector3 endPos)
		{
			return Math.Abs(startPos.X - endPos.X) + Math.Abs(startPos.Z - endPos.Z) * 1.75f;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00017508 File Offset: 0x00015708
		public float TriArea2D(ref Vector3 iA, ref Vector3 iB, ref Vector3 iC)
		{
			return (iB.X * iA.Z - iA.X * iB.Z + (iC.X * iB.Z - iB.X * iC.Z) + (iA.X * iC.Z - iC.X * iA.Z)) * 0.5f;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00017570 File Offset: 0x00015770
		public void StraightenPath(ref Vector3 iStartPos, ref Vector3 iEndPos, List<uint> iTrianglePath, List<PathNode> iStraightPath, MovementProperties iMoveAbilities)
		{
			iStraightPath.Clear();
			if (iTrianglePath.Count == 0 || iTrianglePath[0] == 65535U)
			{
				return;
			}
			uint num = this.mTrianglePath[iTrianglePath.Count - 1];
			ushort num2 = (ushort)(num >> 16);
			ushort num3 = (ushort)num;
			Triangle[] triangles;
			Vector3[] vertices;
			if (num2 == 65535)
			{
				triangles = this.mTriangles;
				vertices = this.mVertices;
			}
			else
			{
				triangles = this.mAnimatedParts[(int)num2].Triangles;
				vertices = this.mAnimatedParts[(int)num2].Vertices;
			}
			Vector3 vector = iStartPos;
			if (num2 != 65535)
			{
				Vector3.Transform(ref vector, ref this.mAnimatedParts[(int)num2].mInvTransform, out vector);
			}
			Triangle triangle = new Triangle(ref vertices[(int)triangles[(int)num3].VertexA], ref vertices[(int)triangles[(int)num3].VertexB], ref vertices[(int)triangles[(int)num3].VertexC]);
			float t;
			float t2;
			Distance.PointTriangleDistanceSq(out t, out t2, ref vector, ref triangle);
			triangle.GetPoint(t, t2, out vector);
			if (num2 != 65535)
			{
				Vector3.Transform(ref vector, ref this.mAnimatedParts[(int)num2].mTransform, out vector);
			}
			uint num4 = this.mTrianglePath[0];
			ushort num5 = (ushort)(num4 >> 16);
			ushort num6 = (ushort)num4;
			if (num5 == 65535)
			{
				triangles = this.mTriangles;
				vertices = this.mVertices;
			}
			else
			{
				triangles = this.mAnimatedParts[(int)num5].Triangles;
				vertices = this.mAnimatedParts[(int)num5].Vertices;
			}
			Vector3 vector2 = iEndPos;
			if (num5 != 65535)
			{
				Vector3.Transform(ref vector2, ref this.mAnimatedParts[(int)num5].mInvTransform, out vector2);
			}
			Triangle triangle2 = new Triangle(ref vertices[(int)triangles[(int)num6].VertexA], ref vertices[(int)triangles[(int)num6].VertexB], ref vertices[(int)triangles[(int)num6].VertexC]);
			Distance.PointTriangleDistanceSq(out t, out t2, ref vector2, ref triangle2);
			triangle2.GetPoint(t, t2, out vector2);
			if (num5 != 65535)
			{
				Vector3.Transform(ref vector2, ref this.mAnimatedParts[(int)num5].mTransform, out vector2);
			}
			this.mPathSmoothingTriangleList.Clear();
			PathNode item = default(PathNode);
			if (num2 == 65535)
			{
				triangles = this.mTriangles;
				vertices = this.mVertices;
			}
			else
			{
				triangles = this.mAnimatedParts[(int)num2].Triangles;
				vertices = this.mAnimatedParts[(int)num2].Vertices;
			}
			item.Position = vector;
			item.Properties = triangles[(int)num3].Properties;
			iStraightPath.Add(item);
			this.mPathSmoothingTriangleList.Add(iTrianglePath.Count - 1);
			Vector3 vector3 = vector;
			Vector3 vector4 = vector3;
			Vector3 vector5 = vector3;
			int num7 = 0;
			int num8 = 0;
			int i = iTrianglePath.Count - 1;
			while (i >= 0)
			{
				uint num9 = iTrianglePath[i];
				ushort num10 = (ushort)(num9 >> 16);
				ushort num11 = (ushort)num9;
				if (num10 == 65535)
				{
					triangles = this.mTriangles;
					vertices = this.mVertices;
				}
				else
				{
					triangles = this.mAnimatedParts[(int)num10].Triangles;
					vertices = this.mAnimatedParts[(int)num10].Vertices;
				}
				Vector3 vector6;
				Vector3 vector7;
				if (i > 0)
				{
					if (num10 == (ushort)(iTrianglePath[i - 1] >> 16))
					{
						triangles[(int)num11].GetPortalPoints(vertices, (ushort)iTrianglePath[i - 1], out vector6, out vector7);
					}
					else
					{
						Vector3.Add(ref vertices[(int)triangles[(int)num11].VertexA], ref vertices[(int)triangles[(int)num11].VertexB], out vector6);
						Vector3.Add(ref vertices[(int)triangles[(int)num11].VertexC], ref vector6, out vector6);
						Vector3.Multiply(ref vector6, 0.33333334f, out vector6);
						vector7 = vector6;
					}
					if (num10 != 65535)
					{
						Vector3.Transform(ref vector6, ref this.mAnimatedParts[(int)num10].mTransform, out vector6);
						Vector3.Transform(ref vector7, ref this.mAnimatedParts[(int)num10].mTransform, out vector7);
					}
				}
				else
				{
					vector6 = vector2;
					vector7 = vector2;
				}
				float num12;
				Vector3.DistanceSquared(ref vector3, ref vector5, out num12);
				if (num12 < 0.001f)
				{
					vector5 = vector7;
					num8 = i;
					goto IL_52B;
				}
				if (this.TriArea2D(ref vector3, ref vector5, ref vector7) > 0f)
				{
					goto IL_52B;
				}
				if (this.TriArea2D(ref vector3, ref vector4, ref vector7) > 0f)
				{
					vector5 = vector7;
					num8 = i;
					goto IL_52B;
				}
				vector3 = vector4;
				int num13 = num7;
				Vector3 position = iStraightPath[iStraightPath.Count - 1].Position;
				Vector3.DistanceSquared(ref position, ref vector3, out num12);
				if (num12 >= 0.001f)
				{
					uint num14 = iTrianglePath[num13];
					ushort num15 = (ushort)(num14 >> 16);
					ushort num16 = (ushort)num14;
					if (num15 == 65535)
					{
						triangles = this.mTriangles;
					}
					else
					{
						triangles = this.mAnimatedParts[(int)num15].Triangles;
					}
					item.Position = vector3;
					item.Properties = triangles[(int)num16].Properties;
					iStraightPath.Add(item);
					this.mPathSmoothingTriangleList.Add(i);
				}
				vector4 = vector3;
				vector5 = vector3;
				num7 = num13;
				num8 = num13;
				i = num13;
				IL_638:
				i--;
				continue;
				IL_52B:
				Vector3.DistanceSquared(ref vector3, ref vector4, out num12);
				if (num12 < 0.001f)
				{
					vector4 = vector6;
					num7 = i;
					goto IL_638;
				}
				if (this.TriArea2D(ref vector3, ref vector4, ref vector6) < 0f)
				{
					goto IL_638;
				}
				if (this.TriArea2D(ref vector3, ref vector5, ref vector6) < 0f)
				{
					vector4 = vector6;
					num7 = i;
					goto IL_638;
				}
				vector3 = vector5;
				num13 = num8;
				Vector3 position2 = iStraightPath[iStraightPath.Count - 1].Position;
				Vector3.DistanceSquared(ref position2, ref vector3, out num12);
				if (num12 >= 0.001f)
				{
					uint num17 = iTrianglePath[num13];
					ushort num18 = (ushort)(num17 >> 16);
					ushort num19 = (ushort)num17;
					if (num18 == 65535)
					{
						triangles = this.mTriangles;
					}
					else
					{
						triangles = this.mAnimatedParts[(int)num18].Triangles;
					}
					item.Position = vector3;
					item.Properties = triangles[(int)num19].Properties;
					iStraightPath.Add(item);
					this.mPathSmoothingTriangleList.Add(i);
				}
				vector4 = vector3;
				vector5 = vector3;
				num7 = num13;
				num8 = num13;
				i = num13;
				goto IL_638;
			}
			if (num5 == 65535)
			{
				triangles = this.mTriangles;
				vertices = this.mVertices;
			}
			else
			{
				triangles = this.mAnimatedParts[(int)num5].Triangles;
				vertices = this.mAnimatedParts[(int)num5].Vertices;
			}
			item.Position = vector2;
			item.Properties = triangles[(int)num6].Properties;
			iStraightPath.Add(item);
			this.mPathSmoothingTriangleList.Add(0);
			for (int j = 0; j < iStraightPath.Count - 1; j++)
			{
				for (int k = this.mPathSmoothingTriangleList[j]; k > this.mPathSmoothingTriangleList[j + 1]; k--)
				{
					uint num20 = iTrianglePath[k];
					ushort num21 = (ushort)(num20 >> 16);
					ushort num22 = (ushort)num20;
					ushort num23 = (ushort)(num20 >> 16);
					ushort num24 = (ushort)num20;
					if (num21 == 65535)
					{
						triangles = this.mTriangles;
						vertices = this.mVertices;
					}
					else
					{
						triangles = this.mAnimatedParts[(int)num21].Triangles;
						vertices = this.mAnimatedParts[(int)num21].Vertices;
					}
					Triangle[] triangles2;
					if (num23 == 65535)
					{
						triangles2 = this.mTriangles;
					}
					else
					{
						triangles2 = this.mAnimatedParts[(int)num23].Triangles;
						Vector3[] vertices2 = this.mAnimatedParts[(int)num23].Vertices;
					}
					if (triangles[(int)num22].Properties != triangles2[(int)num24].Properties)
					{
						Vector3 vector8;
						Vector3 vector9;
						if (num21 == num23)
						{
							triangles[(int)num22].GetPortalPoints(vertices, (ushort)iTrianglePath[k - 1], out vector8, out vector9);
						}
						else
						{
							Vector3.Add(ref vertices[(int)triangles[(int)num22].VertexA], ref vertices[(int)triangles[(int)num22].VertexB], out vector8);
							Vector3.Add(ref vertices[(int)triangles[(int)num22].VertexC], ref vector8, out vector8);
							Vector3.Multiply(ref vector8, 0.33333334f, out vector8);
							vector9 = vector8;
						}
						Vector3 position3 = iStraightPath[j].Position;
						Vector3 position4 = iStraightPath[j + 1].Position;
						Vector3 vector10;
						Vector3.Subtract(ref vector9, ref vector8, out vector10);
						Vector3 vector11;
						Vector3.Subtract(ref position4, ref position3, out vector11);
						Vector3 vector12;
						Vector3.Subtract(ref position3, ref vector8, out vector12);
						float num25 = vector12.X * vector11.Z - vector12.Z * vector11.X;
						float num26 = vector10.X * vector11.Z - vector10.Z * vector11.X;
						float num27 = num25 * num26 / (num26 * num26);
						Vector3.Lerp(ref vector8, ref vector9, num27, out item.Position);
						item.Properties = triangles[(int)num22].Properties;
						Vector3.DistanceSquared(ref position3, ref item.Position, out num27);
						if (num27 > 0.001f)
						{
							Vector3.DistanceSquared(ref position4, ref item.Position, out num27);
						}
						if (num27 > 0.001f)
						{
							iStraightPath.Insert(j + 1, item);
							this.mPathSmoothingTriangleList.Insert(j + 1, k);
							break;
						}
					}
				}
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00017EC6 File Offset: 0x000160C6
		internal NavMeshOctree Octree
		{
			get
			{
				return this.mMesh;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00017ECE File Offset: 0x000160CE
		public Triangle[] Triangles
		{
			get
			{
				return this.mTriangles;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00017ED6 File Offset: 0x000160D6
		public Vector3[] Vertices
		{
			get
			{
				return this.mVertices;
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x00017EDE File Offset: 0x000160DE
		internal List<AnimatedNavMesh> AnimatedParts
		{
			get
			{
				return this.mAnimatedParts;
			}
		}

		// Token: 0x04000300 RID: 768
		private static Vector3 BBBias = new Vector3(0.25f);

		// Token: 0x04000301 RID: 769
		private Vector3[] mVertices;

		// Token: 0x04000302 RID: 770
		private Triangle[] mTriangles;

		// Token: 0x04000303 RID: 771
		private NavMeshOctree mMesh;

		// Token: 0x04000304 RID: 772
		private List<uint> mTrianglePath = new List<uint>(512);

		// Token: 0x04000305 RID: 773
		private List<int> mPathSmoothingTriangleList = new List<int>(512);

		// Token: 0x04000306 RID: 774
		private Dictionary<uint, float> mCostList = new Dictionary<uint, float>();

		// Token: 0x04000307 RID: 775
		private Dictionary<uint, TriangleNode> mClosedList = new Dictionary<uint, TriangleNode>();

		// Token: 0x04000308 RID: 776
		private TriangleHeap mOpenList = new TriangleHeap(512);

		// Token: 0x04000309 RID: 777
		private Dictionary<ushort, List<NeighbourInfo>> mDynamicNeighbours = new Dictionary<ushort, List<NeighbourInfo>>();

		// Token: 0x0400030A RID: 778
		private List<AnimatedNavMesh> mAnimatedParts = new List<AnimatedNavMesh>();

		// Token: 0x0400030B RID: 779
		private LevelModel mLevel;
	}
}
