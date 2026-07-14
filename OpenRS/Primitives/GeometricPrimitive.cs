using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Primitives
{
    public abstract class GeometricPrimitive : IDisposable
    {
        private readonly List<VertexPositionNormal> vertices = [];
        private readonly List<ushort> indices = [];

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private BasicEffect basicEffect;

        protected void AddVertex(Vector3 position, Vector3 normal) => vertices.Add(new VertexPositionNormal(position, normal));

        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            indices.Add((ushort)index);
        }

        protected int CurrentVertex => vertices.Count;

        protected void InitializePrimitive(GraphicsDevice graphicsDevice)
        {
            vertexBuffer = new VertexBuffer(graphicsDevice,
                                            typeof(VertexPositionNormal),
                                            vertices.Count, BufferUsage.None);

            vertexBuffer.SetData(vertices.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort),
                                          indices.Count, BufferUsage.None);

            indexBuffer.SetData(indices.ToArray());

            basicEffect = new BasicEffect(graphicsDevice);

            basicEffect.EnableDefaultLighting();
        }

        ~GeometricPrimitive()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                vertexBuffer?.Dispose();

                indexBuffer?.Dispose();

                basicEffect?.Dispose();
            }
        }

        private static float MaxAlphaChannel => 255.0f;

        public void Draw(Effect effect)
        {
            GraphicsDevice graphicsDevice = effect.GraphicsDevice;

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            graphicsDevice.Indices = indexBuffer;

            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                int primitiveCount = indices.Count / 3;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
            }
        }

        public void Draw(Matrix world, Matrix view, Matrix projection, Color color)
        {
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / MaxAlphaChannel;

            GraphicsDevice device = basicEffect.GraphicsDevice;
            device.DepthStencilState = DepthStencilState.Default;

            if (color.A < 255)
            {
                device.BlendState = BlendState.AlphaBlend;
            }
            else
            {
                device.BlendState = BlendState.Opaque;
            }

            Draw(basicEffect);
        }
    }
}
