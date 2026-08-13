using System.Collections.Generic;

namespace OpenRS.Net.Client.Game
{
    public sealed class ObjectModel(IEnumerable<Vertex> vertices, IEnumerable<Face> faces)
    {
        private static float FullRotationDegrees => 360F;

        private float xRotation;
        private float yRotation;
        private float zRotation;
        private ObjectModelElementCollection<Vertex> vertices = new(vertices, nameof(vertices));
        private ObjectModelElementCollection<Face> faces = new(faces, nameof(faces));

        public IEnumerable<Vertex> Vertices
        {
            get => vertices.Items;
            set => vertices = new(value, nameof(vertices));
        }

        public IEnumerable<Face> Faces
        {
            get => faces.Items;
            set => faces = new(value, nameof(faces));
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
            => faces.Get(index);

        public Face RemoveFace(int index) => faces.Remove(index);

        public void AddVertex(Vertex vertex)
            => vertices.Add(vertex);

        public Vertex GetVertex(int index)
            => vertices.Get(index);

        public Vertex RemoveVertex(int index) => vertices.Remove(index);

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

    }
}
