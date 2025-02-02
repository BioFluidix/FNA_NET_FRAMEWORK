#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2023 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */

/* Derived from code by the Mono.Xna Team (Copyright 2006).
 * Released under the MIT License. See monoxna.LICENSE for details.
 */
#endregion

#region Using Statements

using System;
using System.Collections.Generic;

#endregion

namespace Microsoft.Xna.Framework
{
	public struct BoundingBox : IEquatable<BoundingBox>
	{
		#region Internal Properties

		internal string DebugDisplayString
		{
			get
			{
				return string.Concat(
					"Min( ", Min.DebugDisplayString, " ) \r\n",
					"Max( ", Max.DebugDisplayString, " )"
				);
			}
		}

		#endregion

		#region Public Fields

		public Vector3 Min;

		public Vector3 Max;

		public const int CornerCount = 8;

		#endregion

		#region Private Static Variables

		// These are NOT readonly, for weird performance reasons -flibit
		private static Vector3 MaxVector3 = new Vector3(float.MaxValue);
		private static Vector3 MinVector3 = new Vector3(float.MinValue);

		#endregion

		#region Public Constructors

		public BoundingBox(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		#endregion

		#region Public Methods


		public ContainmentType Contains(Vector3 point)
		{
			ContainmentType result;
			this.Contains(ref point, out result);
			return result;
		}

		public ContainmentType Contains(BoundingBox box)
		{
			// Test if all corner is in the same side of a face by just checking min and max
			if (	box.Max.X < Min.X ||
				box.Min.X > Max.X ||
				box.Max.Y < Min.Y ||
				box.Min.Y > Max.Y ||
				box.Max.Z < Min.Z ||
				box.Min.Z > Max.Z	)
			{
				return ContainmentType.Disjoint;
			}


			if (	box.Min.X >= Min.X &&
				box.Max.X <= Max.X &&
				box.Min.Y >= Min.Y &&
				box.Max.Y <= Max.Y &&
				box.Min.Z >= Min.Z &&
				box.Max.Z <= Max.Z	)
			{
				return ContainmentType.Contains;
			}

			return ContainmentType.Intersects;
		}


		public void Contains(ref Vector3 point, out ContainmentType result)
		{
			// Determine if point is outside of this box.
			if (	point.X < this.Min.X ||
				point.X > this.Max.X ||
				point.Y < this.Min.Y ||
				point.Y > this.Max.Y ||
				point.Z < this.Min.Z ||
				point.Z > this.Max.Z	)
			{
				result = ContainmentType.Disjoint;
			}
			else
			{
				result = ContainmentType.Contains;
			}
		}

		public Vector3[] GetCorners()
		{
			return new Vector3[] {
				new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
				new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
				new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
				new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
				new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
				new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
				new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
				new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
			};
		}

		public void GetCorners(Vector3[] corners)
		{
			if (corners == null)
			{
				throw new ArgumentNullException("corners");
			}
			if (corners.Length < 8)
			{
				throw new ArgumentOutOfRangeException("corners", "Not Enought Corners");
			}
			corners[0].X = this.Min.X;
			corners[0].Y = this.Max.Y;
			corners[0].Z = this.Max.Z;
			corners[1].X = this.Max.X;
			corners[1].Y = this.Max.Y;
			corners[1].Z = this.Max.Z;
			corners[2].X = this.Max.X;
			corners[2].Y = this.Min.Y;
			corners[2].Z = this.Max.Z;
			corners[3].X = this.Min.X;
			corners[3].Y = this.Min.Y;
			corners[3].Z = this.Max.Z;
			corners[4].X = this.Min.X;
			corners[4].Y = this.Max.Y;
			corners[4].Z = this.Min.Z;
			corners[5].X = this.Max.X;
			corners[5].Y = this.Max.Y;
			corners[5].Z = this.Min.Z;
			corners[6].X = this.Max.X;
			corners[6].Y = this.Min.Y;
			corners[6].Z = this.Min.Z;
			corners[7].X = this.Min.X;
			corners[7].Y = this.Min.Y;
			corners[7].Z = this.Min.Z;
		}


		public bool Intersects(BoundingBox box)
		{
			bool result;
			Intersects(ref box, out result);
			return result;
		}

		public void Intersects(ref BoundingBox box, out bool result)
		{
			if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
			{
				if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
				{
					result = false;
					return;
				}

				result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
				return;
			}

			result = false;
			return;
		}


		public bool Equals(BoundingBox other)
		{
			return (this.Min == other.Min) && (this.Max == other.Max);
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Create a bounding box from the given list of points.
		/// </summary>
		/// <param name="points">
		/// The list of Vector3 instances defining the point cloud to bound.
		/// </param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown if the given list has no points.
		/// </exception>
		public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}

			bool empty = true;
			Vector3 minVec = MaxVector3;
			Vector3 maxVec = MinVector3;
			foreach (Vector3 ptVector in points)
			{
				minVec.X = (minVec.X < ptVector.X) ? minVec.X : ptVector.X;
				minVec.Y = (minVec.Y < ptVector.Y) ? minVec.Y : ptVector.Y;
				minVec.Z = (minVec.Z < ptVector.Z) ? minVec.Z : ptVector.Z;

				maxVec.X = (maxVec.X > ptVector.X) ? maxVec.X : ptVector.X;
				maxVec.Y = (maxVec.Y > ptVector.Y) ? maxVec.Y : ptVector.Y;
				maxVec.Z = (maxVec.Z > ptVector.Z) ? maxVec.Z : ptVector.Z;

				empty = false;
			}
			if (empty)
			{
				throw new ArgumentException("Collection is empty", "points");
			}

			return new BoundingBox(minVec, maxVec);
		}


		public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
		{
			BoundingBox result;
			CreateMerged(ref original, ref additional, out result);
			return result;
		}

		public static void CreateMerged(ref BoundingBox original, ref BoundingBox additional, out BoundingBox result)
		{
			result.Min.X = Math.Min(original.Min.X, additional.Min.X);
			result.Min.Y = Math.Min(original.Min.Y, additional.Min.Y);
			result.Min.Z = Math.Min(original.Min.Z, additional.Min.Z);
			result.Max.X = Math.Max(original.Max.X, additional.Max.X);
			result.Max.Y = Math.Max(original.Max.Y, additional.Max.Y);
			result.Max.Z = Math.Max(original.Max.Z, additional.Max.Z);
		}

		#endregion

		#region Public Static Operators and Override Methods

		public override bool Equals(object obj)
		{
			return (obj is BoundingBox) && Equals((BoundingBox) obj);
		}

		public override int GetHashCode()
		{
			return this.Min.GetHashCode() + this.Max.GetHashCode();
		}

		public static bool operator ==(BoundingBox a, BoundingBox b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BoundingBox a, BoundingBox b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return (
				"{{Min:" + Min.ToString() +
				" Max:" + Max.ToString() +
				"}}"
			);
		}

		#endregion
	}
}
