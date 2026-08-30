/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;

namespace Gibbed.Dunia2.FileFormats
{
    public struct Matrix4 : ICloneable
    {
        public Vector4 MatrixTop;
        public Vector4 MatrixMiddle;
        public Vector4 MatrixBottom;
        public Vector4 Position;

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                                 MatrixTop.X,
                                 MatrixTop.Y,
                                 MatrixTop.Z,
                                 MatrixTop.W,
                                 MatrixMiddle.X,
                                 MatrixMiddle.Y,
                                 MatrixMiddle.Z,
                                 MatrixBottom.X,
                                 MatrixBottom.Y,
                                 MatrixBottom.Z,
                                 Position.X,
                                 Position.Y,
                                 Position.Z
                                 );
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return (Matrix4)obj == this;
        }

        public static bool operator !=(Matrix4 a, Matrix4 b)
        {
            return Equals(a.MatrixTop, b.MatrixTop) == false ||
                   Equals(a.MatrixMiddle, b.MatrixMiddle) == false ||
                   Equals(a.MatrixBottom, b.MatrixBottom) == false ||
                   Equals(a.Position, b.Position) == false;
        }

        public static bool operator ==(Matrix4 a, Matrix4 b)
        {
            return Equals(a.MatrixTop, b.MatrixTop) == true &&
                   Equals(a.MatrixMiddle, b.MatrixMiddle) == true &&
                   Equals(a.MatrixBottom, b.MatrixBottom) == true &&
                   Equals(a.Position, b.Position) == true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + MatrixTop.GetHashCode();
                hash = hash * 23 + MatrixMiddle.GetHashCode();
                hash = hash * 23 + MatrixBottom.GetHashCode();
                hash = hash * 23 + Position.GetHashCode();
                return hash;
            }
        }

        public object Clone()
        {
            return new Matrix4()
            {
                MatrixTop = this.MatrixTop,
                MatrixMiddle= this.MatrixMiddle,
                MatrixBottom = this.MatrixBottom,
                Position = this.Position
            };
        }
    }
}
