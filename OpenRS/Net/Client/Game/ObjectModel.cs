using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRS.Net.Client.Game
{
    public sealed class ObjectModel(IEnumerable<Vertex> vertices, IEnumerable<Face> faces)
    {
        private static float FullRotationDegrees => 360F;

        private float xRotation;
        private float yRotation;
        private float zRotation;
        private List<Vertex> vertices = CreateVertexList(vertices);
        private List<Face> faces = CreateFaceList(faces);

        public IEnumerable<Vertex> Vertices
        {
            get => vertices;
            set => vertices = CreateVertexList(value);
        }

        public IEnumerable<Face> Faces
        {
            get => faces;
            set => faces = CreateFaceList(value);
        }

        public float XRotation
        {
            get => xRotation;
            set => xRotation = ClampRotation(value);
        }

        public float YRotation
        {
            get => yRotation;
            set => yRotation = ClampRotation(value);
        }

        public float ZRotation
        {
            get => zRotation;
            set => zRotation = ClampRotation(value);
        }

        public float XScale { get; set; } = 1.0F;

        public float YScale { get; set; } = 1.0F;

        public float ZScale { get; set; } = 1.0F;

        public float XTranslation { get; set; }

        public float YTranslation { get; set; }

        public float ZTranslation { get; set; }

        public int TextureCount { get; set; }

        public ObjectModel()
            : this([], [])
        {
        }

        public void AddFace(Face face)
            => faces.Add(face);

        public Face GetFace(int index)
            => faces[index];

        public Face RemoveFace(int index)
        {
            Face face = faces[index];
            faces.Remove(face);

            return face;
        }

        public void AddVertex(Vertex vertex)
            => vertices.Add(vertex);

        public Vertex GetVertex(int index)
            => vertices[index];

        public Vertex RemoveVertex(int index)
        {
            Vertex vertex = vertices[index];
            vertices.Remove(vertex);

            return vertex;
        }

        public void SetScale(float scale)
        {
            XScale = scale;
            YScale = scale;
            ZScale = scale;
        }

        private static float ClampRotation(float rotation)
        {
            if (rotation > FullRotationDegrees)
            {
                return rotation - FullRotationDegrees;
            }

            if (rotation < -FullRotationDegrees)
            {
                return rotation + FullRotationDegrees;
            }

            return rotation;
        }

        private static List<Face> CreateFaceList(IEnumerable<Face> faces)
            => ValidateCollection(faces, nameof(faces)).ToList();

        private static List<Vertex> CreateVertexList(IEnumerable<Vertex> vertices)
            => ValidateCollection(vertices, nameof(vertices)).ToList();

        private static IEnumerable<TItem> ValidateCollection<TItem>(
            IEnumerable<TItem> items,
            string parameterName)
        {
            if (items is null)
            {
                throw new ArgumentNullException(
                    parameterName,
                    $"The {parameterName} collection cannot be null.");
            }

            return items;
        }
    }
}
