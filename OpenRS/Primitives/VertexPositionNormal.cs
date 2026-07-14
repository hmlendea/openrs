using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Primitives
{
    public struct VertexPositionNormal(Vector3 position, Vector3 normal) : IVertexType
    {
        public Vector3 Position = position;
        public Vector3 Normal = normal;

        public static readonly VertexDeclaration VertexDeclaration = new(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
    }
}
