using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using OpenRS.Net.Client.Game;
using OpenRS.Primitives;

namespace OpenRS
{


    public sealed class ObjectModel
    {

        private List<Vertex> vertices;
        private List<Face> faces;
        private float xRot;
        private float yRot;
        private float zRot;
        private float xScale;
        private float yScale;
        private float zScale;
        private float xTranslate;
        private float yTranslate;
        private float zTranslate;
        private int numTextures;

        public ObjectModel(List<Vertex> vertices, List<Face> faces)
        {
            this.vertices = vertices;// new ClonableStack<Vertex>();
            this.faces = faces;// new Vector();
            // this.faces = faces;
            // this.vertices = vertices;
            xRot = yRot = zRot = 0.0F;
            xScale = yScale = zScale = 1.0F;
            xTranslate = yTranslate = zTranslate = 0.0F;
        }

        public ObjectModel(List<Vertex> vertices, List<Face> faces, List<Vertex> vertices1, List<Face> faces1)
        {
            this.vertices = vertices; //new Vector();
            this.faces = faces;//new Vector();
            //this.faces = faces;
            //this.vertices = vertices;
            xRot = yRot = zRot = 0.0F;
            xScale = yScale = zScale = 1.0F;
            xTranslate = yTranslate = zTranslate = 0.0F;
        }

        public ObjectModel()
        {
            vertices = [];
            faces = [];
            // faces = new Vector();
            // vertices = new Vector();
            xRot = yRot = zRot = 0.0F;
            xScale = yScale = zScale = 1.0F;
            xTranslate = yTranslate = zTranslate = 0.0F;
        }

        public void AddFace(Face face)
        {
            faces.Add(face);
        }

        public Face GetFace(int i)
        {
            return faces[i];
        }

        public Face RemoveFace(int i)
        {
            Face face = faces[i];
            faces.Remove(face);
            return face;
        }

        public List<Face> GetFaces()
        {
            return faces;
        }

        public void AddVert(Vertex vertex)
        {
            vertices.Add(vertex);
        }

        public Vertex RemoveVertex(int i)
        {
            Vertex vertex2 = vertices[i];
            vertices.Remove(vertex2);
            return vertex2;
        }

        public Vertex GetVertex(int i)
        {
            return vertices[i];
        }

        public List<Vertex> GetVertices()
        {
            return vertices;
        }

        public void SetVertices(List<Vertex> vertices)
        {
            this.vertices = vertices;
        }

        public void SetFaces(List<Face> faces)
        {
            this.faces = faces;
        }

        public float GetXRot()
        {
            return xRot;
        }

        public void SetXRot(float xRot)
        {
            if (xRot > 360F)
            {
                xRot -= 360F;
            }
            else
                if (xRot < -360F)
                {
                    xRot += 360F;
                }
            this.xRot = xRot;
        }

        public float GetYRot()
        {
            return yRot;
        }

        public void SetYRot(float yRot)
        {
            if (yRot > 360F)
            {
                yRot -= 360F;
            }
            else
                if (yRot < -360F)
                {
                    yRot += 360F;
                }
            this.yRot = yRot;
        }

        public float GetZRot()
        {
            return zRot;
        }

        public void SetZRot(float zRot)
        {
            if (zRot > 360F)
            {
                zRot -= 360F;
            }
            else
                if (zRot < -360F)
                {
                    zRot += 360F;
                }
            this.zRot = zRot;
        }

        public float GetXScale()
        {
            return xScale;
        }

        public void SetXScale(float xScale)
        {
            this.xScale = xScale;
        }

        public float GetYScale()
        {
            return yScale;
        }

        public void SetYScale(float yScale)
        {
            this.yScale = yScale;
        }

        public float GetZScale()
        {
            return zScale;
        }

        public void SetZScale(float zScale)
        {
            this.zScale = zScale;
        }

        public void SetScale(float scale)
        {
            SetXScale(scale);
            SetYScale(scale);
            SetZScale(scale);
        }

        public float GetXTranslate()
        {
            return xTranslate;
        }

        public void SetXTranslate(float xTranslate)
        {
            this.xTranslate = xTranslate;
        }

        public float GetYTranslate()
        {
            return yTranslate;
        }

        public void SetYTranslate(float yTranslate)
        {
            this.yTranslate = yTranslate;
        }

        public float GetZTranslate()
        {
            return zTranslate;
        }

        public void SetZTranslate(float zTranslate)
        {
            this.zTranslate = zTranslate;
        }

        public void SetNumTextures(int numTextures)
        {
            this.numTextures = numTextures;
        }

        public int GetNumTextures()
        {
            return numTextures;
        }
    }


    public sealed class OB3Model(GraphicsDevice graphicsDevice, GameObject obj) : GeometricPrimitive
    {
    }
}
